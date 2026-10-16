from pathlib import Path
import UnityPy,json
out=Path('/Users/liuzhenhua/Desktop/code/unity-match-3d-puzzle/Tools/LayeredPile/Evidence/selection-physics')
root=Path('/Users/liuzhenhua/Desktop/code/unity-match-3d-puzzle')
models=json.load(open(root/'Assets/GooseReplica/OriginalModels/models.json'))['models']
source=UnityPy.load('work/model-bundles','work/audit-goose-bundles');objects={p.lower():o for p,o in source.container.items()};ios=[];bound=[]
for model in models:
 obj=objects[model['source'].lower()]
 for c in obj.read().m_Component:
  o=c.component.deref()
  if o.type.name!='MonoBehaviour':continue
  t=o.read_typetree()
  if '_objectAnim' not in t:continue
  row=dict(name=model['name'],group=model['group'],source=model['source'],file=o.assets_file.name,pathID=o.path_id,objectAnim=t['_objectAnim'],changeObjScale=t.get('_changeObjScale'),objectCustomMod=t.get('_ObjectCustomMod'))
  if t['_objectAnim']['m_PathID']:
   pp=t['_objectAnim'];assert pp['m_FileID']==0
   a=o.assets_file.objects[pp['m_PathID']];row['binding']=a.read_typetree();bound.append(row)
  ios.append(row)
ns={};s=Path('work/decode_levelmanager_component.py').read_text();exec(s[:s.index("fmt=take('i')")],ns);unitType=next(i for i,t in enumerate(ns['T']) if t['name']=='ObjectUnit')
wx=UnityPy.load('work/wechat-country-objects.bundle','work/wechat-grocery-objects.bundle','work/wechat-barbecue-objects.bundle');wxrows=[]
for path,obj in wx.container.items():
 if obj.type.name!='GameObject':continue
 name=obj.read().m_Name
 if name not in {v['name']for v in ios}:continue
 for c in obj.read().m_Component:
  o=c.component.deref()
  if o.type.name!='MonoBehaviour':continue
  if o.read(check_read=False).m_Script.read().m_ClassName!='ObjectUnit':continue
  ns['B']=o.get_raw_data();ns['p']=28;ns['string']();vals={}
  for f,dt in ns['fields'](unitType):
   if f['name'] in ['notBeCatch','isHiding','isStore','_isSelect']:continue
   vals[f['name']]=ns['read'](dt)
  assert ns['p']==len(ns['B'])
  wxrows.append(dict(name=name,source=path,file=o.assets_file.name,pathID=o.path_id,objectAnim=vals['_objectAnim'],changeObjScale=vals.get('_changeObjScale'),objectCustomMod=vals.get('_ObjectCustomMod'),consumedBytes=ns['p']))
report=dict(iosCount=len(ios),iosBoundCount=len(bound),iosGooseCount=sum(v['name'].startswith('goose') or v['name'].startswith('Goose')for v in ios),wxCount=len(wxrows),wxBoundCount=sum(v['objectAnim']['path_id']!=0 for v in wxrows),ios=ios,wx=wxrows)
(out/'object-anim-bindings.json').write_text(json.dumps(report,ensure_ascii=False,indent=2));print({k:v for k,v in report.items() if k not in ['ios','wx']});print('Bound',bound)
