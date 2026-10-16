from pathlib import Path
import UnityPy,json
project=Path('/Users/liuzhenhua/Desktop/code/unity-match-3d-puzzle')
m=json.load(open(project/'Assets/GooseReplica/OriginalModels/models.json'))['models']; sizes={i['name']:i['normalizedToSource'] for i in json.load(open(project/'Assets/GooseReplica/Resources/Replica/SourceSizes.json'))['items']}
e=UnityPy.load('work/model-bundles','work/audit-goose-bundles'); objs={p.lower():o for p,o in e.container.items()};rows=[]
for model in m:
 d=objs[model['source'].lower()].read()
 for c in d.m_Component:
  o=c.component.read().object_reader
  if o.type.name!='MonoBehaviour':continue
  t=o.read_typetree()
  if '_pickScale' in t:
   rows.append({'name':model['name'],'pickScale':t['_pickScale'],'normalizedToPickScale':t['_pickScale']*sizes[model['name']]/model['root']['scale']['x']});break
 else:raise ValueError(model['name'])
out=project/'Assets/GooseReplica/Resources/Replica/SourceFlightItems.json';out.write_text(json.dumps({'items':rows},indent=2));print('exported',len(rows))
