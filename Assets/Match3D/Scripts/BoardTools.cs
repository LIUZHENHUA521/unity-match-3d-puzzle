using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using DG.Tweening;
using UnityEngine;

// WX ToolReturnBack / ToolAutoFind: j38482, j13232, j19860, j19862, j19259.
// Acquisition/advertising remain local-only; invitation inventory is authorized by Match3DInvitations.
public sealed class BoardTools : MonoBehaviour {
    public const int ToolCount=4,AddTimeType=3,AddTimeSeconds=180;
    public static bool AddTime(){
        var flow=Match3DFlow.Instance;var game=GameManager.Instance;
        if(!Enabled||!flow||flow.IsPvp||flow.Stage==1||!game||!game.GameIsPlaying()||game.currentLevelTime<=0||!CanUse(AddTimeType))return false;
        if(!TrySpend(AddTimeType))return false;
        game.currentLevelTime+=AddTimeSeconds;flow.ShowTips("时间 +3 分钟");return true;
    }
    public const int ReturnCount=3;
    public const float StoreSeconds=.36f,StoreScaleSeconds=.2f;
    // WX metadata field defaults: 3=OutSine, 26=InBack, 27=OutBack.
    public const Ease StoreXEase=Ease.OutSine,StoreYEase=Ease.InBack,StoreZEase=Ease.OutBack,StoreScaleEase=Ease.InBack;
    static BoardTools instance;
    readonly List<GameObject>[] columns={new List<GameObject>(),new List<GameObject>(),new List<GameObject>()};
    int nextColumn;
    readonly int[] used=new int[ToolCount];
    readonly int[] adUses=new int[ToolCount];
    readonly int[] testUses=new int[ToolCount];
    readonly int[] shareUses=new int[ToolCount],freeUses=new int[ToolCount];
    public static int ShareUses(int type)=>instance?instance.shareUses[type]:0;
    public static int FreeUses(int type)=>instance?instance.freeUses[type]:0;
    public static void RecordAcquisition(int type,Match3DRewardAcquisition.Route route){
        var owner=Ensure();if(!owner||type<0||type>=ToolCount)return;
        if(route==Match3DRewardAcquisition.Route.Ad)owner.adUses[type]++;
        else if(route==Match3DRewardAcquisition.Route.Share)owner.shareUses[type]++;
        else if(route==Match3DRewardAcquisition.Route.Free)owner.freeUses[type]++;
    }
    public static void RecordTestUse(int type){var owner=Ensure();if(owner&&type>=0&&type<ToolCount)owner.testUses[type]++;}
    public static int TestUses(int type)=>instance&&type>=0&&type<ToolCount?instance.testUses[type]:0;
    public static void RecordAdUse(int type){var owner=Ensure();if(owner&&type>=0&&type<ToolCount)owner.adUses[type]++;}
    public static int AdUses(int type)=>instance?instance.adUses[type]:0;
    static int paidType=-1;
    public static int Remaining(int type)=>(Match3DInvitations.Instance?Match3DInvitations.Instance.Inventory(type):0);
    public static bool CanUse(int type)=>type>=0&&type<ToolCount&&(Match3DTestPrivileges.UnlimitedTools||paidType==type||(Match3DInvitations.Instance&&!Match3DInvitations.Instance.Busy&&!Match3DInvitations.Instance.HasPending&&Match3DInvitations.Instance.Inventory(type)>0));
    public static bool TrySpend(int type){if(paidType!=type)return false;MarkUsed(type);return true;}
    public static bool RunPaid(int type,Action action){int before=UsedCount(type);paidType=type;try{action();return UsedCount(type)>before;}finally{paidType=-1;}}
    public static void MarkUsed(int type){var owner=Ensure();if(owner&&type>=0&&type<ToolCount)owner.used[type]++;}
    public static int UsedCount(int type)=>instance&&type>=0&&type<ToolCount?instance.used[type]:0;
    bool shuffling;
    readonly List<ShuffleItem> moving=new List<ShuffleItem>();
    public static bool Shuffling=>instance&&instance.shuffling;
    sealed class ShuffleItem { public GameObject obj;public Rigidbody body;public Vector3 start,target;public Quaternion rotation,tilt;public float delay,lift;public bool wasKinematic;public Dictionary<Collider,bool> colliders; }
    public static bool Enabled=>InputController.Instance&&InputController.Instance.UsesSourceFlight;
    public static int StoredCount=>instance?instance.columns.Sum(x=>x.Count(o=>o)):0;
    public static bool IsStored(GameObject obj)=>obj&&instance&&instance.columns.Any(x=>x.Contains(obj));
    static BoardTools Ensure(){
        if(!instance&&Match3DFlow.Instance)instance=Match3DFlow.Instance.gameObject.AddComponent<BoardTools>();
        return instance;
    }
    void Awake(){instance=this;}
    void OnDestroy(){ClearStore();if(instance==this)instance=null;}
    public static void Reset(){Match3DRewardAcquisition.ResetRound();BoardToolVisual.Reset();if(instance){instance.StopAllCoroutines();instance.EndShuffle();instance.ClearStore();Array.Clear(instance.used,0,instance.used.Length);Array.Clear(instance.adUses,0,instance.adUses.Length);Array.Clear(instance.testUses,0,instance.testUses.Length);Array.Clear(instance.shareUses,0,instance.shareUses.Length);Array.Clear(instance.freeUses,0,instance.freeUses.Length);}}
    void ClearStore(){foreach(var col in columns){foreach(var obj in col)if(obj){obj.transform.DOKill();BoardItemPool.Recycle(obj);}col.Clear();}nextColumn=0;}

