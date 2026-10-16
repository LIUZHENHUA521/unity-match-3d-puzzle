import UnityPy,json,collections
from pathlib import Path
E=UnityPy.load('work/model-bundles');rows=[]
for path,pp in E.container.items():
 go=pp.deref()
 if go.type.name!='GameObject':continue
 def ref(o,p):return o.assets_file.objects[p['m_PathID']] if p['m_PathID'] and not p['m_FileID'] else None
 def walk(g,indices):
  d=g.read_typetree();cs=[ref(g,c['component']) for c in d['m_Component']];tr=next(c for c in cs if c.type.name=='Transform');td=tr.read_typetree()
  for c in cs:
   if c.type.name=='MeshRenderer':
    r=c.read_typetree();rows.append(dict(source=path,name=d['m_Name'],indices=indices,layer=d['m_Layer'],active=d['m_IsActive'],enabled=r['m_Enabled'],cast=r['m_CastShadows'],receive=r['m_ReceiveShadows']))
  for i,p in enumerate(td['m_Children']):ct=ref(tr,p);walk(ref(ct,ct.read_typetree()['m_GameObject']),indices+[i])
 walk(go,[])
Path('outputs/goose-analysis/shadow-evidence/item-renderer-flags-ios.json').write_text(json.dumps(rows,ensure_ascii=False,indent=2));print('renderers',len(rows),'prefabs',len(set(r['source'] for r in rows)));print(collections.Counter((r['cast'],r['receive']) for r in rows));print([r for r in rows if r['cast']!=1 or not r['receive']])
