from pathlib import Path
import re
out=Path('/Users/liuzhenhua/Desktop/code/unity-match-3d-puzzle/Assets/GooseReplica/Resources/Replica/Highlight');out.mkdir(parents=True,exist_ok=True)
src=Path('outputs/goose-analysis/highlight-complete-evidence/programs')
common='''#include "UnityCG.cginc"
float2 V2(float a){return a.xx;} float2 V2(float2 a){return a;} float2 V2(float a,float b){return float2(a,b);}
float3 V3(float a){return a.xxx;} float3 V3(float3 a){return a;} float3 V3(float a,float b,float c){return float3(a,b,c);}
float4 V4(float a){return a.xxxx;} float4 V4(float4 a){return a;} float4 V4(float a,float b,float c,float d){return float4(a,b,c,d);}
// GLES clip Z is [-w,w]. Preserve its width equation on Metal/D3D reversed-Z targets.
float SourceClipZ(float4 p){
#if defined(UNITY_REVERSED_Z)
 return p.w-2*p.z;
#elif defined(SHADER_API_GLCORE) || defined(SHADER_API_GLES) || defined(SHADER_API_GLES3)
 return p.z;
#else
 return 2*p.z-p.w;
#endif
}
'''
def conv(s):
 s=re.sub(r'\b(?:lowp|mediump|highp|uniform)\s+','',s)
 s=re.sub(r'\bvec([234])\s*\(',r'V\1(',s);s=re.sub(r'\bvec([234])\b',r'float\1',s)
 s=s.replace('texture2D','tex2D').replace('inversesqrt','rsqrt').replace('fract','frac')
 mapping={'unity_MatrixInvV':'UNITY_MATRIX_I_V','unity_MatrixVP':'UNITY_MATRIX_VP','glstate_matrix_projection':'UNITY_MATRIX_P','unity_ObjectToWorld':'unity_ObjectToWorld','unity_WorldToObject':'unity_WorldToObject'}
 for a,b in mapping.items():s=re.sub(r'hlslcc_mtx4x4'+a+r'\[(\d)\]',r'transpose('+b+r')[\1]',s)
 return s
for name in ['Outline','Glow','Overlay']:
 s=(src/('HighlightPlus_Geometry_'+name)/'1.glsl').read_text();v,f=s.split('#ifdef FRAGMENT');v=v.split('#ifdef VERTEX')[1]
 def parts(t):
  decl=t[t.index('uniform'):t.index('void main()')];decl=re.sub(r'uniform[^;]*(?:hlslcc_[^;]*|_Time)[^;]*;','',decl)
  decl=re.sub(r'(?:attribute|varying)[^;]*;','',decl);decl=re.sub(r'#define[^\n]*','',decl)
  body=t[t.index('{',t.index('void main()'))+1:t.rindex('}')];return decl,body
 vd,vb=parts(v);fd,fb=parts(f)
 # Keep local temporaries inside functions, uniform declarations shared once.
 uniforms=[]
 for d in [vd,fd]:
  uniforms+=re.findall(r'uniform[^;]*;',d)
 uniforms=list(dict.fromkeys(uniforms));vl=re.sub(r'uniform[^;]*;','',vd);fl=re.sub(r'uniform[^;]*;','',fd)
 vb=vb.replace('return;','return o;');fb=fb.replace('return;','return result;').replace('SV_Target0','result')
 for a,b in [('gl_Position','o.pos'),('in_POSITION0','v.vertex'),('in_NORMAL0','v.normal'),('in_TEXCOORD0','v.uv'),('vs_COLOR0','o.color'),('vs_TEXCOORD0','o.uv'),('vs_TEXCOORD1','o.world'),('vs_TEXCOORD2','o.normal')]:vb=vb.replace(a,b)
 for a,b in [('vs_COLOR0','i.color'),('vs_TEXCOORD0','i.uv'),('vs_TEXCOORD1','i.world')]:fb=fb.replace(a,b)
 fb=fb.replace('vec4 hlslcc_FragCoord = vec4(gl_FragCoord.xyz, 1.0/gl_FragCoord.w);','float4 hlslcc_FragCoord = i.pos;')
 if name in ['Outline','Glow']:
  vb=vb.replace('(-u_xlat1.z) + 2.0','(-SourceClipZ(u_xlat1)) + 2.0').replace(' + u_xlat1.z;',' + SourceClipZ(u_xlat1);')
 if name=='Overlay':
  # Offset is toward camera on all APIs (original GLES -0.0001 clip-z).
  vb=vb.replace('vec4(0.0, 0.0, -9.99999975e-05, 0.0)','float4(0,0,0,0)')
  vb=vb.replace('o.pos = u_xlat0 + float4(0,0,0,0);','o.pos = u_xlat0;\n#if defined(UNITY_REVERSED_Z)\n o.pos.z+=0.0001;\n#else\n o.pos.z-=0.0001;\n#endif')
 props='''_MainTex("Texture",2D)="white"{} _Color("Color",Color)=(1,1,1,1)
 _OutlineColor("Outline",Color)=(0,0,0,1) _OutlineWidth("Width",Float)=.002 _OutlineVertexWidth("Vertex width",Float)=0 _ConstantWidth("Constant width",Float)=1 _OutlineDirection("Direction",Vector)=(0,0,0,0)
 _GlowColor("Glow color",Color)=(1,.964705944,.13333334,1) _Glow("Glow",Vector)=(.93,.0025,.75,.5) _Glow2("Glow2",Vector)=(.002,1,1,0) _GlowDirection("Direction",Vector)=(0,0,0,0)
 _OverlayColor("Overlay",Color)=(1,.92156863,.015686275,.5) _OverlayBackColor("Back",Color)=(1,1,1,1) _OverlayData("Data",Vector)=(1,.5,1,1) _OverlayHitPosData("Hit",Vector)=(0,0,0,0) _OverlayHitStartTime("Hit start",Float)=0'''
 z='Always' if name=='Glow' else 'LEqual';ref='4' if name=='Overlay' else '2';st=f'Stencil {{Ref {ref} ReadMask {ref} Comp NotEqual Pass Keep}}'
 text='// Mechanically ported saved WX GLES '+name+' program 1; active Medium branch.\nShader "Goose/SourceHighlight/'+name+'" {Properties{'+props+'}\nSubShader{Tags {"Queue"="Transparent+121"} Pass{Cull Back ZWrite Off ZTest '+z+' Blend SrcAlpha OneMinusSrcAlpha\n'+st+'\nCGPROGRAM\n#pragma target 3.0\n#pragma vertex vert\n#pragma fragment frag\n'+common+conv('\n'.join(uniforms))+'''\nstruct appdata {float4 vertex:POSITION;float3 normal:NORMAL;float2 uv:TEXCOORD0;};
struct v2f{float4 pos:SV_POSITION;float2 uv:TEXCOORD0;float3 world:TEXCOORD1;float3 normal:TEXCOORD2;float4 color:COLOR0;};
v2f vert(appdata v){v2f o=(v2f)0;
'''+conv(vl+vb)+'}\nfloat4 frag(v2f i):SV_Target {float4 result;\n'+conv(fl+fb)+'}\nENDCG\n}}}\n'
 (out/(name+'.shader')).write_text(text)
print('ported',out)
