using System;
using UnityEditor;
using UnityEngine;

// Fixed world-space width; never auto-fit the model, so changing size stays visible.
public sealed class BoardSizePreview : IDisposable {
    readonly PreviewRenderUtility utility;
    GameObject item,prefab;
    readonly Mesh grid;
    readonly Material material;
    public BoardSizePreview(){
        utility=new PreviewRenderUtility(true);
        var camera=utility.camera;
        camera.orthographic=true;camera.nearClipPlane=.1f;camera.farClipPlane=100;
        camera.transform.SetPositionAndRotation(-(Match3DThemePresentation.SourceRotation*Vector3.forward)*20,Match3DThemePresentation.SourceRotation);
        camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=new Color(.13f,.14f,.16f);camera.allowHDR=false;
        utility.lights[0].type=LightType.Directional;utility.lights[0].intensity=.38f;utility.lights[0].transform.rotation=BoardLighting.SavedRotation;
        utility.lights[1].intensity=0;utility.ambientColor=new Color(1.4148f,1.4148f,1.4148f);
        var vertices=new System.Collections.Generic.List<Vector3>();
        for(int i=-4;i<=4;i++){
            vertices.Add(new Vector3(i,-2,-4));vertices.Add(new Vector3(i,-2,4));
            vertices.Add(new Vector3(-4,-2,i));vertices.Add(new Vector3(4,-2,i));
        }
        grid=new Mesh{hideFlags=HideFlags.HideAndDontSave};grid.SetVertices(vertices);
        var indices=new int[vertices.Count];for(int i=0;i<indices.Length;i++)indices[i]=i;grid.SetIndices(indices,MeshTopology.Lines,0);
        material=new Material(Shader.Find("Sprites/Default")){hideFlags=HideFlags.HideAndDontSave,color=new Color(.32f,.35f,.39f)};
        var go=new GameObject("World unit grid",typeof(MeshFilter),typeof(MeshRenderer));go.GetComponent<MeshFilter>().sharedMesh=grid;go.GetComponent<MeshRenderer>().sharedMaterial=material;utility.AddSingleGO(go);
    }
    public Texture Render(Rect rect,GameObject source,float multiplier){
        if(prefab!=source||!item){
            if(item)UnityEngine.Object.DestroyImmediate(item);prefab=source;
            item=UnityEngine.Object.Instantiate(source);item.hideFlags=HideFlags.HideAndDontSave;utility.AddSingleGO(item);
            foreach(var rb in item.GetComponentsInChildren<Rigidbody>(true)){rb.isKinematic=true;rb.detectCollisions=false;}
            foreach(var c in item.GetComponentsInChildren<Collider>(true))c.enabled=false;
        }
        var theme=Match3DModelThemes.Find(null,source);utility.ambientColor=BoardLighting.AmbientFor(theme);utility.lights[0].intensity=BoardLighting.IntensityFor(theme);
        item.transform.SetPositionAndRotation(Vector3.zero,Quaternion.Euler(60,0,25));
        item.transform.localScale=source.transform.localScale*BoardLayoutSettings.BaseModelScale(source)*multiplier;
        float width=Match3DThemePresentation.Size(9f/16)*2*9f/16;
        utility.camera.orthographicSize=width*.5f/(rect.width/rect.height);
        utility.BeginPreview(rect,GUIStyle.none);utility.Render();return utility.EndPreview();
    }
    public void Dispose(){utility.Cleanup();UnityEngine.Object.DestroyImmediate(grid);UnityEngine.Object.DestroyImmediate(material);}
}
