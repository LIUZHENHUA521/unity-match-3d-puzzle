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
    Canvas canvas;RectTransform page,modal;Text timer,remaining,peerText,toast;
    ReplicaPilePhysics pilePhysics;
    ReplicaLayeredPile layeredPile;
    ReplicaThemePresentation sourceTutorialPresentation;
    GameObject sourceTutorialBoundary;
    ReplicaDiagnostics diagnostics;
    ReplicaGameplayHUD gameplayHUD;
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
        var uiRoot=ReplicaUIView.Load("UIRoot",null);
        canvas=uiRoot.GetComponent<Canvas>();canvas.name="Goose UI";
        page=uiRoot.Get<RectTransform>("Page");
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
            if(timer&&t!=displayedTime){timer.text=GameManager.Instance.currentLevelTime>0?(t/60).ToString("00")+":"+(t%60).ToString("00"):(gameplayHUD?"∞":"");displayedTime=t;}
            if(remaining&&count!=displayedCount)remaining.text="剩余 "+count+" / "+InitialCount;
            if(peerText&&(count!=displayedCount||otherCleared!=displayedPeer))peerText.text="我方 "+(InitialCount-count)+" / "+InitialCount+"    对手 "+otherCleared+" / "+InitialCount;
            displayedCount=count;displayedPeer=otherCleared;
        }
    }

    void Clear(){
        displayedTime=displayedCount=displayedPeer=-1;
        foreach(Transform t in page){t.gameObject.SetActive(false);Destroy(t.gameObject);}modal=null;timer=null;remaining=null;peerText=null;toast=null;BlocksGameplay=false;
    }
    void Music(AudioClip clip){if(music.clip!=clip){music.clip=clip;if(clip)music.Play();}music.mute=!SettingsMusic;}
    void Click(){if(SettingsEffects&&library.clickSound)effects.PlayOneShot(library.clickSound);}
    public void SelectSound(){if(SettingsEffects&&library.selectSound)effects.PlayOneShot(library.selectSound);}
    public void MatchSound(){if(SettingsEffects&&library.matchSound)effects.PlayOneShot(library.matchSound);}
    void StopBoard(){if(Camera.main){var highlight=Camera.main.GetComponent<ReplicaSourceHighlight>();if(highlight)highlight.ClearAll();}ReplicaSourceAudio.ResetLevel();CancelSourceCreation();if(sourceTutorialPresentation)sourceTutorialPresentation.End();if(sourceTutorialBoundary){sourceTutorialBoundary.SetActive(false);Destroy(sourceTutorialBoundary);sourceTutorialBoundary=null;}ReplicaSourcePhysics.End();if(InputController.Instance)InputController.Instance.CancelSelection();if(layeredPile)layeredPile.Clear();if(pilePhysics)pilePhysics.Clear();if(diagnostics)diagnostics.ResetProbe();Time.timeScale=1;StopAllCoroutines();GameManager.Instance.StopAllCoroutines();GameManager.Instance.gameStatus=GameManager.GameStatus.PREPARING;LevelGenerator.Instance.ClearBoard();ReplicaSourceObjectPool.Clear();resultShown=false;pendingPick=false;}
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
        BlocksGameplay=true;var view=OpenModal("Reward");
        view.Text("Amounts","本科局：毕业可领 "+ReplicaHomeView.BachelorReward+" 元奖励\n\n清北局：毕业可领 "+ReplicaHomeView.EliteReward+" 元奖励");
        view.Bind("Close",()=>CloseModal());
    }
    public void ShowThemes(){
        Clear();ScreenName="themes";var view=ReplicaUIView.Load("Themes",page);view.Bind("Back",Home);
        for(int i=0;i<library.themes.Length;i++){
            int index=i;var theme=library.themes[i];
            view.Text("Name"+i,theme.title);view.Text("Description"+i,theme.subtitle);
            view.Get<Image>("Goose"+i).sprite=Resources.Load<Sprite>("GooseArt/Sprites/"+theme.gooseSprite);
            view.Bind("Play"+i,()=>Begin(index));
        }
    }
    public bool HasGoose(string id){return PlayerPrefs.GetInt("goose.collection."+id,0)>0;}
    public void ShowCollection(){
        Clear();ScreenName="collection";var view=ReplicaUIView.Load("Collection",page);view.Bind("Back",Home);
        view.Text("Count","已收集 "+library.themes.Count(t=>HasGoose(t.id))+" / "+library.themes.Length);
        for(int i=0;i<library.themes.Length;i++){
            int index=i;var theme=library.themes[i];bool got=HasGoose(theme.id);
            var art=view.Get<Image>("Goose"+i);art.sprite=Resources.Load<Sprite>("GooseArt/Sprites/"+theme.gooseSprite);
            art.color=got?Color.white:new Color(.28f,.28f,.28f,.8f);
            view.Text("Name"+i,theme.title);view.Text("Status"+i,got?"已获得 · 永久收藏":"通关主题第二关解锁");
            view.Text("Play"+i+"Text",got?"再挑战":"去解锁");view.Bind("Play"+i,()=>Begin(index));
        }
    }
    public void Begin(int theme){
        // All solo entry points start directly on the full layered challenge.
        if(network)network.Disconnect();StopBoard();IsPvp=false;ThemeIndex=theme;Stage=2;runDate=Today;StartStage();
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
        if(layered){var goose=obj.GetComponent<ReplicaSourceGooseAnimation>();goose?.InitializeSource();goose?.ReturnToPile();layeredPile.Register(body,initialHeight:position.y);}
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
            // Source yields once per batch. WaitForSeconds already resumes on
            // a later frame; an extra null yield adds ~100 frames to 300 items.
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
        var view=ReplicaUIView.Load(IsPvp?"PvpGameHUD":"GameHUD",page);
        gameplayHUD=view.GetComponent<ReplicaGameplayHUD>();
        if(gameplayHUD)view.Bind("Settings",()=>ShowSettings(true));
        view.Text("Title",IsPvp?"好友对战":CurrentTheme.title+" · 第"+Stage+"关");
        view.Bind("Pause",()=>ShowSettings(true));timer=view.Get<Text>("Timer");remaining=view.Get<Text>("Remaining");
        view.Show("Peer",IsPvp);peerText=IsPvp?view.Get<Text>("Peer"):null;
        view.Text("UndoText",!IsPvp&&Stage==2?"移出":"撤回");moveToolButton=view.Bind("Undo",Undo);
        view.Text("ShuffleText",!IsPvp&&Stage==2?"打乱":"颠锅");
        shuffleToolButton=view.Bind("Shuffle",()=>{if(ReplicaSourceTools.Enabled)ReplicaSourceTools.MessUp();else FindObjectOfType<FoodPileShake>().Shake();});
        view.Text("HintText",IsPvp?"认输":Stage==2?"凑齐":"提示");
        findToolButton=view.Bind("Hint",()=>{if(IsPvp)network.Send(new ReplicaMessage{type="surrender"});else BoosterManager.Instance.HintButton();});
        view.Show("Freeze",!gameplayHUD&&!IsPvp&&Stage!=2);view.Bind("Freeze",()=>BoosterManager.Instance.Freeze());
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
        var view=OpenModal("Result");view.Text("Title",title);view.Text("Detail",detail);
        view.Get<Image>("Goose").sprite=Resources.Load<Sprite>("GooseArt/Sprites/"+CurrentTheme.gooseSprite);
        view.Text("ContinueText",IsPvp?"再次匹配":win&&Stage==1?"挑战第二关":win?"查看图鉴":"再试一次");
        view.Bind("Continue",()=>{if(IsPvp){StopBoard();ShowPvp();}else if(win&&Stage==1)NextStage();else if(win){StopBoard();ShowCollection();}else RestartStage();});
        view.Bind("Home",Home);
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
        Time.timeScale=0;BlocksGameplay=true;var view=OpenModal("ModelSize");
        view.Bind("Close",()=>CloseModal(true));view.Text("Current","当前这局："+AppliedModelSize.ToString("F2")+"×");
        var slider=view.Get<Slider>("Slider");slider.SetValueWithoutNotify(Mathf.RoundToInt(AppliedModelSize*100));
        Action refresh=()=>view.Text("Readout","新大小："+(slider.value/100f).ToString("F2")+"×");
        slider.onValueChanged.AddListener(_=>refresh());refresh();
        view.Bind("Minus",()=>slider.value--);view.Bind("Plus",()=>slider.value++);
        view.Bind("Apply",()=>{ReplicaSourceProfile.ModelSizeMultiplier=slider.value/100f;Time.timeScale=1;RestartStage();});
        view.Bind("Reset",()=>slider.value=100);
    }
    public void ShowSettings(bool ingame){
        if(ingame&&!IsPvp)Time.timeScale=0;BlocksGameplay=true;
        var view=OpenModal("PauseSettings");view.Text("Title",ingame?"暂停 / 设置":"设置");
        Action close=()=>CloseModal(ingame&&!IsPvp);
        view.Bind("Close",close);view.Bind("Continue",close);view.Text("ContinueText",ingame?"继续游戏":"完成");
        SettingToggle(view,"Music",SettingsMusic,value=>{PlayerPrefs.SetInt("goose.music",value?1:0);music.mute=!value;PlayerPrefs.Save();});
        SettingToggle(view,"Effects",SettingsEffects,value=>{PlayerPrefs.SetInt("goose.effects",value?1:0);PlayerPrefs.Save();});
        SettingToggle(view,"Vibration",SettingsVibration,value=>{PlayerPrefs.SetInt("goose.vibration",value?1:0);PlayerPrefs.Save();});
        view.Show("Home",ingame);view.Bind("Home",Home);
        var skin=view.GetComponent<ReplicaSettingsSkin>();
        if(skin){view.Show("Continue",ingame);skin.BindSupport();}
    }
    void SettingToggle(ReplicaUIView view,string key,bool value,Action<bool> change){
        bool current=value;var skin=view.GetComponent<ReplicaSettingsSkin>();
        void Refresh(){view.Text(key+"Text",current?"已开启":"已关闭");if(skin)skin.SetState(key,current);}
        Refresh();view.Bind(key,()=>{current=!current;change(current);Refresh();});
    }
    public void ShowPvp(){
        if(network)network.Disconnect();StopBoard();IsPvp=true;ThemeIndex=0;Stage=2;roomId="";lobbyStatus="邀请朋友来一场三消对决";DrawLobby();Music(library.pvpMusic);
    }
    void DrawLobby(){
        Clear();ScreenName="pvp";var view=ReplicaUIView.Load("PvpLobby",page);view.Bind("Back",Home);
        view.Text("Status",lobbyStatus);view.Show("Room",!string.IsNullOrEmpty(roomId));view.Text("Room","房间号："+roomId);
        var address=view.Get<InputField>("Address");address.SetTextWithoutNotify(endpoint);
        address.onEndEdit.AddListener(v=>{endpoint=v.Trim();PlayerPrefs.SetString("goose.endpoint",endpoint);});
        bool connected=network.Connected;
        foreach(var key in new[]{"Match","Create","Code","Join"})view.Show(key,!connected);
        view.Show("Ready",connected);view.Show("Leave",connected);
        view.Bind("Match",()=>ConnectPvp("match",""));view.Bind("Create",()=>ConnectPvp("create",""));
        var code=view.Get<InputField>("Code");view.Bind("Join",()=>ConnectPvp("join",code.text));
        view.Bind("Ready",()=>network.Send(new ReplicaMessage{type="ready"}));view.Bind("Leave",ShowPvp);
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
    public void ShowHomeFeatureNotice(string message){Notice(message);}
    void Notice(string value){
        if(!toast)toast=ReplicaUIView.Load("Notice",page).Get<Text>("Message");toast.text=value;
        if(modal)modal.SetAsLastSibling();
    }
    ReplicaUIView OpenModal(string prefab){
        if(modal){modal.gameObject.SetActive(false);Destroy(modal.gameObject);}
        var view=ReplicaUIView.Load(prefab,page);modal=(RectTransform)view.transform;return view;
    }
    void CloseModal(bool resume=false){
        if(modal){modal.gameObject.SetActive(false);Destroy(modal.gameObject);modal=null;}
        BlocksGameplay=false;if(resume)Time.timeScale=1;
    }
}
