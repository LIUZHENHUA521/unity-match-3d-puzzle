using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public partial class Match3DFlow {
    public Match3DDailyLevel PotDailyLevel=>dailyLevel;
    RectTransform potReplayOverlay;int potReplayListVersion;string potReplayTier="bachelor";
    public RectTransform PotReplayOverlay(bool full){
        if(potReplayOverlay)Destroy(potReplayOverlay.gameObject);
        potReplayOverlay=PotReplayUI.Rect(page,"Pot replay overlay",Vector2.zero,new Vector2(720,full?1280:190));
        potReplayOverlay.gameObject.AddComponent<Image>().color=new Color(.96f,.97f,.92f,.97f);return potReplayOverlay;
    }
    public void ShowPotReplays(){
        if(PotReplayPlayer.Active)return;
        StopBoard();Clear();ScreenName="pot-replays";BlocksGameplay=true;
        DrawPotReplayList(false,0);
    }
    void DrawPotReplayList(bool masters,int skip){
        int version=++potReplayListVersion;var root=PotReplayOverlay(true);
        PotReplayUI.Label(root,"每日挑战回放",new Vector2(0,-60),new Vector2(650,70));
        PotReplayUI.Button(root,"我的回放",new Vector2(-190,-150),()=>DrawPotReplayList(false,0));
        PotReplayUI.Button(root,"高手回放",new Vector2(0,-150),()=>DrawPotReplayList(true,0));
        PotReplayUI.Button(root,"返回",new Vector2(190,-150),Home);
        if(masters){PotReplayUI.Button(root,"本科局",new Vector2(-100,-230),()=>{potReplayTier="bachelor";DrawPotReplayList(true,0);});PotReplayUI.Button(root,"清北局",new Vector2(100,-230),()=>{potReplayTier="elite";DrawPotReplayList(true,0);});}
        var label=PotReplayUI.Label(root,"正在加载…",new Vector2(0,masters?-290:-230),new Vector2(650,100));
        if(!PotReplayService.Instance){label.text="回放服务未连接";return;}
        StartCoroutine(PotReplayService.Instance.List(masters,skip,dailyLevel,reply=>{
            if(version!=potReplayListVersion||!label)return;
            if(!reply.ok){label.text=reply.error;return;}
            label.text=reply.list==null||reply.list.Length==0?"暂无回放":masters?"同关卡已公开的完整通关录像":"最近的每日挑战录像";
            if(!masters&&!string.IsNullOrEmpty(PotReplayService.Instance.Status))label.text+="\n"+PotReplayService.Instance.Status;
            int row=0;foreach(var item in reply.list??new PotReplayService.Summary[0]){
                // Five per page fit the mobile screen; the API page is sliced by the UI cursor below.
                if(row>=5)break;
                var selected=item;float y=-400-row*110;
                string result=item.status=="win"?"通关":item.status=="fail"?"失败":item.status=="incomplete"?"中断":"退出";
                PotReplayUI.Label(root,$"{item.date} · {(item.tier=="elite"?"清北":"本科")} · 第{item.stage}关\n{result} · {item.duration:0}秒",new Vector2(-100,y),new Vector2(410,95));
                PotReplayUI.Button(root,"观看",new Vector2(220,y+5),()=>{label.text="正在下载录像…";StartCoroutine(PotReplayService.Instance.Detail(selected.id,r=>{if(version!=potReplayListVersion||!label)return;if(!r.ok){label.text=r.error;return;}try{StartupService<PotReplayPlayer>().Load(r.detail);}catch(System.Exception e){label.text=e.Message;}}));});row++;
            }
            if(skip>0)PotReplayUI.Button(root,"上一页",new Vector2(-160,-1030),()=>DrawPotReplayList(masters,Mathf.Max(0,skip-5)));
            if(reply.hasMore||(reply.list?.Length??0)>5)PotReplayUI.Button(root,"下一页",new Vector2(160,-1030),()=>DrawPotReplayList(masters,skip+5));
        },potReplayTier));
    }
    public void BeginPotReplay(Match3DDailyLevel level,int stage,Match3DDailyStage opening){
        StopBoard();LocalPreview=true;IsPvp=false;unlimitedMagnetRun=false;dailyLevel=JsonUtility.FromJson<Match3DDailyLevel>(JsonUtility.ToJson(level));runModelTheme=null;ThemeIndex=0;Stage=stage;runDate=level.date;
        dailyLevel.opening=new Match3DDailySnapshot{stages=new Match3DDailyStage[2]};dailyLevel.opening.stages[stage-1]=opening;
        ComboProbeController.BeginStage(true,level.seed);
        SpawnDailyOpening(SourceProfile(stage==1).prefabs,stage==1,null,null,animate:false);HUD();BlocksGameplay=true;Music(library.gameMusic);
    }
    public void LockPotReplay(){ScreenName="pot-replay";BlocksGameplay=true;GameManager.Instance.gameStatus=GameManager.GameStatus.PREPARING;
        if(gameplayHUD)gameplayHUD.gameObject.SetActive(false);
    }
    public void EndPotReplay(){
        if(potReplayOverlay)Destroy(potReplayOverlay.gameObject);LocalPreview=false;Home();
    }
}
