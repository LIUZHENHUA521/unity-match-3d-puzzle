using UnityEngine;

// WX2.1.12 PhysicsManager + item prefabs. Ordinary mode; real-physics creation's -50 branch is not enabled.
public static class ReplicaSourcePhysics {
    public const float GravityY=-19.81f,Mass=.05f,Drag=0,AngularDrag=.05f,Sleep=.2f,Contact=.02f,Depenetration=1,Bounce=100;
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
        Physics.defaultSolverIterations=6;Physics.defaultSolverVelocityIterations=1;
    }
    public static void Apply(Rigidbody body){
        if(!Active||!body)return;
        body.mass=Mass;body.drag=Drag;body.angularDrag=AngularDrag;body.useGravity=true;
        body.interpolation=RigidbodyInterpolation.None;body.collisionDetectionMode=CollisionDetectionMode.Discrete;
        body.sleepThreshold=Sleep;body.maxDepenetrationVelocity=Depenetration;
        body.solverIterations=6;body.solverVelocityIterations=1;
        foreach(var c in body.GetComponentsInChildren<Collider>(true)){
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
