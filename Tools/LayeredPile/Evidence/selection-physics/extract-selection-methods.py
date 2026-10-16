from pathlib import Path
import json,re
from collections import defaultdict
m=defaultdict(list)
for e in json.load(open('work/wechat-all-method-map.json')):m[str(e['j'])].append(e['type']+'.'+e['name'])
out=Path('/Users/liuzhenhua/Desktop/code/unity-match-3d-puzzle/Tools/LayeredPile/Evidence/selection-physics');out.mkdir(parents=True,exist_ok=True)
ids={'6603','7954','4679','12985','19264','19262','13226','19867','13231','6539','6538','18665','76396','6540','90554','36728','63999','63997','63998','83394','38498','5869','19882','13227','65349','36803','36804','31214','1183','3031','1360','90473','17154','65794'}
for k,v in m.items():
 if any(n.startswith('ObjectUnit.') for n in v):ids.add(k)
manifest=[]; callers=[]
for side in ['main','sub']:
 s=Path('work/goose-'+side+'.wat').read_text();ms=list(re.finditer(r'^  \(func \$j(\d+)\b',s,re.M))
 for i,match in enumerate(ms):
  id=match[1];b=s[match.start():ms[i+1].start() if i+1<len(ms) else len(s)];calls=re.findall(r'call \$j(\d+)\b',b)
  if '6603' in calls or any(c in calls for c in ['76396','76367','76374']):callers.append({'j':id,'names':m[id],'relevantCalls':[{ 'j':c,'names':m[c]} for c in calls if c in ['6603','76396','76367','76374']]});ids.add(id)
  if id not in ids:continue
  b=re.sub(r'call \$j(\d+)\b',lambda z:z[0]+' ;; '+','.join(m[z[1]]),b)
  (out/(side+'-j'+id+'.wat')).write_text(b)
  manifest.append({'j':id,'names':m[id],'side':side,'calls':[{'j':c,'names':m[c]} for c in dict.fromkeys(calls)]})
(out/'manifest.json').write_text(json.dumps(manifest,ensure_ascii=False,indent=2));(out/'callers.json').write_text(json.dumps(callers,ensure_ascii=False,indent=2))
print('saved',len(manifest),'functions');print(json.dumps(callers,ensure_ascii=False,indent=2))
