from pathlib import Path
import json,UnityPy
s=Path('work/decode_levelmanager_component.py').read_text();ns={};exec(s[:s.index("fmt=take('i')")],ns)
e=UnityPy.load('work/goose-wechat-data/extracted/data.unity3d');out=Path('outputs/goose-analysis/audio-evidence')
for o in e.objects:
 if o.type.name!='MonoBehaviour':continue
 d=o.read(check_read=False)
 if d.m_Script.read().m_ClassName!='SoundManager':continue
 b=o.get_raw_data();ns['B']=b;ns['p']=28;name=ns['string']();i=next(i for i,t in enumerate(ns['T']) if t['name']=='SoundManager');vals={};ranges=[]
 for field,dt in ns['fields'](i):
  p=ns['p'];vals[field['name']]=ns['read'](dt);ranges.append([field['name'],dt,p,ns['p']])
 print(o.path_id,len(b),ns['p'],vals);assert len(b)==ns['p'];sources={}
 for key in ['_uiSoundSource','_gooseSoundSource','_effectSoundSource','_musicSoundSource']:
  for p in (vals[key] if isinstance(vals[key],list) else [vals[key]]):
   if p['file_id']==0 and p['path_id']:
    a=o.assets_file.objects[p['path_id']];sources[str(p['path_id'])]=a.read_typetree()
 (out/'sound-manager-wx.json').write_text(json.dumps({'file':o.assets_file.name,'id':o.path_id,'values':vals,'ranges':ranges,'sources':sources},indent=2))
