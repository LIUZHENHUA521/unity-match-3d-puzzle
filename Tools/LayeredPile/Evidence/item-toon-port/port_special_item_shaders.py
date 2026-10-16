"""Mechanical iOS MSL -> Unity Cg port of shipped special item variants.
No BRDF tuning; source pass states retained. Selection overlay is project's compatibility addition.
"""
from pathlib import Path
import json,re
src=Path('outputs/goose-analysis/item-toon-evidence/metal')
out=Path('/Users/liuzhenhua/Desktop/code/unity-match-3d-puzzle/Assets/GooseReplica/Shaders/SourceItems');out.mkdir(exist_ok=True)
builtins={'_WorldSpaceCameraPos','unity_OrthoParams','_WorldSpaceLightPos0','_LightColor0','unity_WorldTransformParams','_Time'}
def vector_casts(s):
 """Metal permits float3(scalar) splats; Cg needs an explicit vector cast.
 Walk balanced parentheses so commas in nested calls do not change arity.
 """
 result=[];cursor=0
 for match in re.finditer(r'\b((?:float|half)[234])\s*\(',s):
  if match.start()<cursor:continue
  begin=match.end();depth=1;end=begin;commas=0
  while depth:
   char=s[end]
   if char=='(':depth+=1
   elif char==')':depth-=1
   elif char==',' and depth==1:commas+=1
   end+=1
  inner=vector_casts(s[begin:end-1])
  result.append(s[cursor:match.start()])
  result.append(('('+match.group(1)+')('+inner+')') if not commas else match.group(1)+'('+inner+')')
  cursor=end
 result.append(s[cursor:]);return ''.join(result)

def cv(s):
 s=s.replace('FGlobals.','').replace('VGlobals.','').replace('fma(','mad(')
 for name,m in {'unity_ObjectToWorld':'unity_ObjectToWorld','unity_WorldToObject':'unity_WorldToObject','unity_MatrixVP':'UNITY_MATRIX_VP','unity_MatrixV':'UNITY_MATRIX_V','unity_MatrixInvV':'UNITY_MATRIX_I_V'}.items():
  s=re.sub(r'hlslcc_mtx4x4'+name+r'\[(\d)\]',r'transpose('+m+r')[\1]',s)
 s=re.sub(r'(\w+)\.sample\(sampler_\w+, ([^\)]+)\)',r'tex2D(\1, \2)',s)
 return vector_casts(s)

def ststruct(s,name,new):
 b=re.search(r'struct '+name+r'\s*\{(.*?)\};',s,re.S).group(1)
 lines=[]
 for typ,n,annot in re.findall(r'(\w+)\s+(\w+)\s*\[\[(.*?)\]\]',b):
  semantic='SV_POSITION' if n=='mtl_Position' else 'POSITION' if n=='POSITION0' else 'NORMAL' if n=='NORMAL0' else 'TANGENT' if n=='TANGENT0' else n
  lines.append(f'{typ} {n}:{semantic};')
 return 'struct '+new+' {'+''.join(lines)+'};'

def getbody(s):return s[s.index('{',s.index('xlatMtlMain('))+1:s.rindex('}')]
def decl(s,kind):
 b=re.search(r'struct '+kind+r'Globals_Type\s*\{(.*?)\};',s,re.S).group(1)
 return [line.strip() for line in b.splitlines() if line.strip() and not any(x in line for x in ['hlslcc_']) and line.strip().split()[1].rstrip(';') not in builtins]

def props(d):
 lines=[]
 for p in d['properties']['m_Props']:
  n=p['m_Name'];t=p['m_Type'];v=[p['m_DefValue['+str(i)+']'] for i in range(4)]
  if t==4: val='2D)="'+p['m_DefTexture']['m_DefaultName']+'"{}'
  elif t in [0,1]: val=('Color' if t==0 else 'Vector')+')=('+','.join(map(str,v))+')'
  else:val='Float)='+str(v[0])
  lines.append(n+'("'+n+'",'+val)
 lines+=['_SelectionGlow("Project selection glow",Float)=0','_SelectionPulse("Project selection pulse",Float)=1','_OutlinePixels("Project selection outline",Float)=1.2']
 return '\n'.join(lines)

