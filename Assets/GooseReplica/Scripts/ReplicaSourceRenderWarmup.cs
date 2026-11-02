using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

// Render mesh-only proxies: never activate a pooled item's scripts, colliders or RNG.
// Resource loading alone does not submit meshes/textures/shader variants to the GPU.
public sealed class ReplicaSourceRenderWarmup : IDisposable {
    readonly HashSet<GameObject> warmed=new HashSet<GameObject>();
    GameObject root;
    Camera camera;
    RenderTexture target;
    public int PreparedCount=>warmed.Count;
    public bool Prepare(GameObject prefab){
        if(!prefab||warmed.Contains(prefab))return false;
        if(!root){
            root=new GameObject("Source render warmup"){hideFlags=HideFlags.HideAndDontSave};
            root.transform.position=new Vector3(10000,10000,10000);
            camera=root.AddComponent<Camera>();camera.enabled=false;
            camera.orthographic=true;camera.orthographicSize=1.5f;camera.nearClipPlane=.1f;camera.farClipPlane=10;
            camera.cullingMask=1<<31;camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=Color.clear;
            camera.allowHDR=false;camera.allowMSAA=false;camera.useOcclusionCulling=false;
            target=new RenderTexture(64,64,16){name="Source warmup target",hideFlags=HideFlags.HideAndDontSave};
            target.Create();camera.targetTexture=target;
        }
        var proxies=new List<GameObject>();
        var previous=RenderTexture.active;
        try{
            foreach(var source in prefab.GetComponentsInChildren<Renderer>(true)){
                if(!source.enabled)continue;
                Mesh mesh=null;
                if(source is SkinnedMeshRenderer skin)mesh=skin.sharedMesh;
                else {var filter=source.GetComponent<MeshFilter>();if(filter)mesh=filter.sharedMesh;}
                if(!mesh)continue;
                var proxy=new GameObject("Warm mesh"){layer=31,hideFlags=HideFlags.HideAndDontSave};
                proxies.Add(proxy);proxy.transform.SetParent(root.transform,false);
                float scale=2f/Mathf.Max(.001f,mesh.bounds.size.magnitude);
                proxy.transform.localScale=Vector3.one*scale;
                proxy.transform.localPosition=Vector3.forward*3-mesh.bounds.center*scale;
                proxy.AddComponent<MeshFilter>().sharedMesh=mesh;
                var renderer=proxy.AddComponent<MeshRenderer>();renderer.sharedMaterials=source.sharedMaterials;
                renderer.shadowCastingMode=ShadowCastingMode.Off;renderer.receiveShadows=source.receiveShadows;
            }
            // A single type per preload frame; no Shader.WarmupAllShaders startup spike.
            camera.Render();warmed.Add(prefab);return true;
        }finally{
            RenderTexture.active=previous;
            foreach(var proxy in proxies){proxy.SetActive(false);UnityEngine.Object.Destroy(proxy);}
        }
    }
    public void Dispose(){
        if(camera)camera.targetTexture=null;
        if(target){target.Release();UnityEngine.Object.Destroy(target);target=null;}
        if(root){root.SetActive(false);UnityEngine.Object.Destroy(root);root=null;camera=null;}
    }
}