    public static List<GameObject> ReturnCandidates(IList<GameObject> stack,int count=ReturnCount)=>stack.Where(x=>x).Take(Mathf.Max(0,count)).ToList();
    // Source uses distinct existing tray types, not the most numerous tray group.
    // At six occupied slots, only the first adjacent pair can safely be completed.
    public static List<GameObject> AutoFindCandidates(IList<GameObject> stack,IList<GameObject> board,Func<int,int,int> random=null){
        var result=new List<GameObject>();if(stack.Count==0||stack.Count>=7)return result;
        string type=null;
        if(stack.Count==6){
            for(int i=1;i<stack.Count;i++)if(stack[i-1]&&stack[i]&&stack[i-1].name==stack[i].name){type=stack[i].name;break;}
            if(type==null)return result;
        }else{
            var types=stack.Where(x=>x).Select(x=>x.name).Distinct().ToList();if(types.Count==0)return result;
            if(random==null)random=(a,b)=>UnityEngine.Random.Range(a,b);
            type=types[random(0,types.Count)];
        }
        int needed=3-stack.Count(x=>x&&x.name==type);
        for(int i=board.Count-1;i>=0&&result.Count<needed;i--){var obj=board[i];if(obj&&obj.name==type&&!stack.Contains(obj))result.Add(obj);}
        // A malformed custom level cannot be allowed to consume the tool or partially fill a tray.
        if(result.Count!=needed)result.Clear();
        return result;
    }
    public static bool CanAutoFind(IList<GameObject> stack){
        if(stack.Count==0||stack.Count>=7)return false;
        if(stack.Count<6)return true;
        for(int i=1;i<stack.Count;i++)if(stack[i-1]&&stack[i]&&stack[i-1].name==stack[i].name)return true;
        return false;
    }
    public static void Store(IEnumerable<GameObject> objects){
        var owner=Ensure();if(!owner)return;
        foreach(var obj in objects){
            if(!obj||IsStored(obj))continue;
            // UseTool removes each item from content and appends it to _boxObjList.
            // Keep that order so a later AutoFind reverse scan can select stored items first.
            if(LevelGenerator.Instance){var board=LevelGenerator.Instance.objectsOnScene;board.Remove(obj);board.Add(obj);}
            if(StoredCount==0)BoardToolVisual.SetStoreVisible(true);
            owner.columns[owner.nextColumn].Add(obj);
            owner.LayoutColumn(owner.nextColumn,obj);
            owner.nextColumn=(owner.nextColumn+1)%owner.columns.Length;
            obj.transform.DOScale(ItemFlightMotion.StoreScale(obj,obj.transform.localScale),StoreScaleSeconds).SetEase(StoreScaleEase);
        }
    }
    public static void ReleaseStored(GameObject obj){
        if(!instance||!obj)return;
        for(int i=0;i<instance.columns.Length;i++)if(instance.columns[i].Remove(obj)){instance.LayoutColumn(i);if(StoredCount==0)BoardToolVisual.SetStoreVisible(false);break;}
    }
    public static Vector3 StorePosition(int column,int index,int count){
        var camera=Camera.main;if(!camera)return Vector3.zero;
        // WX data.unity3d Content scale is 1.4. StorePointList is at local
        // (0,-.46,1.55), with three points at local X (-.83,.01,.83).
        // SetStoreContentPos then walks the point's local up vector by .75 per
        // item (the Content is rotated -90deg around X, so this is camera
        // depth), and applies a small alternating .1 local offset.
        const float contentScale=1.4f;
        const float storePointY=1.55f;
        const float storePointDepth=2f + .46f*contentScale;
        var trayCenter=Match3DTray.Instance?Match3DTray.Instance.SlotPosition(3):camera.transform.TransformPoint(0,-5.04f,1.58f);
        var screen=camera.WorldToScreenPoint(trayCenter);screen.z=storePointDepth;
        var result=camera.ScreenToWorldPoint(screen);
        float x=(column==0?-.83f:column==1?.01f:.83f)*contentScale;
        float localStack=.75f*(1-count+index);
        float stagger=index==0?0:(index%2==1?-.1f:.1f);
        result+=camera.transform.right*(x+stagger*contentScale);
        // Content rotation maps StorePointList local +Y to camera -forward.
        result-=camera.transform.forward*(localStack*contentScale);
        result+=camera.transform.up*(storePointY*contentScale)+BoardToolVisual.StoreLayoutOffset(camera);
        return result;
    }

