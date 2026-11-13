using System.Collections.Generic;
using UnityEngine;

// A small kinematic contact volume pushes the live pile only during probe emergence.
// The collected model's own colliders stay disabled throughout its UI flight.
public sealed class ProbeLiftContact : MonoBehaviour {
    public const float Duration=.2f, Height=.4f, MaxSpeed=2.4f, MaxSpin=4f;
    static readonly Dictionary<GameObject,ProbeLiftContact> active=new Dictionary<GameObject,ProbeLiftContact>();
    readonly List<Collider> candidates=new List<Collider>();
    readonly HashSet<Rigidbody> touched=new HashSet<Rigidbody>();
    GameObject item;Rigidbody mover;SphereCollider shape;Vector3 origin;float elapsed;
    public static int ActiveCount=>active.Count;
    public static Vector3 Lift(Vector3 start,float seconds)=>start+Vector3.up*(Height*Mathf.Clamp01(seconds/Duration));
    public static void Begin(GameObject selected){
        Stop(selected);if(!selected)return;
        var renderers=selected.GetComponentsInChildren<Renderer>();bool found=false;var bounds=new Bounds();
        foreach(var r in renderers)if(r.enabled&&(r is MeshRenderer||r is SkinnedMeshRenderer)){
            if(!found){bounds=r.bounds;found=true;}else bounds.Encapsulate(r.bounds);
        }
        if(!found)return;
        var go=new GameObject("Probe emergence contact");var contact=go.AddComponent<ProbeLiftContact>();
        contact.item=selected;contact.origin=bounds.center;go.transform.position=bounds.center;
        contact.shape=go.AddComponent<SphereCollider>();
        contact.shape.radius=Mathf.Clamp(Mathf.Min(bounds.extents.x,bounds.extents.y,bounds.extents.z)*.8f,.05f,.55f);
        contact.mover=go.AddComponent<Rigidbody>();contact.mover.isKinematic=true;contact.mover.useGravity=false;
        contact.mover.collisionDetectionMode=CollisionDetectionMode.ContinuousSpeculative;
        if(LevelGenerator.Instance)foreach(var obj in LevelGenerator.Instance.objectsOnScene)if(obj)
            contact.candidates.AddRange(obj.GetComponentsInChildren<Collider>());
        contact.FilterContacts();active[selected]=contact;
    }
    bool Eligible(Rigidbody body)=>body&&!body.isKinematic&&body.detectCollisions&&
        ItemPhysicsController.Instance&&ItemPhysicsController.Instance.Contains(body)&&
        (!InputController.Instance||!InputController.Instance.objectsOnStack.Contains(body.gameObject))&&!BoardTools.IsStored(body.gameObject);
    void FilterContacts(){foreach(var other in candidates)if(other&&other!=shape)Physics.IgnoreCollision(shape,other,!Eligible(other.attachedRigidbody));}
    void FixedUpdate(){Step(Time.fixedDeltaTime);}
    public void Step(float seconds){
        if(!item||!item.activeInHierarchy||elapsed>=Duration){Finish();return;}
        FilterContacts();
        foreach(var body in touched)Limit(body);
        elapsed+=seconds;mover.MovePosition(Lift(origin,elapsed));
    }
    void OnCollisionEnter(Collision collision){Touch(collision.rigidbody);}
    void OnCollisionStay(Collision collision){Touch(collision.rigidbody);}
    void Touch(Rigidbody body){if(!Eligible(body))return;touched.Add(body);Limit(body);}
    static void Limit(Rigidbody body){if(!body||body.isKinematic)return;body.velocity=Vector3.ClampMagnitude(body.velocity,MaxSpeed);body.angularVelocity=Vector3.ClampMagnitude(body.angularVelocity,MaxSpin);}
    void Finish(){
        if(shape)shape.enabled=false;
        foreach(var body in touched)Limit(body);
        if(!ReferenceEquals(item,null)&&active.TryGetValue(item,out var current)&&current==this)active.Remove(item);
        Destroy(gameObject);
    }
    public static void Stop(GameObject selected){if(!ReferenceEquals(selected,null)&&active.TryGetValue(selected,out var contact)&&contact)contact.Finish();}
    public static void Clear(){foreach(var contact in new List<ProbeLiftContact>(active.Values))if(contact)contact.Finish();active.Clear();}
    void OnDestroy(){if(!ReferenceEquals(item,null)&&active.TryGetValue(item,out var current)&&current==this)active.Remove(item);}
}
