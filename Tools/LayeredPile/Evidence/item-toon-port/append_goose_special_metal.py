"""Append newly used Full-Toon keyword programs, preserving older evidence/index entries."""
from pathlib import Path
import json,hashlib
base=Path('outputs/goose-analysis/item-material-evidence/ios-exact-programs')
out=Path('outputs/goose-analysis/item-toon-evidence/metal')
index=json.loads((base/'index.json').read_text());manifest=[]
for ident in ['5f13ea99433469a9','a8f2be5a89a42a84']:
 item=next(x for x in index if x['id']==ident);folder=base/item['folder'];form=json.loads((folder/'parsed.json').read_text())
 for variant in json.loads((folder/'variants.json').read_text()):
  kws=variant['keywords']
  if set(kws) not in [{'_ISSINGLECOLOR_ON','DIRECTIONAL'},{'_ISUSEWORLDMATCAP_ON','DIRECTIONAL'}]:continue
  stage=variant['stage'][4:];pi=variant['pass'];code=(folder/(str(variant['index'])+'.metal')).read_bytes()
  name='ToonEffectFull-'+str(pi)+'-'+stage+'-'+'_'.join(kws);dest=out/(name+'.metal')
  if dest.exists():assert dest.read_bytes()==code,'bundle-specific variant diverged: '+name
  else:
   dest.write_bytes(code)
   (out/(name+'.json')).write_text(json.dumps(dict(sourceId=ident,sourceFile=item['sourceFile'],exactProgram=variant['index'],keywords=kws,state=form['m_SubShaders'][0]['m_Passes'][pi]['m_State'],properties=form['m_PropInfo']),indent=2))
  manifest.append(dict(name=name,sourceId=ident,blobIndex=variant['index'],sha256=hashlib.sha256(code).hexdigest()))
(out/'goose-variant-manifest.json').write_text(json.dumps(manifest,indent=2));print('verified programs:',len(manifest),'unique:',len(set(x['name'] for x in manifest)))
