using System;
using System.Collections.Generic;
using UnityEngine;

// WX 2.1.12: NetInitManager.isUseNewAnim=false -> Awake -> RefreshContentDisplay.
// This describes the shipped default; the separately recovered new animation is not selected.
public static class ReplicaSourceFlight {
    public const float MoveSeconds=.3f, ScalePeakSeconds=.1f, ScalePeak=1.4f;
    public const float PunchSeconds=.2f,PunchScale=.03f,HideCollidersSeconds=.05f;
    public const float MergeSeconds=.2f,ShrinkSeconds=.2f,FinalRemovalDelay=.05f;
    public const float MatchCheckSeconds=MoveSeconds+MergeSeconds+ShrinkSeconds;
    [Serializable] class ItemList {public Item[] items;}
    [Serializable] class Item {public string name;public float pickScale,normalizedToPickScale;}
    static Dictionary<string,Item> data;
    static readonly Dictionary<GameObject,float> rawScales=new Dictionary<GameObject,float>();
    static readonly Dictionary<GameObject,float> scales=new Dictionary<GameObject,float>();
    public static void Bind(GameObject obj,string sourceName,bool layered){
        if(data==null){
            data=new Dictionary<string,Item>();var asset=Resources.Load<TextAsset>("Replica/SourceFlightItems");
            if(asset)foreach(var item in JsonUtility.FromJson<ItemList>(asset.text).items)data[item.name]=item;
        }
        if(layered&&data.TryGetValue(sourceName,out var source)){scales[obj]=source.normalizedToPickScale;rawScales[obj]=source.pickScale;}
    }
    public static void Forget(GameObject obj){if(!ReferenceEquals(obj,null)){scales.Remove(obj);rawScales.Remove(obj);}}
    public static Vector3 StoreScale(GameObject obj,Vector3 fallback){
        return rawScales.TryGetValue(obj,out float pick)&&pick>0?PickScale(obj,fallback)*(Mathf.Min(1.4f,pick*1.5f)/pick):fallback;
    }
    public static Vector3 PickScale(GameObject obj,Vector3 boardScale){
        // Existing tray adapts to the safe area. Scale the source model by the same spacing ratio;
        // no per-model bounding-box cap. Original ContentPointList local spacing is .71 and parent Content scale is 1.4.
        float ratio=ReplicaTray.Instance?Vector3.Distance(ReplicaTray.Instance.SlotPosition(0),ReplicaTray.Instance.SlotPosition(1))/.71f:1f;
        return Vector3.one*(scales.TryGetValue(obj,out float size)?size*ratio:boardScale.x*.6f/ReplicaFlow.BoardItemScale);
    }
}
