using System;
using System.Collections.Generic;
using UnityEngine;

// Approved short-transition design, 2026-11-10. Dimensions are container-local.
// This owns the hollow static wall, floor and ceiling; no imported geometry is read.
public sealed class SoftTransitionContainer : MonoBehaviour {
    public const int Sides=16;
    public const float Height=26,BottomApothem=1.15f,TopApothem=4.8f,TiltDegrees=0;
    public const float WallThickness=.3f,CeilingThickness=2f,Clearance=.04f,SpawnCeiling=24;
    public static readonly Quaternion Orientation=Quaternion.Euler(TiltDegrees,0,0);
    static readonly Quaternion InverseOrientation=Quaternion.Inverse(Orientation);
    // Keep the shell upright and the shared 82-degree camera/pan fixed. At the
    // main visible band (~21 units high), the camera projects vertical height
    // upward by ~3 units. A fixed horizontal origin balances that offset while
    // the narrow lower section still lands inside the authored pan.
    public static readonly Vector3 Origin=new Vector3(0,0,-3f);
    static readonly float[] RingHeights={0,6,10,11,12,13,14,20,26};
    static readonly Vector2[] Normals=MakeNormals();
    Mesh wallMesh;
    PhysicMaterial wallMaterial;
    public Mesh WallMesh=>wallMesh;
    public static Vector3 ToLocal(Vector3 world)=>InverseOrientation*(world-Origin);
    public static Vector3 ToWorld(Vector3 local)=>Origin+Orientation*local;
    public static Vector3 ToLocalDirection(Vector3 direction)=>InverseOrientation*direction;
    public static Vector3 ToWorldDirection(Vector3 direction)=>Orientation*direction;
    static Vector2[] MakeNormals(){var normals=new Vector2[Sides];for(int i=0;i<Sides;i++){float a=i*Mathf.PI*2/Sides;normals[i]=new Vector2(Mathf.Cos(a),Mathf.Sin(a));}return normals;}

