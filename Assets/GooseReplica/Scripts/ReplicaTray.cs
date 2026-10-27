using UnityEngine;
using UnityEngine.UI;

// Reference demo: one sliced MatchingPad background plus seven independent cells.
// A world-space Canvas keeps the existing 3D items visible in front of the tray.
public sealed class ReplicaTray : MonoBehaviour {
    public static ReplicaTray Instance {get;private set;}
    const float Width=680f, Height=122f, Spacing=90f;
    // WX data.unity3d: ContentPointList local X = -2.13,-1.42,-.71,0,.71,1.42,2.13;
    // the parent Content is scaled by 1.4, therefore the source world spacing is .994.
    public const float SourcePointLocalSpacing=.71f;
    public const float SourceContentScale=1.4f;
    public const float SourcePointSpacing=SourcePointLocalSpacing*SourceContentScale;
    // Page-space anchor measured from the source portrait layout. The previous
    // 310 value left an oversized gap above the action buttons; lowering the
    // anchor keeps the tray in the lower play area without changing slot depth
    // or the recovered .994 world spacing.
    public const float TrayScreenY=230f;
    RectTransform tray;ReplicaTraySkin skin;
    Renderer legacyRenderer;Collider legacyCollider;
    bool priorRenderer,priorCollider;
    public float ItemScaleFactor {get {Layout();return (skin?Vector3.Distance(skin.slots[0].position,skin.slots[1].position):Spacing*tray.localScale.x)/.333f;}}
    void Awake(){
        Instance=this;
        var legacy=GameObject.Find("Stack");
        if(legacy){
            legacyRenderer=legacy.GetComponent<Renderer>();legacyCollider=legacy.GetComponent<Collider>();
            if(legacyRenderer){priorRenderer=legacyRenderer.enabled;legacyRenderer.enabled=false;}
            if(legacyCollider){priorCollider=legacyCollider.enabled;legacyCollider.enabled=false;}
        }
        tray=(RectTransform)ReplicaUIView.Load("CollectionTray",transform).transform;
        skin=tray.GetComponent<ReplicaTraySkin>();
        var canvas=tray.GetComponent<Canvas>();canvas.worldCamera=Camera.main;
        Layout();tray.gameObject.SetActive(false);
    }
    void LateUpdate(){
        bool visible=ReplicaFlow.Instance&&ReplicaFlow.Instance.ScreenName=="game";
        if(tray.gameObject.activeSelf!=visible)tray.gameObject.SetActive(visible);
        if(visible)Layout();
    }
    void Layout(){
        var camera=Camera.main;if(!camera)return;
        float worldPerPoint=camera.orthographicSize*2*camera.aspect/720f;
        float logicalHeight=720f/camera.aspect;
        float safeBottom=Screen.safeArea.yMin*720f/Mathf.Max(1,Screen.width);
        if(skin){
            skin.ApplyBottomInset(safeBottom);
            tray.position=camera.ViewportToWorldPoint(new Vector3(.5f,(skin.screenCenterFromBottom+safeBottom)/logicalHeight,2f));
            tray.rotation=camera.transform.rotation;tray.localScale=Vector3.one*worldPerPoint;return;
        }
        if(InputController.Instance&&InputController.Instance.UsesSourceFlight){
            // Match source Content camera-local depth 2; keep our safe-area UI placement.
            // The old ground-plane canvas can be occluded by the recovered pot geometry.
            tray.position=camera.ViewportToWorldPoint(new Vector3(.5f,(TrayScreenY+safeBottom)/logicalHeight,2f));
            tray.rotation=camera.transform.rotation;tray.localScale=Vector3.one*worldPerPoint;
            return;
        }
        var ray=camera.ViewportPointToRay(new Vector3(.5f,(TrayScreenY+safeBottom)/logicalHeight,0));
        var plane=new Plane(Vector3.up,new Vector3(0,.08f,0));
        if(plane.Raycast(ray,out float distance))tray.position=ray.GetPoint(distance);
        tray.rotation=Quaternion.Euler(90,0,0);tray.localScale=Vector3.one*worldPerPoint;
    }
    public Vector3 SlotPosition(int index){
        Layout();
        if(skin&&Camera.main){
            var camera=Camera.main;var screen=camera.WorldToScreenPoint(skin.slots[index].position);
            screen.z=1.58f;return camera.ScreenToWorldPoint(screen);
        }
        var position=tray.TransformPoint(new Vector3((index-3)*Spacing,0,0));
        if(InputController.Instance&&InputController.Instance.UsesSourceFlight&&Camera.main){
            // Source ContentPointList is a world-space transform, not a seven-cell pixel grid:
            // local point spacing .71 * Content scale 1.4 = .994 along camera right.
            // Keep the project's safe-area screen anchor, then project the exact source point
            // line back to the recovered Content depth (2 - .3*1.4 = 1.58).
            var camera=Camera.main;
            var centerScreen=camera.WorldToScreenPoint(tray.TransformPoint(Vector3.zero));
            centerScreen.z=1.58f;
            var center=camera.ScreenToWorldPoint(centerScreen);
            position=center+camera.transform.right*((index-3)*SourcePointSpacing);
        }
        else position.y=.3f;
        return position;
    }
    void OnDestroy(){
        if(legacyRenderer)legacyRenderer.enabled=priorRenderer;
        if(legacyCollider)legacyCollider.enabled=priorCollider;
        if(Instance==this)Instance=null;
        if(tray)Destroy(tray.gameObject);
    }
}
