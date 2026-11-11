using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
public static class Match3DInviteBuilder {
 const string Root="Assets/Match3D/Resources/Match3D/";
 [MenuItem("Goose Match3D/UI/Build Invitation Popup")]
 public static void Build(){
  foreach(var id in AssetDatabase.FindAssets("t:Texture2D",new[]{Root+"InviteSkinPS"})){
   var path=AssetDatabase.GUIDToAssetPath(id);var imp=(TextureImporter)AssetImporter.GetAtPath(path);
   imp.textureType=TextureImporterType.Sprite;imp.spriteImportMode=SpriteImportMode.Single;imp.spritePixelsPerUnit=100;
   imp.mipmapEnabled=false;imp.alphaIsTransparency=true;imp.isReadable=false;imp.maxTextureSize=1024;imp.textureCompression=TextureImporterCompression.CompressedHQ;imp.SaveAndReimport();
  }
  var root=Rect("InviteRewards",null,Vector2.zero,new Vector2(720,1560));
  try{
   var shade=root.gameObject.AddComponent<Image>();shade.color=new Color(0,0,0,.6f);
   var content=Rect("Content",root,Vector2.zero,new Vector2(720,1280));root.gameObject.AddComponent<Match3DUIFitContent>().content=content;
   var view=root.gameObject.AddComponent<Match3DUIView>();var skin=root.gameObject.AddComponent<Match3DInviteView>();
   // Source modal is translated as a unit into a 720 x 1280 editable design area.
   Picture("弹窗背景",content,new Rect(65,229,592,878));
   Picture("邀请得道具",content,new Rect(218,341,290,67));
   BuildRewards(content);
   Picture("奖励说明",content,new Rect(153,713,418,88));
   var close=Picture("img_exit_x",content,new Rect(581,317,81,81)).gameObject.AddComponent<Button>();
   var invite=Picture("邀请好友",content,new Rect(217,917,291,132)).gameObject.AddComponent<Button>();
   close.GetComponent<Image>().raycastTarget=true;invite.GetComponent<Image>().raycastTarget=true;close.targetGraphic=close.GetComponent<Image>();invite.targetGraphic=invite.GetComponent<Image>();
   skin.invite=invite;skin.count=Label("InviteCount",content,new Vector2(4,-216),new Vector2(500,55),"当前邀请：0个好友",30);
   skin.status=Label("Status",content,new Vector2(0,-385),new Vector2(520,48),"",20);
   view.bindings=new[]{new Match3DUIView.Binding{key="Close",target=close.gameObject},new Match3DUIView.Binding{key="Invite",target=invite.gameObject}};
   PrefabUtility.SaveAsPrefabAsset(root.gameObject,Root+"UI/InviteRewards.prefab");AssetDatabase.SaveAssets();
  }finally{Object.DestroyImmediate(root.gameObject);}
 }
 static void BuildRewards(Transform content){
  // Display order matches the toolbar; protocol inventory order remains 0,1,3,2.
  string[] names={"移出","吸附","加时","洗牌"};
  string[] icons={"image_btn_remove","image_btn_adsorpt","img_btn_clock","image_btn_shuffle"};
  var row=Rect("道具奖励",content,new Vector2(0,110),new Vector2(520,180));
  for(int i=0;i<names.Length;i++){
   var cell=Rect(names[i],row,new Vector2(-195+i*130,0),new Vector2(120,180));
   var circle=RewardPicture("背景","ToolTextSkinPS/icon-circle",cell,new Vector2(0,10),new Vector2(118,118));
   circle.color=new Color(1,1,1,.28f);
   RewardPicture("图标","GameplaySkin/"+icons[i],cell,new Vector2(0,14),new Vector2(96,96));
   RewardPicture("名称","GameplaySkin/"+names[i],cell,new Vector2(0,-66),new Vector2(80,42));
  }
 }
 static Image RewardPicture(string name,string asset,Transform parent,Vector2 pos,Vector2 size){
  var sprite=AssetDatabase.LoadAssetAtPath<Sprite>(Root+asset+".png");
  if(!sprite)throw new System.InvalidOperationException("Missing invitation reward artwork: "+asset);
  var image=Rect(name,parent,pos,size).gameObject.AddComponent<Image>();
  image.sprite=sprite;image.preserveAspect=true;image.raycastTarget=false;return image;
 }
 static RectTransform Rect(string name,Transform parent,Vector2 pos,Vector2 size){var r=new GameObject(name,typeof(RectTransform)).GetComponent<RectTransform>();r.SetParent(parent,false);r.sizeDelta=size;r.anchoredPosition=pos;return r;}
 static Image Picture(string name,Transform parent,Rect bounds){var r=Rect(name,parent,new Vector2(bounds.center.x-360,670-bounds.center.y),bounds.size);var im=r.gameObject.AddComponent<Image>();im.sprite=AssetDatabase.LoadAssetAtPath<Sprite>(Root+"InviteSkinPS/"+name+".png");im.raycastTarget=false;return im;}
 static Text Label(string name,Transform parent,Vector2 pos,Vector2 size,string text,int fontSize){var r=Rect(name,parent,pos,size);var t=r.gameObject.AddComponent<Text>();t.font=AssetDatabase.LoadAssetAtPath<Font>(Root+"Fonts/DefaultChinese.otf");t.text=text;t.fontSize=fontSize;t.fontStyle=FontStyle.Bold;t.color=new Color32(31,121,195,255);t.alignment=TextAnchor.MiddleCenter;t.raycastTarget=false;return t;}
}
