using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class Match3DSignInBuilder {
    const string Root="Assets/Match3D/Resources/Match3D/";
    static readonly Color Pink=new Color32(248,91,132,255);
    static RectTransform Rect(string name,Transform parent,float x,float y,float w,float h){
        var rect=new GameObject(name,typeof(RectTransform)).GetComponent<RectTransform>();rect.SetParent(parent,false);
        rect.sizeDelta=new Vector2(w,h);rect.anchoredPosition=new Vector2(x,y);return rect;
    }
    static Sprite Sprite(string asset){var s=AssetDatabase.LoadAssetAtPath<Sprite>(Root+asset+".png");if(!s)throw new InvalidOperationException("Missing sign-in artwork: "+asset);return s;}
    static Image Picture(string name,string asset,Transform parent,float x,float y,float w,float h){
        var image=Rect(name,parent,x,y,w,h).gameObject.AddComponent<Image>();image.sprite=Sprite(asset);image.raycastTarget=false;return image;
    }
    static Image Art(string asset,Transform parent,float left,float top,float w,float h)=>Picture(asset,"SignInSkinPS/"+asset,parent,left+w*.5f-360,700-top-h*.5f,w,h);
    static Text Label(string name,Transform parent,float x,float y,float w,float h,int size,Color color,string value=""){
        var text=Rect(name,parent,x,y,w,h).gameObject.AddComponent<Text>();text.font=AssetDatabase.LoadAssetAtPath<Font>(Root+"Fonts/DefaultChinese.otf");
        text.fontSize=size;text.fontStyle=FontStyle.Bold;text.color=color;text.alignment=TextAnchor.MiddleCenter;text.raycastTarget=false;text.supportRichText=false;text.text=value;return text;
    }
    [MenuItem("Goose Match3D/UI/Build Seven Day Sign In")]
    public static void Build(){
        AssetDatabase.Refresh();
        foreach(string path in Directory.GetFiles(Root+"SignInSkinPS","*.png")){
            var imp=(TextureImporter)AssetImporter.GetAtPath(path);imp.textureType=TextureImporterType.Sprite;imp.spriteImportMode=SpriteImportMode.Single;
            imp.alphaIsTransparency=true;imp.mipmapEnabled=false;imp.npotScale=TextureImporterNPOTScale.None;imp.spritePixelsPerUnit=100;
            imp.maxTextureSize=1024;imp.textureCompression=TextureImporterCompression.CompressedHQ;
            if(path.EndsWith("today_highlight.png"))imp.spriteBorder=new Vector4(24,24,24,24);
            imp.SaveAndReimport();
        }
        var root=Rect("SevenDaySignIn",null,0,0,720,1560);
        try {
            root.gameObject.AddComponent<Image>().color=new Color(0,0,0,.65f);
            var ui=root.gameObject.AddComponent<Match3DUIView>();var view=root.gameObject.AddComponent<Match3DSignInView>();
            var content=Rect("Content",root,0,0,720,1160);root.gameObject.AddComponent<Match3DUIFitContent>().content=content;
            Art("image_SignRabbit",content,54,168,615,199);
            Art("image_Sign_bg",content,40,298,643,851);
            Art("img_fnt_7day",content,167,261,363,174);
            var closeImage=Art("img_exit_x",content,624,290,81,81);closeImage.raycastTarget=true;
            var close=closeImage.gameObject.AddComponent<Button>();close.targetGraphic=closeImage;
            view.cards=new Match3DSignInView.Card[7];string[] days={"第一天","第二天","第三天","第四天","第五天","第六天","第七天 · 豪华礼包"};
            for(int i=0;i<7;i++){
                float width=i==6?554:180,left=i==6?83:83+(i%3)*187,top=i==6?870:452+(i/3)*209;
                var cell=Rect("Day "+(i+1),content,left+width*.5f-360,700-top-101.5f,width,203);
                Picture("Card","SignInSkinPS/"+(i==6?"image_yellowSign_cube":"image_BlueSign_cube"),cell,0,0,width,203);
                var card=new Match3DSignInView.Card();view.cards[i]=card;
                var highlight=Picture("Today highlight","SignInSkinPS/today_highlight",cell,0,0,width+4,207);highlight.type=Image.Type.Sliced;card.highlight=highlight.gameObject;
                Label("Day title",cell,0,77,width-12,38,26,i==6?new Color32(180,62,32,255):Color.white,days[i]);
                card.tag=Label("State",cell,0,46,width-12,23,15,new Color32(111,136,159,255));
                card.caption=Label("Rewards",cell,0,-72,width-8,36,23,Pink);
                card.icons=new Image[4];card.amounts=new Text[4];
                for(int j=0;j<4;j++){
                    card.icons[j]=Picture("Reward icon "+j,"GameplaySkin/image_btn_remove",cell,0,-9,92,92);card.icons[j].preserveAspect=true;
                    card.amounts[j]=Label("Reward amount "+j,cell,0,-72,128,36,22,Pink);
                }
                var mask=Picture("Claimed overlay","SignInSkinPS/"+(i==6?"image_yellowSign_cube":"image_BlueSign_cube"),cell,0,0,width,203);
                mask.color=new Color(.25f,.45f,.65f,.5f);card.claimed=mask.gameObject;
                Picture("Check","SignInSkinPS/img_yes02",mask.transform,width*.5f-45,-45,65,53);
            }
            var buttonImage=Art("image_btn_red02",content,220,1070,286,126);buttonImage.raycastTarget=true;
            view.claim=buttonImage.gameObject.AddComponent<Button>();view.claim.targetGraphic=buttonImage;
            view.buttonLabel=Label("Label",buttonImage.transform,-3,8,248,74,43,Color.white,"立即签到");
            var stroke=view.buttonLabel.gameObject.AddComponent<Match3DTextStroke>();stroke.color=new Color32(177,31,71,255);stroke.radius=3;
            view.buttonLabel.resizeTextForBestFit=true;view.buttonLabel.resizeTextMinSize=28;view.buttonLabel.resizeTextMaxSize=43;
            view.status=Label("Rules and status",content,0,-523,650,52,21,Color.white);
            view.toolIcons=new[]{Sprite("GameplaySkin/image_btn_remove"),Sprite("GameplaySkin/image_btn_adsorpt"),Sprite("GameplaySkin/image_btn_shuffle"),Sprite("GameplaySkin/img_btn_clock")};
            ui.bindings=new[]{new Match3DUIView.Binding{key="Close",target=close.gameObject},new Match3DUIView.Binding{key="Claim",target=view.claim.gameObject}};
            view.Render(Match3DSignInState.Preview(),"立即签到","累计签到，漏签不重置 · 每日零点刷新");
            PrefabUtility.SaveAsPrefabAsset(root.gameObject,Root+"UI/SevenDaySignIn.prefab");AssetDatabase.SaveAssets();
        } finally {UnityEngine.Object.DestroyImmediate(root.gameObject);}
    }
}
