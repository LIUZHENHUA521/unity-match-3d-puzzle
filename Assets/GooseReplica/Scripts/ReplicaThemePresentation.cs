using UnityEngine;
using System.Linq;
// Saved WX Main Camera world pose + CameraControl.Start / ScreenMa.GetScreenOffset(1,0).
// WX LevelManager ctor/Awake select flatTopView (2); CameraControl.Start disables movement and rotation.
// No height-follow or drag-orbit animation is wired into the normal solo input path.
// See Tools/LayeredPile/camera-shadow-alignment.md for call-path evidence.
public sealed class ReplicaThemePresentation : MonoBehaviour {
 public bool Active=>visual;
 public const int SourceControlMode=2; // LevelControlType.flatTopView, not an inspector tuning option.
 public static readonly Quaternion SourceRotation=new Quaternion(.6560590267f,0,0,.7547096610f);
 public static Vector3 SourcePosition=>new Vector3(0,0,.5f)+SourceRotation*new Vector3(0,0,-30);
 GameObject visual;Camera camera;CameraFitter fitter;bool fitted,orthographic;Vector3 position;Quaternion rotation;float size,near,far,fov;Rect rect;CameraClearFlags clear;Color background;bool hdr;int culling;
 Renderer[] legacy;bool[] enabledStates;ReplicaSourceLighting lighting;
 public void Begin(string theme){
  End();var prefab=Resources.Load<GameObject>("Replica/Scenes/"+theme);if(!prefab)throw new System.InvalidOperationException("Missing scene visual "+theme);
  visual=Instantiate(prefab);visual.name=theme+" source visuals";
  if(!lighting)lighting=GetComponent<ReplicaSourceLighting>();if(!lighting)lighting=gameObject.AddComponent<ReplicaSourceLighting>();lighting.Begin();
  // The source theme prefab owns the complete backdrop. Hide the legacy
  // Cooking Pot while the source presentation is active so the two bowls do
  // not overlap; restore its original renderer states when leaving the theme.
  var pot=GameObject.Find("Cooking Pot");if(pot){legacy=pot.GetComponentsInChildren<Renderer>(true);enabledStates=legacy.Select(r=>r.enabled).ToArray();foreach(var r in legacy)r.enabled=false;}
  camera=Camera.main;if(!camera)return;position=camera.transform.position;rotation=camera.transform.rotation;size=camera.orthographicSize;orthographic=camera.orthographic;near=camera.nearClipPlane;far=camera.farClipPlane;fov=camera.fieldOfView;rect=camera.rect;clear=camera.clearFlags;background=camera.backgroundColor;hdr=camera.allowHDR;culling=camera.cullingMask;
  fitter=camera.GetComponent<CameraFitter>();if(fitter){fitted=fitter.enabled;fitter.enabled=false;}
  camera.transform.SetPositionAndRotation(SourcePosition,SourceRotation);camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=Color.clear;camera.allowHDR=false;camera.cullingMask=503;camera.orthographic=true;camera.nearClipPlane=.3f;camera.farClipPlane=1000;camera.rect=new Rect(0,0,1,1);Fit();
 }
 public static float Size(float aspect)=>6.5f*Mathf.Max(1,(9f/16)/Mathf.Max(.01f,aspect));
 public void Fit(){if(camera&&visual){camera.orthographicSize=Size(camera.aspect);camera.fieldOfView=60*Mathf.Max(1,(9f/16)/camera.aspect);}}
 void LateUpdate(){Fit();}
 public void End(){
  if(lighting)lighting.End();
  if(visual){visual.SetActive(false);Destroy(visual);visual=null;}
  if(legacy!=null)for(int i=0;i<legacy.Length;i++)if(legacy[i])legacy[i].enabled=enabledStates[i];legacy=null;
  if(camera){camera.transform.SetPositionAndRotation(position,rotation);camera.orthographic=orthographic;camera.orthographicSize=size;camera.nearClipPlane=near;camera.farClipPlane=far;camera.fieldOfView=fov;camera.rect=rect;camera.clearFlags=clear;camera.backgroundColor=background;camera.allowHDR=hdr;camera.cullingMask=culling;}
  if(fitter)fitter.enabled=fitted;camera=null;fitter=null;
 }
 void OnDestroy(){End();}
}
