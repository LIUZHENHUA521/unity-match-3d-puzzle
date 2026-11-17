using System.Collections.Generic;
using UnityEngine;

// A short circular shell. Keep the legacy pan supports and seeded spawn samples.
public sealed class PotFirstStageBoundary : MonoBehaviour {
    public const int Segments=48;
    public const float Height=8f,BottomRadius=2.65f,Thickness=.25f;
    public static float Radius(float height)=>BottomRadius+Mathf.Tan(12*Mathf.Deg2Rad)*height;
    readonly List<Mesh> meshes=new List<Mesh>();
    PhysicMaterial surface;
    public static void Create(Transform parent){
        var root=new GameObject("Lv1Col");root.transform.SetParent(parent,false);
        root.AddComponent<PotFirstStageBoundary>().Build();
    }
    void Build(){
        surface=new PhysicMaterial("Pot first stage surface") {dynamicFriction=0,staticFriction=0,bounciness=0,frictionCombine=PhysicMaterialCombine.Minimum,bounceCombine=PhysicMaterialCombine.Minimum};
        float top=Radius(Height);
        // Closed profile in radius/height space: normals face the cavity.
        Revolve("Pot circular wall",new[]{new Vector2(BottomRadius,0),new Vector2(top,Height),new Vector2(top+Thickness,Height),new Vector2(BottomRadius+Thickness,0)});
        Revolve("Pot circular ceiling",new[]{new Vector2(0,Height),new Vector2(0,Height+Thickness),new Vector2(top+Thickness,Height+Thickness),new Vector2(top+Thickness,Height)});
    }
    void Revolve(string label,Vector2[] profile){
        var vertices=new List<Vector3>();var triangles=new List<int>();
        foreach(var p in profile)for(int i=0;i<Segments;i++){
            float angle=i*Mathf.PI*2/Segments;
            vertices.Add(new Vector3(Mathf.Cos(angle)*p.x,p.y,Mathf.Sin(angle)*p.x));
        }
        for(int row=0;row<profile.Length;row++)for(int i=0;i<Segments;i++){
            int next=(i+1)%Segments,nextRow=(row+1)%profile.Length;
            int a=row*Segments+i,b=row*Segments+next,c=nextRow*Segments+next,d=nextRow*Segments+i;
            // At the axis use a single triangle, avoiding degenerate triangles.
            if(profile[row].x>0){triangles.Add(a);triangles.Add(b);triangles.Add(c);}
            if(profile[nextRow].x>0){triangles.Add(a);triangles.Add(c);triangles.Add(d);}
        }
        var mesh=new Mesh{name=label};mesh.SetVertices(vertices);mesh.SetTriangles(triangles,0);mesh.RecalculateNormals();mesh.RecalculateBounds();meshes.Add(mesh);
        var obj=new GameObject(label);obj.layer=ItemColliderGeometry.WallLayer;obj.transform.SetParent(transform,false);
        var collider=obj.AddComponent<MeshCollider>();collider.sharedMesh=mesh;collider.convex=false;collider.sharedMaterial=surface;
        collider.contactOffset=BoardPhysicsSettings.Active?BoardPhysicsSettings.Contact:Physics.defaultContactOffset;
    }
    void OnDestroy(){foreach(var mesh in meshes)if(mesh)Destroy(mesh);if(surface)Destroy(surface);}
}
