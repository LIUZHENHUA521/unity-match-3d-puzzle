"""Offline conversion of the eight static, uncompressed GLBs supplied by goose-catch.
Usage: python3 Tools/convert_food_models.py /path/to/goose-catch/src/assets
Emits editor-only mesh JSON and the embedded base-color maps; Unity builds native assets.
"""
import json, struct, sys
from pathlib import Path
source = Path(sys.argv[1])
out = Path(__file__).resolve().parents[1] / 'Assets/FoodPile/Editor/Source'
out.mkdir(parents=True, exist_ok=True)
names = ['cheese','ice-cream','sandwich','coockie-man','toast','pancake-big','sandwich-toast','hotdog']
for name in names:
    blob = (source / (name+'.glb')).read_bytes()
    magic, version, length = struct.unpack_from('<III',blob)
    assert magic == 0x46546c67 and version == 2 and length == len(blob)
    size, kind = struct.unpack_from('<II',blob,12)
    gltf = json.loads(blob[20:20+size]); start = 20+size
    bsize, bkind = struct.unpack_from('<II',blob,start); data = blob[start+8:start+8+bsize]
    assert len(gltf['nodes']) == 1 and not any(k in gltf['nodes'][0] for k in ['matrix','translation','rotation','scale'])
    def read(index):
        a=gltf['accessors'][index]; v=gltf['bufferViews'][a['bufferView']]
        assert 'sparse' not in a and not a.get('normalized')
        fmt={5126:'f',5125:'I',5123:'H',5121:'B'}[a['componentType']]
        count={'VEC3':3,'VEC2':2,'SCALAR':1}[a['type']]
        width=struct.calcsize('<'+fmt*count); stride=v.get('byteStride',width)
        offset=v.get('byteOffset',0)+a.get('byteOffset',0)
        return [struct.unpack_from('<'+fmt*count,data,offset+i*stride) for i in range(a['count'])]
    primitives=gltf['meshes'][0]['primitives']; assert len(primitives)==1
    p=primitives[0]; assert p.get('mode',4)==4
    vertices=read(p['attributes']['POSITION']); normals=read(p['attributes']['NORMAL']); uv=read(p['attributes']['TEXCOORD_0']); idx=[v[0] for v in read(p['indices'])]
    low=[min(v[i] for v in vertices) for i in range(3)]; high=[max(v[i] for v in vertices) for i in range(3)]
    center=[(a+b)/2 for a,b in zip(low,high)]; factor=.46/max(b-a for a,b in zip(low,high))
    # glTF right-handed -> Unity left-handed. Flip winding with reflected Z, and texture V.
    vert=[{'x':(v[0]-center[0])*factor,'y':(v[1]-center[1])*factor,'z':-(v[2]-center[2])*factor} for v in vertices]
    norm=[{'x':v[0],'y':v[1],'z':-v[2]} for v in normals]
    tris=[j for i in range(0,len(idx),3) for j in (idx[i],idx[i+2],idx[i+1])]
    mat=gltf['materials'][p.get('material',0)]; ti=mat['pbrMetallicRoughness']['baseColorTexture']['index']; image=gltf['images'][gltf['textures'][ti]['source']]; view=gltf['bufferViews'][image['bufferView']]
    texture=image['name']+'.png'; (out/texture).write_bytes(data[view.get('byteOffset',0):view.get('byteOffset',0)+view['byteLength']])
    mesh={'name':name,'texture':texture,'vertices':vert,'normals':norm,'uv':[{'x':u,'y':1-v} for u,v in uv],'triangles':tris}
    (out/(name+'.json')).write_text(json.dumps(mesh,separators=(',',':')))
    print(name, len(vertices), 'vertices', len(tris)//3, 'triangles')