    public static float DesignApothem(float h){
        h=Mathf.Clamp(h,0,Height);
        if(h<=10)return BottomApothem+.05f*h;
        if(h>=14)return 2.1f+.225f*(h-14);
        float t=(h-10)/4,t2=t*t,t3=t2*t;
        return (2*t3-3*t2+1)*1.65f+(t3-2*t2+t)*.2f+(-2*t3+3*t2)*2.1f+(t3-t2)*.9f;
    }
    // Use the actual piecewise planar collider surface when fitting items.
    public static float Apothem(float h){
        for(int i=1;i<RingHeights.Length;i++)if(h<=RingHeights[i])
            return Mathf.Lerp(DesignApothem(RingHeights[i-1]),DesignApothem(RingHeights[i]),Mathf.InverseLerp(RingHeights[i-1],RingHeights[i],h));
        return TopApothem;
    }
    public static Vector3 Vertex(float h,int index,float thickness=0){
        float r=(DesignApothem(h)+thickness)/Mathf.Cos(Mathf.PI/Sides),a=(index+.5f)*Mathf.PI*2/Sides;
        return new Vector3(Mathf.Cos(a)*r,h,Mathf.Sin(a)*r);
    }
    public static bool Contains(Vector3 world,float margin=0){
        var p=ToLocal(world);if(p.y<margin||p.y>Height-margin)return false;
        float r=Apothem(p.y)-margin;var xz=new Vector2(p.x,p.z);
        foreach(var n in Normals)if(Vector2.Dot(n,xz)>r)return false;
        return true;
    }
    public static GameObject Create(){
        var root=new GameObject("Short transition container");root.transform.SetPositionAndRotation(Origin,Orientation);
        var component=root.AddComponent<SoftTransitionContainer>();component.Build();return root;
    }
    void Build(){
        wallMaterial=new PhysicMaterial("Container smooth surface"){dynamicFriction=0,staticFriction=0,bounciness=0,frictionCombine=PhysicMaterialCombine.Minimum,bounceCombine=PhysicMaterialCombine.Minimum};
        var vertices=new List<Vector3>();var triangles=new List<int>();
        foreach(float h in RingHeights){for(int i=0;i<Sides;i++)vertices.Add(Vertex(h,i));for(int i=0;i<Sides;i++)vertices.Add(Vertex(h,i,WallThickness));}
        for(int row=0;row<RingHeights.Length-1;row++)for(int i=0;i<Sides;i++){
            int next=(i+1)%Sides,a=row*Sides*2+i,b=row*Sides*2+next,c=b+Sides*2,d=a+Sides*2;
            Quad(triangles,a,b,c,d); // Inner normals face the empty playable cavity.
            Quad(triangles,a+Sides,d+Sides,c+Sides,b+Sides);
        }
        for(int i=0;i<Sides;i++){
            int next=(i+1)%Sides,top=(RingHeights.Length-1)*Sides*2;
            Quad(triangles,i,i+Sides,next+Sides,next); // annular bottom, never a disk
            Quad(triangles,top+i,top+next,top+next+Sides,top+i+Sides);
        }
        wallMesh=new Mesh{name="Short transition hollow wall"};wallMesh.SetVertices(vertices);wallMesh.SetTriangles(triangles,0);wallMesh.RecalculateNormals();wallMesh.RecalculateBounds();
        var wall=Child("Container wall").AddComponent<MeshCollider>();wall.sharedMesh=wallMesh;wall.convex=false;Configure(wall);
        float floorWidth=(BottomApothem+WallThickness)*2/Mathf.Cos(Mathf.PI/Sides);
        Box("Container floor",new Vector3(0,-WallThickness*.5f,0),new Vector3(floorWidth,WallThickness,floorWidth));
        float topWidth=(TopApothem+WallThickness)*2/Mathf.Cos(Mathf.PI/Sides);
        // Extend the safety lid outward. A thin lid can let an overlapping,
        // rotating flat item resolve onto its upper face in dense large-size piles.
        Box("Container ceiling",new Vector3(0,Height+CeilingThickness*.5f,0),new Vector3(topWidth,CeilingThickness,topWidth));
    }
    static void Quad(List<int> t,int a,int b,int c,int d){t.Add(a);t.Add(b);t.Add(c);t.Add(a);t.Add(c);t.Add(d);}
    GameObject Child(string label){var obj=new GameObject(label);obj.layer=ItemColliderGeometry.WallLayer;obj.transform.SetParent(transform,false);return obj;}
    void Box(string label,Vector3 center,Vector3 size){var box=Child(label).AddComponent<BoxCollider>();box.center=center;box.size=size;Configure(box);}
    void Configure(Collider collider){collider.sharedMaterial=wallMaterial;collider.contactOffset=BoardPhysicsSettings.Active?BoardPhysicsSettings.Contact:Physics.defaultContactOffset;}
    void OnDestroy(){if(wallMesh)Destroy(wallMesh);if(wallMaterial)Destroy(wallMaterial);}

