"""Rebuild the source-prefab size factors without reimporting meshes or changing prefabs."""
from pathlib import Path
import json
import re

project = Path(__file__).resolve().parents[2]
root = project / 'Assets/GooseReplica/OriginalModels'
models = json.loads((root / 'models.json').read_text())['models']
items = []
for model in models:
    prefab = root / 'Prefabs' / model['group'] / (model['name'] + '.prefab')
    if not prefab.exists():
        raise FileNotFoundError(prefab)
    transforms = []
    for document in prefab.read_text().split('--- !u!4 &')[1:]:
        document = document.split('--- !u!')[0]
        ident = int(document.splitlines()[0])
        father = int(re.search(r'm_Father: \{fileID: (\d+)\}', document).group(1))
        scale = re.search(r'm_LocalScale: \{x: ([^,]+), y: ([^,]+), z: ([^}]+)\}', document)
        transforms.append((ident, father, float(scale.group(1))))
    wrapper = next(t[0] for t in transforms if t[1] == 0)
    visual = next(t for t in transforms if t[1] == wrapper)
    items.append({'name': model['name'], 'normalizedToSource': model['root']['scale']['x'] / visual[2]})
output = project / 'Assets/GooseReplica/Resources/Replica/SourceSizes.json'
output.write_text(json.dumps({'items': items}, ensure_ascii=False, indent=2))
print(f'Wrote {len(items)} size factors to {output}')
