using System;
using System.Collections.Generic;
using UnityEngine;

// Legacy breakfast radius helper for historical diagnostics only. Runtime coordinates and walls now
// come from BoardEnvironment per theme. Shared visibility and model-size values remain here.
public static class BoardLayoutSettings {
    public const float VisibleMin=4,VisibleMax=5,MinVisibleY=21,MaxHeightStep=.5f,GrowSeconds=.75f,OptimizationInterval=1f;
    public static readonly float[] Heights={0,3,6,9,12,15,18,21,24,30};
    public static readonly float[] Widths={1.399999976f,1.620112658f,1.840225339f,2.060338020f,2.280450583f,3.173188448f,4.065925598f,4.958663464f,5.851400852f,5.851400852f};
    public static float HalfExtent(float height){
        for(int i=1;i<Heights.Length;i++)if(height<=Heights[i])return Mathf.Lerp(Widths[i-1],Widths[i],Mathf.InverseLerp(Heights[i-1],Heights[i],height))*.45f;
        return Widths[Widths.Length-1]*.45f;
    }
    [Serializable] class Sizes {public Size[] items;}
    [Serializable] class Size {public string name;public float normalizedToSource;}
    static Dictionary<string,float> sizes;
    public const string ModelSizeKey="goose.modelSizeMultiplier";
    public static float ModelSizeMultiplier {
        get => Mathf.Clamp(PlayerPrefs.GetFloat(ModelSizeKey,1f),.5f,1.25f);
        set {PlayerPrefs.SetFloat(ModelSizeKey,Mathf.Round(Mathf.Clamp(value,.5f,1.25f)*100f)/100f);PlayerPrefs.Save();}
    }
    public static float ModelScale(GameObject prefab){
        float multiplier=Match3DTrayDisplaySettings.Current?Match3DTrayDisplaySettings.Current.BoardMultiplier(prefab):1;
        return BaseModelScale(prefab)*multiplier;
    }
    public static float BaseModelScale(GameObject prefab){
        string id=Match3DTrayDisplaySettings.AdapterId(prefab);
        if(!string.IsNullOrEmpty(id)&&ItemCatalog.TryGetSize(id,out float value))return value*ModelSizeMultiplier;
        return OriginalScale(prefab.name)*ModelSizeMultiplier;
    }
    public static float ModelScale(string name){
        if(ItemCatalog.TryGetSize(name,out float value))return value*ModelSizeMultiplier;
        return OriginalScale(name)*ModelSizeMultiplier;
    }
    static float OriginalScale(string name){
        if(sizes==null){sizes=new Dictionary<string,float>(StringComparer.OrdinalIgnoreCase);var data=Resources.Load<TextAsset>("Match3D/SourceSizes");if(data)foreach(var item in JsonUtility.FromJson<Sizes>(data.text).items)sizes[item.name]=item.normalizedToSource;}
        return sizes.TryGetValue(name,out float value)?value:Match3DFlow.BoardItemScale;
    }
    public static void ClearCache(){sizes=null;ItemCatalog.ClearCache();}
}
