"""Decode purchased static GLBs into deterministic Unity mesh inputs.

Usage: python3 Tools/prepare_pot_food_packs.py /path/to/downloads
Source GLBs are preserved byte-for-byte outside Assets; runtime uses native meshes.
"""
import hashlib
import json
from pathlib import Path
import shutil
import struct
import sys

ROOT = Path(__file__).resolve().parents[1]
PACKS = [('vegetable', '田园菜篮'), ('meat', '肉肉盛宴'), ('grocery', '元气超市'),
         ('fruit', '缤纷果园'), ('sweets', '甜蜜小屋')]
LABELS = [
 '芦笋 甜菜 彩椒 西兰花 卷心菜 胡萝卜 菜花 小番茄 辣椒 玉米 黄瓜 茄子 大蒜 大葱 生菜 蘑菇 洋葱 豌豆荚 土豆 南瓜 萝卜 半颗红洋葱 红薯 番茄 芜菁 西葫芦',
 '培根 烤排骨 汉堡肉饼 奶酪轮 鸡胸肉 鸡腿 鸡翅 烤牛排 火腿 火腿片 羊排 肉丸 带骨肉 烤肉串 意式香肠 猪排 生火腿 普罗沃洛奶酪 生牛排 烤鸡 萨拉米 萨拉米片 三文鱼 香肠 瑞士奶酪 丁骨牛排',
 '法棍 吐司 黄油 麦片 薯片 巧克力 咖啡袋 咖啡罐 饼干 鸡蛋盒 面粉 蜂蜜 果酱 果汁 番茄酱 牛奶 芥末酱 橄榄油 花生酱 大米 罐头汤 意大利面 茶叶 金枪鱼罐头 矿泉水 酸奶',
 '苹果 半颗牛油果 香蕉 蓝莓 腰果樹果 樱桃 半颗椰子 半颗火龙果 无花果 葡萄 半颗猕猴桃 柠檬 芒果 橙子 半颗木瓜 半颗百香果 桃子 梨 菠萝 李子 石榴 树莓 杨桃 草莓 橘子 西瓜片',
 '球形棒棒糖 巧克力球 蛋糕切片 拐杖糖 糖果罐 巧克力蛋 吉拿棒 棉花糖 纸杯蛋糕 甜甜圈 闪电泡芙 糖球机 小熊软糖 冰淇淋甜筒 糖豆 甘草糖 马卡龙 棉花糖串 松饼 冰棒 布丁 夹心饼干 旋涡棒棒糖 蛋糕卷 华夫饼 包装糖果',
]
# Keep the source order stable: indices are persisted in opening layouts/replays.

def single_food_item(model):
    """Extract one intact item from the reviewed multi-item food meshes.

    Weld positions only for connectivity; keep original UV/normal seam vertices.
    Lamb meat and its bone are separate islands, so retain both positive-X islands.
    Original purchased GLBs remain untouched.
    """
    expected = {'ChickenWings': [48, 48, 48], 'LambChops': [41, 41, 66, 66],
                'Meatballs': [38, 38, 38, 38, 38], 'SwissRoll': [65, 78],
                'Licorice': [126, 126], 'JellyBeans': [40] * 6}
    if model['name'] not in expected:
        return model
    vertices = model['vertices']
    parents = list(range(len(vertices)))

    def root(i):
        while parents[i] != i:
            parents[i] = parents[parents[i]]
            i = parents[i]
        return i

    def join(a, b):
        parents[root(a)] = root(b)

    positions = {}
    for i, vertex in enumerate(vertices):
        key = tuple(round(vertex[axis], 5) for axis in 'xyz')
        if key in positions:
            join(i, positions[key])
        positions[key] = i
    triangles = model['triangles']
    for i in range(0, len(triangles), 3):
        join(triangles[i], triangles[i + 1])
        join(triangles[i], triangles[i + 2])
    islands = {}
    for i in range(len(vertices)):
        islands.setdefault(root(i), []).append(i)
    groups = list(islands.values())
    if sorted(map(len, groups)) != expected[model['name']]:
        raise ValueError('Source geometry changed; review single-item selection: ' + model['name'])
    if model['name'] == 'LambChops':
        selected = [g for g in groups if sum(vertices[i]['x'] for i in g) > 0]
        assert sorted(map(len, selected)) == [41, 66], 'Keep one complete chop with its bone'
    elif model['name'] == 'SwissRoll':
        selected = [max(groups, key=len)]  # Whole roll, without the loose slice.
    elif model['name'] == 'Licorice':
        selected = [min(groups, key=lambda g: sum(vertices[i]['x'] for i in g) / len(g))]
    else:
        selected = [min(groups, key=lambda g: sum(vertices[i]['z'] for i in g) / len(g))]
    indices = sorted(i for group in selected for i in group)
    remap = {old: new for new, old in enumerate(indices)}
    offset = {a: (min(vertices[i][a] for i in indices) + max(vertices[i][a] for i in indices)) / 2
              for a in 'xyz'}
    offset['y'] = min(vertices[i]['y'] for i in indices)
    return {**model,
            'vertices': [{a: vertices[i][a] - offset[a] for a in 'xyz'} for i in indices],
            'normals': [model['normals'][i] for i in indices],
            'uv': [model['uv'][i] for i in indices],
            'triangles': [remap[j] for k in range(0, len(triangles), 3)
                          if triangles[k] in remap for j in triangles[k:k + 3]]}

