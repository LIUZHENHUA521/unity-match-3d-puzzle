using System;
using System.Collections.Generic;
using UnityEngine;

// WX 2.1.12: NetInitManager.isUseNewAnim=false -> Awake -> RefreshContentDisplay.
// This describes the shipped default; the separately recovered new animation is not selected.
public static class ItemFlightMotion {
    public const float MoveSeconds=.3f, ScalePeakSeconds=.1f, ScalePeak=1.4f;
    public const float PunchSeconds=.2f,PunchScale=.03f,HideCollidersSeconds=.05f;
    public const float MergeSeconds=.2f,ShrinkSeconds=.2f,FinalRemovalDelay=.05f;
    public const float MatchCheckSeconds=MoveSeconds+MergeSeconds+ShrinkSeconds;
    public const float MagnetShakeSeconds=.11f,MagnetLiftSeconds=.12f,MagnetPullSeconds=.32f,MagnetLandSeconds=.1f;
    public const float MagnetSeconds=MagnetShakeSeconds+MagnetLiftSeconds+MagnetPullSeconds+MagnetLandSeconds;
    public const float MagnetStaggerSeconds=.07f;
    public static Vector3 MagnetPosition(Vector3 start,Vector3 slot,Vector3 up,Vector3 right,float spacing,float elapsed){
        float t=Mathf.Clamp(elapsed,0,MagnetSeconds);
        if(t<MagnetShakeSeconds){
            float u=t/MagnetShakeSeconds;
            return start+right*(Mathf.Sin(u*Mathf.PI*4)*.035f*spacing*Mathf.Sin(u*Mathf.PI));
        }
        var lifted=start+up*(.32f*spacing);
        if(t<MagnetShakeSeconds+MagnetLiftSeconds){
            float u=(t-MagnetShakeSeconds)/MagnetLiftSeconds;
            return Vector3.Lerp(start,lifted,1-(1-u)*(1-u));
        }
        float pullStart=MagnetShakeSeconds+MagnetLiftSeconds;
        if(t<pullStart+MagnetPullSeconds){
            float u=(t-pullStart)/MagnetPullSeconds,q=u*u;
            float side=Vector3.Dot(start-slot,right)>=0?1:-1;
            var control=lifted+up*(.45f*spacing)+right*(side*.28f*spacing);
            return (1-q)*(1-q)*lifted+2*(1-q)*q*control+q*q*slot;
        }
        float landing=(t-pullStart-MagnetPullSeconds)/MagnetLandSeconds;
        return slot+up*(Mathf.Sin(Mathf.Clamp01(landing)*Mathf.PI)*.065f*spacing);
    }
    public static Vector3 MagnetScale(Vector3 initial,Vector3 target,float elapsed){
        float t=Mathf.Clamp(elapsed,0,MagnetSeconds),pullStart=MagnetShakeSeconds+MagnetLiftSeconds;
        if(t<pullStart)return initial*(1+.1f*Mathf.SmoothStep(0,1,t/pullStart));
        float u=(t-pullStart)/MagnetPullSeconds;
        if(u<1)return Vector3.Lerp(initial*1.1f,target,u*u);
        return target*(1+.06f*Mathf.Sin(Mathf.Clamp01((t-pullStart-MagnetPullSeconds)/MagnetLandSeconds)*Mathf.PI));
    }
    [Serializable] class ItemList {public Item[] items;}
    [Serializable] class Item {public string name;public float pickScale,normalizedToPickScale;}
    static Dictionary<string,Item> data;
    static readonly Dictionary<GameObject,float> fallbackScales=new Dictionary<GameObject,float>();
    static readonly Dictionary<GameObject,float> rawScales=new Dictionary<GameObject,float>();
    static readonly Dictionary<GameObject,float> scales=new Dictionary<GameObject,float>();
    static void Load(){
        if(data==null){
            data=new Dictionary<string,Item>();var asset=Resources.Load<TextAsset>("Match3D/SourceFlightItems");
            if(asset)foreach(var item in JsonUtility.FromJson<ItemList>(asset.text).items)data[item.name]=item;
        }
    }
    public static Vector3 ScaleForPrefab(string sourceName,Vector3 boardScale,float spacing){
        Load();return Vector3.one*(data.TryGetValue(sourceName,out var source)?source.normalizedToPickScale*spacing/.71f:boardScale.x*.6f/Match3DFlow.BoardItemScale);
    }
    public static Vector3 ScaleForPrefab(GameObject prefab,float spacing){
        string id=Match3DTrayDisplaySettings.AdapterId(prefab);
        float baseline=BoardLayoutSettings.BaseModelScale(prefab);
        if(!string.IsNullOrEmpty(id)&&ItemCatalog.TryGet(id,out var item))
            baseline=(item.trayBaseScale>0?item.trayBaseScale:item.normalizedToSource)*BoardLayoutSettings.ModelSizeMultiplier;
        return ScaleForPrefab(prefab.name,prefab.transform.localScale*baseline,spacing);
    }
    public static void Bind(GameObject obj,string sourceName,bool layered,GameObject prefab=null){
        Load();Forget(obj);
        if(prefab){
            string id=Match3DTrayDisplaySettings.AdapterId(prefab);
            float baseline=BoardLayoutSettings.BaseModelScale(prefab);
            if(!string.IsNullOrEmpty(id)&&ItemCatalog.TryGet(id,out var item))
                baseline=(item.trayBaseScale>0?item.trayBaseScale:item.normalizedToSource)*BoardLayoutSettings.ModelSizeMultiplier;
            fallbackScales[obj]=prefab.transform.localScale.x*baseline*.6f/Match3DFlow.BoardItemScale;
        }
        if(layered&&data.TryGetValue(sourceName,out var source)){scales[obj]=source.normalizedToPickScale;rawScales[obj]=source.pickScale;}
    }
    public static void Forget(GameObject obj){if(!ReferenceEquals(obj,null)){scales.Remove(obj);rawScales.Remove(obj);fallbackScales.Remove(obj);}}
    public static Vector3 StoreScale(GameObject obj,Vector3 fallback){
        var display=Match3DTrayDisplayBinding.For(obj);
        if(display)fallback=PickScale(obj,display.boardScale);
        else if(fallbackScales.ContainsKey(obj))fallback=PickScale(obj,fallback);
        return rawScales.TryGetValue(obj,out float pick)&&pick>0?PickScale(obj,fallback)*(Mathf.Min(1.4f,pick*1.5f)/pick):fallback;
    }
    public static Vector3 PickScale(GameObject obj,Vector3 boardScale){
        // Existing tray adapts to the safe area. Scale the source model by the same spacing ratio;
        // no per-model bounding-box cap. Original ContentPointList local spacing is .71 and parent Content scale is 1.4.
        float ratio=Match3DTray.Instance?Vector3.Distance(Match3DTray.Instance.SlotPosition(0),Match3DTray.Instance.SlotPosition(1))/.71f:1f;
        if(scales.TryGetValue(obj,out float size))return Vector3.one*(size*ratio);
        // Fallback models store a 1x world scale, unlike source models whose scale
        // already follows the live slot spacing. Compensate only this path so
        // newly collected models match items that were in the tray before zooming.
        float fallback=fallbackScales.TryGetValue(obj,out float cached)?cached:boardScale.x*.6f/Match3DFlow.BoardItemScale;
        var zoom=BoardCameraZoom.Instance;
        if(zoom&&zoom.Available)fallback/=zoom.Zoom;
        return Vector3.one*fallback;
    }
}
