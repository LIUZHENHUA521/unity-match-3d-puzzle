using UnityEngine;

// The visible pan is above the legacy Y=0 container floor. Share its actual
// surface with placement and rescue items already stranded below that surface.
public sealed class PotPanFloor : MonoBehaviour {
    static PotPanFloor current;
    Collider[] surfaces;
    float rayTop,rayLength;
    public static void Bind(Transform pan){
        var floor=pan.GetComponent<PotPanFloor>();if(!floor)floor=pan.gameObject.AddComponent<PotPanFloor>();
        floor.surfaces=pan.GetComponentsInChildren<MeshCollider>();
        Physics.SyncTransforms();
        var bounds=new Bounds(pan.position,Vector3.zero);
        foreach(var surface in floor.surfaces)bounds.Encapsulate(surface.bounds);
        floor.rayTop=bounds.max.y+1;floor.rayLength=bounds.size.y+2;current=floor;
    }
    public static bool TryHeight(Vector3 position,out float height){
        height=0;if(!current||!current.isActiveAndEnabled)return false;
        bool found=false;var ray=new Ray(new Vector3(position.x,current.rayTop,position.z),Vector3.down);
        foreach(var surface in current.surfaces)if(surface&&surface.enabled&&surface.Raycast(ray,out var hit,current.rayLength)&&hit.normal.y>.5f){
            if(!found||hit.point.y>height)height=hit.point.y;found=true;
        }
        return found;
    }
    public static bool Recover(Rigidbody body){
        if(!body||!body.gameObject.activeInHierarchy||!current||body.transform.position.y>current.rayTop)return false;
        if(!TryHeight(body.transform.position,out float height))return false;
        // Cheap rejection for the ordinary pile. Only inspect geometry near the floor.
        if(body.transform.position.y>height+1)return false;
        var points=SoftTransitionContainer.SolidPoints(body.gameObject);
        float bottom=float.PositiveInfinity,top=float.NegativeInfinity;
        foreach(var point in points){bottom=Mathf.Min(bottom,point.y);top=Mathf.Max(top,point.y);}
        // Never move a supported/partially visible item or alter normal contact settling.
        if(top>=height-.01f)return false;
        var position=body.transform.position+Vector3.up*(height+SoftTransitionContainer.Clearance-bottom);
        if(BoardEnvironment.UsesSoftContainer)position=SoftTransitionContainer.FittedPosition(body.gameObject,position);
        body.transform.position=position;body.position=position;
        if(!body.isKinematic){body.velocity=Vector3.zero;body.angularVelocity=Vector3.zero;body.WakeUp();}
        return true;
    }
    void OnDisable(){if(current==this)current=null;}
}
