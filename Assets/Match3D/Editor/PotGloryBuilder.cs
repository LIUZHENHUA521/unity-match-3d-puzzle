using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class PotGloryBuilder {
    const string Root="Assets/Match3D/Resources/Match3D/";
    static RectTransform Rect(Transform parent,string name,float x,float y,float w,float h){
        var r=new GameObject(name,typeof(RectTransform)).GetComponent<RectTransform>();r.SetParent(parent,false);
        r.anchorMin=r.anchorMax=r.pivot=new Vector2(0,1);r.anchoredPosition=new Vector2(x,-y);r.sizeDelta=new Vector2(w,h);return r;
    }
    static Image Art(Transform parent,string name,string asset,float x,float y,float w,float h){
        var r=Rect(parent,name,x,y,w,h);var i=r.gameObject.AddComponent<Image>();i.sprite=AssetDatabase.LoadAssetAtPath<Sprite>(Root+"PotGlorySkin/"+asset+".png");
        if(!i.sprite)throw new Exception("Missing glory art "+asset);i.raycastTarget=false;return i;
    }
    static Button Button(Image i){i.raycastTarget=true;var b=i.gameObject.AddComponent<Button>();b.targetGraphic=i;b.navigation=new Navigation{mode=Navigation.Mode.None};return b;}
    static Text Text(Transform parent,string name,string value,float x,float y,float w,float h,int size,string color){
        var r=Rect(parent,name,x,y,w,h);var t=r.gameObject.AddComponent<Text>();t.font=AssetDatabase.LoadAssetAtPath<Font>(Root+"Fonts/DefaultChinese.otf");
        t.verticalOverflow=VerticalWrapMode.Overflow;t.horizontalOverflow=HorizontalWrapMode.Overflow;t.fontSize=size;t.fontStyle=FontStyle.Bold;t.alignment=TextAnchor.MiddleLeft;t.text=value;ColorUtility.TryParseHtmlString("#"+color,out var c);t.color=c;t.raycastTarget=false;return t;
    }
    static Button Link(Transform parent,string name,string label,float x,float y,float w){
        var root=Rect(parent,name,x,y,w,56);var hit=root.gameObject.AddComponent<Image>();hit.color=Color.clear;
        var t=Text(root,"Label",label,0,0,w,56,26,"8B6049");t.alignment=TextAnchor.MiddleCenter;return Button(hit);
    }
    [MenuItem("Pot/UI/Build Glory")]
    public static void Build(){
        AssetDatabase.Refresh();
        foreach(var guid in AssetDatabase.FindAssets("t:Texture2D",new[]{Root+"PotGlorySkin"})){
            var imp=(TextureImporter)AssetImporter.GetAtPath(AssetDatabase.GUIDToAssetPath(guid));imp.textureType=TextureImporterType.Sprite;imp.spriteImportMode=SpriteImportMode.Single;
            imp.mipmapEnabled=false;imp.alphaIsTransparency=true;imp.maxTextureSize=2048;imp.textureCompression=TextureImporterCompression.CompressedHQ;imp.SaveAndReimport();
        }
        var root=Rect(null,"PotGlory",0,0,720,1564);root.pivot=new Vector2(.5f,.5f);
        try{
            root.gameObject.AddComponent<Image>().color=new Color(0,0,0,.7f);
            root.gameObject.AddComponent<Match3DUIView>().bindings=new Match3DUIView.Binding[0];
            var design=Rect(root,"Content - PSD coordinates",0,0,720,1564);design.anchorMin=design.anchorMax=design.pivot=new Vector2(.5f,.5f);
            root.gameObject.AddComponent<Match3DUIFitContent>().content=design;
            var view=root.gameObject.AddComponent<PotGloryView>();
            Art(design,"Panel and original title","pot-panel",13,115,694,1363);
            view.count=Text(design,"Glory count","已收获 <color=#F65300><size=46>0</size></color> 次通关荣耀",166,449,390,64,33,"75361B");view.count.alignment=TextAnchor.MiddleCenter;
            view.close=Button(Art(design,"Close","pot-close",627,244,81,81));
            var viewport=Rect(design,"List viewport",44,520,633,850);viewport.gameObject.AddComponent<Image>().color=Color.clear;viewport.gameObject.AddComponent<RectMask2D>();
            var content=Rect(viewport,"Rows",0,0,633,850);view.content=content;
            view.scroll=viewport.gameObject.AddComponent<ScrollRect>();view.scroll.viewport=viewport;view.scroll.content=content;view.scroll.horizontal=false;view.scroll.movementType=ScrollRect.MovementType.Clamped;
            view.gold=Row(design,true);view.blue=Row(design,false);view.gold.gameObject.SetActive(false);view.blue.gameObject.SetActive(false);
            view.more=Link(content,"Load more","加载更多",180,0,270);var mr=(RectTransform)view.more.transform;mr.anchorMin=mr.anchorMax=new Vector2(.5f,1);mr.pivot=new Vector2(.5f,1);view.more.gameObject.SetActive(false);
            view.status=Text(design,"Status","正在加载荣耀…",90,790,540,180,28,"8B6049");view.status.alignment=TextAnchor.MiddleCenter;
            view.retry=Link(design,"Retry","点击重试",240,990,240);view.retry.gameObject.SetActive(false);
            view.recordings=Link(design,"All recordings","全部录像",120,1480,220);view.masters=Link(design,"Masters","高手回放",380,1480,220);
            view.recordings.GetComponentInChildren<Text>().color=view.masters.GetComponentInChildren<Text>().color=new Color32(255,240,206,255);
            PrefabUtility.SaveAsPrefabAsset(root.gameObject,Root+"UI/PotGlory.prefab");
        }finally{UnityEngine.Object.DestroyImmediate(root.gameObject);}
        string path=Root+"UI/Home.prefab";var homeRoot=PrefabUtility.LoadPrefabContents(path);
        try{
            var home=homeRoot.GetComponent<Match3DHomeView>();var old=home.artwork.Find("PotGloryEntry");if(old)UnityEngine.Object.DestroyImmediate(old.gameObject);
            home.glory=Button(Art(home.artwork,"PotGloryEntry","pot-entry",554,547,150,137));
            PrefabUtility.SaveAsPrefabAsset(homeRoot,path);
        }finally{PrefabUtility.UnloadPrefabContents(homeRoot);}
        AssetDatabase.SaveAssets();
    }
    static PotGloryRow Row(Transform parent,bool elite){
        var root=Rect(parent,elite?"Gold row template":"Blue row template",0,0,633,279);
        var row=root.gameObject.AddComponent<PotGloryRow>();
        Art(root,"Card",elite?"pot-gold-card":"pot-blue-card",0,0,633,279);
        row.tier=Text(root,"Tier",elite?"清北局":"本科局",313,23,220,54,38,"833B23");
        row.date=Text(root,"Date","2026.11.16",355,82,220,38,28,"8B6049");
        row.duration=Text(root,"Duration","03:28",470,119,145,52,37,"F65300");
        row.certificate=Button(Art(root,"View certificate","pot-certificate-button",42,182,260,77));
        row.replay=Button(Art(root,"View replay","pot-replay-button",330,181,261,77));
        var cover=Art(row.replay.transform,"Pending cover","pot-blue-button",0,0,261,77);cover.color=new Color(.65f,.65f,.65f,1);
        row.replayState=Text(cover.transform,"Pending text","回放待同步",0,0,261,64,27,"FFFFFF");row.replayState.alignment=TextAnchor.MiddleCenter;
        row.badge=Art(root,"New badge","pot-new",469,3,149,74).gameObject;
        return row;
    }
}
