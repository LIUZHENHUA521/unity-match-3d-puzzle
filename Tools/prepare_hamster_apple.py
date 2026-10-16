"""Build a compact hamster_apple source for Unity. Requires numpy, pillow, meshoptimizer.
Usage: python Tools/prepare_hamster_apple.py input.glb output.glb
Original GLB is never modified. Unity source uses centered .43-unit bounds.
"""
import io,json,struct,sys,ctypes
from pathlib import Path
import numpy as np
import meshoptimizer
from PIL import Image
blob=Path(sys.argv[1]).read_bytes(); size=struct.unpack_from('<I',blob,12)[0]
g=json.loads(blob[20:20+size]); off=20+size; data=blob[off+8:]
assert len(g['meshes'])==1 and len(g['meshes'][0]['primitives'])==1
assert g['nodes'][0].get('matrix',np.eye(4).flatten().tolist())==np.eye(4).flatten().tolist()
p=g['meshes'][0]['primitives'][0]
def read(i):
 a=g['accessors'][i]; v=g['bufferViews'][a['bufferView']]; dt={5126:'<f4',5125:'<u4',5123:'<u2'}[a['componentType']]; n={'VEC3':3,'VEC2':2,'SCALAR':1}[a['type']]
 return np.ndarray((a['count'],n),dtype=dt,buffer=data,offset=v.get('byteOffset',0)+a.get('byteOffset',0),strides=(v.get('byteStride',np.dtype(dt).itemsize*n),np.dtype(dt).itemsize)).copy()
pos=read(p['attributes']['POSITION']); norm=read(p['attributes']['NORMAL']); uv=read(p['attributes']['TEXCOORD_0']); indices=read(p['indices']).flatten().astype(np.uint32); original_faces=len(indices)//3; print('original triangles',original_faces)
reduced=np.empty_like(indices)
count=meshoptimizer.simplify_with_attributes(reduced,indices,pos,uv,np.array([1.,1.],dtype=np.float32),target_index_count=9000,target_error=ctypes.c_float(.05))
indices=reduced[:count]; used,indices=np.unique(indices,return_inverse=True); pos=pos[used];norm=norm[used];uv=uv[used]; indices=indices.astype(np.uint32)
image=g['images'][g['textures'][g['materials'][0]['pbrMetallicRoughness']['baseColorTexture']['index']]['source']]; v=g['bufferViews'][image['bufferView']]
im=Image.open(io.BytesIO(data[v.get('byteOffset',0):v.get('byteOffset',0)+v['byteLength']])).convert('RGB'); print('original texture',im.size); im.thumbnail((1024,1024))
root=Path(__file__).resolve().parents[1]/'Assets/GooseReplica/Editor/MeshySource';root.mkdir(parents=True,exist_ok=True)
im.save(root/'hamster_apple.png',optimize=True)
center=(pos.min(0)+pos.max(0))/2; scale=.43/np.ptp(pos,axis=0).max(); unity=(pos-center)*scale;unity[:,2]*=-1; unorm=norm.copy();unorm[:,2]*=-1
tri=indices.reshape(-1,3)[:,[0,2,1]].flatten()
def vectors(a,keys):return [dict(zip(keys,map(float,row))) for row in a]
(root/'hamster_apple.json').write_text(json.dumps(dict(name='hamster_apple',texture='hamster_apple.png',vertices=vectors(unity,'xyz'),normals=vectors(unorm,'xyz'),uv=vectors(uv,'xy'),triangles=tri.tolist()),separators=(',',':')))
# Export the optimized glTF as a portable, textured review asset.
buf=bytearray();views=[];access=[]
def append(raw):
 while len(buf)%4:buf.append(0)
 offset=len(buf);buf.extend(raw);views.append(dict(buffer=0,byteOffset=offset,byteLength=len(raw)));return len(views)-1
def acc(a,t,ctype):
 vi=append(a.tobytes()); d=dict(bufferView=vi,componentType=ctype,count=len(a),type=t)
 if t=='VEC3':d.update(min=a.min(0).tolist(),max=a.max(0).tolist())
 access.append(d);return len(access)-1
pi=acc(pos,'VEC3',5126);ni=acc(norm,'VEC3',5126);ui=acc(uv,'VEC2',5126);ii=acc(indices,'SCALAR',5125)
out=io.BytesIO();im.save(out,format='JPEG',quality=88); iv=append(out.getvalue())
j=dict(asset={'version':'2.0'},scene=0,scenes=[{'nodes':[0]}],nodes=[{'mesh':0}],meshes=[{'primitives':[{'attributes':{'POSITION':pi,'NORMAL':ni,'TEXCOORD_0':ui},'indices':ii,'material':0}]}],materials=[{'pbrMetallicRoughness':{'baseColorTexture':{'index':0},'metallicFactor':0,'roughnessFactor':.8}}],textures=[{'source':0}],images=[{'bufferView':iv,'mimeType':'image/jpeg'}],accessors=access,bufferViews=views,buffers=[{'byteLength':len(buf)}])
js=json.dumps(j,separators=(',',':')).encode();js+=b' '*((-len(js))%4);buf+=b'\0'*((-len(buf))%4)
glb=struct.pack('<III',0x46546c67,2,28+len(js)+len(buf))+struct.pack('<II',len(js),0x4e4f534a)+js+struct.pack('<II',len(buf),0x004e4942)+buf
Path(sys.argv[2]).write_bytes(glb);
Path(sys.argv[2]).with_suffix('.stats.json').write_text(json.dumps({'original_triangles':original_faces,'optimized_triangles':count//3,'vertices':len(pos),'texture_size':list(im.size),'unity_max_dimension':.43},indent=2));print('triangles',count//3,'vertices',len(pos),'GLB bytes',len(glb))
