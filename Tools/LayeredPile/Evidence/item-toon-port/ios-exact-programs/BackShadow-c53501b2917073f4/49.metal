#include <metal_stdlib>
#include <metal_texture>
using namespace metal;
#ifndef XLT_REMAP_O
	#define XLT_REMAP_O {0, 1, 2, 3, 4, 5, 6, 7}
#endif
constexpr constant uint xlt_remap_o[] = XLT_REMAP_O;
struct FGlobals_Type
{
    float4 unity_LODFade;
};

struct Mtl_FragmentOut
{
    half4 SV_Target0 [[ color(xlt_remap_o[0]) ]];
};

fragment Mtl_FragmentOut xlatMtlMain(
    constant FGlobals_Type& FGlobals [[ buffer(0) ]],
    sampler samplerunity_DitherMask [[ sampler (0) ]],
    texture2d<half, access::sample > unity_DitherMask [[ texture(0) ]] ,
    float4 mtl_FragCoord [[ position ]])
{
    Mtl_FragmentOut output;
    float4 hlslcc_FragCoord = float4(mtl_FragCoord.xyz, 1.0/mtl_FragCoord.w);
    float2 u_xlat0;
    half u_xlat16_0;
    bool u_xlatb0;
    float u_xlat1;
    bool u_xlatb1;
    u_xlat0.xy = hlslcc_FragCoord.xy * float2(0.25, 0.25);
    u_xlat16_0 = unity_DitherMask.sample(samplerunity_DitherMask, u_xlat0.xy).w;
    u_xlatb1 = 0.0<FGlobals.unity_LODFade.x;
    u_xlat1 = (u_xlatb1) ? 1.0 : -1.0;
    u_xlat0.x = fma((-float(u_xlat16_0)), u_xlat1, FGlobals.unity_LODFade.x);
    u_xlatb0 = u_xlat0.x<0.0;
    if(((int(u_xlatb0) * int(0xffffffffu)))!=0){discard_fragment();}
    output.SV_Target0 = half4(0.0, 0.0, 0.0, 0.0);
    return output;
}
