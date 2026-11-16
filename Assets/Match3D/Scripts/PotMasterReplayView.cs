using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public sealed class PotMasterReplayView : MonoBehaviour {
    const float RowHeight=112,RowGap=12,RowStride=RowHeight+RowGap,ViewportHeight=856;
    public RectTransform content;public Text status;public Button retry,close;
    Match3DFlow flow;Action dismiss;string owner,date;PotReplayService.Reply list;
    readonly PotMasterReplayAccess access=new PotMasterReplayAccess();
    Match3DRewardedVideo ads;bool loading;int generation;float nextRefresh;
    public static RectTransform Rect(Transform parent,string name,float x,float y,float w,float h){
        var r=new GameObject(name,typeof(RectTransform)).GetComponent<RectTransform>();r.SetParent(parent,false);
        r.anchorMin=r.anchorMax=r.pivot=new Vector2(0,1);r.anchoredPosition=new Vector2(x,-y);r.sizeDelta=new Vector2(w,h);return r;
    }
    public static Image Art(Transform parent,string name,string asset,float x,float y,float w,float h){
        var i=Rect(parent,name,x,y,w,h).gameObject.AddComponent<Image>();i.sprite=Resources.Load<Sprite>("Match3D/PotMasterReplaySkin/"+asset);i.raycastTarget=false;return i;
    }
    public static Text Label(Transform parent,string name,string value,float x,float y,float w,float h,int size){
        var t=Rect(parent,name,x,y,w,h).gameObject.AddComponent<Text>();t.font=Match3DDefaultFont.Current;t.fontSize=size;t.fontStyle=FontStyle.Bold;t.color=new Color32(0,130,218,255);t.alignment=TextAnchor.MiddleCenter;t.raycastTarget=false;t.supportRichText=false;t.text=value;return t;
    }
    public static Button MakeButton(Transform parent,string name,string label,float x,float y,float w,float h,int size=25){
        var art=Art(parent,name,"pot-button",x,y,w,h);art.raycastTarget=true;var b=art.gameObject.AddComponent<Button>();b.targetGraphic=art;b.navigation=new Navigation{mode=Navigation.Mode.None};
        var text=Label(art.transform,"Label",label,0,0,w,h-8,size);text.color=Color.white;var outline=text.gameObject.AddComponent<Outline>();outline.effectColor=new Color32(0,100,170,255);outline.effectDistance=new Vector2(1,-1);return b;
    }
    public static PotMasterReplayView Create(Transform parent){
        var root=Rect(parent,"PotMasterReplays",0,0,720,1280);root.anchorMin=Vector2.zero;root.anchorMax=Vector2.one;root.offsetMin=root.offsetMax=Vector2.zero;
        root.gameObject.AddComponent<Image>().color=new Color(0,0,0,.7f);
        var design=Rect(root,"Pot PSD layout",0,0,720,1280);design.anchorMin=design.anchorMax=design.pivot=new Vector2(.5f,.5f);
        root.gameObject.AddComponent<Match3DUIFitContent>().content=design;
        var view=root.gameObject.AddComponent<PotMasterReplayView>();
        Art(design,"Panel","pot-panel",65,35,592,1200);
        var close=Art(design,"Close","pot-close",581,117,81,81);close.raycastTarget=true;view.close=close.gameObject.AddComponent<Button>();view.close.targetGraphic=close;
        var viewport=Rect(design,"Viewport",81,295,558,ViewportHeight);viewport.gameObject.AddComponent<Image>().color=Color.clear;viewport.gameObject.AddComponent<RectMask2D>();
        view.content=Rect(viewport,"Rows",0,0,558,ViewportHeight);var scroll=viewport.gameObject.AddComponent<ScrollRect>();scroll.viewport=viewport;scroll.content=view.content;scroll.horizontal=false;scroll.movementType=ScrollRect.MovementType.Clamped;
        view.status=Label(design,"Status","正在加载…",100,1162,520,48,22);
        view.retry=MakeButton(design,"Retry","重试",270,670,180,72);view.retry.gameObject.SetActive(false);
        return view;
    }
    public void Connect(Match3DFlow f,Action onClose){
        flow=f;dismiss=onClose;owner=PotMasterReplayAccess.Account();date=Match3DFlow.Today;
        ads=gameObject.AddComponent<Match3DRewardedVideo>();gameObject.name="PotMasterReplay-"+Guid.NewGuid().ToString("N");
        close.onClick.AddListener(()=>dismiss());retry.onClick.AddListener(Fetch);Fetch();
    }
    bool Current()=>this&&isActiveAndEnabled&&owner==PotMasterReplayAccess.Account()&&date==Match3DFlow.Today;
    void Update(){if(!Current()){access.Cancel();dismiss?.Invoke();return;}if(!loading&&!access.Busy&&Time.realtimeSinceStartup>=nextRefresh)Fetch();}
    void OnDisable(){generation++;access.Cancel();if(ads)ads.Cancel();}
    void Fetch(){
        if(loading||access.Busy)return;
        nextRefresh=Time.realtimeSinceStartup+30;
        if(string.IsNullOrEmpty(owner)||!PotReplayService.Instance){status.text="回放服务暂不可用，请稍后重试";retry.gameObject.SetActive(true);return;}
        loading=true;nextRefresh=Time.realtimeSinceStartup+30;int request=++generation;status.text="正在加载…";retry.gameObject.SetActive(false);
        StartCoroutine(Match3DStartupRunner.Run(PotReplayService.Instance.Masters(reply=>{
            if(!Current()||request!=generation)return;loading=false;
            if(reply==null||!reply.ok){status.text=reply?.error??"加载失败，请重试";retry.gameObject.SetActive(true);return;}
            Render(reply);
        }),error=>{if(!Current()||request!=generation)return;loading=false;status.text="加载失败，请重试";retry.gameObject.SetActive(true);}));
    }
    public void Render(PotReplayService.Reply reply){
        if(list!=null&&list.levelVersion!=reply.levelVersion)access.Cancel();
        list=reply;foreach(Transform child in content)Destroy(child.gameObject);
        if(!PotMasterReplayAccess.Visible(reply,date)){status.text="当前关卡通关回放不足 6 个";content.sizeDelta=new Vector2(558,ViewportHeight);return;}
        int index=0;foreach(var item in reply.list){
            if(index>=10)break;var selected=item;
            var row=Art(content,"Pot replay row "+(index+1),"pot-row",0,index*RowStride,558,RowHeight);
            Label(row.transform,"Rank",(++index).ToString(),14,28,40,50,30);
            var avatar=Rect(row.transform,"Avatar",61,19,74,74).gameObject.AddComponent<PotAvatarGraphic>();avatar.color=new Color32(193,235,250,255);avatar.raycastTarget=false;
            avatar.gameObject.AddComponent<PotMasterReplayAvatar>().Load(selected.avatarUrl);
            var name=Label(row.transform,"Player",string.IsNullOrWhiteSpace(item.nickName)?"微信玩家":item.nickName,154,29,158,50,24);name.resizeTextForBestFit=true;name.resizeTextMinSize=16;name.resizeTextMaxSize=24;
            int seconds=Mathf.Max(0,Mathf.FloorToInt(item.duration));Label(row.transform,"Duration",$"{seconds/60:00}:{seconds%60:00}",322,29,96,50,24);
            MakeButton(row.transform,"Watch","回放",437,30,105,64,25).onClick.AddListener(()=>Watch(selected));
        }
        content.sizeDelta=new Vector2(558,Mathf.Max(ViewportHeight,index*RowStride-RowGap));status.text="观看完整视频后播放所选回放";
    }
    void Watch(PotReplayService.Summary row){
        if(!Current()||loading||list==null)return;
        int request=access.Begin(row.id);if(request<0)return;
        if(access.Paid(row.id)){Download(row,request);return;}
        status.text="正在加载视频…";
        ads.ShowPotMasterReplay((ok,error)=>{
            if(!Current()||!access.Current(request))return;
            if(!access.CompleteAd(request,ok)){status.text=string.IsNullOrEmpty(error)?"完整观看视频后才能观看回放":error;return;}
            Download(row,request);
        });
    }
    void Download(PotReplayService.Summary row,int request){
        status.text="正在确认回放…";
        StartCoroutine(Match3DStartupRunner.Run(DownloadCurrent(row,request),error=>{
            if(!Current()||!access.Current(request))return;access.Failed(request);status.text="网络异常，点击同一回放可重试";
        }));
    }
    IEnumerator DownloadCurrent(PotReplayService.Summary row,int request){
        // Recheck the current daily version after the video; an admin may replace it while the ad is playing.
        PotReplayService.Reply latest=null;yield return PotReplayService.Instance.Masters(reply=>latest=reply);
        if(!Current()||!access.Current(request))yield break;
        if(latest==null||!latest.ok){access.Failed(request);status.text="确认失败，点击同一回放可重试";yield break;}
        if(!PotMasterReplayAccess.Visible(latest,date)||latest.levelVersion!=list.levelVersion||!Array.Exists(latest.list,item=>item.id==row.id)){
            access.Cancel();Render(latest);status.text="关卡或回放列表已更新，请重新选择";yield break;
        }
        status.text="正在下载回放…";
        flow.OpenPotMasterReplay(row.id,list.levelVersion,()=>Current()&&access.Current(request),error=>{if(!Current())return;access.Failed(request);status.text=error+"，点击同一回放可重试";},access.Consumed);
    }
}
