from pathlib import Path
import json,UnityPy
schema=Path('work/decode_levelmanager_component.py').read_text();ns={};exec(schema[:schema.index("fmt=take('i')")],ns)
unit_type=next(i for i,t in enumerate(ns['T']) if t['name']=='ObjectUnit')
root=Path('/Users/liuzhenhua/Desktop/code/unity-match-3d-puzzle');ios={x['name']:x for x in json.load(open(root/'Assets/GooseReplica/Resources/Replica/SourceAudioItems.json'))['items']};rows=[];diff=[]
e=UnityPy.load('work/model-bundles','work/audit-goose-bundles');node=None
for o in e.objects:
 if o.type.name=='MonoBehaviour' and '_effectSoundType' in o.read_typetree():node=o.serialized_type.node;break
assert node
wx=UnityPy.load('work/wechat-country-objects.bundle','work/wechat-grocery-objects.bundle','work/wechat-barbecue-objects.bundle')
for path,obj in wx.container.items():
 if obj.type.name!='GameObject':continue
 name=obj.read().m_Name
 if name not in ios:continue
 for c in obj.read().m_Component:
  o=c.component.deref()
  if o.type.name!='MonoBehaviour':continue
  if o.read(check_read=False).m_Script.read().m_ClassName!='ObjectUnit':continue
  b=o.get_raw_data();ns['B']=b;ns['p']=28;ns['string']();vals={}
  for f,dt in ns['fields'](unit_type):
   if f['name'] in ['notBeCatch','isHiding','isStore','_isSelect']:continue
   vals[f['name']]=ns['read'](dt)
  assert ns['p']==len(b),(name,ns['p'],len(b))
  row={'name':name,'select':vals['_effectSoundType'],'usePitch':bool(vals['_soundUsePitch']),'combine':vals['_endSoundType'],'combineDefault':bool(vals['_endSoundCombie']),'goose':bool(vals['_isGoose'])};rows.append(row)
  if row!=ios[name]:diff.append({'wx':row,'ios':ios[name]})
report={'wxObjects':len(rows),'differences':diff,'items':rows};Path('outputs/goose-analysis/audio-evidence/wx-ios-audio-profiles.json').write_text(json.dumps(report,indent=2));print('wx profiles',len(rows),'diff',diff)
