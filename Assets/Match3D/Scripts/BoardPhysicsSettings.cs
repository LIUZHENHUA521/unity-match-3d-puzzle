using UnityEngine;

// Adapted compound bodies need enough contact resolution to settle before sleep.
public static class BoardPhysicsSettings {
    public const float GravityY=-19.81f,Mass=.05f,Drag=0,AngularDrag=.05f,Sleep=.005f,Contact=.02f,Depenetration=10,Bounce=100;
    public static bool Active {get;private set;}
    static Vector3 gravity;
    static float sleep,contact,depenetration,bounce;
    static int solver,velocitySolver;
    static PhysicMaterial surface;
    public static void Begin(){
        if(Active)return;
        gravity=Physics.gravity;sleep=Physics.sleepThreshold;contact=Physics.defaultContactOffset;
        depenetration=Physics.defaultMaxDepenetrationVelocity;bounce=Physics.bounceThreshold;
        solver=Physics.defaultSolverIterations;velocitySolver=Physics.defaultSolverVelocityIterations;
        Active=true;
        Physics.gravity=new Vector3(0,GravityY,0);Physics.sleepThreshold=Sleep;Physics.defaultContactOffset=Contact;
        Physics.defaultMaxDepenetrationVelocity=Depenetration;Physics.bounceThreshold=Bounce;
        Physics.defaultSolverIterations=12;Physics.defaultSolverVelocityIterations=4;
    }
    public static void Apply(Rigidbody body){
        if(!Active||!body)return;
        body.mass=Mass;body.drag=Drag;body.angularDrag=AngularDrag;body.useGravity=true;
        body.interpolation=RigidbodyInterpolation.None;body.collisionDetectionMode=body.GetComponent<BoardItemIdentity>()?CollisionDetectionMode.ContinuousDynamic:CollisionDetectionMode.Discrete;
        body.sleepThreshold=Sleep;body.maxDepenetrationVelocity=Depenetration;
        body.solverIterations=12;body.solverVelocityIterations=4;
        var token=body.GetComponent<BoardItemPoolToken>();token?.CacheComponents();
        foreach(var c in token?token.Colliders:body.GetComponentsInChildren<Collider>(true)){
            c.contactOffset=Contact;
            // Source project default material is ObjMa, including colliders with no explicit material.
            if(!c.sharedMaterial)c.sharedMaterial=DefaultSurface;
        }
    }
    public static PhysicMaterial DefaultSurface {get{
        if(!surface)surface=new PhysicMaterial("Source ObjMa"){dynamicFriction=.6f,staticFriction=.6f,bounciness=0,frictionCombine=(PhysicMaterialCombine)1,bounceCombine=(PhysicMaterialCombine)1};
        return surface;
    }}
    public static void End(){
        if(!Active)return;
        Physics.gravity=gravity;Physics.sleepThreshold=sleep;Physics.defaultContactOffset=contact;
        Physics.defaultMaxDepenetrationVelocity=depenetration;Physics.bounceThreshold=bounce;
        Physics.defaultSolverIterations=solver;Physics.defaultSolverVelocityIterations=velocitySolver;Active=false;
    }
}
