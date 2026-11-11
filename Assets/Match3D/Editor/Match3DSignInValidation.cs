using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[InitializeOnLoad] public static class Match3DSignInValidation {
    const string Key="Match3DSignInValidation.Pending";
    static Match3DSignInValidation(){EditorApplication.playModeStateChanged+=s=>{
        if(s==PlayModeStateChange.EnteredPlayMode&&SessionState.GetBool(Key,false)){
            SessionState.SetBool(Key,false);new GameObject("Sign in validation").AddComponent<Match3DSignInProbe>();
        }
    };}
    public static void Run(){
        Match3DSignInBuilder.Build();EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
        SessionState.SetBool(Key,true);EditorApplication.EnterPlaymode();
    }
}
public sealed class Match3DSignInProbe : MonoBehaviour {
    const string Output="docs/validation/seven-day-sign-in/";
    const BindingFlags Flags=BindingFlags.Instance|BindingFlags.NonPublic;
    string endpoint;Match3DInvitations account;Match3DFlow flow;Canvas canvas;
    Match3DSignInView View=>FindObjectOfType<Match3DSignInView>();
    Match3DRewardPopup Receipt=>FindObjectOfType<Match3DRewardPopup>();
    void Start(){File.WriteAllText(Output+"result.txt","");StartCoroutine(Guard(Check()));}
    void Assert(bool value,string text){if(!value)throw new Exception(text);File.AppendAllText(Output+"result.txt","PASS "+text+"\n");}
    IEnumerator Guard(IEnumerator root){
        var stack=new Stack<IEnumerator>();stack.Push(root);
        while(stack.Count>0){
            object current=null;bool moved=false;
            try{moved=stack.Peek().MoveNext();if(moved)current=stack.Peek().Current;else stack.Pop();}
            catch(Exception e){File.AppendAllText(Output+"result.txt","FAIL "+e+"\n");EditorApplication.Exit(1);yield break;}
            if(!moved)continue;if(current is IEnumerator nested)stack.Push(nested);else yield return current;
        }
        File.AppendAllText(Output+"result.txt","ALL PASSED\n");EditorApplication.Exit(0);
    }
    IEnumerator Wait(Func<bool> predicate){float deadline=Time.realtimeSinceStartup+25;while(!predicate()&&Time.realtimeSinceStartup<deadline)yield return null;Assert(predicate(),"asynchronous operation completed");}
    IEnumerator Control(string body){using(var req=new UnityWebRequest(endpoint+"/fixture","POST")){
        req.uploadHandler=new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(body));req.downloadHandler=new DownloadHandlerBuffer();yield return req.SendWebRequest();Assert(req.result==UnityWebRequest.Result.Success,"local fixture control");
    }}
    IEnumerator Refresh(){account.RefreshSignIn();yield return Wait(()=>!account.SignInBusy);}
    IEnumerator DismissReceipt(){yield return Wait(()=>Receipt);Assert(Receipt.buttonLabel.text=="开心收下","sign-in uses the existing reward receipt");Receipt.confirm.onClick.Invoke();yield return null;}
    IEnumerator Check(){
        endpoint=File.ReadAllText(Output+"endpoint.txt");
        flow=new GameObject("Sign in flow fixture").AddComponent<Match3DFlow>();flow.enabled=false;
        flow.library=ScriptableObject.CreateInstance<Match3DTheme>();
        canvas=(Canvas)typeof(Match3DFlow).GetField("canvas",Flags).GetValue(flow);
        canvas.GetComponent<CanvasGroup>().interactable=true;
        var page=(RectTransform)typeof(Match3DFlow).GetField("page",Flags).GetValue(flow);
        foreach(Transform child in page)Destroy(child.gameObject);yield return null;
        account=new GameObject("Sign in account fixture").AddComponent<Match3DInvitations>();account.Initialize(endpoint);
        var home=Match3DHomeView.Create(flow,page,Match3DHomeView.Tab.Home);yield return null;
        Assert(home.signIn&&home.signIn.gameObject.activeInHierarchy,"real home entry is visible");
        account.OnInvitationBridge("{\"code\":\"signin-fixture-"+Guid.NewGuid().ToString("N")+"\"}");
        yield return Wait(()=>account.Ready&&!account.Busy&&!account.SignInBusy&&account.SignInState!=null);
        Assert(account.SignInCurrent&&account.SignInState.day==1&&account.Inventory(0)==0,"login obtains server sign-in state without granting");
        Assert(home.signIn.GetComponentInChildren<Match3DCircleGraphic>().enabled,"claimable reward lights home badge");
        home.signIn.onClick.Invoke();yield return Wait(()=>View&&!account.SignInBusy&&!View.GetComponent<Match3DModalMotion>().Playing);yield return new WaitForSecondsRealtime(.2f);
        Assert(flow.BlocksGameplay&&View.claim.interactable&&View.buttonLabel.text=="立即签到","home click opens actual modal and enables claim");
        Assert(View.cards[0].highlight.activeSelf&&!View.cards[0].claimed.activeSelf&&View.cards[1].tag.text=="待解锁","today and future card states");
        Capture(720,1560,"day1-long");Capture(720,960,"day1-short");
        yield return Control("{\"drop\":1}");
        View.claim.onClick.Invoke();View.claim.onClick.Invoke();
        Assert(account.SignInBusy&&!View.claim.interactable,"rapid double click locks claim immediately");
        yield return Wait(()=>!account.SignInBusy);
        Assert(account.Inventory(0)==1&&account.SignInState.claimedDays==1&&!account.SignInState.canClaim,"lost response is retried without double grant");
        Assert(!home.signIn.GetComponentInChildren<Match3DCircleGraphic>().enabled,"successful claim clears home badge");
        yield return DismissReceipt();
        Assert(View.cards[0].claimed.activeSelf&&!View.claim.interactable&&View.buttonLabel.text=="今日已签到","claimed overlay and daily disabled button");
        View.GetComponent<Match3DUIView>().Get<Button>("Close").onClick.Invoke();yield return null;
        Assert(!flow.BlocksGameplay&&!View,"close restores home interaction");
        home.signIn.onClick.Invoke();yield return Wait(()=>View&&!account.SignInBusy&&!View.GetComponent<Match3DModalMotion>().Playing);
        Assert(account.Inventory(0)==1&&!View.claim.interactable,"reopen does not credit again");
        yield return Control("{\"advanceDays\":3}");yield return Refresh();
        Assert(account.SignInState.day==2&&account.SignInState.canClaim,"missed days preserve cumulative progress");
        yield return Control("{\"reject\":true}");
        View.claim.onClick.Invoke();yield return Wait(()=>!account.SignInBusy);
        Assert(account.Inventory(3)==0&&!account.SignInCurrent&&View.buttonLabel.text=="刷新重试","server failure does not credit locally and permits refresh");
        yield return Control("{\"reject\":false}");View.claim.onClick.Invoke();yield return Wait(()=>!account.SignInBusy);
        Assert(account.SignInCurrent&&account.SignInState.canClaim&&account.Inventory(3)==0,"refresh reconciles without auto claiming");
        View.claim.onClick.Invoke();yield return Wait(()=>!account.SignInBusy);yield return DismissReceipt();
        Assert(account.Inventory(3)==1,"day two add-time enters permanent inventory");
        for(int day=3;day<=7;day++){
            yield return Control("{\"advanceDays\":1}");yield return Refresh();
            Assert(account.SignInState.day==day&&View.cards[day-1].highlight.activeSelf,"next card unlocked on server day "+day);
            if(day==7){yield return new WaitForSecondsRealtime(.3f);Capture(720,1280,"day7-ready");}
            View.claim.onClick.Invoke();yield return Wait(()=>!account.SignInBusy);yield return DismissReceipt();
        }
        Assert(account.Inventory(0)==5&&account.Inventory(1)==4&&account.Inventory(2)==4&&account.Inventory(3)==5,"all seven rewards total 18 usable tools");
        Assert(account.SignInState.cycle==1&&account.SignInState.claimedDays==7&&!View.claim.interactable,"day seven remains complete for remainder of day");
        yield return new WaitForSecondsRealtime(.2f);Capture(720,1280,"day7-complete");
        yield return Control("{\"advanceDays\":1}");
        account.SendMessage("OnApplicationPause",false);yield return Wait(()=>!account.SignInBusy);
        Assert(account.SignInState.cycle==2&&account.SignInState.day==1&&account.SignInState.claimedDays==0&&View.claim.interactable,"resume refresh starts next cycle on next day");
        yield return account.Use(0,()=>true);Assert(account.Inventory(0)==4&&account.SignInState.cycle==2,"sign-in rewards use existing inventory debit without losing progress");
        // Server-timed expiry, independent of device calendar.
        typeof(Match3DInvitations).GetField("signInRefreshAt",Flags).SetValue(account,0f);
        typeof(Match3DInvitations).GetField("signInRetryAt",Flags).SetValue(account,0f);
        yield return Wait(()=>account.SignInCurrent&&!account.SignInBusy);
        Assert(account.SignInState.cycle==2,"midnight timer refreshes state automatically");
        yield return Control("{\"unauthorized\":true}");yield return Refresh();
        Assert(!account.Ready&&account.SignInState==null&&!home.signIn.GetComponentInChildren<Match3DCircleGraphic>().enabled,"expired identity clears sign-in state and badge");
        account.OnInvitationBridge("{\"code\":\"other-signin-fixture-"+Guid.NewGuid().ToString("N")+"\"}");
        yield return Wait(()=>account.Ready&&!account.Busy&&!account.SignInBusy&&account.SignInState!=null);
        Assert(account.Inventory(0)==0&&account.SignInState.cycle==1&&account.SignInState.claimedDays==0,"different account never sees previous account progress");
    }
    void Capture(int width,int height,string name){
        var camera=new GameObject("Sign in capture camera").AddComponent<Camera>();camera.clearFlags=CameraClearFlags.SolidColor;
        camera.backgroundColor=new Color(.14f,.18f,.22f);var target=new RenderTexture(width,height,24);target.Create();camera.targetTexture=target;
        canvas.renderMode=RenderMode.ScreenSpaceCamera;canvas.worldCamera=camera;canvas.planeDistance=1;
        camera.Render();Canvas.ForceUpdateCanvases();foreach(var fit in canvas.GetComponentsInChildren<Match3DUIFitContent>())fit.Fit();Canvas.ForceUpdateCanvases();
        foreach(var button in View.GetComponentsInChildren<Button>()){
            var corners=new Vector3[4];button.GetComponent<RectTransform>().GetWorldCorners(corners);
            foreach(var corner in corners){var p=camera.WorldToViewportPoint(corner);Assert(p.x>=0&&p.x<=1&&p.y>=0&&p.y<=1,name+" button within screen");}
        }
        foreach(var label in View.GetComponentsInChildren<Text>()){
            if(string.IsNullOrEmpty(label.text))continue;
            label.cachedTextGenerator.Populate(label.text,label.GetGenerationSettings(label.rectTransform.rect.size));
            Assert(label.cachedTextGenerator.characterCountVisible>=label.text.Replace(" ","").Length,name+" text fits: "+label.text);
        }
        camera.Render();var old=RenderTexture.active;RenderTexture.active=target;var texture=new Texture2D(width,height,TextureFormat.RGB24,false);
        texture.ReadPixels(new Rect(0,0,width,height),0,0);texture.Apply();File.WriteAllBytes(Output+name+".png",texture.EncodeToPNG());
        RenderTexture.active=old;canvas.renderMode=RenderMode.ScreenSpaceOverlay;canvas.worldCamera=null;camera.targetTexture=null;
        Destroy(texture);target.Release();Destroy(target);Destroy(camera.gameObject);
    }
}
