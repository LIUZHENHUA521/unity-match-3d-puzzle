import UnityPy,json
from pathlib import Path
manifest=json.loads(Path('outputs/goose-analysis/missing-goose-assets.json').read_text());names={r['name'] for r in manifest};bundles=sorted({r['bundle'] for r in manifest})
source=Path('work/export_source_colliders.py').read_text();source=source[:source.index('out=Path(')].replace("UnityPy.load('work/model-bundles')",'UnityPy.load(*bundles)')
exec(source)
rows=[r for r in rows if r['name'] in names];assert len(rows)==12,[(r['name']) for r in rows]
assert all(r['col'] is not None and r['realCol'] is not None for r in rows)
root=Path('/Users/liuzhenhua/Desktop/code/unity-match-3d-puzzle');dst=root/'Assets/GooseReplica/Resources/Replica/SourceColliders.json';data=json.loads(dst.read_text());existing={r['name'] for r in data['items']};new=[r for r in rows if r['name'] not in existing]
for r in rows:
 if r['name'] in existing:assert next(v for v in data['items'] if v['name']==r['name'])==r
n=len(data['items']);data['items'].extend(new);dst.write_text(json.dumps(data,ensure_ascii=False,separators=(',',':')))
out=Path('outputs/goose-analysis/physics-audit');(out/'goose-collider-profiles-ios.json').write_text(json.dumps({'source':'iOS1.11.03 IPA; precise prefab ObjectUnit col/realCol references; 12 Goose missing from earlier export','items':rows},ensure_ascii=False,indent=2))
bodies=[]
for path,ptr in E.container.items():
 g=ptr.deref()
 if g.type.name!='GameObject':continue
 d=g.read_typetree()
 if d['m_Name'] not in names:continue
 for p in d['m_Component']:
  c=g.assets_file.objects[p['component']['m_PathID']]
  if c.type.name=='Rigidbody':bodies.append({'name':d['m_Name'],'body':c.read_typetree()})
(out/'goose-rigidbodies-ios.json').write_text(json.dumps(bodies,ensure_ascii=False,indent=2))
print('appended',len(new),'profiles',n,'->',len(data['items']));print('bodies',len(bodies));print([(r['name'],r['col'],r['realCol'],sum(len(n['colliders']) for n in r['nodes'])) for r in rows]);print('body-settings',[(b['name'],b['body']['m_Mass'],b['body']['m_Drag'],b['body']['m_AngularDrag'],b['body']['m_CollisionDetection']) for b in bodies])
