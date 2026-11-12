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
    static IEnumerator Core() {
        Check(Match3DFlow.Instance && Match3DFlow.Instance.StartupComplete, "real scene finishes startup");
        Physics.autoSimulation = false; Start(); yield return null;
        var owner = ComboProbeController.Instance; var view = owner.View;
        Check(view && !view.Ring.gameObject.activeSelf, "original panel loaded, ring inactive before first triple");
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
        Triple(801); Check(owner.State.Combo == 2 && owner.State.Stars == 3 && owner.State.Count == 1, "production three-click continuation grants one bound model");
        Check(view.Ring.gameObject.activeSelf && view.Ring.AnimationState.GetCurrent(0).Animation.Name == "Purple circle", "actual authored purple Spine animation runs at two combo");
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
        slot.OnPointerDown(press); slot.OnPointerUp(press);
        Check(InputController.Instance.AcceptedPicks == accepts + 1 && !owner.State.Contains(top), "short press accepts exactly one physical target; accepted=" + InputController.Instance.AcceptedPicks + " before=" + accepts + " interact=" + owner.CanInteract);
        Check(!owner.UseTop(1, top), "stale click cannot consume next model");
        slot.OnPointerDown(press); yield return new WaitForSecondsRealtime(.3f);
        Time.timeScale = 0; yield return null;
        Check(!view.Preview.Active, "pause immediately clears preview"); Time.timeScale = 1;
        yield return new WaitForSecondsRealtime(1.2f);
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
            Capture("showcase-combo-"+(i+1),720,1565);
        }
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
        var live=LevelGenerator.Instance.objectsOnScene.First(x=>x&&x.activeInHierarchy);
        Pick(live);Check(input.ReviveSourceTray(),"start return sequence before transition cancellation");
        flow.Home();Start();yield return new WaitForSecondsRealtime(1.8f);
        Check(!input.ReturningToBowl&&!BoardTools.Shuffling&&owner.State.Count==0&&input.objectsOnStack.Count==0,"restart cancels return and prevents delayed revival shuffle in the new stage");
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
            Check(BoardTools.StoredCount==0&&!BoardToolVisual.HasBackground,"temporary region is absent: "+name);
            Check(Mathf.Abs(camera.orthographicSize-Match3DThemePresentation.Size(camera.aspect))<.0001f,"original camera scale preserved: "+name);
            float trayTop=0;
            foreach(var art in Match3DTray.Instance.GetComponentsInChildren<Image>()){
                if(!art.sprite||art.color.a==0)continue;var corners=new Vector3[4];art.rectTransform.GetWorldCorners(corners);
                foreach(var corner in corners)trayTop=Mathf.Max(trayTop,camera.WorldToViewportPoint(corner).y*GameplayBoardLayout.Height(camera)-GameplayBoardLayout.SafeBottom);
            }
            Check(GameplayBoardLayout.ProbeRect.yMin>trayTop+3,"probe region remains above the main tray: "+name+" top="+trayTop);
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
