"""Preserve each bundle-specific shader and its exact parsed-to-blob mapping."""
import UnityPy,lz4.block,json,hashlib,struct,argparse
from pathlib import Path
out=Path('outputs/goose-analysis/item-material-evidence/ios-exact-programs');out.mkdir(exist_ok=True)
parser=argparse.ArgumentParser();parser.add_argument('bundles',nargs='?',default='work/model-bundles');parser.add_argument('--append',action='store_true');args=parser.parse_args()
index=json.loads((out/'index.json').read_text()) if args.append and (out/'index.json').exists() else []
known={x['id'] for x in index}
for o in UnityPy.load(args.bundles).objects:
 if o.type.name!='Shader':continue
 d=o.read_typetree();parsed=d['m_ParsedForm'];name=parsed['m_Name'];ident=hashlib.sha1((o.assets_file.name+':'+str(o.path_id)).encode()).hexdigest()[:16];dest=out/(name+'-'+ident);dest.mkdir(exist_ok=True)
 if ident in known:continue
 raw=lz4.block.decompress(bytes(d['compressedBlob']),uncompressed_size=d['decompressedLengths'][0][0]);(dest/'program.raw').write_bytes(raw);(dest/'parsed.json').write_text(json.dumps(parsed,ensure_ascii=False,indent=2))
 entries=[]
 for i in range(struct.unpack_from('<i',raw)[0]):
  off,size,segment=struct.unpack_from('<3i',raw,4+i*12);entry=raw[off:off+size];loc=entry.find(b'#include');entryinfo={'index':i,'offset':off,'size':size,'segment':segment,'hasMetal':loc>=0}
  if loc>=0:(dest/(str(i)+'.metal')).write_bytes(entry[loc:].split(b'\x00')[0])
  entries.append(entryinfo)
 variants=[]
 for si,sub in enumerate(parsed['m_SubShaders']):
  for pi,p in enumerate(sub['m_Passes']):
   for stage in ['progVertex','progFragment']:
    for prog in p[stage]['m_PlayerSubPrograms'][0] if p[stage]['m_PlayerSubPrograms'] else []:
     variants.append({'subshader':si,'pass':pi,'passName':p['m_State']['m_Name'],'stage':stage,'index':prog['m_BlobIndex'],'keywords':[parsed['m_KeywordNames'][x] for x in prog['m_KeywordIndices']]})
 (dest/'variants.json').write_text(json.dumps(variants,indent=2));index.append({'id':ident,'name':name,'sourceFile':o.assets_file.name,'pathID':str(o.path_id),'folder':dest.name,'entries':entries})
(out/'index.json').write_text(json.dumps(index,indent=2));print('bundle-specific shaders',len(index))
