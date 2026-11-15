using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class PotSameGamesBuilder {
    const string Root="Assets/Match3D/Resources/Match3D/";
    static RectTransform Rect(string name,Transform parent,float x,float y,float w,float h) {
        var rect=new GameObject(name,typeof(RectTransform)).GetComponent<RectTransform>();
        rect.SetParent(parent,false);rect.anchoredPosition=new Vector2(x,y);rect.sizeDelta=new Vector2(w,h);return rect;
    }
    static Image Picture(string name,string asset,Transform parent,float x,float y,float w,float h) {
        var sprite=AssetDatabase.LoadAssetAtPath<Sprite>(Root+asset+".png");
        if(!sprite)throw new InvalidOperationException("Missing same-games sprite: "+asset);
        var image=Rect(name,parent,x,y,w,h).gameObject.AddComponent<Image>();
        image.sprite=sprite;image.raycastTarget=false;return image;
    }
    static Button Button(Image image) {
        image.raycastTarget=true;var button=image.gameObject.AddComponent<Button>();button.targetGraphic=image;return button;
    }
    static Match3DUIView.Binding Bind(string key,Component target)=>new Match3DUIView.Binding{key=key,target=target.gameObject};
    [MenuItem("Pot/UI/Build Same Games")]
    public static void Build() {
        AssetDatabase.Refresh();
        // Only the new entry/cards/title are imported; the shared popup art is untouched.
        foreach(var guid in AssetDatabase.FindAssets("t:Texture2D",new[]{Root+"PotSameGamesSkin"})) {
            var importer=(TextureImporter)AssetImporter.GetAtPath(AssetDatabase.GUIDToAssetPath(guid));
            importer.textureType=TextureImporterType.Sprite;importer.spriteImportMode=SpriteImportMode.Single;
            importer.mipmapEnabled=false;importer.alphaIsTransparency=true;importer.isReadable=false;
            importer.maxTextureSize=1024;importer.textureCompression=TextureImporterCompression.CompressedHQ;importer.SaveAndReimport();
        }
        var root=Rect("PotSameGames",null,0,0,720,1280);
        try {
            root.gameObject.AddComponent<Image>().color=new Color(0,0,0,.64f);
            var content=Rect("Content - edit UI here",root,0,0,720,940);
            root.gameObject.AddComponent<Match3DUIFitContent>().content=content;
            var ui=root.gameObject.AddComponent<Match3DUIView>();
            Picture("Shared rainbow panel","InviteSkinPS/弹窗背景",content,0,0,592,772);
            Picture("Title","PotSameGamesSkin/pot-title",content,0,251,254,62);
            var close=Button(Picture("Close","InviteSkinPS/img_exit_x",content,262,264,81,81));
            var crazy=Row(content,"CrazyCombo","pot-crazy-row",94);
            var current=Row(content,"CurrentGame","pot-current-row",-86);
            var text=Rect("Coming soon",content,0,-266,480,64).gameObject.AddComponent<Text>();
            text.font=AssetDatabase.LoadAssetAtPath<Font>(Root+"Fonts/DefaultChinese.otf");
            text.text="尽情期待";text.fontSize=32;text.fontStyle=FontStyle.Bold;
            text.color=new Color32(44,139,204,255);text.alignment=TextAnchor.MiddleCenter;text.raycastTarget=false;
            ui.bindings=new[]{Bind("Close",close),Bind("CrazyCombo",crazy),Bind("CurrentGame",current)};
            PrefabUtility.SaveAsPrefabAsset(root.gameObject,Root+"UI/PotSameGames.prefab");
        } finally { UnityEngine.Object.DestroyImmediate(root.gameObject); }
        InstallHomeEntry();AssetDatabase.SaveAssets();
    }
    static Button Row(Transform parent,string name,string asset,float y) {
        var row=Picture(name,"PotSameGamesSkin/"+asset,parent,.5f,y,521,174);
        // The source card includes its button artwork. Only that button is interactive.
        var hit=Rect("Action",row.transform,170,-16.5f,153,82).gameObject.AddComponent<Image>();
        hit.color=Color.white;var button=Button(hit);var colors=button.colors;
        colors.normalColor=colors.highlightedColor=colors.selectedColor=new Color(0,0,0,0);
        colors.pressedColor=new Color(0,0,0,.12f);button.colors=colors;
        return button;
    }
    static void InstallHomeEntry() {
        string path=Root+"UI/Home.prefab";var root=PrefabUtility.LoadPrefabContents(path);
        try {
            var home=root.GetComponent<Match3DHomeView>();
            var existing=home.artwork.Find("PotSameGamesEntry");
            if(existing)UnityEngine.Object.DestroyImmediate(existing.gameObject);
            var entry=Button(Picture("PotSameGamesEntry","PotSameGamesSkin/pot-entry",home.artwork,631.5f,-438,153,145));
            var rect=(RectTransform)entry.transform;rect.anchorMin=rect.anchorMax=new Vector2(0,1);
            home.sameGames=entry;
            PrefabUtility.SaveAsPrefabAsset(root,path);
        } finally { PrefabUtility.UnloadPrefabContents(root); }
    }
}
