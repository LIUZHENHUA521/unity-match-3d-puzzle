using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class PotCertificateBuilder {
    const string Root="Assets/Match3D/Resources/Match3D/";
    static RectTransform Rect(string name,Transform parent,float x,float y,float w,float h){var r=new GameObject(name,typeof(RectTransform)).GetComponent<RectTransform>();r.SetParent(parent,false);r.sizeDelta=new Vector2(w,h);r.anchoredPosition=new Vector2(x,y);return r;}
    static Sprite Sprite(string name){var s=AssetDatabase.LoadAssetAtPath<Sprite>(Root+"PotCertificate/pot-"+name+".png");if(!s)throw new Exception("Missing certificate sprite "+name);return s;}
    static Image Pic(string name,string asset,Transform parent,float x,float y,float w,float h){var im=Rect(name,parent,x,y,w,h).gameObject.AddComponent<Image>();im.sprite=Sprite(asset);im.raycastTarget=false;return im;}
    static Text Label(string name,Transform parent,float x,float y,float w,float h,int size,string value=""){
        var t=Rect(name,parent,x,y,w,h).gameObject.AddComponent<Text>();t.font=AssetDatabase.LoadAssetAtPath<Font>(Root+"Fonts/DefaultChinese.otf");t.fontSize=size;t.color=new Color32(78,57,39,255);t.alignment=TextAnchor.MiddleCenter;t.raycastTarget=false;t.supportRichText=false;t.resizeTextForBestFit=true;t.resizeTextMinSize=12;t.resizeTextMaxSize=size;t.text=value;return t;
    }
    // Cocos labels use fractional sizes; preserve their effective size after ancestor scales.
    static Text CertificateLabel(string name,Transform parent,float x,float y,float w,float h,float size,Color32 color,bool bold=false,string value="",bool shrink=false){
        int pixels=Mathf.CeilToInt(size);float scale=size/pixels;
        var t=Label(name,parent,x,y,w/scale,h/scale,pixels,value);
        t.rectTransform.localScale=Vector3.one*scale;t.color=color;t.fontStyle=bold?FontStyle.Bold:FontStyle.Normal;
        t.resizeTextForBestFit=shrink;t.resizeTextMinSize=8;t.resizeTextMaxSize=pixels;
        t.horizontalOverflow=HorizontalWrapMode.Overflow;t.verticalOverflow=VerticalWrapMode.Overflow;
        if(shrink)t.horizontalOverflow=HorizontalWrapMode.Wrap;
        return t;
    }
    [MenuItem("Pot/UI/Build Certificate")]
    public static void Build(){
        AssetDatabase.Refresh();
        foreach(var id in AssetDatabase.FindAssets("t:Texture2D",new[]{Root+"PotCertificate"})){
            var imp=(TextureImporter)AssetImporter.GetAtPath(AssetDatabase.GUIDToAssetPath(id));imp.textureType=TextureImporterType.Sprite;imp.spriteImportMode=SpriteImportMode.Single;imp.spritePixelsPerUnit=100;imp.mipmapEnabled=false;imp.alphaIsTransparency=true;imp.npotScale=TextureImporterNPOTScale.None;imp.maxTextureSize=1024;imp.textureCompression=TextureImporterCompression.Uncompressed;imp.SaveAndReimport();
        }
        var root=Rect("PotCertificate",null,0,0,720,1280);var v=root.gameObject.AddComponent<PotCertificateView>();
        v.shade=root.gameObject.AddComponent<Image>();v.shade.color=new Color(0,0,0,.64f);
        root.gameObject.AddComponent<Match3DUIView>().bindings=new Match3DUIView.Binding[0];
        var fit=Rect("Fitted layout",root,0,0,720,1280);root.gameObject.AddComponent<Match3DUIFitContent>().content=fit;
        v.card=Rect("Certificate card",fit,0,80,590,431);
        Pic("Diploma","bg_diploma_02",v.card,0,0,590,431);
        v.banner=Rect("Banner",v.card,0,130,403,105);v.bannerGroup=v.banner.gameObject.AddComponent<CanvasGroup>();
        Pic("Left wing","certificateicon_wings",v.banner,-179,35,73,84);
        Pic("Right wing","certificateicon_wings",v.banner,179,35,73,84).transform.localScale=new Vector3(-1,1,1);
        Pic("Ribbon","certificatetitle_02",v.banner,0,0,403,105);
        var blue=new Color32(18,59,140,255);var purple=new Color32(107,63,160,255);
        var gold=new Color32(229,138,31,255);var brown=new Color32(80,60,40,255);
        var main=Rect("Main info",v.card,0,-4.024f,500,160);v.mainGroup=main.gameObject.AddComponent<CanvasGroup>();
        v.title=CertificateLabel("Title",main,0,54.942f,540,48,37.3f,blue,true);
        // Requested visual correction: the local Cocos prefab has no enabled dynamic outline.
        // Keep a 1.25 layout-pixel gold edge even with the fractional font-size scale.
        var titleOutline=v.title.gameObject.AddComponent<Outline>();
        titleOutline.effectColor=new Color32(229,178,76,255);
        float outlineWidth=1.25f/v.title.rectTransform.localScale.x;
        titleOutline.effectDistance=new Vector2(outlineWidth,-outlineWidth);titleOutline.useGraphicAlpha=true;
        v.mode=CertificateLabel("Mode",main,0,16.893f,400,34,19.9f,purple);
        CertificateLabel("Duration caption",main,-46.612f,-14.83f,42,34,19.1f,blue,false,"用时");
        v.duration=CertificateLabel("Duration",main,23.107f,-14.83f,92,34,21,gold,true);
        v.footer=Rect("Details",v.card,0,-90.063f,480,80);v.footerGroup=v.footer.gameObject.AddComponent<CanvasGroup>();
        var values=new Text[3];string[] captions={"玩家昵称","通关日期","证书编号"};
        for(int i=0;i<3;i++){
            float x=(i-1)*160;
            CertificateLabel("Caption "+i,v.footer,x,28.902f,100,40.32f,16*.8f,purple,false,captions[i]);
            Pic("Left ornament "+i,"certificateicon_07",v.footer,x-48,28.902f,14,19.6f);
            Pic("Right ornament "+i,"certificateicon_07",v.footer,x+48,28.902f,14,19.6f).transform.localScale=new Vector3(-1,1,1);
            // Original content: scale .8 * .55; value label: another 1.9.
            float y=28.902f-36.296f*.8f;
            Pic("Field "+i,"certificateicon_03",v.footer,x,y,249*.8f*.55f,60*.8f*.55f);
            values[i]=CertificateLabel("Value "+i,v.footer,x,y,120*.8f*.55f*1.9f,35.28f*.8f*.55f*1.9f,18.7f*.8f*.55f*1.9f,brown,shrink:true);
        }
        v.nickname=values[0];v.date=values[1];v.number=values[2];
        v.stamp=Rect("Stamp",v.card,200,-15,86,78);v.stampGroup=v.stamp.gameObject.AddComponent<CanvasGroup>();v.stampImage=v.stamp.gameObject.AddComponent<Image>();v.stampImage.raycastTarget=false;
        v.bachelorStamps=new Sprite[6];v.eliteStamps=new Sprite[6];for(int i=0;i<6;i++){v.bachelorStamps[i]=Sprite("yz_"+(i+1).ToString("00"));v.eliteStamps[i]=Sprite("icon_yinzhang_"+(i+1).ToString("00"));}
        v.actions=Rect("Actions",fit,0,-300,600,250);v.actionsGroup=v.actions.gameObject.AddComponent<CanvasGroup>();
        v.status=Label("Status",v.actions,0,65,590,64,22);v.status.color=Color.white;
        var button=Pic("Claim","button",v.actions,0,-15,240,87);button.raycastTarget=true;v.claim=button.gameObject.AddComponent<Button>();v.claim.targetGraphic=button;
        v.buttonLabel=CertificateLabel("Claim label",button.transform,0,0,220,65,28,new Color32(255,255,255,255),true,"领取红包");
        var homeRect=Rect("Home",fit,0,-465,250,60);var graphic=homeRect.gameObject.AddComponent<Image>();graphic.color=Color.clear;v.home=homeRect.gameObject.AddComponent<Button>();v.home.targetGraphic=graphic;Label("Home label",homeRect,0,0,250,60,22,"返回首页").color=new Color(1,1,1,.85f);
        PrefabUtility.SaveAsPrefabAsset(root.gameObject,Root+"UI/PotCertificate.prefab");UnityEngine.Object.DestroyImmediate(root.gameObject);AssetDatabase.SaveAssets();
    }
}
