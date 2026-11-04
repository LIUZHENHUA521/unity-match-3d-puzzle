using System;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
[InitializeOnLoad] public static class Match3DMailValidation {
 const string Output="work/mail-validation";
 static Match3DMailValidation(){EditorApplication.playModeStateChanged+=s=>{if(s==PlayModeStateChange.EnteredPlayMode&&SessionState.GetBool("mail-validation",false))EditorApplication.delayCall+=Start;};}
 public static void Run(){Match3DMailBuilder.Build();Directory.CreateDirectory(Output);SessionState.SetBool("mail-validation",true);EditorSceneManager.OpenScene(Match3DBuilder.Scene);EditorApplication.isPlaying=true;}
 static void Start(){new GameObject("Mail validation driver").AddComponent<BoardValidationDriver>().StartCoroutine(Guard(Check()));}
 static IEnumerator Guard(IEnumerator routine){while(true){object current;try{if(!routine.MoveNext())break;current=routine.Current;}catch(Exception e){Finish(e.ToString());yield break;}yield return current;}Finish(null);}
 static void Assert(bool value,string label){if(!value)throw new Exception(label);File.AppendAllText(Output+"/result.txt","PASS "+label+"\n");}
 static IEnumerator Idle(Match3DInvitations service){float until=Time.realtimeSinceStartup+20;while(service.Busy&&Time.realtimeSinceStartup<until)yield return null;}
 static IEnumerator Check(){
  File.WriteAllText(Output+"/result.txt","");float deadline=Time.realtimeSinceStartup+30;while((!Match3DFlow.Instance||!Match3DInvitations.Instance)&&Time.realtimeSinceStartup<deadline)yield return null;
  var flow=Match3DFlow.Instance;var service=Match3DInvitations.Instance;Assert(flow&&service,"game initialized");
  service.Initialize("http://127.0.0.1:8099");service.OnInvitationBridge("{\"code\":\"mail-fixture-user\"}");yield return Idle(service);Assert(service.Ready&&service.MailLoaded&&service.Mails.Count==20&&service.MailNextCursor==20,"login and first page");
  var home=UnityEngine.Object.FindObjectOfType<Match3DHomeView>();home.GetComponent<Match3DUIView>().Get<Button>("Settings").onClick.Invoke();yield return Idle(service);yield return null;var menu=UnityEngine.Object.FindObjectOfType<Match3DHomeMenu>();Assert(menu&&menu.mailDot.activeSelf,"menu entry and red dot");Capture("menu");menu.GetComponent<Match3DUIView>().Get<Button>("Mail").onClick.Invoke();yield return Idle(service);yield return null;var view=UnityEngine.Object.FindObjectOfType<Match3DMailboxView>();Assert(view&&flow.BlocksGameplay,"menu opens mailbox");Capture("mailbox");
  var row=view.list.GetComponentsInChildren<Match3DUIView>().First();row.Get<Button>("Open").onClick.Invoke();yield return Idle(service);yield return null;var dialog=UnityEngine.Object.FindObjectOfType<Match3DCommonDialog>();Assert(dialog&&dialog.title.text=="道具领取","configurable claim dialog");Capture("claim-dialog");string savedBody=dialog.body.text;dialog.body.text=string.Join("\n",Enumerable.Repeat("长邮件内容测试：领取前请阅读说明。",50));Canvas.ForceUpdateCanvases();var scroll=dialog.body.GetComponentInParent<ScrollRect>();Assert(scroll.content.rect.height>scroll.viewport.rect.height,"long body scrolls");dialog.body.text=savedBody;Assert(service.Inventory(0)==0,"opening does not grant");dialog.left.onClick.Invoke();yield return null;Assert(!UnityEngine.Object.FindObjectOfType<Match3DCommonDialog>()&&service.Inventory(0)==0,"cancel does not grant");
  row=view.list.GetComponentsInChildren<Match3DUIView>().First();row.Get<Button>("Open").onClick.Invoke();yield return Idle(service);dialog=UnityEngine.Object.FindObjectOfType<Match3DCommonDialog>();dialog.right.onClick.Invoke();yield return Idle(service);yield return null;Assert(service.Inventory(0)==2&&service.Inventory(1)==3&&service.Inventory(2)==1,"confirm credits actual inventory");Assert(!UnityEngine.Object.FindObjectOfType<Match3DCommonDialog>(),"success closes detail");
  service.ClaimMail("fixture-mail-023",ok=>{});yield return Idle(service);Assert(service.Inventory(0)==2,"claim retry does not duplicate");service.RefreshMail(true);yield return Idle(service);Assert(service.Mails.Count==24&&service.MailNextCursor==-1,"load more pagination");
  row=view.list.GetComponentsInChildren<Match3DUIView>().First();row.Get<Button>("Open").onClick.Invoke();yield return Idle(service);dialog=UnityEngine.Object.FindObjectOfType<Match3DCommonDialog>();Assert(dialog&&!dialog.right.interactable&&dialog.rightLabel.text=="已领取","claimed action disabled");dialog.close.onClick.Invoke();yield return null;
  view.GetComponent<Match3DUIView>().Get<Button>("Close").onClick.Invoke();yield return null;Assert(!flow.BlocksGameplay,"close restores input");
  // A later account has no historical all-server mail.
  yield return new WaitForSecondsRealtime(.05f);service.OnInvitationBridge("{\"code\":\"mail-new-user\"}");yield return Idle(service);flow.ShowMailbox();yield return Idle(service);yield return null;view=UnityEngine.Object.FindObjectOfType<Match3DMailboxView>();Assert(service.Mails.Count==0&&view.status.text=="暂时没有邮件","empty mailbox");Capture("empty");
 }
 static void Capture(string name){
  var camera=Camera.main;var canvases=UnityEngine.Object.FindObjectsOfType<Canvas>().Where(c=>c.isRootCanvas&&c.gameObject.activeInHierarchy).ToArray();var modes=canvases.Select(c=>c.renderMode).ToArray();var cameras=canvases.Select(c=>c.worldCamera).ToArray();var distances=canvases.Select(c=>c.planeDistance).ToArray();var target=new RenderTexture(720,1280,24);var old=camera.targetTexture;var active=RenderTexture.active;target.Create();camera.targetTexture=target;
  for(int i=0;i<canvases.Length;i++){canvases[i].renderMode=RenderMode.ScreenSpaceCamera;canvases[i].worldCamera=camera;canvases[i].planeDistance=1;}
  Canvas.ForceUpdateCanvases();foreach(var home in UnityEngine.Object.FindObjectsOfType<Match3DHomeView>())home.SendMessage("Layout");Canvas.ForceUpdateCanvases();foreach(var fit in UnityEngine.Object.FindObjectsOfType<Match3DUIFitContent>())fit.Fit();Canvas.ForceUpdateCanvases();camera.Render();RenderTexture.active=target;var tex=new Texture2D(720,1280,TextureFormat.RGB24,false);tex.ReadPixels(new Rect(0,0,720,1280),0,0);tex.Apply();File.WriteAllBytes(Output+"/"+name+"-720x1280.png",tex.EncodeToPNG());camera.targetTexture=old;RenderTexture.active=active;
  for(int i=0;i<canvases.Length;i++){canvases[i].renderMode=modes[i];canvases[i].worldCamera=cameras[i];canvases[i].planeDistance=distances[i];}UnityEngine.Object.Destroy(tex);target.Release();UnityEngine.Object.Destroy(target);
 }
 static void Finish(string error){SessionState.SetBool("mail-validation",false);File.AppendAllText(Output+"/result.txt",error==null?"ALL PASSED\n":"FAIL "+error);Debug.Log(File.ReadAllText(Output+"/result.txt"));EditorApplication.Exit(error==null?0:1);}
}
