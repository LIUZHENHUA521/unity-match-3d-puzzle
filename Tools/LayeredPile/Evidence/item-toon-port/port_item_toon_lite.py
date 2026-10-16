"""Port matching iOS item vertex/fragment, using exact local variants, not scene WX approximations."""
from pathlib import Path
import json,re,ast,hashlib
# Share lexical transforms with the independently ported special-material implementation.
helper=ast.parse(Path('work/port_special_item_shaders.py').read_text())
exec(compile(ast.Module(body=[n for n in helper.body if isinstance(n,ast.FunctionDef) and n.name in ['vector_casts','cv','ststruct','getbody','decl','props']],type_ignores=[]),'special lexical helpers','exec'))
builtins={'_WorldSpaceCameraPos','unity_OrthoParams','_WorldSpaceLightPos0','_LightColor0','unity_WorldTransformParams','_Time'}
root=Path('/Users/liuzhenhua/Desktop/code/unity-match-3d-puzzle');out=root/'Assets/GooseReplica/Shaders/SourceItems';out.mkdir(exist_ok=True)
evidence=Path('outputs/goose-analysis/item-material-evidence');data=json.loads((root/'Assets/GooseReplica/OriginalModels/models.json').read_text());seen={};records=[];variant_audit=[]
for mat in data['materials']:
 if mat['shader'] not in ['ToonLite1','ToonLite2']:continue
 ident=mat['portedShader'].split('/')[-1];src=evidence/'ios-exact-programs'/(mat['shader']+'-'+mat['shaderSourceId']);parsed=json.loads((src/'parsed.json').read_text());variants=json.loads((src/'variants.json').read_text());kws=set(mat['keywords'])|{'DIRECTIONAL'}
 def source(stage,pi):
  rows=[v for v in variants if v['pass']==pi and v['stage']==stage and set(v['keywords'])==kws];assert rows,(ident,stage,pi,kws)
  index=rows[0]['index'];return (src/(str(index)+'.metal')).read_text(),index
 digest=tuple(hashlib.sha256(source(stage,pi)[0].encode()).hexdigest() for pi in [0,1] for stage in ['progVertex','progFragment'])
 variant_audit.append(dict(material=mat['name'],materialId=mat['id'],shader=ident,sourceId=mat['shaderSourceId'],programHashes=list(digest)))
 if ident in seen:assert seen[ident]==digest,(ident,mat['name'],'vertex/base/add program mismatch');continue
 seen[ident]=digest;passes=[]
 for pi in [0,1]:
  vs,vi=source('progVertex',pi);fs,fi=source('progFragment',pi);state=parsed['m_SubShaders'][0]['m_Passes'][pi]['m_State'];base=pi==0
  globals=list(dict.fromkeys(decl(vs,'V')+decl(fs,'F')))
  globals+=['sampler2D '+t+';' for t in re.findall(r'texture2d<[^>]+>\s+(\w+)\s+\[\[',fs)]
  bodyv=cv(getbody(vs)).replace('Mtl_VertexOut','v2f');bodyf=cv(getbody(fs)).replace('Mtl_FragmentOut output;','float4 result;').replace('output.SV_Target0','result').replace('return output;','return result;')
  structout=ststruct(vs,'Mtl_VertexOut','v2f')
  if base:
   structout=structout.replace('};',' SHADOW_COORDS(8) };')
   bodyv=bodyv.replace('return output;', '''
#if defined(SHADOWS_SCREEN)
#if defined(UNITY_NO_SCREENSPACE_SHADOWS)
output._ShadowCoord=mul(unity_WorldToShadow[0],mul(unity_ObjectToWorld,input.POSITION0));
#else
output._ShadowCoord=ComputeScreenPos(output.mtl_Position);
#endif
#endif
return output;''')
   bodyf='UNITY_LIGHT_ATTENUATION(sourceAttenuation,input,float3(input.TEXCOORD1.w,input.TEXCOORD2.w,input.TEXCOORD3.w));\n'+bodyf
   bodyf,n=re.subn(r'(u_xlat[\w.]+) = \(u_xlatb\d+\) \? 0.0 : \1;',lambda m:m.group(0)+'\n'+m.group(1)+' *= sourceAttenuation;',bodyf);assert n==1,ident
   globals+=['float _SelectionGlow, _SelectionPulse;']
   bodyf=bodyf.replace('return result;', '''// Preserve the project's subtle press feedback; separate from source material math.
 float3 sn=normalize(float3(input.TEXCOORD1.z,input.TEXCOORD2.z,input.TEXCOORD3.z));
 float3 sv=normalize(_WorldSpaceCameraPos-float3(input.TEXCOORD1.w,input.TEXCOORD2.w,input.TEXCOORD3.w));
 float sr=pow(1-saturate(dot(sn,sv)),2);
 result.rgb+=_SelectionGlow*lerp(.35,1,_SelectionPulse)*float3(1,.93,.78)*(.025+sr*.12);
 return result;''')
  blend=state['rtBlend0'];bn={0:'Zero',1:'One',5:'SrcAlpha',10:'OneMinusSrcAlpha'}
  passes.append('''Pass { Name "FORWARD_%s" Tags {"LightMode"="%s"}
Cull %s ZWrite %s ZTest LEqual Blend %s %s ColorMask %s
CGPROGRAM
#pragma target 3.0
#pragma vertex vert
#pragma fragment frag
%s
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"
%s
%s
%s
v2f vert(appdata input){%s}
float4 frag(v2f input):SV_Target {%s}
ENDCG
}'''%('BASE' if base else 'ADD','ForwardBase' if base else 'ForwardAdd',['Off','Front','Back'][int(state['culling']['val'])],'On' if state['zWrite']['val'] else 'Off',bn[int(blend['srcBlend']['val'])],bn[int(blend['destBlend']['val'])],'RGB' if blend['colMask']['val']==14 else 'RGBA','#pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight' if base else '#pragma multi_compile DIRECTIONAL','\n'.join(globals),ststruct(vs,'Mtl_VertexIn','appdata'),structout,bodyv,bodyf))
  records.append(dict(shader=ident,source=str(src),passIndex=pi,vertexBlob=vi,fragmentBlob=fi,keywords=sorted(kws),fragmentSha256=hashlib.sha256(fs.encode()).hexdigest()))
 properties=props({'properties':parsed['m_PropInfo']}).replace(',Color)',',Vector)')
 text='// Mechanical port of matching iOS 1.11.03 item Metal; selection overlay is project compatibility.\nShader "'+mat['portedShader']+'" { Properties {\n'+properties+'\n}\nSubShader { Tags {"RenderType"="Opaque" "Queue"="Geometry"}\nUsePass "Goose/Original Item/SELECTION_OUTLINE"\n'+'\n'.join(passes)+'\nUsePass "Goose/Source/ToonLite2/ShadowCaster"\n}\n}\n'
 (out/(ident+'.shader')).write_text(text)
 print('ported',ident)
(evidence/'ported-toon-lite-programs.json').write_text(json.dumps(records,indent=2))

(evidence/'toon-lite-cross-bundle-audit.json').write_text(json.dumps(variant_audit,indent=2));print('cross-bundle verified materials',len(variant_audit),'variants',len(seen))
