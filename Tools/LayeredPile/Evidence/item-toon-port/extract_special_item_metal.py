import UnityPy,lz4.block,json
from pathlib import Path
from UnityPy.export.ShaderConverter import ShaderProgram
from UnityPy.streams import EndianBinaryReader
out=Path('outputs/goose-analysis/item-toon-evidence/metal');out.mkdir(parents=True,exist_ok=True)
manifest=[]
for path in sorted(Path('work/model-bundles').glob('*.bundle')):
 for o in UnityPy.load(str(path)).objects:
  if o.type.name!='Shader':continue
  d=o.read_typetree();form=d['m_ParsedForm'];n=form['m_Name']
  if n not in ['ToonGlass','ToonGlassBack','ToonEffectFull']:continue
  raw=lz4.block.decompress(bytes(d['compressedBlob']),uncompressed_size=d['decompressedLengths'][0][0]);s=ShaderProgram(EndianBinaryReader(raw,endian='<'),(2021,3,0,0))
  for pi,ps in enumerate(form['m_SubShaders'][0]['m_Passes']):
   for stage in ['Vertex','Fragment']:
    for info in ps['prog'+stage]['m_PlayerSubPrograms'][0]:
     kws=[form['m_KeywordNames'][i] for i in info['m_KeywordIndices']]
     if set(kws)-set(['DIRECTIONAL','_ISSINGLECOLOR_ON','_ISUSEWORLDMATCAP_ON','_ISUSENORMALMAP_ON','SHADOWS_SCREEN','SHADOWS_DEPTH','UNITY_PASS_SHADOWCASTER']):continue
     code=s.m_SubPrograms[info['m_BlobIndex']].m_ProgramCode;code=code[code.index(b'#include'):].split(b'\0')[0]
     name=n+'-'+str(pi)+'-'+stage+'-'+('_'.join(kws) or 'none');dest=out/(name+'.metal')
     if dest.exists():assert dest.read_bytes()==code,name
     else:dest.write_bytes(code);(out/(name+'.json')).write_text(json.dumps(dict(bundle=str(path),pathId=o.path_id,blobIndex=info['m_BlobIndex'],keywords=kws,state=ps['m_State'],properties=form['m_PropInfo']),indent=2))
     manifest.append(dict(name=name,bundle=str(path),blobIndex=info['m_BlobIndex']))
(out/'manifest.json').write_text(json.dumps(manifest,indent=2));print('unique',len(set(x['name'] for x in manifest)))
