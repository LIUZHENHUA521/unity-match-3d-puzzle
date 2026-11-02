using System;
using UnityEngine;

// Per-instance state only; never changes model geometry or its collider hierarchy.
public sealed class ReplicaTrayDisplayBinding : MonoBehaviour {
    [NonSerialized] public ReplicaTrayDisplaySettings.Pose pose;
    [NonSerialized] public Vector3 boardScale;
    [NonSerialized] public Quaternion boardRotation;
    public static void Bind(GameObject obj,GameObject prefab){
        var binding=obj.GetComponent<ReplicaTrayDisplayBinding>();
        var settings=ReplicaTrayDisplaySettings.Current;
        if(settings&&settings.TryGet(prefab,out var configured)){
            if(!binding)binding=obj.AddComponent<ReplicaTrayDisplayBinding>();
            binding.pose=configured.Copy();binding.CaptureBoardPose();
        }else if(binding)binding.pose=null;
    }
    public void CaptureBoardPose(){boardScale=transform.localScale;boardRotation=transform.rotation;}
    public static ReplicaTrayDisplayBinding For(GameObject obj){var b=obj?obj.GetComponent<ReplicaTrayDisplayBinding>():null;return b&&b.pose!=null?b:null;}
}
