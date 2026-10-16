import UnityPy,json
from pathlib import Path
E=UnityPy.load('work/goose-wechat-data/extracted/data.unity3d')
def arr(d):return [d.x,d.y,d.z]
def add(a,b):return [x+y for x,y in zip(a,b)]
def mul(a,b):return [x*y for x,y in zip(a,b)]
def cross(a,b):return [a[1]*b[2]-a[2]*b[1],a[2]*b[0]-a[0]*b[2],a[0]*b[1]-a[1]*b[0]]
def rot(q,v):
 qv=[q.x,q.y,q.z];t=[2*x for x in cross(qv,v)];return add(v,add([q.w*x for x in t],cross(qv,t)))
def world(t,p=[0,0,0]):
 p=add(arr(t.m_LocalPosition),rot(t.m_LocalRotation,mul(arr(t.m_LocalScale),p)))
 return world(t.m_Father.read(),p) if t.m_Father.m_PathID else p
def path(t):return (path(t.m_Father.read())+'/' if t.m_Father.m_PathID else '')+t.m_GameObject.read().m_Name
out=[]
for o in E.objects:
 if o.type.name!='Transform':continue
 d=o.read();name=d.m_GameObject.read().m_Name
 if name.startswith('Point') or name.startswith('PlateItem'):
  p=path(d)
  if any(s in p for s in ['Content','Plate']):out.append({'path':p,'world':world(d),'local':arr(d.m_LocalPosition),'rotation':str(d.m_LocalRotation)})
Path('outputs/goose-analysis/flight-timing/slot-geometry-wx.json').write_text(json.dumps(out,indent=2));print(json.dumps(out,indent=2))
