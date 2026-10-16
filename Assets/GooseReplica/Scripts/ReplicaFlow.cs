using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(-500)]
public class ReplicaFlow : MonoBehaviour {
    public static ReplicaFlow Instance {get;private set;}
    public ReplicaTheme library;
    public bool IsPvp {get;private set;}
    public bool ServerApplying {get;private set;}
    public bool BlocksGameplay {get;private set;}
    public int InitialCount {get;private set;}
    public bool CreationInProgress {get;private set;}
    public float PendingCreationWait {get;private set;}
    IEnumerator sourceCreationSteps;Coroutine sourceCreationRunner;int creationVersion;
    public int Stage {get;private set;}
    public int ThemeIndex {get;private set;}
    public string ScreenName {get;private set;}
    public string LastResult {get;private set;}
    public int TodaySeed {get;private set;}
    public float AppliedModelSize {get;private set;}=1f;
    Canvas canvas;RectTransform page,modal;Font font;Text timer,remaining,peerText,toast;
    ReplicaPilePhysics pilePhysics;
    ReplicaLayeredPile layeredPile;
    ReplicaThemePresentation sourceTutorialPresentation;
    GameObject sourceTutorialBoundary;
    ReplicaDiagnostics diagnostics;
    Button moveToolButton,findToolButton,shuffleToolButton;
    ReplicaNetwork network;AudioSource music,effects;int pvpUndo=3;bool resultShown,pendingPick;
    int displayedTime=-1,displayedCount=-1,displayedPeer=-1;
    string runDate,roomId="",lobbyStatus="",endpoint;int otherCleared;
    public bool SettingsMusic {get {return PlayerPrefs.GetInt("goose.music",1)==1;} }
    public bool SettingsEffects {get {return PlayerPrefs.GetInt("goose.effects",1)==1;} }
    public bool SettingsVibration {get {return PlayerPrefs.GetInt("goose.vibration",1)==1;} }
    ThemeEntry CurrentTheme {get {return library.themes[ThemeIndex];} }
    public static string Today {get {return DateTime.UtcNow.AddHours(8).ToString("yyyy-MM-dd");} }
    void Awake(){Instance=this;}
    System.Collections.IEnumerator Start(){
#if UNITY_WEBGL && !UNITY_EDITOR
        bool sdkReady=false;
        WeChatWASM.WX.InitSDK(code=>{sdkReady=true;Debug.Log("WX SDK init: "+code);});
        while(!sdkReady)yield return null;
#endif
#if UNITY_WEBGL && !UNITY_EDITOR
        Application.targetFrameRate=60;
        QualitySettings.antiAliasing=0;
        if(Camera.main){Camera.main.allowHDR=false;Camera.main.allowMSAA=false;}
#endif
        // One bounded shadow map covers the pot, including the camera's 12-unit height.
        QualitySettings.shadows=ShadowQuality.All;
        QualitySettings.shadowResolution=ShadowResolution.Medium;
        QualitySettings.shadowDistance=20f;
        QualitySettings.shadowCascades=0;
        foreach(var c in FindObjectsOfType<Canvas>())c.gameObject.SetActive(false);
        font=library.font?library.font:Resources.Load<Font>("Replica/Fonts/GooseSans");
        if(!font)font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        canvas=new GameObject("Goose UI",typeof(RectTransform),typeof(Canvas),typeof(CanvasScaler),typeof(GraphicRaycaster)).GetComponent<Canvas>();
        canvas.renderMode=RenderMode.ScreenSpaceOverlay;canvas.sortingOrder=100;
        var scaler=canvas.GetComponent<CanvasScaler>();scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;scaler.referenceResolution=new Vector2(720,1280);scaler.matchWidthOrHeight=0;
        page=Rect("Page",canvas.transform,Vector2.zero,new Vector2(720,1280));Stretch(page);
        music=gameObject.AddComponent<AudioSource>();music.loop=true;music.volume=.25f;
        effects=gameObject.AddComponent<AudioSource>();effects.volume=.65f;
        pilePhysics=gameObject.AddComponent<ReplicaPilePhysics>();
        layeredPile=gameObject.AddComponent<ReplicaLayeredPile>();
        gameObject.AddComponent<ReplicaSourceObjectPool>();
        gameObject.AddComponent<ReplicaTray>();
        diagnostics=gameObject.AddComponent<ReplicaDiagnostics>();
        network=gameObject.AddComponent<ReplicaNetwork>();network.Received=Receive;
        endpoint=PlayerPrefs.GetString("goose.endpoint",library.pvpEndpoint);Home();
#if UNITY_WEBGL && !UNITY_EDITOR
        WeChatWASM.WX.ReportGameStart();
#endif
        yield break;
    }
    void OnDestroy(){CancelSourceCreation();if(sourceTutorialPresentation)sourceTutorialPresentation.End();if(sourceTutorialBoundary)Destroy(sourceTutorialBoundary);ReplicaSourcePhysics.End();if(Instance==this)Instance=null;Time.timeScale=1;}
    void Update(){
        if(ScreenName=="game") {
            if(!IsPvp&&Stage==2&&InputController.Instance){
                var input=InputController.Instance;
                bool ready=GameManager.Instance.GameIsPlaying()&&!BlocksGameplay&&!ReplicaSourceTools.Shuffling;
                if(moveToolButton)moveToolButton.interactable=ready&&ReplicaSourceTools.CanUse(0)&&input.objectsOnStack.Count>0;
                if(findToolButton)findToolButton.interactable=ready&&ReplicaSourceTools.CanUse(1)&&ReplicaSourceTools.CanAutoFind(input.objectsOnStack);
                if(shuffleToolButton)shuffleToolButton.interactable=ready&&ReplicaSourceTools.CanUse(2);
            }
            int t=Mathf.CeilToInt(GameManager.Instance.currentLevelTime);
            int count=LevelGenerator.Instance.objectsOnScene.Count;
            if(timer&&t!=displayedTime){timer.text=GameManager.Instance.currentLevelTime>0?(t/60).ToString("00")+":"+(t%60).ToString("00"):"";displayedTime=t;}
            if(remaining&&count!=displayedCount)remaining.text="剩余 "+count+" / "+InitialCount;
            if(peerText&&(count!=displayedCount||otherCleared!=displayedPeer))peerText.text="我方 "+(InitialCount-count)+" / "+InitialCount+"    对手 "+otherCleared+" / "+InitialCount;
            displayedCount=count;displayedPeer=otherCleared;
        }
    }