def prepare(source):
    root = ROOT / 'Assets/Match3D/IncomingModels/PotFoodPacks'
    (root / 'Editor').mkdir(parents=True, exist_ok=True)
    (root / 'Textures').mkdir(exist_ok=True)
    originals = ROOT / 'SourceAssets/PotFoodPacks'
    originals.mkdir(parents=True, exist_ok=True)
    manifests = []
    for (pack, title), labels in zip(PACKS, LABELS):
        path = source / f'{pack}_pack.glb'
        blob = path.read_bytes()
        magic, version, length = struct.unpack_from('<III', blob)
        assert (magic, version, length) == (0x46546C67, 2, len(blob))
        size, kind = struct.unpack_from('<II', blob, 12)
        assert kind == 0x4E4F534A
        gltf = json.loads(blob[20:20 + size])
        bsize, bkind = struct.unpack_from('<II', blob, 20 + size)
        assert bkind == 0x004E4942
        data = blob[28 + size:28 + size + bsize]
        assert len(gltf['nodes']) == len(gltf['meshes']) == 26
        assert not gltf.get('animations') and not gltf.get('skins')
        if path.resolve() != (originals / path.name).resolve():
            shutil.copyfile(path, originals / path.name)

        def read(index):
            a = gltf['accessors'][index]
            v = gltf['bufferViews'][a['bufferView']]
            assert 'sparse' not in a and not a.get('normalized')
            fmt = {5126: 'f', 5125: 'I', 5123: 'H', 5121: 'B'}[a['componentType']]
            count = {'VEC3': 3, 'VEC2': 2, 'SCALAR': 1}[a['type']]
            width = struct.calcsize('<' + fmt * count)
            offset = v.get('byteOffset', 0) + a.get('byteOffset', 0)
            return [struct.unpack_from('<' + fmt * count, data, offset + i * v.get('byteStride', width)) for i in range(a['count'])]

        textures = []
        for img in gltf['images']:
            assert img['mimeType'] == 'image/png'
            v = gltf['bufferViews'][img['bufferView']]
            name = img['name'] + '.png'
            (root / 'Textures' / name).write_bytes(data[v.get('byteOffset', 0):v.get('byteOffset', 0) + v['byteLength']])
            textures.append(name)
        mats = []
        for mat in gltf['materials']:
            pbr = mat['pbrMetallicRoughness']
            assert pbr.get('baseColorFactor', [1, 1, 1, 1]) == [1, 1, 1, 1]
            mats.append({'name': mat['name'], 'texture': textures[gltf['textures'][pbr['baseColorTexture']['index']]['source']]})
        models = []
        for node, label in zip(gltf['nodes'], labels.split()):
            # Translation is only the display-grid arrangement, not part of an item.
            assert not any(k in node for k in ('children', 'matrix', 'scale', 'rotation'))
            primitives = gltf['meshes'][node['mesh']]['primitives']
            assert len(primitives) == 1
            p = primitives[0]
            assert p.get('mode', 4) == 4
            vertices = read(p['attributes']['POSITION'])
            normals = read(p['attributes']['NORMAL'])
            uv = read(p['attributes']['TEXCOORD_0'])
            indices = [v[0] for v in read(p['indices'])]
            assert len(indices) % 3 == 0 and max(indices) < len(vertices)
            models.append({'name': node['name'], 'label': label, 'material': p['material'],
                'vertices': [dict(x=x, y=y, z=-z) for x, y, z in vertices],
                'normals': [dict(x=x, y=y, z=-z) for x, y, z in normals],
                'uv': [dict(x=u, y=1-v) for u, v in uv],
                'triangles': [j for i in range(0, len(indices), 3) for j in (indices[i], indices[i+2], indices[i+1])]})
        assert len(models) == 26
        if pack in ('meat', 'sweets'):
            models = [single_food_item(model) for model in models]
        payload = {'id': 'pot-' + pack, 'pack': pack, 'title': title, 'materials': mats, 'models': models}
        (root / 'Editor' / f'pot-{pack}.json').write_text(json.dumps(payload, ensure_ascii=False, separators=(',', ':')))
        manifests.append({'file': path.name, 'sha256': hashlib.sha256(blob).hexdigest(), 'theme': payload['id'], 'title': title,
                          'models': [{'name': m['name'], 'label': m['label'], 'triangles': len(m['triangles']) // 3} for m in models]})
        print(title, len(models), 'models;', sum(len(m['triangles']) // 3 for m in models), 'triangles')
    (originals / 'manifest.json').write_text(json.dumps(manifests, ensure_ascii=False, indent=2) + '\n')

if __name__ == '__main__':
    prepare(Path(sys.argv[1]))
