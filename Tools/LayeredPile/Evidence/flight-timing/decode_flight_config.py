from pathlib import Path
import struct,json,contextlib,io
s=Path('work/decode_levelmanager_component.py').read_text();s=s[:s.index("fmt=take('i')")]
ns={};exec(s,ns)
ns['B']=Path('work/flight-config-raw.bin').read_bytes();ns['p']=28
name=ns['string']();take=ns['take'];pointer=ns['pointer'];string=ns['string'];align=ns['align']
fmt=take('i');n=take('i');ns['p']+=n;align();refs=[pointer() for _ in range(take('i'))];ss=string();prefab=pointer();prefrefs=[pointer() for _ in range(take('i'))];mods=[string() for _ in range(take('i'))];nodes=take('i');assert nodes==0
print(name,'start',ns['p'])
vals={};ranges=[]
T=ns['T'];ty=ns['ty'];desc=ns['desc'];read=ns['read'];i=next(i for i,t in enumerate(T) if t['name']=='ObjAnimConfig')
for f in T[i]['fields']:
 d=desc(ty(f['typeIndex']));start=ns['p']
 if d==('nonserialized','AnimationCurve'):
  n=take('i');assert n<100,n
  keys=[dict(zip(['time','value','inSlope','outSlope','weightedMode','inWeight','outWeight'],take('ffffiff'))) for _ in range(n)]
  v={'keys':keys, 'preInfinity':take('i'),'postInfinity':take('i'),'rotationOrder':take('i')}
 else:v=read(d)
 vals[f['name']]=v;ranges.append({'name':f['name'],'start':start,'end':ns['p'],'descriptor':d})
assert ns['p']==len(ns['B']),(ns['p'],len(ns['B']))
Path('outputs/goose-analysis/flight-timing/object-anim-config-ios.json').write_text(json.dumps({'name':name,'source':'sharedassets0.assets path_id766 referenced by LevelObjAnimManager957','values':vals,'ranges':ranges},indent=2))
print(json.dumps(vals,indent=2))
