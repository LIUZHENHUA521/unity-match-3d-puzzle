"""Export only the original Cocos certificate's referenced atlas frames for Unity."""
import json, pathlib, plistlib, re
from PIL import Image
root=pathlib.Path('/Users/liuzhenhua/Desktop/code/crazyCombo/client/assets/resources')
out=pathlib.Path(__file__).resolve().parents[2]/'Assets/Match3D/Resources/Match3D/PotCertificate'
out.mkdir(parents=True,exist_ok=True)
manifest=[]
for atlas,names in [('newSpriteAsset/newCertificate',['bg_diploma_02','Certificatetitle_02','CertificateIcon_wings','CertificateIcon_03','CertificateIcon_07']),('images/Certificate',[f'icon_yinzhang_{i:02}' for i in range(1,7)]),('images/0314',[f'yz_{i:02}' for i in range(1,7)]),('newSpriteAsset/newhomeIcons',[])]:
 meta=json.loads((root/(atlas+'.plist.meta')).read_text())
 if not names:names=[meta['subMetas']['994b4']['name']]
 frames=plistlib.loads((root/(atlas+'.plist')).read_bytes())['frames'];im=Image.open(root/(atlas+'.png')).convert('RGBA')
 for name in names:
  frame=frames.get(name,frames.get(name+'.png'));nums=lambda s:list(map(int,re.findall(r'-?\d+',s)))
  x,y,w,h=nums(frame['textureRect']);rot=frame['textureRotated']
  pic=im.crop((x,y,x+(h if rot else w),y+(w if rot else h)))
  if rot:pic=pic.transpose(Image.Transpose.ROTATE_90)
  ow,oh=nums(frame['spriteSourceSize']);dx,dy=nums(frame['spriteOffset']);canvas=Image.new('RGBA',(ow,oh));canvas.paste(pic,((ow-pic.width)//2+dx,(oh-pic.height)//2-dy))
  dest='pot-'+('button' if atlas.endswith('newhomeIcons') else name.lower())+'.png';canvas.save(out/dest)
  manifest.append({'file':dest,'atlas':atlas,'frame':name})
(out/'pot-sources.json').write_text(json.dumps(manifest,ensure_ascii=False,indent=2)+'\n')
print('Exported',len(manifest),'original atlas frames')
