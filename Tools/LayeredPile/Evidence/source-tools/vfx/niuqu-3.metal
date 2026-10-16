#include <metal_stdlib>
#include <metal_texture>
using namespace metal;
#ifndef XLT_REMAP_O
	#define XLT_REMAP_O {0, 1, 2, 3, 4, 5, 6, 7}
#endif
constexpr constant uint xlt_remap_o[] = XLT_REMAP_O;
struct FGlobals_Type
{
    float4 _Time;
    half4 _Niuqu_ST;
    float _Niuqu_U;
    float _Niuqu_V;
    half _V;
    half4 _Tex_ST;
    float _Tex_U;
    float _Tex_V;
    half4 _Tex_Color;
    half4 _Mask_ST;
    half _Tex_Color_Scale;
};

struct Mtl_FragmentIn
{
    float4 TEXCOORD1 [[ user(TEXCOORD1) ]] ;
    float4 COLOR0 [[ user(COLOR0) ]] ;
};

struct Mtl_FragmentOut
{
    half4 SV_Target0 [[ color(xlt_remap_o[0]) ]];
};

fragment Mtl_FragmentOut xlatMtlMain(
    constant FGlobals_Type& FGlobals [[ buffer(0) ]],
    sampler sampler_Tex [[ sampler (0) ]],
    sampler sampler_Niuqu [[ sampler (1) ]],
    sampler sampler_Mask [[ sampler (2) ]],
    texture2d<half, access::sample > _Niuqu [[ texture(0) ]] ,
    texture2d<half, access::sample > _Tex [[ texture(1) ]] ,
    texture2d<half, access::sample > _Mask [[ texture(2) ]] ,
    Mtl_FragmentIn input [[ stage_in ]])
{
    Mtl_FragmentOut output;
    float4 u_xlat0;
    half4 u_xlat16_0;
    half4 u_xlat16_1;
    float2 u_xlat2;
    float2 u_xlat3;
    u_xlat0.xy = fma(input.TEXCOORD1.xy, float2(FGlobals._Niuqu_ST.xy), float2(FGlobals._Niuqu_ST.zw));
    u_xlat16_1.xy = half2(fma(float2(FGlobals._Niuqu_U, FGlobals._Niuqu_V), FGlobals._Time.yy, u_xlat0.xy));
    u_xlat16_0.x = _Niuqu.sample(sampler_Niuqu, float2(u_xlat16_1.xy)).x;
    u_xlat3.xy = fma(input.TEXCOORD1.xy, float2(FGlobals._Tex_ST.xy), float2(FGlobals._Tex_ST.zw));
    u_xlat16_1.xy = half2(fma(float2(FGlobals._Tex_U, FGlobals._Tex_V), FGlobals._Time.yy, u_xlat3.xy));
    u_xlat0.xy = fma(float2(u_xlat16_0.xx), float2(FGlobals._V), float2(u_xlat16_1.xy));
    u_xlat16_0 = _Tex.sample(sampler_Tex, u_xlat0.xy);
    u_xlat0 = float4(u_xlat16_0) * float4(FGlobals._Tex_Color);
    u_xlat2.xy = fma(input.TEXCOORD1.xy, float2(FGlobals._Mask_ST.xy), float2(FGlobals._Mask_ST.zw));
    u_xlat16_1 = _Mask.sample(sampler_Mask, u_xlat2.xy);
    u_xlat0 = u_xlat0 * float4(u_xlat16_1);
    u_xlat0 = u_xlat0 * input.COLOR0;
    u_xlat0 = u_xlat0 * float4(FGlobals._Tex_Color_Scale);
    output.SV_Target0 = half4(u_xlat0);
    return output;
}
