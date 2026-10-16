"""Mechanical GLSL fragment port. Directional fragment + recovered SHADOWS_SCREEN attenuation and ShadowCaster.
Unity provides the same world TBN and UV varyings; the active Unity light and native shadow sampler provide platform-dependent uniforms.
"""
import UnityPy,lz4.block,json,re
from pathlib import Path
from UnityPy.export.ShaderConverter import ShaderProgram
from UnityPy.streams import EndianBinaryReader
root=Path('/Users/liuzhenhua/Desktop/code/unity-match-3d-puzzle/Assets/GooseReplica/Shaders/Source');root.mkdir(exist_ok=True)
seen=set()
for bundle in ['countryside','countrysidestovesub','barbecue','barbecuesub']:
 E=UnityPy.load('work/wechat-'+bundle+'.bundle')
 for o in E.objects:
  if o.type.name!='Shader':continue
  d=o.read_typetree();name=d['m_ParsedForm']['m_Name']
  if not name.startswith('ToonLite'):continue
  raw=lz4.block.decompress(bytes(d['compressedBlob']),uncompressed_size=d['decompressedLengths'][0][0]);program=ShaderProgram(EndianBinaryReader(raw,endian='<'),(2021,3,0,0))
  for world in [False,True]:
   ident=name+('World' if world else '')
   if ident in seen:continue
   candidates=[(i,p) for i,p in enumerate(program.m_SubPrograms) if i>0 and set(p.m_Keywords)<=set(['DIRECTIONAL','LIGHTPROBE_SH','_ISUSEWORLDMATCAP_ON']) and 'DIRECTIONAL' in p.m_Keywords and ('_ISUSEWORLDMATCAP_ON' in p.m_Keywords)==world]
   if not candidates:continue
   i,p=min(candidates,key=lambda t:len(t[1].m_Keywords));source=p.m_ProgramCode.decode();frag=source.split('#ifdef FRAGMENT')[1]
   assert not re.search(r'vs_TEXCOORD[4-9]',frag),ident
   sourcepath=Path('outputs/goose-analysis/visual-camera-evidence')/(ident+'-selected.glsl');sourcepath.write_text(source)
   decl=frag[frag.index('uniform'):frag.index('#define SV_Target0')]
   decl=re.sub(r'varying[^;]*;','',decl)
   decl=re.sub(r'uniform\s+(?:(?:mediump|lowp|highp)\s+)?(?:vec\d|float)\s+(?:hlslcc_[^;]+|_WorldSpaceCameraPos|_Time|unity_OrthoParams|_WorldSpaceLightPos0|_LightColor0);','',decl)
   locals=frag[frag.index('\n',frag.index('#define SV_Target0')):frag.index('void main()')]
   body=frag[frag.index('{',frag.index('void main()'))+1:frag.rindex('}')]
   def convert(s):
    s=re.sub(r'\b(?:lowp|mediump|highp|uniform)\s+','',s)
    s=re.sub(r'\bvec([234])\s*\(',r'V\1(',s)
    s=re.sub(r'\bvec([234])\b',r'float\1',s)
    s=s.replace('texture2D','tex2D').replace('inversesqrt','rsqrt')
    mapping={'unity_MatrixV':'UNITY_MATRIX_V','unity_MatrixInvV':'UNITY_MATRIX_I_V','unity_ObjectToWorld':'unity_ObjectToWorld','unity_WorldToObject':'unity_WorldToObject'}
    for old,new in mapping.items():s=re.sub(r'hlslcc_mtx4x4'+old+r'\[(\d)\]',r'transpose('+new+r')[\1]',s)
    # Keep runtime main light uniforms; direction can rotate with source light rig.
    # SHADOWS_SCREEN variant multiplies the clamped cell term before all shadow masks.
    pattern=r'(u_xlat[\w.]+) = \(u_xlatb\d+\) \? 0.0 : \1;'
    s,n=re.subn(pattern,lambda m:m.group(0)+'\n    '+m.group(1)+' *= sourceAttenuation;',s)
    if 'void' not in s and '_LightColor0.w==' in s: assert n==1,ident
    s=s.replace('return;','return SV_Target0;')
    return s
   props=[]
   for pr in d['m_ParsedForm']['m_PropInfo']['m_Props']:
    n=pr['m_Name'];t=pr['m_Type'];v=[pr['m_DefValue['+str(j)+']'] for j in range(4)]
    if t==4:props.append(n+'("'+n+'",2D)="'+pr['m_DefTexture']['m_DefaultName']+'"{}')
    elif t in [0,1]:props.append(n+'("'+n+'",Vector)=('+','.join(map(str,v))+')')
    else:props.append(n+'("'+n+'",Float)='+str(v[0]))
   props.append('_SourceLightDirection("Saved light direction",Vector)=(0,1,0,0)')
   head='''// Generated from the saved WX GLES directional fragment; see Tools/LayeredPile/visual-camera-alignment.md.
Shader "Goose/Source/IDENT" { Properties { PROPS }
SubShader { Tags { "RenderType"="Opaque" } Pass { Tags { "LightMode"="ForwardBase" } CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma target 3.0
#pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"
float4 _SourceLightDirection;
DECL
float2 V2(float a){return a.xx;} float2 V2(float2 a){return a;} float2 V2(float a,float b){return float2(a,b);}
float3 V3(float a){return a.xxx;} float3 V3(float3 a){return a;} float3 V3(float a,float b,float c){return float3(a,b,c);}
float4 V4(float a){return a.xxxx;} float4 V4(float4 a){return a;} float4 V4(float a,float b,float c,float d){return float4(a,b,c,d);}
struct appdata {float4 vertex:POSITION;float3 normal:NORMAL;float4 tangent:TANGENT;float2 uv:TEXCOORD0;};
struct v2f {float4 pos:SV_POSITION;float2 uv:TEXCOORD0;float4 t0:TEXCOORD1;float4 t1:TEXCOORD2;float4 t2:TEXCOORD3;SHADOW_COORDS(4)};
float4 _texcoord_ST;
v2f vert(appdata v){v2f o;o.pos=UnityObjectToClipPos(v.vertex);o.uv=v.uv*_texcoord_ST.xy+_texcoord_ST.zw;
float3 p=mul(unity_ObjectToWorld,v.vertex).xyz,n=UnityObjectToWorldNormal(v.normal),t=UnityObjectToWorldDir(v.tangent.xyz),b=cross(n,t)*v.tangent.w*unity_WorldTransformParams.w;
o.t0=float4(t.x,b.x,n.x,p.x);o.t1=float4(t.y,b.y,n.y,p.y);o.t2=float4(t.z,b.z,n.z,p.z);TRANSFER_SHADOW(o);return o;}
float4 frag(v2f i):SV_Target {
float2 vs_TEXCOORD0=i.uv;float4 vs_TEXCOORD1=i.t0,vs_TEXCOORD2=i.t1,vs_TEXCOORD3=i.t2,SV_Target0;
LOCALS
UNITY_LIGHT_ATTENUATION(sourceAttenuation,i,float3(i.t0.w,i.t1.w,i.t2.w));
BODY
}
ENDCG }
Pass { Name "ShadowCaster" Tags { "LightMode"="ShadowCaster" } ZWrite On ZTest LEqual Cull Back
CGPROGRAM
#pragma vertex shadowVert
#pragma fragment shadowFrag
#pragma multi_compile_shadowcaster
#include "UnityCG.cginc"
struct shadowV2f { V2F_SHADOW_CASTER; };
shadowV2f shadowVert(appdata_base v) { shadowV2f o; TRANSFER_SHADOW_CASTER_NORMALOFFSET(o) return o; }
float4 shadowFrag(shadowV2f i):SV_Target { SHADOW_CASTER_FRAGMENT(i) }
ENDCG
} }
}
'''
   text=head.replace('IDENT',ident).replace('PROPS','\n'.join(props)).replace('DECL',convert(decl)).replace('LOCALS',convert(locals)).replace('BODY',convert(body))
   assert 'hlslcc_' not in text,ident
   (root/(ident+'.shader')).write_text(text);seen.add(ident);print('ported',ident,'variant',i,p.m_Keywords)
