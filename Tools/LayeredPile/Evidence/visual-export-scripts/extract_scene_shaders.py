import UnityPy,lz4.block,json
from pathlib import Path
from UnityPy.export.ShaderConverter import ShaderProgram
from UnityPy.streams import EndianBinaryReader
out=Path('outputs/goose-analysis/visual-camera-evidence/programs');out.mkdir(exist_ok=True)
for name in ['countryside','countrysidestovesub','grocery-scene','barbecue','barbecuesub']:
 for o in UnityPy.load('work/wechat-'+name+'.bundle').objects:
  if o.type.name!='Shader':continue
  d=o.read_typetree();n=d['m_ParsedForm']['m_Name'];dest=out/n.replace('/','_');dest.mkdir(exist_ok=True)
  if (dest/'variants.json').exists():continue
  raw=lz4.block.decompress(bytes(d['compressedBlob']),uncompressed_size=d['decompressedLengths'][0][0]);s=ShaderProgram(EndianBinaryReader(raw,endian='<'),(2021,3,0,0));variants=[]
  for i,p in enumerate(s.m_SubPrograms):
   if i==0:continue
   (dest/(str(i)+'.glsl')).write_bytes(p.m_ProgramCode);variants.append(dict(index=i,keywords=p.m_Keywords))
  (dest/'variants.json').write_text(json.dumps(variants,indent=2));(dest/'parsed.json').write_text(json.dumps(d['m_ParsedForm'],indent=2));print(n,variants[:5])
