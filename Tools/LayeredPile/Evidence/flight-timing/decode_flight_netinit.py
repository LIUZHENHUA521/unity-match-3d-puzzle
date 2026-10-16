from pathlib import Path
import json,struct,hashlib,contextlib,io
s=Path('work/decode_levelmanager_component.py').read_text();s=s[:s.index("fmt=take('i')")];ns={};exec(s,ns);ns['B']=Path('work/flight-net-init-wx-raw.bin').read_bytes();ns['p']=32
take=ns['take'];align=ns['align'];pointer=ns['pointer'];string=ns['string']
fmt=take('i');n=take('i');ns['p']+=n;align();refs=[pointer() for _ in range(take('i'))];ss=string();prefab=pointer();prefrefs=[pointer() for _ in range(take('i'))];mods=[string() for _ in range(take('i'))];nodes=take('i')
# Independently verify native descriptor ordering against WX metadata access flags/SerializeField.
wx_source=Path('work/recover_serialization.py').read_text().split('reg=')[0].replace('work/partial-extracted/global-metadata.dat','work/goose-wechat-data/extracted/Il2CppData/Metadata/global-metadata.dat').replace('work/metadata-types.json','work/goose-wechat-metadata-types.json')
w={}
with contextlib.redirect_stdout(io.StringIO()):exec(wx_source,w)
mem=Path('work/goose-wasm-main-memory.bin').read_bytes();u=lambda p:struct.unpack_from('<I',mem,p)[0]
t=next(t for t in w['T'] if t['name']=='NetInitManager');wanted=[]
for f in t['fields']:
 ptr=u(184576+4*f['typeIndex']);bits=u(ptr+4);a=bits&65535;k=(bits>>16)&255
 if not a&(0x10|0x40|0x80) and (a&7==6 or 'SerializeField' in w['attrs'].get(f['token'],[])) and k!=0x15:wanted.append(f['name'])
actual=[f['name'].replace('_sdkConfig','_wxSDKConfig') for f,d in ns['fields'](708) if f['name']!='LocalServerUrl']
assert wanted==actual,(wanted,actual)
values={};ranges=[]
for f,d in ns['fields'](708):
 if f['name']=='LocalServerUrl':continue
 start=ns['p'];name=f['name'].replace('_sdkConfig','_wxSDKConfig');values[name]=ns['read'](d);ranges.append({'name':name,'start':start,'end':ns['p']})
assert ns['p']==len(ns['B']);print({k:v for k,v in values.items() if 'Anim' in k});print(ns['p'],len(ns['B']))
Path('outputs/goose-analysis/flight-timing/net-init-wx.json').write_text(json.dumps({'artifact':'WX2.1.12 data.unity3d level0 NetInitManager path_id145','sha256':hashlib.sha256(ns['B']).hexdigest(),'fullyConsumedBytes':ns['p'],'wxSerializedFieldOrderVerified':True,'values':values,'ranges':ranges},indent=2))
