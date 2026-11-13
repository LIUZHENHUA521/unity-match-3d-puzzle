using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

// Run only in a disposable project. No gameplay startup or network services are invoked.
public static class BoardCameraZoomValidation {
    static int checks;
    static void Check(bool ok, string why) { if (!ok) throw new Exception(why); checks++; Debug.Log("ZOOM PASS " + why); }
    static void Set(object target, string name, object value) => target.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(target, value);
    static Touch Finger(int id, Vector2 point, TouchPhase phase = TouchPhase.Moved) => new Touch { fingerId = id, position = point, phase = phase };
    static void Capture(Camera camera, BoardCameraZoom zoom, GameObject oldControls) {
        UnityEngine.Object.DestroyImmediate(oldControls);
        zoom.ResetView(); zoom.ProcessContacts(0, default, default, p => false);
        camera.transform.SetPositionAndRotation(Match3DThemePresentation.SourcePosition, Match3DThemePresentation.SourceRotation);
        camera.orthographicSize = Match3DThemePresentation.Size(9f/16); camera.aspect = 9f/16;
        camera.clearFlags = CameraClearFlags.SolidColor; camera.backgroundColor = new Color(.65f,.85f,1);
        UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Match3D/Scenes/CountrysideStove"));
        new GameObject("Lighting fixture").AddComponent<BoardLighting>().Begin();
        var root = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Match3D/UI/UIRoot"));
        var canvas = root.GetComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceCamera; canvas.worldCamera = camera; canvas.planeDistance = 1;
        var page = root.GetComponent<Match3DUIView>().Get<RectTransform>("Page");
        var hud = Match3DUIView.Load("GameHUD", page).GetComponent<Match3DGameplayHUD>();
        typeof(Match3DGameplayHUD).GetMethod("Awake", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(hud, null);
        hud.GetComponent<BoardCameraZoomControls>().SendMessage("LateUpdate");
        var rt = new RenderTexture(720,1280,24); camera.targetTexture = rt; rt.Create();
        Canvas.ForceUpdateCanvases(); camera.Render(); RenderTexture.active = rt;
        var image = new Texture2D(720,1280,TextureFormat.RGB24,false); image.ReadPixels(new Rect(0,0,720,1280),0,0); image.Apply();
        File.WriteAllBytes("../controls.png",image.EncodeToPNG());
        RenderTexture.active = null; camera.targetTexture = null;
    }
    static void IncomingTraySizes(Camera camera, BoardCameraZoom zoom, Action fit) {
        Match3DDefaultFont.InitializeFallback();
        var tray = new GameObject("Pot tray scale fixture").AddComponent<Match3DTray>();
        typeof(Match3DTray).GetMethod("Awake", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(tray,null);
        var prefab = new GameObject("PotFallbackPrefab");
        var cached = new GameObject("PotCachedIncoming"); ItemFlightMotion.Bind(cached,prefab.name,true,prefab);
        var plain = new GameObject("PotUnmappedIncoming");
        var original = new GameObject("PotSourceIncoming"); ItemFlightMotion.Bind(original,"BBQHotdog",true);
        var items = new[]{cached,plain,original};
        var sizes = new float[items.Length]; var stores = new float[items.Length];
        zoom.ResetView(); zoom.ProcessContacts(0,default,default,p=>false); fit();
        for(int i=0;i<items.Length;i++) {
            var pick=ItemFlightMotion.PickScale(items[i],Vector3.one);
            sizes[i]=pick.x/camera.orthographicSize;
            stores[i]=ItemFlightMotion.StoreScale(items[i],pick).x/camera.orthographicSize;
        }
        foreach(float multiplier in new[]{1.2f,1.6f,1f}) {
            zoom.Gesture(camera.pixelRect.center,camera.pixelRect.center,multiplier/zoom.Zoom);
            for(int frame=0;frame<100;frame++)fit();
            for(int i=0;i<items.Length;i++) {
                var pick=ItemFlightMotion.PickScale(items[i],Vector3.one);
                Check(Mathf.Abs(pick.x/camera.orthographicSize-sizes[i])<.0001f,"incoming tray size invariant "+items[i].name+" at "+multiplier);
                Check(Mathf.Abs(ItemFlightMotion.StoreScale(items[i],pick).x/camera.orthographicSize-stores[i])<.0001f,"incoming storage size invariant "+items[i].name+" at "+multiplier);
            }
        }
        foreach(var item in items){ItemFlightMotion.Forget(item);UnityEngine.Object.DestroyImmediate(item);}
        UnityEngine.Object.DestroyImmediate(prefab);
    }
    public static void Run() {
        try {
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
            var camera = new GameObject("Zoom camera").AddComponent<Camera>(); camera.tag = "MainCamera";
            camera.orthographic = true; camera.aspect = 9f / 16; camera.rect = new Rect(0, 0, 1, 1);
            camera.transform.SetPositionAndRotation(Match3DThemePresentation.SourcePosition, Match3DThemePresentation.SourceRotation);
            camera.orthographicSize = 7;
            var origin = camera.transform.position;
            var flow = new GameObject("Flow fixture").AddComponent<Match3DFlow>();
            typeof(Match3DFlow).GetField("<Instance>k__BackingField", BindingFlags.Static | BindingFlags.NonPublic).SetValue(null, flow);
            Set(flow, "<ScreenName>k__BackingField", "game");
            var presentation = flow.gameObject.AddComponent<Match3DThemePresentation>();
            Set(presentation, "visual", new GameObject("Board fixture"));
            var zoom = flow.gameObject.AddComponent<BoardCameraZoom>(); typeof(BoardCameraZoom).GetMethod("Awake", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(zoom, null);
            new GameObject("Game fixture").AddComponent<GameManager>();
            new GameObject("Input fixture").AddComponent<InputController>();
            GameManager.Instance.gameStatus = GameManager.GameStatus.PLAY;
            var input = InputController.Instance; input.SendMessage("Awake");
            zoom.ProcessContacts(0, default, default, p => false);
            void Fit() { var old = camera.transform.position; float oldSize = camera.orthographicSize; camera.transform.position = origin; camera.orthographicSize = 7; zoom.Apply(camera, old, oldSize, 1f / 60); }
            var midpoint = camera.pixelRect.center + new Vector2(30, 30);
            var anchor = camera.ScreenToWorldPoint(new Vector3(midpoint.x, midpoint.y, 30));
            zoom.Gesture(midpoint, midpoint, 1.4f); Fit();
            Check(Mathf.Abs(zoom.Zoom - 1.4f) < .0001f, "pinch zooms to 1.4");
            Check(Vector2.Distance(camera.WorldToScreenPoint(anchor), midpoint) < .01f, "midpoint world anchor preserved");
            zoom.Gesture(midpoint, midpoint + new Vector2(24, 12), 1); Fit();
            Check(Vector2.Distance(camera.WorldToScreenPoint(anchor), midpoint + new Vector2(24, 12)) < .01f, "two-finger pan follows fingers");
            zoom.Gesture(midpoint, midpoint, 10); Fit(); Check(zoom.Zoom == 1.6f, "upper limit 1.6");
            zoom.Gesture(midpoint, midpoint, .01f); for (int i = 0; i < 90; i++) Fit();
            Check(zoom.Zoom == 1 && Vector3.Distance(camera.transform.position, origin) < .00001f && camera.orthographicSize == 7, "pinch restores exact original pose and size");
            zoom.Gesture(midpoint, midpoint + new Vector2(90,70), 1); Fit();
            Check(camera.transform.position == origin && zoom.Zoom == 1, "dragging at 1.0 cannot move the original layout");
            foreach (float aspect in new[] { 9f/16, 9f/19.5f, 3f/4 }) {
                camera.aspect = aspect;
                var clamped = BoardCameraZoom.ClampOffset(new Vector2(100, -100), 7, aspect, 1.6f);
                Check(Mathf.Abs(clamped.x - 7 * (1 - 1 / 1.6f) * aspect) < .0001f && clamped.y < 0, "pan bounds " + aspect);
            }
            camera.aspect = 9f/16;
            var trayItem = GameObject.CreatePrimitive(PrimitiveType.Cube); trayItem.name = "Settled tray item";
            trayItem.transform.position = camera.ScreenToWorldPoint(new Vector3(300, 230, 1.58f));
            input.objectsOnStack.Add(trayItem);
            Vector3 before = camera.WorldToScreenPoint(trayItem.transform.position);
            float sizeBefore = trayItem.transform.localScale.x / camera.orthographicSize;
            zoom.Gesture(midpoint, midpoint, 1.6f); Fit();
            Check(Vector3.Distance(camera.WorldToScreenPoint(trayItem.transform.position), before) < .001f && Mathf.Abs(trayItem.transform.localScale.x / camera.orthographicSize - sizeBefore) < .0001f, "settled tray item screen position and size fixed");
            var target = GameObject.CreatePrimitive(PrimitiveType.Cube); target.tag = "Collectable"; target.layer = ItemColliderGeometry.PickLayer;
            target.transform.position = camera.ScreenToWorldPoint(new Vector3(midpoint.x,midpoint.y,30));
            target.transform.localScale = Vector3.one * .3f; target.AddComponent<Rigidbody>().useGravity = false;
            Physics.SyncTransforms();
            Check(input.PickAt(camera.WorldToScreenPoint(target.transform.position)) == target, "zoomed world mesh remains pickable");
            var originalTargetPosition = target.transform.position;
            var tools = new GameObject("Stored items fixture").AddComponent<BoardTools>();
            typeof(BoardTools).GetMethod("Awake", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(tools,null);
            var stored = GameObject.CreatePrimitive(PrimitiveType.Cube);
            stored.transform.position = camera.ScreenToWorldPoint(new Vector3(220, 260, 7));
            var columns = (System.Collections.Generic.List<GameObject>[])typeof(BoardTools).GetField("columns", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(tools);
            columns[0].Add(stored);
            Vector3 storedScreen = camera.WorldToScreenPoint(stored.transform.position);
            float storedScale = stored.transform.localScale.x / camera.orthographicSize;
            zoom.Restore(); for (int i = 0; i < 100; i++) Fit();
            Check(Vector3.Distance(camera.WorldToScreenPoint(stored.transform.position),storedScreen) < .001f && Mathf.Abs(stored.transform.localScale.x / camera.orthographicSize - storedScale) < .0001f, "stored item screen position and size fixed");
            Check(target.transform.position == originalTargetPosition, "camera changes never move board bodies");
            Check(Vector3.Distance(camera.transform.position, origin) < .00001f && zoom.Zoom == 1, "reset button restores original view");
            // Exercise the same contact state machine used by Update, without mocking Unity Input.
            zoom.ResetView(); zoom.ProcessContacts(0, default, default, p => false);
            var a = Finger(3, new Vector2(Screen.width * .35f, Screen.height * .625f)); var b = Finger(7, new Vector2(Screen.width * .65f, Screen.height * .625f));
            Check(!zoom.ProcessContacts(1, a, default, p => false), "ordinary single finger remains a pick");
            Check(zoom.ProcessContacts(2, a, b, p => false), "second finger consumes existing selection");
            b.position += Vector2.right * 60; zoom.ProcessContacts(2, a, b, p => false); Fit();
            Check(zoom.Zoom > 1, "two live contacts actually zoom");
            Check(zoom.ProcessContacts(1, a, default, p => false), "remaining finger cannot pick");
            Check(zoom.ProcessContacts(0, default, default, p => false), "emulated release swallowed");
            Check(!zoom.ProcessContacts(1, a, default, p => false), "new touch can pick after all fingers lift");
            zoom.ProcessContacts(0, default, default, p => false);
            float saved = zoom.Zoom;
            zoom.ProcessContacts(2, a, b, p => true); b.position += Vector2.right * 100;
            zoom.ProcessContacts(2, a, b, p => false); Fit();
            Check(zoom.Zoom == saved, "UI-origin gesture rejected for whole sequence");
            zoom.ProcessContacts(0, default, default, p => false);
            zoom.ProcessContacts(3, a, b, p => false); zoom.ProcessContacts(2, a, b, p => false);
            Check(zoom.Zoom == saved && zoom.SuppressPick, "third-finger interruption suppressed");
            zoom.ProcessContacts(0, default, default, p => false);
            zoom.ProcessContacts(1, a, default, p => true);
            zoom.ProcessContacts(2, a, b, p => false); b.position += Vector2.right * 20;
            zoom.ProcessContacts(2, a, b, p => false);
            Check(zoom.Zoom == saved, "first finger starting on UI cannot become a board pinch");
            zoom.ResetView(); Fit();
            Check(zoom.Zoom == 1 && camera.transform.position == origin, "new round reset");
            var controls = new GameObject("HUD", typeof(RectTransform)).AddComponent<BoardCameraZoomControls>();
            Match3DDefaultFont.InitializeFallback(); controls.Build((RectTransform)controls.transform);
            zoom.ProcessContacts(0, default, default, p => false); controls.SendMessage("LateUpdate");
            var buttons = controls.GetComponentsInChildren<Button>();
            Check(buttons.Length == 3 && !buttons[0].interactable && buttons[2].interactable, "HUD lower boundary button state");
            buttons[2].onClick.Invoke(); Fit(); Check(Mathf.Abs(zoom.Zoom - 1.2f) < .001f, "plus button steps by 0.2");
            Set(presentation, "camera", camera); Set(presentation, "zoom", zoom);
            presentation.Fit(); float fittedSize = camera.orthographicSize;
            presentation.Fit(); Check(Mathf.Abs(camera.orthographicSize - fittedSize) < .00001f && Mathf.Abs(fittedSize - Match3DThemePresentation.Size(camera.aspect) / zoom.Zoom) < .00001f && zoom.Zoom > 1, "per-frame board fitting preserves zoom");
            float beforeBlocked = zoom.Zoom;
            Set(input, "<IsResolving>k__BackingField", true); zoom.Gesture(midpoint,midpoint,1.3f);
            Check(zoom.Zoom == beforeBlocked, "active item motion blocks camera changes");
            Set(input, "<IsResolving>k__BackingField", false);
            Set(flow, "blocksGameplay", true); zoom.Gesture(midpoint,midpoint,1.3f);
            Check(zoom.Zoom == beforeBlocked, "modal blocks camera changes");
            Set(flow, "blocksGameplay", false);
            // Reopening a round adds a new presentation on the persistent Flow object.
            // Keep the old component present, matching LayeredItemPile.Clear/Begin.
            zoom.ReleasePresentation(presentation); Set(presentation, "visual", null);
            Check(!zoom.Available, "closed presentation hides zoom");
            for (int round = 0; round < 3; round++) {
                var next = flow.gameObject.AddComponent<Match3DThemePresentation>();
                Set(next, "visual", new GameObject("Round " + round));
                zoom.BindPresentation(next); zoom.ProcessContacts(0, default, default, p => false);
                Check(zoom.Available && zoom.Zoom == 1, "new round rebinds zoom " + round);
                zoom.Step(.2f); Fit();
                Check(Mathf.Abs(zoom.Zoom - 1.2f) < .001f, "reopened round can zoom " + round);
                zoom.ReleasePresentation(presentation);
                Check(zoom.Available && zoom.Zoom > 1, "old presentation cannot reset current round " + round);
                zoom.ReleasePresentation(next); Set(next, "visual", null); presentation = next;
            }
            Set(presentation, "visual", new GameObject("Capture board")); zoom.BindPresentation(presentation);
            trayItem.SetActive(false); stored.SetActive(false); target.SetActive(false);
            IncomingTraySizes(camera,zoom,Fit);
            Capture(camera, zoom, controls.gameObject);
            File.WriteAllText("../result.txt", "PASS " + checks + " Unity checks: zoom, midpoint, pan, exact reset, foreground, contact lifecycle and HUD.\n");
            EditorApplication.Exit(0);
        } catch (Exception e) { File.WriteAllText("../result.txt", "FAIL " + e); Debug.LogException(e); EditorApplication.Exit(1); }
    }
}
