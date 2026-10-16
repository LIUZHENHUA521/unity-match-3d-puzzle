using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using DG.Tweening;
using UnityEngine;

// WX ToolReturnBack / ToolAutoFind: j38482, j13232, j19860, j19862, j19259.
// Acquisition/advertising remain local-only; native tool visuals are reconstructed separately.
public sealed class ReplicaSourceTools : MonoBehaviour {
    public const int ReturnCount=3;
    public const float StoreSeconds=.3f,StoreScaleSeconds=.2f;
    // WX metadata field defaults: 3=OutSine, 26=InBack, 27=OutBack.
    public const Ease StoreXEase=Ease.OutSine,StoreYEase=Ease.InBack,StoreZEase=Ease.OutBack,StoreScaleEase=Ease.InBack;
    static ReplicaSourceTools instance;
    readonly List<GameObject>[] columns={new List<GameObject>(),new List<GameObject>(),new List<GameObject>()};
    int nextColumn;
    readonly int[] used=new int[3];
    public static bool CanUse(int type)=>type>=0&&type<3&&(!instance||instance.used[type]<1);
    public static void MarkUsed(int type){var owner=Ensure();if(owner&&type>=0&&type<3)owner.used[type]++;}
    public static int UsedCount(int type)=>instance&&type>=0&&type<3?instance.used[type]:0;
    bool shuffling;
    readonly List<ShuffleItem> moving=new List<ShuffleItem>();
    public static bool Shuffling=>instance&&instance.shuffling;
    sealed class ShuffleItem { public GameObject obj;public Rigidbody body;public Vector3 target;public float speed,distance;public Dictionary<Collider,bool> colliders; }
    public static bool Enabled=>InputController.Instance&&InputController.Instance.UsesSourceFlight;
    public static int StoredCount=>instance?instance.columns.Sum(x=>x.Count(o=>o)):0;
    public static bool IsStored(GameObject obj)=>obj&&instance&&instance.columns.Any(x=>x.Contains(obj));
    static ReplicaSourceTools Ensure(){
        if(!instance&&ReplicaFlow.Instance)instance=ReplicaFlow.Instance.gameObject.AddComponent<ReplicaSourceTools>();
        return instance;
    }
    void Awake(){instance=this;}
    void OnDestroy(){ClearStore();if(instance==this)instance=null;}
    public static void Reset(){ReplicaSourceToolVisual.Reset();if(instance){instance.StopAllCoroutines();instance.EndShuffle();instance.ClearStore();Array.Clear(instance.used,0,instance.used.Length);}}
    void ClearStore(){foreach(var col in columns){foreach(var obj in col)if(obj){obj.transform.DOKill();ReplicaSourceObjectPool.Recycle(obj);}col.Clear();}nextColumn=0;}

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
            if(StoredCount==0)ReplicaSourceToolVisual.SetStoreVisible(true);
            owner.columns[owner.nextColumn].Add(obj);
            owner.LayoutColumn(owner.nextColumn);
            owner.nextColumn=(owner.nextColumn+1)%owner.columns.Length;
            obj.transform.DOScale(ReplicaSourceFlight.StoreScale(obj,obj.transform.localScale),StoreScaleSeconds).SetEase(StoreScaleEase);
            ReplicaSourceToolVisual.Return(obj);
        }
    }
    public static void ReleaseStored(GameObject obj){
        if(!instance||!obj)return;
        for(int i=0;i<instance.columns.Length;i++)if(instance.columns[i].Remove(obj)){instance.LayoutColumn(i);if(StoredCount==0)ReplicaSourceToolVisual.SetStoreVisible(false);break;}
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
        var trayCenter=ReplicaTray.Instance?ReplicaTray.Instance.SlotPosition(3):camera.transform.TransformPoint(0,-5.04f,1.58f);
        var screen=camera.WorldToScreenPoint(trayCenter);screen.z=storePointDepth;
        var result=camera.ScreenToWorldPoint(screen);
        float x=(column==0?-.83f:column==1?.01f:.83f)*contentScale;
        float localStack=.75f*(1-count+index);
        float stagger=index==0?0:(index%2==1?-.1f:.1f);
        result+=camera.transform.right*(x+stagger*contentScale);
        // Content rotation maps StorePointList local +Y to camera -forward.
        result-=camera.transform.forward*(localStack*contentScale);
        result+=camera.transform.up*(storePointY*contentScale);
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
        if(!Enabled||!CanUse(2)||Shuffling||!GameManager.Instance.GameIsPlaying())return false;
        var owner=Ensure();if(!owner)return false;
        // j65101 uses the monotone maxHeight after CheckObjMaxHeight, not a fresh upward peak.
        float? peak=ReplicaLayeredPile.Instance&&ReplicaLayeredPile.Instance.Running?(float?)ReplicaLayeredPile.Instance.RefreshSourceMaxHeight():null;
        var candidates=ShuffleCandidates(LevelGenerator.Instance.objectsOnScene,InputController.Instance.objectsOnStack,peak);
        if(candidates.Count==0)return false;
        MarkUsed(2);owner.StartCoroutine(owner.Shuffle(candidates));return true;
    }
    IEnumerator Shuffle(List<GameObject> candidates){
        shuffling=true;InputController.Instance.CancelSelection();ReplicaSourceToolVisual.MessUp();
        var targets=candidates.Select(x=>x.transform.position).ToList();
        ReorderForShuffle(candidates,(a,b)=>UnityEngine.Random.Range(a,b));
        targets.Sort((a,b)=>a.y.CompareTo(b.y));
        Vector3 axis=ShuffleAxis(ReplicaSourceEnvironment.CurrentName);
        float maxDistance=0;
        for(int i=0;i<candidates.Count;i++){
            var obj=candidates[i];var body=obj.GetComponent<Rigidbody>();
            var item=new ShuffleItem{obj=obj,body=body,target=targets[i],speed=UnityEngine.Random.Range(540f,720f),
                distance=Vector3.Cross(obj.transform.position,axis).magnitude,
                colliders=obj.GetComponentsInChildren<Collider>(true).ToDictionary(c=>c,c=>c.enabled)};
            moving.Add(item);maxDistance=Mathf.Max(maxDistance,item.distance);
            if(body)body.isKinematic=true;foreach(var c in item.colliders.Keys)c.enabled=false;
        }
        float elapsed=0;
        do{
            elapsed+=Time.deltaTime;yield return null;
            foreach(var item in moving)if(item.obj){
                float ratio=maxDistance>0?item.distance/maxDistance:0;
                item.obj.transform.RotateAround(Vector3.zero,axis,item.speed*Time.deltaTime*Mathf.Lerp(1.2f,.15f,ratio));
                var pos=item.obj.transform.position;pos.y=Mathf.Lerp(pos.y,item.target.y,Time.deltaTime*2);item.obj.transform.position=pos;
            }
        }while(elapsed<=1f);
        foreach(var item in moving)if(item.obj){
            foreach(var c in item.colliders)if(c.Key)c.Key.enabled=c.Value;
            item.obj.transform.DOMove(item.target,.3f).SetUpdate(true);
        }
        yield return new WaitForSeconds(.3f);
        foreach(var item in moving)if(item.obj){item.obj.transform.DOKill();item.obj.transform.position=item.target;}
        EndShuffle();
    }
    void EndShuffle(){
        foreach(var item in moving){
            if(item.obj)item.obj.transform.DOKill();
            foreach(var c in item.colliders)if(c.Key)c.Key.enabled=c.Value;
            // Source ends per candidate: scale one, CheckVisible, then CheckRigid(depth=1).
            if(item.body&&ReplicaLayeredPile.Instance)ReplicaLayeredPile.Instance.FinishSourceShuffle(item.body);
        }
        moving.Clear();shuffling=false;
    }
    void LayoutColumn(int column){
        var items=columns[column];items.RemoveAll(x=>!x);
        for(int i=0;i<items.Count;i++){
            var obj=items[i];obj.transform.DOKill();var pos=StorePosition(column,i,items.Count);
            obj.transform.DOMoveX(pos.x,StoreSeconds).SetEase(StoreXEase);
            obj.transform.DOMoveY(pos.y,StoreSeconds).SetEase(StoreYEase);
            obj.transform.DOMoveZ(pos.z,StoreSeconds).SetEase(StoreZEase);
        }
    }
}
