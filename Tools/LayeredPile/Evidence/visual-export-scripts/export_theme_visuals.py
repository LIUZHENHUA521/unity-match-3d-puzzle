import UnityPy,json,hashlib,re
from pathlib import Path
from UnityPy.helpers.MeshHelper import MeshHandler
root=Path('/Users/liuzhenhua/Desktop/code/unity-match-3d-puzzle/Assets/GooseReplica/SourceScenes');root.mkdir(exist_ok=True);(root/'Textures').mkdir(exist_ok=True)
meshes={};materials={};sprites={};textures={};records=[]
def key(o):return hashlib.sha1((o.assets_file.name+':'+str(o.path_id)).encode()).hexdigest()[:16]
def ref(o,p):
 if not p['m_PathID']:return None
 if p['m_FileID']:raise ValueError('external '+str(p))
 return o.assets_file.objects[p['m_PathID']]
def tex(o):
 if o is None:return ''
 k=key(o)
 if k not in textures:o.read().image.save(root/'Textures'/(k+'.png'));textures[k]=True
 return k+'.png'
def mat(o):
 k=key(o)
 if k in materials:return k
 d=o.read_typetree();props=d['m_SavedProperties'];te=dict(props['m_TexEnvs']);colors=dict(props['m_Colors']);floats=dict(props['m_Floats'])
 main=next((te[n] for n in ['_BaseColor','_MainTex','_BaseMap'] if n in te and te[n]['m_Texture']['m_PathID']),None)
 ao=te.get('_AOTex');em=te.get('_EmissiveTex');col=colors.get('_BaseTint',colors.get('_Color',dict(r=1,g=1,b=1,a=1)));col['a']=1
 shader=ref(o,d['m_Shader']) if not d['m_Shader']['m_FileID'] else None;shadername=shader.read_typetree()['m_ParsedForm']['m_Name'] if shader else 'Fallback'
 materials[k]={'shader':shadername+('World' if '_ISUSEWORLDMATCAP_ON' in d.get('m_ValidKeywords',[]) else ''),'floats':[dict(name=n,value=v) for n,v in floats.items()],'vectors':[dict(name=n,value=v) for n,v in colors.items()],'textures':[dict(name=n,texture=tex(ref(o,v['m_Texture'])),scale=v['m_Scale'],offset=v['m_Offset']) for n,v in te.items()],'id':k,'name':d['m_Name'],'texture':tex(ref(o,main['m_Texture'])) if main else '', 'ao':tex(ref(o,ao['m_Texture'])) if ao else '', 'emission':tex(ref(o,em['m_Texture'])) if em else '', 'tint':col,'scale':main['m_Scale'] if main else dict(x=1,y=1),'offset':main['m_Offset'] if main else dict(x=0,y=0),'aoScale':floats.get('_AOScale',1)}
 return k
def mesh(o,p):
 if p['m_FileID']==1:
  i=p['m_PathID'];assert i in [10202,10209,10210],i;k='builtin'+str(i);meshes[k]={'id':k,'builtin':i};return k
 m=ref(o,p);k=key(m)
 if k not in meshes:
  h=MeshHandler(m.read());h.process();meshes[k]={'id':k,'name':m.read().m_Name,'vertices':[dict(zip('xyz',v[:3])) for v in h.m_Vertices],'tangents':[dict(zip('xyzw',v[:4])) for v in h.m_Tangents or []],'normals':[dict(zip('xyz',v[:3])) for v in h.m_Normals or []],'uv':[dict(zip('xy',v[:2])) for v in h.m_UV0 or []],'submeshes':[{'indices':[i for t in group for i in t]} for group in h.get_triangles()]}
 return k
def sprite(o):
 k=key(o)
 if k not in sprites:
  d=o.read();d.image.save(root/'Textures'/(k+'.png'));sprites[k]={'id':k,'texture':k+'.png','ppu':d.m_PixelsToUnits,'pivot':{'x':d.m_Pivot.x,'y':d.m_Pivot.y}}
 return k
for filename,name,sub in [('countryside','CountrysideStove','countrysidestovesub'),('grocery-scene','GroceryStore',None),('barbecue','Barbecue','barbecuesub')]:
 E=UnityPy.load('work/wechat-'+filename+'.bundle');go=next(p.deref() for k,p in E.container.items() if k.endswith('/'+name+'.prefab'));F=go.assets_file.objects;gd=go.read_typetree();bg=next(F[c['component']['m_PathID']].read_typetree() for c in gd['m_Component'] if F[c['component']['m_PathID']].type.name=='MonoBehaviour');nodes=[];subparent=bg['subObjParTrans']['m_PathID'];excluded={bg[x]['m_PathID'] for x in ['level1Obj','level2Obj','customCol']};attach=-1
 def walk(g,parent):
  global attach
  d=g.read_typetree();cs=[g.assets_file.objects[c['component']['m_PathID']] for c in d['m_Component']];tr=next(c for c in cs if c.type.name=='Transform');t=tr.read_typetree()
  if g.path_id in excluded:return
  n={'name':d['m_Name'],'active':bool(d['m_IsActive']),'parent':parent,'position':t['m_LocalPosition'],'rotation':t['m_LocalRotation'],'scale':t['m_LocalScale'],'mesh':'','materials':[],'sprite':'','enabled':False};idx=len(nodes);nodes.append(n)
  if tr.path_id==subparent:attach=idx
  r=next((c for c in cs if c.type.name=='MeshRenderer'),None);f=next((c for c in cs if c.type.name=='MeshFilter'),None)
  if r and f:
   rd=r.read_typetree()
   if rd['m_Enabled']:
    n['mesh']=mesh(f,f.read_typetree()['m_Mesh']);n['materials']=[mat(ref(r,p)) for p in rd['m_Materials']];n['enabled']=True
  sr=next((c for c in cs if c.type.name=='SpriteRenderer'),None)
  if sr:
   sd=sr.read_typetree()
   if sd['m_Enabled'] and sd['m_Sprite']['m_PathID']:n['sprite']=sprite(ref(sr,sd['m_Sprite']));n['enabled']=True;n['color']=sd['m_Color'];n['sortingOrder']=sd['m_SortingOrder']
  for p in t['m_Children']:
   ct=ref(tr,p);walk(ref(ct,ct.read_typetree()['m_GameObject']),idx)
 walk(go,-1)
 if sub:
  S=UnityPy.load('work/wechat-'+sub+'.bundle');sg=next(iter(S.container.values())).deref();assert attach>=0;walk(sg,attach)
 records.append({'name':name,'nodes':nodes});print(name,len(nodes),'meshes',sum(bool(n['mesh']) for n in nodes),'sprites',sum(bool(n['sprite']) for n in nodes))
(root/'visuals.json').write_text(json.dumps({'scenes':records,'meshes':list(meshes.values()),'materials':list(materials.values()),'sprites':list(sprites.values())},ensure_ascii=False,separators=(',',':')))
print('exported',len(meshes),'meshes',len(materials),'materials')
