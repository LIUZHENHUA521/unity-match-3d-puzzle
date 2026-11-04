using System;
using UnityEngine;

// Per-instance state only; never changes model geometry or its collider hierarchy.
public sealed class Match3DTrayDisplayBinding : MonoBehaviour {
    [NonSerialized] public Match3DTrayDisplaySettings.Pose pose;
    [NonSerialized] public Vector3 boardScale;
    [NonSerialized] public Quaternion boardRotation;
    public static void Bind(GameObject obj,GameObject prefab){
        var binding=obj.GetComponent<Match3DTrayDisplayBinding>();
        var settings=Match3DTrayDisplaySettings.Current;
        if(settings&&settings.TryGet(prefab,out var configured)){
            if(!binding)binding=obj.AddComponent<Match3DTrayDisplayBinding>();
            binding.pose=configured.Copy();binding.CaptureBoardPose();
        }else if(binding)binding.pose=null;
    }
    public void CaptureBoardPose(){boardScale=transform.localScale;boardRotation=transform.rotation;}
    public static Match3DTrayDisplayBinding For(GameObject obj){var b=obj?obj.GetComponent<Match3DTrayDisplayBinding>():null;return b&&b.pose!=null?b:null;}
}
