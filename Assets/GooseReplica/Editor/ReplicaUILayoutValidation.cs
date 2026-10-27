using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class ReplicaUILayoutValidation {
    public static void Run(){
        if(!Application.isPlaying)throw new Exception("Start game first");
        var canvas=GameObject.Find("Goose UI").GetComponent<Canvas>();
        var page=canvas.GetComponent<ReplicaUIView>().Get<RectTransform>("Page");
        foreach(Transform child in page)child.gameObject.SetActive(false);
        var camera=Camera.main;var oldTarget=camera.targetTexture;var oldActive=RenderTexture.active;
        float oldAspect=camera.aspect;var oldMode=canvas.renderMode;var oldCamera=canvas.worldCamera;float oldPlane=canvas.planeDistance;
        string report="";
        try{
            foreach(var dimensions in new[]{new Vector2Int(720,1560),new Vector2Int(720,1280),new Vector2Int(768,1024)}){
                var target=new RenderTexture(dimensions.x,dimensions.y,24);target.Create();camera.targetTexture=target;camera.aspect=(float)dimensions.x/dimensions.y;
                canvas.renderMode=RenderMode.ScreenSpaceCamera;canvas.worldCamera=camera;canvas.planeDistance=1;
                try{
                    foreach(string name in new[]{"Home","GameHUD","PauseSettings","Result","Themes","Collection","ModelSize","PvpLobby"}){
                        var prefab=Resources.Load<ReplicaUIView>("Replica/UI/"+name);var root=(RectTransform)prefab.transform;
                        if(root.sizeDelta!=new Vector2(720,1560)||root.anchorMin!=root.anchorMax)throw new Exception("Invalid authoring canvas "+name);
                        var view=ReplicaUIView.Load(name,page);
                        try{
                            Canvas.ForceUpdateCanvases();camera.Render();var fit=view.GetComponent<ReplicaUIFitContent>();if(fit)fit.Fit();Canvas.ForceUpdateCanvases();
                            foreach(var button in view.GetComponentsInChildren<Button>()){
                                var corners=new Vector3[4];((RectTransform)button.transform).GetWorldCorners(corners);
                                foreach(var corner in corners){var p=camera.WorldToViewportPoint(corner);if(p.x<-.002f||p.x>1.002f||p.y<-.002f||p.y>1.002f)throw new Exception(name+" "+button.name+" outside "+dimensions+": "+p);}
                            }
                            report+="PASS "+name+" button bounds "+dimensions+"\n";
                            if(name=="PauseSettings"){
                                camera.Render();RenderTexture.active=target;var image=new Texture2D(dimensions.x,dimensions.y,TextureFormat.RGB24,false);
                                image.ReadPixels(new Rect(0,0,dimensions.x,dimensions.y),0,0);image.Apply();
                                File.WriteAllBytes("Temp/ui-migration/layout-"+dimensions.x+"x"+dimensions.y+".png",image.EncodeToPNG());UnityEngine.Object.DestroyImmediate(image);
                            }
                        }finally{UnityEngine.Object.DestroyImmediate(view.gameObject);}
                    }
                }finally{camera.targetTexture=oldTarget;RenderTexture.active=oldActive;UnityEngine.Object.DestroyImmediate(target);}
            }
            report+="ALL LAYOUT CHECKS PASSED\n";
        }catch(Exception e){report+="FAIL "+e+"\n";throw;}
        finally{
            camera.targetTexture=oldTarget;RenderTexture.active=oldActive;canvas.renderMode=oldMode;canvas.worldCamera=oldCamera;canvas.planeDistance=oldPlane;
            camera.aspect=oldAspect;ReplicaFlow.Instance.Home();File.WriteAllText("Temp/ui-migration/layout-validation.txt",report);
        }
    }
}
