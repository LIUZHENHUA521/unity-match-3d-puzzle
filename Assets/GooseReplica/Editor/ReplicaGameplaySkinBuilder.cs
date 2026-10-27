using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class ReplicaGameplaySkinBuilder {
    const string Art="Assets/GooseReplica/Resources/Replica/GameplaySkin/";
    const string UI="Assets/GooseReplica/Resources/Replica/UI/";
    static Sprite Sprite(string name)=>AssetDatabase.LoadAssetAtPath<Sprite>(Art+name+".png");
    static RectTransform Rect(string name,Transform parent,Vector2 anchor,Vector2 position,Vector2 size){
        var r=new GameObject(name,typeof(RectTransform)).GetComponent<RectTransform>();r.SetParent(parent,false);r.anchorMin=r.anchorMax=anchor;r.pivot=new Vector2(.5f,.5f);r.anchoredPosition=position;r.sizeDelta=size;return r;
    }
    static Image Image(string name,Transform parent,string sprite,Vector2 anchor,Vector2 position,Vector2 size){
        var image=Rect(name,parent,anchor,position,size).gameObject.AddComponent<Image>();image.sprite=Sprite(sprite);image.raycastTarget=false;return image;
    }
    static void Place(RectTransform r,Transform parent,Vector2 anchor,Vector2 position,Vector2 size){r.SetParent(parent,false);r.anchorMin=r.anchorMax=anchor;r.pivot=new Vector2(.5f,.5f);r.anchoredPosition3D=position;r.sizeDelta=size;r.localScale=Vector3.one;r.localRotation=Quaternion.identity;}
    [MenuItem("Goose/UI/Apply exported picnic gameplay art")]
    public static void Build(){
        if(EditorApplication.isPlaying)throw new Exception("Stop Play Mode first");
        var stage=PrefabStageUtility.GetCurrentPrefabStage();if(stage!=null&&stage.scene.isDirty)throw new Exception("Save prefab edits first");StageUtility.GoToMainStage();
        foreach(var path in Directory.GetFiles(Art,"*.png")){
            var importer=(TextureImporter)AssetImporter.GetAtPath(path);importer.textureType=TextureImporterType.Sprite;importer.spriteImportMode=SpriteImportMode.Single;importer.spritePixelsPerUnit=100;
            importer.alphaIsTransparency=true;importer.mipmapEnabled=false;importer.npotScale=TextureImporterNPOTScale.None;importer.wrapMode=TextureWrapMode.Clamp;importer.maxTextureSize=2048;importer.textureCompression=TextureImporterCompression.Uncompressed;importer.SaveAndReimport();
        }
        if(!File.Exists(UI+"PvpGameHUD.prefab"))AssetDatabase.CopyAsset(UI+"GameHUD.prefab",UI+"PvpGameHUD.prefab");
        var root=PrefabUtility.LoadPrefabContents(UI+"GameHUD.prefab");
        try{
            if(root.GetComponent<ReplicaGameplayHUD>())throw new Exception("Gameplay skin already applied; edit the prefab directly.");
            var view=root.GetComponent<ReplicaUIView>();var bindings=view.bindings.ToList();
            var header=Rect("Header - safe area",root.transform,new Vector2(0,1),Vector2.zero,new Vector2(720,180));header.pivot=new Vector2(0,1);
            var footer=Rect("Tools - safe area",root.transform,Vector2.zero,Vector2.zero,new Vector2(720,180));footer.pivot=Vector2.zero;
            foreach(string key in new[]{"Title","Remaining","Peer","Freeze"})view.Show(key,false);
            var settings=Image("Settings",header,"image_setting 拷贝",new Vector2(0,1),new Vector2(75,-111),new Vector2(70,70));settings.raycastTarget=true;
            var settingsButton=settings.gameObject.AddComponent<Button>();settingsButton.targetGraphic=settings;
            bindings.Add(new ReplicaUIView.Binding{key="Settings",target=settings.gameObject});
            var pause=view.Get<RectTransform>("Pause");Place(pause,header,new Vector2(0,1),new Vector2(165,-111),new Vector2(70,70));view.Get<Image>("Pause").sprite=Sprite("image_pause");
            var timerBg=Image("Timer frame",header,"img_whiteLine_cube",new Vector2(0,1),new Vector2(360,-120),new Vector2(254,82));
            timerBg.transform.SetAsFirstSibling();
            Image("Clock",header,"img_icon_time",new Vector2(0,1),new Vector2(286.5f,-119.5f),new Vector2(53,55));
            var timer=view.Get<Text>("Timer");Place(timer.rectTransform,header,new Vector2(0,1),new Vector2(390,-120),new Vector2(132,62));timer.fontSize=46;timer.fontStyle=FontStyle.Bold;timer.color=new Color32(0,119,190,255);timer.text="7:35";
            var outline=timer.GetComponent<Outline>()??timer.gameObject.AddComponent<Outline>();outline.effectColor=Color.white;outline.effectDistance=new Vector2(2,-2);
            var skin=root.AddComponent<ReplicaGameplayHUD>();skin.header=header;skin.footer=footer;skin.quantities=new Text[3];
            var keys=new[]{"Undo","Hint","Shuffle"};var labels=new[]{"移出","吸附","洗牌"};var icons=new[]{"image_btn_remove","image_btn_adsorpt","image_btn_shuffle"};
            float[] centerX={157,361,565};
            for(int i=0;i<3;i++){
                var button=view.Get<Button>(keys[i]);var rect=(RectTransform)button.transform;
                Place(rect,footer,Vector2.zero,new Vector2(centerX[i],94.5f),new Vector2(174,135));
                var bg=button.GetComponent<Image>();bg.sprite=Sprite("image_btn_prop_blue 拷贝");bg.color=Color.white;bg.type=UnityEngine.UI.Image.Type.Simple;
                view.Get<Text>(keys[i]+"Text").gameObject.SetActive(false);
                Image("Icon",rect,icons[i],new Vector2(.5f,.5f),new Vector2(0,19),new Vector2(i==0?71:i==1?74:69,i==0?81:i==1?80:71));
                Image("Label artwork",rect,labels[i],new Vector2(.5f,.5f),new Vector2(0,-26),new Vector2(i==2?63:62,35));
                var badge=Image("Remaining uses",rect,"image_number_circle-background",new Vector2(.5f,.5f),new Vector2(65,52.5f),new Vector2(44,44));
                var number=Rect("Count",badge.transform,new Vector2(.5f,.5f),Vector2.zero,new Vector2(44,44)).gameObject.AddComponent<Text>();number.font=timer.font;number.fontSize=31;number.color=Color.white;number.alignment=TextAnchor.MiddleCenter;number.raycastTarget=false;number.text="1";number.verticalOverflow=VerticalWrapMode.Overflow;number.horizontalOverflow=HorizontalWrapMode.Overflow;
                skin.quantities[i==0?0:i==1?1:2]=number;
            }
            view.bindings=bindings.ToArray();PrefabUtility.SaveAsPrefabAsset(root,UI+"GameHUD.prefab");
        }finally{PrefabUtility.UnloadPrefabContents(root);}
        BuildTray();
        var country="Assets/GooseReplica/Resources/Replica/Scenes/CountrysideStove.prefab";root=PrefabUtility.LoadPrefabContents(country);
        try{root.GetComponentInChildren<ReplicaCameraBackdrop>(true).GetComponent<SpriteRenderer>().sprite=Sprite("img_picnic_bg");PrefabUtility.SaveAsPrefabAsset(root,country);}finally{PrefabUtility.UnloadPrefabContents(root);}
        AssetDatabase.SaveAssets();
    }
    static void BuildTray(){
        var root=PrefabUtility.LoadPrefabContents(UI+"CollectionTray.prefab");
        try{
            var view=root.GetComponent<ReplicaUIView>();var rect=(RectTransform)root.transform;rect.sizeDelta=new Vector2(720,159);
            var skin=root.GetComponent<ReplicaTraySkin>()??root.AddComponent<ReplicaTraySkin>();skin.screenCenterFromBottom=237.5f;skin.slots=new RectTransform[7];
            var frame=view.Get<Image>("Frame");frame.sprite=Sprite("img_Blue_box");frame.type=UnityEngine.UI.Image.Type.Simple;frame.rectTransform.sizeDelta=rect.sizeDelta;frame.rectTransform.anchoredPosition=Vector2.zero;
            var footer=Image("Toolbar background",root.transform,"img_Blue_cube",new Vector2(.5f,.5f),new Vector2(0,-147.5f),new Vector2(720,180));footer.transform.SetAsFirstSibling();skin.footerBackground=footer.rectTransform;
            for(int i=0;i<7;i++){
                var slot=view.Get<RectTransform>("Slot"+i);slot.anchoredPosition=new Vector2((i-3)*98.5f,3);slot.sizeDelta=new Vector2(90,92);slot.GetComponent<Image>().enabled=false;skin.slots[i]=slot;
            }
            PrefabUtility.SaveAsPrefabAsset(root,UI+"CollectionTray.prefab");
        }finally{PrefabUtility.UnloadPrefabContents(root);}
    }
}
