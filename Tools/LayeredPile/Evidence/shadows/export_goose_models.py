import UnityPy,json,hashlib,re
from pathlib import Path
from UnityPy.helpers.MeshHelper import MeshHandler
root=Path('/Users/liuzhenhua/Desktop/code/unity-match-3d-puzzle/Assets/GooseReplica/OriginalModels');root.mkdir(parents=True,exist_ok=True);(root/'Textures').mkdir(exist_ok=True)
e=UnityPy.load('work/model-bundles');meshes={};mats={};textures={}
def key(o):return hashlib.sha1((o.assets_file.name+':'+str(o.path_id)).encode()).hexdigest()[:16]
def ref(o,p):
 if not p or not p.get('m_PathID'):return None
 if p['m_FileID']:raise ValueError('external dependency '+str(p))
 return o.assets_file.objects[p['m_PathID']]
def mesh(o):
 k=key(o)
 if k not in meshes:
  d=o.read();h=MeshHandler(d);h.process()
  meshes[k]={'id':k,'name':d.m_Name,'vertices':[dict(zip(('x','y','z'),v[:3])) for v in h.m_Vertices],'normals':[dict(zip(('x','y','z'),v[:3])) for v in (h.m_Normals or [])],'uv':[dict(zip(('x','y'),v[:2])) for v in (h.m_UV0 or [])],'submeshes':[{'indices':[i for tri in group for i in tri]} for group in h.get_triangles()]}
 return k
def texture(o):
 if o is None:return ''
 k=key(o)
 if k not in textures:
  d=o.read();name=re.sub(r'[^\w.-]','_',d.m_Name)+'_'+k+'.png';d.image.save(root/'Textures'/name);textures[k]=name
 return textures[k]
def material(o):
 k=key(o)
 if k not in mats:
  t=o.read_typetree();s=t['m_SavedProperties'];te=dict(s['m_TexEnvs']);colors=dict(s['m_Colors']);floats=dict(s['m_Floats']);main=next((te[x] for x in ['_BaseColor','_MainTex','_BaseMap'] if x in te and te[x]['m_Texture']['m_PathID']),None);ao=te.get('_AOTex');tint=colors.get('_BaseTint',colors.get('_Color',{'r':1,'g':1,'b':1,'a':1}));tint['a']=1
  mats[k]={'id':k,'name':t['m_Name'],'texture':texture(ref(o,main['m_Texture'])) if main else '', 'ao':texture(ref(o,ao['m_Texture'])) if ao else '', 'tint':tint,'scale':main['m_Scale'] if main else {'x':1,'y':1},'offset':main['m_Offset'] if main else {'x':0,'y':0}}
 return k
def node(go):
 t=go.read_typetree();comps=[ref(go,p['component']) for p in t['m_Component']];tr=next(o for o in comps if o.type.name=='Transform');tt=tr.read_typetree()
 n={'name':t['m_Name'],'active':bool(t['m_IsActive']),'position':tt['m_LocalPosition'],'rotation':tt['m_LocalRotation'],'scale':tt['m_LocalScale'],'mesh':'','materials':[],'colliders':[],'children':[]}
 for c in comps:
  if c.type.name=='MeshFilter':n['mesh']=mesh(ref(c,c.read_typetree()['m_Mesh']))
  elif c.type.name=='MeshRenderer':
   ct=c.read_typetree();n['castShadows']=ct['m_CastShadows'];n['receiveShadows']=bool(ct['m_ReceiveShadows']);n['rendererEnabled']=bool(ct['m_Enabled']);n['materials']=[material(ref(c,p)) for p in ct['m_Materials']]
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
data={'models':models,'meshes':list(meshes.values()),'materials':list(mats.values())}
(root/'models.json').write_text(json.dumps(data,ensure_ascii=False,separators=(',',':')))
(root/'provenance.json').write_text(json.dumps({'source':'goose-1.11.03.ipa','models':[{k:v for k,v in m.items() if k!='root'} for m in models],'errors':errors},ensure_ascii=False,indent=2))
print('models',len(models),'meshes',len(meshes),'materials',len(mats),'textures',len(textures),'errors',errors)
