using System;
using System.IO;
using UnityEditor;
using UnityEngine;

// Deliberately small, explicit vocabulary. Unknown assets remain medium and are labelled for review.
public static class ModelAdapterSizing {
    public struct Suggestion { public string tier,reason; public float longest; }
    public static Suggestion Suggest(string id,Vector3 bounds) {
        string key=(id??"").Replace("_","").Replace("-","").ToLowerInvariant();
        string tier="中";bool known=true;
        switch(key){
            case "egg":case "saltedegg":case "egg2":case "apple":case "applegreen":case "orange":case "garlic":case "chocolatebar":tier="小";break;
            case "bottle":case "pumpkin":case "pizza":case "watermelon":case "thermosbottle":tier="大";break;
            case "avocado":case "banana":case "bread":case "broccoli":case "burger":case "chickenleg":case "corndog":case "croissant":case "cupcake":case "donut":case "eggplant":case "fries":case "hotdog":case "icecream":case "popsicle":case "soda":case "steak":break;
            default:known=false;break;
        }
        float longest=tier=="小"?1.3f:tier=="大"?2.6f:1.9f;
        float max=Mathf.Max(bounds.x,bounds.y,bounds.z);
        float fullness=max>0?bounds.x*bounds.y*bounds.z/(max*max*max):0;
        // Limit correction to 10%; don't infer real-world dimensions from source file units.
        float factor=max<=0?1:fullness>.55f?.9f:fullness<.12f?1.1f:1;
        return new Suggestion{tier=tier,longest=Mathf.Round(longest*factor*100)/100,
            reason=(known?"名称分类":"未识别名称，默认中档")+(factor<1?"；饱满形状缩小 10%":factor>1?"；细长/扁平形状增大 10%":"；保持档位尺寸")};
    }
    public static void Apply(ItemCatalog.Item item,ModelAdapterReport report){
        // Reimport only initializes old/unclassified entries. Saved automatic baseline stays stable.
        if(!string.IsNullOrEmpty(item.sizeTier))return;
        var suggestion=Suggest(report.logicalId,report.visualSize);
        if(item.trayBaseScale<=0)item.trayBaseScale=item.normalizedToSource>0?item.normalizedToSource:ModelAdapterRules.DefaultGameplayScale;
        item.sizeTier=suggestion.tier;item.sizeReason=suggestion.reason;
        item.normalizedToSource=suggestion.longest/ModelAdapterRules.SourceLongestSide;
    }
    [MenuItem("Goose/初始化导入模型大小层次")]
    public static void MigrateExisting(){
        var data=JsonUtility.FromJson<ItemCatalog.Data>(File.ReadAllText(ModelAdapterService.CatalogPath));
        int changed=0;
        foreach(var item in data.items){
            if(!string.IsNullOrEmpty(item.sizeTier))continue;
            string path=Path.Combine(Path.GetDirectoryName(item.prefabPath),"report.json");
            if(!File.Exists(path))continue;
            Apply(item,JsonUtility.FromJson<ModelAdapterReport>(File.ReadAllText(path)));changed++;
        }
        if(changed>0){File.WriteAllText(ModelAdapterService.CatalogPath,JsonUtility.ToJson(data,true));AssetDatabase.ImportAsset(ModelAdapterService.CatalogPath);BoardLayoutSettings.ClearCache();}
        Debug.Log("BOARD_SIZE_MIGRATION "+changed+" items; prefabs and tray poses preserved.");
    }
}
