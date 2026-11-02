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
    static readonly Dictionary<GameObject,float> fallbackScales=new Dictionary<GameObject,float>();
    static readonly Dictionary<GameObject,float> rawScales=new Dictionary<GameObject,float>();
    static readonly Dictionary<GameObject,float> scales=new Dictionary<GameObject,float>();
    static void Load(){
        if(data==null){
            data=new Dictionary<string,Item>();var asset=Resources.Load<TextAsset>("Replica/SourceFlightItems");
            if(asset)foreach(var item in JsonUtility.FromJson<ItemList>(asset.text).items)data[item.name]=item;
        }
    }
    public static Vector3 ScaleForPrefab(string sourceName,Vector3 boardScale,float spacing){
        Load();return Vector3.one*(data.TryGetValue(sourceName,out var source)?source.normalizedToPickScale*spacing/.71f:boardScale.x*.6f/ReplicaFlow.BoardItemScale);
    }
    public static Vector3 ScaleForPrefab(GameObject prefab,float spacing){
        string id=ReplicaTrayDisplaySettings.AdapterId(prefab);
        float baseline=ReplicaSourceProfile.BaseModelScale(prefab);
        if(!string.IsNullOrEmpty(id)&&ReplicaSourceCatalog.TryGet(id,out var item))
            baseline=(item.trayBaseScale>0?item.trayBaseScale:item.normalizedToSource)*ReplicaSourceProfile.ModelSizeMultiplier;
        return ScaleForPrefab(prefab.name,prefab.transform.localScale*baseline,spacing);
    }
    public static void Bind(GameObject obj,string sourceName,bool layered,GameObject prefab=null){
        Load();Forget(obj);
        if(prefab){
            string id=ReplicaTrayDisplaySettings.AdapterId(prefab);
            float baseline=ReplicaSourceProfile.BaseModelScale(prefab);
            if(!string.IsNullOrEmpty(id)&&ReplicaSourceCatalog.TryGet(id,out var item))
                baseline=(item.trayBaseScale>0?item.trayBaseScale:item.normalizedToSource)*ReplicaSourceProfile.ModelSizeMultiplier;
            fallbackScales[obj]=prefab.transform.localScale.x*baseline*.6f/ReplicaFlow.BoardItemScale;
        }
        if(layered&&data.TryGetValue(sourceName,out var source)){scales[obj]=source.normalizedToPickScale;rawScales[obj]=source.pickScale;}
    }
    public static void Forget(GameObject obj){if(!ReferenceEquals(obj,null)){scales.Remove(obj);rawScales.Remove(obj);fallbackScales.Remove(obj);}}
    public static Vector3 StoreScale(GameObject obj,Vector3 fallback){
        var display=ReplicaTrayDisplayBinding.For(obj);
        if(display)fallback=PickScale(obj,display.boardScale);
        else if(fallbackScales.ContainsKey(obj))fallback=PickScale(obj,fallback);
        return rawScales.TryGetValue(obj,out float pick)&&pick>0?PickScale(obj,fallback)*(Mathf.Min(1.4f,pick*1.5f)/pick):fallback;
    }
    public static Vector3 PickScale(GameObject obj,Vector3 boardScale){
        // Existing tray adapts to the safe area. Scale the source model by the same spacing ratio;
        // no per-model bounding-box cap. Original ContentPointList local spacing is .71 and parent Content scale is 1.4.
        float ratio=ReplicaTray.Instance?Vector3.Distance(ReplicaTray.Instance.SlotPosition(0),ReplicaTray.Instance.SlotPosition(1))/.71f:1f;
        return Vector3.one*(scales.TryGetValue(obj,out float size)?size*ratio:fallbackScales.TryGetValue(obj,out float fallback)?fallback:boardScale.x*.6f/ReplicaFlow.BoardItemScale);
    }
}
