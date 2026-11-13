using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

// Production play-mode integration, with frozen physics only where a precise occlusion fixture is required.
[InitializeOnLoad]
public static class ComboProbeValidation {
    const string Key = "combo-probe-validation";
    public const string Output = "work/combo-probe/return-to-bowl-results";
    static readonly BindingFlags Flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
    static int serial;
    static ComboProbeValidation() {
        EditorApplication.playModeStateChanged += state => {
            if (state == PlayModeStateChange.EnteredPlayMode && SessionState.GetBool(Key, false))
                EditorApplication.delayCall += () => new GameObject("Combo probe validation").AddComponent<BoardValidationDriver>().StartCoroutine(Guard());
        };
    }
    public static void Run() {
        Directory.CreateDirectory(Output); File.WriteAllText(Output + "/checks.txt", "");
        ComboProbeAssetBuilder.Build(); PureState(); BoardToolsValidation.Run();
        Check(File.ReadAllText("Temp/goose-source-tools-validation.txt").StartsWith("PASS"), "existing remove/magnet/shuffle selection regressions");
        SessionState.SetBool(Key, true); EditorSceneManager.OpenScene(Match3DBuilder.Scene); EditorApplication.isPlaying = true;
    }
    static void Check(bool success, string message) {
        if (!success) throw new Exception(message);
        File.AppendAllText(Output + "/checks.txt", "PASS " + message + "\n");
    }
    static void ValidateArcPixels() {
        var root = new GameObject("Arc framebuffer validation", typeof(RectTransform), typeof(Canvas)); root.layer = 30;
        var camera = new GameObject("Arc validation camera").AddComponent<Camera>();
        var pixels = new Texture2D(256,256,TextureFormat.RGBA32,false);
        var prior = RenderTexture.active;
        try {
            root.transform.position = new Vector3(5000,5000,5000);
            camera.transform.position = root.transform.position - Vector3.forward * 10;
            camera.enabled = false; camera.orthographic = true; camera.orthographicSize = 128; camera.aspect = 1;
            camera.clearFlags = CameraClearFlags.SolidColor; camera.backgroundColor = Color.clear; camera.cullingMask = 1 << 30; camera.allowHDR = false;
            var canvas = root.GetComponent<Canvas>(); canvas.renderMode = RenderMode.WorldSpace; canvas.worldCamera = camera;
            var arc = new GameObject("Arc",typeof(RectTransform),typeof(ComboProbeArcGraphic)).GetComponent<ComboProbeArcGraphic>();
            arc.gameObject.layer = 30; arc.transform.SetParent(root.transform,false);
            arc.rectTransform.sizeDelta = new Vector2(154,128)*1.27f; arc.Initialize(ComboProbeAssets.Load().sunshine);
            Color32[][] withoutStencil = new Color32[5][];
            float[] fills = {0,.08f,.38f,.75f,0};
            foreach (int depth in new[] {0,24}) {
                var rt = new RenderTexture(256,256,depth,RenderTextureFormat.ARGB32); rt.Create();
                try {
                    camera.targetTexture = rt; int previousCount = 0;
                    for (int i=0;i<fills.Length;i++) {
                        arc.fillAmount = fills[i]; Canvas.ForceUpdateCanvases(); camera.Render(); RenderTexture.active = rt;
                        pixels.ReadPixels(new Rect(0,0,256,256),0,0); pixels.Apply(); var sample = pixels.GetPixels32();
                        File.WriteAllBytes(Output+"/arc-depth-"+depth+"-step-"+i+".png",pixels.EncodeToPNG());
                        int visible = sample.Count(p=>p.a>20);
                        Check(fills[i]==0 ? visible==0 : visible>previousCount,"actual arc pixels follow chance count on depth "+depth+" step "+i+" pixels="+visible+" vertices="+arc.canvasRenderer.GetMesh()?.vertexCount);
                        if(depth==0) withoutStencil[i]=sample;
                        else Check(sample.SequenceEqual(withoutStencil[i]),"arc pixels identical with and without stencil at step "+i);
                        previousCount=visible;
                    }
                } finally { camera.targetTexture=null; RenderTexture.active=prior; rt.Release(); Object.DestroyImmediate(rt); }
            }
        } finally { RenderTexture.active=prior; Object.DestroyImmediate(pixels); Object.DestroyImmediate(camera.gameObject); Object.DestroyImmediate(root); }
    }
    static void PureState() {
        var state = new ComboProbeState();
        Check(!state.Pick(1, ProbePickSource.Manual, false) && state.Stars == 0, "before first match no combo or stars");
        Check(!state.Pick(2, ProbePickSource.Manual, true) && state.Combo == 1 && state.Stars == 3, "first triple lights three stars without reward");
        state.Pick(3, ProbePickSource.Manual, false); state.Pick(4, ProbePickSource.Manual, false);
        Check(state.Stars == 1 && state.Pick(5, ProbePickSource.Manual, true) && state.Combo == 2 && state.Stars == 3, "last-star triple continues combo and rewards once");
        Check(!state.Pick(5, ProbePickSource.Manual, true) && state.Combo == 2, "duplicate operation cannot count or reward twice");
        state.Pick(6, ProbePickSource.Manual, false); int stars = state.Stars;
        Check(!state.Pick(7, ProbePickSource.Automatic, true) && state.Combo == 2 && state.Stars == stars, "automatic magnet does not change combo or stars");
        Check(!state.Pick(8, ProbePickSource.Probe, true) && state.Combo == 3 && state.Stars == 3, "probe triple continues without self-reward");
        state.Pick(9, ProbePickSource.Manual, false); state.Pick(10, ProbePickSource.Manual, false); state.Pick(11, ProbePickSource.Manual, false);
        Check(state.Combo == 0 && state.Stars == 0, "third non-match breaks combo");
        for (int i = 1; i <= 10; i++) Check(state.Add(i), "reward token " + i);
        Check(state.Column(0).SequenceEqual(new long[] { 1, 5, 9 }) && state.Column(1).SequenceEqual(new long[] { 2, 6, 10 }) && state.Top(2) == 7 && state.Top(3) == 8, "ten rewards rotate through four LIFO stacks");
        Check(!state.Add(10) && state.NextColumn == 2, "duplicate binding cannot advance next column");
        state.Remove(3); state.Remove(7); state.Remove(1); state.Add(11);
        Check(state.Top(2) == 11 && state.Column(0).SequenceEqual(new long[] { 5, 9 }), "middle removal preserves stack and round-robin pointer");
        state.Pick(12, ProbePickSource.Manual, true);
        Check(state.Combo == 1 && state.Count == 8, "restart combo retains earned rewards");
        state.Reset(); Check(state.Count == 0 && state.Combo == 0 && state.NextColumn == 0, "new stage resets all logical state");
    }
    static IEnumerator Guard() {
        float deadline = Time.realtimeSinceStartup + 60;
        while ((!Match3DFlow.Instance || !Match3DFlow.Instance.StartupComplete) && Time.realtimeSinceStartup < deadline) yield return null;
        string failure = null; bool automatic = Physics.autoSimulation;
        var baseline = new RenderTexture(720,1565,24); baseline.Create();
        var camera = Camera.main; var oldTarget = camera.targetTexture; var oldAspect = camera.aspect;
        camera.targetTexture = baseline; camera.aspect = 720f/1565;
        var rewardConfig=Match3DRewardAcquisition.Config;
        Match3DRewardAcquisition.Configure(new Match3DRewardAcquisition.Configuration{revive=new Match3DRewardAcquisition.Rules{freeUsesPerRound=4}});
        var steps = Core();
        while (true) {
            bool more = false; try { more = steps.MoveNext(); } catch (Exception e) { failure = e.ToString(); }
            if (!more || failure != null) break; yield return steps.Current;
        }
        (steps as IDisposable)?.Dispose();
        Match3DRewardAcquisition.Configure(rewardConfig);
        if (Match3DFlow.Instance) Match3DFlow.Instance.Home(); Physics.autoSimulation = automatic;
        camera.targetTexture = oldTarget; camera.aspect = oldAspect; baseline.Release(); Object.Destroy(baseline);
        File.AppendAllText(Output + "/checks.txt", failure == null ? "COMPLETE\n" : "FAIL " + failure + "\n");
        SessionState.SetBool(Key, false); if (failure != null) Debug.LogError(failure);
        EditorApplication.Exit(failure == null ? 0 : 1);
    }
    static void Start(int stage = 1) {
        var flow = Match3DFlow.Instance; flow.Home();
        string oldTheme = EditorPrefs.GetString(Match3DFlow.EditorThemePreference, "");
        EditorPrefs.SetString(Match3DFlow.EditorThemePreference, "playful-toys");
        try { flow.Begin(0); } finally { EditorPrefs.SetString(Match3DFlow.EditorThemePreference, oldTheme); }
        if (stage == 2) flow.NextStage(); BoardCreationTestSupport.FinishPending(flow);
        GameManager.Instance.gameStatus = GameManager.GameStatus.PLAY; GameManager.Instance.currentLevelTime = 600;
        typeof(Match3DFlow).GetProperty("LocalPreview").SetValue(flow, true);
    }
    static void ClearFixture() {
        foreach (var obj in LevelGenerator.Instance.objectsOnScene.ToArray()) obj.SetActive(false);
        InputController.Instance.ResetStack();
    }
    static GameObject Spawn(int type, Vector3 position, GameObject prefab = null) {
        if (!prefab) { var toys = Match3DModelThemes.Get("playful-toys", "CountrysideStove").prefabs; prefab = toys[type % toys.Length]; }
        var flow = Match3DFlow.Instance;
        var obj = (GameObject)typeof(Match3DFlow).GetMethod("CreateBoardItem", Flags).Invoke(flow,
            new object[] { new ChallengeTile { id = 10000 + ++serial, type = type }, prefab, position, Quaternion.identity, true, false, null });
        LayeredItemPile.Instance.Detach(obj.GetComponent<Rigidbody>());
        obj.GetComponent<Rigidbody>().isKinematic = true;
        return obj;
    }
    static void Pick(GameObject obj, ProbePickSource source = ProbePickSource.Manual) => InputController.Instance.AddObjectOnStack(obj, source: source);
    static void Triple(int type, ProbePickSource last = ProbePickSource.Manual) {
        for (int i = 0; i < 3; i++) Pick(Spawn(type, new Vector3(-2 + i, 4, 1)), i == 2 ? last : ProbePickSource.Manual);
    }
    static void ValidateProbeLiftPhysics(){
        var objects=new List<GameObject>();
        try{
            var origin=new Vector3(40,50,0);
            var selected=GameObject.CreatePrimitive(PrimitiveType.Sphere);objects.Add(selected);
            selected.transform.position=origin;selected.transform.localScale=Vector3.one*.5f;selected.GetComponent<Collider>().enabled=false;
            Rigidbody[] bodies=new Rigidbody[3];
            for(int i=0;i<3;i++){
                var go=GameObject.CreatePrimitive(PrimitiveType.Cube);objects.Add(go);
                go.transform.localScale=Vector3.one*.3f;
                go.transform.position=origin+(i==0?new Vector3(0,.43f,0):i==1?new Vector3(.25f,.43f,0):new Vector3(3,.43f,0));
                var body=go.AddComponent<Rigidbody>();body.useGravity=false;
                LevelGenerator.Instance.objectsOnScene.Add(go);ItemPhysicsController.Instance.Register(body);
                body.isKinematic=i==1;bodies[i]=body;
            }
            var frozen=bodies[1].position;var far=bodies[2].position;var near=bodies[0].position;
            Physics.SyncTransforms();ProbeLiftContact.Begin(selected);
            Check(ProbeLiftContact.ActiveCount==1,"probe creates temporary physical contact volume");
            for(int step=0;step<12;step++){
                foreach(var contact in Object.FindObjectsOfType<ProbeLiftContact>())if(contact.gameObject.activeSelf)contact.Step(.02f);
                Physics.Simulate(.02f);
                Check(bodies[0].velocity.magnitude<=ProbeLiftContact.MaxSpeed+.01f,"probe contact speed stays bounded "+step);
            }
            Check(bodies[0].position.y>near.y+.02f,"actual collision lifts nearby dynamic model");
            Check(bodies[1].isKinematic&&bodies[1].position==frozen,"probe does not unfreeze or move frozen model");
            Check(Vector3.Distance(bodies[2].position,far)<.001f,"probe does not push distant models");
            Check(ProbeLiftContact.ActiveCount==0,"contact volume is removed before tray flight");
            ProbeLiftContact.Begin(selected);ProbeLiftContact.Clear();
            Check(ProbeLiftContact.ActiveCount==0,"restart cancels emergence contacts immediately");
        }finally{
            ProbeLiftContact.Clear();
            foreach(var obj in objects)if(obj){
                var body=obj.GetComponent<Rigidbody>();if(body)ItemPhysicsController.Instance.RemoveForPick(body);
                LevelGenerator.Instance.objectsOnScene.Remove(obj);Object.DestroyImmediate(obj);
            }
        }
    }
    static IEnumerator Core() {
        Check(Match3DFlow.Instance && Match3DFlow.Instance.StartupComplete, "real scene finishes startup");
        ValidateArcPixels();
        Physics.autoSimulation = false; Start(); yield return null;
        ClearFixture();
        ValidateProbeLiftPhysics();
        var storageInput=InputController.Instance;
        float restingY=GameplayBoardLayout.ProbeY(Camera.main);
        var storageItems=Enumerable.Range(0,4).Select(i=>Spawn(960+i,new Vector3(i*.2f,4,0))).ToArray();
        foreach(var item in storageItems)Pick(item);
        int usesBeforeStore=BoardTools.UsedCount(0);
        Check(BoardTools.RunPaid(0,()=>BoosterManager.Instance.Undo()),"move button stores during incoming flights");
        Check(storageInput.objectsOnStack.SequenceEqual(new[]{storageItems[3]})&&BoardTools.StoredCount==3,"move keeps fourth tray item and stores first three");
        Check(BoardTools.UsedCount(0)==usesBeforeStore+1,"storage spends once");
        yield return new WaitForSecondsRealtime(.6f);
        Check(BoardToolVisual.HasBackground&&Mathf.Abs(GameplayBoardLayout.ProbeY(Camera.main)-restingY-GameplayBoardLayout.StoreClearance)<.01f,"storage raises complete combo region");
        Physics.SyncTransforms();
        foreach(var item in storageItems.Take(3)){
            var point=Camera.main.WorldToScreenPoint(item.transform.position);
            Check(storageInput.PickAt(point)==item,"stored item remains raycast selectable");
        }
        for(int i=0;i<3;i++){
            var item=storageItems[i];var renderers=item.GetComponentsInChildren<Renderer>().Where(r=>r.enabled&&(r is MeshRenderer||r is SkinnedMeshRenderer)).ToArray();
            var bounds=renderers[0].bounds;foreach(var renderer in renderers.Skip(1))bounds.Encapsulate(renderer.bounds);
            var center=Camera.main.WorldToScreenPoint(bounds.center);var anchor=Camera.main.WorldToScreenPoint(BoardTools.StorePosition(i,0,1));
            Check(Vector2.Distance(center,anchor)<1,"stored model visual center aligns with cell "+i);
            var background=GameObject.Find("Source StoreContentBG").transform;
            var localCenter=background.InverseTransformPoint(bounds.center);
            float[] artCenters={74.5f,173f,271f};
            Check(Mathf.Abs((localCenter.x+.5f)*349-artCenters[i])<1&&Mathf.Abs((.5f-localCenter.y)*156-73)<1,
                "model center matches recessed artwork cell in texture pixels "+i);
            Check(Quaternion.Angle(item.transform.rotation,BoardTools.StoreRotation(item))<.1f,"stored model uses configured pose without extra tilt "+i);
        }
        foreach(var size in new[]{new Vector2Int(720,1280),new Vector2Int(720,1565),new Vector2Int(768,1024)})Capture("temporary-store-"+size.x+"x"+size.y,size.x,size.y);
        Check(BoardTools.RunPaid(0,()=>BoosterManager.Instance.Undo())&&BoardTools.StoredCount==4&&storageInput.objectsOnStack.Count==0,"fewer than three moves remaining item into existing storage");
        Check(!BoardTools.RunPaid(0,()=>BoosterManager.Instance.Undo()),"empty tray does not spend move");
        yield return new WaitForSecondsRealtime(.6f);
        foreach(var item in storageItems)Pick(item);
        yield return new WaitForSecondsRealtime(.6f);
        Check(BoardTools.StoredCount==0&&!BoardToolVisual.HasBackground&&Mathf.Abs(GameplayBoardLayout.ProbeY(Camera.main)-restingY)<.01f,"taking all stored items hides storage and restores combo position");
        Check(storageInput.objectsOnStack.Count==4&&!storageInput.IsBusy,"stored items return to tray without loss or stuck flights");
        Check(BoardTools.RunPaid(0,()=>BoosterManager.Instance.Undo()),"storage can reopen");
        // Re-pick immediately, before the storage handoff sequence finishes.
        foreach(var item in storageItems.Take(3))Pick(item);
        yield return new WaitForSecondsRealtime(.6f);
        Check(BoardTools.StoredCount==0&&storageInput.objectsOnStack.Count==4&&!storageInput.IsBusy,"immediate storage re-pick cancels old flight and completes tray arrival");
        Check(BoardTools.RunPaid(0,()=>BoosterManager.Instance.Undo()),"storage reopens before reset");
        Start();yield return null;
        Check(BoardTools.StoredCount==0&&BoardToolVisual.StoreExpansion==0,"new round clears storage and panel offset");
        var owner = ComboProbeController.Instance; var view = owner.View;
        Check(view && !view.Ring.gameObject.activeSelf, "original panel loaded, ring inactive before first triple");
        Check(view.HintText == "先消一组，点亮三星", "new player sees first elimination instruction");
        Capture("first-stage-empty", 720, 1565);
        ClearFixture();
        // Deep objects share a type but have unique spawn tokens. A broad upper object physically occludes them.
        var hidden = new List<GameObject>();
        for (int i = 0; i < 16; i++) hidden.Add(Spawn(900, new Vector3((i % 4 - 1.5f) * .12f, 1 + i * .015f, (i / 4 - 1.5f) * .12f)));
        var blocker = Spawn(901, new Vector3(0, 5, 0)); blocker.transform.localScale *= 4;
        ValidateThumbnailPose(hidden[0].GetComponent<ComboProbeTarget>());
        Physics.SyncTransforms();
        Check(owner.IsOccluded(hidden[0]), "actual 3D mesh occlusion identifies hidden target");
        Triple(800); Check(owner.State.Combo == 1 && owner.State.Stars == 3 && owner.State.Count == 0, "production first triple lights stars");
        Check(view.HintText == "3次取物内再消除，获得探物", "first match teaches remaining picks and reward");
        Capture("hint-earn-probe", 720, 1565);
        Pick(Spawn(801, new Vector3(-2, 4, 1)));
        Check(view.HintText == "2次取物内再消除，获得探物", "reward instruction follows actual remaining chances");
        Pick(Spawn(801, new Vector3(-1, 4, 1)));
        Check(view.HintText == "最后1次取物，消除可续连", "last chance overrides reward instruction");
        Pick(Spawn(801, new Vector3(0, 4, 1))); Check(owner.State.Combo == 2 && owner.State.Stars == 3 && owner.State.Count == 1, "production three-click continuation grants one bound model");
        Check(view.Ring.gameObject.activeSelf && view.Ring.AnimationState.GetCurrent(0).Animation.Name == "Purple circle", "actual authored purple Spine animation runs at two combo");
        Check(view.HintText == "点击模型，取出被挡物品", "first reward explains hidden target retrieval");
        long firstReward = owner.State.Top(0);
        Check(view.Rewards.IsPending(firstReward) && !owner.UseTop(0,firstReward), "reward is bound immediately but cannot be consumed before its star lands");
        yield return new WaitForSeconds(.2f); Capture("reward-star-flying",720,1565);
        Time.timeScale=0; float frozenReveal=view.Rewards.RevealScale(firstReward);
        yield return new WaitForSecondsRealtime(.18f);
        Check(view.Rewards.IsPending(firstReward) && view.Rewards.RevealScale(firstReward)==frozenReveal,"pause freezes reward flight and cannot reveal a model");
        Time.timeScale=1; yield return new WaitForSeconds(.29f);
        Check(!view.Rewards.IsPending(firstReward) && view.Rewards.RevealScale(firstReward)<1.16f,"landing reveals the reserved model with a bounded pop");
        Capture("reward-model-appearing",720,1565);
        for (int i = 1; i < 10; i++) Check(owner.GrantReward(), "physical occlusion reward " + (i + 1));
        view.Refresh();
        Check(owner.State.Column(0).Count == 3 && owner.State.Column(1).Count == 3 && owner.State.Column(2).Count == 2 && owner.State.Column(3).Count == 2, "production reward distribution is 3/3/2/2");
        long bottom = owner.State.Column(0)[0], sibling = owner.State.Column(0)[1];
        var original = owner.Target(bottom); var other = owner.Target(sibling);
        Check(original && other && original.gameObject.name == other.gameObject.name, "same-type targets have separate instance bindings");
        int before = owner.State.Count; InputController.Instance.AddObjectOnStack(original.gameObject, magnetic: true);
        Check(!owner.State.Contains(bottom) && owner.State.Contains(sibling) && owner.State.Count == before - 1, "ordinary/automatic source pickup removes only its lower-layer mapping");
        int combo = owner.State.Combo, stars = owner.State.Stars;
        Check(combo == 2 && stars == 3, "automatic pickup preserved combo state");
        yield return new WaitForSeconds(1.8f);
        Check(view.Rewards.ActiveCount==0 && !owner.State.Contains(bottom),"rapid stacked rewards finish without restoring a consumed binding");
        long top = owner.State.Top(1); var target = owner.Target(top);
        int accepts = InputController.Instance.AcceptedPicks;
        var slot = view.Slot(1); var screen = Camera.main.WorldToScreenPoint(slot.transform.position);
        var press = new PointerEventData(EventSystem.current) { position = screen, button = PointerEventData.InputButton.Left, pointerId = 7,
            pointerPressRaycast = new RaycastResult { module = view.Root.GetComponent<GraphicRaycaster>() } };
        slot.OnPointerDown(press); yield return new WaitForSecondsRealtime(.3f);
        Check(view.Preview.Active && view.Preview.TargetToken == top && view.Preview.ParticleCount > 20, "real long-press draws particle connection to bound target");
        Capture("long-press-particles", 720, 1565);
        slot.OnPointerUp(press);
        Check(!view.Preview.Active && InputController.Instance.AcceptedPicks == accepts && owner.State.Contains(top), "long-press release never consumes model or stars");
        slot.OnPointerDown(press); yield return new WaitForSecondsRealtime(.3f);
        var drag=new PointerEventData(EventSystem.current){position=screen+Vector3.right*100, pointerId=7};
        slot.OnDrag(drag); slot.OnPointerUp(press);
        Check(!view.Preview.Active && InputController.Instance.AcceptedPicks==accepts,"drag cancels long-press without pickup");
        Check(!owner.HasUsedProbe && view.HintText == "点击模型，取出被挡物品", "long press and cancelled drag do not complete first-use teaching");
        slot.OnPointerDown(press); slot.OnPointerUp(press);
        Check(InputController.Instance.AcceptedPicks == accepts + 1 && !owner.State.Contains(top), "short press accepts exactly one physical target; accepted=" + InputController.Instance.AcceptedPicks + " before=" + accepts + " interact=" + owner.CanInteract);
        ValidateProbeCollisionIsolation(target.gameObject);
        Check(ProbeLiftContact.ActiveCount==0&&PotProbeActivation.ActiveCount==1,"probe starts transmission before emergence contact");
        var activationStart=target.transform.position;
        var activationScale=target.transform.localScale;
        yield return new WaitForSeconds(.18f);
        Check(Vector3.Distance(target.transform.position,activationStart)<.0001f&&ProbeLiftContact.ActiveCount==0,"light travels while linked object stays in pile");
        Capture("pot-probe-transmission",720,1565);
        var effect=Object.FindObjectOfType<PotProbeActivation>();float pausedTime=effect.Elapsed;
        Time.timeScale=0;yield return new WaitForSecondsRealtime(.15f);
        Check(effect&&Mathf.Abs(effect.Elapsed-pausedTime)<.0001f&&Vector3.Distance(target.transform.position,activationStart)<.0001f,"pause freezes transmission and target");
        Time.timeScale=1;
        yield return new WaitForSeconds(.24f);
        Check(Vector3.Distance(target.transform.position,activationStart)<.0001f&&target.transform.localScale.magnitude>activationScale.magnitude*1.02f,"arrival activates target with scale pulse before movement");
        Capture("pot-probe-activation",720,1565);
        yield return new WaitForSeconds(.18f);
        Check(ProbeLiftContact.ActiveCount==1&&target.transform.position.y>activationStart.y,"target emerges only after activation completes");
        var pausedLiftPosition=target.transform.position;
        Time.timeScale=0;yield return new WaitForSecondsRealtime(.15f);
        Check(Vector3.Distance(target.transform.position,pausedLiftPosition)<.0001f&&ProbeLiftContact.ActiveCount==1,"pause freezes both emergence animation and contact lifetime");
        Time.timeScale=1;
        Capture("pot-probe-emergence",720,1565);

        Check(owner.HasUsedProbe && view.HintText == "点击取物 · 长按定位", "successful use switches to regular interaction hint immediately");
        Check(!owner.UseTop(1, top), "stale click cannot consume next model");
        slot.OnPointerDown(press); yield return new WaitForSecondsRealtime(.3f);
        Time.timeScale = 0; yield return null;
        Check(!view.Preview.Active, "pause immediately clears preview"); Time.timeScale = 1;
        yield return new WaitForSecondsRealtime(1.2f);
        Check(ProbeLiftContact.ActiveCount==0&&PotProbeActivation.ActiveCount==0,"real probe flight removes contact and activation visual before arrival");
        // A third same-name physical target completes a triple using a probe: no reward loop.
        int countBefore = owner.State.Count;
        var matchingColumn = Enumerable.Range(0, 4).First(c => owner.State.Top(c) != 0);
        Check(owner.UseTop(matchingColumn, owner.State.Top(matchingColumn)), "third same-type probe pickup is accepted");
        Check(owner.State.Count == countBefore - 1 && owner.State.Combo == 3 && owner.State.Stars == 3, "probe match refills stars without granting a new model");
        yield return new WaitForSecondsRealtime(1.2f);
        var oldToken = original.Token;
        var recycled = Spawn(900, new Vector3(1, 1, 0));
        Check(recycled.GetComponent<ComboProbeTarget>().Token != oldToken && !owner.Target(oldToken), "pool checkout token cannot revive a consumed binding");
        // The move tool returns every tray item, including flights, without a shuffle or storage.
        var input=InputController.Instance; var flow=Match3DFlow.Instance;
        var returning=new List<GameObject>();var boardScales=new List<Vector3>();var rotations=new List<Quaternion>();
        for(int i=0;i<6;i++){
            var item=Spawn(820+i,new Vector3(i*.1f,4,0));returning.Add(item);boardScales.Add(item.transform.localScale);rotations.Add(item.transform.rotation);Pick(item);
        }
        int earned=owner.State.Count;int remainingStars=owner.State.Stars;int toolUses=BoardTools.UsedCount(0);
        Check(BoardTools.RunPaid(0,()=>input.ReturnSourceTrayToBowl()),"move tool accepts all six tray items while pickup flights are still running");
        Check(input.objectsOnStack.Count==0&&input.ReturningToBowl&&BoardTools.UsedCount(0)==toolUses+1,"move clears the whole logical tray and spends once");
        Check(!input.ReturnSourceTrayToBowl()&&!input.ReviveSourceTray()&&!BoardTools.MessUp(),"return animation rejects duplicate move, revival and shuffle");
        accepts=input.AcceptedPicks;Pick(returning[0]);Check(input.AcceptedPicks==accepts,"return animation blocks picking moving items");
        yield return new WaitForSecondsRealtime(.1f);
        Time.timeScale=0;var pausedPosition=returning[0].transform.position;
        yield return new WaitForSecondsRealtime(.15f);
        Check(Vector3.Distance(pausedPosition,returning[0].transform.position)<.0001f&&input.ReturningToBowl,"pause freezes return flight without releasing gameplay lock");Time.timeScale=1;
        yield return new WaitForSecondsRealtime(.6f);
        Check(!input.ReturningToBowl&&!BoardTools.Shuffling&&BoardTools.StoredCount==0&&!BoardToolVisual.HasBackground,"ordinary move finishes in the pot without storage or shuffle");
        for(int i=0;i<returning.Count;i++){
            var item=returning[i];Check(Vector3.Distance(item.transform.localScale,boardScales[i])<.001f&&Quaternion.Angle(item.transform.rotation,rotations[i])<.1f,"returned item restores full board scale and rotation "+i);
            Check(ItemPhysicsController.Instance.Contains(item.GetComponent<Rigidbody>())&&item.GetComponent<Rigidbody>().detectCollisions&&item.GetComponentsInChildren<Collider>().Any(c=>c.enabled),"returned item restores board physics and colliders "+i);
        }
        Check(owner.State.Count==earned&&owner.State.Stars==remainingStars,"move preserves earned probes and never refills stars");
        Pick(returning[0]);yield return new WaitForSecondsRealtime(.5f);Check(input.objectsOnStack.Contains(returning[0]),"returned item is collectible again");
        input.UndoLast();yield return new WaitForSecondsRealtime(.5f);
        Check(input.objectsOnStack.Count==0&&owner.State.Count==earned,"undo after return does not restore consumed probe bindings");
        for(int i=0;i<7;i++)Pick(Spawn(830+i,new Vector3(i*.1f,4,0)));
        Check(flow.RevivePending&&Time.timeScale==0,"seven different items open the actual revival offer");
        yield return new WaitForSecondsRealtime(.6f);Capture("revival-offer",720,1565);
        var popup=Object.FindObjectOfType<Match3DReviveView>();int shuffleUses=BoardTools.UsedCount(2);popup.revive.onClick.Invoke();
        Check(flow.ClearReviveUsed&&input.ReturningToBowl&&!BoardTools.Shuffling&&input.objectsOnStack.Count==0,"real revival returns all seven before starting shuffle");
        yield return new WaitForSecondsRealtime(.6f);
        Check(!input.ReturningToBowl&&BoardTools.Shuffling,"revival starts shuffle only after return completes");
        yield return new WaitForSecondsRealtime(1.3f);
        Check(!BoardTools.Busy&&BoardTools.UsedCount(0)==toolUses+1&&BoardTools.UsedCount(2)==shuffleUses&&owner.State.Count==earned,"revival finishes without spending move/shuffle inventory or losing probes");
        foreach(var size in new[]{new Vector2Int(720,1280),new Vector2Int(720,1565),new Vector2Int(720,1600),new Vector2Int(768,1024)})
            Capture("restored-scale-"+size.x+"x"+size.y,size.x,size.y);
        int generation = owner.Generation; Start(2); yield return null;
        Check(owner.Generation != generation && owner.State.Count == 0 && owner.State.Combo == 0 && BoardTools.StoredCount == 0, "stage transition clears probe stacks without a temporary region");
        var boardTransforms = LevelGenerator.Instance.objectsOnScene.Select(o => o.transform.position).ToArray();
        Capture("second-stage-empty", 720, 1565);
        Check(boardTransforms.SequenceEqual(LevelGenerator.Instance.objectsOnScene.Select(o => o.transform.position)), "camera layout never changes board world coordinates");
        GameManager.Instance.currentLevelTime=0;flow.HandleResult(false);
        Check(flow.RevivePending,"timeout opens actual revival offer even with empty tray");
        yield return new WaitForSecondsRealtime(.6f);Capture("time-revival-offer",720,1565);
        Object.FindObjectOfType<Match3DReviveView>().revive.onClick.Invoke();
        Check(flow.TimeReviveUsed&&GameManager.Instance.currentLevelTime==300&&input.ReturningToBowl,"time revival keeps five-minute benefit and begins return/shuffle sequence");
        yield return new WaitForSecondsRealtime(1.9f);Check(!BoardTools.Busy,"empty-tray time revival completes shuffle");
        var targetOnBoard = LevelGenerator.Instance.objectsOnScene[0]; var tokenOnBoard = targetOnBoard.GetComponent<ComboProbeTarget>().Token;
        owner.State.Add(tokenOnBoard); view.Refresh();
        Start(); yield return null;
        Check(owner.State.Count == 0 && !owner.Target(tokenOnBoard), "restart invalidates old stage identities");
        // No eligible targets: no reward and no pointer advance.
        ClearFixture(); int pointer = owner.State.NextColumn;
        Check(!owner.GrantReward() && owner.State.NextColumn == pointer, "empty candidate set does not advance rotation");
        Match3DFlow.Instance.Home();
        Check(!owner.Running && !view.gameObject.activeSelf && !view.Preview.Active, "home cleans up display, input and preview");
        // Capture the actual settled stage, separate from the deliberately oversized occlusion fixture.
        Physics.autoSimulation = true; Start(2); yield return new WaitForSeconds(3);
        for (int i = 0; i < 12; i++) owner.GrantReward();
        Check(owner.State.Count >= 4, "settled production pile supplies real occluded reward targets");
        var storeItems = LevelGenerator.Instance.objectsOnScene.Where(o=>o&&o.activeInHierarchy).GroupBy(o=>o.name).Select(g=>g.First()).Take(6).ToArray();
        foreach(var item in storeItems) Pick(item,ProbePickSource.Automatic);
        yield return new WaitForSeconds(1);
        Check(BoardTools.RunPaid(0,()=>input.ReturnSourceTrayToBowl()), "showcase uses real whole-tray return");
        yield return new WaitForSeconds(1);
        string[] names = { "Blue circle", "Purple circle", "Orange circle", "red circle", "colored circle" };
        for(int i=0;i<5;i++) {
            owner.State.Pick(10000+i,ProbePickSource.Manual,true); view.Refresh();
            yield return new WaitForSeconds(.55f);
            Check(view.Ring.AnimationState.GetCurrent(0).Animation.Name==names[i],"Spine animation color "+(i+1));
            Check(view.Chances.UsesColorStars==(i>=4),"lit stars match the ring rainbow tier "+(i+1));
            Capture("showcase-combo-"+(i+1),720,1565);
            if (i == 0) {
                // Same pose/frame: reproduce the former double-alpha dark halo without editing the asset.
                var correctMaterial = view.Ring.material;
                var doubleAlpha = new Material(correctMaterial);
                doubleAlpha.EnableKeyword("_STRAIGHT_ALPHA_INPUT"); doubleAlpha.SetFloat("_StraightAlphaInput", 1);
                try {
                    view.Ring.material = doubleAlpha;
                    Capture("ring-double-alpha-before",720,1565);
                } finally {
                    view.Ring.material = correctMaterial; Object.Destroy(doubleAlpha);
                }
            }
        }
        Check(owner.GrantReward(),"production pile supplies another rainbow-tier reward");
        yield return new WaitForSeconds(.22f); Capture("showcase-reward-flying",720,1565);
        yield return new WaitForSeconds(.27f); Capture("showcase-reward-reveal",720,1565);
        yield return new WaitForSeconds(.3f);
        int previewColumn = Enumerable.Range(0,4).First(c=>owner.State.Top(c)!=0);
        view.Preview.Show(owner,owner.State.Top(previewColumn),view.Slot(previewColumn).transform);
        yield return new WaitForSeconds(.2f); Capture("showcase-preview",720,1565); view.Preview.Clear();
        // Put fresh instances of each displayed top model into the REAL main tray,
        // preserving the original probe bindings for a side-by-side pose comparison.
        for (int column = 0; column < 4; column++) {
            var shown = owner.Target(owner.State.Top(column));
            if (shown) Pick(Spawn(1500 + column, new Vector3(0, 4, 0), shown.Prefab), ProbePickSource.Automatic);
        }
        yield return new WaitForSeconds(.7f); Capture("showcase-tray-comparison",720,1565);
        Check(Mathf.Abs(view.Chances.Progress - .75f) < .001f, "three chances fill the authored golden arc");
        owner.State.Pick(11000, ProbePickSource.Manual, false); view.Refresh();
        yield return new WaitForSeconds(.08f);
        Check(view.Chances.Progress > .38f && view.Chances.Progress < .75f && view.Chances.StarAlpha(2) > 0 && view.Chances.StarAlpha(2) < 1, "spending a chance animates the arc and extinguishes its star");
        Capture("chance-extinguishing",720,1565);
        float pausedProgress = view.Chances.Progress, pausedAlpha = view.Chances.StarAlpha(2);
        Time.timeScale = 0; yield return new WaitForSecondsRealtime(.12f);
        Check(view.Chances.Progress == pausedProgress && view.Chances.StarAlpha(2) == pausedAlpha, "pause freezes chance feedback");
        Time.timeScale = 1; yield return new WaitForSeconds(.24f);
        Check(Mathf.Abs(view.Chances.Progress - .38f) < .001f && view.Chances.StarAlpha(2) == 0 && view.Chances.StarAlpha(1) == 1, "two remaining stars settle at the top arc threshold");
        Capture("chance-two-stars",720,1565);
        owner.State.Pick(11001, ProbePickSource.Manual, false); view.Refresh(); yield return new WaitForSeconds(.24f);
        Check(Mathf.Abs(view.Chances.Progress - .08f) < .001f && view.Chances.StarAlpha(1) == 0 && view.Chances.StarAlpha(0) == 1, "one remaining chance keeps only the left star and arc segment");
        Check(view.HintText == "最后1次取物，消除可续连", "last chance warning takes priority over retained reward instructions");
        Capture("chance-one-star",720,1565);
        owner.State.Pick(11002, ProbePickSource.Manual, false); view.Refresh();
        Check(view.Ring.gameObject.activeSelf && view.Ring.color.a == 1, "broken combo starts with the breathing ring still visible");
        yield return new WaitForSeconds(.12f);
        Check(view.Ring.gameObject.activeSelf && view.Ring.color.a > 0 && view.Ring.color.a < 1, "broken combo fades the Spine ring instead of hiding it abruptly");
        yield return new WaitForSeconds(.24f);
        Check(view.Chances.Progress == 0 && view.Chances.StarAlpha(0) == 0 && !view.Ring.gameObject.activeSelf, "broken combo clears the arc and all lit stars");
        Check(!view.Chances.UsesColorStars,"broken combo restores normal star skin while grey stars remain visible");
        Check(owner.HasUsedProbe && view.HintText == "点击取物 · 长按定位", "learning survives stage changes and retained probes remain usable after combo breaks");
        Capture("chance-empty",720,1565);
        owner.State.Pick(11003, ProbePickSource.Manual, true); view.Refresh(); yield return new WaitForSeconds(.1f);
        Check(view.Chances.Progress > 0 && view.Chances.Progress < .75f && view.Chances.StarScale(0) > 1 && view.Chances.FlashAlpha(0) > 0, "lighting stars bounces and flashes while the arc refills");
        Capture("chance-lighting",720,1565);
        owner.State.Pick(11004, ProbePickSource.Manual, false); view.Refresh();
        owner.State.Pick(11005, ProbePickSource.Manual, true); view.Refresh();
        owner.State.Pick(11006, ProbePickSource.Manual, false); view.Refresh(); yield return new WaitForSeconds(.4f);
        Check(Mathf.Abs(view.Chances.Progress - .38f) < .001f && view.Chances.StarAlpha(2) == 0 && view.Chances.FlashAlpha(2) == 0 && Mathf.Abs(view.Chances.StarScale(0) - 1) < .001f, "rapid spend/refill/spend settles to the latest state without stale flashes");
        owner.State.Pick(11007, ProbePickSource.Manual, true); view.Refresh(); yield return new WaitForSeconds(.4f);
        Check(Mathf.Abs(view.Chances.Progress - .75f) < .001f && view.Chances.StarAlpha(2) == 1, "another elimination refills all three chances");
        Capture("chance-refilled",720,1565);
        view.ResetView();
        Check(view.Chances.Progress == 0 && Enumerable.Range(0,3).All(i => view.Chances.StarAlpha(i) == 0 && view.Chances.FlashAlpha(i) == 0), "reset immediately clears in-flight chance visuals");
        view.Refresh();
        var live=LevelGenerator.Instance.objectsOnScene.First(x=>x&&x.activeInHierarchy);
        Pick(live);Check(input.ReviveSourceTray(),"start return sequence before transition cancellation");
        flow.Home();Start();yield return new WaitForSecondsRealtime(1.8f);
        Check(!input.ReturningToBowl&&!BoardTools.Shuffling&&owner.State.Count==0&&input.objectsOnStack.Count==0,"restart cancels return and prevents delayed revival shuffle in the new stage");
        Physics.autoSimulation=false; ClearFixture();
        for(int i=0;i<4;i++)Spawn(1900,new Vector3(i*.1f,1,0));
        var rewardBlocker=Spawn(1901,new Vector3(0,5,0));rewardBlocker.transform.localScale*=4;Physics.SyncTransforms();
        int pendingColumn=owner.State.NextColumn; Check(owner.GrantReward(),"create reward for in-flight invalidation regression");
        long pendingToken=owner.State.Top(pendingColumn);
        Check(view.Rewards.IsPending(pendingToken),"invalidation fixture begins with a genuinely flying reward");
        Pick(owner.Target(pendingToken).gameObject,ProbePickSource.Automatic);yield return null;
        Check(!owner.State.Contains(pendingToken)&&!view.Rewards.IsPending(pendingToken)&&view.Rewards.ActiveCount==0,"collecting target during flight cancels the star without creating a stale model");
        Check(owner.GrantReward()&&view.Rewards.ActiveCount>0,"another star is in flight before restart");
        flow.Home();Start(2);yield return new WaitForSeconds(.8f);
        Check(view.Rewards.ActiveCount==0&&owner.State.Count==0,"restart removes in-flight stars and prevents delayed rewards in the new stage");
    }
    static void ValidateProbeCollisionIsolation(GameObject picked) {
        var body = picked.GetComponent<Rigidbody>();
        Check(body.isKinematic && !body.detectCollisions && picked.GetComponentsInChildren<Collider>(true).All(c=>!c.enabled),"probe disables body and all child collisions in the accepting frame before flight or scale");
        var originalScene=picked.scene;
        var isolated=SceneManager.CreateScene("Probe collision regression",new CreateSceneParameters(LocalPhysicsMode.Physics3D));
        GameObject neighbour=null;
        try {
            SceneManager.MoveGameObjectToScene(picked,isolated);
            neighbour=GameObject.CreatePrimitive(PrimitiveType.Cube); neighbour.layer=picked.layer;
            SceneManager.MoveGameObjectToScene(neighbour,isolated);
            // Force a large overlap in the extraction corridor. The unregistered neighbour
            // is genuinely dynamic: passing cannot be explained by globally freezing physics.
            neighbour.transform.position=picked.transform.position;
            neighbour.transform.localScale=Vector3.one*2;
            var other=neighbour.AddComponent<Rigidbody>(); other.useGravity=false;
            var before=other.position;
            var physics=isolated.GetPhysicsScene();
            for(int i=0;i<8;i++) physics.Simulate(1f/60);
            Check(!other.isKinematic && Vector3.Distance(other.position,before)<.00001f && other.velocity.sqrMagnitude<.000001f,"disabled display colliders cannot push an unregistered neighbour outside the contact volume");
        } finally {
            SceneManager.MoveGameObjectToScene(picked,originalScene);
            if(neighbour)Object.DestroyImmediate(neighbour);
            SceneManager.UnloadSceneAsync(isolated);
        }
    }
    static void ValidateThumbnailPose(ComboProbeTarget target) {
        var rotation = target.transform.rotation; var scale = target.transform.localScale;
        var first = new ComboProbeIcons(); var second = new ComboProbeIcons();
        try {
            var upright = ReadIcon(first.For(target));
            target.transform.rotation = Quaternion.Euler(43, 127, 81); target.transform.localScale *= 1.7f;
            var tilted = ReadIcon(second.For(target));
            Check(upright.Count(p => p.a > 100) > 100 && upright.SequenceEqual(tilted), "probe thumbnail is independent of random board rotation and scale");
        } finally { target.transform.rotation = rotation; target.transform.localScale = scale; first.Dispose(); second.Dispose(); }
    }
    static Color32[] ReadIcon(Texture texture) {
        var previous = RenderTexture.active; var rt = RenderTexture.GetTemporary(144, 144, 0, RenderTextureFormat.ARGB32);
        var copy = new Texture2D(144, 144, TextureFormat.RGBA32, false);
        try {
            Graphics.Blit(texture, rt); RenderTexture.active = rt;
            copy.ReadPixels(new Rect(0, 0, 144, 144), 0, 0); copy.Apply(); return copy.GetPixels32();
        } finally { RenderTexture.active = previous; RenderTexture.ReleaseTemporary(rt); Object.Destroy(copy); }
    }
    static void Capture(string name, int width, int height) {
        var camera = Camera.main; var previous = camera.targetTexture; var previousAspect = camera.aspect;
        var overlays = Object.FindObjectsOfType<Canvas>().Where(c => c.isRootCanvas && c.renderMode == RenderMode.ScreenSpaceOverlay).ToArray();
        var cameras = overlays.Select(c => c.worldCamera).ToArray(); var distances = overlays.Select(c => c.planeDistance).ToArray();
        var rt = new RenderTexture(width, height, 24); rt.Create(); var active = RenderTexture.active;
        var texture = new Texture2D(width, height, TextureFormat.RGB24, false);
        try {
            camera.targetTexture = rt; camera.aspect = width / (float)height;
            foreach (var c in overlays) { c.renderMode = RenderMode.ScreenSpaceCamera; c.worldCamera = camera; c.planeDistance = .5f; }
            foreach (var presentation in Object.FindObjectsOfType<Match3DThemePresentation>()) if (presentation.Active) presentation.Fit();
            Match3DTray.Instance.SendMessage("LateUpdate");
            foreach (var tools in Object.FindObjectsOfType<BoardToolVisual>()) tools.SendMessage("LateUpdate");
            // Captures switch aspect synchronously; settle stored models into the new projection.
            if(BoardTools.StoredCount>0){
                var columns=(List<GameObject>[])typeof(BoardTools).GetField("columns",Flags).GetValue(Object.FindObjectOfType<BoardTools>());
                for(int column=0;column<columns.Length;column++)for(int i=0;i<columns[column].Count;i++)
                    if(columns[column][i]){
                        var item=columns[column][i];var rotation=BoardTools.StoreRotation(item);
                        item.transform.position=BoardTools.StoreItemPosition(item,BoardTools.StorePosition(column,i,columns[column].Count),rotation,item.transform.localScale);
                        item.transform.rotation=rotation;
                    }
            }
            ComboProbeController.Instance.View.SendMessage("LateUpdate");
            Canvas.ForceUpdateCanvases(); foreach (var fit in Object.FindObjectsOfType<Match3DUIFitContent>()) fit.Fit(); Canvas.ForceUpdateCanvases();
            camera.Render(); RenderTexture.active = rt;
            texture.ReadPixels(new Rect(0, 0, width, height), 0, 0); texture.Apply(); File.WriteAllBytes(Output + "/" + name + ".png", texture.EncodeToPNG());
            // Store model positions are normally animated into their anchors. Measure physical projections too.
            var report = new List<string>();
            var ring = ComboProbeController.Instance.View.Ring;
            var mesh = ring.canvasRenderer.GetMesh();
            report.Add("ring active="+ring.gameObject.activeInHierarchy+" cull="+ring.canvasRenderer.cull+" vertices="+(mesh ? mesh.vertexCount : 0)+" bounds="+(mesh ? mesh.bounds.ToString() : "none")+" scale="+ring.transform.lossyScale+" texture="+ring.mainTexture+" rect="+ring.rectTransform.rect);
            if((mesh ? mesh.vertexCount : 0)>0)report.Add("ring vertex="+camera.WorldToScreenPoint(ring.transform.TransformPoint(mesh.vertices[0]))+" colors="+string.Join(",",mesh.colors32.Take(4)));
            Check(BoardToolVisual.HasBackground==(BoardTools.StoredCount>0),"temporary region follows occupancy: "+name);
            Check(Mathf.Abs(camera.orthographicSize-Match3DThemePresentation.Size(camera.aspect))<.0001f,"original camera scale preserved: "+name);
            float trayTop=0;
            foreach(var art in Match3DTray.Instance.GetComponentsInChildren<Image>()){
                if(!art.sprite||art.color.a==0)continue;var corners=new Vector3[4];art.rectTransform.GetWorldCorners(corners);
                foreach(var corner in corners)trayTop=Mathf.Max(trayTop,camera.WorldToViewportPoint(corner).y*GameplayBoardLayout.Height(camera)-GameplayBoardLayout.SafeBottom);
            }
            Check(GameplayBoardLayout.ProbeRect.yMin>trayTop+3,"probe region remains above the main tray: "+name+" top="+trayTop);
            var artBounds = new Bounds(); bool firstArt = true;
            foreach (var art in ComboProbeController.Instance.View.GetComponentsInChildren<Image>()) {
                if (art.name != "Blue four-slot panel" && art.name != "Chance dark 0") continue;
                var corners = new Vector3[4]; art.rectTransform.GetWorldCorners(corners);
                foreach(var corner in corners) { var point=camera.WorldToViewportPoint(corner); if(firstArt){artBounds=new Bounds(point,Vector3.zero);firstArt=false;}else artBounds.Encapsulate(point); }
            }
            Check(artBounds.size.x>=.95f && artBounds.min.x>=0 && artBounds.max.x<=1,"visible probe artwork covers at least 95 percent without horizontal clipping: "+name+" width="+artBounds.size.x);
            File.WriteAllLines(Output + "/" + name + "-layout.txt", report);
            var icon = ComboProbeController.Instance.View.GetComponentsInChildren<RawImage>().FirstOrDefault(i => i.texture);
            if (icon && icon.texture) {
                var iconRt = RenderTexture.GetTemporary(144,144,0); Graphics.Blit(icon.texture,iconRt);
                RenderTexture.active = iconRt; var iconImage = new Texture2D(144,144,TextureFormat.RGBA32,false);
                iconImage.ReadPixels(new Rect(0,0,144,144),0,0); iconImage.Apply();
                Check(iconImage.GetPixels32().Count(p=>p.a>100)>100, "cached model thumbnail contains visible geometry: "+name);
                File.WriteAllBytes(Output + "/model-thumbnail.png",iconImage.EncodeToPNG()); Object.Destroy(iconImage);
                RenderTexture.ReleaseTemporary(iconRt);
            }
        } finally {
            camera.targetTexture = previous; camera.aspect = previousAspect; RenderTexture.active = active;
            for (int i = 0; i < overlays.Length; i++) { overlays[i].renderMode = RenderMode.ScreenSpaceOverlay; overlays[i].worldCamera = cameras[i]; overlays[i].planeDistance = distances[i]; }
            foreach (var presentation in Object.FindObjectsOfType<Match3DThemePresentation>()) if (presentation.Active) presentation.Fit();
            Match3DTray.Instance.SendMessage("LateUpdate");
            ComboProbeController.Instance.View.SendMessage("LateUpdate");
            Object.Destroy(texture); rt.Release(); Object.Destroy(rt);
        }
    }
}
