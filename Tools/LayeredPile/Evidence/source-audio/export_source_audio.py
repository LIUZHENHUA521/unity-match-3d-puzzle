from pathlib import Path
import UnityPy,json,hashlib
root=Path('/Users/liuzhenhua/Desktop/code/unity-match-3d-puzzle');out=root/'Assets/GooseReplica/Resources/Replica/SourceAudio';out.mkdir(exist_ok=True)
e=UnityPy.load('work/model-bundles','work/audit-goose-bundles');objs={p.lower():o for p,o in e.container.items()};models=json.load(open(root/'Assets/GooseReplica/OriginalModels/models.json'))['models'];rows=[]
T=json.load(open('work/goose-wechat-metadata-types.json'));names=[f['name'] for t in T if t['name']=='SoundType' for f in t['fields'] if f['name']!='value__']
need={23,24,25,26,27,28,29,30,31,32,33,34,35}
for model in models:
 for c in objs[model['source'].lower()].read().m_Component:
  o=c.component.read().object_reader
  if o.type.name!='MonoBehaviour':continue
  t=o.read_typetree()
  if '_effectSoundType' in t:
   row={'name':model['name'],'select':t['_effectSoundType'],'usePitch':bool(t['_soundUsePitch']),'combine':t['_endSoundType'],'combineDefault':bool(t['_endSoundCombie']),'goose':bool(t['_isGoose'])};rows.append(row);need.update([row['select'],row['combine']]);break
assert len(rows)==83
(root/'Assets/GooseReplica/Resources/Replica/SourceAudioItems.json').write_text(json.dumps({'items':rows,'names':names},indent=2))
e=UnityPy.load('work/source-audio-bundles');clips=[]
for o in e.objects:
 if o.type.name!='AudioClip':continue
 d=o.read();clips.append((d.m_Name,o,d))
print('need names',[names[n] for n in sorted(need) if n]);print('available',[(n,o.path_id) for n,o,d in clips if n in [names[i] for i in need]])
manifest=[]
for id in sorted(need):
 if id==0:continue
 name=names[id];matches=[(o,d) for n,o,d in clips if n==name]
 if not matches:print('MISSING',id,name);continue
 o,d=matches[0]
 for filename,data in d.samples.items():
  dest=out/(name+Path(filename).suffix);dest.write_bytes(data)
  manifest.append({'id':id,'name':name,'asset':o.assets_file.name,'pathId':o.path_id,'file':str(dest.relative_to(root)),'sha256':hashlib.sha256(data).hexdigest(),'samples':len(data),'frequency':d.m_Frequency,'channels':d.m_Channels,'length':d.m_Length})
print('exported',len(manifest));Path('outputs/goose-analysis/audio-evidence/audio-export-manifest.json').write_text(json.dumps(manifest,indent=2))
