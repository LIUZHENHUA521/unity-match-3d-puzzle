using UnityEngine;
using DG.Tweening;

// Press feedback is separate from the collection tween: colliders remain enabled only here.
public sealed class ReplicaSelectionFeedback : MonoBehaviour {
    public float liftDistance=.4f,liftDuration=.2f;
    ReplicaSourceHighlight sourceHighlight;
    static readonly int Pulse=Shader.PropertyToID("_SelectionPulse");
    public GameObject Selected => body?body.gameObject:null;
    Rigidbody body;Renderer[] renderers;MaterialPropertyBlock block;
    bool managed,wasKinematic,moves,sourceMode;RigidbodyInterpolation interpolation;CollisionDetectionMode detection;
    Vector3 from,to;float elapsed;
    public const float SourceLiftOffset=-.4f,SourceLiftSeconds=.2f;
    static readonly int Glow=Shader.PropertyToID("_SelectionGlow");
    void Awake(){block=new MaterialPropertyBlock();}
    public void Begin(GameObject item,Camera camera,bool move=true){
        Cancel();if(!item||!camera)return;body=item.GetComponent<Rigidbody>();if(!body)return;
        renderers=item.GetComponentsInChildren<Renderer>();
        sourceHighlight=ReplicaSourceHighlight.For(camera);sourceHighlight.Select(item);
        foreach(var r in renderers){r.GetPropertyBlock(block);block.SetFloat(Glow,0);block.SetFloat(Pulse,0);r.SetPropertyBlock(block);}
        moves=move;if(!moves)return;
        var physics=ReplicaPilePhysics.Instance;managed=physics&&physics.Contains(body);
        sourceMode=InputController.Instance&&InputController.Instance.UsesSourceFlight;
        if(sourceMode){
            // BeSelect(true): keep solid colliders and saved Discrete/None settings.
            // HideRigid skips _isSelect; contact response is left to the physics engine.
            if(managed)physics.SetSelected(body,true);
            body.isKinematic=true;body.transform.DOKill(false);
            body.transform.DOMove(body.transform.position+camera.transform.forward*SourceLiftOffset,SourceLiftSeconds).SetEase(Ease.Linear);
            return;
        }
        wasKinematic=body.isKinematic;interpolation=body.interpolation;detection=body.collisionDetectionMode;
        if(managed)physics.RemoveForPick(body);
        if(!body.isKinematic){body.velocity=Vector3.zero;body.angularVelocity=Vector3.zero;}
        body.isKinematic=true;body.collisionDetectionMode=CollisionDetectionMode.ContinuousSpeculative;
        body.interpolation=RigidbodyInterpolation.Interpolate;
        from=body.position;to=from-camera.transform.forward*liftDistance;elapsed=0;
        if(managed)physics.WakeSelectionNeighbours(from);
    }
    void FixedUpdate(){
        if(sourceMode||!body||!moves||elapsed>=liftDuration)return;
        elapsed+=Time.fixedDeltaTime;float t=Mathf.Clamp01(elapsed/Mathf.Max(.01f,liftDuration));
        body.MovePosition(Vector3.Lerp(from,to,1-(1-t)*(1-t)));
    }
    void ClearVisual(){
        if(sourceHighlight)sourceHighlight.Clear();sourceHighlight=null;
        if(renderers!=null&&block!=null)foreach(var r in renderers)if(r){r.GetPropertyBlock(block);block.SetFloat(Glow,0);block.SetFloat(Pulse,0);r.SetPropertyBlock(block);}
    }
    void ResetHeld(){body=null;renderers=null;managed=false;moves=false;sourceMode=false;}
    // Successful GetUnit goes straight to BeClick, without BeSelect(false).
    public void Commit(){
        if(!sourceMode){Cancel();return;}
        ClearVisual();
        if(body){body.transform.DOKill(false);if(managed&&ReplicaPilePhysics.Instance)ReplicaPilePhysics.Instance.SetSelected(body,false);}
        ResetHeld();
    }
    public void Cancel(){
        ClearVisual();
        if(body&&moves){
            if(sourceMode){
                body.transform.DOKill(false);
                if(managed&&ReplicaPilePhysics.Instance)ReplicaPilePhysics.Instance.SetSelected(body,false);
                body.isKinematic=false;body.velocity=Vector3.zero;body.angularVelocity=Vector3.zero;
            }else{
                body.interpolation=interpolation;
                body.collisionDetectionMode=CollisionDetectionMode.Discrete;
                if(managed&&ReplicaPilePhysics.Instance){ReplicaPilePhysics.Instance.Register(body);ReplicaPilePhysics.Instance.WakeNear(body.position);}
                else{body.isKinematic=wasKinematic;body.collisionDetectionMode=detection;if(!wasKinematic){body.velocity=Vector3.zero;body.angularVelocity=Vector3.zero;}}
            }
        }
        ResetHeld();
    }
    void OnDisable(){Cancel();}
    void OnApplicationFocus(bool focus){if(!focus)Cancel();}
}
