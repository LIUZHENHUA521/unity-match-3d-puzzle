using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class PotCertificateValidation {
    const string Output="work/pot-certificate-validation/";
    const BindingFlags Flags=BindingFlags.Instance|BindingFlags.NonPublic;
    static int step;static double due;static string report="";
    static bool optionsEnabled;static EnterPlayModeOptions options;
    static Match3DFlow flow;static Match3DInvitations account;static PotCertificateView view;
    static void Set(object o,string n,object v)=>o.GetType().GetField(n,Flags).SetValue(o,v);
    static object Get(object o,string n)=>o.GetType().GetField(n,Flags).GetValue(o);
    static void Check(bool ok,string text){if(!ok)throw new Exception(text);report+="PASS "+text+"\n";}
    public static void Run(){
        Directory.CreateDirectory(Output);PotCertificateBuilder.Build();
        optionsEnabled=EditorSettings.enterPlayModeOptionsEnabled;options=EditorSettings.enterPlayModeOptions;
        EditorSettings.enterPlayModeOptionsEnabled=true;EditorSettings.enterPlayModeOptions=EnterPlayModeOptions.DisableDomainReload;
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);step=0;due=EditorApplication.timeSinceStartup+3;
        EditorApplication.update+=Tick;EditorApplication.isPlaying=true;
    }
    static void Tick(){
        if(EditorApplication.timeSinceStartup<due||!EditorApplication.isPlaying)return;
        try{
            switch(step++){
                case 0:
                    flow=new GameObject("Pot flow fixture").AddComponent<Match3DFlow>();flow.enabled=false;
                    flow.library=Resources.Load<Match3DTheme>("Match3D/ThemeLibrary");
                    // UI-only fixture: no game startup, account login or network transport.
                    if(!flow.library)flow.library=ScriptableObject.CreateInstance<Match3DTheme>();
                    ((CanvasGroup)Get(flow,"startupInput")).interactable=true;
                    Set(flow,"effects",flow.gameObject.AddComponent<AudioSource>());flow.gameObject.AddComponent<AudioListener>();
                    Set(flow,"<ScreenName>k__BackingField","game");Set(flow,"resultShown",true);
                    var page=(RectTransform)Get(flow,"page");foreach(Transform t in page)t.gameObject.SetActive(false);
                    account=new GameObject("Pot account fixture").AddComponent<Match3DInvitations>();account.enabled=false;
                    typeof(Match3DInvitations).GetField("<Instance>k__BackingField",BindingFlags.Static|BindingFlags.NonPublic).SetValue(null,account);
                    Set(account,"<State>k__BackingField",new Match3DInvitations.Reply{inviteCode="alice"});Set(account,"token","fixture");
                    Open("bachelor");Time.timeScale=0;Check(view.Playing,"entrance starts");
                    Check(!CanShow(),"rewards blocked before certificate confirmation");
                    Check(!flow.ReceivePotSettlement("alice","old-run","old-record",null,"old",null),"stale run cannot cover certificate");
                    Check(!flow.ReceivePotSettlement("bob","run","record",null,"foreign",null),"foreign account cannot settle certificate");
                    Capture("entrance",.2f,720,1280);Capture("stamp-flight",.85f,720,1280);break;
                case 1:
                    Check(!view.Playing&&Time.timeScale==0,"unscaled entrance completes while game paused");
                    Check(view.StampSoundCount==1,"stamp audio plays once at impact while paused");view.Skip();Check(view.StampSoundCount==1,"repeated skip does not replay stamp");
                    Check(flow.library.winSound&&flow.library.winSound.name=="pot-victory","crazy combo victory clip assigned");
                    var sound=(AudioSource)Get(flow,"effects");sound.Stop();flow.PlayPotCertificateSound(flow.library.winSound);Check(sound.isPlaying,"victory clip plays on game effect source");
                    Check(!view.status.gameObject.activeSelf&&view.GetComponentsInChildren<Button>().Length==1,"small captions and close button removed");
                    Capture("bachelor",2,720,1280);Capture("bachelor-long",2,720,1560);Capture("bachelor-tablet",2,768,1024);
                    view.claim.onClick.Invoke();Check(view.Confirmed&&!CanShow(),"early confirm waits for settlement");
                    Check(flow.ReceivePotSettlement("alice","run","record",Data("bachelor"),"voucher",null),"matching settlement accepted");
                    Check(CanShow()&&view.gameObject.activeInHierarchy,"reward released and certificate kept for home navigation");
                    view.claim.onClick.Invoke();Check((bool)Get(flow,"potRewardsReleased"),"repeated click keeps one release");
                    Open("elite");view.claim.onClick.Invoke();Check(!view.Confirmed&&!view.Playing,"first animation click only skips");
                    flow.ReceivePotSettlement("alice","run","record",Data("elite"),"voucher2",null);
                    Check(!CanShow(),"fast receipt still waits for player confirmation");
                    var presenter=((Canvas)Get(flow,"canvas")).gameObject.AddComponent<Match3DRewardPresenter>();
                    presenter.Enqueue(new[]{Match3DRewardItem.RedPacket(2)},"联系客服兑换",0,()=>{},()=>account.State.inviteCode=="alice");
                    typeof(Match3DRewardPresenter).GetMethod("Update",Flags).Invoke(presenter,null);Check(!presenter.IsShowing,"queued popup cannot cover certificate");
                    Capture("elite",2,720,1280);view.claim.onClick.Invoke();Check(CanShow(),"fast receipt released after confirmation");
                    typeof(Match3DRewardPresenter).GetMethod("Update",Flags).Invoke(presenter,null);Check(presenter.IsShowing,"confirmed receipt presents actual reward popup");
                    Set(account,"<State>k__BackingField",new Match3DInvitations.Reply{inviteCode="bob"});
                    typeof(Match3DRewardPresenter).GetMethod("Update",Flags).Invoke(presenter,null);Check(!presenter.IsShowing,"account switch dismisses old reward popup");
                    Set(account,"<State>k__BackingField",new Match3DInvitations.Reply{inviteCode="alice"});
                    Open("bachelor");view.Skip();flow.ReceivePotSettlement("alice","run","record",Data("bachelor"),null,"daily_quota_exhausted");view.claim.onClick.Invoke();
                    Check(view.number.text.StartsWith("POT-")&&!view.claim.interactable&&view.status.text.Contains("名额"),"quota failure retains certificate and explains no reward");
                    Capture("quota-exhausted",2,720,1280);
                    Open("bachelor");view.Skip();flow.ReceivePotSettlement("alice","run","record",null,null,"settlement_rejected");view.claim.onClick.Invoke();Check(view.status.text.Contains("未通过验证")&&view.number.text=="未通过验证","permanent rejection is not pending");
                    Open("elite");Set(account,"<State>k__BackingField",new Match3DInvitations.Reply{inviteCode="bob"});
                    typeof(Match3DFlow).GetMethod("CheckPotCertificateOwner",Flags).Invoke(flow,null);
                    Check(!view.gameObject.activeSelf,"account switch clears certificate");
                    var records=new GameObject("Pot records fixture").AddComponent<Match3DGameRecords>();records.enabled=false;
                    typeof(Match3DGameRecords).GetField("<Instance>k__BackingField",BindingFlags.Static|BindingFlags.NonPublic).SetValue(null,records);
                    Set(records,"<LastFinished>k__BackingField",new Match3DGameRecords.Record{stage=2,status="win",recordId="preview",endedAt=1794830000000});
                    Set(flow,"<LocalPreview>k__BackingField",true);Set(flow,"dailyLevel",new Match3DDailyLevel{tier="elite"});
                    Check((bool)typeof(Match3DFlow).GetMethod("ShowPotCertificate",Flags).Invoke(flow,null),"editor daily victory uses real certificate entry");
                    view=(PotCertificateView)Get(flow,"potCertificate");Check(view.number.text=="预览"&&!view.claim.interactable,"editor preview cannot claim real reward");
                    Check(!flow.ReceivePotSettlement("bob",null,"preview",Data("elite"),"invalid",null),"preview ignores reward settlement");
                    Finish(null);return;
            }
            due=EditorApplication.timeSinceStartup+2;
        }catch(Exception e){Finish(e);}
    }
    static bool CanShow()=>(bool)typeof(Match3DFlow).GetProperty("CanShowReceivedRewards",Flags).GetValue(flow);
    static PotCertificateData Data(string tier)=>new PotCertificateData{runId="run",recordId="record",rewardTier=tier,dateKey="2026-11-16",activeDurationMs=713000,certificateNo="POT-1234567890ABCDEF",nickName="微信玩家"};
    static void Open(string tier){
        if(view){view.gameObject.SetActive(false);UnityEngine.Object.Destroy(view.gameObject);}
        var page=(RectTransform)Get(flow,"page");view=Match3DUIView.Load("PotCertificate",page).GetComponent<PotCertificateView>();
        Set(flow,"modal",(RectTransform)view.transform);Set(flow,"potCertificate",view);Set(flow,"potCertificateOwner","alice");Set(flow,"potCertificateRun","run");Set(flow,"potCertificateRecord","record");Set(flow,"potSettled",false);Set(flow,"potRewardsReleased",false);Set(flow,"potVoucher",null);
        view.Configure(Data(tier),()=>typeof(Match3DFlow).GetMethod("ContinuePotCertificate",Flags).Invoke(flow,null));
    }
    static void Capture(string name,float time,int width,int height){
        view.Sample(time);var canvas=(Canvas)Get(flow,"canvas");var camera=new GameObject("Pot capture camera").AddComponent<Camera>();camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=new Color32(37,50,75,255);camera.transform.position=new Vector3(0,0,-10);
        var rt=new RenderTexture(width,height,24);rt.Create();var old=RenderTexture.active;var mode=canvas.renderMode;
        camera.targetTexture=rt;canvas.renderMode=RenderMode.ScreenSpaceCamera;canvas.worldCamera=camera;canvas.planeDistance=1;
        Canvas.ForceUpdateCanvases();view.GetComponent<PotCertificateLayout>().Fit();Canvas.ForceUpdateCanvases();
        foreach(var button in view.GetComponentsInChildren<Button>()){var corners=new Vector3[4];((RectTransform)button.transform).GetWorldCorners(corners);foreach(var corner in corners){var p=camera.WorldToViewportPoint(corner);Check(p.x>=0&&p.x<=1&&p.y>=0&&p.y<=1,name+" button on screen");}}
        if(time>=2){var corners=new Vector3[4];view.card.GetWorldCorners(corners);var left=camera.WorldToViewportPoint(corners[0]);var right=camera.WorldToViewportPoint(corners[2]);Check(Mathf.Abs(left.x)<.002f&&Mathf.Abs(right.x-1)<.002f,name+" diploma fills full viewport width");Check(Mathf.Abs((left.y+right.y)*.5f-.5f)<.002f,name+" diploma vertically centered");}
        camera.Render();RenderTexture.active=rt;var tex=new Texture2D(width,height,TextureFormat.RGB24,false);tex.ReadPixels(new Rect(0,0,width,height),0,0);tex.Apply();File.WriteAllBytes(Output+name+".png",tex.EncodeToPNG());
        canvas.renderMode=mode;canvas.worldCamera=null;RenderTexture.active=old;camera.targetTexture=null;UnityEngine.Object.DestroyImmediate(tex);UnityEngine.Object.DestroyImmediate(rt);UnityEngine.Object.DestroyImmediate(camera.gameObject);
    }
    static void Finish(Exception error){
        EditorApplication.update-=Tick;EditorSettings.enterPlayModeOptionsEnabled=optionsEnabled;EditorSettings.enterPlayModeOptions=options;
        report+=error==null?"ALL CHECKS PASSED\n":"FAIL "+error+"\n";File.WriteAllText(Output+"result.txt",report);Debug.Log(report);EditorApplication.Exit(error==null?0:1);
    }
}
