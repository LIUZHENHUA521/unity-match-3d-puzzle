from pathlib import Path
import json
s=Path('work/decode_levelmanager_component.py').read_text();s=s[:s.index("fmt=take('i')")];ns={};exec(s,ns)
old_desc=ns['desc']
def desc(v):
 d=old_desc(v)
 return ('simple','int') if d==('nonserialized','LayerMask') else d
ns['desc']=desc
out=Path('outputs/goose-analysis/highlight-complete-evidence')
for f in out.glob('*.bin'):
 ns['B']=f.read_bytes();ns['p']=28;name=ns['string']();cls='HighlightProfile' if int(f.stem)<740 else 'HighlightEffect';i=next(i for i,t in enumerate(ns['T']) if t['name']==cls)
 vals={};ranges=[]
 for field,d in ns['fields'](i):
  if 'nonserialized' in str(d):continue
  start=ns['p'];v=ns['read'](d);vals[field['name']]=v;ranges.append({'name':field['name'],'start':start,'end':ns['p'],'descriptor':d})
 assert ns['p']==len(ns['B']),(f,ns['p'],len(ns['B']))
 (f.with_suffix('.json')).write_text(json.dumps({'name':name,'class':cls,'values':vals,'ranges':ranges},indent=2));print(f.name,name,vals)
