using UnityEngine;
using System.Linq;
// Apply the fixed top-down camera pose and viewport offset for solo play.
// The normal input path does not move, orbit or follow pile height.
// Provenance and call-path evidence: Tools/LayeredPile/camera-shadow-alignment.md
// and docs/refactoring/2026-11-04-naming.md.
public sealed class Match3DThemePresentation : MonoBehaviour {
 public bool Active=>visual;
 public const int SourceControlMode=2; // LevelControlType.flatTopView, not an inspector tuning option.
 public static readonly Quaternion SourceRotation=Quaternion.Euler(90f,0,0);
 public static Vector3 SourcePosition=>new Vector3(0,0,.5f)+SourceRotation*new Vector3(0,0,-30);
 GameObject visual;Camera camera;CameraFitter fitter;bool fitted,orthographic;Vector3 position;Quaternion rotation;float size,near,far,fov;Rect rect;CameraClearFlags clear;Color background;bool hdr;int culling;
 Transform pan;
 public static readonly Vector3 PanBowlLocalCenter=new Vector3(.105f,0,0);
 public Vector3 PanBowlCenter=>pan?pan.TransformPoint(PanBowlLocalCenter):Vector3.zero;
 Collider[] legacyColliders;bool[] colliderStates;
 Renderer[] legacy;bool[] enabledStates;BoardLighting lighting;
 public void Begin(string theme){
  End();var prefab=Resources.Load<GameObject>("Match3D/Scenes/"+theme);if(!prefab)throw new System.InvalidOperationException("Missing scene visual "+theme);
  visual=Instantiate(prefab);visual.name=theme+" source visuals";
  AddPanCollision(visual);
  pan=visual.transform.Find("Grab3D Frying Pan");
  if(!lighting)lighting=GetComponent<BoardLighting>();if(!lighting)lighting=gameObject.AddComponent<BoardLighting>();lighting.Begin();
  // The source theme prefab owns the complete backdrop. Hide the legacy
  // Cooking Pot while the source presentation is active so the two bowls do
  // not overlap; restore its original renderer states when leaving the theme.
  // Own both visual and collision suppression for the full presentation lifetime,
  // including first-stage -> second-stage reuse. Hidden renderers still collide.
  var pot=GameObject.Find("Cooking Pot");if(pot){
   legacy=pot.GetComponentsInChildren<Renderer>(true);enabledStates=legacy.Select(r=>r.enabled).ToArray();foreach(var r in legacy)r.enabled=false;
   legacyColliders=pot.GetComponentsInChildren<Collider>(true);colliderStates=legacyColliders.Select(c=>c.enabled).ToArray();foreach(var c in legacyColliders)c.enabled=false;
  }
  camera=Camera.main;if(!camera)return;position=camera.transform.position;rotation=camera.transform.rotation;size=camera.orthographicSize;orthographic=camera.orthographic;near=camera.nearClipPlane;far=camera.farClipPlane;fov=camera.fieldOfView;rect=camera.rect;clear=camera.clearFlags;background=camera.backgroundColor;hdr=camera.allowHDR;culling=camera.cullingMask;
  fitter=camera.GetComponent<CameraFitter>();if(fitter){fitted=fitter.enabled;fitter.enabled=false;}
  camera.transform.SetPositionAndRotation(SourcePosition,SourceRotation);camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=Color.clear;camera.allowHDR=false;camera.cullingMask=503;camera.orthographic=true;camera.nearClipPlane=.3f;camera.farClipPlane=1000;camera.rect=new Rect(0,0,1,1);Fit();
 }
 // The visual and camera are owned by this presentation for the whole run.
 // Stage transitions reuse them without pose changes; a vertical collider does
 // not require rotating the camera. Begin also restores this pose on a restart.
 public static void AddPanCollision(GameObject scene){
  var pan=scene.transform.Find("Grab3D Frying Pan");if(!pan)return;
  // The replacement mesh sits above the recovered source floor. Match its inner
  // bowl exactly so thin items cannot settle underneath the visible surface.
  foreach(var mesh in pan.GetComponentsInChildren<MeshFilter>()){
   mesh.gameObject.layer=ItemColliderGeometry.WallLayer;
   var collider=mesh.GetComponent<MeshCollider>();if(!collider)collider=mesh.gameObject.AddComponent<MeshCollider>();
   collider.sharedMesh=mesh.sharedMesh;collider.convex=false;collider.isTrigger=false;
   collider.sharedMaterial=BoardPhysicsSettings.DefaultSurface;
  }
 }
 // Screen-space item size relative to the original camera; fixed throughout play.
 public const float ItemScreenScale=.9f;
 public static float OriginalSize(float aspect)=>6.5f*Mathf.Max(1,(9f/16)/Mathf.Max(.01f,aspect));
 public static float Size(float aspect)=>OriginalSize(aspect)/ItemScreenScale;
 public void Fit(){if(camera&&visual){camera.orthographicSize=Size(camera.aspect);camera.fieldOfView=60*Mathf.Max(1,(9f/16)/camera.aspect);if(GameplayBoardLayout.Enabled)GameplayBoardLayout.FitBoard(camera,PanBowlCenter,4.1f);}}
 void LateUpdate(){Fit();}
 public void End(){
  if(lighting)lighting.End();
  if(visual){visual.SetActive(false);Destroy(visual);visual=null;}pan=null;
  if(legacy!=null)for(int i=0;i<legacy.Length;i++)if(legacy[i])legacy[i].enabled=enabledStates[i];legacy=null;
  if(legacyColliders!=null)for(int i=0;i<legacyColliders.Length;i++)if(legacyColliders[i])legacyColliders[i].enabled=colliderStates[i];legacyColliders=null;colliderStates=null;
  if(camera){camera.transform.SetPositionAndRotation(position,rotation);camera.orthographic=orthographic;camera.orthographicSize=size;camera.nearClipPlane=near;camera.farClipPlane=far;camera.fieldOfView=fov;camera.rect=rect;camera.clearFlags=clear;camera.backgroundColor=background;camera.allowHDR=hdr;camera.cullingMask=culling;}
  if(fitter)fitter.enabled=fitted;camera=null;fitter=null;
 }
 void OnDestroy(){End();}
}
