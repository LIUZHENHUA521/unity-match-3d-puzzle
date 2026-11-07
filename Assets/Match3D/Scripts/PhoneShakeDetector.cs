using UnityEngine;

// WX 2.1.12 LevelManager.ShakePhone (j83399): continuous force, not ShakeObj.
public static class PhoneShakeDetector {
    public const float Threshold=2f,MaxExcess=4f;
    public const float DefaultForce=7f,IosForce=6f;
    public static Vector3 Force(Vector3 acceleration,float interval,float fixedDeltaTime,bool ios){
        float magnitude=acceleration.magnitude;
        if(float.IsNaN(magnitude)||float.IsInfinity(magnitude)||float.IsNaN(interval)||float.IsInfinity(interval)||
            fixedDeltaTime<=0||float.IsNaN(fixedDeltaTime)||float.IsInfinity(fixedDeltaTime)||magnitude<=Threshold)return Vector3.zero;
        // Source maps accelerometer x/y/z to world x/z/y, preserving signs.
        var direction=new Vector3(acceleration.x,acceleration.z,acceleration.y)/magnitude;
        float timing=Mathf.Clamp(interval,.033f,1f)*5f*6f/fixedDeltaTime*.02f;
        return direction*(Mathf.Clamp(magnitude-Threshold,0,MaxExcess)*(ios?IosForce:DefaultForce)*timing);
    }
}