    void Clear(){
        displayedTime=displayedCount=displayedPeer=-1;
        foreach(Transform t in page)Destroy(t.gameObject);modal=null;timer=null;remaining=null;peerText=null;toast=null;BlocksGameplay=false;
    }
    void Music(AudioClip clip){if(music.clip!=clip){music.clip=clip;if(clip)music.Play();}music.mute=!SettingsMusic;}
    void Click(){if(SettingsEffects&&library.clickSound)effects.PlayOneShot(library.clickSound);}
    public void SelectSound(){if(SettingsEffects&&library.selectSound)effects.PlayOneShot(library.selectSound);}
    public void MatchSound(){if(SettingsEffects&&library.matchSound)effects.PlayOneShot(library.matchSound);}
    void StopBoard(){if(Camera.main){var highlight=Camera.main.GetComponent<ReplicaSourceHighlight>();if(highlight)highlight.ClearAll();}ReplicaSourceAudio.ResetLevel();CancelSourceCreation();if(sourceTutorialPresentation)sourceTutorialPresentation.End();if(sourceTutorialBoundary){sourceTutorialBoundary.SetActive(false);Destroy(sourceTutorialBoundary);sourceTutorialBoundary=null;}ReplicaSourcePhysics.End();if(InputController.Instance)InputController.Instance.CancelSelection();if(layeredPile)layeredPile.Clear();if(pilePhysics)pilePhysics.Clear();if(diagnostics)diagnostics.ResetProbe();Time.timeScale=1;StopAllCoroutines();GameManager.Instance.StopAllCoroutines();GameManager.Instance.gameStatus=GameManager.GameStatus.PREPARING;LevelGenerator.Instance.ClearBoard();resultShown=false;pendingPick=false;}
    public void Home(){
        if(network)network.Disconnect();StopBoard();IsPvp=false;Clear();ScreenName="home";Music(library.homeMusic);
        ReplicaHomeView.Create(this,page,ReplicaHomeView.Tab.Home);
    }
    public void ShowHomeTab(ReplicaHomeView.Tab tab){
        if(tab==ReplicaHomeView.Tab.Home){Home();return;}
        if(network)network.Disconnect();StopBoard();IsPvp=false;Clear();
        ScreenName=tab==ReplicaHomeView.Tab.Collection?"collection-tab":"pets-tab";
        Music(library.homeMusic);ReplicaHomeView.Create(this,page,tab);
    }
    public void HomeClick(){Click();}
    public void ShowRewardInfo(){
        BlocksGameplay=true;
        var panel=Modal("红包奖励");
        Label(panel,"本科局：毕业可领 "+ReplicaHomeView.BachelorReward+" 元奖励\n\n清北局：毕业可领 "+ReplicaHomeView.EliteReward+" 元奖励",new Vector2(360,475),new Vector2(550,180),31);
        Label(panel,"奖励规则即将开放",new Vector2(360,670),new Vector2(500,70),27);
        Button(panel,"知道了",new Vector2(360,845),new Vector2(420,95),()=>{Destroy(panel.gameObject);modal=null;BlocksGameplay=false;},"PopBtn2",32);
    }
    void IconButton(string sprite,string text,Vector2 pos,Action action){
        Button(page,"",pos,new Vector2(118,118),action,sprite);Label(page,text,pos+new Vector2(0,91),new Vector2(220,44),26);
    }
    public void ShowThemes(){
        Clear();ScreenName="themes";Background(new Color(.96f,.91f,.75f));Title("主题玩法",Home);
        Label(page,"每个主题都有一只等待你的大鹅",new Vector2(360,180),new Vector2(650,55),26);
        for(int i=0;i<library.themes.Length;i++){
            int index=i;var theme=library.themes[i];float y=350+i*240;
            Art(page,"PopBG2",new Vector2(360,y),new Vector2(620,205),false);
            Art(page,theme.gooseSprite,new Vector2(132,y),new Vector2(140,160));
            Label(page,theme.title,new Vector2(420,y-55),new Vector2(350,54),36);
            Label(page,theme.subtitle,new Vector2(430,y),new Vector2(390,46),23);
            Button(page,"挑战",new Vector2(440,y+62),new Vector2(240,66),()=>Begin(index),"MainBtnBg",28);
        }
    }
    public bool HasGoose(string id){return PlayerPrefs.GetInt("goose.collection."+id,0)>0;}
    public void ShowCollection(){
        Clear();ScreenName="collection";Background(new Color(.96f,.91f,.75f));Title("我的大鹅",Home);
        Label(page,"已收集 "+library.themes.Count(t=>HasGoose(t.id))+" / "+library.themes.Length,new Vector2(360,190),new Vector2(600,60),30);
        for(int i=0;i<library.themes.Length;i++){
            int index=i;var t=library.themes[i];float y=380+i*240;bool got=HasGoose(t.id);
            var art=Art(page,t.gooseSprite,new Vector2(180,y),new Vector2(185,185));art.color=got?Color.white:new Color(.28f,.28f,.28f,.8f);
            Label(page,t.title,new Vector2(450,y-45),new Vector2(370,55),32);
            Label(page,got?"已获得 · 永久收藏":"通关主题第二关解锁",new Vector2(450,y+14),new Vector2(400,50),24);
            Button(page,got?"再挑战":"去解锁",new Vector2(450,y+75),new Vector2(250,65),()=>Begin(index),"PopBtn1",25);
        }
    }
    public void Begin(int theme){
        if(network)network.Disconnect();StopBoard();IsPvp=false;ThemeIndex=theme;Stage=1;runDate=Today;StartStage();
    }
    public void RestartStage(){StopBoard();StartStage();}
    public void NextStage(){
        // The source keeps the theme presentation alive while MainLevel rolls
        // into the layered challenge. Rebuilding the same scene and collision
        // shell here caused a synchronous hitch before the creation coroutine
        // could show its first batch.
        var reusedPresentation=sourceTutorialPresentation;
        var reusedBoundary=sourceTutorialBoundary;
        sourceTutorialPresentation=null;
        sourceTutorialBoundary=null;
        Stage=2;StopBoard();StartStage(reusedPresentation,reusedBoundary);
    }
    void StartStage(ReplicaThemePresentation reusedPresentation=null,GameObject reusedBoundary=null){
        var date=DateTime.ParseExact(runDate,"yyyy-MM-dd",System.Globalization.CultureInfo.InvariantCulture).AddHours(4);
        TodaySeed=ChallengePlan.DailySeed(CurrentTheme.id,Stage,date);
        bool sourceTutorial=!IsPvp&&Stage==1;
        var source=sourceTutorial||Stage==2? (sourceTutorial?ReplicaSourceLevelPlan.GetTutorial(CurrentTheme.sourceEnvironment):ReplicaSourceLevelPlan.Get(CurrentTheme.sourceEnvironment)) : null;
        var prefabs=source!=null?source.prefabs:CurrentTheme.prefabs;
        IReplicaSourceRandom sourceRandom=source!=null?ReplicaSourceGlobalRandom.Instance:null;
        var tiles=source!=null?ChallengePlan.GenerateSource(sourceRandom,prefabs.Length,source.layers):ChallengePlan.Generate(TodaySeed,prefabs.Length,new[]{new ChallengeLayer(12,2,.65f)});
        Spawn(tiles,TodaySeed,source!=null?source.seconds:120,prefabs,sourceRandom,sourceTutorial,reusedPresentation,reusedBoundary);HUD();Music(library.gameMusic);

    }
    public const float BoardItemScale = 1.5f;
    sealed class SourceCreateInfo {public ChallengeTile tile;public Vector3 position;}
    public const int SourceFrameCreateCount=3;
    public const float SourceCreateSeconds=2f,SourceMaximumCreateInterval=.03f;
    public static float SourceCreateInterval(int count)=>Mathf.Min(SourceCreateSeconds/Mathf.Max(1,count)*SourceFrameCreateCount,SourceMaximumCreateInterval);
    void Spawn(List<ChallengeTile> tiles,int seed,int seconds,GameObject[] sourcePrefabs=null,IReplicaSourceRandom sourceRandom=null,bool sourceTutorial=false,ReplicaThemePresentation reusedPresentation=null,GameObject reusedBoundary=null){
        var prefabs=sourcePrefabs??CurrentTheme.prefabs;
        LastResult="";InitialCount=tiles.Count;
        bool layered=!IsPvp&&Stage==2;
        ReplicaSourceColliders.ConfigureLayers();
        if(!IsPvp)ReplicaSourcePhysics.Begin();
        GameManager.Instance.currentLevelTime=seconds;GameManager.Instance.gameStatus=GameManager.GameStatus.PREPARING;
        if(layered){
            AppliedModelSize=ReplicaSourceProfile.ModelSizeMultiplier;
            ReplicaSourceEnvironment.Select(CurrentTheme.sourceEnvironment);layeredPile.Begin(true,reusedPresentation,reusedBoundary);
            var random=sourceRandom??ReplicaSourceGlobalRandom.Instance;
            // LoopCreateObj builds all descriptions bottom-to-top, then List.Sort compares Y.
            // Rotation is sampled later, at CreateUnitObj, in the sorted creation order.
            var descriptions=new List<SourceCreateInfo>(tiles.Count);float previousHeight=1.5f;
            foreach(var group in tiles.GroupBy(t=>t.layer).OrderBy(g=>g.Key)){
                float height=group.First().height;
                foreach(var tile in group)descriptions.Add(new SourceCreateInfo{tile=tile,position=ReplicaSourceEnvironment.Position(previousHeight,height,random)});
                previousHeight=height; // saved _layerMinDistance = 0
            }
            descriptions.Sort((a,b)=>a.position.y.CompareTo(b.position.y));
            CreationInProgress=true;int version=++creationVersion;
            sourceCreationSteps=SourceCreationSteps(descriptions,prefabs,random,version);
            sourceCreationRunner=StartCoroutine(RunSourceCreation(version));
            return;
        }
        if(sourceTutorial){
            // The first MainLevel uses the same source scene, boundary and
            // Rigidbody setup as the ordinary source board, but has no hidden
            // layers or later visibility scan. Item positions still come from
            // the source GetPos interval [0,6].
            ReplicaSourceEnvironment.Select(CurrentTheme.sourceEnvironment);
            if(!sourceTutorialPresentation)sourceTutorialPresentation=gameObject.AddComponent<ReplicaThemePresentation>();
            sourceTutorialPresentation.Begin(CurrentTheme.sourceEnvironment);
            sourceTutorialBoundary=ReplicaSourceEnvironment.CreateBoundary();
            sourceTutorialBoundary.name=CurrentTheme.sourceEnvironment+" source tutorial collision reference";
            var random=sourceRandom??ReplicaSourceGlobalRandom.Instance;
            foreach(var tile in tiles)
                CreateBoardItem(tile,prefabs[tile.type],ReplicaSourceEnvironment.Position(0f,6f,random),ReplicaSourceEnvironment.Rotation(random),false,true);
            StartCoroutine(Ready(false));
            return;
        }
        var legacyRandom=new System.Random(seed);int lastLayer=-1,local=0;
        foreach(var tile in tiles){
            if(tile.layer!=lastLayer){local=0;lastLayer=tile.layer;}
            float x=(local%3-1)*.48f,z=((local/3)%3-1)*.48f+.15f,y=tile.height+(local/9)*.32f;local++;
            CreateBoardItem(tile,prefabs[tile.type],new Vector3(x,y,z),Quaternion.Euler(legacyRandom.Next(-10,11),legacyRandom.Next(360),legacyRandom.Next(-10,11)),false);
        }
        StartCoroutine(Ready(IsPvp));
    }
    void CreateBoardItem(ChallengeTile tile,GameObject prefab,Vector3 position,Quaternion rotation,bool layered,bool sourceBoard=false){
        var obj=layered?ReplicaSourceObjectPool.Get(prefab,prefab.name):Instantiate(prefab);
        obj.transform.SetPositionAndRotation(position,rotation);
        bool sourceModel=layered||sourceBoard;
        obj.transform.localScale*=sourceModel?ReplicaSourceProfile.ModelScale(prefab.name):BoardItemScale;
        ReplicaSourceColliders.Apply(obj,prefab.name,sourceModel);
        ReplicaSourceFlight.Bind(obj,prefab.name,layered);
        if(layered)ReplicaSourceAudio.Bind(obj,prefab.name);
        obj.name="type_"+tile.type;var tileComponent=obj.GetComponent<ReplicaTile>()??obj.AddComponent<ReplicaTile>();tileComponent.id=tile.id;
        var body=obj.GetComponent<Rigidbody>();body.useGravity=true;ReplicaSourcePhysics.Apply(body);
        // ObjectUnit.Init calls Rigidbody.set_isKinematic(true), j6539. The creation
        // loop activates eligible bodies only after the initial growth reaches full size.
        pilePhysics.Register(body,layered);
        LevelGenerator.Instance.objectsOnScene.Add(obj);
        if(layered){var goose=obj.GetComponent<ReplicaSourceGooseAnimation>();goose?.InitializeSource();goose?.ReturnToPile();layeredPile.Register(body);}
    }
    IEnumerator SourceCreationSteps(List<SourceCreateInfo> descriptions,GameObject[] prefabs,IReplicaSourceRandom random,int version){
        float wait=SourceCreateInterval(descriptions.Count);
        for(int i=0;i<descriptions.Count;i++){
            if(version!=creationVersion)yield break;
            var info=descriptions[i];CreateBoardItem(info.tile,prefabs[info.tile.type],info.position,ReplicaSourceEnvironment.Rotation(random),true);
            // Source tests the zero-based index: first yield after item 0, then 3,6,... .
            if(i%SourceFrameCreateCount==0)yield return wait;
        }
        yield return .5f;
        if(version!=creationVersion)yield break;
        layeredPile.StopCreationGrowth(); // StopCoroutine("LoopCrowObjSize")
        yield return .2f;
        if(version!=creationVersion)yield break;
        // The special real-physics branch restores gravity here; its saved switch is false.
        yield return .3f;
        if(version!=creationVersion)yield break;
        layeredPile.FinishCreation(); // FinishCreate(true), height check, then optimization starts.
        CreationInProgress=false;PendingCreationWait=0;
        GameManager.Instance.gameStatus=GameManager.GameStatus.PLAY;
        BoosterManager.Instance.BoostersInteractibility(true);
    }
    IEnumerator RunSourceCreation(int version){
        var steps=sourceCreationSteps;
        while(version==creationVersion&&steps.MoveNext()){
            PendingCreationWait=(float)steps.Current;
            yield return new WaitForSeconds(PendingCreationWait);
        }
        if(version==creationVersion){sourceCreationSteps=null;sourceCreationRunner=null;PendingCreationWait=0;}
    }
    void CancelSourceCreation(){
        creationVersion++;CreationInProgress=false;PendingCreationWait=0;
        if(sourceCreationRunner!=null)StopCoroutine(sourceCreationRunner);
        sourceCreationRunner=null;sourceCreationSteps=null;
    }
#if UNITY_EDITOR
    // Test driver takes over the exact suspended production iterator, including its Current wait.
    // No alternative synchronous spawn implementation is used by fixture setup.
    public IEnumerator TakeOverSourceCreationForValidation(){
        if(sourceCreationRunner!=null)StopCoroutine(sourceCreationRunner);
        sourceCreationRunner=null;return sourceCreationSteps;
    }
#endif
    IEnumerator Ready(bool pvp){
        // Parked items are pickable: do not lock input until the entire queue settles.
        yield return new WaitForSeconds(3);
        if(pvp){network.Send(new ReplicaMessage{type="loaded"});Notice("已准备好，等待对手…");}
        else {GameManager.Instance.gameStatus=GameManager.GameStatus.PLAY;BoosterManager.Instance.BoostersInteractibility(true);}
    }
    void HUD(){
        Clear();ScreenName="game";resultShown=false;BlocksGameplay=false;
        Label(page,IsPvp?"好友对战":CurrentTheme.title+" · 第"+Stage+"关",new Vector2(320,70),new Vector2(550,65),34,Color.white,true);
        Button(page,"",new Vector2(649,70),new Vector2(72,72),()=>ShowSettings(true),"PauseBtn");
        if(!IsPvp&&Stage==2)Button(page,"大小 "+AppliedModelSize.ToString("F2")+"×",new Vector2(593,150),new Vector2(175,58),ShowModelSize,"PopBtn1",23);
        timer=Label(page,"",new Vector2(360,150),new Vector2(250,65),45,Color.white);
        remaining=Label(page,"",new Vector2(360,220),new Vector2(520,50),27,Color.white);
        if(IsPvp)peerText=Label(page,"",new Vector2(360,280),new Vector2(680,46),24,Color.white);
        else diagnostics.Show(this,page);
        float controlsY=page.rect.height-155;
        moveToolButton=Button(page,!IsPvp&&Stage==2?"移出":"撤回",new Vector2(135,controlsY),new Vector2(180,90),Undo,"PopBtn1",29);
        shuffleToolButton=Button(page,!IsPvp&&Stage==2?"打乱":"颠锅",new Vector2(360,controlsY),new Vector2(180,90),()=>{if(ReplicaSourceTools.Enabled)ReplicaSourceTools.MessUp();else FindObjectOfType<FoodPileShake>().Shake();},"MainBtnBg",29);
        findToolButton=Button(page,IsPvp?"认输":Stage==2?"凑齐":"提示",new Vector2(585,controlsY),new Vector2(180,90),()=>{if(IsPvp)network.Send(new ReplicaMessage{type="surrender"});else BoosterManager.Instance.HintButton();},"PopBtn2",29);
        if(!IsPvp&&Stage!=2)Button(page,"冻结时间",new Vector2(360,controlsY+90),new Vector2(240,48),()=>BoosterManager.Instance.Freeze(),"PopBtn1",23);
    }
    public bool HandleResult(bool win){
        if(ScreenName!="game"||resultShown)return true;
        if(IsPvp){GameManager.Instance.gameStatus=GameManager.GameStatus.PREPARING;return true;}
        resultShown=true;GameManager.Instance.gameStatus=win?GameManager.GameStatus.FINISH:GameManager.GameStatus.FAIL;
        if(win&&Stage==2){PlayerPrefs.SetInt("goose.collection."+CurrentTheme.id,1);PlayerPrefs.SetString("goose.completed."+CurrentTheme.id,runDate);PlayerPrefs.Save();}
        Result(win,win?(Stage==1?"第一关通过！":"抓到大鹅啦！"):"差一点就成功了",win?(Stage==1?"热身结束，真正的挑战来了":"已点亮 "+CurrentTheme.title+" 图鉴"):"同一关再试一次，留意槽位和遮挡");return true;
    }
    void Result(bool win,string title,string detail){
        LastResult=title;BlocksGameplay=true;Time.timeScale=1;
        var panel=Modal(title);Art(panel,CurrentTheme.gooseSprite,new Vector2(360,445),new Vector2(205,230));
        Label(panel,detail,new Vector2(360,650),new Vector2(580,90),27);
        Button(panel,IsPvp?"再次匹配":win&&Stage==1?"挑战第二关":win?"查看图鉴":"再试一次",new Vector2(360,790),new Vector2(420,100),()=>{if(IsPvp){StopBoard();ShowPvp();}else if(win&&Stage==1)NextStage();else if(win){StopBoard();ShowCollection();}else RestartStage();},"MainBtnBg",32);
        Button(panel,"返回首页",new Vector2(360,920),new Vector2(350,86),Home,"PopBtn1",29);
        var clip=win?library.winSound:library.loseSound;if(SettingsEffects&&clip)effects.PlayOneShot(clip);
#if UNITY_WEBGL && !UNITY_EDITOR
        if(!win&&SettingsVibration)WeChatWASM.WX.VibrateShort(new WeChatWASM.VibrateShortOption{type="light"});
#elif UNITY_IOS || UNITY_ANDROID
        if(!win&&SettingsVibration&&!Application.isEditor)Handheld.Vibrate();
#endif
    }
    void Undo(){
        if(IsPvp){if(pvpUndo>0&&!pendingPick&&!InputController.Instance.IsBusy){pendingPick=true;network.Send(new ReplicaMessage{type="undo"});}}
        else BoosterManager.Instance.Undo();
    }
    public bool InterceptPick(GameObject obj){
        if(!IsPvp||ServerApplying)return false;
        if(pendingPick||BlocksGameplay||InputController.Instance.IsBusy)return true;
        var tile=obj.GetComponent<ReplicaTile>();if(tile){pendingPick=true;network.Send(new ReplicaMessage{type="pick",tileId=tile.id});}return true;
    }
    public void ShowModelSize(){
        if(IsPvp||Stage!=2||ScreenName!="game")return;
        Time.timeScale=0;BlocksGameplay=true;
        var panel=Modal("调整模型大小");
        Action close=()=>{Destroy(panel.gameObject);modal=null;BlocksGameplay=false;Time.timeScale=1;};
        Button(panel,"×",new Vector2(622,250),new Vector2(66,66),close,"CloseBtn",30);
        Label(panel,"当前这局："+AppliedModelSize.ToString("F2")+"×",new Vector2(360,385),new Vector2(540,65),29);
        var readout=Label(panel,"",new Vector2(360,480),new Vector2(500,75),43);
        var track=Rect("ModelSizeSlider",panel,new Vector2(360,580),new Vector2(440,60));
        var hit=track.gameObject.AddComponent<Image>();hit.color=new Color(0,0,0,.04f);
        var rail=Rect("Rail",track,new Vector2(220,30),new Vector2(440,12));rail.gameObject.AddComponent<Image>().color=new Color(.48f,.35f,.20f);rail.GetComponent<Image>().raycastTarget=false;
        var handleArea=Rect("Handle area",track,new Vector2(220,30),new Vector2(410,60));
        var handle=Rect("Handle",handleArea,Vector2.zero,new Vector2(42,60));
        handle.sizeDelta=new Vector2(42,0);
        var handleImage=handle.gameObject.AddComponent<Image>();handleImage.color=new Color(1f,.66f,.15f);
        var slider=track.gameObject.AddComponent<Slider>();slider.targetGraphic=handleImage;slider.handleRect=handle;
        slider.minValue=50;slider.maxValue=125;slider.wholeNumbers=true;slider.value=Mathf.RoundToInt(AppliedModelSize*100);
        Action refresh=()=>readout.text="新大小："+(slider.value/100f).ToString("F2")+"×";
        slider.onValueChanged.AddListener(_=>refresh());refresh();
        Label(panel,"小 0.50×                         大 1.25×",new Vector2(360,645),new Vector2(520,50),23);
        Button(panel,"− 0.01",new Vector2(200,710),new Vector2(200,65),()=>slider.value--,"PopBtn1",25);
        Button(panel,"＋ 0.01",new Vector2(520,710),new Vector2(200,65),()=>slider.value++,"PopBtn1",25);
        Label(panel,"1.00× = 刚才偏大的版本\n保持物品大小比例，重开后生效",new Vector2(360,795),new Vector2(540,90),25);
        Button(panel,"按此大小重开",new Vector2(360,900),new Vector2(460,90),()=>{
            ReplicaSourceProfile.ModelSizeMultiplier=slider.value/100f;Time.timeScale=1;RestartStage();
        },"PopBtn2",30);
        Button(panel,"恢复 1.00×",new Vector2(360,995),new Vector2(300,58),()=>slider.value=100,"PopBtn1",24);
    }
    public void ShowSettings(bool ingame){
        if(ingame&&!IsPvp)Time.timeScale=0;BlocksGameplay=true;
        var panel=Modal(ingame?"暂停 / 设置":"设置");
        Action close=()=>{Destroy(panel.gameObject);modal=null;BlocksGameplay=false;if(ingame&&!IsPvp)Time.timeScale=1;};
        Button(panel,"×",new Vector2(622,250),new Vector2(66,66),close,"CloseBtn",30);
        SettingToggle(panel,"音乐",400,SettingsMusic,value=>{PlayerPrefs.SetInt("goose.music",value?1:0);music.mute=!value;PlayerPrefs.Save();});
        SettingToggle(panel,"音效",535,SettingsEffects,value=>{PlayerPrefs.SetInt("goose.effects",value?1:0);PlayerPrefs.Save();});
        SettingToggle(panel,"震动",670,SettingsVibration,value=>{PlayerPrefs.SetInt("goose.vibration",value?1:0);PlayerPrefs.Save();});
        Button(panel,ingame?"继续游戏":"完成",new Vector2(360,840),new Vector2(430,95),close,"PopBtn2",32);
        if(ingame)Button(panel,"返回首页",new Vector2(360,965),new Vector2(370,85),Home,"PopBtn1",29);
    }
    void SettingToggle(Transform p,string name,float y,bool value,Action<bool> change){
        Label(p,name,new Vector2(215,y),new Vector2(190,65),34);Button button=null;bool current=value;
        button=Button(p,current?"已开启":"已关闭",new Vector2(465,y),new Vector2(210,78),()=>{current=!current;change(current);button.GetComponentInChildren<Text>().text=current?"已开启":"已关闭";},"PopBtn1",28);
    }
    public void ShowPvp(){
        if(network)network.Disconnect();StopBoard();IsPvp=true;ThemeIndex=0;Stage=2;roomId="";lobbyStatus="邀请朋友来一场三消对决";DrawLobby();Music(library.pvpMusic);
    }
    void DrawLobby(){
        Clear();ScreenName="pvp";Background(new Color(.89f,.95f,.98f));Title("好友对战",Home);
        Art(page,"onlineLogo",new Vector2(360,285),new Vector2(430,170));
        Label(page,lobbyStatus,new Vector2(360,440),new Vector2(650,120),30);
        if(!string.IsNullOrEmpty(roomId))Label(page,"房间号："+roomId,new Vector2(360,560),new Vector2(650,55),38);
        var address=InputBox(page,endpoint,new Vector2(360,670),new Vector2(630,64));address.onEndEdit.AddListener(v=>{endpoint=v.Trim();PlayerPrefs.SetString("goose.endpoint",endpoint);});
        if(!network.Connected){
            Button(page,"快速匹配",new Vector2(360,790),new Vector2(460,95),()=>ConnectPvp("match",""),"MainBtnBg",32);
            Button(page,"创建房间",new Vector2(205,920),new Vector2(290,85),()=>ConnectPvp("create",""),"PopBtn1",28);
            var code=InputBox(page,"",new Vector2(505,920),new Vector2(250,85));code.characterLimit=6;
            Button(page,"加入房间",new Vector2(360,1050),new Vector2(450,85),()=>ConnectPvp("join",code.text),"PopBtn2",29);
        }else {
            Button(page,"准备",new Vector2(360,790),new Vector2(450,100),()=>network.Send(new ReplicaMessage{type="ready"}),"MainBtnBg",34);
            Button(page,"取消 / 离开",new Vector2(360,960),new Vector2(430,90),ShowPvp,"PopBtn1",29);
        }
        Label(page,"双方同配置 · 先清空者胜\n每人可撤回 3 次，断线判负",new Vector2(360,1190),new Vector2(650,80),23);
    }
    void ConnectPvp(string action,string room){lobbyStatus="正在连接…";network.Connect(endpoint,action,room);Notice(lobbyStatus);}
    void Receive(ReplicaMessage m){
        if(m.type=="connected"){DrawLobby();return;}
        if(m.type=="queued"){lobbyStatus="正在等待另一位真人玩家…";DrawLobby();return;}
        if(m.type=="room"){roomId=m.roomId;lobbyStatus=m.players.Length+" / 2 位玩家\n"+m.players.Count(x=>x.ready)+" 人已准备";DrawLobby();return;}
        if(m.type=="prepare"){
            StopBoard();IsPvp=true;pvpUndo=3;otherCleared=0;var tiles=new List<ChallengeTile>();
            for(int i=0;i<m.deck.Length;i++)tiles.Add(new ChallengeTile{id=i,type=m.deck[i],layer=0,height=.65f});
            Spawn(tiles,m.seed,m.seconds);HUD();Notice("双方正在加载物品…");return;
        }
        if(m.type=="start"){GameManager.Instance.gameStatus=GameManager.GameStatus.PLAY;GameManager.Instance.currentLevelTime=300;Notice("对战开始！");return;}
        if(m.type=="move"){
            if(m.playerId!=network.PlayerId){otherCleared=m.cleared;return;}
            pendingPick=false;pvpUndo=m.undoLeft;ServerApplying=true;
            if(m.action=="pick"){
                var obj=LevelGenerator.Instance.objectsOnScene.FirstOrDefault(x=>x&&x.GetComponent<ReplicaTile>().id==m.tileId);
                if(obj)InputController.Instance.AddObjectOnStack(obj);
            }else if(m.action=="undo")InputController.Instance.UndoLast();
            ServerApplying=false;return;
        }
        if(m.type=="result"){
            pendingPick=false;StopAllCoroutines();GameManager.Instance.StopAllCoroutines();BlocksGameplay=true;Time.timeScale=1;
            bool win=m.winner==network.PlayerId;string reason=m.reason=="disconnect"?"对手已断线":m.reason=="overflow"?"一方槽位已满":m.reason=="timeout"?"时间到，按消除数量结算":m.reason=="load_timeout"?"加载超时，本局取消":"本局已结束";
            resultShown=true;StartCoroutine(FinishPvp(win,string.IsNullOrEmpty(m.winner)?"平局 / 本局结束":win?"对战胜利！":"本次惜败",reason));return;
        }
        if(m.type=="left"){if(ScreenName=="pvp"){lobbyStatus="房间已结束";network.Disconnect();DrawLobby();}return;}
        if(m.type=="error"){pendingPick=false;Notice(m.message);return;}
        if(m.type=="disconnected"){
            pendingPick=false;if(ScreenName=="game"&&!resultShown){GameManager.Instance.gameStatus=GameManager.GameStatus.FAIL;resultShown=true;Result(false,"连接已断开","请检查网络后重新匹配");}
            else if(ScreenName=="pvp"){lobbyStatus=m.message;network.Disconnect();DrawLobby();}
        }
    }
    IEnumerator FinishPvp(bool win,string title,string reason){
        yield return new WaitForSecondsRealtime(.9f);
        GameManager.Instance.gameStatus=GameManager.GameStatus.FINISH;Result(win,title,reason);
    }
    void Notice(string value){if(!toast)toast=Label(page,value,new Vector2(360,350),new Vector2(680,90),23,Color.white,true);else toast.text=value;}
    void Title(string text,Action back){Label(page,text,new Vector2(360,90),new Vector2(510,75),43);Button(page,"返回",new Vector2(90,88),new Vector2(135,68),back,"PopBtn1",25);}
    RectTransform Modal(string title){
        if(modal)Destroy(modal.gameObject);modal=Rect("Modal",page,Vector2.zero,new Vector2(720,1280));Stretch(modal);
        var shade=modal.gameObject.AddComponent<Image>();shade.color=new Color(0,0,0,.58f);shade.raycastTarget=true;
        Art(modal,"PopBG2",new Vector2(360,640),new Vector2(650,850),false);Label(modal,title,new Vector2(360,280),new Vector2(550,80),43);return modal;
    }
    void Background(Color color){var r=Rect("Background",page,Vector2.zero,new Vector2(720,1280));Stretch(r);r.gameObject.AddComponent<Image>().color=color;}
    public Image Art(Transform parent,string sprite,Vector2 pos,Vector2 size,bool preserve=true){var r=Rect(sprite,parent,pos,size);var im=r.gameObject.AddComponent<Image>();im.sprite=Resources.Load<Sprite>("GooseArt/Sprites/"+sprite);im.preserveAspect=preserve;im.raycastTarget=false;return im;}
    public Text Label(Transform parent,string text,Vector2 pos,Vector2 size,int fontSize,Color? color=null,bool outline=false){
        var r=Rect(text,parent,pos,size);var t=r.gameObject.AddComponent<Text>();t.font=font;t.fontSize=fontSize;t.text=text;t.alignment=TextAnchor.MiddleCenter;t.color=color??new Color(.30f,.18f,.09f);t.raycastTarget=false;t.horizontalOverflow=HorizontalWrapMode.Wrap;t.verticalOverflow=VerticalWrapMode.Overflow;
        if(outline){var o=r.gameObject.AddComponent<Outline>();o.effectColor=new Color(.2f,.1f,0,.9f);o.effectDistance=new Vector2(2,-2);}return t;
    }
    public Button Button(Transform parent,string text,Vector2 pos,Vector2 size,Action action,string sprite="PopBtn1",int fontSize=28){
        var im=Art(parent,sprite,pos,size,false);im.raycastTarget=true;var b=im.gameObject.AddComponent<Button>();b.targetGraphic=im;b.onClick.AddListener(()=>{Click();action();});
        if(text.Length>0){var t=Label(im.transform,text,new Vector2(size.x/2,size.y/2),size,fontSize);Stretch(t.rectTransform);}return b;
    }
    InputField InputBox(Transform parent,string value,Vector2 pos,Vector2 size){
        var im=Art(parent,"InputBg",pos,size,false);im.raycastTarget=true;var field=im.gameObject.AddComponent<InputField>();var text=Label(im.transform,value,new Vector2(size.x/2,size.y/2),size-new Vector2(25,8),23);text.alignment=TextAnchor.MiddleLeft;Stretch(text.rectTransform);text.rectTransform.offsetMin=new Vector2(12,3);text.rectTransform.offsetMax=new Vector2(-12,-3);field.textComponent=text;field.text=value;return field;
    }
    static RectTransform Rect(string name,Transform parent,Vector2 pos,Vector2 size){var r=new GameObject(name,typeof(RectTransform)).GetComponent<RectTransform>();r.SetParent(parent,false);r.anchorMin=r.anchorMax=new Vector2(0,1);r.pivot=new Vector2(.5f,.5f);r.anchoredPosition=new Vector2(pos.x,-pos.y);r.sizeDelta=size;return r;}
    static void Stretch(RectTransform r){r.anchorMin=Vector2.zero;r.anchorMax=Vector2.one;r.offsetMin=r.offsetMax=Vector2.zero;}
}
