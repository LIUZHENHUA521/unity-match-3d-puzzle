using UnityEngine;

// Acceleration is in g on both Unity mobile and WeChat. Require a reversal,
// rather than treating a single bump or an orientation change as a shake.
public sealed class PhoneShakeDetector {
    public const float Threshold=1.1f, Cooldown=2f;
    Vector3 gravity,firstImpulse;
    float lastSample,firstTime,nextAllowed;
    bool initialized,hasImpulse;
    public void Reset(){initialized=false;hasImpulse=false;}
    public bool Sample(Vector3 acceleration,float now){
        if(float.IsNaN(acceleration.sqrMagnitude)||float.IsInfinity(acceleration.sqrMagnitude))return false;
        if(!initialized||now-lastSample>.25f||now<lastSample){
            gravity=acceleration;lastSample=now;initialized=true;hasImpulse=false;return false;
        }
        float dt=now-lastSample;lastSample=now;
        if(dt<=0)return false;
        gravity=Vector3.Lerp(gravity,acceleration,dt/(.25f+dt));
        var impulse=acceleration-gravity;
        if(now<nextAllowed){hasImpulse=false;return false;}
        if(hasImpulse&&now-firstTime>.45f)hasImpulse=false;
        if(impulse.sqrMagnitude<Threshold*Threshold)return false;
        if(hasImpulse&&now-firstTime>=.06f&&Vector3.Dot(firstImpulse,impulse)<-.25f*firstImpulse.magnitude*impulse.magnitude){
            hasImpulse=false;nextAllowed=now+Cooldown;return true;
        }
        if(!hasImpulse){firstImpulse=impulse;firstTime=now;hasImpulse=true;}
        return false;
    }
}
