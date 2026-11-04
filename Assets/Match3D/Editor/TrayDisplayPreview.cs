using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Sprites;

// A cropped view of the actual tray geometry, without fitting the item itself.
public sealed class TrayDisplayPreview : IDisposable {
    readonly PreviewRenderUtility utility;
    readonly GameObject tray;
    readonly Match3DTraySkin skin;
    GameObject item,prefab;
    Mesh frameMesh;Material frameMaterial;
    public const float PortraitAspect=9f/16;
    public static float WorldPerPoint=>Match3DThemePresentation.Size(PortraitAspect)*2*PortraitAspect/720f;
    public float Spacing=>Vector3.Distance(skin.slots[0].position,skin.slots[1].position);
    public Camera Camera=>utility.camera;
    public GameObject Item=>item;
    public TrayDisplayPreview(){
        utility=new PreviewRenderUtility(true);
        utility.camera.orthographic=true;utility.camera.nearClipPlane=.3f;utility.camera.farClipPlane=20;
        utility.camera.transform.SetPositionAndRotation(Vector3.zero,Match3DThemePresentation.SourceRotation);
        utility.camera.clearFlags=CameraClearFlags.SolidColor;utility.camera.backgroundColor=new Color(.13f,.14f,.16f,1);utility.camera.allowHDR=false;
        utility.lights[0].type=LightType.Directional;utility.lights[0].intensity=.38f;utility.lights[0].transform.rotation=BoardLighting.SavedRotation;
        utility.lights[1].intensity=0;utility.ambientColor=new Color(1.4148f,1.4148f,1.4148f,1);
        tray=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Match3D/UI/CollectionTray"));
        tray.hideFlags=HideFlags.HideAndDontSave;utility.AddSingleGO(tray);
        var canvas=tray.GetComponent<Canvas>();canvas.renderMode=RenderMode.WorldSpace;canvas.worldCamera=utility.camera;
        tray.transform.SetPositionAndRotation(utility.camera.transform.forward*2,utility.camera.transform.rotation);tray.transform.localScale=Vector3.one*WorldPerPoint;
        skin=tray.GetComponent<Match3DTraySkin>();if(skin.footerBackground)skin.footerBackground.gameObject.SetActive(false);
        Canvas.ForceUpdateCanvases();
        // Preview scenes do not submit world-space Canvas graphics reliably.
        // Draw the same Simple Image sprite/rect as a mesh in this scene; keep
        // the real prefab's RectTransforms as the authoritative slot anchors.
        var frame=tray.GetComponent<Match3DUIView>().Get<Image>("Frame");
        var rect=frame.rectTransform.rect;var uv=DataUtility.GetOuterUV(frame.sprite);
        frameMesh=new Mesh{hideFlags=HideFlags.HideAndDontSave};
        frameMesh.vertices=new[]{new Vector3(rect.xMin,rect.yMin,0),new Vector3(rect.xMax,rect.yMin,0),new Vector3(rect.xMax,rect.yMax,0),new Vector3(rect.xMin,rect.yMax,0)};
        frameMesh.uv=new[]{new Vector2(uv.x,uv.y),new Vector2(uv.z,uv.y),new Vector2(uv.z,uv.w),new Vector2(uv.x,uv.w)};
        frameMesh.colors=new[]{Color.white,Color.white,Color.white,Color.white};frameMesh.triangles=new[]{0,2,1,0,3,2};frameMesh.RecalculateBounds();
        frameMaterial=new Material(Shader.Find("Sprites/Default")){hideFlags=HideFlags.HideAndDontSave,mainTexture=frame.sprite.texture,color=frame.color};
        var renderedFrame=new GameObject("Tray art preview",typeof(MeshFilter),typeof(MeshRenderer));renderedFrame.transform.SetParent(frame.transform,false);
        renderedFrame.GetComponent<MeshFilter>().sharedMesh=frameMesh;renderedFrame.GetComponent<MeshRenderer>().sharedMaterial=frameMaterial;
        canvas.enabled=false;
    }
    public void Select(GameObject source){
        if(prefab==source&&item)return;
        if(item)UnityEngine.Object.DestroyImmediate(item);prefab=source;if(!source)return;
        item=UnityEngine.Object.Instantiate(source);item.name=source.name;item.hideFlags=HideFlags.HideAndDontSave;utility.AddSingleGO(item);
        foreach(var body in item.GetComponentsInChildren<Rigidbody>(true)){body.isKinematic=true;body.detectCollisions=false;}
        foreach(var collider in item.GetComponentsInChildren<Collider>(true))collider.enabled=false;
        if(item.transform.childCount>0){item.transform.GetChild(0).localPosition=Vector3.zero;item.transform.GetChild(0).localRotation=Quaternion.identity;}
    }
    public void Apply(Match3DTrayDisplaySettings.Pose pose,int slot=3){
        if(!item)return;
        var cameraRotation=utility.camera.transform.rotation;
        var position=skin.slots[slot].position-utility.camera.transform.forward*.42f;
        item.transform.SetPositionAndRotation(Match3DTrayDisplaySettings.Position(pose,position,cameraRotation,Spacing),Match3DTrayDisplaySettings.Rotation(pose,cameraRotation));
        item.transform.localScale=ItemFlightMotion.ScaleForPrefab(prefab,Spacing)*(pose?.scale??1);
    }
    public Texture Render(Rect rect,Match3DTrayDisplaySettings.Pose pose,int slot=3,bool closeup=false){
        Apply(pose,slot);
        // Zoom only the camera crop. The object and slot retain their true ratio.
        float width=(closeup?220:760)*WorldPerPoint;
        utility.camera.orthographicSize=width*.5f/Mathf.Max(.1f,rect.width/rect.height);
        utility.BeginPreview(rect,GUIStyle.none);utility.Render();return utility.EndPreview();
    }
    public Texture2D Capture(Match3DTrayDisplaySettings.Pose pose,int width=720,int height=190,bool closeup=false){
        var texture=Render(new Rect(0,0,width,height),pose,3,closeup);
        var rt=RenderTexture.GetTemporary(width,height,0,RenderTextureFormat.ARGB32);var old=RenderTexture.active;
        try{Graphics.Blit(texture,rt);RenderTexture.active=rt;var image=new Texture2D(width,height,TextureFormat.RGB24,false);image.ReadPixels(new Rect(0,0,width,height),0,0);image.Apply();return image;}
        finally{RenderTexture.active=old;RenderTexture.ReleaseTemporary(rt);}
    }
    public void Dispose(){utility.Cleanup();if(frameMesh)UnityEngine.Object.DestroyImmediate(frameMesh);if(frameMaterial)UnityEngine.Object.DestroyImmediate(frameMaterial);}
}
