#if UNITY_EDITOR
using System;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;

// Tests source resource values and reversible global state, not original-device pixels or GI timing.
public static class SourceAmbientValidation {
 [Serializable] class Evidence {public Record[] records;}
 [Serializable] class Record {public string type,file;public int id;public Settings data;}
 [Serializable] class RGBA {public float r,g,b,a;public Color Value=>new Color(r,g,b,a);}
 [Serializable] class Pointer {public int m_FileID;public long m_PathID;}
 [Serializable] class Settings {
  public int m_AmbientMode,m_DefaultReflectionMode,m_DefaultReflectionResolution,m_ReflectionBounces,m_FogMode;
  public RGBA m_AmbientSkyColor,m_AmbientEquatorColor,m_AmbientGroundColor,m_SubtractiveShadowColor,m_FogColor;
  public float m_AmbientIntensity,m_ReflectionIntensity,m_FogDensity,m_LinearFogStart,m_LinearFogEnd;
  public bool m_Fog;public Pointer m_SkyboxMaterial,m_CustomReflection;
 }
 sealed class State {
  public AmbientMode mode=RenderSettings.ambientMode;
  public Color sky=RenderSettings.ambientSkyColor,equator=RenderSettings.ambientEquatorColor,ground=RenderSettings.ambientGroundColor,subtractive=RenderSettings.subtractiveShadowColor,fogColor=RenderSettings.fogColor;
  public float intensity=RenderSettings.ambientIntensity,reflectionIntensity=RenderSettings.reflectionIntensity,fogDensity=RenderSettings.fogDensity,fogStart=RenderSettings.fogStartDistance,fogEnd=RenderSettings.fogEndDistance;
  public bool fog=RenderSettings.fog;public FogMode fogMode=RenderSettings.fogMode;
  public Material skybox=RenderSettings.skybox;public Texture custom=RenderSettings.customReflectionTexture;
  public DefaultReflectionMode reflectionMode=RenderSettings.defaultReflectionMode;
  public int reflectionResolution=RenderSettings.defaultReflectionResolution,reflectionBounces=RenderSettings.reflectionBounces;
  public SphericalHarmonicsL2 probe=RenderSettings.ambientProbe;
  public void Restore(){
   RenderSettings.ambientMode=mode;RenderSettings.ambientSkyColor=sky;RenderSettings.ambientEquatorColor=equator;RenderSettings.ambientGroundColor=ground;RenderSettings.subtractiveShadowColor=subtractive;RenderSettings.ambientIntensity=intensity;
   RenderSettings.skybox=skybox;RenderSettings.defaultReflectionMode=reflectionMode;RenderSettings.defaultReflectionResolution=reflectionResolution;RenderSettings.reflectionBounces=reflectionBounces;RenderSettings.reflectionIntensity=reflectionIntensity;RenderSettings.customReflectionTexture=custom;
   RenderSettings.fog=fog;RenderSettings.fogMode=fogMode;RenderSettings.fogColor=fogColor;RenderSettings.fogDensity=fogDensity;RenderSettings.fogStartDistance=fogStart;RenderSettings.fogEndDistance=fogEnd;RenderSettings.ambientProbe=probe;
  }
  public void CheckRestored(){
   Check(RenderSettings.ambientMode==mode,"restore mode");Colour(RenderSettings.ambientSkyColor,sky,"restore sky");Colour(RenderSettings.ambientEquatorColor,equator,"restore equator");Colour(RenderSettings.ambientGroundColor,ground,"restore ground");Colour(RenderSettings.subtractiveShadowColor,subtractive,"restore subtractive");Near(RenderSettings.ambientIntensity,intensity,"restore intensity");
   Check(RenderSettings.skybox==skybox&&RenderSettings.customReflectionTexture==custom&&RenderSettings.defaultReflectionMode==reflectionMode&&RenderSettings.defaultReflectionResolution==reflectionResolution&&RenderSettings.reflectionBounces==reflectionBounces,"restore reflection objects/settings");Near(RenderSettings.reflectionIntensity,reflectionIntensity,"restore reflection intensity");
   Check(RenderSettings.fog==fog&&RenderSettings.fogMode==fogMode,"restore fog state");Colour(RenderSettings.fogColor,fogColor,"restore fog color");Near(RenderSettings.fogDensity,fogDensity,"restore fog density");Near(RenderSettings.fogStartDistance,fogStart,"restore fog start");Near(RenderSettings.fogEndDistance,fogEnd,"restore fog end");
   var actual=RenderSettings.ambientProbe;for(int rgb=0;rgb<3;rgb++)for(int n=0;n<9;n++)Near(actual[rgb,n],probe[rgb,n],"restore SH "+rgb+"/"+n);
  }
 }
 static void Check(bool ok,string message){if(!ok)throw new Exception("SOURCE_AMBIENT_VALIDATION: "+message);}
 static void Near(float actual,float expected,string label){Check(Mathf.Abs(actual-expected)<.00001f,label+": actual="+actual+", expected="+expected);}
 static void Colour(Color actual,Color expected,string label){Near(actual.r,expected.r,label+".r");Near(actual.g,expected.g,label+".g");Near(actual.b,expected.b,label+".b");Near(actual.a,expected.a,label+".a");}
 static void CheckSource(Settings e){
  Check((int)AmbientMode.Flat==3&&(int)DefaultReflectionMode.Skybox==0,"enum mapping");Check((int)RenderSettings.ambientMode==e.m_AmbientMode,"ambient mode");
  Colour(RenderSettings.ambientSkyColor,e.m_AmbientSkyColor.Value,"HDR sky");Colour(RenderSettings.ambientEquatorColor,e.m_AmbientEquatorColor.Value,"equator");Colour(RenderSettings.ambientGroundColor,e.m_AmbientGroundColor.Value,"ground");Colour(RenderSettings.subtractiveShadowColor,e.m_SubtractiveShadowColor.Value,"subtractive shadow");Near(RenderSettings.ambientIntensity,e.m_AmbientIntensity,"ambient intensity");
  Check(e.m_SkyboxMaterial.m_PathID==0&&e.m_CustomReflection.m_PathID==0,"source reflection references null");Check(!RenderSettings.skybox&&!RenderSettings.customReflectionTexture,"applied reflection references null");
  Check((int)RenderSettings.defaultReflectionMode==e.m_DefaultReflectionMode&&RenderSettings.defaultReflectionResolution==e.m_DefaultReflectionResolution&&RenderSettings.reflectionBounces==e.m_ReflectionBounces,"reflection configuration");Near(RenderSettings.reflectionIntensity,e.m_ReflectionIntensity,"reflection intensity");
  Check(RenderSettings.fog==e.m_Fog&&(int)RenderSettings.fogMode==e.m_FogMode,"fog state");Colour(RenderSettings.fogColor,e.m_FogColor.Value,"fog color");Near(RenderSettings.fogDensity,e.m_FogDensity,"fog density");Near(RenderSettings.fogStartDistance,e.m_LinearFogStart,"fog start");Near(RenderSettings.fogEndDistance,e.m_LinearFogEnd,"fog end");
 }
 public static string Run(){
  Directory.CreateDirectory("Temp");const string output="Temp/goose-source-ambient-validation.txt";
  try {var result=Execute();File.WriteAllText(output,result);return result;}catch(Exception ex){File.WriteAllText(output,"FAIL "+ex);throw;}
 }
 static string Execute(){
  var raw=File.ReadAllText("Tools/LayeredPile/Evidence/lighting-settings-wx.json");var evidence=JsonUtility.FromJson<Evidence>("{\"records\":"+raw+"}");Settings source=null;
  foreach(var record in evidence.records)if(record.type=="RenderSettings"&&record.file=="level0"&&record.id==55)source=record.data;
  Check(source!=null,"level0 RenderSettings 55 evidence");
  var before=new State();var cube=new Cubemap(16,TextureFormat.RGBA32,false);var sentinelSky=new Material(Shader.Find("Skybox/Procedural"));var rig=new GameObject("Source ambient validation");var lighting=rig.AddComponent<ReplicaSourceLighting>();
  try {
   // Deliberately different pre-existing state, including a nonempty custom reflection and SH.
   RenderSettings.ambientMode=AmbientMode.Custom;RenderSettings.ambientSkyColor=new Color(.07f,.13f,.29f,1);RenderSettings.ambientEquatorColor=new Color(.33f,.2f,.1f,1);RenderSettings.ambientGroundColor=new Color(.11f,.19f,.23f,1);RenderSettings.ambientIntensity=.73f;RenderSettings.subtractiveShadowColor=new Color(.19f,.27f,.37f,1);
   RenderSettings.skybox=sentinelSky;RenderSettings.defaultReflectionMode=DefaultReflectionMode.Custom;RenderSettings.defaultReflectionResolution=64;RenderSettings.reflectionBounces=2;RenderSettings.reflectionIntensity=.67f;RenderSettings.customReflectionTexture=cube;
   RenderSettings.fog=true;RenderSettings.fogMode=FogMode.Linear;RenderSettings.fogColor=new Color(.12f,.14f,.16f,1);RenderSettings.fogDensity=.03f;RenderSettings.fogStartDistance=2;RenderSettings.fogEndDistance=53;
   var sh=new SphericalHarmonicsL2();for(int rgb=0;rgb<3;rgb++)for(int n=0;n<9;n++)sh[rgb,n]=(rgb*9+n+1)*.003f;RenderSettings.ambientProbe=sh;var sentinel=new State();
   lighting.Begin();CheckSource(source);lighting.Begin();CheckSource(source);lighting.End();sentinel.CheckRestored();lighting.End();sentinel.CheckRestored();
   lighting.Begin();CheckSource(source);UnityEngine.Object.DestroyImmediate(rig);rig=null;sentinel.CheckRestored();
   return "PASS source environment: level0 RenderSettings 55 matches Flat=3, HDR sky 1.4148108, equator/ground/intensity/subtractive, null skybox/custom reflection, reflection mode/resolution/bounces/intensity, fog; repeated Begin/End and OnDestroy restore nondefault environment, reflection objects and 27 SH coefficients. No claim about original-device pixels or runtime GI/probe generation.";
  } finally {if(rig){lighting.End();UnityEngine.Object.DestroyImmediate(rig);}before.Restore();UnityEngine.Object.DestroyImmediate(sentinelSky);UnityEngine.Object.DestroyImmediate(cube);}
 }
}
#endif
