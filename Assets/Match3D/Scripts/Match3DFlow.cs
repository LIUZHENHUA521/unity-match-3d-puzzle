using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(-500)]
public partial class Match3DFlow : MonoBehaviour {
    public static Match3DFlow Instance {get;private set;}
    public Match3DTheme library;
    public bool IsPvp {get;private set;}
    public bool LocalPreview {get;private set;}
    bool unlimitedMagnetRun;
    public bool UnlimitedTools=>!IsPvp&&unlimitedMagnetRun;
    public bool UnlimitedMagnet=>UnlimitedTools;
    public bool ServerApplying {get;private set;}
    bool blocksGameplay;
    public bool BlocksGameplay {get=>blocksGameplay||(rewardPresenter&&rewardPresenter.IsShowing);private set=>blocksGameplay=value;}
    public int InitialCount {get;private set;}
    public bool CreationInProgress {get;private set;}
    public float PendingCreationWait {get;private set;}
    IEnumerator sourceCreationSteps;Coroutine sourceCreationRunner;int creationVersion;
    public int Stage {get;private set;}
    public int ThemeIndex {get;private set;}
    public string ScreenName {get;private set;}
    public int ScreenVersion {get;private set;}
    public string LastResult {get;private set;}
    public int TodaySeed {get;private set;}
    public float AppliedModelSize {get;private set;}=1f;
    CanvasGroup startupInput;
    Match3DStartupView startupView;
    bool startupSdkReady,startupFontReady,startupServicesReady;
    public bool StartupComplete {get;private set;}
    Canvas canvas;RectTransform page,modal;Text timer,remaining,peerText;Match3DTips tips;
    ItemPhysicsController pilePhysics;
    LayeredItemPile layeredPile;
    Match3DStagePreloader stagePreloader;
    Match3DDiagnostics diagnostics;
    Match3DGameplayHUD gameplayHUD;
    Button moveToolButton,findToolButton,shuffleToolButton;
    Match3DNetwork network;AudioSource music,effects;int pvpUndo=3;bool resultShown,pendingPick;
    int displayedTime=-1,displayedCount=-1,displayedPeer=-1;
    string runDate,roomId="",lobbyStatus="",endpoint;int otherCleared;
    public bool SettingsMusic {get {return PlayerPrefs.GetInt("goose.music",1)==1;} }
    public bool SettingsEffects {get {return PlayerPrefs.GetInt("goose.effects",1)==1;} }
    public bool SettingsVibration {get {return PlayerPrefs.GetInt("goose.vibration",1)==1;} }
    ThemeEntry CurrentTheme {get {return library.themes[ThemeIndex];} }
    public static string Today {get {return DateTime.UtcNow.AddHours(8).ToString("yyyy-MM-dd");} }
    void Awake(){
        Instance=this;
        // Hide the inherited demo before SDK/font initialization can yield a frame.
        // Keep its objects and colliders available to the existing gameplay services.
        HideLegacyPresentation();
        // Show the same branded cover as the WeChat loader before initialization yields.
        Match3DDefaultFont.InitializeFallback();
        var uiRoot=Match3DUIView.Load("UIRoot",null);
        canvas=uiRoot.GetComponent<Canvas>();canvas.name="Goose UI";
        page=uiRoot.Get<RectTransform>("Page");
        startupInput=uiRoot.gameObject.AddComponent<CanvasGroup>();
        startupInput.interactable=false;
#if !DAILY_WEB_PREVIEW
        ScreenName="loading";
        startupView=Match3DUIView.Load("StartupLoading",page).GetComponent<Match3DStartupView>();
        startupView.SetProgress(0,"正在准备游戏…");
#endif
    }
    public static void HideLegacyPresentation(){
        var pot=GameObject.Find("Cooking Pot");
        if(pot)foreach(var renderer in pot.GetComponentsInChildren<Renderer>(true))renderer.enabled=false;
        foreach(var c in FindObjectsOfType<Canvas>())c.gameObject.SetActive(false);
    }
    System.Collections.IEnumerator Start(){
        // Let the loading canvas paint before starting any synchronous service work.
        yield return null;
        while(true){
            Exception failure=null;
            yield return Match3DStartupRunner.Run(InitializeStartup(),error=>failure=error);
            if(failure==null)break;
            Debug.LogWarning("Startup incomplete: "+failure.Message);
            if(!startupView){Debug.LogException(failure);yield break;}
            bool retry=false;startupInput.interactable=true;
            startupView.ShowError(failure.Message,()=>retry=true);
            while(!retry)yield return null;
            startupInput.interactable=false;
        }
        if(startupView)startupView.SetProgress(1,"准备完成");
        yield return null;
        StartupComplete=true;startupInput.interactable=true;
#if UNITY_WEBGL && !UNITY_EDITOR && !DAILY_WEB_PREVIEW
        WeChatWASM.WX.ReportGameStart();
#endif
#if !DAILY_WEB_PREVIEW
        // Home cancels Flow coroutines, so all startup work must finish first.
        Home();
#else
        LocalPreview=true;ScreenName="preview";
        new GameObject("DailyWebPreview").AddComponent<DailyWebPreview>();
#endif
    }
    T StartupService<T>() where T:Component {var existing=GetComponent<T>();return existing?existing:gameObject.AddComponent<T>();}
    IEnumerator InitializeStartup(){
        if(startupView)startupView.SetProgress(.05f,"正在连接微信…");
#if UNITY_WEBGL && !UNITY_EDITOR && !DAILY_WEB_PREVIEW
        if(!startupSdkReady){
            bool completed=false;
            WeChatWASM.WX.InitSDK(code=>{completed=true;Debug.Log("WX SDK init: "+code);});
            float deadline=Time.realtimeSinceStartup+20;
            while(!completed&&Time.realtimeSinceStartup<deadline)yield return null;
            if(!completed)throw new InvalidOperationException("微信初始化超时，请重试");
            startupSdkReady=true;
        }
#endif
#if UNITY_WEBGL && !UNITY_EDITOR && !DAILY_WEB_PREVIEW
        Application.targetFrameRate=60;
        QualitySettings.antiAliasing=0;
        if(Camera.main){Camera.main.allowHDR=false;Camera.main.allowMSAA=false;}
#endif
        if(startupView)startupView.SetProgress(.2f,"正在准备字体…");
        if(!startupFontReady){yield return Match3DDefaultFont.Initialize();startupFontReady=true;}
        // One bounded shadow map covers the pot, including the camera's 12-unit height.
        QualitySettings.shadows=ShadowQuality.All;
        QualitySettings.shadowResolution=ShadowResolution.Medium;
        QualitySettings.shadowDistance=20f;
        QualitySettings.shadowCascades=0;
        Match3DDefaultFont.Apply(canvas.gameObject);
        if(startupView)startupView.SetProgress(.4f,"正在准备游戏资源…");
        yield return null;
        if(!startupServicesReady){
            if(!music){music=gameObject.AddComponent<AudioSource>();music.loop=true;music.volume=.25f;}
            if(!effects){effects=gameObject.AddComponent<AudioSource>();effects.volume=.65f;}
            pilePhysics=StartupService<ItemPhysicsController>();
            layeredPile=StartupService<LayeredItemPile>();
            StartupService<BoardItemPool>();
            stagePreloader=StartupService<Match3DStagePreloader>();
            StartupService<Match3DTray>();
            diagnostics=StartupService<Match3DDiagnostics>();
#if !DAILY_WEB_PREVIEW
        network=StartupService<Match3DNetwork>();network.Received=Receive;
        endpoint=PlayerPrefs.GetString("goose.endpoint",library.pvpEndpoint);
        if(endpoint.TrimEnd('/')=="https://goose-3d-pvp-197372-5-1312757046.sh.run.tcloudbase.com"||endpoint.TrimEnd('/')=="https://lzh.tax") {
            PlayerPrefs.DeleteKey("goose.endpoint");PlayerPrefs.Save();endpoint=library.pvpEndpoint;
        }
        StartupService<Match3DABExperiments>().Initialize(endpoint);
        StartupService<Match3DGameRecords>().Initialize(endpoint);
        StartupService<Match3DRewards>().Initialize(endpoint);
        StartupService<Match3DMerchantAuthorization>().Initialize(endpoint);
        StartupService<Match3DCustomerService>().Initialize(endpoint);
        if(!Match3DInvitations.Instance)StartupService<Match3DInvitations>().Initialize(endpoint);
#endif
            startupServicesReady=true;
        }
        if(startupView)startupView.SetProgress(.65f,"正在同步账号数据…");
#if UNITY_WEBGL && !UNITY_EDITOR && !DAILY_WEB_PREVIEW
        var account=Match3DInvitations.Instance;bool ready=false;
        if(account.HasPending&&!account.Busy)account.RefreshRecordLogin();
        yield return account.EnsureReady(value=>ready=value);
        if(!ready)throw new InvalidOperationException(string.IsNullOrEmpty(account.Status)?"登录失败，请重试":account.Status);
        // Login includes unfinished-tool recovery and the initial mailbox request.
        float accountDeadline=Time.realtimeSinceStartup+35;
        while(account.Busy&&Time.realtimeSinceStartup<accountDeadline)yield return null;
        if(account.Busy)throw new InvalidOperationException("账号同步超时，请重试");
        if(!account.Ready)throw new InvalidOperationException("登录已失效，请重试");
        if(account.HasPending)throw new InvalidOperationException("道具数据同步失败，请重试");
        float abDeadline=Time.realtimeSinceStartup+3;
        while(Match3DABExperiments.Instance&&!Match3DABExperiments.Instance.Ready&&Time.realtimeSinceStartup<abDeadline)yield return null;
        if(!account.MailLoaded){
            account.RefreshMail();
            float mailDeadline=Time.realtimeSinceStartup+20;
            while(account.Busy&&Time.realtimeSinceStartup<mailDeadline)yield return null;
            if(!account.MailLoaded||account.Busy)throw new InvalidOperationException("邮件同步失败，请重试");
        }
#endif
        if(startupView)startupView.SetProgress(.9f,"正在准备主页…");
        var home=Resources.LoadAsync<GameObject>("Match3D/UI/Home");yield return home;
        if(!home.asset)throw new InvalidOperationException("主页资源加载失败，请重试");
    }
    void OnDestroy(){CancelSourceCreation();if(stagePreloader)stagePreloader.Cancel();BoardPhysicsSettings.End();if(Instance==this)Instance=null;Time.timeScale=1;}
    void Update(){
        if(ScreenName=="game") {
            TickProgressDisplay();
            if(BoardTools.Enabled){
                if(moveToolButton)moveToolButton.interactable=true;
                if(findToolButton)findToolButton.interactable=true;
                if(shuffleToolButton)shuffleToolButton.interactable=true;
            }
            int t=Mathf.CeilToInt(GameManager.Instance.currentLevelTime);
            int count=LevelGenerator.Instance.objectsOnScene.Count;
            if(timer&&t!=displayedTime){timer.text=GameManager.Instance.currentLevelTime>0?(t/60).ToString("00")+":"+(t%60).ToString("00"):(gameplayHUD&&Stage==1?"∞":"00:00");displayedTime=t;}
            if(remaining&&count!=displayedCount)remaining.text="剩余 "+count+" / "+InitialCount;
            if(peerText&&(count!=displayedCount||otherCleared!=displayedPeer))peerText.text="我方 "+(InitialCount-count)+" / "+InitialCount+"    对手 "+otherCleared+" / "+InitialCount;
            displayedCount=count;displayedPeer=otherCleared;
        }
    }

