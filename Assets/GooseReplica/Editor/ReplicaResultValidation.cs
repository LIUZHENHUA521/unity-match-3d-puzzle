using System;
using System.Collections;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
[InitializeOnLoad]
public static class ReplicaResultValidation {
    const string Output="work/result-ui";
    static ReplicaResultValidation(){EditorApplication.playModeStateChanged+=Changed;}
    public static void Run(){ReplicaResultBuilder.Build();Directory.CreateDirectory(Output);File.WriteAllText(Output+"/validation.txt","");SessionState.SetBool("result-ui-validation",true);EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);EditorApplication.isPlaying=true;}
    static void Changed(PlayModeStateChange state){if(state==PlayModeStateChange.EnteredPlayMode&&SessionState.GetBool("result-ui-validation",false))new GameObject("Validation").AddComponent<SourceValidationDriver>().StartCoroutine(Check());}
    static void Assert(bool pass,string message){if(!pass)throw new Exception(message);File.AppendAllText(Output+"/validation.txt","PASS "+message+"\n");}
    static IEnumerator Check(){
        var steps=Steps();while(true){object current;try{if(!steps.MoveNext())break;current=steps.Current;}catch(Exception e){File.AppendAllText(Output+"/validation.txt","FAIL "+e);Debug.LogException(e);SessionState.SetBool("result-ui-validation",false);EditorApplication.Exit(1);yield break;}yield return current;}
        SessionState.SetBool("result-ui-validation",false);EditorApplication.Exit(0);
    }
    static IEnumerator Steps(){
        var camera=new GameObject("Camera").AddComponent<Camera>();camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=new Color(.12f,.22f,.25f);camera.orthographic=true;
        var canvas=new GameObject("Canvas",typeof(RectTransform)).AddComponent<Canvas>();canvas.renderMode=RenderMode.ScreenSpaceCamera;canvas.worldCamera=camera;canvas.planeDistance=10;
        var scaler=canvas.gameObject.AddComponent<CanvasScaler>();scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;scaler.referenceResolution=new Vector2(720,1280);scaler.matchWidthOrHeight=0;canvas.gameObject.AddComponent<GraphicRaycaster>();
        foreach(bool win in new[]{false,true}){
            var view=ReplicaUIView.Load(win?"ResultSuccess":"ResultFailure",canvas.transform);var skin=view.GetComponent<ReplicaResultView>();int clicks=0;
            Time.timeScale=0;skin.Configure(7,win?1:.3f,win?1:.62f,()=>clicks++,()=>clicks++,()=>clicks++);
            Assert(!skin.contentGroup.interactable,"input disabled during entrance");yield return new WaitForSecondsRealtime(.9f);
            Assert(skin.contentGroup.interactable&&Mathf.Abs(skin.animatedContent.localScale.x-1)<.001f,"unscaled entrance animation settles");
            Assert(Mathf.Abs(skin.fill.fillAmount-(win?1:.3f))<.001f,"real progress reaches target");Assert(skin.best.text==(win?"100%":"62%"),"best score is independent of current progress");Assert(skin.attempts.text=="今日已挑战7次","dynamic daily attempts");
            skin.primary.onClick.Invoke();if(!win)skin.secondary.onClick.Invoke();skin.share.onClick.Invoke();Assert(clicks==(win?2:3),"buttons invoke callbacks once");
            Capture(camera,canvas,view,win?"success":"failure",720,1280);
            Capture(camera,canvas,view,win?"success-tall":"failure-tall",720,1560);
            UnityEngine.Object.Destroy(view.gameObject);yield return null;
        }
        Time.timeScale=1;
    }
    static void Capture(Camera camera,Canvas canvas,ReplicaUIView view,string name,int w,int h){
        var rt=new RenderTexture(w,h,24);rt.Create();camera.targetTexture=rt;Canvas.ForceUpdateCanvases();view.GetComponent<ReplicaUIFitContent>().Fit();Canvas.ForceUpdateCanvases();Assert(view.GetComponent<ReplicaResultView>().best.cachedTextGenerator.characterCountVisible>0,"percentage has rendered glyphs");camera.Render();var previous=RenderTexture.active;RenderTexture.active=rt;var tex=new Texture2D(w,h,TextureFormat.RGB24,false);tex.ReadPixels(new Rect(0,0,w,h),0,0);tex.Apply();File.WriteAllBytes(Output+"/"+name+".png",tex.EncodeToPNG());RenderTexture.active=previous;camera.targetTexture=null;UnityEngine.Object.Destroy(tex);rt.Release();UnityEngine.Object.Destroy(rt);
    }
}
