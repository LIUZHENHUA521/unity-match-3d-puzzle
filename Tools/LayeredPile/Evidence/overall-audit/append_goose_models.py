from pathlib import Path
exec(Path('work/export_goose_models.py').read_text().split('models=[];errors=[]')[0])
old=json.loads((root/'models.json').read_text()); targets=json.loads(Path('outputs/goose-analysis/missing-goose-assets.json').read_text()); extra=[]
for item in targets:
 env=UnityPy.load(item['bundle']);go=env.container[item['path']].deref()
 def check(go):
  t=go.read_typetree();comps=[ref(go,p['component']) for p in t['m_Component']]
  unsupported=[c.type.name for c in comps if c.type.name in ['SkinnedMeshRenderer','MeshCollider','Animator','Animation']]
  if unsupported:print('EXTRA COMPONENTS',t['m_Name'],unsupported)
  tr=next(c for c in comps if c.type.name=='Transform')
  for p in tr.read_typetree()['m_Children']:
   ct=ref(tr,p);check(ref(ct,ct.read_typetree()['m_GameObject']))
 check(go)
 extra.append({'name':item['name'],'group':item['path'].split('/TargetObjPrefabs/')[1].split('/')[0],'legacy':False,'source':item['path'],'bundle':item['bundle'],'root':node(go)})
for field,values in [('models',extra),('meshes',meshes.values()),('materials',mats.values()),('textureInfos',texture_infos.values())]:
 k='name' if field=='models' else 'id';merged={x[k]:x for x in old[field]};merged.update({x[k]:x for x in values});old[field]=list(merged.values())
(root/'models.json').write_text(json.dumps(old,ensure_ascii=False,separators=(',',':')))
(evidence/'goose-materials-source.json').write_text(json.dumps(source_materials,ensure_ascii=False,indent=2))
(evidence/'goose-shader-parsed-ios.json').write_text(json.dumps(source_shaders,ensure_ascii=False,indent=2))
print('totals',{k:len(v) for k,v in old.items()});print('new shader variants',sorted(set(m['portedShader'] for m in mats.values())))
