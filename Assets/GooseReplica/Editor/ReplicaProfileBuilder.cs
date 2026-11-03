using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
public static class ReplicaProfileBuilder {
 const string Root="Assets/GooseReplica/Resources/Replica/";
 static RectTransform Rect(string n,Transform p,float x,float y,float w,float h)=>ReplicaMailBuilder.Rect(n,p,x,y,w,h);
 static Image Pic(string name,Transform parent,float x,float y,float w,float h,string folder="ProfileSkinPS") {var image=Rect(name,parent,x,y,w,h).gameObject.AddComponent<Image>();image.sprite=AssetDatabase.LoadAssetAtPath<Sprite>(Root+folder+"/"+name+".png");image.raycastTarget=false;return image;}
 static Text Label(string name,Transform parent,float x,float y,float w,float h,string value,int size){var text=Rect(name,parent,x,y,w,h).gameObject.AddComponent<Text>();text.font=AssetDatabase.LoadAssetAtPath<Font>(Root+"Fonts/DefaultChinese.otf");text.text=value;text.fontSize=size;text.fontStyle=FontStyle.Bold;text.color=new Color32(40,112,187,255);text.raycastTarget=false;text.supportRichText=false;text.alignment=TextAnchor.MiddleLeft;text.resizeTextForBestFit=true;text.resizeTextMinSize=18;text.resizeTextMaxSize=size;return text;}
 static Button Button(Image image){image.raycastTarget=true;var button=image.gameObject.AddComponent<Button>();button.targetGraphic=image;return button;}
 static Text Field(Transform parent,string name,float y,string caption,float labelWidth=130){var label=Label(name+" label",parent,-238+labelWidth/2,y,labelWidth,43,caption,30);label.color=new Color32(20,150,218,255);return Label(name,parent,-238+labelWidth+(490-labelWidth)/2,y,490-labelWidth,43,"未提供",29);}
 [MenuItem("Goose Replica/UI/Build User Profile")]
 public static void Build(){
  AssetDatabase.Refresh();foreach(var id in AssetDatabase.FindAssets("t:Texture2D",new[]{Root+"ProfileSkinPS"})){
   var importer=(TextureImporter)AssetImporter.GetAtPath(AssetDatabase.GUIDToAssetPath(id));importer.textureType=TextureImporterType.Sprite;importer.spriteImportMode=SpriteImportMode.Single;importer.spritePixelsPerUnit=100;importer.mipmapEnabled=false;importer.alphaIsTransparency=true;importer.isReadable=false;var platform=importer.GetDefaultPlatformTextureSettings();platform.maxTextureSize=1024;importer.SetPlatformTextureSettings(platform);importer.textureCompression=TextureImporterCompression.CompressedHQ;importer.SaveAndReimport();
  }
  var root=Rect("UserProfile",null,0,0,720,1280);try{
   root.gameObject.AddComponent<Image>().color=new Color(0,0,0,.6f);var content=Rect("Content - edit UI here",root,0,0,720,1280);root.gameObject.AddComponent<ReplicaUIFitContent>().content=content;
   var ui=root.gameObject.AddComponent<ReplicaUIView>();var profile=root.gameObject.AddComponent<ReplicaUserProfileView>();
   Pic("image_User_tittle",content,-5,343,599,172);Pic("image_User_bg",content,.5f,-68,655,722);Pic("用户信息",content,0,222,223,63);
   var close=Button(Pic("img_exit_x",content,313,258,81,81,"MailSkinPS"));Pic("img_avatarCircleframe",content,1,50,174,174);
   var mask=Rect("Avatar circle mask",content,1,56,150,150).gameObject.AddComponent<ReplicaCircleGraphic>();mask.raycastTarget=false;mask.gameObject.AddComponent<Mask>().showMaskGraphic=false;var avatar=Rect("Wechat Avatar",mask.transform,0,0,150,150).gameObject.AddComponent<RawImage>();avatar.raycastTarget=false;profile.avatar=avatar;profile.defaultAvatar=AssetDatabase.LoadAssetAtPath<Texture2D>(Root+"ProfileSkinPS/图层 45.png");avatar.texture=profile.defaultAvatar;
   var sync=Button(Pic("组 12",content,0,-43,220,55,"MailSkinPS"));profile.sync=sync;profile.syncLabel=Label("Sync label",sync.transform,0,0,210,50,"同步微信资料",24);profile.syncLabel.color=Color.white;profile.syncLabel.alignment=TextAnchor.MiddleCenter;
   Pic("image_BlueWhite_cube",content,0,-252,570,315);Pic("image_LightBlue_cube",content,.5f,-252,541,287);
   profile.nickname=Field(content,"Nickname",-137,"昵称：",92);profile.userId=Field(content,"User ID",-191,"用户ID：",130);((RectTransform)profile.userId.transform).sizeDelta=new Vector2(310,43);((RectTransform)profile.userId.transform).anchoredPosition=new Vector2(47,-191);
   profile.copy=Button(Pic("image_btn_copy",content,238,-191,33,33));profile.gender=Field(content,"Gender",-245,"性别：",92);profile.location=Field(content,"Location",-299,"位置：",92);profile.registered=Field(content,"Registered",-353,"注册时间：",154);profile.registered.resizeTextMaxSize=26;
   profile.status=Label("Status",content,0,-391,490,28,"微信授权需在小游戏中进行",18);profile.status.alignment=TextAnchor.MiddleCenter;profile.status.color=new Color32(76,139,191,255);profile.status.resizeTextMinSize=16;
   ui.bindings=new[]{new ReplicaUIView.Binding{key="Close",target=close.gameObject}};PrefabUtility.SaveAsPrefabAsset(root.gameObject,Root+"UI/UserProfile.prefab");AssetDatabase.SaveAssets();
  }finally{Object.DestroyImmediate(root.gameObject);}
 }
}
