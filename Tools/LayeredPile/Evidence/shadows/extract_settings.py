import UnityPy,json
from pathlib import Path
E=UnityPy.load('work/goose-wechat-data/extracted/data.unity3d');out=[]
for o in E.objects:
 if o.type.name=='QualitySettings':out.append(dict(type=o.type.name,file=o.assets_file.name,id=o.path_id,data=o.read_typetree()))
 if o.type.name=='Light':
  d=o.read_typetree();F=o.assets_file.objects;go=F[d['m_GameObject']['m_PathID']].read_typetree();t=next(F[c['component']['m_PathID']] for c in go['m_Component'] if F[c['component']['m_PathID']].type.name=='Transform');chain=[]
  while t:
   td=t.read_typetree();g=F[td['m_GameObject']['m_PathID']].read_typetree();chain.append(dict(id=t.path_id,name=g['m_Name'],active=g['m_IsActive'],data=td));p=td['m_Father'];t=F[p['m_PathID']] if p['m_PathID'] else None
  out.append(dict(type='LightHierarchy',id=o.path_id,file=o.assets_file.name,chain=chain))
p=Path('outputs/goose-analysis/shadow-evidence');p.mkdir(exist_ok=True);(p/'quality-light-hierarchy.json').write_text(json.dumps(out,indent=2));print(json.dumps(out,indent=2))
