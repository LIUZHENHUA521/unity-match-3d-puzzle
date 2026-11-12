using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Deterministic opening repair while the board is held. Preserve clean poses;
// repair a dirty layout without changing size, count or gameplay RNG.
public sealed class BoardOpeningSeparation {
    public const float Tolerance=.01f;
    const float Clearance=.003f;
    sealed class Shape {
        public Collider collider;public Vector3 offset,baseOffset;public Quaternion rotation,baseRotation;public Bounds bounds,baseBounds;
    }
    sealed class Item {
        public GameObject obj;public Rigidbody body;public int id;public bool visible;
        public Vector3 original,position;public Quaternion originalRotation,rotation;public Bounds bounds;public Shape[] shapes;
    }
    readonly List<Item> collidable=new List<Item>();
    readonly List<Item> obstacles=new List<Item>(),moving=new List<Item>();
    readonly List<Shape> walls=new List<Shape>();
    readonly bool firstStage;float minimumVisibleY;
    int cursor,attempt=-1;bool complete;Match3DDailyRandom random;
    public int MovedCount {get;private set;}
    public bool Complete=>complete;
    static Shape Read(Collider c,Vector3 origin)=>new Shape{collider=c,offset=c.transform.position-origin,rotation=c.transform.rotation,bounds=c.bounds,baseOffset=c.transform.position-origin,baseRotation=c.transform.rotation,baseBounds=c.bounds};
    public BoardOpeningSeparation(IEnumerable<GameObject> objects,Func<Rigidbody,bool> visible,bool firstStage,float visibleThreshold=float.NegativeInfinity){
        this.firstStage=firstStage;Physics.SyncTransforms();
        foreach(var obj in objects.Where(o=>o&&o.activeInHierarchy)){
            var body=obj.GetComponent<Rigidbody>();if(!body)continue;
            var cols=obj.GetComponentsInChildren<Collider>().Where(c=>c.enabled&&!c.isTrigger&&c.gameObject.layer==ItemColliderGeometry.BodyLayer).ToArray();
            if(cols.Length==0)throw new InvalidOperationException("Missing opening body: "+obj.name);
            var bounds=cols[0].bounds;foreach(var c in cols)bounds.Encapsulate(c.bounds);
            var item=new Item{obj=obj,body=body,id=obj.GetComponent<Match3DTile>()?.id??0,visible=visible(body),original=obj.transform.position,position=obj.transform.position,originalRotation=obj.transform.rotation,rotation=obj.transform.rotation,bounds=bounds,shapes=cols.Select(c=>Read(c,obj.transform.position)).ToArray()};
            (item.visible?moving:obstacles).Add(item);
        }
        minimumVisibleY=float.IsNegativeInfinity(visibleThreshold)?moving.Select(m=>m.original.y).DefaultIfEmpty(0).Min():visibleThreshold;
        moving.Sort((a,b)=>{int order=b.bounds.size.sqrMagnitude.CompareTo(a.bounds.size.sqrMagnitude);return order!=0?order:a.id.CompareTo(b.id);});
        obstacles.Sort((a,b)=>a.id.CompareTo(b.id));collidable.AddRange(obstacles);
        foreach(var c in UnityEngine.Object.FindObjectsOfType<Collider>())
            if(c.enabled&&!c.isTrigger&&!c.attachedRigidbody&&(c.gameObject.layer==ItemColliderGeometry.WallLayer||c.gameObject.layer==0))walls.Add(Read(c,Vector3.zero));
        // Stable hierarchy order also makes ties independent of FindObjects order.
        walls.Sort((a,b)=>string.CompareOrdinal(Path(a.collider.transform),Path(b.collider.transform)));
    }
    static string Path(Transform t)=>t.parent?Path(t.parent)+"/"+t.GetSiblingIndex()+":"+t.name:t.GetSiblingIndex()+":"+t.name;
    static void Orient(Item item,Quaternion rotation){
        item.rotation=rotation;var delta=rotation*Quaternion.Inverse(item.originalRotation);
        var x=delta*Vector3.right;var y=delta*Vector3.up;var z=delta*Vector3.forward;
        x=new Vector3(Mathf.Abs(x.x),Mathf.Abs(x.y),Mathf.Abs(x.z));y=new Vector3(Mathf.Abs(y.x),Mathf.Abs(y.y),Mathf.Abs(y.z));z=new Vector3(Mathf.Abs(z.x),Mathf.Abs(z.y),Mathf.Abs(z.z));
        foreach(var shape in item.shapes){
            shape.offset=delta*shape.baseOffset;shape.rotation=delta*shape.baseRotation;
            shape.bounds=new Bounds(item.original+delta*(shape.baseBounds.center-item.original),2*(x*shape.baseBounds.extents.x+y*shape.baseBounds.extents.y+z*shape.baseBounds.extents.z));
        }
        item.bounds=item.shapes[0].bounds;foreach(var shape in item.shapes)item.bounds.Encapsulate(shape.bounds);
    }
    static Bounds At(Item item,Vector3 p){var bounds=item.bounds;bounds.center+=p-item.original;return bounds;}
    static Bounds At(Shape shape,Vector3 delta){var bounds=shape.bounds;bounds.center+=delta;return bounds;}
    bool Contact(Item item,Vector3 position,out Vector3 correction){
        float deepest=Tolerance;correction=Vector3.zero;var bounds=At(item,position);var delta=position-item.original;
        foreach(var other in collidable){
            if(other==item)continue;
            if(!bounds.Intersects(At(other,other.position)))continue;
            foreach(var a in item.shapes)foreach(var b in other.shapes){
                if(!At(a,delta).Intersects(At(b,other.position-other.original)))continue;
                if(Physics.ComputePenetration(a.collider,position+a.offset,a.rotation,b.collider,other.position+b.offset,b.rotation,out var direction,out float depth)&&depth>deepest){deepest=depth;correction=direction*(depth+Clearance);}
            }
        }
        foreach(var wall in walls){
            if(!bounds.Intersects(wall.bounds))continue;
            foreach(var a in item.shapes)if(At(a,delta).Intersects(wall.bounds)&&Physics.ComputePenetration(a.collider,position+a.offset,a.rotation,wall.collider,wall.offset,wall.rotation,out var direction,out float depth)&&depth>deepest){deepest=depth;correction=direction*(depth+Clearance);}
        }
        return deepest>Tolerance;
    }
    Vector3 Fit(Item item,Vector3 position){
        if(firstStage){
            var bounds=At(item,position);if(bounds.min.y<.5f)position.y+=.5f-bounds.min.y;
            var xz=Vector2.ClampMagnitude(new Vector2(position.x,position.z),FirstStageContainer.HorizontalHalfExtent);return new Vector3(xz.x,position.y,xz.y);
        }
        if(SoftTransitionContainer.Contains(position,.04f))return position;
        var local=SoftTransitionContainer.ToLocal(position);
        local.y=Mathf.Clamp(local.y,.05f,SoftTransitionContainer.Height-.05f);
        float radius=SoftTransitionContainer.Apothem(local.y)-.05f;
        var horizontal=Vector2.ClampMagnitude(new Vector2(local.x,local.z),radius);
        local.x=horizontal.x;local.z=horizontal.y;return SoftTransitionContainer.ToWorld(local);
    }
    bool InContainer(Vector3 position)=>firstStage?(position.y>=0&&position.y<=10&&new Vector2(position.x,position.z).magnitude<=FirstStageContainer.HorizontalHalfExtent+.001f):SoftTransitionContainer.Contains(position);
    bool Inside(Item item,Vector3 position){
        if(position.y<minimumVisibleY-.0001f||!InContainer(position))return false;
        // A root inside the shell does not guarantee an eccentric child is inside.
        // Each convex primitive needs an interior center AND no wall penetration;
        // together these prevent candidates jumping completely across a thin wall.
        foreach(var shape in item.shapes)if(!InContainer(shape.bounds.center+position-item.original))return false;
        return true;
    }
    bool TryPosition(Item item,Vector3 position,int iterations,out Vector3 result){
        result=position;
        for(int i=0;i<iterations;i++){
            result.y=Mathf.Max(minimumVisibleY,result.y);result=Fit(item,result);
            if(!Contact(item,result,out var correction))return Inside(item,result);
            result+=correction;
        }
        result.y=Mathf.Max(minimumVisibleY,result.y);result=Fit(item,result);return Inside(item,result)&&!Contact(item,result,out _);
    }
    // Each step has a fixed geometric work limit; caller time budgets only
    // decide when to yield, never which candidate is selected.
    public void Step(){
        if(Complete)return;
        if(cursor==moving.Count){complete=true;return;}
        var item=moving[cursor];Vector3 result=item.original;bool found;
        if(attempt<0){
            found=Inside(item,result)&&!Contact(item,result,out _);
            if(!found)found=TryPosition(item,result,24,out result);
            random=new Match3DDailyRandom(unchecked(item.id*73856093+19349663));attempt=0;
        }else{
            if(attempt>=512)throw new InvalidOperationException("未能在预算内完成开局修复："+item.id+" ("+cursor+"/"+moving.Count+", y="+minimumVisibleY+")");
            if(attempt%8==0)Orient(item,Quaternion.Euler(random.Range(0,360),random.Range(0,360),random.Range(0,360)));
            float y=random.Range(Mathf.Max(firstStage?1.5f:0,minimumVisibleY),firstStage?8f:SoftTransitionContainer.Height-.5f);
            var border=BoardEnvironment.GetBorder(y);
            var candidate=new Vector3(border.position.x+random.Range(-1,1)*border.scale.x*.48f,y,border.position.z+random.Range(-1,1)*border.scale.z*.48f);
            found=TryPosition(item,candidate,12,out result);attempt++;
        }
        if(!found)return;
        item.position=result;collidable.Add(item);cursor++;attempt=-1;complete=cursor==moving.Count;
    }
    public void Apply(){
        if(!Complete)throw new InvalidOperationException("Opening separation is incomplete");
        foreach(var item in moving)if(!Inside(item,item.position)||Contact(item,item.position,out _))throw new InvalidOperationException("开局重叠检查未通过："+item.id);
        foreach(var item in moving)if((item.position-item.original).sqrMagnitude>1e-12f||Quaternion.Angle(item.rotation,item.originalRotation)>.0001f){
            item.obj.transform.SetPositionAndRotation(item.position,item.rotation);item.body.position=item.position;item.body.rotation=item.rotation;MovedCount++;
            if(!item.body.isKinematic){item.body.velocity=Vector3.zero;item.body.angularVelocity=Vector3.zero;}
        }
        Physics.SyncTransforms();
    }
}
