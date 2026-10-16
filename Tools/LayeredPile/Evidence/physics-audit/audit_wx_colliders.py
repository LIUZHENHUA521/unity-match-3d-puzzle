import json,UnityPy
from pathlib import Path
source=Path('work/export_source_colliders.py').read_text()
source=source[:source.index("out=Path(")].replace("UnityPy.load('work/model-bundles')", "UnityPy.load('work/wechat-country-objects.bundle','work/wechat-grocery-objects.bundle','work/wechat-barbecue-objects.bundle')")
exec(source)
ios={r['name']:r for r in json.load(open('outputs/goose-analysis/spawn-collider-evidence/collider-profiles-ios.json'))['items']}
def normalize(r):
 return {k:v for k,v in r.items() if k not in ['source']}
diff=[]
for r in rows:
 if r['name'] not in ios:diff.append({'name':r['name'],'difference':'missing iOS'});continue
 a=normalize(r);b=normalize(ios[r['name']]);
 if a!=b:
  keys=[k for k in a if a[k]!=b.get(k)]
  diff.append({'name':r['name'],'keys':keys,'wx':a,'ios':b})
out=Path('outputs/goose-analysis/physics-audit');out.mkdir(exist_ok=True)
(out/'wx-collider-profiles.json').write_text(json.dumps(rows,ensure_ascii=False,indent=2))
(out/'wx-ios-collider-comparison.json').write_text(json.dumps({'wxProfiles':len(rows),'iosProfiles':len(ios),'differences':diff},ensure_ascii=False,indent=2))
print('profiles',len(rows),'differences',len(diff)); print([(r['name'],r.get('keys',r.get('difference'))) for r in diff])
