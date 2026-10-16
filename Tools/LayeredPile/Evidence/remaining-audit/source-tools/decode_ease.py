"""Run from research root. Decode WX field-default compressed signed int constants.
Only reads metadata; writes local evidence JSON. No production edits.
"""
from pathlib import Path
import json, struct, hashlib
root=Path('/Users/liuzhenhua/Documents/Codex/2026-10-13/r')
metadata=root/'work/goose-wechat-data/extracted/Il2CppData/Metadata/global-metadata.dat'
b=metadata.read_bytes()
# Header v29 fieldDefaultValuesOffset/Size and default-data base; independently
# decoded by work/read_wechat_metadata.py Il2CppGlobalMetadataHeader schema.
assert struct.unpack_from('<I',b,4)[0]==29
start,size,base=4378704,146496,4525200
rows={struct.unpack_from('<iii',b,p)[0]:(p,*struct.unpack_from('<iii',b,p)[1:]) for p in range(start,start+size,12)}
t=next(t for t in json.loads((root/'work/goose-wechat-metadata-types.json').read_text()) if t['name']=='Ease' and t['namespace']=='DG.Tweening')
result=[]
for i,f in enumerate(t['fields']):
 if i==0:continue
 entry,typ,idx=rows[t['fieldStart']+i];value=b[base+idx]
 assert value<128 and value%2==0 # every Ease constant is single-byte nonnegative zigzag
 result.append(dict(name=f['name'],value=value>>1,fieldIndex=t['fieldStart']+i,defaultEntryOffset=entry,dataOffset=base+idx,encodedHex=f'{value:02x}'))
out=dict(metadata=str(metadata),sha256=hashlib.sha256(b).hexdigest(),encoding='IL2CPP v29 compressed signed integer (positive values unsigned >> 1)',values=result)
Path(__file__).with_name('wx-ease-enum.json').write_text(json.dumps(out,indent=2)+'\n')