    public static Vector3 ShuffleAxis(string theme)=>Quaternion.Euler(theme=="GroceryStore"?-6.5f:-7f,0,0)*Vector3.up;
    public static List<GameObject> ShuffleCandidates(IEnumerable<GameObject> board,IList<GameObject> stack,float? recordedMaxHeight=null){
        var eligible=board.Where(x=>x&&!stack.Contains(x)&&!IsStored(x)).ToList();
        float peak=recordedMaxHeight??Mathf.Max(0,eligible.Select(x=>x.transform.position.y).DefaultIfEmpty(0).Max());
        return eligible.Where(x=>x.transform.position.y>peak-7f).ToList();
    }
    // This is deliberately not Fisher-Yates: one Random.Range(0,N), remove, append per object.
    public static void ReorderForShuffle<T>(List<T> items,Func<int,int,int> random){
        for(int i=0;i<items.Count;i++){int index=random(0,items.Count);var value=items[index];items.RemoveAt(index);items.Add(value);}
    }
    public static bool MessUp(){
        if(ItemPhysicsController.Instance)ItemPhysicsController.Instance.ReleaseDailyOpening();
        if(!Enabled||!CanUse(2)||Shuffling||!GameManager.Instance.GameIsPlaying())return false;
        var owner=Ensure();if(!owner)return false;
        // j65101 uses the monotone maxHeight after CheckObjMaxHeight, not a fresh upward peak.
        float? peak=LayeredItemPile.Instance&&LayeredItemPile.Instance.Running?(float?)LayeredItemPile.Instance.RefreshSourceMaxHeight():null;
        var candidates=ShuffleCandidates(LevelGenerator.Instance.objectsOnScene,InputController.Instance.objectsOnStack,peak);
        if(candidates.Count==0)return false;
        if(!TrySpend(2))return false;owner.StartCoroutine(owner.Shuffle(candidates));return true;
    }
    IEnumerator Shuffle(List<GameObject> candidates){
        shuffling=true;InputController.Instance.CancelSelection();
        var targets=candidates.Select(x=>x.transform.position).ToList();
        ReorderForShuffle(candidates,(a,b)=>UnityEngine.Random.Range(a,b));
        targets.Sort((a,b)=>a.y.CompareTo(b.y));
        for(int i=0;i<candidates.Count;i++){
            var obj=candidates[i];var body=obj.GetComponent<Rigidbody>();
            // Deterministic variation avoids consuming gameplay randomness for decoration.
            var item=new ShuffleItem{obj=obj,body=body,start=obj.transform.position,target=targets[i],
                rotation=obj.transform.rotation,tilt=Quaternion.Euler(i%2==0?12f:-12f,0,i%3==0?9f:-9f),
                delay=(i%4)*.025f,lift=.45f+(i%3)*.12f,wasKinematic=body&&body.isKinematic,
                colliders=obj.GetComponentsInChildren<Collider>(true).ToDictionary(c=>c,c=>c.enabled)};
            moving.Add(item);obj.transform.DOKill();
            if(body){if(!body.isKinematic){body.velocity=Vector3.zero;body.angularVelocity=Vector3.zero;}body.isKinematic=true;}
            foreach(var c in item.colliders.Keys)c.enabled=false;
        }
        float elapsed=0;
        while(elapsed<1.075f){
            foreach(var item in moving)if(item.obj){
                float t=Mathf.Clamp(elapsed-item.delay,0,1),height,tilt;
                Vector3 position;
                if(t<.15f){
                    float u=t/.15f;u=1-(1-u)*(1-u);
                    position=item.start;height=item.lift*u;tilt=u;
                }else if(t<.7f){
                    float u=(t-.15f)/.55f;float eased=u*u*(3-2*u);
                    position=Vector3.Lerp(item.start,item.target,eased);
                    height=item.lift+Mathf.Sin(u*Mathf.PI)*item.lift*.6f;tilt=1;
                }else{
                    float u=(t-.7f)/.3f;
                    position=item.target;
                    // Touch down, then a small upward bounce; never dip below the target.
                    height=u<.7f?item.lift*(1-(u/.7f)*(u/.7f)):
                        Mathf.Sin((u-.7f)/.3f*Mathf.PI)*item.lift*.1f;
                    tilt=1-u*u*(3-2*u);
                }
                item.obj.transform.position=position+Vector3.up*Mathf.Max(0,height);
                item.obj.transform.rotation=item.rotation*Quaternion.Slerp(Quaternion.identity,item.tilt,tilt);
            }
            yield return null;elapsed+=Time.deltaTime;
        }
        foreach(var item in moving)if(item.obj){item.obj.transform.position=item.target;item.obj.transform.rotation=item.rotation;}
        EndShuffle();
    }
    void EndShuffle(){
        foreach(var item in moving){
            if(item.obj){item.obj.transform.DOKill();item.obj.transform.rotation=item.rotation;}
            if(item.obj&&item.body&&LayeredItemPile.Instance&&LayeredItemPile.Instance.Running&&BoardEnvironment.UsesSoftContainer){
                LayeredItemPile.Instance.RestoreToolScale(item.body);
                SoftTransitionContainer.Fit(item.obj);
                item.body.position=item.obj.transform.position;
            }
            if(item.body)item.body.isKinematic=item.wasKinematic;
            foreach(var c in item.colliders)if(c.Key)c.Key.enabled=c.Value;
            // Source ends per candidate: scale one, CheckVisible, then CheckRigid(depth=1).
            if(item.body&&LayeredItemPile.Instance)LayeredItemPile.Instance.FinishSourceShuffle(item.body);
        }
        moving.Clear();shuffling=false;
    }
    void LayoutColumn(int column,GameObject newlyMoved=null){
        var items=columns[column];items.RemoveAll(x=>!x);
        for(int i=0;i<items.Count;i++){
            var obj=items[i];obj.transform.DOKill();var pos=StorePosition(column,i,items.Count);
            if(obj==newlyMoved){
                // Queue the moved objects visibly: a short side-slide first, then a
                // clean handoff into the storage slot. The stagger makes the three
                // objects read as a deliberate sequence instead of one clump.
                var camera=Camera.main;var side=camera?camera.transform.right*.72f:Vector3.right*.72f;
                var handoff=obj.transform.position+side+Vector3.up*.08f;
                var sequence=DOTween.Sequence().SetUpdate(true);
                sequence.AppendInterval(i*.055f);
                sequence.Append(obj.transform.DOMove(handoff,.12f).SetEase(Ease.OutQuad));
                sequence.Append(obj.transform.DOMove(pos,.24f).SetEase(Ease.InOutSine));
                var settleRotation=obj.transform.rotation*Quaternion.Euler(0,0,(i%2==0?7f:-7f));
                sequence.Join(obj.transform.DORotateQuaternion(settleRotation,.16f).SetEase(Ease.OutQuad));
            }else{
                obj.transform.DOMoveX(pos.x,StoreSeconds).SetEase(StoreXEase);
                obj.transform.DOMoveY(pos.y,StoreSeconds).SetEase(StoreYEase);
                obj.transform.DOMoveZ(pos.z,StoreSeconds).SetEase(StoreZEase);
            }
        }
    }
}
