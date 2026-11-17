#if UNITY_EDITOR || DAILY_WEB_PREVIEW
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

// Authoring-only, synchronous capture. No frame/physics update runs while visual
// flags are exposed; all modified state is restored even when rendering fails.
public static class PotStructurePreview {
    const int CaptureLayer=31;
    public const int Width=540,Height=960,MaxImageLength=180000;
    public static string Capture(){
        var pile=LayeredItemPile.Instance;
        var presentation=pile?pile.PotStructurePresentation:null;
        if(!pile||!presentation)throw new InvalidOperationException("关卡尚未准备好，无法生成结构图");
        var renderers=pile.PotStructureRenderers.Concat(presentation.PotStructurePanRenderers).Where(r=>r).Distinct().ToArray();
        if(renderers.Length==0)throw new InvalidOperationException("关卡没有可预览的物品");
        var enabled=renderers.Select(r=>r.enabled).ToArray();
        var layers=renderers.Select(r=>r.gameObject).Distinct().ToDictionary(o=>o,o=>o.layer);
        var points=new List<Vector3>();
        foreach(var obj in LevelGenerator.Instance.objectsOnScene)
            foreach(var c in obj.GetComponentsInChildren<Collider>(true))
                if(!c.isTrigger&&c.gameObject.layer==ItemColliderGeometry.BodyLayer)AddCollider(points,c);
        if(pile.PotStructureBoundary)
            foreach(var c in pile.PotStructureBoundary.GetComponentsInChildren<Collider>())
                if(c.enabled&&!c.isTrigger)AddCollider(points,c);
        var bounds=renderers[0].bounds;
        foreach(var r in renderers)bounds.Encapsulate(r.bounds);
        foreach(var p in points)bounds.Encapsulate(p);
        var root=new GameObject("Pot structure capture"){hideFlags=HideFlags.HideAndDontSave};
        RenderTexture target=null;Texture2D texture=null;Mesh lines=null;Material material=null;CommandBuffer commands=null;
        var active=RenderTexture.active;
        try{
            var camera=root.AddComponent<Camera>();camera.enabled=false;
            camera.orthographic=true;camera.aspect=(float)Width/Height;camera.cullingMask=1<<CaptureLayer;
            camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=new Color(.18f,.20f,.22f,1);
            camera.allowHDR=false;camera.allowMSAA=false;camera.useOcclusionCulling=false;
            Fit(camera,bounds);
            var light=root.AddComponent<Light>();light.type=LightType.Directional;light.cullingMask=1<<CaptureLayer;light.intensity=.6f;light.shadows=LightShadows.None;
            foreach(var pair in layers)pair.Key.layer=CaptureLayer;
            foreach(var r in renderers)r.enabled=true;
            var shader=Resources.Load<Shader>("PotStructureLines");
            if(!shader)throw new InvalidOperationException("结构图线框资源缺失");
            material=new Material(shader){hideFlags=HideFlags.HideAndDontSave};
            lines=new Mesh{name="Pot physical collider lines",indexFormat=IndexFormat.UInt32};
            lines.SetVertices(points);lines.SetIndices(Enumerable.Range(0,points.Count).ToArray(),MeshTopology.Lines,0);
            commands=new CommandBuffer{name="Pot collider overlay"};commands.DrawMesh(lines,Matrix4x4.identity,material);
            camera.AddCommandBuffer(CameraEvent.AfterEverything,commands);
            target=new RenderTexture(Width,Height,24);target.Create();camera.targetTexture=target;
            camera.Render();RenderTexture.active=target;
            texture=new Texture2D(Width,Height,TextureFormat.RGB24,false);
            texture.ReadPixels(new Rect(0,0,Width,Height),0,0);texture.Apply();
            for(int quality=85;quality>=25;quality-=10){
                var image="data:image/jpeg;base64,"+Convert.ToBase64String(texture.EncodeToJPG(quality));
                if(image.Length<=MaxImageLength)return image;
            }
            throw new InvalidOperationException("结构预览图超过保存大小限制");
        }finally{
            for(int i=0;i<renderers.Length;i++)if(renderers[i])renderers[i].enabled=enabled[i];
            foreach(var pair in layers)if(pair.Key)pair.Key.layer=pair.Value;
            RenderTexture.active=active;
            // Temporary objects only: immediate release also prevents repeat captures
            // in the same frame retaining cameras, line buffers and GPU targets.
            UnityEngine.Object.DestroyImmediate(root);
            commands?.Release();
            if(target){target.Release();UnityEngine.Object.DestroyImmediate(target);}
            if(texture)UnityEngine.Object.DestroyImmediate(texture);
            if(lines)UnityEngine.Object.DestroyImmediate(lines);
            if(material)UnityEngine.Object.DestroyImmediate(material);
        }
    }
    public static void Fit(Camera camera,Bounds bounds){
        var rotation=Quaternion.Euler(12,25,0);var inverse=Quaternion.Inverse(rotation);
        Vector3 min=Vector3.one*float.PositiveInfinity,max=Vector3.one*float.NegativeInfinity;
        for(int i=0;i<8;i++){
            var p=inverse*Vector3.Scale(bounds.extents,new Vector3((i&1)==0?-1:1,(i&2)==0?-1:1,(i&4)==0?-1:1));
            min=Vector3.Min(min,p);max=Vector3.Max(max,p);
        }
        var half=(max-min)*.5f;float distance=half.z+20;
        camera.transform.SetPositionAndRotation(bounds.center-rotation*Vector3.forward*distance,rotation);
        camera.orthographicSize=Mathf.Max(half.y,half.x/camera.aspect)*1.09f;
        camera.nearClipPlane=.1f;camera.farClipPlane=distance+half.z+20;
    }
    static void Line(List<Vector3> p,Vector3 a,Vector3 b){p.Add(a);p.Add(b);}
    public static void AddCollider(List<Vector3> p,Collider collider){
        var t=collider.transform;
        if(collider is BoxCollider box){
            var v=new Vector3[8];for(int i=0;i<8;i++)v[i]=t.TransformPoint(box.center+Vector3.Scale(box.size*.5f,new Vector3((i&1)==0?-1:1,(i&2)==0?-1:1,(i&4)==0?-1:1)));
            for(int i=0;i<8;i++)for(int bit=1;bit<=4;bit*=2)if((i&bit)==0)Line(p,v[i],v[i|bit]);
        }else if(collider is SphereCollider sphere){
            var s=Abs(t.lossyScale);float r=sphere.radius*Mathf.Max(s.x,Mathf.Max(s.y,s.z));var center=t.TransformPoint(sphere.center);
            Ring(p,center,Vector3.right*r,Vector3.up*r);Ring(p,center,Vector3.right*r,Vector3.forward*r);Ring(p,center,Vector3.up*r,Vector3.forward*r);
        }else if(collider is CapsuleCollider capsule){
            var s=Abs(t.lossyScale);int d=capsule.direction;
            float r=capsule.radius*Mathf.Max(s[(d+1)%3],s[(d+2)%3]);float h=Mathf.Max(0,capsule.height*s[d]*.5f-r);
            var axis=Vector3.zero;axis[d]=1;var side=Vector3.zero;side[(d+1)%3]=1;
            var up=t.rotation*axis;var u=t.rotation*side;var v=Vector3.Cross(up,u);var center=t.TransformPoint(capsule.center);
            Ring(p,center+up*h,u*r,v*r);Ring(p,center-up*h,u*r,v*r);
            foreach(var a in new[]{u,-u,v,-v}){
                Line(p,center+up*h+a*r,center-up*h+a*r);
                Arc(p,center+up*h,a*r,up*r);Arc(p,center-up*h,a*r,-up*r);
            }
        }else if(collider is MeshCollider mesh&&mesh.sharedMesh){
            var vertices=mesh.sharedMesh.vertices;var triangles=mesh.sharedMesh.triangles;var seen=new HashSet<ulong>();
            for(int i=0;i<triangles.Length;i+=3)for(int j=0;j<3;j++){
                int a=triangles[i+j],b=triangles[i+(j+1)%3];ulong key=((ulong)(uint)Math.Min(a,b)<<32)|(uint)Math.Max(a,b);
                if(seen.Add(key))Line(p,t.TransformPoint(vertices[a]),t.TransformPoint(vertices[b]));
            }
        }else throw new InvalidOperationException("不支持的结构图碰撞体："+collider.GetType().Name);
    }
    static Vector3 Abs(Vector3 s)=>new Vector3(Mathf.Abs(s.x),Mathf.Abs(s.y),Mathf.Abs(s.z));
    static void Ring(List<Vector3> p,Vector3 center,Vector3 u,Vector3 v){
        for(int i=0;i<32;i++){float a=i*Mathf.PI/16,b=(i+1)*Mathf.PI/16;Line(p,center+u*Mathf.Cos(a)+v*Mathf.Sin(a),center+u*Mathf.Cos(b)+v*Mathf.Sin(b));}
    }
    static void Arc(List<Vector3> p,Vector3 center,Vector3 u,Vector3 v){
        for(int i=0;i<8;i++){float a=i*Mathf.PI/16,b=(i+1)*Mathf.PI/16;Line(p,center+u*Mathf.Cos(a)+v*Mathf.Sin(a),center+u*Mathf.Cos(b)+v*Mathf.Sin(b));}
    }
}
#endif
