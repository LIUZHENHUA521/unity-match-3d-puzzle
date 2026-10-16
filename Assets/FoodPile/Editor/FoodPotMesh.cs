using System.Collections.Generic;
using UnityEngine;

// Revolves a cross-section into an open cooking pot, including inner wall and thick lip.
public static class FoodPotMesh
{
    public static Mesh Create()
    {
        var profile = new Vector2[] {
            new Vector2(0,0), new Vector2(.65f,0), new Vector2(.82f,.07f),
            new Vector2(.96f,.26f), new Vector2(1.07f,.51f), new Vector2(1.10f,.57f),
            new Vector2(1.16f,.57f), new Vector2(1.18f,.51f), new Vector2(1.06f,.19f),
            new Vector2(.87f,-.07f), new Vector2(0,-.10f)
        };
        const int segments=80;
        var vertices=new List<Vector3>();var triangles=new List<int>();
        for(int ring=0;ring<profile.Length;ring++)
            for(int i=0;i<=segments;i++) {
                float a=i*Mathf.PI*2/segments;
                vertices.Add(new Vector3(Mathf.Cos(a)*profile[ring].x,profile[ring].y,Mathf.Sin(a)*profile[ring].x));
            }
        for(int r=0;r<profile.Length-1;r++) for(int i=0;i<segments;i++) {
            int a=r*(segments+1)+i,b=a+segments+1;
            triangles.Add(a);triangles.Add(a+1);triangles.Add(b);
            triangles.Add(a+1);triangles.Add(b+1);triangles.Add(b);
        }
        var mesh=new Mesh {name="Open cooking pot"};mesh.SetVertices(vertices);mesh.SetTriangles(triangles,0);mesh.RecalculateNormals();mesh.RecalculateBounds();return mesh;
    }
}
