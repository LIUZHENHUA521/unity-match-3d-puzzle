using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

// Saved WX level0 SceneChange light + Medium quality profile. Never persist global settings.
public sealed class ReplicaSourceLighting : MonoBehaviour {
 public static readonly Quaternion SavedRotation=new Quaternion(-.07652297616004944f,-.8228871822357178f,.5578038692474365f,.07652299106121063f);
 public Light SourceLight {get; private set;}
 public bool Active=>SourceLight;
 bool applied;Light[] previousLights;bool[] previousEnabled;Light sun;
 ShadowQuality shadows;ShadowResolution resolution;ShadowProjection projection;int cascades,pixelLights;
 float distance,nearOffset;
 AmbientMode ambientMode;Color sky,equator,ground,subtractive,fogColor;
 float ambientIntensity,reflectionIntensity,fogDensity,fogStart,fogEnd;
 Material skybox;bool fog;FogMode fogMode;
 DefaultReflectionMode reflectionMode;int reflectionResolution,reflectionBounces;
 Texture customReflection;SphericalHarmonicsL2 ambientProbe;
 public void Begin(){
  End();applied=true;previousLights=FindObjectsOfType<Light>();previousEnabled=previousLights.Select(x=>x.enabled).ToArray();foreach(var l in previousLights)l.enabled=false;
  shadows=QualitySettings.shadows;resolution=QualitySettings.shadowResolution;projection=QualitySettings.shadowProjection;cascades=QualitySettings.shadowCascades;distance=QualitySettings.shadowDistance;nearOffset=QualitySettings.shadowNearPlaneOffset;pixelLights=QualitySettings.pixelLightCount;
  QualitySettings.shadows=ShadowQuality.HardOnly;QualitySettings.shadowResolution=ShadowResolution.Medium;QualitySettings.shadowProjection=ShadowProjection.StableFit;QualitySettings.shadowCascades=0;QualitySettings.shadowDistance=40;QualitySettings.shadowNearPlaneOffset=3;QualitySettings.pixelLightCount=1;
  // Request API no-split (0); Unity 2022.3 normalizes the getter to 1 (verified in batch),
  // matching the source serialized single-cascade value 1.
  sun=RenderSettings.sun;ambientMode=RenderSettings.ambientMode;sky=RenderSettings.ambientSkyColor;equator=RenderSettings.ambientEquatorColor;ground=RenderSettings.ambientGroundColor;ambientIntensity=RenderSettings.ambientIntensity;reflectionIntensity=RenderSettings.reflectionIntensity;subtractive=RenderSettings.subtractiveShadowColor;skybox=RenderSettings.skybox;fog=RenderSettings.fog;
  fogColor=RenderSettings.fogColor;fogMode=RenderSettings.fogMode;fogDensity=RenderSettings.fogDensity;fogStart=RenderSettings.fogStartDistance;fogEnd=RenderSettings.fogEndDistance;
  reflectionMode=RenderSettings.defaultReflectionMode;reflectionResolution=RenderSettings.defaultReflectionResolution;reflectionBounces=RenderSettings.reflectionBounces;customReflection=RenderSettings.customReflectionTexture;ambientProbe=RenderSettings.ambientProbe;
  // WX data.unity3d / level0 RenderSettings 55. HDR values are not clamped or gamma-converted.
  // Flat=3 and Skybox reflection=0 are verified against UnityEngine.CoreModule enums.
  RenderSettings.ambientMode=AmbientMode.Flat;
  RenderSettings.ambientSkyColor=new Color(1.4148107767105103f,1.4148107767105103f,1.4148107767105103f,1);
  RenderSettings.ambientEquatorColor=new Color(.11400000005960464f,.125f,.13300000131130219f,1);
  RenderSettings.ambientGroundColor=new Color(.04699999839067459f,.0430000014603138f,.03500000014901161f,1);
  RenderSettings.ambientIntensity=1;
  RenderSettings.subtractiveShadowColor=new Color(.41999998688697815f,.4779999852180481f,.6269999742507935f,1);
  RenderSettings.skybox=null;RenderSettings.defaultReflectionMode=DefaultReflectionMode.Skybox;
  RenderSettings.defaultReflectionResolution=128;RenderSettings.reflectionBounces=1;
  RenderSettings.reflectionIntensity=.46000000834465027f;RenderSettings.customReflectionTexture=null;
  RenderSettings.fog=false;RenderSettings.fogColor=new Color(.5f,.5f,.5f,1);RenderSettings.fogMode=FogMode.ExponentialSquared;
  RenderSettings.fogDensity=.009999999776482582f;RenderSettings.fogStartDistance=0;RenderSettings.fogEndDistance=300;
  // The package's zero serialized ambientProbe is not evidence of the evaluated runtime SH.
  // Leave Flat-mode probe generation to Unity; do not force a zero probe or schedule DynamicGI.

  var g=new GameObject("Source SceneChange Directional Light");g.transform.SetParent(transform,false);g.transform.SetPositionAndRotation(new Vector3(0,3,0),SavedRotation);SourceLight=g.AddComponent<Light>();SourceLight.type=LightType.Directional;SourceLight.color=Color.white;SourceLight.intensity=.3799999952316284f;SourceLight.cullingMask=471;SourceLight.shadows=LightShadows.Hard;SourceLight.shadowResolution=LightShadowResolution.FromQualitySettings;SourceLight.shadowStrength=1;SourceLight.shadowBias=.05000000074505806f;SourceLight.shadowNormalBias=.4000000059604645f;SourceLight.shadowNearPlane=.20000000298023224f;SourceLight.renderMode=LightRenderMode.Auto;RenderSettings.sun=SourceLight;
 }
 // Use only if the caller has established the original LightAngle transform from its callback.
 public void SetAngle(Quaternion angle){if(SourceLight)SourceLight.transform.rotation=angle*SavedRotation;}
 public void End(){
  if(!applied)return;applied=false;if(SourceLight){SourceLight.enabled=false;Destroy(SourceLight.gameObject);SourceLight=null;}
  for(int i=0;i<previousLights.Length;i++)if(previousLights[i])previousLights[i].enabled=previousEnabled[i];previousLights=null;previousEnabled=null;
  QualitySettings.shadows=shadows;QualitySettings.shadowResolution=resolution;QualitySettings.shadowProjection=projection;QualitySettings.shadowCascades=cascades;QualitySettings.shadowDistance=distance;QualitySettings.shadowNearPlaneOffset=nearOffset;QualitySettings.pixelLightCount=pixelLights;
  RenderSettings.sun=sun;RenderSettings.ambientMode=ambientMode;RenderSettings.ambientSkyColor=sky;RenderSettings.ambientEquatorColor=equator;RenderSettings.ambientGroundColor=ground;RenderSettings.ambientIntensity=ambientIntensity;RenderSettings.reflectionIntensity=reflectionIntensity;RenderSettings.subtractiveShadowColor=subtractive;RenderSettings.skybox=skybox;RenderSettings.fog=fog;
  RenderSettings.fogColor=fogColor;RenderSettings.fogMode=fogMode;RenderSettings.fogDensity=fogDensity;RenderSettings.fogStartDistance=fogStart;RenderSettings.fogEndDistance=fogEnd;
  RenderSettings.defaultReflectionMode=reflectionMode;RenderSettings.defaultReflectionResolution=reflectionResolution;RenderSettings.reflectionBounces=reflectionBounces;RenderSettings.customReflectionTexture=customReflection;
  // Restore the captured evaluated probe last, after the settings that may invalidate it.
  RenderSettings.ambientProbe=ambientProbe;
 }
 void OnDestroy(){End();}
}