    // Capture full physical primitive geometry without syncing the global physics scene.
    // Pick colliders and temporarily disabled solid colliders are handled separately:
    // only the solid layer participates, even while undo/shuffle has disabled it.
    public static List<Vector3> SolidPoints(GameObject obj){
        var result=new List<Vector3>();var token=obj.GetComponent<BoardItemPoolToken>();token?.CacheComponents();
        foreach(var collider in token?token.Colliders:obj.GetComponentsInChildren<Collider>(true)){
            if(!collider||collider.isTrigger||collider.gameObject.layer!=ItemColliderGeometry.BodyLayer)continue;
            if(collider is BoxCollider box)AddBox(result,box.transform,box.center,box.size);
            else if(collider is SphereCollider sphere){
                var s=Abs(sphere.transform.lossyScale);float r=sphere.radius*Mathf.Max(s.x,Mathf.Max(s.y,s.z));
                AddWorldBox(result,sphere.transform.TransformPoint(sphere.center),Quaternion.identity,Vector3.one*(2*r));
            }else if(collider is CapsuleCollider capsule){
                var s=Abs(capsule.transform.lossyScale);int axis=capsule.direction;float radius=capsule.radius*Mathf.Max(s[(axis+1)%3],s[(axis+2)%3]);
                var size=Vector3.one*(2*radius);size[axis]=Mathf.Max(capsule.height*s[axis],2*radius);
                AddWorldBox(result,capsule.transform.TransformPoint(capsule.center),capsule.transform.rotation,size);
            }else if(collider is MeshCollider mesh&&mesh.sharedMesh)AddBox(result,mesh.transform,mesh.sharedMesh.bounds.center,mesh.sharedMesh.bounds.size);
            else throw new InvalidOperationException("Unsupported container item collider: "+collider.GetType().Name);
        }
        if(result.Count==0)throw new InvalidOperationException("No solid item collider: "+obj.name);
        return result;
    }
    static Vector3 Abs(Vector3 v)=>new Vector3(Mathf.Abs(v.x),Mathf.Abs(v.y),Mathf.Abs(v.z));
    static void AddBox(List<Vector3> points,Transform tr,Vector3 center,Vector3 size){
        for(int i=0;i<8;i++)points.Add(tr.TransformPoint(center+Vector3.Scale(size*.5f,new Vector3((i&1)==0?-1:1,(i&2)==0?-1:1,(i&4)==0?-1:1))));
    }
    static void AddWorldBox(List<Vector3> points,Vector3 center,Quaternion rotation,Vector3 size){
        for(int i=0;i<8;i++)points.Add(center+rotation*Vector3.Scale(size*.5f,new Vector3((i&1)==0?-1:1,(i&2)==0?-1:1,(i&4)==0?-1:1)));
    }
    public static Vector3 FittedPosition(GameObject obj,Vector3 desired){
        var points=SolidPoints(obj);var current=obj.transform.position;
        var min=Vector3.one*float.PositiveInfinity;var max=Vector3.one*float.NegativeInfinity;
        for(int i=0;i<points.Count;i++){points[i]=ToLocalDirection(points[i]-current);min=Vector3.Min(min,points[i]);max=Vector3.Max(max,points[i]);}
        var offset=(min+max)*.5f;float halfHeight=(max.y-min.y)*.5f;
        var center=ToLocal(desired)+offset;var supports=new float[Sides];float required=0;
        for(int i=0;i<Sides;i++){
            foreach(var p in points)supports[i]=Mathf.Max(supports[i],Vector2.Dot(Normals[i],new Vector2(p.x-offset.x,p.z-offset.z)));
            required=Mathf.Max(required,supports[i]+Clearance);
        }
        float ceiling=Height-halfHeight-Clearance,floor=halfHeight+Clearance;
        if(floor>ceiling||required>Apothem(ceiling-halfHeight))throw new InvalidOperationException("Item cannot fit selected container: "+obj.name);
        center.y=Mathf.Clamp(center.y,floor,ceiling);
        if(Apothem(center.y-halfHeight)<required){
            float low=center.y,high=ceiling;for(int i=0;i<24;i++){float mid=(low+high)*.5f;if(Apothem(mid-halfHeight)<required)low=mid;else high=mid;}center.y=high;
        }
        float apothem=Apothem(center.y-halfHeight)-Clearance,factor=1;var horizontal=new Vector2(center.x,center.z);
        for(int i=0;i<Sides;i++){float dot=Vector2.Dot(Normals[i],horizontal);if(dot>0)factor=Mathf.Min(factor,Mathf.Max(0,apothem-supports[i])/dot);}
        center.x*=factor;center.z*=factor;
        return ToWorld(center-offset);
    }
    public static void Fit(GameObject obj){obj.transform.position=FittedPosition(obj,obj.transform.position);}
}
