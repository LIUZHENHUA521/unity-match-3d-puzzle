using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public partial class Match3DFlow {
    public Match3DDailyLevel PotDailyLevel=>dailyLevel;
    RectTransform potReplayOverlay;int potReplayListVersion;string potReplayTier="bachelor";
    Match3DTips potReplayDownloadTip;bool potReplayDownloading;int potReplayDownloadVersion;
    public RectTransform PotReplayOverlay(bool full){
        if(potReplayOverlay)Destroy(potReplayOverlay.gameObject);
        potReplayOverlay=PotReplayUI.Rect(page,"Pot replay overlay",Vector2.zero,new Vector2(720,full?1280:190));
        potReplayOverlay.gameObject.AddComponent<Image>().color=new Color(.96f,.97f,.92f,.97f);return potReplayOverlay;
    }
    public void ShowPotReplays(bool masters=false){
        if(PotReplayPlayer.Active)return;
        if(masters){ShowPotMasterReplays();return;}
        StopBoard();Clear();ScreenName="pot-replays";BlocksGameplay=true;
        DrawPotReplayList(masters,0);
    }
    void DrawPotReplayList(bool masters,int skip){
        CancelPotReplayDownloadTip();
        int version=++potReplayListVersion;var root=PotReplayOverlay(true);
        PotReplayUI.Label(root,"每日挑战回放",new Vector2(0,-60),new Vector2(650,70));
        PotReplayUI.Button(root,"我的回放",new Vector2(-190,-150),()=>DrawPotReplayList(false,0));
        PotReplayUI.Button(root,"高手回放",new Vector2(0,-150),()=>ShowPotMasterReplays());
        PotReplayUI.Button(root,"返回",new Vector2(190,-150),()=>{Home();ShowPotGlory();});
        if(masters){PotReplayUI.Button(root,"本科局",new Vector2(-100,-230),()=>{potReplayTier="bachelor";DrawPotReplayList(true,0);});PotReplayUI.Button(root,"清北局",new Vector2(100,-230),()=>{potReplayTier="elite";DrawPotReplayList(true,0);});}
        var label=PotReplayUI.Label(root,"正在加载…",new Vector2(0,masters?-290:-230),new Vector2(650,100));
        if(!PotReplayService.Instance){label.text="回放服务未连接";return;}
        ShowPotReplayLoadingTip("加载回放中");
        StartCoroutine(Match3DStartupRunner.Run(PotReplayService.Instance.List(masters,skip,dailyLevel,reply=>{
            if(version!=potReplayListVersion||!label||ScreenName!="pot-replays")return;
            CancelPotReplayDownloadTip();
            if(!reply.ok){label.text=reply.error;return;}
            label.text=reply.list==null||reply.list.Length==0?"暂无回放":masters?"同关卡已公开的完整通关录像":"最近的每日挑战录像";
            if(!masters&&!string.IsNullOrEmpty(PotReplayService.Instance.Status))label.text+="\n"+PotReplayService.Instance.Status;
            int row=0;foreach(var item in reply.list??new PotReplayService.Summary[0]){
                // Five per page fit the mobile screen; the API page is sliced by the UI cursor below.
                if(row>=5)break;
                var selected=item;float y=-400-row*110;
                string result=item.status=="win"?"通关":item.status=="fail"?"失败":item.status=="incomplete"?"中断":"退出";
                PotReplayUI.Label(root,$"{item.date} · {(item.tier=="elite"?"清北":"本科")} · 第{item.stage}关\n{result} · {item.duration:0}秒",new Vector2(-100,y),new Vector2(410,95));
                PotReplayUI.Button(root,"观看",new Vector2(220,y+5),()=>DownloadPotReplay(selected.id,version,label));row++;
            }
            if(skip>0)PotReplayUI.Button(root,"上一页",new Vector2(-160,-1030),()=>DrawPotReplayList(masters,Mathf.Max(0,skip-5)));
            if(reply.hasMore||(reply.list?.Length??0)>5)PotReplayUI.Button(root,"下一页",new Vector2(160,-1030),()=>DrawPotReplayList(masters,skip+5));
        },potReplayTier),error=>{
            if(version!=potReplayListVersion||!label||ScreenName!="pot-replays")return;
            CancelPotReplayDownloadTip();label.text=error.Message;Notice("回放列表加载失败："+error.Message);
        }));
    }
    void ShowPotReplayLoadingTip(string message){
        if(!potReplayDownloadTip)potReplayDownloadTip=Match3DTips.Create(canvas.transform);
        potReplayDownloadTip.Show(message,persistent:true);
    }
    public void CompletePotReplayLoading(){CancelPotReplayDownloadTip();}
    void CancelPotReplayDownloadTip(){
        ++potReplayDownloadVersion;potReplayDownloading=false;
        if(potReplayDownloadTip)potReplayDownloadTip.Hide();
    }
    void DownloadPotReplay(string id,int listVersion,Text label){
        OpenPotReplay(id,()=>listVersion==potReplayListVersion&&ScreenName=="pot-replays"&&label,error=>{if(label)label.text=error;});
    }
    // Both the home tier buttons and the replay list use this loading lifetime.
    public void OpenPotReplay(string id,System.Func<bool> current,System.Action<string> failed,System.Action started=null,System.Func<PotReplayData,bool> validate=null){
        if(potReplayDownloading)return;
        if(!PotReplayService.Instance){failed?.Invoke("回放服务未连接");Notice("回放服务未连接");return;}
        int request=++potReplayDownloadVersion;potReplayDownloading=true;
        ShowPotReplayLoadingTip("下载回放中");
        void Failed(string error){
            if(request!=potReplayDownloadVersion)return;
            bool visible=current();CancelPotReplayDownloadTip();if(!visible)return;
            string message=string.IsNullOrEmpty(error)?"回放下载失败，请重试":error;
            failed?.Invoke(message);Notice(message);
        }
        StartCoroutine(Match3DStartupRunner.Run(PotReplayService.Instance.Detail(id,reply=>{
            if(request!=potReplayDownloadVersion)return;
            if(!current()){CancelPotReplayDownloadTip();return;}
            if(reply==null||!reply.ok){Failed(reply?.error);return;}
            if(validate!=null&&!validate(reply.detail)){Failed("关卡或录像已变化，请重新打开列表");return;}
            try{StartupService<PotReplayPlayer>().Load(reply.detail);started?.Invoke();}
            catch(System.Exception error){CancelPotReplayDownloadTip();failed?.Invoke(error.Message);Notice("回放加载失败："+error.Message);}
        }),error=>Failed(error.Message)));
    }
    public void BeginPotReplay(Match3DDailyLevel level,int stage,Match3DDailyStage opening){
        ShowPotReplayLoadingTip("下载回放中");
        StopBoard();LocalPreview=true;IsPvp=false;unlimitedMagnetRun=false;dailyLevel=JsonUtility.FromJson<Match3DDailyLevel>(JsonUtility.ToJson(level));runModelTheme=null;ThemeIndex=0;Stage=stage;runDate=level.date;
        dailyLevel.opening=new Match3DDailySnapshot{stages=new Match3DDailyStage[2]};dailyLevel.opening.stages[stage-1]=opening;
        ComboProbeController.BeginStage(true,level.seed);
        SpawnDailyOpening(SourceProfile(stage==1).prefabs,stage==1,null,null,animate:false);HUD(replay:true);BlocksGameplay=true;Music(library.gameMusic);
    }
    public void LockPotReplay(){ScreenName="pot-replay";BlocksGameplay=true;GameManager.Instance.gameStatus=GameManager.GameStatus.PREPARING;
        if(gameplayHUD){gameplayHUD.SetReplayMode();gameplayHUD.GetComponent<Match3DUIView>().Bind("Pause",ShowPotReplaySettings);}
    }
    public RectTransform PotReplayHeader=>gameplayHUD.header;
    public void UpdatePotReplayTime(float seconds){
        int t=Mathf.CeilToInt(Mathf.Max(0,seconds));
        if(timer)timer.text=seconds<=0&&Stage==1?"∞":$"{t/60:00}:{t%60:00}";
    }
    public void ShowPotReplaySettings(){
        var player=PotReplayPlayer.Instance;if(!PotReplayPlayer.Active||player.SettingsOpen)return;
        player.SetSettingsOpen(true);
        // Reuse settings construction without the live game's resume/exit callbacks.
        ShowSettings(false);var view=modal.GetComponent<Match3DUIView>();view.Text("Title","暂停 / 设置");
        System.Action close=()=>{CloseModal();BlocksGameplay=true;player.SetSettingsOpen(false);};
        view.Bind("Close",close);view.Bind("Continue",close);view.Bind("Home",player.Exit);
        var skin=view.GetComponent<Match3DSettingsSkin>();
        if(skin)skin.BindSupport(true,player.Exit);
        else{view.Show("Home",true);view.Text("ContinueText","继续回放");}
    }
    public RectTransform PotReplayToolTarget(int type)=>gameplayHUD?gameplayHUD.ReplayToolTarget(type):null;
    public void EndPotReplay(){
        if(potReplayOverlay)Destroy(potReplayOverlay.gameObject);LocalPreview=false;bool glory=potReplayReturnToGlory;potReplayReturnToGlory=false;bool masters=potReplayReturnToMasters;potReplayReturnToMasters=false;Home();if(masters)ShowPotMasterReplays();else if(glory)ShowPotGlory();
    }
}
