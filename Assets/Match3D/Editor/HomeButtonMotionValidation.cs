using System;
using System.Collections;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[InitializeOnLoad]
public static class HomeButtonMotionValidation {
    static HomeButtonMotionValidation(){EditorApplication.update+=Poll;}
    static void Poll(){
        if(!File.Exists("Temp/run-home-button-motion-check")||EditorApplication.isCompiling||EditorApplication.isUpdating)return;
        if(!EditorApplication.isPlayingOrWillChangePlaymode){EditorApplication.isPlaying=true;return;}
        if(!Application.isPlaying)return;
        if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().path!="Assets/Scenes/Match3D.unity"){
            UnityEditor.SceneManagement.EditorSceneManager.LoadSceneInPlayMode("Assets/Scenes/Match3D.unity",new UnityEngine.SceneManagement.LoadSceneParameters(UnityEngine.SceneManagement.LoadSceneMode.Single));return;
        }
        if(!Match3DFlow.Instance||string.IsNullOrEmpty(Match3DFlow.Instance.ScreenName))return;
        File.Delete("Temp/run-home-button-motion-check");
        new GameObject("Home motion validation").AddComponent<BoardValidationDriver>().StartCoroutine(Guard());
    }
    static void Check(bool ok,string reason){if(!ok)throw new Exception(reason);}
    static IEnumerator Guard(){
        float speed=Time.timeScale;var steps=Run();string failure=null;
        while(true){object current=null;bool next=false;try{next=steps.MoveNext();if(next)current=steps.Current;}catch(Exception e){failure=e.ToString();}if(!next||failure!=null)break;yield return current;}
        Time.timeScale=speed;
        File.WriteAllText("Temp/home-button-motion-validation.txt",failure??"PASS: staggered entrance, paused-time playback, authored final transforms, press/release, exit cancellation, early disable cleanup, return-home replay.");
        EditorApplication.isPlaying=false;
    }
    static IEnumerator Run(){
        var flow=Match3DFlow.Instance;flow.Home();Time.timeScale=0;
        var home=UnityEngine.Object.FindObjectOfType<Match3DHomeView>();var ui=home.GetComponent<Match3DUIView>();
        var elite=ui.Get<Button>("Elite");var bachelor=ui.Get<Button>("Bachelor");
        var prefab=Resources.Load<Match3DUIView>("Match3D/UI/Home");
        var expected=prefab.Get<RectTransform>("Elite");var rect=(RectTransform)elite.transform;
        var group=elite.GetComponent<CanvasGroup>();var second=bachelor.GetComponent<CanvasGroup>();
        Check(group&&second&&group.alpha==0&&second.alpha==0,"initially hidden");
        Check(!group.blocksRaycasts,"input gated during entrance");
        yield return new WaitForSecondsRealtime(.14f);
        Check(group.alpha>second.alpha,"elite leads bachelor");
        yield return new WaitForSecondsRealtime(.6f);
        Check(group.alpha==1&&second.alpha==1&&group.blocksRaycasts&&second.blocksRaycasts,"visible and clickable while paused");
        Check(Vector2.Distance(rect.anchoredPosition,expected.anchoredPosition)<.01f&&Vector3.Distance(rect.localScale,expected.localScale)<.001f,"authored transform restored");
        var motion=elite.GetComponent<Match3DHomeButtonMotion>();var pointer=new PointerEventData(EventSystem.current);
        motion.OnPointerDown(pointer);yield return new WaitForSecondsRealtime(.12f);
        Check(Vector3.Distance(rect.localScale,expected.localScale*.96f)<.001f,"press scale");
        motion.OnPointerUp(pointer);yield return new WaitForSecondsRealtime(.12f);
        Check(Vector3.Distance(rect.localScale,expected.localScale)<.001f,"release scale");
        motion.OnPointerDown(pointer);motion.OnPointerExit(pointer);yield return new WaitForSecondsRealtime(.12f);
        Check(Vector3.Distance(rect.localScale,expected.localScale)<.001f,"drag off cancels press");
        flow.Home();yield return null;
        home=UnityEngine.Object.FindObjectOfType<Match3DHomeView>();elite=home.GetComponent<Match3DUIView>().Get<Button>("Elite");
        group=elite.GetComponent<CanvasGroup>();Check(group.alpha<1,"return home replays");
        elite.gameObject.SetActive(false);
        Check(group.alpha==1&&group.blocksRaycasts&&Vector3.Distance(elite.transform.localScale,expected.localScale)<.001f,"early disable restores");
    }
}
