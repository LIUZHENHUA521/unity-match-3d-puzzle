import UnityPy,json
from pathlib import Path
out=Path('/Users/liuzhenhua/Desktop/code/unity-match-3d-puzzle/Tools/LayeredPile/Evidence/selection-physics')
s=Path('work/decode_levelmanager_component.py').read_text();ns={};exec(s[:s.index("fmt=take('i')")],ns)
e=UnityPy.load('work/goose-wechat-data/extracted/data.unity3d')
for o in e.objects:
 if o.type.name!='MonoBehaviour':continue
 try:
  obj=o.read(check_read=False);cls=obj.m_Script.deref().read().m_ClassName
 except Exception:continue
 if cls!='DOTweenSettings':continue
 B=o.get_raw_data();ns['B']=B;ns['p']=28;name=ns['string']();i=next(i for i,t in enumerate(ns['T']) if t['name']==cls)
 vals={};ranges=[]
 for f,d in ns['fields'](i):
  if 'nonserialized' in str(d):continue
  start=ns['p'];vals[f['name']]=ns['read'](d);ranges.append(dict(name=f['name'],start=start,end=ns['p']))
 result=dict(name=name,cls=cls,values=vals,ranges=ranges,sourceFile=o.assets_file.name,pathID=o.path_id,consumed=ns['p'],size=len(B))
 print(result);(out/'dotween-settings.json').write_text(json.dumps(result,indent=2));(out/'dotween-settings.bin').write_bytes(B)
