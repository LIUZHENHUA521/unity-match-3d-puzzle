using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputController : Singleton<InputController>
{
    public List<GameObject> objectsOnStack = new List<GameObject>();
    public List<GameObject> arrivedObject = new List<GameObject>();
    public bool IsResolving { get; private set; }
    private Coroutine resolution;
    public bool ReturningToBowl { get; private set; }
    public bool IsBusy => ReturningToBowl || IsResolving || resolution != null || HasPendingVisuals;
    public bool UsesSourceFlight => Match3DFlow.Instance && !Match3DFlow.Instance.IsPvp && (Match3DFlow.Instance.Stage==1||Match3DFlow.Instance.Stage==2);
    public bool HasPendingVisuals => sourceFlying.Count>0 || sourceCombining.Count>0;
    // A completed triple must not release victory while another triple is still
    // animating: its Matched() callback has not necessarily updated the record yet.
    public bool CanEvaluateWin => !UsesSourceFlight || (sourceCompletionCheckReady && !HasPendingVisuals);
    readonly HashSet<GameObject> sourceFlying=new HashSet<GameObject>();
    readonly HashSet<GameObject> sourceCombining=new HashSet<GameObject>();
    readonly Dictionary<GameObject,List<Tween>> sourceMoves=new Dictionary<GameObject,List<Tween>>();
    bool sourceCompletionCheckReady=true;
    public float LastFlightDuration {get;private set;}

    private readonly List<GameObject> pickHistory = new List<GameObject>();
    private readonly Dictionary<GameObject, Vector3> scales = new Dictionary<GameObject, Vector3>();
    readonly Dictionary<GameObject, Quaternion> boardRotations = new Dictionary<GameObject, Quaternion>();
    private readonly Dictionary<GameObject, Dictionary<Collider,bool>> boardColliders = new Dictionary<GameObject, Dictionary<Collider,bool>>();
    private Match3DSelectionFeedback selection;
    private void Awake() { Input.multiTouchEnabled = true; selection=gameObject.AddComponent<Match3DSelectionFeedback>(); }
    public void CancelSelection(){CancelPointer();activeFinger=null;}
    private void ClearPointerTarget(){pressedObject=null;if(selection)selection.Cancel();}
    private void CancelPointer(){ClearPointerTarget();pointerHeld=false;pointerStartedOnUI=false;}
    private void OnDisable(){CancelPointer();activeFinger=null;}
    private void OnApplicationFocus(bool focus){if(!focus){CancelPointer();activeFinger=null;}}
    private GameObject pressedObject;
    private Vector2 lastTargetPosition;
    private bool pointerHeld, pointerStartedOnUI;
    public int PointerStarts {get;private set;}
    public int PointerHits {get;private set;}
    public int AcceptedPicks {get;private set;}
    public string LastPickState {get;private set;}="idle";
    private int? activeFinger;
    private bool PointerOnButton(Vector2 position)
    {
        if (!EventSystem.current) return false;
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current) { position = position }, results);
        return results.Any(r => r.gameObject.GetComponentInParent<UnityEngine.UI.Selectable>() != null);
    }
    RaycastHit[] pickHits = new RaycastHit[32];
    readonly HashSet<Rigidbody> pickBodies = new HashSet<Rigidbody>();
    private GameObject PickExact(Camera camera, Vector2 position)
    {
        if (!camera.pixelRect.Contains(position)) return null;
        var ray = camera.ScreenPointToRay(position);
        int count;
        // A full buffer may have omitted the nearest mesh. Grow and retry rather
        // than relying on Physics' unspecified hit order in a dense pile.
        while ((count = Physics.RaycastNonAlloc(ray, pickHits, camera.farClipPlane, 1 << ItemColliderGeometry.PickLayer,
                                               QueryTriggerInteraction.Ignore)) == pickHits.Length)
            System.Array.Resize(ref pickHits, pickHits.Length * 2);
        pickBodies.Clear();
        Rigidbody nearest = null; float distance = camera.farClipPlane;
        for (int i = 0; i < count; i++) {
            var hit = pickHits[i]; var body = hit.collider.attachedRigidbody;
            if (!body || !body.CompareTag("Collectable")) continue;
            // Collider-only legacy objects may own multiple shapes: compare all
            // their distances. Mesh items can be tested just once per query.
            var geometry = body.GetComponent<ItemMeshPicking>() ?? body.gameObject.AddComponent<ItemMeshPicking>();
            bool meshItem = geometry.HasRenderGeometry;
            if (meshItem && !pickBodies.Add(body)) continue;
            if (ItemMeshPicking.Raycast(body.gameObject, ray, distance, hit.distance, out float meshDistance)) {
                nearest = body; distance = meshDistance;
            }
        }
        var selected = nearest;
        return selected && (!LayeredItemPile.Instance || LayeredItemPile.Instance.CanPick(selected)) &&
            (selected.gameObject == pressedObject || BoardTools.IsStored(selected.gameObject) || !selected.isKinematic ||
             (ItemPhysicsController.Instance && ItemPhysicsController.Instance.Contains(selected))) ? selected.gameObject : null;
    }
    public GameObject PickAt(Vector2 screenPosition)
    {
        var camera = Camera.main;
        if (!camera || !camera.pixelRect.Contains(screenPosition)) return null;
        // One screen ray; mesh refinement lets actual gaps expose rear items without nearby-ray snapping.
        return PickExact(camera, screenPosition);
    }
    private void BeginPointer(Vector2 position)
    {
        CancelPointer();
        if(BoardTools.Busy||(IsBusy&&!UsesSourceFlight)||objectsOnStack.Count>=7)return;
        PointerStarts++;pointerHeld=true;pointerStartedOnUI=PointerOnButton(position);
        UpdatePointer(position);
    }
    private void UpdatePointer(Vector2 position)
    {
        if(!pointerHeld)return;
        if(pointerStartedOnUI||PointerOnButton(position)){
            ClearPointerTarget();LastPickState="ui";return;
        }
        var target=PickAt(position);
        if(!target){ClearPointerTarget();LastPickState="miss";return;}
        if(target==pressedObject){lastTargetPosition=position;return;}
        // WX HasTouchMove j38504: with _isMoveTouchChangeObj, don't switch targets
        // merely because physics changed what is under a stationary finger.
        if(pressedObject && Mathf.Approximately(lastTargetPosition.x,position.x) &&
            Mathf.Approximately(lastTargetPosition.y,position.y))return;
        ClearPointerTarget();pressedObject=target;lastTargetPosition=position;PointerHits++;
        // ObjectUnit.BeSelect exits for isStore; stored objects can still be released into the tray.
        if(!BoardTools.IsStored(target))selection.Begin(target,Camera.main,!(Match3DFlow.Instance&&Match3DFlow.Instance.IsPvp));
        LastPickState="hit";
    }
    private void EndPointer(Vector2 position)
    {
        var selected=pressedObject;
        bool collect=pointerHeld&&!pointerStartedOnUI&&!PointerOnButton(position);
        // Successful source GetTarget invokes BeClick directly, not LostTarget/BeSelect(false).
        if(UsesSourceFlight){if(collect&&selected)AddObjectOnStack(selected);CancelPointer();}
        else{CancelPointer();if(collect&&selected)AddObjectOnStack(selected);}
    }
    private void Update()
    {
        if (BoardCameraZoom.Instance && BoardCameraZoom.Instance.ProcessTouches(PointerOnButton)) {
            LastPickState="camera gesture";CancelSelection();return;
        }
        if (!GameManager.Instance.GameIsPlaying() || (Match3DFlow.Instance && Match3DFlow.Instance.BlocksGameplay)) {
            LastPickState="blocked";CancelPointer(); activeFinger = null; return;
        }
        if (Input.touchCount > 0) {
            for (int i = 0; i < Input.touchCount; i++) {
                var touch = Input.GetTouch(i);
                // Like uGUI, a newly observed contact is a press even when Began
                // was not sampled (a desktop click can arrive entirely between frames).
                // Keep the identifier separate from the absence of a contact.
                if (!activeFinger.HasValue && touch.phase != TouchPhase.Canceled) {
                    activeFinger = touch.fingerId; BeginPointer(touch.position);
                }
                if (touch.fingerId == activeFinger) {
                    if (touch.phase == TouchPhase.Ended) { EndPointer(touch.position); activeFinger = null; }
                    else if (touch.phase == TouchPhase.Canceled) { LastPickState="cancel";CancelPointer(); activeFinger = null; }
                    else UpdatePointer(touch.position);
                }
            }
            return; // A touch must not also be consumed as an emulated mouse click.
        }
        if (activeFinger.HasValue) { activeFinger = null; CancelPointer(); return; }
        if (Input.GetMouseButtonDown(0)) BeginPointer(Input.mousePosition);
        if (Input.GetMouseButton(0)) UpdatePointer(Input.mousePosition);
        if (Input.GetMouseButtonUp(0)) EndPointer(Input.mousePosition);
    }
    sealed class MagnetFlight { public Vector3 start; public Quaternion rotation; public float started,delay; }
    readonly Dictionary<GameObject,MagnetFlight> magnetFlights=new Dictionary<GameObject,MagnetFlight>();
    sealed class ProbeFlight {public Vector3 start;public Quaternion rotation;public float started;public bool contactStarted;}
    readonly Dictionary<GameObject,ProbeFlight> probeFlights=new Dictionary<GameObject,ProbeFlight>();
    const float MagnetSeconds=ItemFlightMotion.MagnetSeconds;
    public void AddObjectOnStack(GameObject obj,bool playSound=true,bool magnetic=false,float magnetDelay=0,ProbePickSource source=ProbePickSource.Manual,Vector3? potProbeOrigin=null)
    {
        if (BoardTools.Busy || !GameManager.Instance.GameIsPlaying() || (IsResolving&&!UsesSourceFlight) || !obj || !obj.CompareTag("Collectable") ||
            objectsOnStack.Count >= 7 || objectsOnStack.Contains(obj)) return;
        if (Match3DFlow.Instance && Match3DFlow.Instance.InterceptPick(obj)) return;
        if(UsesSourceFlight&&playSound&&LayeredItemPile.Instance)LayeredItemPile.Instance.RecordSelectionHeight(obj.transform.position.y);
        if(selection&&selection.Selected==obj){if(UsesSourceFlight)selection.Commit();CancelPointer();}
        if(!UsesSourceFlight)BoardTools.ReleaseStored(obj);
        AcceptedPicks++;LastPickState="picked";
        if(playSound&&!magnetic&&source==ProbePickSource.Manual&&Match3DFlow.Instance)Match3DFlow.Instance.PotVibrate();
        if(playSound&&!UsesSourceFlight&&Match3DFlow.Instance)Match3DFlow.Instance.SelectSound();
        if(UsesSourceFlight)obj.transform.DOKill(false);
        pickHistory.Add(obj);
        if(LayeredItemPile.Instance)LayeredItemPile.Instance.Detach(obj.GetComponent<Rigidbody>());
        var trayDisplay=UsesSourceFlight?Match3DTrayDisplayBinding.For(obj):null;
        if(trayDisplay&&!BoardTools.IsStored(obj))trayDisplay.CaptureBoardPose();
        scales[obj] = trayDisplay?trayDisplay.boardScale:obj.transform.localScale;
        boardRotations[obj] = trayDisplay?trayDisplay.boardRotation:obj.transform.rotation;
        var body = obj.GetComponent<Rigidbody>();
        if (ItemPhysicsController.Instance) ItemPhysicsController.Instance.RemoveForPick(body);
        var potReplayTile=obj.GetComponent<Match3DTile>();if(potReplayTile)PotReplayRecorder.Event("pick",potReplayTile.id,magnetic?1:source==ProbePickSource.Probe?2:0);
        if(UsesSourceFlight){
            // Disable contact before any pickup movement/scale, for manual, probe
            // and magnet paths alike. A 50 ms grace period can kick the pile.
            body.isKinematic=true;body.detectCollisions=false;
            obj.GetComponent<SpecialItemAnimation>()?.EnterTray();
            if(playSound)GameAudioCatalog.Select(obj);
        }else{
            if(body.isKinematic)body.isKinematic=false;
            body.velocity=Vector3.zero;body.angularVelocity=Vector3.zero;
            body.detectCollisions=false;body.interpolation=RigidbodyInterpolation.None;body.isKinematic=true;
        }
        var colliderStates=new Dictionary<Collider,bool>();
        foreach (var collider in obj.GetComponentsInChildren<Collider>(true)){colliderStates[collider]=collider.enabled;collider.enabled=false;}
        boardColliders[obj]=colliderStates;
        objectsOnStack.Add(obj);
        objectsOnStack = objectsOnStack.GroupBy(x => x.name).SelectMany(g => g).ToList();
        if(UsesSourceFlight){if(magnetic)magnetFlights[obj]=new MagnetFlight{start=obj.transform.position,rotation=obj.transform.rotation,started=Time.unscaledTime,delay=magnetDelay};SourcePick(obj,magnetic?ProbePickSource.Automatic:source,potProbeOrigin);BoardTools.ReleaseStored(obj);return;}
        Reposition();
        if (resolution != null) StopCoroutine(resolution);
        resolution = StartCoroutine(Resolve());
    }
    private void SourcePick(GameObject obj,ProbePickSource source,Vector3? potProbeOrigin)
    {
        // Each pickup invalidates the completion checkpoint. CanEvaluateWin also
        // waits for every concurrent flight/combine to finish before settlement.
        sourceCompletionCheckReady=false;
        bool probe=source==ProbePickSource.Probe;
        if(probe){
            probeFlights[obj]=new ProbeFlight{start=obj.transform.position,rotation=obj.transform.rotation,started=Time.time};
            if(potProbeOrigin.HasValue)PotProbeActivation.Begin(obj,potProbeOrigin.Value);
        }
        LastFlightDuration=magnetFlights.TryGetValue(obj,out var magnet)?MagnetSeconds+magnet.delay:ItemFlightMotion.MoveSeconds;
        if(probe)LastFlightDuration+=PotProbeActivation.Duration+ProbeLiftContact.Duration;
        float flightSeconds=LastFlightDuration;
        var initial=scales[obj];var target=ItemFlightMotion.PickScale(obj,initial);
        var display=Match3DTrayDisplayBinding.For(obj);if(display)target*=display.pose.scale;
        sourceFlying.Add(obj);
        if(magnet!=null){
            float elapsed=0;
            DOTween.To(()=>elapsed,t=>{
                elapsed=t;if(obj)obj.transform.localScale=ItemFlightMotion.MagnetScale(initial,target,t-magnet.delay);
            },flightSeconds,flightSeconds).SetUpdate(true).SetEase(Ease.Linear).SetTarget(obj.transform).OnComplete(()=>{
                if(!obj)return;
                sourceFlying.Remove(obj);magnetFlights.Remove(obj);probeFlights.Remove(obj);ProbeLiftContact.Stop(obj);PotProbeActivation.Stop(obj);
                if(objectsOnStack.Contains(obj)&&!arrivedObject.Contains(obj))arrivedObject.Add(obj);
            });
            BoardToolVisual.MagnetTrail(obj,magnet.delay);
        }else{
        // BeClick starts scale at release; RefreshContentDisplay starts all three axes immediately.
        obj.transform.DOScale(initial*ItemFlightMotion.ScalePeak,ItemFlightMotion.ScalePeakSeconds)
            .SetDelay(probe?PotProbeActivation.Duration+ProbeLiftContact.Duration:0).SetUpdate(!probe).SetEase(Ease.OutQuad).OnComplete(()=>{
                if(!obj)return;
                obj.transform.DOScale(target,ItemFlightMotion.MoveSeconds-ItemFlightMotion.ScalePeakSeconds)
                    .SetUpdate(!probe).SetEase(Ease.InSine).OnComplete(()=>{
                        if(!obj)return;
                        sourceFlying.Remove(obj);magnetFlights.Remove(obj);probeFlights.Remove(obj);ProbeLiftContact.Stop(obj);PotProbeActivation.Stop(obj);
                        if(objectsOnStack.Contains(obj)&&!arrivedObject.Contains(obj))arrivedObject.Add(obj);
                        obj.transform.DOPunchScale(initial*ItemFlightMotion.PunchScale,ItemFlightMotion.PunchSeconds,1,1);
                    });
            });
        }
        RepositionSource();
        // DelayToCheckContent executes its match scan BEFORE its first yield.
        var match=objectsOnStack.GroupBy(x=>x.name).FirstOrDefault(g=>g.Count()>=3);
        if(match!=null){
            var items=match.Take(3).Reverse().ToArray();
            foreach(var item in items){
                objectsOnStack.Remove(item);arrivedObject.Remove(item);pickHistory.Remove(item);
                LevelGenerator.Instance.objectsOnScene.Remove(item);sourceCombining.Add(item);
            }
            StartCoroutine(SourceCombine(items));
        }
        if(ComboProbeController.Instance)ComboProbeController.Instance.PickAccepted(obj,source,match!=null);
        // A seventh item which completes a triple has already freed its three logical slots.
        if(objectsOnStack.Count>=7)UIManager.Instance.ShowOutOfBoxesPopUp();
        BoosterManager.Instance.CheckHintInteractibility();
    }
    private void RepositionSource(){
        for(int i=0;i<objectsOnStack.Count;i++){
            var obj=objectsOnStack[i];if(!obj)continue;
            if(sourceMoves.TryGetValue(obj,out var old))foreach(var tween in old)if(tween.IsActive())tween.Kill();
            var slot=Match3DTray.Instance?Match3DTray.Instance.SlotPosition(i):new Vector3(-1+i*.333f,.3f,-1.62f);
            var display=Match3DTrayDisplayBinding.For(obj);
            var cameraRotation=Camera.main?Camera.main.transform.rotation:Quaternion.identity;
            float spacing=Match3DTray.Instance?Vector3.Distance(Match3DTray.Instance.SlotPosition(0),Match3DTray.Instance.SlotPosition(1)):Match3DTray.SourcePointSpacing;
            slot=Match3DTrayDisplaySettings.Position(display?display.pose:null,slot,cameraRotation,spacing);
            var rotation=display?Match3DTrayDisplaySettings.Rotation(display.pose,cameraRotation):Camera.main?cameraRotation*Quaternion.Euler(-90,0,0):Quaternion.identity;
            if(probeFlights.TryGetValue(obj,out var probe)){
                float elapsed=Mathf.Max(0,Time.time-probe.started);
                float duration=PotProbeActivation.Duration+ProbeLiftContact.Duration+ItemFlightMotion.MoveSeconds;
                var motion=DOTween.To(()=>elapsed,t=>{
                    if(!obj)return;
                    // Repositioning after another pick retains this pickup's original clock.
                    float local=t-PotProbeActivation.Duration;
                    if(local<0){
                        obj.transform.position=probe.start;obj.transform.rotation=probe.rotation;
                        obj.transform.localScale=scales[obj]*PotProbeActivation.ActivationScale(t);
                    }else if(local<ProbeLiftContact.Duration){
                        if(!probe.contactStarted){probe.contactStarted=true;ProbeLiftContact.Begin(obj);}
                        obj.transform.localScale=scales[obj];
                        obj.transform.position=ProbeLiftContact.Lift(probe.start,local);obj.transform.rotation=probe.rotation;
                    }else{
                        ProbeLiftContact.Stop(obj);
                        float u=Mathf.Clamp01((local-ProbeLiftContact.Duration)/ItemFlightMotion.MoveSeconds);
                        obj.transform.position=Vector3.Lerp(ProbeLiftContact.Lift(probe.start,ProbeLiftContact.Duration),slot,u);
                        obj.transform.rotation=Quaternion.Slerp(probe.rotation,rotation,u*u);
                    }
                },duration,Mathf.Max(.001f,duration-elapsed)).SetEase(Ease.Linear).SetTarget(obj.transform);
                sourceMoves[obj]=new List<Tween>{motion};continue;
            }
            if(magnetFlights.TryGetValue(obj,out var magnet)){
                float elapsed=Mathf.Max(0,Time.unscaledTime-magnet.started);
                float remaining=Mathf.Max(.001f,MagnetSeconds+magnet.delay-elapsed);
                var up=Camera.main?Camera.main.transform.up:Vector3.up;
                var right=Camera.main?Camera.main.transform.right:Vector3.right;
                var motion=DOTween.To(()=>elapsed,t=>{
                    if(!obj)return;
                    float local=Mathf.Clamp(t-magnet.delay,0,MagnetSeconds);
                    obj.transform.position=ItemFlightMotion.MagnetPosition(magnet.start,slot,up,right,spacing,local);
                    float pullStart=ItemFlightMotion.MagnetShakeSeconds+ItemFlightMotion.MagnetLiftSeconds;
                    float turn=Mathf.Clamp01((local-pullStart)/ItemFlightMotion.MagnetPullSeconds);
                    obj.transform.rotation=Quaternion.Slerp(magnet.rotation,rotation,turn);
                    if(local<pullStart){
                        float shake=local/ItemFlightMotion.MagnetShakeSeconds;
                        float angle=local<ItemFlightMotion.MagnetShakeSeconds?Mathf.Sin(shake*Mathf.PI*4)*3*Mathf.Sin(shake*Mathf.PI):-5*Mathf.Sin((local-ItemFlightMotion.MagnetShakeSeconds)/ItemFlightMotion.MagnetLiftSeconds*Mathf.PI);
                        obj.transform.rotation=Quaternion.AngleAxis(angle,Vector3.Cross(right,up))*magnet.rotation;
                    }
                },MagnetSeconds+magnet.delay,remaining).SetEase(Ease.Linear).SetUpdate(true).SetTarget(obj.transform);
                sourceMoves[obj]=new List<Tween>{motion};
                continue;
            }
            // RefreshContentDisplay applies the SAME .3 s to existing and incoming items.
            sourceMoves[obj]=new List<Tween>{
                obj.transform.DOMoveX(slot.x,ItemFlightMotion.MoveSeconds).SetUpdate(true).SetEase(Ease.Linear),
                obj.transform.DOMoveY(slot.y,ItemFlightMotion.MoveSeconds).SetUpdate(true).SetEase(Ease.OutQuad),
                obj.transform.DOMoveZ(slot.z,ItemFlightMotion.MoveSeconds).SetUpdate(true).SetEase(Ease.Linear),
                obj.transform.DORotateQuaternion(rotation,ItemFlightMotion.MoveSeconds).SetUpdate(true).SetEase(Ease.InQuad)
            };
        }
    }
    private IEnumerator SourceCombine(GameObject[] items){
        // The original old branch waits .3, converges the two sides .2, shrinks the centre .2,
        // checks completion, then keeps the zero-sized centre another .05 before compacting.
        yield return new WaitForSeconds(ItemFlightMotion.MoveSeconds);
        while(items.Any(x=>x&&sourceFlying.Contains(x)))yield return null;
        if(!items[1])yield break;
        var center=items[1].transform.position;
        foreach(int i in new[]{0,2})if(items[i])items[i].transform.DOMove(center,ItemFlightMotion.MergeSeconds).SetEase(Ease.OutQuad);
        yield return new WaitForSeconds(ItemFlightMotion.MergeSeconds);
        RemoveSourceVisual(items[0]);RemoveSourceVisual(items[2]);
        if(items[1]){
            items[1].transform.DOScale(Vector3.zero,ItemFlightMotion.ShrinkSeconds).SetEase(Ease.InBack);
        }
        yield return new WaitForSeconds(ItemFlightMotion.ShrinkSeconds);
        GameAudioCatalog.Combine(items[1]);
        sourceCompletionCheckReady=true;
        if(Match3DGameRecords.Instance)Match3DGameRecords.Instance.Matched();
        yield return new WaitForSeconds(ItemFlightMotion.FinalRemovalDelay);
        RemoveSourceVisual(items[1]);
        RepositionSource();BoosterManager.Instance.CheckHintInteractibility();
    }
    private void RemoveSourceVisual(GameObject obj){
        if(!obj)return;
        obj.transform.DOKill();magnetFlights.Remove(obj);probeFlights.Remove(obj);ProbeLiftContact.Stop(obj);PotProbeActivation.Stop(obj);sourceFlying.Remove(obj);sourceCombining.Remove(obj);sourceMoves.Remove(obj);
        scales.Remove(obj);boardRotations.Remove(obj);boardColliders.Remove(obj);ItemFlightMotion.Forget(obj);GameAudioCatalog.Forget(obj);
        // LevelObjectPool.Recycle keeps the ObjectUnit for a later Get(objName);
        // only the logical board list is removed at combine time.
        BoardItemPool.Recycle(obj);
    }
    private void Reposition()
    {
        if(UsesSourceFlight){RepositionSource();return;}
        for (int i=0; i<objectsOnStack.Count; i++) {
            var item = objectsOnStack[i];
            item.transform.DOKill();
            var slot = Match3DTray.Instance ? Match3DTray.Instance.SlotPosition(i) : new Vector3(-1+i*.333f, .3f, -1.62f);
            // Start travelling on release; selection already supplied the small lift.
            // Use a local easing approximation, not a recovered original animation curve.
            float moveTime = arrivedObject.Contains(item) ? .20f : .30f;
            item.transform.DOMove(slot, moveTime).SetEase(Ease.OutQuad);
            item.transform.DORotate(Vector3.zero, moveTime).SetEase(Ease.OutQuad);
            if (LevelGenerator.Instance.foodPile) {
                // Keep collected items within their slots; undo restores the enlarged board scale.
                float slotScale = Match3DFlow.Instance ? .6f / Match3DFlow.BoardItemScale : .6f;
                if(Match3DTray.Instance)slotScale *= Match3DTray.Instance.ItemScaleFactor;
                var renderers=item.GetComponentsInChildren<Renderer>();
                if(renderers.Length>0){
                    // Evaluate unrotated, full-scale bounds before tray tweening.
                    var savedRotation=item.transform.rotation;var savedScale=item.transform.localScale;
                    item.transform.rotation=Quaternion.identity;item.transform.localScale=scales[item];
                    var bounds=renderers[0].bounds;foreach(var r in renderers)bounds.Encapsulate(r.bounds);
                    float maxSize=Mathf.Max(bounds.size.x,bounds.size.y,bounds.size.z);
                    if(maxSize>0)slotScale=Mathf.Min(slotScale,.28f*(Match3DTray.Instance?Match3DTray.Instance.ItemScaleFactor:1)/maxSize);
                    item.transform.rotation=savedRotation;item.transform.localScale=savedScale;
                }
                item.transform.DOScale(scales[item] * slotScale, moveTime).SetEase(Ease.OutQuad);
            }
        }
    }
    private IEnumerator Resolve()
    {
        yield return new WaitForSeconds(.32f);
        if (!GameManager.Instance.GameIsPlaying()) yield break;
        arrivedObject = new List<GameObject>(objectsOnStack);
        while (true) {
            var match = objectsOnStack.GroupBy(x => x.name).FirstOrDefault(g => g.Count() >= 3);
            if (match == null) break;
            IsResolving = true;
            var items = match.Take(3).ToArray();
            foreach (var item in items) item.transform.DOScale(Vector3.zero, .18f);
            yield return new WaitForSeconds(.2f);
            foreach (var item in items) {
                objectsOnStack.Remove(item); arrivedObject.Remove(item); scales.Remove(item); boardRotations.Remove(item); boardColliders.Remove(item); pickHistory.Remove(item);
                LevelGenerator.Instance.objectsOnScene.Remove(item); item.transform.DOKill(); Destroy(item);
            }
            if (Match3DFlow.Instance) Match3DFlow.Instance.MatchSound();
            Reposition();
            yield return new WaitForSeconds(.32f);
        }
        IsResolving = false;
        resolution = null;
        BoosterManager.Instance.CheckHintInteractibility();
        if (objectsOnStack.Count >= 7) UIManager.Instance.ShowOutOfBoxesPopUp();
    }
    public bool ReturnSourceTrayToBowl() {
        if(!UsesSourceFlight||BoardTools.Busy||!BoardTools.CanUse(0)||!GameManager.Instance.GameIsPlaying())return false;
        var items=objectsOnStack.Where(x=>x).ToList();
        if(items.Count==0||!BoardTools.TrySpend(0))return false;
        BeginReturnToBowl(items,false);return true;
    }
    public bool MoveSourceItemsToStore(int count=BoardTools.ReturnCount) {
        if(!UsesSourceFlight||BoardTools.Busy||!BoardTools.CanUse(0)||!GameManager.Instance.GameIsPlaying())return false;
        var items=BoardTools.ReturnCandidates(objectsOnStack,Mathf.Min(count,BoardTools.ReturnCount));
        if(items.Count==0||!BoardTools.TrySpend(0))return false;
        PotMoveTrayItemsToStore(items);return true;
    }
    void PotMoveTrayItemsToStore(List<GameObject> items){
        CancelSelection();
        foreach(var obj in items){
            obj.transform.DOKill();magnetFlights.Remove(obj);probeFlights.Remove(obj);ProbeLiftContact.Stop(obj);PotProbeActivation.Stop(obj);sourceMoves.Remove(obj);sourceFlying.Remove(obj);
            objectsOnStack.Remove(obj);arrivedObject.Remove(obj);pickHistory.Remove(obj);
            var body=obj.GetComponent<Rigidbody>();body.isKinematic=true;body.detectCollisions=true;
            if(boardColliders.TryGetValue(obj,out var flags))foreach(var flag in flags)if(flag.Key)flag.Key.enabled=flag.Value;
            obj.GetComponent<SpecialItemAnimation>()?.EnterStore();
        }
        BoardTools.Store(items);
        RepositionSource();BoosterManager.Instance.CheckHintInteractibility();
    }
    // Both revival routes move the entire tray into storage without spending ordinary tools.
    // An empty tray is valid for a time revival.
    public bool ReviveSourceTray(){
        if(!UsesSourceFlight||BoardTools.Busy)return false;
        PotMoveTrayItemsToStore(objectsOnStack.Where(x=>x).ToList());return true;
    }
    sealed class BowlReturn {
        public GameObject obj; public Rigidbody body;
        public Vector3 start,target,scale,boardScale;
        public Quaternion rotation,boardRotation;
        public Dictionary<Collider,bool> colliders;
    }
    void BeginReturnToBowl(List<GameObject> items,bool shuffleAfter){
        CancelSelection();ReturningToBowl=true;
        if(ItemPhysicsController.Instance)ItemPhysicsController.Instance.ReleaseDailyOpening();
        var flights=new List<BowlReturn>();
        var center=LayeredItemPile.Instance&&LayeredItemPile.Instance.Running?LayeredItemPile.Instance.ReturnPosition():new Vector3(0,2,.15f);
        for(int i=0;i<items.Count;i++){
            var obj=items[i];obj.transform.DOKill();
            magnetFlights.Remove(obj);probeFlights.Remove(obj);ProbeLiftContact.Stop(obj);PotProbeActivation.Stop(obj);sourceMoves.Remove(obj);sourceFlying.Remove(obj);
            objectsOnStack.Remove(obj);arrivedObject.Remove(obj);pickHistory.Remove(obj);
            var item=new BowlReturn{obj=obj,body=obj.GetComponent<Rigidbody>(),start=obj.transform.position,
                scale=obj.transform.localScale,rotation=obj.transform.rotation,
                boardScale=scales.TryGetValue(obj,out var savedScale)?savedScale:obj.transform.localScale,
                boardRotation=boardRotations.TryGetValue(obj,out var savedRotation)?savedRotation:Quaternion.identity,
                colliders=boardColliders.TryGetValue(obj,out var flags)?flags:obj.GetComponentsInChildren<Collider>(true).ToDictionary(c=>c,c=>c.enabled)};
            float angle=i*Mathf.PI*2/Mathf.Max(1,items.Count);
            item.target=center+new Vector3(Mathf.Cos(angle),i*.08f,Mathf.Sin(angle))*(items.Count>1?.9f:0);
            // Fit using full board geometry, then restore the tray pose for the flight.
            obj.transform.localScale=item.boardScale;obj.transform.rotation=item.boardRotation;
            if(BoardEnvironment.UsesSoftContainer)item.target=SoftTransitionContainer.FittedPosition(obj,item.target);
            obj.transform.localScale=item.scale;obj.transform.rotation=item.rotation;
            foreach(var collider in item.colliders.Keys)if(collider)collider.enabled=false;
            item.body.isKinematic=true;item.body.detectCollisions=false;
            scales.Remove(obj);boardRotations.Remove(obj);boardColliders.Remove(obj);
            obj.GetComponent<SpecialItemAnimation>()?.ReturnToPile();flights.Add(item);
        }
        RepositionSource();BoosterManager.Instance.CheckHintInteractibility();
        StartCoroutine(ReturnToBowl(flights,shuffleAfter));
    }
    IEnumerator ReturnToBowl(List<BowlReturn> flights,bool shuffleAfter){
        const float duration=.48f;
        float elapsed=0;
        // Defer at least one frame so the revive modal can restore gameplay/time first.
        do {
            yield return null;elapsed+=Time.deltaTime;
            float t=Mathf.Clamp01(elapsed/duration),ease=t*t*(3-2*t);
            foreach(var item in flights)if(item.obj){
                item.obj.transform.position=Vector3.Lerp(item.start,item.target,ease)+Vector3.up*(Mathf.Sin(t*Mathf.PI)*1.1f);
                item.obj.transform.localScale=Vector3.Lerp(item.scale,item.boardScale,ease);
                item.obj.transform.rotation=Quaternion.Slerp(item.rotation,item.boardRotation,ease);
            }
        }while(elapsed<duration);
        foreach(var item in flights)if(item.obj){
            var obj=item.obj;obj.transform.SetPositionAndRotation(item.target,item.boardRotation);obj.transform.localScale=item.boardScale;
            item.body.position=item.target;item.body.rotation=item.boardRotation;
            item.body.detectCollisions=true;item.body.interpolation=RigidbodyInterpolation.None;
            item.body.isKinematic=false;item.body.velocity=Vector3.zero;item.body.angularVelocity=Vector3.zero;
            foreach(var collider in item.colliders)if(collider.Key)collider.Key.enabled=collider.Value;
            if(LayeredItemPile.Instance)LayeredItemPile.Instance.Register(item.body,initialHeight:item.target.y);
            if(ItemPhysicsController.Instance)ItemPhysicsController.Instance.Register(item.body);
        }
        ReturningToBowl=false;
        if(shuffleAfter&&GameManager.Instance.GameIsPlaying())BoardTools.ShuffleAfterRevival();
        BoosterManager.Instance.CheckHintInteractibility();
    }
    public void UndoLast()
    {
        if (IsBusy || !GameManager.Instance.GameIsPlaying() || arrivedObject.Count == 0) return;
        var obj = pickHistory.Last();
        pickHistory.RemoveAt(pickHistory.Count-1);
        objectsOnStack.Remove(obj); arrivedObject.Remove(obj);
        obj.transform.DOKill();
        obj.transform.localScale = scales[obj]; scales.Remove(obj); boardRotations.Remove(obj);
        var display=UsesSourceFlight?Match3DTrayDisplayBinding.For(obj):null;
        bool layered=LayeredItemPile.Instance&&LayeredItemPile.Instance.Running;
        var target=layered?LayeredItemPile.Instance.ReturnPosition():LevelGenerator.Instance.CreateRandomPosition();
        if(layered&&BoardEnvironment.UsesSoftContainer){
            var startRotation=obj.transform.rotation;
            if(display)obj.transform.rotation=display.boardRotation;
            target=SoftTransitionContainer.FittedPosition(obj,target);
            obj.transform.rotation=startRotation;
        }
        if(display)obj.transform.DORotateQuaternion(display.boardRotation,.35f).SetEase(Ease.OutQuad);
        obj.transform.DOMove(target, .35f).OnComplete(() => {
            if (!obj) return;
            var body = obj.GetComponent<Rigidbody>();
            if(layered&&BoardEnvironment.UsesSoftContainer){
                if(display)obj.transform.rotation=display.boardRotation;
                SoftTransitionContainer.Fit(obj);
                body.position=obj.transform.position;body.rotation=obj.transform.rotation;
            }
            body.isKinematic = false; body.detectCollisions = true;
            // Source pickups animate Transform in Update. Re-enabling interpolation here
            // lets the previous physics pose overwrite a later pickup's final tween pose.
            body.interpolation = UsesSourceFlight ? RigidbodyInterpolation.None : RigidbodyInterpolation.Interpolate;
            // Restore source per-shape enabled flags, not every collider indiscriminately.
            if(boardColliders.TryGetValue(obj,out var colliderStates)){
                foreach(var state in colliderStates)if(state.Key)state.Key.enabled=state.Value;
                boardColliders.Remove(obj);
            }
            if(ItemPhysicsController.Instance){ItemPhysicsController.Instance.Register(body);ItemPhysicsController.Instance.WakeNear(body.position);}
            if(LayeredItemPile.Instance)LayeredItemPile.Instance.Register(body);
        });
        Reposition();
        BoosterManager.Instance.CheckHintInteractibility();
    }
    public void ResetStack()
    {
        CancelPointer(); activeFinger = null;BoardTools.Reset();
        StopAllCoroutines(); ReturningToBowl=false; resolution = null; IsResolving = false;
        foreach (var item in objectsOnStack) if (item) item.transform.DOKill();
        foreach(var item in sourceCombining)if(item){item.transform.DOKill();ItemFlightMotion.Forget(item);GameAudioCatalog.Forget(item);BoardItemPool.Recycle(item);}
        foreach(var item in LevelGenerator.Instance.objectsOnScene)ItemFlightMotion.Forget(item);
        magnetFlights.Clear();probeFlights.Clear();ProbeLiftContact.Clear();PotProbeActivation.Clear();sourceCombining.Clear();sourceFlying.Clear();sourceMoves.Clear();sourceCompletionCheckReady=true;
        objectsOnStack.Clear(); arrivedObject.Clear(); scales.Clear(); boardRotations.Clear(); boardColliders.Clear(); pickHistory.Clear();
    }
}
