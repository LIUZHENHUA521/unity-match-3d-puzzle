using System;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class ReplicaUIPreviewSetup {
    public static readonly Vector2 PhoneSize=new Vector2(720,1560);
    const string Folder="Assets/GooseReplica/Resources/Replica/UI";
    [MenuItem("Goose/UI/Set 720 x 1560 prefab preview")]
    public static void Apply(){
        if(EditorApplication.isPlaying)throw new InvalidOperationException("Stop Play Mode first.");
        var stage=PrefabStageUtility.GetCurrentPrefabStage();
        if(stage!=null && stage.scene.isDirty)throw new InvalidOperationException("Save current prefab edits first.");
        StageUtility.GoToMainStage();
        foreach(var guid in AssetDatabase.FindAssets("t:Prefab",new[]{Folder})){
            string path=AssetDatabase.GUIDToAssetPath(guid);
            var root=PrefabUtility.LoadPrefabContents(path);
            try{
                var rect=(RectTransform)root.transform;
                if(root.name=="UIRoot"){
                    var scaler=root.GetComponent<CanvasScaler>();
                    scaler.referenceResolution=PhoneSize;scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;
                    scaler.screenMatchMode=CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;scaler.matchWidthOrHeight=0;
                    rect.localScale=Vector3.one;
                }else if(root.name!="CollectionTray"){
                    rect.anchorMin=rect.anchorMax=rect.pivot=new Vector2(.5f,.5f);
                    rect.anchoredPosition3D=Vector3.zero;rect.localRotation=Quaternion.identity;
                    rect.localScale=Vector3.one;rect.sizeDelta=PhoneSize;
                    if(root.name=="Home"||root.name=="CollectionTab"||root.name=="PetsTab"){
                        var view=root.GetComponent<ReplicaUIView>();
                        var safe=view.Get<RectTransform>("Safe content");safe.offsetMin=safe.offsetMax=Vector2.zero;
                        var bar=(RectTransform)view.Get<Transform>("Tab Home").parent;
                        var footer=view.Get<RectTransform>("Footer background");footer.anchoredPosition=Vector2.zero;footer.sizeDelta=new Vector2(0,bar.rect.height);
                    }
                    bool fit=new[]{"PauseSettings","Reward","Result","ModelSize","Themes","Collection","PvpLobby"}.Contains(root.name);
                    if(fit && !root.GetComponent<ReplicaUIFitContent>()){
                        var children=root.transform.Cast<Transform>().ToArray();
                        var content=new GameObject("Content - edit UI here",typeof(RectTransform)).GetComponent<RectTransform>();
                        content.SetParent(root.transform,false);content.anchorMin=content.anchorMax=content.pivot=new Vector2(.5f,.5f);content.sizeDelta=PhoneSize;
                        foreach(var child in children){
                            // Full-screen background stays on the stretched root.
                            if(child.name=="Background")continue;
                            var r=(RectTransform)child;var min=r.anchorMin;var max=r.anchorMax;var pos=r.anchoredPosition3D;var size=r.sizeDelta;
                            r.SetParent(content,false);r.anchorMin=min;r.anchorMax=max;r.anchoredPosition3D=pos;r.sizeDelta=size;
                        }
                        root.AddComponent<ReplicaUIFitContent>().content=content;
                    }
                }
                PrefabUtility.SaveAsPrefabAsset(root,path);
            }finally{PrefabUtility.UnloadPrefabContents(root);}
        }
        AssetDatabase.SaveAssets();
    }
}
