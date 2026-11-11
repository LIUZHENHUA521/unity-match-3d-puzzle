using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class MerchantAuthorizationValidation {
    const string Marker="Temp/run-merchant-authorization-check",Output="Library/MerchantAuthorizationValidation";
    static readonly BindingFlags Flags=BindingFlags.Instance|BindingFlags.NonPublic;
    static MerchantAuthorizationValidation(){EditorApplication.update+=Poll;}
    public static void RunBatch(){
        UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/Scenes/Match3D.unity");
        var prefab=UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Match3D/Resources/Match3D/UI/CommonDialog.prefab");
        var dialog=prefab.GetComponent<Match3DCommonDialog>();
        if(dialog.title.transform.parent.Find("Title ribbon").GetSiblingIndex()>=dialog.title.transform.GetSiblingIndex())throw new Exception("Saved ribbon covers title");
        Directory.CreateDirectory(Output);File.Delete(Output+"/result.txt");File.WriteAllText(Marker,"");
        SessionState.SetBool("merchant-auth-batch",true);
    }
    static void Poll(){
        if(SessionState.GetBool("merchant-auth-batch",false)&&!EditorApplication.isPlayingOrWillChangePlaymode&&!File.Exists(Marker)&&File.Exists(Output+"/result.txt")){
            var result=File.ReadAllText(Output+"/result.txt");
            if(result.Contains("COMPLETE")||result.Contains("FAIL")){SessionState.SetBool("merchant-auth-batch",false);EditorApplication.Exit(result.Contains("COMPLETE")?0:1);return;}
        }
        if(File.Exists("Temp/compile-merchant-authorization-webgl")&&!EditorApplication.isCompiling&&!EditorApplication.isUpdating&&!EditorApplication.isPlayingOrWillChangePlaymode){
            File.Delete("Temp/compile-merchant-authorization-webgl");Directory.CreateDirectory(Output);
            try{
                Directory.CreateDirectory(Output+"/assemblies");
                var settings=new UnityEditor.Build.Player.ScriptCompilationSettings{target=BuildTarget.WebGL,group=BuildTargetGroup.WebGL};
                var result=UnityEditor.Build.Player.PlayerBuildInterface.CompilePlayerScripts(settings,Output+"/assemblies");
                if(result.assemblies==null||result.assemblies.Count==0)throw new Exception("No WebGL assemblies");
                File.WriteAllText(Output+"/webgl.txt","PASS WebGL player compilation: "+result.assemblies.Count+" assemblies\n");
            }catch(Exception e){File.WriteAllText(Output+"/webgl.txt","FAIL "+e);}
            return;
        }
        if(!File.Exists(Marker)||EditorApplication.isCompiling||EditorApplication.isUpdating)return;
        if(!EditorApplication.isPlayingOrWillChangePlaymode){SessionState.SetBool("merchant-auth-started-play",true);EditorApplication.isPlaying=true;return;}
        if(!Application.isPlaying||!Match3DFlow.Instance||!Match3DFlow.Instance.StartupComplete||!Match3DMerchantAuthorization.Instance)return;
        File.Delete(Marker);Directory.CreateDirectory(Output);
        new GameObject("Merchant authorization validation").AddComponent<BoardValidationDriver>().StartCoroutine(Guard());
    }
    static Match3DCommonDialog Dialog=>UnityEngine.Object.FindObjectOfType<Match3DCommonDialog>();
    static void Check(bool yes,string message){if(!yes)throw new Exception(message);File.AppendAllText(Output+"/result.txt","PASS "+message+"\n");}
    static IEnumerator Wait(Func<bool> ready,float seconds=8){float deadline=Time.realtimeSinceStartup+seconds;while(!ready()&&Time.realtimeSinceStartup<deadline)yield return null;}
    static void Set(object obj,string field,object value)=>obj.GetType().GetField(field,Flags).SetValue(obj,value);
    static void Setup(string token){
        Match3DMerchantAuthorization.EditorEnabled=false;
        var auth=Match3DMerchantAuthorization.Instance;auth.StopAllCoroutines();Set(auth,"busy",false);Set(auth,"attempted",false);
        Match3DFlow.Instance.Home();
        var account=Match3DInvitations.Instance;
        Set(account,"token",token);typeof(Match3DInvitations).GetProperty("State").SetValue(account,new Match3DInvitations.Reply{inviteCode="fixture-owner",inventory=new int[4]});
        auth.Initialize("http://127.0.0.1:18766");Match3DMerchantAuthorization.EditorEnabled=true;auth.OnHome();
    }
    static IEnumerator Guard(){
        File.WriteAllText(Output+"/result.txt","");
        var account=Match3DInvitations.Instance;var originalToken=typeof(Match3DInvitations).GetField("token",Flags).GetValue(account);var originalState=account.State;
        var auth=Match3DMerchantAuthorization.Instance;var originalEndpoint=typeof(Match3DMerchantAuthorization).GetField("endpoint",Flags).GetValue(auth);
        Exception failure=null;yield return Match3DStartupRunner.Run(Run(),e=>failure=e);
        auth.StopAllCoroutines();Set(account,"token",originalToken);typeof(Match3DInvitations).GetProperty("State").SetValue(account,originalState);Set(auth,"endpoint",originalEndpoint);
        Match3DMerchantAuthorization.EditorEnabled=false;Match3DMerchantAuthorization.EditorOpenOverride=null;
        Match3DFlow.Instance.Home();File.AppendAllText(Output+"/result.txt",failure==null?"COMPLETE\n":"FAIL "+failure);
        if(SessionState.GetBool("merchant-auth-started-play",false)){SessionState.SetBool("merchant-auth-started-play",false);EditorApplication.isPlaying=false;}
    }
    static IEnumerator Run(){
        Setup("pending");yield return Wait(()=>Dialog);yield return new WaitForSecondsRealtime(.5f);
        Check(Dialog&&Dialog.title.text=="开通微信收款授权","unapproved player sees startup prompt");
        Check(Dialog.leftLabel.text=="暂不授权"&&Dialog.rightLabel.text=="立即授权","both choices are clear");
        Check(Match3DFlow.Instance.BlocksGameplay,"prompt blocks underlying home taps");
        foreach(var text in new[]{Dialog.title,Dialog.body,Dialog.leftLabel,Dialog.rightLabel})foreach(char ch in text.text.Where(c=>!char.IsWhiteSpace(c)).Distinct())Check(text.font.HasCharacter(ch),"font contains "+ch);
        Check(Dialog.title.transform.GetSiblingIndex()>Dialog.title.transform.parent.Find("Title ribbon").GetSiblingIndex(),"title renders above its ribbon");
        Capture();Dialog.left.onClick.Invoke();yield return null;
        Check(!Dialog&&!Match3DFlow.Instance.BlocksGameplay,"skip closes prompt and restores interaction");
        Match3DFlow.Instance.Home();yield return new WaitForSecondsRealtime(.2f);Check(!Dialog,"skip is respected on subsequent home visits");
        Setup("authorized");yield return new WaitForSecondsRealtime(.3f);Check(!Dialog,"already authorized player is not prompted");
        Setup("status-error");yield return new WaitForSecondsRealtime(.3f);Check(!Dialog,"status errors are not mistaken for missing consent");
        Setup("delayed");Match3DFlow.Instance.ShowHomeTab(Match3DHomeView.Tab.Collection);yield return new WaitForSecondsRealtime(.7f);Check(!Dialog,"late startup result does not interrupt another screen");
        Setup("slow-prepare");yield return Wait(()=>Dialog);int launches=0;Match3DMerchantAuthorization.EditorOpenOverride=(r,done)=>launches++;
        Dialog.right.onClick.Invoke();Dialog.right.onClick.Invoke();Check(Dialog.left.interactable&&Dialog.close.interactable&&!Dialog.right.interactable,"in-flight request prevents repeats but can be dismissed");
        Dialog.left.onClick.Invoke();yield return new WaitForSecondsRealtime(.7f);Check(!Dialog&&launches==0,"closing during preparation prevents late native launch");
        Setup("pending");yield return Wait(()=>Dialog);Match3DMerchantAuthorization.EditorOpenOverride=(r,done)=>{launches++;done("cancelled");done("returned");};
        Dialog.right.onClick.Invoke();yield return Wait(()=>!Dialog);Check(!Dialog&&launches==1&&!Match3DFlow.Instance.BlocksGameplay,"native cancellation restores play; repeated callback ignored");
        Setup("success");yield return Wait(()=>Dialog);Match3DMerchantAuthorization.EditorOpenOverride=(r,done)=>{Check(r.packageInfo=="fixture-package","native page receives server package");done("returned");};
        Dialog.right.onClick.Invoke();yield return Wait(()=>!Dialog);Check(!Dialog&&!Match3DFlow.Instance.BlocksGameplay,"server confirmed authorization closes prompt");
        Setup("no-consent");yield return Wait(()=>Dialog);Match3DMerchantAuthorization.EditorOpenOverride=(r,done)=>done("returned");Dialog.right.onClick.Invoke();
        yield return Wait(()=>Dialog&&Dialog.right.interactable);Check(Dialog&&Dialog.status.text.Contains("尚未确认"),"native page success alone is not treated as consent");Dialog.close.onClick.Invoke();yield return null;
        Setup("pending");yield return Wait(()=>Dialog);Action<string> stale=null;Match3DMerchantAuthorization.EditorOpenOverride=(r,done)=>stale=done;Dialog.right.onClick.Invoke();yield return Wait(()=>stale!=null);
        typeof(Match3DInvitations).GetProperty("State").SetValue(Match3DInvitations.Instance,new Match3DInvitations.Reply{inviteCode="different-owner",inventory=new int[4]});stale("returned");yield return new WaitForSecondsRealtime(.2f);
        Check(!Dialog&&!Match3DFlow.Instance.BlocksGameplay,"account switch discards prior authorization result");
    }
    static void Capture(){
        var camera=Camera.main;var canvases=UnityEngine.Object.FindObjectsOfType<Canvas>().Where(c=>c.isRootCanvas&&c.gameObject.activeInHierarchy).ToArray();
        var modes=canvases.Select(c=>c.renderMode).ToArray();var cameras=canvases.Select(c=>c.worldCamera).ToArray();var distances=canvases.Select(c=>c.planeDistance).ToArray();
        var rt=new RenderTexture(720,1280,24);rt.Create();var old=camera.targetTexture;var active=RenderTexture.active;camera.targetTexture=rt;
        try{
            for(int i=0;i<canvases.Length;i++){canvases[i].renderMode=RenderMode.ScreenSpaceCamera;canvases[i].worldCamera=camera;canvases[i].planeDistance=1;}
            Canvas.ForceUpdateCanvases();camera.Render();RenderTexture.active=rt;
            var tex=new Texture2D(720,1280,TextureFormat.RGB24,false);tex.ReadPixels(new Rect(0,0,720,1280),0,0);tex.Apply();File.WriteAllBytes(Output+"/startup-prompt.png",tex.EncodeToPNG());UnityEngine.Object.Destroy(tex);
        }finally{camera.targetTexture=old;RenderTexture.active=active;for(int i=0;i<canvases.Length;i++){canvases[i].renderMode=modes[i];canvases[i].worldCamera=cameras[i];canvases[i].planeDistance=distances[i];}rt.Release();UnityEngine.Object.Destroy(rt);}
    }
}
