using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
public static class Match3DRewardsBuilder {
 [MenuItem("Goose Match3D/UI/Build Red Packets")]
 public static void Build(){
  const string root="Assets/Match3D/Resources/Match3D/";
  var go=PrefabUtility.LoadPrefabContents(root+"UI/Mailbox.prefab");
  try{
   go.name="RedPackets";var old=go.GetComponent<Match3DMailboxView>();var view=go.AddComponent<Match3DRewardsView>();
   view.list=old.list;view.rowTemplate=old.rowTemplate;view.status=old.status;view.refresh=old.refresh;view.more=old.more;
   view.refresh.GetComponentInChildren<Text>().text="刷新红包";
   var title=go.transform.Find("Content - edit UI here/Title");Object.DestroyImmediate(title.GetComponent<Image>());
   var text=title.gameObject.AddComponent<Text>();text.font=AssetDatabase.LoadAssetAtPath<Font>(root+"Fonts/DefaultChinese.otf");text.fontSize=40;text.fontStyle=FontStyle.Bold;text.text="我的红包";text.alignment=TextAnchor.MiddleCenter;text.color=new Color32(32,118,185,255);text.raycastTarget=false;((RectTransform)title).sizeDelta=new Vector2(300,62);
   Object.DestroyImmediate(old);PrefabUtility.SaveAsPrefabAsset(go,root+"UI/RedPackets.prefab");
  }finally{PrefabUtility.UnloadPrefabContents(go);}
  AssetDatabase.SaveAssets();
 }
}
