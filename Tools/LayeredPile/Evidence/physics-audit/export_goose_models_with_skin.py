import UnityPy,json,hashlib,re
from pathlib import Path
from UnityPy.helpers.MeshHelper import MeshHandler
root=Path('/Users/liuzhenhua/Desktop/code/unity-match-3d-puzzle/Assets/GooseReplica/OriginalModels');root.mkdir(parents=True,exist_ok=True);(root/'Textures').mkdir(exist_ok=True)
e=UnityPy.load('work/model-bundles');meshes={};mats={};textures={};texture_infos={};source_materials={};source_shaders={}
evidence=Path('outputs/goose-analysis/item-material-evidence');evidence.mkdir(parents=True,exist_ok=True)
def key(o):return hashlib.sha1((o.assets_file.name+':'+str(o.path_id)).encode()).hexdigest()[:16]
def ref(o,p):
 if not p or not p.get('m_PathID'):return None
 if p['m_FileID']:raise ValueError('external dependency '+str(p))
 return o.assets_file.objects[p['m_PathID']]
def mesh(o):
 k=key(o)
 if k not in meshes:
  d=o.read();h=MeshHandler(d);h.process()
  meshes[k]={'id':k,'name':d.m_Name,'vertices':[dict(zip(('x','y','z'),v[:3])) for v in h.m_Vertices],'tangents':[dict(zip(('x','y','z','w'),v[:4])) for v in (h.m_Tangents or [])],'normals':[dict(zip(('x','y','z'),v[:3])) for v in (h.m_Normals or [])],'uv':[dict(zip(('x','y'),v[:2])) for v in (h.m_UV0 or [])],'submeshes':[{'indices':[i for tri in group for i in tri]} for group in h.get_triangles()]}
  bind=d.m_BindPose or []
  meshes[k]['bindposes']=[{'values':[getattr(v,'e'+str(r)+str(c)) for r in range(4) for c in range(4)]} for v in bind]
  if h.m_BoneWeights:
   assert len(h.m_BoneWeights)==len(h.m_Vertices) and len(h.m_BoneIndices)==len(h.m_Vertices)
   meshes[k]['boneWeights']=[dict(zip(('weight0','weight1','weight2','weight3'),w),**dict(zip(('boneIndex0','boneIndex1','boneIndex2','boneIndex3'),map(int,b)))) for w,b in zip(h.m_BoneWeights,h.m_BoneIndices)]
  else:meshes[k]['boneWeights']=[]

 return k
def texture(o):
 if o is None:return ''
 k=key(o)
 if k not in textures:
  d=o.read();t=o.read_typetree();name=re.sub(r'[^\w.-]','_',d.m_Name)+'_'+k+'.png';d.image.save(root/'Textures'/name);textures[k]=name
  settings=t['m_TextureSettings'];texture_infos[k]={'id':k,'file':name,'name':t['m_Name'],'sourceFile':o.assets_file.name,'sourcePathID':str(o.path_id),'width':t['m_Width'],'height':t['m_Height'],'format':t['m_TextureFormat'],'colorSpace':t['m_ColorSpace'],'srgb':t['m_ColorSpace']==1,'mipCount':t['m_MipCount'],'filterMode':settings['m_FilterMode'],'aniso':settings['m_Aniso'],'mipBias':settings['m_MipBias'],'wrapU':settings['m_WrapU'],'wrapV':settings['m_WrapV'],'wrapW':settings['m_WrapW'],'lightmapFormat':t['m_LightmapFormat'],'isPreProcessed':t['m_IsPreProcessed']} 
 return textures[k]
def material(o):
 k=key(o)
 if k not in mats:
  t=o.read_typetree();s=t['m_SavedProperties'];te=dict(s['m_TexEnvs']);colors=dict(s['m_Colors']);floats=dict(s['m_Floats']);main=next((te[x] for x in ['_BaseColor','_MainTex','_BaseMap'] if x in te and te[x]['m_Texture']['m_PathID']),None);ao=te.get('_AOTex');tint=dict(colors.get('_BaseTint',colors.get('_Color',{'r':1,'g':1,'b':1,'a':1})));tint['a']=1
  shader=ref(o,t['m_Shader']);st=shader.read_typetree();shadername=st['m_ParsedForm']['m_Name'];source_shaders[shadername]=st['m_ParsedForm']
  source_materials[k]={'sourceFile':o.assets_file.name,'pathID':str(o.path_id),'tree':t}
  mats[k]={'id':k,'name':t['m_Name'],'texture':texture(ref(o,main['m_Texture'])) if main else '', 'ao':texture(ref(o,ao['m_Texture'])) if ao else '', 'tint':tint,'scale':main['m_Scale'] if main else {'x':1,'y':1},'offset':main['m_Offset'] if main else {'x':0,'y':0},
   'portedShader':('Goose/Source/BackShadow' if shadername=='BackShadow' else 'Goose/SourceItems/'+shadername+''.join(suffix for keyword,suffix in [('_CUSTOMINVERTLIGHTDIR_ON','Invert'),('_FADECOLORDIRY_ON','FadeY'),('_ISSINGLECOLOR_ON','Single'),('_ISUSEWORLDMATCAP_ON','World')] if keyword in t.get('m_ValidKeywords',[]))),'shader':shadername,'shaderSourceId':key(shader),'keywords':t.get('m_ValidKeywords',[]),'invalidKeywords':t.get('m_InvalidKeywords',[]),'renderQueue':t.get('m_CustomRenderQueue',-1),'enableInstancingVariants':t.get('m_EnableInstancingVariants',False),'doubleSidedGI':t.get('m_DoubleSidedGI',False),'lightmapFlags':t.get('m_LightmapFlags',0),'disabledShaderPasses':t.get('disabledShaderPasses',[]),'stringTagMap':[{'name':a,'value':b} for a,b in t.get('stringTagMap',[])],
   'floats':[{'name':n,'value':v} for n,v in s['m_Floats']],'ints':[{'name':n,'value':v} for n,v in s.get('m_Ints',[])],'vectors':[{'name':n,'value':v} for n,v in s['m_Colors']],'textures':[{'name':n,'texture':texture(ref(o,v['m_Texture'])),'scale':v['m_Scale'],'offset':v['m_Offset']} for n,v in s['m_TexEnvs']]}
 return k
