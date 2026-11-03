using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
public static class ReplicaResultBuilder {
    const string Root="Assets/GooseReplica/Resources/Replica/";
    static RectTransform Rect(string name,Transform parent,float x,float y,float w,float h){var r=new GameObject(name,typeof(RectTransform)).GetComponent<RectTransform>();r.SetParent(parent,false);r.sizeDelta=new Vector2(w,h);r.anchoredPosition=new Vector2(x,y);return r;}
    static Image Pic(string name,string asset,Transform parent,float x,float y){var s=AssetDatabase.LoadAssetAtPath<Sprite>(Root+"ResultSkinPS/"+asset+".png");if(!s)throw new Exception("Missing result sprite "+asset);var im=Rect(name,parent,x,y,s.rect.width,s.rect.height).gameObject.AddComponent<Image>();im.sprite=s;im.raycastTarget=false;return im;}
    static Button Button(Image im){im.raycastTarget=true;var b=im.gameObject.AddComponent<Button>();b.targetGraphic=im;return b;}
    static Text Label(string name,Transform parent,float x,float y,float w,float h,int size){var t=Rect(name,parent,x,y,w,h).gameObject.AddComponent<Text>();t.font=AssetDatabase.LoadAssetAtPath<Font>(Root+"Fonts/DefaultChinese.otf");t.fontSize=size;t.fontStyle=FontStyle.Bold;t.color=Color.white;t.alignment=TextAnchor.MiddleCenter;t.raycastTarget=false;t.verticalOverflow=VerticalWrapMode.Overflow;return t;}
    [MenuItem("Goose Replica/UI/Build Result Screens")]
    public static void Build(){
        AssetDatabase.Refresh();
        foreach(var id in AssetDatabase.FindAssets("t:Texture2D",new[]{Root+"ResultSkinPS"})){
            var imp=(TextureImporter)AssetImporter.GetAtPath(AssetDatabase.GUIDToAssetPath(id));imp.textureType=TextureImporterType.Sprite;imp.spriteImportMode=SpriteImportMode.Single;imp.spritePixelsPerUnit=100;imp.mipmapEnabled=false;imp.alphaIsTransparency=true;imp.npotScale=TextureImporterNPOTScale.None;imp.maxTextureSize=1024;imp.textureCompression=TextureImporterCompression.Uncompressed;imp.SaveAndReimport();
        }
        BuildOne(true);BuildOne(false);AssetDatabase.SaveAssets();
    }
    static void BuildOne(bool win){
        string name=win?"ResultSuccess":"ResultFailure";
        var root=Rect(name,null,0,0,720,1280);var shade=root.gameObject.AddComponent<Image>();shade.color=new Color(0,0,0,.64f);
        root.gameObject.AddComponent<ReplicaUIView>().bindings=new ReplicaUIView.Binding[0];
        var fit=Rect("720 x 1280 layout",root,0,0,720,1280);root.gameObject.AddComponent<ReplicaUIFitContent>().content=fit;
        var content=Rect("Animated content",fit,0,0,720,1280);var skin=root.gameObject.AddComponent<ReplicaResultView>();skin.shade=shade;skin.animatedContent=content;skin.contentGroup=content.gameObject.AddComponent<CanvasGroup>();
        Pic("Title",win?"success-title":"failure-title",content,0,385);
        skin.attempts=Label("Daily attempts",content,0,245,660,68,43);skin.attempts.text="今日已挑战1次";
        Pic("Message",win?"success-message":"failure-message",content,0,170);
        Pic("Progress track","progress-track",content,0,-140);
        skin.fill=Pic("Progress fill","progress-fill",content,0,-139);skin.fill.type=Image.Type.Filled;skin.fill.fillMethod=Image.FillMethod.Horizontal;skin.fill.fillOrigin=0;skin.fill.fillAmount=win?1:.3f;
        skin.marker=Rect("Progress marker",content,win?290:-116,-140,1,1);
        // Dash ends before the best label, and moves with the current result.
        for(int i=0;i<3;i++){var dash=Rect("Dash "+i,skin.marker,0,-22-i*14,4,9).gameObject.AddComponent<Image>();dash.raycastTarget=false;dash.color=Color.white;}
        Pic("Rabbit","rabbit",skin.marker,-7,30);
        Pic("Best label","best-label",skin.marker,0,-75);
        skin.best=Label("Best percentage",skin.marker,0,-110,130,44,34);skin.best.text=win?"100%":"30%";var outline=skin.best.gameObject.AddComponent<Outline>();outline.effectColor=new Color32(251,88,126,255);outline.effectDistance=new Vector2(2,-2);
        skin.primary=Button(Pic(win?"Home":"Retry",win?"home":"retry",content,0,win?-510:-380));
        skin.secondary=Button(Pic("Continue","continue",content,0,-510));skin.secondary.gameObject.SetActive(!win);
        skin.share=Button(Pic("Share","share",content,280,-510));
        PrefabUtility.SaveAsPrefabAsset(root.gameObject,Root+"UI/"+name+".prefab");UnityEngine.Object.DestroyImmediate(root.gameObject);
    }
}
