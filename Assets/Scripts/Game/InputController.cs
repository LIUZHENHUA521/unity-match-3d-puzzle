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
    public bool IsBusy => IsResolving || resolution != null || HasPendingVisuals;
    public bool UsesSourceFlight => ReplicaFlow.Instance && !ReplicaFlow.Instance.IsPvp && ReplicaFlow.Instance.Stage==2;
    public bool HasPendingVisuals => sourceFlying.Count>0 || sourceCombining.Count>0;
    public bool CanEvaluateWin => !UsesSourceFlight || sourceCompletionCheckReady;
    readonly HashSet<GameObject> sourceFlying=new HashSet<GameObject>();
    readonly HashSet<GameObject> sourceCombining=new HashSet<GameObject>();
    readonly Dictionary<GameObject,List<Tween>> sourceMoves=new Dictionary<GameObject,List<Tween>>();
    bool sourceCompletionCheckReady=true;
    public float LastFlightDuration {get;private set;}

    private readonly List<GameObject> pickHistory = new List<GameObject>();
    private readonly Dictionary<GameObject, Vector3> scales = new Dictionary<GameObject, Vector3>();
    private readonly Dictionary<GameObject, Dictionary<Collider,bool>> boardColliders = new Dictionary<GameObject, Dictionary<Collider,bool>>();
    private ReplicaSelectionFeedback selection;
    private void Awake() { Input.multiTouchEnabled = false; selection=gameObject.AddComponent<ReplicaSelectionFeedback>(); }
    public void CancelSelection(){CancelPointer();activeFinger=-1;}
    private void ClearPointerTarget(){pressedObject=null;if(selection)selection.Cancel();}
    private void CancelPointer(){ClearPointerTarget();pointerHeld=false;pointerStartedOnUI=false;}
    private void OnDisable(){CancelPointer();activeFinger=-1;}
    private void OnApplicationFocus(bool focus){if(!focus){CancelPointer();activeFinger=-1;}}
    private GameObject pressedObject;
    private Vector2 lastTargetPosition;
    private bool pointerHeld, pointerStartedOnUI;
    public int PointerStarts {get;private set;}
    public int PointerHits {get;private set;}
    public int AcceptedPicks {get;private set;}
    public string LastPickState {get;private set;}="idle";
    private int activeFinger = -1;
    private bool PointerOnButton(Vector2 position)
    {
        if (!EventSystem.current) return false;
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current) { position = position }, results);
        return results.Any(r => r.gameObject.GetComponentInParent<UnityEngine.UI.Selectable>() != null);
    }
    private GameObject PickExact(Camera camera, Vector2 position)
    {
        if (!camera.pixelRect.Contains(position) ||
            !Physics.Raycast(camera.ScreenPointToRay(position), out var hit, 100f,
                             Physics.DefaultRaycastLayers & ~((1<<ReplicaSourceColliders.BodyLayer)|(1<<ReplicaSourceColliders.WallLayer)), QueryTriggerInteraction.Ignore)) return null;
        var body = hit.collider.attachedRigidbody;
        return body && (!ReplicaLayeredPile.Instance || ReplicaLayeredPile.Instance.CanPick(body)) && body.CompareTag("Collectable") && (body.gameObject==pressedObject || ReplicaSourceTools.IsStored(body.gameObject) || !body.isKinematic || (ReplicaPilePhysics.Instance && ReplicaPilePhysics.Instance.Contains(body))) ? body.gameObject : null;
    }
    public GameObject PickAt(Vector2 screenPosition)
    {
        var camera = Camera.main;
        if (!camera || !camera.pixelRect.Contains(screenPosition)) return null;
        // WX CheckClickRay j13231 uses one ray. Nearby misses must not select another item.
        return PickExact(camera, screenPosition);
    }
    private void BeginPointer(Vector2 position)
    {
        CancelPointer();
        if(ReplicaSourceTools.Shuffling||(IsBusy&&!UsesSourceFlight)||objectsOnStack.Count>=7)return;
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
        if(!ReplicaSourceTools.IsStored(target))selection.Begin(target,Camera.main,!(ReplicaFlow.Instance&&ReplicaFlow.Instance.IsPvp));
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
        if (!GameManager.Instance.GameIsPlaying() || (ReplicaFlow.Instance && ReplicaFlow.Instance.BlocksGameplay)) {
            LastPickState="blocked";CancelPointer(); activeFinger = -1; return;
        }
        if (Input.touchCount > 0) {
            for (int i = 0; i < Input.touchCount; i++) {
                var touch = Input.GetTouch(i);
                if (touch.phase == TouchPhase.Began && activeFinger < 0) {
                    activeFinger = touch.fingerId; BeginPointer(touch.position);
                } else if (touch.fingerId == activeFinger) {
                    if (touch.phase == TouchPhase.Ended) { EndPointer(touch.position); activeFinger = -1; }
                    else if (touch.phase == TouchPhase.Canceled) { LastPickState="cancel";CancelPointer(); activeFinger = -1; }
                    else UpdatePointer(touch.position);
                }
            }
            return; // A touch must not also be consumed as an emulated mouse click.
        }
        if (activeFinger >= 0) { activeFinger = -1; CancelPointer(); return; }
        if (Input.GetMouseButtonDown(0)) BeginPointer(Input.mousePosition);
        if (Input.GetMouseButton(0)) UpdatePointer(Input.mousePosition);
        if (Input.GetMouseButtonUp(0)) EndPointer(Input.mousePosition);
    }
    public void AddObjectOnStack(GameObject obj,bool playSound=true)
    {
        if (ReplicaSourceTools.Shuffling || !GameManager.Instance.GameIsPlaying() || (IsResolving&&!UsesSourceFlight) || !obj || !obj.CompareTag("Collectable") ||
            objectsOnStack.Count >= 7 || objectsOnStack.Contains(obj)) return;
        if (ReplicaFlow.Instance && ReplicaFlow.Instance.InterceptPick(obj)) return;
        if(UsesSourceFlight&&playSound&&ReplicaLayeredPile.Instance)ReplicaLayeredPile.Instance.RecordSourceTouchHeight(obj.transform.position.y);
        if(selection&&selection.Selected==obj){if(UsesSourceFlight)selection.Commit();CancelPointer();}
        if(!UsesSourceFlight)ReplicaSourceTools.ReleaseStored(obj);
        AcceptedPicks++;LastPickState="picked";
        if(playSound&&!UsesSourceFlight&&ReplicaFlow.Instance)ReplicaFlow.Instance.SelectSound();
        if(UsesSourceFlight)obj.transform.DOKill(false);
        pickHistory.Add(obj);
        if(ReplicaLayeredPile.Instance)ReplicaLayeredPile.Instance.Detach(obj.GetComponent<Rigidbody>());
        scales[obj] = obj.transform.localScale;
        var body = obj.GetComponent<Rigidbody>();
        if (ReplicaPilePhysics.Instance) ReplicaPilePhysics.Instance.RemoveForPick(body);
        if(UsesSourceFlight){
            // BeClick only freezes the body. No temporary dynamic state, velocity clear,
            // interpolation change or neighbour wake is part of this source path.
            body.isKinematic=true;body.detectCollisions=true;
            obj.GetComponent<ReplicaSourceGooseAnimation>()?.EnterTray();
            if(playSound)ReplicaSourceAudio.Select(obj);
        }else{
            if(body.isKinematic)body.isKinematic=false;
            body.velocity=Vector3.zero;body.angularVelocity=Vector3.zero;
            body.detectCollisions=false;body.interpolation=RigidbodyInterpolation.None;body.isKinematic=true;
        }
        var colliderStates=new Dictionary<Collider,bool>();
        foreach (var collider in obj.GetComponentsInChildren<Collider>(true)){colliderStates[collider]=collider.enabled;if(!UsesSourceFlight)collider.enabled=false;}
        boardColliders[obj]=colliderStates;
        objectsOnStack.Add(obj);
        objectsOnStack = objectsOnStack.GroupBy(x => x.name).SelectMany(g => g).ToList();
        if(UsesSourceFlight){SourcePick(obj);ReplicaSourceTools.ReleaseStored(obj);return;}
        Reposition();
        if (resolution != null) StopCoroutine(resolution);
        resolution = StartCoroutine(Resolve());
    }
    private void SourcePick(GameObject obj)
    {
        // CheckLevelState runs at each EndCombine, not after every concurrent match has finished.
        // A later pickup invalidates an earlier completion checkpoint until a new callback runs.
        sourceCompletionCheckReady=false;
        LastFlightDuration=ReplicaSourceFlight.MoveSeconds;
        var initial=scales[obj];var target=ReplicaSourceFlight.PickScale(obj,initial);
        sourceFlying.Add(obj);
        // BeClick starts scale at release; RefreshContentDisplay starts all three axes immediately.
        obj.transform.DOScale(initial*ReplicaSourceFlight.ScalePeak,ReplicaSourceFlight.ScalePeakSeconds)
            .SetUpdate(true).SetEase(Ease.OutQuad).OnComplete(()=>{
                if(!obj)return;
                obj.transform.DOScale(target,ReplicaSourceFlight.MoveSeconds-ReplicaSourceFlight.ScalePeakSeconds)
                    .SetUpdate(true).SetEase(Ease.InSine).OnComplete(()=>{
                        if(!obj)return;
                        sourceFlying.Remove(obj);
                        if(objectsOnStack.Contains(obj)&&!arrivedObject.Contains(obj))arrivedObject.Add(obj);
                        obj.transform.DOPunchScale(initial*ReplicaSourceFlight.PunchScale,ReplicaSourceFlight.PunchSeconds,1,1);
                    });
            });
        StartCoroutine(SourceHideColliders(obj));
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
        // A seventh item which completes a triple has already freed its three logical slots.
        if(objectsOnStack.Count>=7)UIManager.Instance.ShowOutOfBoxesPopUp();
        BoosterManager.Instance.CheckHintInteractibility();
    }
    private IEnumerator SourceHideColliders(GameObject obj){
        yield return new WaitForSeconds(ReplicaSourceFlight.HideCollidersSeconds);
        if(!obj||(!objectsOnStack.Contains(obj)&&!sourceCombining.Contains(obj)))yield break;
        var body=obj.GetComponent<Rigidbody>();if(body)body.detectCollisions=false;
        foreach(var collider in obj.GetComponentsInChildren<Collider>(true))collider.enabled=false;
    }
    private void RepositionSource(){
        for(int i=0;i<objectsOnStack.Count;i++){
            var obj=objectsOnStack[i];if(!obj)continue;
            if(sourceMoves.TryGetValue(obj,out var old))foreach(var tween in old)if(tween.IsActive())tween.Kill();
            var slot=ReplicaTray.Instance?ReplicaTray.Instance.SlotPosition(i):new Vector3(-1+i*.333f,.3f,-1.62f);
            // RefreshContentDisplay applies the SAME .3 s to existing and incoming items.
            sourceMoves[obj]=new List<Tween>{
                obj.transform.DOMoveX(slot.x,ReplicaSourceFlight.MoveSeconds).SetUpdate(true).SetEase(Ease.Linear),
                obj.transform.DOMoveY(slot.y,ReplicaSourceFlight.MoveSeconds).SetUpdate(true).SetEase(Ease.OutQuad),
                obj.transform.DOMoveZ(slot.z,ReplicaSourceFlight.MoveSeconds).SetUpdate(true).SetEase(Ease.Linear),
                obj.transform.DORotateQuaternion(Camera.main?Camera.main.transform.rotation*Quaternion.Euler(-90,0,0):Quaternion.identity,ReplicaSourceFlight.MoveSeconds).SetUpdate(true).SetEase(Ease.InQuad)
            };
        }
    }
    private IEnumerator SourceCombine(GameObject[] items){
        // The original old branch waits .3, converges the two sides .2, shrinks the centre .2,
        // checks completion, then keeps the zero-sized centre another .05 before compacting.
        yield return new WaitForSeconds(ReplicaSourceFlight.MoveSeconds);
        if(!items[1])yield break;
        var center=items[1].transform.position;
        foreach(int i in new[]{0,2})if(items[i])items[i].transform.DOMove(center,ReplicaSourceFlight.MergeSeconds).SetEase(Ease.OutQuad);
        yield return new WaitForSeconds(ReplicaSourceFlight.MergeSeconds);
        RemoveSourceVisual(items[0]);RemoveSourceVisual(items[2]);
        if(items[1]){
            items[1].transform.DOScale(Vector3.zero,ReplicaSourceFlight.ShrinkSeconds).SetEase(Ease.InBack);
        }
        yield return new WaitForSeconds(ReplicaSourceFlight.ShrinkSeconds);
        ReplicaSourceAudio.Combine(items[1]);
        sourceCompletionCheckReady=true;
        yield return new WaitForSeconds(ReplicaSourceFlight.FinalRemovalDelay);
        RemoveSourceVisual(items[1]);
        RepositionSource();BoosterManager.Instance.CheckHintInteractibility();
    }
    private void RemoveSourceVisual(GameObject obj){
        if(!obj)return;
        obj.transform.DOKill();sourceFlying.Remove(obj);sourceCombining.Remove(obj);sourceMoves.Remove(obj);
        scales.Remove(obj);boardColliders.Remove(obj);ReplicaSourceFlight.Forget(obj);ReplicaSourceAudio.Forget(obj);
        // LevelObjectPool.Recycle keeps the ObjectUnit for a later Get(objName);
        // only the logical board list is removed at combine time.
        ReplicaSourceObjectPool.Recycle(obj);
    }
    private void Reposition()
    {
        if(UsesSourceFlight){RepositionSource();return;}
        for (int i=0; i<objectsOnStack.Count; i++) {
            var item = objectsOnStack[i];
            item.transform.DOKill();
            var slot = ReplicaTray.Instance ? ReplicaTray.Instance.SlotPosition(i) : new Vector3(-1+i*.333f, .3f, -1.62f);
            // Start travelling on release; selection already supplied the small lift.
            // Use a local easing approximation, not a recovered original animation curve.
            float moveTime = arrivedObject.Contains(item) ? .20f : .30f;
            item.transform.DOMove(slot, moveTime).SetEase(Ease.OutQuad);
            item.transform.DORotate(Vector3.zero, moveTime).SetEase(Ease.OutQuad);
            if (LevelGenerator.Instance.foodPile) {
                // Keep collected items within their slots; undo restores the enlarged board scale.
                float slotScale = ReplicaFlow.Instance ? .6f / ReplicaFlow.BoardItemScale : .6f;
                if(ReplicaTray.Instance)slotScale *= ReplicaTray.Instance.ItemScaleFactor;
                var renderers=item.GetComponentsInChildren<Renderer>();
                if(renderers.Length>0){
                    // Evaluate unrotated, full-scale bounds before tray tweening.
                    var savedRotation=item.transform.rotation;var savedScale=item.transform.localScale;
                    item.transform.rotation=Quaternion.identity;item.transform.localScale=scales[item];
                    var bounds=renderers[0].bounds;foreach(var r in renderers)bounds.Encapsulate(r.bounds);
                    float maxSize=Mathf.Max(bounds.size.x,bounds.size.y,bounds.size.z);
                    if(maxSize>0)slotScale=Mathf.Min(slotScale,.28f*(ReplicaTray.Instance?ReplicaTray.Instance.ItemScaleFactor:1)/maxSize);
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
                objectsOnStack.Remove(item); arrivedObject.Remove(item); scales.Remove(item); boardColliders.Remove(item); pickHistory.Remove(item);
                LevelGenerator.Instance.objectsOnScene.Remove(item); item.transform.DOKill(); Destroy(item);
            }
            if (ReplicaFlow.Instance) ReplicaFlow.Instance.MatchSound();
            Reposition();
            yield return new WaitForSeconds(.32f);
        }
        IsResolving = false;
        resolution = null;
        BoosterManager.Instance.CheckHintInteractibility();
        if (objectsOnStack.Count >= 7) UIManager.Instance.ShowOutOfBoxesPopUp();
    }
    public bool MoveSourceItemsToStore(int count=3)
    {
        if(!UsesSourceFlight||ReplicaSourceTools.Shuffling||!ReplicaSourceTools.CanUse(0)||!GameManager.Instance.GameIsPlaying())return false;
        var items=ReplicaSourceTools.ReturnCandidates(objectsOnStack,count);if(items.Count==0)return false;
        ReplicaSourceAudio.Tool(24);CancelPointer();
        foreach(var obj in items){
            obj.transform.DOKill();sourceMoves.Remove(obj);sourceFlying.Remove(obj);
            objectsOnStack.Remove(obj);arrivedObject.Remove(obj);pickHistory.Remove(obj);
            var body=obj.GetComponent<Rigidbody>();body.isKinematic=true;body.detectCollisions=true;
            if(boardColliders.TryGetValue(obj,out var flags))foreach(var flag in flags)if(flag.Key)flag.Key.enabled=flag.Value;
            obj.GetComponent<ReplicaSourceGooseAnimation>()?.EnterStore();
        }
        ReplicaSourceTools.Store(items);ReplicaSourceTools.MarkUsed(0);
        RepositionSource();BoosterManager.Instance.CheckHintInteractibility();return true;
    }
    public void UndoLast()
    {
        if (IsBusy || !GameManager.Instance.GameIsPlaying() || arrivedObject.Count == 0) return;
        var obj = pickHistory.Last();
        pickHistory.RemoveAt(pickHistory.Count-1);
        objectsOnStack.Remove(obj); arrivedObject.Remove(obj);
        obj.transform.DOKill();
        obj.transform.localScale = scales[obj]; scales.Remove(obj);
        obj.transform.DOMove(ReplicaLayeredPile.Instance && ReplicaLayeredPile.Instance.Running ? ReplicaLayeredPile.Instance.ReturnPosition() : LevelGenerator.Instance.CreateRandomPosition(), .35f).OnComplete(() => {
            if (!obj) return;
            var body = obj.GetComponent<Rigidbody>();
            body.isKinematic = false; body.detectCollisions = true;
            body.interpolation = RigidbodyInterpolation.Interpolate;
            // Restore source per-shape enabled flags, not every collider indiscriminately.
            if(boardColliders.TryGetValue(obj,out var colliderStates)){
                foreach(var state in colliderStates)if(state.Key)state.Key.enabled=state.Value;
                boardColliders.Remove(obj);
            }
            if(ReplicaPilePhysics.Instance){ReplicaPilePhysics.Instance.Register(body);ReplicaPilePhysics.Instance.WakeNear(body.position);}
            if(ReplicaLayeredPile.Instance)ReplicaLayeredPile.Instance.Register(body);
        });
        Reposition();
        BoosterManager.Instance.CheckHintInteractibility();
    }
    public void ResetStack()
    {
        CancelPointer(); activeFinger = -1;ReplicaSourceTools.Reset();
        StopAllCoroutines(); resolution = null; IsResolving = false;
        foreach (var item in objectsOnStack) if (item) item.transform.DOKill();
        foreach(var item in sourceCombining)if(item){item.transform.DOKill();ReplicaSourceFlight.Forget(item);ReplicaSourceAudio.Forget(item);ReplicaSourceObjectPool.Recycle(item);}
        foreach(var item in LevelGenerator.Instance.objectsOnScene)ReplicaSourceFlight.Forget(item);
        sourceCombining.Clear();sourceFlying.Clear();sourceMoves.Clear();sourceCompletionCheckReady=true;
        objectsOnStack.Clear(); arrivedObject.Clear(); scales.Clear(); boardColliders.Clear(); pickHistory.Clear();
    }
}