def node(go):
 t=go.read_typetree();comps=[ref(go,p['component']) for p in t['m_Component']];tr=next(o for o in comps if o.type.name=='Transform');tt=tr.read_typetree()
 n={'transformId':str(tr.path_id),'name':t['m_Name'],'active':bool(t['m_IsActive']),'position':tt['m_LocalPosition'],'rotation':tt['m_LocalRotation'],'scale':tt['m_LocalScale'],'mesh':'','materials':[],'colliders':[],'children':[]}
 for c in comps:
  if c.type.name=='MeshFilter':n['mesh']=mesh(ref(c,c.read_typetree()['m_Mesh']))
  elif c.type.name in ['MeshRenderer','SkinnedMeshRenderer']:
   ct=c.read_typetree();n['castShadows']=ct['m_CastShadows'];n['receiveShadows']=bool(ct['m_ReceiveShadows']);n['rendererEnabled']=bool(ct['m_Enabled']);n['materials']=[material(ref(c,p)) for p in ct['m_Materials']]
   if c.type.name=='SkinnedMeshRenderer':
    n.update({'skinned':True,'mesh':mesh(ref(c,ct['m_Mesh'])),'boneIds':[str(p['m_PathID']) for p in ct['m_Bones']],'rootBoneId':str(ct['m_RootBone']['m_PathID']),'boundsCenter':ct['m_AABB']['m_Center'],'boundsExtents':ct['m_AABB']['m_Extent'],'skinQuality':ct['m_Quality'],'updateWhenOffscreen':ct['m_UpdateWhenOffscreen'],'skinnedMotionVectors':ct.get('m_SkinnedMotionVectors',True)})
  elif c.type.name in ['BoxCollider','SphereCollider','CapsuleCollider']:
   ct=c.read_typetree()
   if ct['m_Enabled'] and not ct['m_IsTrigger']:n['colliders'].append({'type':c.type.name,'center':ct['m_Center'],'size':ct.get('m_Size',{'x':0,'y':0,'z':0}),'radius':ct.get('m_Radius',0),'height':ct.get('m_Height',0),'direction':ct.get('m_Direction',1)})
 for p in tt['m_Children']:
  childtr=ref(tr,p);n['children'].append(node(ref(childtr,childtr.read_typetree()['m_GameObject'])))
 return n
models=[];errors=[]
for path,pp in sorted(e.container.items()):
 try:
  go=pp.deref();group=path.split('/LevelObjPrefab/')[-1].split('/')[0] if '/LevelObjPrefab/' in path else 'Legacy';models.append({'name':Path(path).stem,'group':group,'legacy':'/Old/' in path,'source':path,'bundle':str(go.assets_file.parent.name),'root':node(go)})
 except Exception as ex:errors.append({'source':path,'error':str(ex)})
data={'models':models,'meshes':list(meshes.values()),'materials':list(mats.values()),'textureInfos':list(texture_infos.values())}
(root/'models.json').write_text(json.dumps(data,ensure_ascii=False,separators=(',',':')))
(root/'provenance.json').write_text(json.dumps({'source':'goose-1.11.03.ipa','models':[{k:v for k,v in m.items() if k!='root'} for m in models],'errors':errors},ensure_ascii=False,indent=2))
print('models',len(models),'meshes',len(meshes),'materials',len(mats),'textures',len(textures),'errors',errors)

(evidence/'materials-source.json').write_text(json.dumps(source_materials,ensure_ascii=False,indent=2))
(evidence/'shader-parsed-ios.json').write_text(json.dumps(source_shaders,ensure_ascii=False,indent=2))
(evidence/'textures-source.json').write_text(json.dumps(list(texture_infos.values()),ensure_ascii=False,indent=2))
from collections import Counter
inventory=Counter(m['shader']+'|'+','.join(m['keywords']) for m in mats.values())
(evidence/'inventory.json').write_text(json.dumps({'source':'goose-1.11.03.ipa model-bundles; material settings are iOS source evidence, not asserted WX identity','modelCount':len(models),'meshCount':len(meshes),'materialCount':len(mats),'textureCount':len(textures),'shaderVariants':inventory,'tangentMeshes':sum(len(m['tangents'])==len(m['vertices']) and bool(m['vertices']) for m in meshes.values()),'errors':errors},ensure_ascii=False,indent=2))
print('inventory',inventory)
