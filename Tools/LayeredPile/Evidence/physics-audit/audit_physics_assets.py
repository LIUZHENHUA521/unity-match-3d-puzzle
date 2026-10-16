import UnityPy,json,re,math
from pathlib import Path
root=Path('/Users/liuzhenhua/Desktop/code/unity-match-3d-puzzle')
data=json.loads((root/'Assets/GooseReplica/OriginalModels/models.json').read_text());models={m['name']:m for m in data['models']}
source_sizes={x['name']:x['normalizedToSource'] for x in json.loads((root/'Assets/GooseReplica/Resources/Replica/SourceSizes.json').read_text())['items']}
report={'source':'WX2.1.12 saved bundles vs iOS1.11.03 exported models/current project resources','transformComparisons':[],'scaleRecovery':[],'environmentComparisons':[]}
for bundle in ['country','grocery','barbecue']:
 env=UnityPy.load('work/wechat-'+bundle+'-objects.bundle')
 for path,ptr in env.container.items():
  go=ptr.deref()
  if go.type.name!='GameObject':continue
  name=go.read_typetree()['m_Name']
  if name not in models:continue
  changes=[];count=[0]
  def walk(go,node,path):
   d=go.read_typetree();f=go.assets_file.objects;t=next(f[c['component']['m_PathID']].read_typetree() for c in d['m_Component'] if f[c['component']['m_PathID']].type.name=='Transform');count[0]+=1
   for k,v in [('name',d['m_Name']),('position',t['m_LocalPosition']),('rotation',t['m_LocalRotation']),('scale',t['m_LocalScale']),('active',bool(d['m_IsActive']))]:
    if node[k]!=v:changes.append({'path':path,'field':k,'wx':v,'ios':node[k]})
   if len(t['m_Children'])!=len(node['children']):changes.append({'path':path,'field':'childCount'});return
   for i,(p,n) in enumerate(zip(t['m_Children'],node['children'])):
    tr=f[p['m_PathID']];walk(f[tr.read_typetree()['m_GameObject']['m_PathID']],n,path+[i])
  walk(go,models[name]['root'],[]);report['transformComparisons'].append({'name':name,'nodes':count[0],'differences':changes})
for name,m in models.items():
 prefab=(root/f"Assets/GooseReplica/OriginalModels/Prefabs/{m['group']}/{name}.prefab").read_text()
 # importer wrapper has scale 1; child visual is the transform with father != 0 whose GameObject is source root.
 scales=re.findall(r'm_LocalScale: \{x: ([^,]+), y: ([^,]+), z: ([^}]+)\}',prefab)
 # source visual root may be serialized before wrapper, identify its Transform by source root name GameObject.
 objects={int(id):body for id,body in re.findall(r'--- !u!\d+ &(\d+)\n(.*?)(?=\n--- !u!|\Z)',prefab,re.S)}
 transforms=[]
 for oid,body in objects.items():
  if body.startswith('Transform:') and 'm_Father: {fileID: 0}' in body:wrapper=oid
 for oid,body in objects.items():
  if body.startswith('Transform:') and 'm_Father: {fileID: '+str(wrapper)+'}' in body:
   scale=list(map(float,re.search(r'm_LocalScale: \{x: ([^,]+), y: ([^,]+), z: ([^}]+)\}',body).groups()));break
 expected=list(m['root']['scale'].values());factor=source_sizes[name];recovered=[x*factor for x in scale]
 report['scaleRecovery'].append({'name':name,'source':expected,'prefabVisual':scale,'factor':factor,'recovered':recovered,'maxError':max(abs(a-b) for a,b in zip(expected,recovered))})
for name in ['CountrysideStove','GroceryStore','Barbecue']:
 src=json.loads(Path(f'outputs/goose-analysis/physics-theme-evidence/{name}.json').read_text());current=json.loads((root/f'Assets/GooseReplica/Resources/Replica/Environments/{name}.json').read_text());report['environmentComparisons'].append({'name':name,'identical':src==current,'nodes':len(src['nodes']),'colliders':sum(len(n['boxes']) for n in src['nodes'])})
out=Path('outputs/goose-analysis/physics-audit/assets-comparison.json');out.write_text(json.dumps(report,ensure_ascii=False,indent=2))
print('transform differences',[(r['name'],r['differences']) for r in report['transformComparisons'] if r['differences']]);print('scales',len(report['scaleRecovery']),'max error',max(r['maxError'] for r in report['scaleRecovery']));print('environments',report['environmentComparisons'])
