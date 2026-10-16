"""Recover generic Transform curves from shipped Unity streamed/dense/constant clips."""
import UnityPy,json,zlib,struct,math,hashlib
from pathlib import Path
project=Path('/Users/liuzhenhua/Desktop/code/unity-match-3d-puzzle')
out=project/'Assets/GooseReplica/OriginalModels/Animations';out.mkdir(exist_ok=True)
evidence=project/'Tools/LayeredPile/Evidence/goose-animation';evidence.mkdir(parents=True,exist_ok=True)
models=json.loads((project/'Assets/GooseReplica/OriginalModels/models.json').read_text())['models']
paths={}
def collect(n,path):
 paths[zlib.crc32(path.encode())]=path
 for c in n['children']:collect(c,(path+'/' if path else '')+c['name'])
def find(n,name):
 if n['name']==name and any(c['name']=='shenti' for c in n['children']):return n
 for c in n['children']:
  r=find(c,name)
  if r:return r
animroots=[]
for m in models:
 if not m['name'].startswith('Goose_'):continue
 def rootfind(n,p):
  if any(c['name']=='shenti' for c in n['children']):return n,p
  for c in n['children']:
   r=rootfind(c,p+'/'+c['name'])
   if r:return r
 n,p=rootfind(m['root'],m['root']['name']);collect(n,'');animroots.append({'model':m['name'],'group':m['group'],'path':p})
exports=[];audit=[]
for o in UnityPy.load('work/anim-bundles').objects:
 if o.type.name!='AnimationClip':continue
 t=o.read_typetree()
 if t['m_Name']=='HandCatch':continue # separate hand rig, not a goose skeleton
 mc=t['m_MuscleClip'];c=mc['m_Clip']['data'];stream=c['m_StreamedClip'];dense=c['m_DenseClip'];constant=c['m_ConstantClip']['data'];start=mc['m_StartTime'];end=mc['m_StopTime'];channels={}
 assert start==0
 blob=struct.pack('<'+'I'*len(stream['data']),*stream['data']);off=0;frames=[]
 while off<len(blob):
  time,count=struct.unpack_from('<fi',blob,off);off+=8;keys=[]
  for _ in range(count):
   index,*co=struct.unpack_from('<i4f',blob,off);off+=20;channels.setdefault(index,[]).append((time,co));keys.append(index)
  frames.append((time,len(keys)))
 curves=[];index=0
 for binding in t['m_ClipBindingConstant']['genericBindings']:
  assert binding['path'] in paths, (t['m_Name'],binding)
  assert binding['typeID']==4 and binding['attribute'] in [1,2,3] and not binding['isPPtrCurve']
  prefix={1:'m_LocalPosition',2:'m_LocalRotation',3:'m_LocalScale'}[binding['attribute']];dim=4 if binding['attribute']==2 else 3
  for axis in 'xyzw'[:dim]:
   ks=[]
   if index<stream['curveCount']:
    source=channels[index]
    finite=[(tm,co) for tm,co in source if 0<=tm<=end]
    def evaluated(tm):
     a,co=max((v for v in source if v[0]<=tm),key=lambda v:v[0]);dt=tm-a
     if a<-1e30:return co[3],0
     return ((co[0]*dt+co[1])*dt+co[2])*dt+co[3],(3*co[0]*dt+2*co[1])*dt+co[2]
    times=sorted(set([0,end]+[tm for tm,co in finite]))
    for tm in times:
     val,outtan=evaluated(tm)
     previous=[v for v in source if v[0]<tm and v[0]>-1e30]
     if previous:
      a,co=previous[-1];dt=tm-a;intan=(3*co[0]*dt+2*co[1])*dt+co[2]
     else:intan=outtan
     ks.append({'time':tm,'value':val,'inTangent':intan,'outTangent':outtan})
   elif index<stream['curveCount']+dense['m_CurveCount']:
    di=index-stream['curveCount'];count=dense['m_CurveCount']
    for f in range(dense['m_FrameCount']):
     tm=dense['m_BeginTime']+f/dense['m_SampleRate'];v=dense['m_SampleArray'][f*count+di]
     prev=dense['m_SampleArray'][max(0,f-1)*count+di];nxt=dense['m_SampleArray'][min(dense['m_FrameCount']-1,f+1)*count+di]
     ks.append({'time':tm,'value':v,'inTangent':(v-prev)*dense['m_SampleRate'],'outTangent':(nxt-v)*dense['m_SampleRate']})
   else:
    v=constant[index-stream['curveCount']-dense['m_CurveCount']];ks=[{'time':tm,'value':v,'inTangent':0,'outTangent':0} for tm in [0,end]]
   assert all(math.isfinite(v) for k in ks for v in k.values())
   curves.append({'path':paths[binding['path']],'property':prefix+'.'+axis,'keys':ks});index+=1
 assert index==stream['curveCount']+dense['m_CurveCount']+len(constant)
 # Verify every cubic segment against recovered Hermite keyframes at several interior points.
 maxerr=0
 for index,curve in enumerate(curves[:stream['curveCount']]):
  for a,b in zip(curve['keys'],curve['keys'][1:]):
   dt=b['time']-a['time']
   for u in [.125,.25,.5,.75,.875]:
    tm=a['time']+u*dt;ot,co=max((v for v in channels[index] if v[0]<=tm),key=lambda v:v[0]);odt=tm-ot
    expected=((co[0]*odt+co[1])*odt+co[2])*odt+co[3]
    actual=(2*u**3-3*u*u+1)*a['value']+(u**3-2*u*u+u)*dt*a['outTangent']+(-2*u**3+3*u*u)*b['value']+(u**3-u*u)*dt*b['inTangent']
    maxerr=max(maxerr,abs(expected-actual))
 assert maxerr<1e-4,(t['m_Name'],maxerr)
 exports.append({'name':t['m_Name'],'length':end,'sampleRate':t['m_SampleRate'],'loop':mc['m_LoopTime'],'curves':curves})
 audit.append({'name':t['m_Name'],'sourceFile':o.assets_file.name,'pathID':str(o.path_id),'rawSha256':hashlib.sha256(o.get_raw_data()).hexdigest(),'length':end,'loop':mc['m_LoopTime'],'streamed':stream['curveCount'],'dense':dense['m_CurveCount'],'constant':len(constant),'curveCount':len(curves),'keyCount':sum(len(v['keys']) for v in curves),'maxHermiteError':maxerr,'events':len(t['m_Events'])})
 (evidence/(t['m_Name']+'-source.json')).write_text(json.dumps(t,separators=(',',':')))
(out/'source-clips.json').write_text(json.dumps({'models':animroots,'clips':exports},separators=(',',':')))
(evidence/'inventory.json').write_text(json.dumps({'source':'goose-1.11.03.ipa anim_assets bundles','models':animroots,'clips':audit},indent=2))
print(json.dumps(audit,indent=2))