    void Clear(){
        ScreenVersion++;
        ResetToolOffer();
        displayedTime=displayedCount=displayedPeer=-1;
        foreach(Transform t in page){t.gameObject.SetActive(false);Destroy(t.gameObject);}modal=null;timer=null;remaining=null;peerText=null;if(tips)tips.Hide();BlocksGameplay=false;
    }
    void Music(AudioClip clip){if(music.clip!=clip){music.clip=clip;if(clip)music.Play();}music.mute=!SettingsMusic;}
    void Click(){if(SettingsEffects&&library.clickSound)effects.PlayOneShot(library.clickSound);}
    public void SelectSound(){if(SettingsEffects&&library.selectSound)effects.PlayOneShot(library.selectSound);}
    public void MatchSound(){if(SettingsEffects&&library.matchSound)effects.PlayOneShot(library.matchSound);}
    void StopBoard(bool preservePool=false){ComboProbeController.End();ResetToolOffer();if(Match3DGameRecords.Instance)Match3DGameRecords.Instance.Finish("quit","home");ResetRevival();if(stagePreloader)stagePreloader.Cancel();if(Camera.main){var highlight=Camera.main.GetComponent<ItemHighlight>();if(highlight)highlight.ClearAll();}GameAudioCatalog.ResetLevel();CancelSourceCreation();BoardPhysicsSettings.End();if(InputController.Instance)InputController.Instance.CancelSelection();if(layeredPile)layeredPile.Clear();if(pilePhysics)pilePhysics.Clear();if(diagnostics)diagnostics.ResetProbe();Time.timeScale=1;StopAllCoroutines();GameManager.Instance.StopAllCoroutines();GameManager.Instance.gameStatus=GameManager.GameStatus.PREPARING;LevelGenerator.Instance.ClearBoard();if(!preservePool)BoardItemPool.Clear();resultShown=false;pendingPick=false;}
    public void Home(){
#if DAILY_WEB_PREVIEW
        if(DailyWebPreview.Instance)DailyWebPreview.Instance.Replay();
        return;
#else
        if(network)network.Disconnect();StopBoard();LocalPreview=false;IsPvp=false;Clear();ScreenName="home";Music(library.homeMusic);
        Match3DHomeView.Create(this,page,Match3DHomeView.Tab.Home);
        if(Match3DMerchantAuthorization.Instance)Match3DMerchantAuthorization.Instance.OnHome();
#endif
    }
    public void ShowHomeTab(Match3DHomeView.Tab tab){
        if(tab==Match3DHomeView.Tab.Home){Home();return;}
        if(network)network.Disconnect();StopBoard();IsPvp=false;Clear();
        ScreenName=tab==Match3DHomeView.Tab.Collection?"collection-tab":"pets-tab";
        Music(library.homeMusic);Match3DHomeView.Create(this,page,tab);
    }
#if DAILY_WEB_PREVIEW
    public void SetDailyPreviewBusy(bool value){startupInput.interactable=!value;BlocksGameplay=value;}
#endif
    public void HomeClick(){Click();}
    public void ShowRewardInfo(){
        BlocksGameplay=true;var view=OpenModal("Reward");
        view.Text("Amounts","本科局：毕业可领 "+Match3DHomeView.BachelorReward+" 元奖励\n\n清北局：毕业可领 "+Match3DHomeView.EliteReward+" 元奖励");
        view.Bind("Close",()=>CloseModal());
    }
    public void ShowThemes(){
        // Both home challenge entries use the sole countryside theme directly.
        BeginReward(0,"elite");
    }
    public bool HasGoose(string id){return PlayerPrefs.GetInt("goose.collection."+id,0)>0;}
    public void ShowCollection(){
        Clear();ScreenName="collection";var view=Match3DUIView.Load("Collection",page);view.Bind("Back",Home);
        view.Text("Count","已收集 "+library.themes.Count(t=>HasGoose(t.id))+" / "+library.themes.Length);
        for(int i=0;i<library.themes.Length;i++){
            int index=i;var theme=library.themes[i];bool got=HasGoose(theme.id);
            var art=view.Get<Image>("Goose"+i);art.sprite=Resources.Load<Sprite>("GooseArt/Sprites/"+theme.gooseSprite);
            art.color=got?Color.white:new Color(.28f,.28f,.28f,.8f);
            view.Text("Name"+i,theme.title);view.Text("Status"+i,got?"已获得 · 永久收藏":"通关主题第二关解锁");
            view.Text("Play"+i+"Text",got?"再挑战":"去解锁");view.Bind("Play"+i,()=>Begin(index));
        }
    }
    Match3DDailyLevel dailyLevel;
    string challengeTier="bachelor";
    string CurrentModelThemeTitle => Match3DModelThemes.Title(ActiveModelTheme,CurrentTheme.title);
    LevelLayoutConfig.Profile SourceProfile(bool tutorial=false) => Match3DModelThemes.Get(ActiveModelTheme,CurrentTheme.sourceEnvironment,tutorial);
    public string SourceEnvironmentForTheme(int theme){return library.themes[theme].sourceEnvironment;}
    public void BeginReward(int theme,string tier){
#if UNITY_EDITOR
        // Editor play does not have a WeChat identity or reward qualification.
        Begin(theme);challengeTier=tier;LocalPreview=true;unlimitedMagnetRun=true;
        ShowTips("编辑器本地试玩 · 道具免费 · 不发放红包");
#else
        if(Match3DRewards.Instance)Match3DRewards.Instance.Begin(theme,tier,library.themes[theme].id);
#endif
    }
    public void BeginQualified(int theme,string runId,string tier,string version,Match3DDailyLevel level=null,bool testToolsEnabled=false){
        BeginProgressRun();unlimitedMagnetRun=testToolsEnabled;runModelTheme=null;dailyLevel=level;challengeTier=tier;LocalPreview=false;
        if(network)network.Disconnect();StopBoard();IsPvp=false;ThemeIndex=theme;Stage=1;runDate=level!=null?level.date:Today;Match3DGameRecords.Instance.QualifiedRun(runId,tier,version,level);StartStage();
    }
    public void ShowRedPackets(){BlocksGameplay=true;var view=OpenModal("RedPackets");view.Bind("Close",()=>CloseModal());view.GetComponent<Match3DRewardsView>().Connect();}
    public void BeginDailyPractice(int theme,Match3DDailyLevel level){
        if(level==null){Begin(theme);return;}
        BeginProgressRun();runModelTheme=null;
        if(network)network.Disconnect();StopBoard();IsPvp=false;unlimitedMagnetRun=Match3DTestPrivileges.AccountEnabled;dailyLevel=level;ThemeIndex=theme;Stage=1;runDate=level.date;
        if(Match3DGameRecords.Instance)Match3DGameRecords.Instance.NewDailyRun(level);StartStage();
    }
    public void Begin(int theme){
        BeginProgressRun();
        dailyLevel=null;runModelTheme=null;challengeTier="bachelor";
#if UNITY_EDITOR
        var selectedTheme=UnityEditor.EditorPrefs.GetString(EditorThemePreference,"");
        if(Match3DModelThemes.TryGet(selectedTheme,SourceEnvironmentForTheme(theme),out _))runModelTheme=selectedTheme;
#endif
        // Ordinary MainLevel starts with the recovered first-stage row.
        if(network)network.Disconnect();StopBoard();LocalPreview=false;IsPvp=false;unlimitedMagnetRun=Match3DTestPrivileges.AccountEnabled;ThemeIndex=theme;Stage=1;runDate=Today;if(Match3DGameRecords.Instance)Match3DGameRecords.Instance.NewRun();StartStage();
    }
    public void RestartStage(){if(Match3DGameRecords.Instance)Match3DGameRecords.Instance.Finish("quit","restart");StopBoard();StartStage();}
    public void NextStage(){
        // The source keeps the theme presentation alive while MainLevel rolls
        // into the layered challenge. Rebuilding the same scene and collision
        // shell here caused a synchronous hitch before the creation coroutine
        // could show its first batch.
        layeredPile.TakePresentation(out var reusedPresentation,out var reusedBoundary);
        StopBoard(true);Stage=2;StartStage(reusedPresentation,reusedBoundary,false);
        ShowTips((dailyLevel?.tier??challengeTier)=="elite"?"基础题完成，进入清北压轴题！":"基础题完成，进入本科压轴题！");
    }
    void StartStage(Match3DThemePresentation reusedPresentation=null,GameObject reusedBoundary=null,bool countAttempt=true){
        progressDisplay.ResetStage();nextProgressSample=0;
        if(!IsPvp){
            if(PlayerPrefs.GetString("goose.result.date","")!=Today){PlayerPrefs.SetString("goose.result.date",Today);PlayerPrefs.SetInt("goose.result.attempts",0);}
            // Count a new challenge at tutorial start, and each retry of the final stage.
            if(countAttempt)PlayerPrefs.SetInt("goose.result.attempts",PlayerPrefs.GetInt("goose.result.attempts",0)+1);
            PlayerPrefs.Save();
        }
        var date=DateTime.ParseExact(runDate,"yyyy-MM-dd",System.Globalization.CultureInfo.InvariantCulture).AddHours(4);
        TodaySeed=dailyLevel!=null&&!IsPvp?dailyLevel.seed:ChallengePlan.DailySeed(CurrentTheme.id,Stage,date);
        bool sourceTutorial=!IsPvp&&Stage==1;
        var source=sourceTutorial||Stage==2?SourceProfile(sourceTutorial):null;
        var prefabs=source!=null?source.prefabs:CurrentTheme.prefabs;
        IBoardRandom sourceRandom=dailyLevel!=null&&!IsPvp?dailyLevel.RandomForStage(Stage):source!=null?BoardGlobalRandom.Instance:null;
        if(dailyLevel!=null&&!IsPvp&&dailyLevel.generatorVersion=="shared-v2"){
            if(!dailyLevel.IsValid(prefabs.Length)){Notice("每日关卡版本不兼容，请更新游戏");return;}
            SpawnDailyOpening(prefabs,sourceTutorial,reusedPresentation,reusedBoundary);HUD();Music(library.gameMusic);return;
        }
        var tiles=sourceTutorial?LevelLayoutConfig.GenerateFirstStage(source,sourceRandom):dailyLevel!=null&&!IsPvp?Match3DDailyLevel.Generate(prefabs.Length,dailyLevel.layers,sourceRandom):source!=null?ChallengePlan.GenerateSource(sourceRandom,prefabs.Length,source.layers):ChallengePlan.Generate(TodaySeed,prefabs.Length,new[]{new ChallengeLayer(12,2,.65f)});
        Spawn(tiles,TodaySeed,dailyLevel!=null&&!IsPvp&&Stage==2?dailyLevel.seconds:source!=null?source.seconds:120,prefabs,sourceRandom,sourceTutorial,reusedPresentation,reusedBoundary);HUD();Music(library.gameMusic);
        if(sourceTutorial)stagePreloader.Begin(CurrentTheme.sourceEnvironment,SourceProfile());

    }
    public const float BoardItemScale = 1.5f;
    sealed class BoardCreateInfo {public ChallengeTile tile;public Vector3 position;}
    public const int SourceFrameCreateCount=3;
    public const int HiddenCreationSliceLimit=12;
    public const float SourceCreateSeconds=2f,SourceMaximumCreateInterval=.03f;
    public const double CreationSliceBudgetMilliseconds=2.5;
    public const float SlowCreationFrameSeconds=1f/45;
    public static int CreationSliceLimit(bool hidden,float previousFrameSeconds)=>previousFrameSeconds>SlowCreationFrameSeconds?1:(hidden?HiddenCreationSliceLimit:SourceFrameCreateCount);
    public int SlowFrameCreationYieldCount {get;private set;}
    public double MaxCreationSliceMilliseconds {get;private set;}
    public int CreationBudgetYieldCount {get;private set;}
#if UNITY_EDITOR
    [NonSerialized] public double CreationBudgetOverride=-1;
    [NonSerialized] public float CreationFrameSecondsOverride=-1;
    [NonSerialized] public bool UseLegacyCreationScheduleForValidation;
#endif
    public static float SourceCreateInterval(int count)=>Mathf.Min(SourceCreateSeconds/Mathf.Max(1,count)*SourceFrameCreateCount,SourceMaximumCreateInterval);
    void Spawn(List<ChallengeTile> tiles,int seed,int seconds,GameObject[] sourcePrefabs=null,IBoardRandom sourceRandom=null,bool sourceTutorial=false,Match3DThemePresentation reusedPresentation=null,GameObject reusedBoundary=null){
        var prefabs=sourcePrefabs??CurrentTheme.prefabs;
        LastResult="";InitialCount=tiles.Count;
        if(!IsPvp&&Match3DGameRecords.Instance)Match3DGameRecords.Instance.Prepare(CurrentTheme.id,Stage,runDate,tiles.Count,seconds);
        bool layered=!IsPvp&&(Stage==1||Stage==2);
        ComboProbeController.BeginStage(layered,seed);
        ItemColliderGeometry.ConfigureLayers();
        if(!IsPvp)BoardPhysicsSettings.Begin();
        GameManager.Instance.currentLevelTime=seconds;GameManager.Instance.gameStatus=GameManager.GameStatus.PREPARING;
        if(layered){
            AppliedModelSize=BoardLayoutSettings.ModelSizeMultiplier;
            float highestLayer=tiles.Max(t=>t.height);
            float visibilityLimit=sourceTutorial?4f:dailyLevel!=null?dailyLevel.limitHeight:SourceProfile().limitHeight;
            if(!sourceTutorial)visibilityLimit=BoardEnvironment.MapHeight(visibilityLimit,highestLayer);
            BoardEnvironment.Select(CurrentTheme.sourceEnvironment,sourceTutorial);layeredPile.Begin(true,reusedPresentation,reusedBoundary,visibilityLimit,sourceTutorial);
            var random=sourceRandom??BoardGlobalRandom.Instance;
            // LoopCreateObj builds all descriptions bottom-to-top, then List.Sort compares Y.
            // Rotation is sampled later, at CreateUnitObj, in the sorted creation order.
            var descriptions=new List<BoardCreateInfo>(tiles.Count);float previousHeight=1.5f;
            foreach(var group in tiles.GroupBy(t=>t.layer).OrderBy(g=>g.Key)){
                float height=sourceTutorial?group.First().height:BoardEnvironment.MapHeight(group.First().height,highestLayer);
                foreach(var tile in group)descriptions.Add(new BoardCreateInfo{tile=tile,position=BoardEnvironment.Position(previousHeight,height,random)});
                previousHeight=height; // saved _layerMinDistance = 0
            }
            descriptions.Sort((a,b)=>a.position.y.CompareTo(b.position.y));
            CreationInProgress=true;int version=++creationVersion;
            sourceCreationSteps=SourceCreationSteps(descriptions,prefabs,random,version);
            sourceCreationRunner=StartCoroutine(RunSourceCreation(version));
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
    GameObject CreateBoardItem(ChallengeTile tile,GameObject prefab,Vector3 position,Quaternion rotation,bool layered,bool sourceBoard=false,Match3DDailyItem openingItem=null){
        var obj=layered?BoardItemPool.Get(prefab,prefab.name):Instantiate(prefab);
        obj.transform.SetPositionAndRotation(position,rotation);
        bool sourceModel=layered||sourceBoard;
        obj.transform.localScale*=sourceModel?BoardLayoutSettings.ModelScale(prefab):BoardItemScale;
        ItemColliderGeometry.Apply(obj,prefab.name,sourceModel);
        if(layered&&BoardEnvironment.UsesSoftContainer){SoftTransitionContainer.Fit(obj);position=obj.transform.position;}
        if(openingItem!=null){obj.transform.SetPositionAndRotation(openingItem.position,openingItem.rotation);obj.transform.localScale=openingItem.scale;position=openingItem.position;}
        ItemFlightMotion.Bind(obj,prefab.name,layered,prefab);
        if(layered)GameAudioCatalog.Bind(obj,prefab.name);
        Match3DTrayDisplayBinding.Bind(obj,prefab);
        obj.name="type_"+tile.type;var tileComponent=obj.GetComponent<Match3DTile>()??obj.AddComponent<Match3DTile>();tileComponent.id=tile.id;
        var body=obj.GetComponent<Rigidbody>();body.useGravity=true;BoardPhysicsSettings.Apply(body);
        // ObjectUnit.Init calls Rigidbody.set_isKinematic(true), j6539. The creation
        // loop activates eligible bodies only after the initial growth reaches full size.
        pilePhysics.Register(body,layered);
        LevelGenerator.Instance.objectsOnScene.Add(obj);
        ComboProbeController.Register(obj,prefab);
        if(layered){var goose=obj.GetComponent<SpecialItemAnimation>();goose?.InitializeSource();goose?.ReturnToPile();layeredPile.Register(body,initialHeight:position.y);}
        return obj;
    }
    IEnumerator SourceCreationSteps(List<BoardCreateInfo> descriptions,GameObject[] prefabs,IBoardRandom random,int version){
        float wait=SourceCreateInterval(descriptions.Count);
        MaxCreationSliceMilliseconds=0;CreationBudgetYieldCount=0;SlowFrameCreationYieldCount=0;BoardItemPool.ResetMetrics();
        double budget=CreationSliceBudgetMilliseconds;
#if UNITY_EDITOR
        if(CreationBudgetOverride>=0)budget=CreationBudgetOverride;
#endif
        var slice=System.Diagnostics.Stopwatch.StartNew();int createdInSlice=0;
        for(int i=0;i<descriptions.Count;i++){
            if(version!=creationVersion)yield break;
            // Backpressure bounds each item's wait between scale steps as well as
            // the work per tick. Do not accumulate an entire half-grown board.
            while(layeredPile.CreationGrowingCount>=LayeredItemPile.CreationGrowingLimit){
                yield return 0f;
                if(version!=creationVersion)yield break;
                slice.Restart();createdInSlice=0;
            }
            var info=descriptions[i];var created=CreateBoardItem(info.tile,prefabs[info.tile.type],info.position,BoardEnvironment.Rotation(random),true);
            double elapsed=slice.Elapsed.TotalMilliseconds;
            MaxCreationSliceMilliseconds=System.Math.Max(MaxCreationSliceMilliseconds,elapsed);
            createdInSlice++;
            bool hiddenPreparation=Stage==2&&!layeredPile.IsInitiallyVisible(created.transform.position.y);
#if UNITY_EDITOR
            hiddenPreparation&=!UseLegacyCreationScheduleForValidation;
#endif
            float previousFrameSeconds=Time.unscaledDeltaTime;
#if UNITY_EDITOR
            if(CreationFrameSecondsOverride>=0)previousFrameSeconds=CreationFrameSecondsOverride;
#endif
            int limit=CreationSliceLimit(hiddenPreparation,previousFrameSeconds);
#if UNITY_EDITOR
            // Deterministic iterator fixtures control timing themselves.
            if(CreationBudgetOverride>=0||UseLegacyCreationScheduleForValidation)limit=HiddenCreationSliceLimit;
#endif
            // Hidden supports retain exactly the same Register/visibility/physics
            // path. Only their presentation delay is removed. Visible entries
            // retain source global-index batches, with the existing time budget.
            if(!hiddenPreparation&&i%SourceFrameCreateCount==0){yield return wait;slice.Restart();createdInSlice=0;}
            else if(i+1<descriptions.Count&&(elapsed>=budget||createdInSlice>=limit)){
                CreationBudgetYieldCount++;if(limit==1)SlowFrameCreationYieldCount++;yield return 0f;slice.Restart();createdInSlice=0;
            }
        }
        yield return .5f;
        if(version!=creationVersion)yield break;
        // A bounded growth queue can outlive the source's fixed half-second tail.
        // Keep input locked and drain it naturally; never snap the remainder to full size.
        while(layeredPile.CreationGrowingCount>0){
            yield return 0f;
            if(version!=creationVersion)yield break;
        }
        layeredPile.StopCreationGrowth(); // StopCoroutine("LoopCrowObjSize")
        yield return .2f;
        if(version!=creationVersion)yield break;
        // The special real-physics branch restores gravity here; its saved switch is false.
        yield return .3f;
        if(version!=creationVersion)yield break;
        layeredPile.FinishCreation(); // FinishCreate(true), height check, then optimization starts.
        CreationInProgress=false;PendingCreationWait=0;
        GameManager.Instance.gameStatus=GameManager.GameStatus.PLAY;
        if(!IsPvp&&Match3DGameRecords.Instance)Match3DGameRecords.Instance.StartRecord();
        BoosterManager.Instance.BoostersInteractibility(true);
    }
    IEnumerator RunSourceCreation(int version){
        var steps=sourceCreationSteps;
        while(version==creationVersion&&steps.MoveNext()){
            PendingCreationWait=(float)steps.Current;
            // Source yields once per batch. WaitForSeconds already resumes on
            // a later frame; an extra null yield adds ~100 frames to 300 items.
            if(PendingCreationWait>0)yield return new WaitForSeconds(PendingCreationWait);
            else yield return null;
            // Unlike WaitForSeconds, a budget's null yield also resumes while
            // paused. Hold the iterator here until scaled time resumes.
            while(version==creationVersion&&Time.timeScale<=0)yield return null;
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
        if(pvp){network.Send(new Match3DMessage{type="loaded"});Notice("已准备好，等待对手…");}
        else {GameManager.Instance.gameStatus=GameManager.GameStatus.PLAY;if(Match3DGameRecords.Instance)Match3DGameRecords.Instance.StartRecord();BoosterManager.Instance.BoostersInteractibility(true);}
    }
    void HUD(){
        Clear();ScreenName="game";resultShown=false;BlocksGameplay=false;
        var view=Match3DUIView.Load(IsPvp?"PvpGameHUD":"GameHUD",page);
        gameplayHUD=view.GetComponent<Match3DGameplayHUD>();
        if(gameplayHUD)view.Show("Settings",false);
        view.Text("Title",IsPvp?"好友对战":CurrentModelThemeTitle+" · 第"+Stage+"关");
        view.Bind("Pause",()=>ShowSettings(true));timer=view.Get<Text>("Timer");remaining=view.Get<Text>("Remaining");
        view.Show("Peer",IsPvp);peerText=IsPvp?view.Get<Text>("Peer"):null;
        view.Text("UndoText",!IsPvp?"移出":"撤回");moveToolButton=view.Bind("Undo",()=>UseTool(0,Undo));
        view.Text("ShuffleText","打乱");
        shuffleToolButton=view.Bind("Shuffle",()=>UseTool(2,()=>BoardTools.MessUp()));
        view.Text("HintText",IsPvp?"认输":"凑齐");
        findToolButton=view.Bind("Hint",()=>UseTool(1,()=>{if(IsPvp)network.Send(new Match3DMessage{type="surrender"});else BoosterManager.Instance.HintButton();}));
        if(!IsPvp){view.Bind("AddTime",()=>UseTool(BoardTools.AddTimeType,()=>BoardTools.AddTime()));view.Show("AddTime",BoardTools.Enabled);}
        foreach(var key in new[]{"Undo","Hint"})view.Show(key,IsPvp||BoardTools.Enabled);
        view.Show("Shuffle",!IsPvp&&BoardTools.Enabled);
        view.Show("Freeze",false);view.Bind("Freeze",()=>BoosterManager.Instance.Freeze());
    }
    public bool HandleResult(bool win){
        if(ScreenName!="game"||resultShown||revivePending)return true;
        if(IsPvp){GameManager.Instance.gameStatus=GameManager.GameStatus.PREPARING;return true;}
        if(!win&&TryOfferRevival())return true;
        return FinishSoloResult(win);
    }
    bool FinishSoloResult(bool win){
        if(Match3DGameRecords.Instance)Match3DGameRecords.Instance.Finish(win?"win":"fail",win?"cleared":GameManager.Instance.currentLevelTime<=0&&Stage!=1?"timeout":"tray_full");
        if(win&&Stage==1){NextStage();return true;}
        resultShown=true;GameManager.Instance.gameStatus=win?GameManager.GameStatus.FINISH:GameManager.GameStatus.FAIL;
        if(win&&Stage==2){PlayerPrefs.SetInt("goose.collection."+CurrentTheme.id,1);PlayerPrefs.SetString("goose.completed."+CurrentTheme.id,runDate);PlayerPrefs.Save();}
        Result(win,win?(Stage==1?"第一关通过！":"抓到大鹅啦！"):"差一点就成功了",win?(Stage==1?"热身结束，真正的挑战来了":"已点亮 "+CurrentModelThemeTitle+" 图鉴"):"同一关再试一次，留意槽位和遮挡");return true;
    }
    void Result(bool win,string title,string detail){
        LastResult=title;BlocksGameplay=true;Time.timeScale=1;
        if(!IsPvp){
            float progress=DisplayProgress(win);
            string key=progressDisplay.BestKey(CurrentTheme.id,Stage);
            float best=Mathf.Max(progress,PlayerPrefs.GetFloat(key,0));PlayerPrefs.SetFloat(key,best);PlayerPrefs.Save();
            var result=OpenModal(win?"ResultSuccess":"ResultFailure").GetComponent<Match3DResultView>();
            result.Configure(PlayerPrefs.GetInt("goose.result.attempts",1),progress,best,win?(Action)Home:RestartStage,Home,ShareResult);
        }else{
        var view=OpenModal("Result");view.Text("Title",title);view.Text("Detail",detail);
        view.Get<Image>("Goose").sprite=Resources.Load<Sprite>("GooseArt/Sprites/"+CurrentTheme.gooseSprite);
        view.Text("ContinueText",IsPvp?"再次匹配":win&&Stage==1?"挑战第二关":win?"查看图鉴":"再试一次");
        view.Bind("Continue",()=>{if(IsPvp){StopBoard();ShowPvp();}else if(win&&Stage==1)NextStage();else if(win){StopBoard();ShowCollection();}else RestartStage();});
        view.Bind("Home",Home);
        }
        var clip=win?library.winSound:library.loseSound;if(SettingsEffects&&clip)effects.PlayOneShot(clip);
#if UNITY_WEBGL && !UNITY_EDITOR && !DAILY_WEB_PREVIEW
        if(!win&&SettingsVibration)WeChatWASM.WX.VibrateShort(new WeChatWASM.VibrateShortOption{type="light"});
#elif UNITY_IOS || UNITY_ANDROID
        if(!win&&SettingsVibration&&!Application.isEditor)Handheld.Vibrate();
#endif
    }
#if UNITY_WEBGL && !UNITY_EDITOR && !DAILY_WEB_PREVIEW
    [System.Runtime.InteropServices.DllImport("__Internal")] static extern void GooseShareResult(string query);
#endif
    void ShareResult(){
#if UNITY_WEBGL && !UNITY_EDITOR && !DAILY_WEB_PREVIEW
        GooseShareResult(DailyShareQuery());
#else
        Notice("请在微信小游戏中分享给好友");
#endif
    }
    void Undo(){
        if(IsPvp){if(pvpUndo>0&&!pendingPick&&!InputController.Instance.IsBusy){pendingPick=true;network.Send(new Match3DMessage{type="undo"});}}
        else BoosterManager.Instance.Undo();
    }
    public bool InterceptPick(GameObject obj){
        if(!IsPvp||ServerApplying)return false;
        if(pendingPick||BlocksGameplay||InputController.Instance.IsBusy)return true;
        var tile=obj.GetComponent<Match3DTile>();if(tile){pendingPick=true;network.Send(new Match3DMessage{type="pick",tileId=tile.id});}return true;
    }
    public void ShowModelSize(){
        if(IsPvp||Stage!=2||ScreenName!="game")return;
        Time.timeScale=0;BlocksGameplay=true;var view=OpenModal("ModelSize");
        view.Bind("Close",()=>CloseModal(true));view.Text("Current","当前这局："+AppliedModelSize.ToString("F2")+"×");
        var slider=view.Get<Slider>("Slider");slider.SetValueWithoutNotify(Mathf.RoundToInt(AppliedModelSize*100));
        Action refresh=()=>view.Text("Readout","新大小："+(slider.value/100f).ToString("F2")+"×");
        slider.onValueChanged.AddListener(_=>refresh());refresh();
        view.Bind("Minus",()=>slider.value--);view.Bind("Plus",()=>slider.value++);
        view.Bind("Apply",()=>{BoardLayoutSettings.ModelSizeMultiplier=slider.value/100f;Time.timeScale=1;RestartStage();});
        view.Bind("Reset",()=>slider.value=100);
    }
    public void ShowUserProfile(){
        BlocksGameplay=true;var view=OpenModal("UserProfile");view.Bind("Close",()=>CloseModal());view.GetComponent<Match3DUserProfileView>().Connect(Match3DInvitations.Instance);
    }
    public void ShowMailbox(){
        BlocksGameplay=true;var view=OpenModal("Mailbox");view.Bind("Close",()=>CloseModal());view.GetComponent<Match3DMailboxView>().Connect(Match3DInvitations.Instance);
    }
    public void ShowInvites(){
        BlocksGameplay=true;var view=OpenModal("InviteRewards");
        view.Bind("Close",()=>CloseModal());view.Bind("Invite",()=>Match3DInvitations.Instance.Share());
        view.GetComponent<Match3DInviteView>().Connect(Match3DInvitations.Instance);
        Match3DInvitations.Instance.Refresh();
    }
    void UseTool(int type,Action action){
        if(BoardTools.Enabled&&!IsPvp){
            string reason=ToolUnavailableReason(type);
            if(reason!=null){ShowTips(reason);return;}
        }else if(BlocksGameplay)return;
        if(!BoardTools.Enabled||IsPvp){action();return;}
        if(UnlimitedTools){
            if(BoardTools.RunPaid(type,()=>{BoosterManager.Instance.CheckHintInteractibility();action();}))BoardTools.RecordTestUse(type);
            else ShowTips("当前无法使用道具");
            return;
        }
        var service=Match3DInvitations.Instance;
        if(service&&(service.Busy||service.HasPending)){ShowTips("道具库存处理中，请稍后重试");return;}
        if(Match3DRewardAcquisition.Next(Match3DRewardAcquisition.Entry.Tool).route==Match3DRewardAcquisition.Route.Free)ShowToolOffer(type,action);
        else if(service&&service.Ready&&service.Inventory(type)>0)StartCoroutine(UseInventory(type,action,service));
        else ShowToolOffer(type,action);
    }

    public string ToolUnavailableReason(int type){
        if(type<0||type>=BoardTools.ToolCount)return "道具类型无效";
        if(BlocksGameplay)return "请先关闭当前弹窗";
        if(!GameManager.Instance.GameIsPlaying())return "当前无法使用道具，请在游戏中使用";
        if(type==BoardTools.AddTimeType&&(IsPvp||Stage==1))return "当前关卡不限时，无需使用加时";
        if(type==BoardTools.AddTimeType&&GameManager.Instance.currentLevelTime<=0)return "时间已用完，请先完成复活";
        if(InputController.Instance.ReturningToBowl)return "物品正在回锅，请稍候";
        if(BoardTools.Shuffling)return "正在洗牌，请稍候";
        var input=InputController.Instance;
        if(type==0&&input.objectsOnStack.Count==0)return "槽位中没有可移出的物品";
        if(type==1){
            if(input.objectsOnStack.Count==0)return "请先选择一个物品，再使用吸附";
            if(input.objectsOnStack.Count>=7)return "槽位已满，暂时无法吸附";
            if(!BoardTools.CanAutoFind(input.objectsOnStack))return "槽位即将满，需要一对相同物品才能吸附";
        }
        if(type==2&&BoardTools.ShuffleCandidates(LevelGenerator.Instance.objectsOnScene,input.objectsOnStack).Count==0)return "场上没有可洗牌的物品";
        return null;
    }
    bool CanApplyInventory(int boardVersion)=>ScreenName=="game"&&BoardTools.Enabled&&creationVersion==boardVersion&&GameManager.Instance.GameIsPlaying();
    IEnumerator UseInventory(int type,Action action,Match3DInvitations service){
        int boardVersion=creationVersion;
        BlocksGameplay=true;float speed=Time.timeScale;Time.timeScale=0;
        yield return service.Use(type,()=>{
            if(!CanApplyInventory(boardVersion))return false;
            return BoardTools.RunPaid(type,()=>{BoosterManager.Instance.CheckHintInteractibility();action();});
        });
        Time.timeScale=speed;BlocksGameplay=modal!=null;
        if(!string.IsNullOrEmpty(service.Status))Notice(service.Status);
    }
    public void ShowSettings(bool ingame){
        if(ingame&&Match3DInvitations.Instance&&Match3DInvitations.Instance.Busy)return;
        if(ingame&&!IsPvp)Time.timeScale=0;BlocksGameplay=true;
        var view=OpenModal("PauseSettings");view.Text("Title",ingame?"暂停 / 设置":"设置");
        Action close=()=>CloseModal(ingame&&!IsPvp);
        view.Bind("Close",close);view.Bind("Continue",close);view.Text("ContinueText",ingame?"继续游戏":"完成");
        SettingToggle(view,"Music",SettingsMusic,value=>{PlayerPrefs.SetInt("goose.music",value?1:0);music.mute=!value;PlayerPrefs.Save();});
        SettingToggle(view,"Effects",SettingsEffects,value=>{PlayerPrefs.SetInt("goose.effects",value?1:0);PlayerPrefs.Save();});
        SettingToggle(view,"Vibration",SettingsVibration,value=>{PlayerPrefs.SetInt("goose.vibration",value?1:0);PlayerPrefs.Save();});
        view.Show("Home",ingame);view.Bind("Home",Home);
        var skin=view.GetComponent<Match3DSettingsSkin>();
        if(skin){view.Show("Continue",false);view.Show("Home",false);skin.BindSupport(ingame);}
    }
    void SettingToggle(Match3DUIView view,string key,bool value,Action<bool> change){
        bool current=value;var skin=view.GetComponent<Match3DSettingsSkin>();
        void Refresh(){view.Text(key+"Text",current?"已开启":"已关闭");if(skin)skin.SetState(key,current);}
        Refresh();view.Bind(key,()=>{current=!current;change(current);Refresh();});
    }
    public void ShowPvp(){
        if(network)network.Disconnect();StopBoard();IsPvp=true;ThemeIndex=0;Stage=2;roomId="";lobbyStatus="邀请朋友来一场三消对决";DrawLobby();Music(library.pvpMusic);
    }
    void DrawLobby(){
        Clear();ScreenName="pvp";var view=Match3DUIView.Load("PvpLobby",page);view.Bind("Back",Home);
        view.Text("Status",lobbyStatus);view.Show("Room",!string.IsNullOrEmpty(roomId));view.Text("Room","房间号："+roomId);
        var address=view.Get<InputField>("Address");address.SetTextWithoutNotify(endpoint);
        address.onEndEdit.AddListener(v=>{endpoint=v.Trim();PlayerPrefs.SetString("goose.endpoint",endpoint);});
        bool connected=network.Connected;
        foreach(var key in new[]{"Match","Create","Code","Join"})view.Show(key,!connected);
        view.Show("Ready",connected);view.Show("Leave",connected);
        view.Bind("Match",()=>ConnectPvp("match",""));view.Bind("Create",()=>ConnectPvp("create",""));
        var code=view.Get<InputField>("Code");view.Bind("Join",()=>ConnectPvp("join",code.text));
        view.Bind("Ready",()=>network.Send(new Match3DMessage{type="ready"}));view.Bind("Leave",ShowPvp);
    }
    void ConnectPvp(string action,string room){lobbyStatus="正在连接…";network.Connect(endpoint,action,room);Notice(lobbyStatus);}
    void Receive(Match3DMessage m){
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
                var obj=LevelGenerator.Instance.objectsOnScene.FirstOrDefault(x=>x&&x.GetComponent<Match3DTile>().id==m.tileId);
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
    public void ShowHomeFeatureNotice(string message){Notice(message);}
    public void Notice(string value){ShowTips(value);}
    public void ShowTips(string message){
        if(string.IsNullOrWhiteSpace(message)||!canvas)return;
        if(!tips)tips=Match3DTips.Create(canvas.transform);
        tips.Show(message);
    }
    Match3DUIView OpenModal(string prefab){
        if(modal){modal.gameObject.SetActive(false);Destroy(modal.gameObject);}
        var view=Match3DUIView.Load(prefab,page);modal=(RectTransform)view.transform;Match3DModalMotion.Play(view.gameObject);return view;
    }
    void CloseModal(bool resume=false){
        if(modal){modal.gameObject.SetActive(false);Destroy(modal.gameObject);modal=null;}
        BlocksGameplay=false;if(resume)Time.timeScale=1;
    }
}
