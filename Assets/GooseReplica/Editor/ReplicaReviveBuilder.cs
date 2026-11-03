using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
public static class ReplicaReviveBuilder {
    const string Root="Assets/GooseReplica/Resources/Replica/";
    static RectTransform Rect(string name,Transform parent,float x,float y,float w,float h){var r=new GameObject(name,typeof(RectTransform)).GetComponent<RectTransform>();r.SetParent(parent,false);r.sizeDelta=new Vector2(w,h);r.anchoredPosition=new Vector2(x,y);return r;}
    static Image Pic(string name,string asset,Transform parent,float x,float y){var s=AssetDatabase.LoadAssetAtPath<Sprite>(Root+"ReviveSkinPS/"+asset+".png");if(!s)throw new Exception("Missing revive sprite "+asset);var im=Rect(name,parent,x,y,s.rect.width,s.rect.height).gameObject.AddComponent<Image>();im.sprite=s;im.raycastTarget=false;return im;}
    static Button Button(Image im){im.raycastTarget=true;var b=im.gameObject.AddComponent<Button>();b.targetGraphic=im;return b;}
    [MenuItem("Goose Replica/UI/Build Revive Screens")]
    public static void Build(){
        AssetDatabase.Refresh();foreach(var id in AssetDatabase.FindAssets("t:Texture2D",new[]{Root+"ReviveSkinPS"})){
            var imp=(TextureImporter)AssetImporter.GetAtPath(AssetDatabase.GUIDToAssetPath(id));imp.textureType=TextureImporterType.Sprite;imp.spriteImportMode=SpriteImportMode.Single;imp.spritePixelsPerUnit=100;imp.mipmapEnabled=false;imp.alphaIsTransparency=true;imp.npotScale=TextureImporterNPOTScale.None;imp.maxTextureSize=1024;imp.textureCompression=TextureImporterCompression.Uncompressed;imp.SaveAndReimport();
        }
        BuildOne(false);BuildOne(true);AssetDatabase.SaveAssets();
    }
    static void BuildOne(bool timeout){
        string name=timeout?"ReviveTime":"ReviveClear";
        var root=Rect(name,null,0,0,720,1280);var shade=root.gameObject.AddComponent<Image>();shade.color=new Color(0,0,0,.64f);
        root.gameObject.AddComponent<ReplicaUIView>().bindings=new ReplicaUIView.Binding[0];
        var fit=Rect("720 x 1280 layout",root,0,0,720,1280);root.gameObject.AddComponent<ReplicaUIFitContent>().content=fit;
        var c=Rect("Animated content",fit,0,0,720,1280);var v=root.gameObject.AddComponent<ReplicaReviveView>();v.shade=shade;v.animatedContent=c;v.group=c.gameObject.AddComponent<CanvasGroup>();
        Pic("Panel","panel",c,0,0);
        var label=Rect("Completion",c,0,405,660,76).gameObject.AddComponent<Text>();label.font=AssetDatabase.LoadAssetAtPath<Font>(Root+"Fonts/DefaultChinese.otf");label.fontSize=48;label.fontStyle=FontStyle.Bold;label.color=Color.white;label.alignment=TextAnchor.MiddleCenter;label.verticalOverflow=VerticalWrapMode.Overflow;label.raycastTarget=false;label.text="关卡已完成：58%";v.progress=label;
        Pic("Icon",timeout?"clock":"broom",c,0,101);
        Pic("Description",timeout?"time-message":"clear-message",c,0,-93);
        var status=Rect("Ad status",c,0,-307,510,45).gameObject.AddComponent<Text>();status.font=label.font;status.fontSize=23;status.color=new Color32(30,112,173,255);status.alignment=TextAnchor.MiddleCenter;status.verticalOverflow=VerticalWrapMode.Overflow;status.raycastTarget=false;status.text="观看完整视频后复活";v.status=status;
        v.revive=Button(Pic("Revive","revive-button",c,0,-224));v.close=Button(Pic("Close","close",c,285,309));
        PrefabUtility.SaveAsPrefabAsset(root.gameObject,Root+"UI/"+name+".prefab");UnityEngine.Object.DestroyImmediate(root.gameObject);
    }
}
