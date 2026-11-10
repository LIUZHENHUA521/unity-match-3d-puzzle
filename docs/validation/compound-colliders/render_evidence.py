"""Render orthographic mesh silhouettes and baked primitive outlines from Unity evidence."""
import json, sys
from pathlib import Path
import numpy as np
import matplotlib
matplotlib.use('Agg')
import matplotlib.pyplot as plt
from matplotlib.collections import PolyCollection
from matplotlib.patches import Polygon, Rectangle
from PIL import Image, ImageDraw

source=Path(sys.argv[1]); dest=Path(sys.argv[2]); dest.mkdir(parents=True, exist_ok=True)
def vec(v): return np.array([v['x'],v['y'],v['z']])
def rotation(q):
 x,y,z,w=(q[k] for k in 'xyzw')
 return np.array([[1-2*(y*y+z*z),2*(x*y-z*w),2*(x*z+y*w)], [2*(x*y+z*w),1-2*(x*x+z*z),2*(y*z-x*w)], [2*(x*z-y*w),2*(y*z+x*w),1-2*(x*x+y*y)]])
def hull(points):
 pts=sorted(set(map(tuple,points)))
 def cross(o,a,b):return (a[0]-o[0])*(b[1]-o[1])-(a[1]-o[1])*(b[0]-o[0])
 lower=[];upper=[]
 for p in pts:
  while len(lower)>=2 and cross(lower[-2],lower[-1],p)<=0:lower.pop()
  lower.append(p)
 for p in reversed(pts):
  while len(upper)>=2 and cross(upper[-2],upper[-1],p)<=0:upper.pop()
  upper.append(p)
 return lower[:-1]+upper[:-1]
rows=[]
for path in sorted(source.glob('*.json')):
 d=json.loads(path.read_text())
 if 'meshTriangles' not in d:continue
 vertices=np.array([vec(v) for v in d['meshTriangles']]); tris=vertices.reshape(-1,3,3)
 low=vertices.min(0);high=vertices.max(0);mid=(low+high)/2;extent=(high-low).max()*.60
 fig, axes=plt.subplots(1,3,figsize=(9,3.1),facecolor='#17212b')
 for ax, dims, name in zip(axes,[(0,1),(0,2),(2,1)],['XY','XZ','ZY']):
  ax.set_facecolor('#17212b');ax.set_aspect('equal');ax.axis('off')
  ax.add_collection(PolyCollection(tris[:,:,dims],facecolors='#668eac',edgecolors='none'))
  ax.add_patch(Rectangle(low[list(dims)],*(high-low)[list(dims)],fill=False,edgecolor='#ffb65d',linewidth=.9,linestyle='--'))
  for shape in d['shapes']:
   r=rotation(shape['rotation']); c=vec(shape['center'])
   if shape['type']=='BoxCollider':
    pts=np.array([[x,y,z] for x in [-.5,.5] for y in [-.5,.5] for z in [-.5,.5]])*vec(shape['size']);pts=(pts@r.T+c)[:,dims]
   else:
    offset=np.zeros(3)
    if shape['type']=='CapsuleCollider':offset[shape['direction']]=max(0,shape['height']/2-shape['radius'])
    ends=np.array([c+r@offset,c-r@offset])[:,dims]
    theta=np.linspace(0,2*np.pi,64,endpoint=False);circle=np.column_stack([np.cos(theta),np.sin(theta)])*shape['radius']
    pts=np.concatenate([circle+end for end in ends])
   ax.add_patch(Polygon(hull(pts),closed=True,fill=False,edgecolor='#8fffac',linewidth=.85))
  ax.set_xlim(mid[dims[0]]-extent,mid[dims[0]]+extent);ax.set_ylim(mid[dims[1]]-extent,mid[dims[1]]+extent)
  ax.set_title(name,color='#b8c8d8',fontsize=8)
 ratio=d['fittedVolume']/d['oldBoxVolume'];fig.suptitle(f"{d['id']} | {len(d['shapes'])} shapes | volume sum / old box {ratio:.2f}",color='white',fontsize=10)
 fig.subplots_adjust(left=.02,right=.98,bottom=.03,top=.81,wspace=.05);fig.savefig(dest/(d['id']+'.png'),dpi=120);plt.close(fig)
 rows.append({'id':d['id'],'count':len(d['shapes']),'volumeRatio':ratio,'types':[s['type'] for s in d['shapes']]})
(dest/'summary.json').write_text(json.dumps(rows,indent=2))
for name, ids in [('all-items',[r['id'] for r in rows]),('examples',['ToyBasketBall','ToyDuck','ToyDinosaur','ToyPlane','ToyHelicopter','ToyBear','Kitchen_soda-glass','Party_Popsicle'])]:
 ids=[i for i in ids if (dest/(i+'.png')).exists()];tile=(720,248);canvas=Image.new('RGB',(tile[0]*2,tile[1]*((len(ids)+1)//2)), '#17212b')
 for n,id in enumerate(ids):
  im=Image.open(dest/(id+'.png')).convert('RGB').resize(tile);canvas.paste(im,((n%2)*tile[0],(n//2)*tile[1]))
 canvas.save(dest/(name+'.png'))
print('Rendered',len(rows),'items;',sum(r['count'] for r in rows),'shapes; mean volume ratio',np.mean([r['volumeRatio'] for r in rows]) if rows else 0)
