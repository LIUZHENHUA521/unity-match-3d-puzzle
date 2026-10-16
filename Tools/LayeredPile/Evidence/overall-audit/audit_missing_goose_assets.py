import zipfile,UnityPy,json
from pathlib import Path
names={'Goose_Tomato','Goose_SteelCup','Goose_Farmer','Goose_Watermelon','Goose_ChiliSauce','Goose_ColaCup','Goose_Egg','Goose_Noodles','Goose_Eggplant','Goose_GreenPepper','Goose_Pancake','Goose_Potato'}
out=Path('work/audit-goose-bundles');out.mkdir(exist_ok=True);result=[]
with zipfile.ZipFile('work/goose-1.11.03.ipa') as z:
 for n in z.namelist():
  fn=Path(n).name
  if not fn.endswith('.bundle') or not fn.startswith(('gooseitem_assets_','goosefirst_assets_')):continue
  p=out/fn
  if not p.exists():p.write_bytes(z.read(n))
  e=UnityPy.load(str(p))
  for path,pp in e.container.items():
   if Path(path).stem in names:
    o=pp.deref();d=o.read_typetree();cs=[o.assets_file.objects[c['component']['m_PathID']] for c in d['m_Component']]
    result.append(dict(name=Path(path).stem,path=path,bundle=str(p),components=[c.type.name for c in cs]));print(result[-1],flush=True)
Path('outputs/goose-analysis/missing-goose-assets.json').write_text(json.dumps(result,ensure_ascii=False,indent=2));print('found',len(result),'missing',names-set(x['name'] for x in result),flush=True)