def renderpass(name,pi,kws,overlay):
 prefix=f'{name}-{pi}-';key='_'.join(kws)
 vs=(src/(prefix+'Vertex-'+key+'.metal')).read_text();fs=(src/(prefix+'Fragment-'+key+'.metal')).read_text();d=json.loads((src/(prefix+'Fragment-'+key+'.json')).read_text());state=d['state'];tags=dict(state['m_Tags']['tags']);lm=tags['LIGHTMODE'];shadow=name=='ToonEffectFull' and pi==1
 globals=list(dict.fromkeys(decl(vs,'V')+decl(fs,'F')))
 tex=re.findall(r'texture2d<[^>]+>\s+(\w+)\s+\[\[',fs)
 globals+=['sampler2D '+t+';' for t in tex]
 bodyv=cv(getbody(vs)).replace('Mtl_VertexOut','v2f')
 bodyf=cv(getbody(fs)).replace('Mtl_FragmentOut output;','float4 result;').replace('output.SV_Target0','result').replace('return output;','return result;')
 structout=ststruct(vs,'Mtl_VertexOut','v2f')
 if shadow:
  structout=structout.replace('};',' SHADOW_COORDS(8) };')
  bodyv=bodyv.replace('return output;',"""
#if defined(SHADOWS_SCREEN)
#if defined(UNITY_NO_SCREENSPACE_SHADOWS)
output._ShadowCoord=mul(unity_WorldToShadow[0],mul(unity_ObjectToWorld,input.POSITION0));
#else
output._ShadowCoord=ComputeScreenPos(output.mtl_Position);
#endif
#endif
return output;""")
  bodyf='UNITY_LIGHT_ATTENUATION(sourceAttenuation,input,float3(input.TEXCOORD1.w,input.TEXCOORD2.w,input.TEXCOORD3.w));\n'+bodyf
  bodyf,n=re.subn(r'(u_xlat[\w.]+) = \(u_xlatb\d+\) \? 0.0 : \1;',lambda m:m.group(0)+'\n'+m.group(1)+' *= sourceAttenuation;',bodyf);assert n==1
 if overlay:
  globals+=['float _SelectionGlow, _SelectionPulse;']
  weak='''float3 sn=normalize(float3(input.TEXCOORD1.z,input.TEXCOORD2.z,input.TEXCOORD3.z));
 float3 sv=normalize(_WorldSpaceCameraPos-float3(input.TEXCOORD1.w,input.TEXCOORD2.w,input.TEXCOORD3.w));
 float sr=pow(1-saturate(dot(sn,sv)),2);
 result.rgb+=_SelectionGlow*lerp(.35,1,_SelectionPulse)*float3(1,.93,.78)*(.025+sr*.12);
 return result;'''
  bodyf=bodyf.replace('return result;',weak)
 blend=state['rtBlend0'];bn={0:'Zero',1:'One',5:'SrcAlpha',10:'OneMinusSrcAlpha'}
 blendstr='Blend '+bn[int(blend['srcBlend']['val'])]+' '+bn[int(blend['destBlend']['val'])]
 passname='SOURCE_OUTLINE' if name=='ToonEffectFull' and pi==0 else 'FORWARD_BASE' if lm=='FORWARDBASE' else 'FORWARD_ADD'
 pragma='#pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight' if shadow else '#pragma multi_compile DIRECTIONAL'
 return f'''Pass {{ Name "{passname}" Tags {{"LightMode"="{'ForwardBase' if lm=='FORWARDBASE' else 'ForwardAdd'}"}}
 Cull {['Off','Front','Back'][int(state['culling']['val'])]} ZWrite {'On' if state['zWrite']['val'] else 'Off'} ZTest LEqual
 {blendstr} ColorMask {'RGB' if blend['colMask']['val']==14 else 'RGBA'}
 CGPROGRAM
 #pragma vertex vert
 #pragma fragment frag
 #pragma target 3.0
 {pragma}
 #include "UnityCG.cginc"
 #include "Lighting.cginc"
 #include "AutoLight.cginc"
 {' '.join(globals)}
 {ststruct(vs,'Mtl_VertexIn','appdata')}
 {structout}
 v2f vert(appdata input){{ {bodyv} }}
 float4 frag(v2f input):SV_Target {{ {bodyf} }}
 ENDCG
 }}'''

for name,tail,kws in [('ToonGlass','','DIRECTIONAL'),('ToonGlass','Single','_ISSINGLECOLOR_ON DIRECTIONAL'),('ToonGlass','World','_ISUSEWORLDMATCAP_ON DIRECTIONAL'),('ToonGlassBack','','DIRECTIONAL'),('ToonEffectFull','','DIRECTIONAL'),('ToonEffectFull','Single','_ISSINGLECOLOR_ON DIRECTIONAL'),('ToonEffectFull','World','_ISUSEWORLDMATCAP_ON DIRECTIONAL')]:
 kws=kws.split();d=json.loads((src/(name+'-0-Fragment-'+'_'.join(kws)+'.json')).read_text());isglass='Glass' in name
 passes=[renderpass(name,i,kws,overlay=(i==0 if isglass else i==1)) for i in range(2 if isglass else 3)]
 if not isglass:
  passes.append('UsePass "Goose/Source/ToonLite2/ShadowCaster"')
 tags='"Queue"="'+('Transparent-1' if name=='ToonGlassBack' else 'Transparent+1' if isglass else 'Geometry')+'" "RenderType"="'+('Transparent' if isglass else 'Opaque')+'"'
 text='''// Generated by work/port_special_item_shaders.py from matching iOS Metal vertex + fragment.
// Original math/pass states retained; project selection overlay is separately marked.
Shader "Goose/SourceItems/'''+name+tail+'" { Properties {\n'+props(d)+'\n}\n SubShader { Tags { '+tags+' }\n UsePass "Goose/Original Item/SELECTION_OUTLINE"\n'+'\n'.join(passes)+'\n}\n}\n'
 (out/(name+tail+'.shader')).write_text(text)
 print('ported',name+tail)
