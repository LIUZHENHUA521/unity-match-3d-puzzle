import json,math
from pathlib import Path
P=Path('/Users/liuzhenhua/Desktop/code/unity-match-3d-puzzle');S=P/'Assets/GooseReplica/SourceToolVisuals/Modules';R=P/'Builds/WechatProject/visual-captures/source-tool-module-roundtrip';errors=[];count=0
# Object refs are intentionally remapped after restore; runtime native Unity emits owner refs not source CAB IDs.
def compare(s,t,path):
 global count
 if isinstance(s,dict):
  if 'instanceID' in s:return
  if not isinstance(t,dict):errors.append((path,'type',s,t));return
  for k,v in s.items():
   if k in ['m_LightmapIndex','m_LightmapIndexDynamic','m_LightmapTilingOffset','m_LightmapTilingOffsetDynamic']:continue
   if k not in t:errors.append((path+'.'+k,'missing',v,None))
   else:compare(v,t[k],path+'.'+k)
 elif isinstance(s,list):
  if not isinstance(t,list):errors.append((path,'type',s,t));return
  if len(s)!=len(t):errors.append((path,'length',len(s),len(t)))
  for i,(a,b) in enumerate(zip(s,t)):compare(a,b,path+'['+str(i)+']')
 elif isinstance(s,(float,int)) and isinstance(t,(float,int)):
  count+=1
  if abs(s-t)>max(1e-6,abs(s)*1e-6):errors.append((path,'value',s,t))
 else:
  count+=1
  if s!=t:errors.append((path,'value',s,t))
for file in S.glob('*.json'):
 if not (R/file.name).exists():errors.append((file.name,'missing file',None,None));continue
 source=json.loads(file.read_text());actual=json.loads((R/file.name).read_text())
 if len(actual)==1 and isinstance(next(iter(actual.values())),dict):actual=next(iter(actual.values()))
 compare(source,actual,file.name)
report={'status':'PASS' if not errors else 'FAIL','checkedScalarFields':count,'differences':errors}
out=Path('outputs/goose-analysis/source-tools-evidence/vfx/native-module-comparison.json');out.write_text(json.dumps(report,indent=2));print(report['status'],count,'checked,',len(errors),'differences');print(json.dumps(errors[:30],indent=1)[:10000])
