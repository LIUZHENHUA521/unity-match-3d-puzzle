using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEditor;
using UnityEngine.UI;
public static class Match3DInviteValidation {
 public static void BuildAndCheck(){
  Match3DInviteBuilder.Build();
  UnityEditor.SceneManagement.EditorSceneManager.NewScene(UnityEditor.SceneManagement.NewSceneSetup.EmptyScene);
  const string output="docs/validation/invite-add-time";Directory.CreateDirectory(output);
  var camera=new GameObject("Preview camera").AddComponent<Camera>();camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=new Color(.2f,.25f,.3f);
  var canvas=new GameObject("Preview canvas",typeof(RectTransform)).AddComponent<Canvas>();canvas.renderMode=RenderMode.ScreenSpaceCamera;canvas.worldCamera=camera;canvas.planeDistance=10;
  var scaler=canvas.gameObject.AddComponent<CanvasScaler>();scaler.uiScaleMode=CanvasScaler.ScaleMode.ConstantPixelSize;
  var prefab=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Match3D/Resources/Match3D/UI/InviteRewards.prefab");
  var root=UnityEngine.Object.Instantiate(prefab,canvas.transform);var rect=(RectTransform)root.transform;
  rect.anchorMin=Vector2.zero;rect.anchorMax=Vector2.one;rect.offsetMin=rect.offsetMax=Vector2.zero;
  var row=root.transform.Find("Content/道具奖励");Assert(row&&row.childCount==4,"four visible tools");
  string[] names={"移出","吸附","加时","洗牌"};string[] icons={"image_btn_remove","image_btn_adsorpt","img_btn_clock","image_btn_shuffle"};
  for(int i=0;i<4;i++){
   var cell=row.GetChild(i);Assert(cell.name==names[i],"toolbar display order");
   var icon=cell.Find("图标").GetComponent<Image>();Assert(icon.sprite&&icon.sprite.name==icons[i]&&icon.preserveAspect&&!icon.raycastTarget,"tool artwork");
   Assert(cell.Find("名称").GetComponent<Image>().sprite.name==names[i],"tool name artwork");
  }
  var view=root.GetComponent<Match3DUIView>();Assert(view.Get<Button>("Invite")&&view.Get<Button>("Close"),"button bindings");
  foreach(int height in new[]{1280,960}){
   var target=new RenderTexture(720,height,24);target.Create();camera.targetTexture=target;
   // The camera canvas adopts the render target dimensions on its first render.
   camera.Render();Canvas.ForceUpdateCanvases();root.GetComponent<Match3DUIFitContent>().Fit();Canvas.ForceUpdateCanvases();
   Assert(Mathf.Abs(rect.rect.height-height)<1,"preview uses target dimensions");
   var corners=new Vector3[4];view.Get<Button>("Invite").GetComponent<RectTransform>().GetWorldCorners(corners);
   foreach(var corner in corners){var point=camera.WorldToViewportPoint(corner);Assert(point.x>=0&&point.x<=1&&point.y>=0&&point.y<=1,"invite button fits screen");}
   camera.Render();var active=RenderTexture.active;RenderTexture.active=target;
   var image=new Texture2D(720,height,TextureFormat.RGB24,false);image.ReadPixels(new Rect(0,0,720,height),0,0);image.Apply();
   File.WriteAllBytes(output+"/invite-"+height+".png",image.EncodeToPNG());RenderTexture.active=active;camera.targetTexture=null;
   UnityEngine.Object.DestroyImmediate(image);target.Release();UnityEngine.Object.DestroyImmediate(target);
  }
  UnityEngine.Object.DestroyImmediate(root);UnityEngine.Object.DestroyImmediate(canvas.gameObject);UnityEngine.Object.DestroyImmediate(camera.gameObject);
  File.WriteAllText(output+"/layout-result.txt","PASS four tool icons and names; toolbar order; button bindings; 720x1280 and 720x960 layout; Unity prefab rebuild\n");
 }
 public static void Run(){Match3DInvitations.Instance.StartCoroutine(Check());}
 static IEnumerator Check(){
  var service=Match3DInvitations.Instance;service.Initialize("http://127.0.0.1:8098");string user="test-"+Guid.NewGuid().ToString("N");
  service.OnInvitationBridge("{\"code\":\""+user+"\"}");while(service.Busy)yield return null;
  Assert(service.Ready,"login");
  using(var req=UnityWebRequest.Put("http://127.0.0.1:8098/invites/login",System.Text.Encoding.UTF8.GetBytes("{\"code\":\"friend-"+user+"\",\"inviter\":\""+service.State.inviteCode+"\"}"))){req.method="POST";req.SetRequestHeader("Content-Type","application/json");yield return req.SendWebRequest();Assert(req.result==UnityWebRequest.Result.Success,"friend login");}
  service.OnInvitationBridge("{\"code\":\""+user+"\"}");while(service.Busy)yield return null;
  Assert(service.State.invited==1&&service.Inventory(0)==1&&service.Inventory(1)==1&&service.Inventory(2)==1&&service.Inventory(3)==1,"four tool rewards");
  BoardTools.Reset();Assert(BoardTools.TrySpend(0),"free use");Assert(!BoardTools.TrySpend(0),"cannot bypass server debit");
  int calls=0;yield return service.Use(0,()=>BoardTools.RunPaid(0,()=>{if(BoardTools.TrySpend(0))calls++;}));
  Assert(calls==1&&service.Inventory(0)==0&&!service.HasPending,"paid once");
  yield return service.Use(1,()=>false);Assert(service.Inventory(1)==1&&!service.HasPending,"invalid effect refunded");
  // Simulate shutdown with an unresolved debit, then authenticate again and recover.
  string pending="{\"type\":2,\"requestId\":\""+Guid.NewGuid().ToString("N")+"\"}";
  UnityEngine.PlayerPrefs.SetString("goose.invites.pending."+service.State.inviteCode,pending);UnityEngine.PlayerPrefs.Save();
  service.OnInvitationBridge("{\"code\":\""+user+"\"}");while(service.Busy)yield return null;
  Assert(service.Inventory(2)==1&&!service.HasPending,"unfinished request canceled");
  var popup=UnityEngine.Object.FindObjectOfType<Match3DInviteView>();if(popup)popup.GetComponent<Match3DUIView>().Get<UnityEngine.UI.Button>("Close").onClick.Invoke();
  Assert(!Match3DFlow.Instance.BlocksGameplay,"close unblocks");
  File.WriteAllText("Temp/invitation-tests/unity-result.txt","PASS login; new-user reward; free use; bypass denied; paid once; invalid effect refund; pending recovery; close modal");
 }
 static void Assert(bool pass,string name){if(!pass)throw new Exception("Invitation validation failed: "+name);}
}
