using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class AddTimeToolBuilder {
    const string Art="Assets/Match3D/Resources/Match3D/GameplaySkin/";
    const string Hud="Assets/Match3D/Resources/Match3D/UI/GameHUD.prefab";
    [MenuItem("Goose/UI/Build add-time tool")]
    public static void Build(){
        if(EditorApplication.isPlaying)throw new InvalidOperationException("Stop Play Mode before building the toolbar");
        AssetDatabase.Refresh();
        foreach(var file in new[]{"img_btn_clock","加时"}){
            var importer=(TextureImporter)AssetImporter.GetAtPath(Art+file+".png");
            importer.textureType=TextureImporterType.Sprite;importer.spriteImportMode=SpriteImportMode.Single;
            importer.alphaIsTransparency=true;importer.mipmapEnabled=false;importer.npotScale=TextureImporterNPOTScale.None;
            importer.wrapMode=TextureWrapMode.Clamp;importer.textureCompression=TextureImporterCompression.Uncompressed;importer.SaveAndReimport();
        }
        var root=PrefabUtility.LoadPrefabContents(Hud);
        try{
            var view=root.GetComponent<Match3DUIView>();var hud=root.GetComponent<Match3DGameplayHUD>();
            var bindings=view.bindings.ToList();
            var button=bindings.FirstOrDefault(x=>x.key=="AddTime").target;
            if(!button){
                button=UnityEngine.Object.Instantiate(view.Get<Button>("Hint").gameObject,hud.footer,false);button.name="AddTime";
                button.GetComponent<Button>().onClick.RemoveAllListeners();
                bindings.Add(new Match3DUIView.Binding{key="AddTime",target=button});
                view.bindings=bindings.ToArray();
            }
            var icon=button.transform.Find("Icon").GetComponent<Image>();icon.sprite=AssetDatabase.LoadAssetAtPath<Sprite>(Art+"img_btn_clock.png");
            icon.preserveAspect=true;icon.rectTransform.sizeDelta=new Vector2(87,82);
            var label=button.transform.Find("Label artwork").GetComponent<Image>();label.sprite=AssetDatabase.LoadAssetAtPath<Sprite>(Art+"加时.png");label.preserveAspect=true;label.rectTransform.sizeDelta=label.sprite.rect.size;
            hud.quantities=new[]{view.Get<Button>("Undo"),view.Get<Button>("Hint"),view.Get<Button>("Shuffle"),button.GetComponent<Button>()}
                .Select(x=>x.transform.Find("Remaining uses/Count").GetComponent<Text>()).ToArray();
            var keys=new[]{"Undo","Hint","AddTime","Shuffle"};
            for(int i=0;i<keys.Length;i++){
                var rect=view.Get<RectTransform>(keys[i]);rect.anchoredPosition=new Vector2(105+i*170,94.5f);rect.localScale=Vector3.one*.9f;
                rect.SetSiblingIndex(i);
            }
            PrefabUtility.SaveAsPrefabAsset(root,Hud);
        }finally{PrefabUtility.UnloadPrefabContents(root);}
        AssetDatabase.SaveAssets();
    }
}
