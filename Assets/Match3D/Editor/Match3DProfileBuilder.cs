using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
public static class Match3DProfileBuilder {
 const string Root="Assets/Match3D/Resources/Match3D/";
 static RectTransform Rect(string n,Transform p,float x,float y,float w,float h)=>Match3DMailBuilder.Rect(n,p,x,y,w,h);
 static Image Pic(string name,Transform parent,float x,float y,float w,float h,string folder="ProfileSkinPS") {var image=Rect(name,parent,x,y,w,h).gameObject.AddComponent<Image>();image.sprite=AssetDatabase.LoadAssetAtPath<Sprite>(Root+folder+"/"+name+".png");image.raycastTarget=false;return image;}
 static Text Label(string name,Transform parent,float x,float y,float w,float h,string value,int size){var text=Rect(name,parent,x,y,w,h).gameObject.AddComponent<Text>();text.font=AssetDatabase.LoadAssetAtPath<Font>(Root+"Fonts/DefaultChinese.otf");text.text=value;text.fontSize=size;text.fontStyle=FontStyle.Bold;text.color=new Color32(40,112,187,255);text.raycastTarget=false;text.supportRichText=false;text.alignment=TextAnchor.MiddleLeft;text.resizeTextForBestFit=true;text.resizeTextMinSize=18;text.resizeTextMaxSize=size;return text;}
 static Button Button(Image image){image.raycastTarget=true;var button=image.gameObject.AddComponent<Button>();button.targetGraphic=image;return button;}
 static Text Field(Transform parent,string name,float y,string caption,float labelWidth=130){var label=Label(name+" label",parent,-251+labelWidth/2,y,labelWidth,43,caption,32);label.color=new Color32(20,150,218,255);return Label(name,parent,-251+labelWidth+(490-labelWidth)/2,y,490-labelWidth,43,"未提供",32);}
 [MenuItem("Goose Match3D/UI/Build User Profile")]
 public static void Build(){
  AssetDatabase.Refresh();foreach(var id in AssetDatabase.FindAssets("t:Texture2D",new[]{Root+"ProfileSkinPS"})){
   var importer=(TextureImporter)AssetImporter.GetAtPath(AssetDatabase.GUIDToAssetPath(id));importer.textureType=TextureImporterType.Sprite;importer.spriteImportMode=SpriteImportMode.Single;importer.spritePixelsPerUnit=100;importer.mipmapEnabled=false;importer.alphaIsTransparency=true;importer.isReadable=false;var platform=importer.GetDefaultPlatformTextureSettings();platform.maxTextureSize=1024;importer.SetPlatformTextureSettings(platform);importer.textureCompression=TextureImporterCompression.CompressedHQ;importer.SaveAndReimport();
  }
  var root=Rect("UserProfile",null,0,0,720,1280);try{
   root.gameObject.AddComponent<Image>().color=new Color(0,0,0,.6f);var content=Rect("Content - edit UI here",root,0,0,720,1280);root.gameObject.AddComponent<Match3DUIFitContent>().content=content;
   var ui=root.gameObject.AddComponent<Match3DUIView>();var profile=root.gameObject.AddComponent<Match3DUserProfileView>();
   Pic("image_User_tittle",content,-5,343,599,172);Pic("image_User_bg",content,.5f,-68,655,722);Pic("用户信息",content,0,222,223,63);
   var close=Button(Pic("img_exit_x",content,313,258,81,81,"MailSkinPS"));Pic("img_avatarCircleframe",content,1,50,174,174);
   var holder=Rect("Avatar circle mask",content,1,56,150,150);var avatar=Rect("Wechat Avatar",holder,0,0,150,150).gameObject.AddComponent<PotAvatarGraphic>();avatar.raycastTarget=false;profile.avatar=avatar;profile.defaultAvatar=AssetDatabase.LoadAssetAtPath<Texture2D>(Root+"ProfileSkinPS/图层 45.png");avatar.texture=profile.defaultAvatar;
   profile.sync=Button(Pic("img_add_photo",content,61.5f,9.5f,57,57));
   Pic("image_BlueWhite_cube",content,0,-212,570,315);Pic("image_LightBlue_cube",content,.5f,-212,541,287);
   profile.nickname=Field(content,"Nickname",-105,"昵称：",92);profile.userId=Field(content,"User ID",-159,"用户ID：",130);
   profile.copy=Button(Pic("image_btn_copy",content,238.5f,-158.5f,33,33));profile.gender=Field(content,"Gender",-213,"性别：",92);profile.location=Field(content,"Location",-267,"位置：",92);profile.registered=Field(content,"Registered",-321,"注册时间：",162);profile.registered.resizeTextMaxSize=32;
   // Reserve the right column for controls, including with long WeChat values.
   foreach(var field in new[]{profile.nickname,profile.userId,profile.gender,profile.location}){var rect=(RectTransform)field.transform;var width=rect.sizeDelta.x-30;rect.anchoredPosition-=new Vector2(15,0);rect.sizeDelta=new Vector2(width,43);}
   profile.editNickname=Button(Pic("image_btn_edit",content,238.5f,-105.5f,33,33));profile.editGender=Button(Pic("image_btn_edit",content,238.5f,-211.5f,33,33));profile.editLocation=Button(Pic("image_btn_edit",content,238.5f,-265.5f,33,33));
   profile.status=Label("Status",content,0,-391,540,28,"",18);profile.status.alignment=TextAnchor.MiddleCenter;profile.status.color=new Color32(76,139,191,255);profile.status.resizeTextMinSize=16;profile.status.gameObject.SetActive(false);
   ui.bindings=new[]{new Match3DUIView.Binding{key="Close",target=close.gameObject}};PrefabUtility.SaveAsPrefabAsset(root.gameObject,Root+"UI/UserProfile.prefab");AssetDatabase.SaveAssets();
  }finally{Object.DestroyImmediate(root.gameObject);}
 }
}
