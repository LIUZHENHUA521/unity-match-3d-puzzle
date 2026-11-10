#if DAILY_WEB_PREVIEW
using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Scripting;

// Only included in the standalone authoring player. No account or reward services.
public sealed class DailyWebPreview : MonoBehaviour {
    public static DailyWebPreview Instance {get;private set;}
    [Serializable] sealed class Command {public string action;public bool hasOpening;public int stage;public Match3DDailyLevel level;}
    [Serializable] sealed class Reply {public string type,message;public int stage,count;public Package package;}
    [Serializable] sealed class Package {public string date,tier,candidateId,previewImage;public int seed;public Match3DDailySnapshot opening;}
    [DllImport("__Internal")] static extern void GooseDailyPreviewEvent(string json);
    Match3DDailyLevel candidate;bool busy;
    void Start(){Instance=this;WebGLInput.captureAllKeyboardInput=false;Emit(new Reply{type="ready"});}
    void Emit(Reply reply){GooseDailyPreviewEvent(JsonUtility.ToJson(reply));}
    [Preserve] public void Receive(string json){
        if(busy)return;
        try{
            var command=JsonUtility.FromJson<Command>(json);
            if(command.action=="load"){
                var level=command.level;
                // JsonUtility materializes an empty nested class for a missing JSON field.
                if(level!=null&&!command.hasOpening)level.opening=null;
                if(level==null||string.IsNullOrEmpty(level.candidateId)||level.seed<=0)throw new Exception("候选参数缺失");
                var check=JsonUtility.FromJson<Match3DDailyLevel>(JsonUtility.ToJson(level));check.generatorVersion="daily-v1";
                if(!check.IsValid(23)||!DateTime.TryParseExact(level.date,"yyyy-MM-dd",System.Globalization.CultureInfo.InvariantCulture,System.Globalization.DateTimeStyles.None,out _))throw new Exception("候选参数无效");
                if(level.opening!=null&&!level.opening.IsValid(level.initialCount,23))throw new Exception("布局版本不兼容，请更换候选");
                candidate=level;StartCoroutine(Run(candidate.opening==null?Generate():Play(2)));
            }else if(command.action=="play"&&candidate?.opening!=null){StartCoroutine(Run(Play(command.stage==1?1:2)));}
        }catch(Exception e){Emit(new Reply{type="error",message=e.Message});}
    }
    public void Replay(){if(!busy&&candidate?.opening!=null)StartCoroutine(Run(Play(Match3DFlow.Instance.Stage)));}
    IEnumerator Run(IEnumerator steps){
        busy=true;string failure=null;
        // Flatten nested iterators so timeouts and validation failures always reach the parent.
        var stack=new System.Collections.Generic.Stack<IEnumerator>();stack.Push(steps);
        while(stack.Count>0){
            var current=stack.Peek();bool next=false;object value=null;
            try{next=current.MoveNext();if(next)value=current.Current;}catch(Exception e){failure=e.Message;}
            if(failure!=null)break;
            if(!next){(stack.Pop() as IDisposable)?.Dispose();continue;}
            if(value is IEnumerator nested)stack.Push(nested);else yield return value;
        }
        while(stack.Count>0)(stack.Pop() as IDisposable)?.Dispose();
        busy=false;Match3DFlow.Instance.SetDailyPreviewBusy(false);
        if(failure!=null)Emit(new Reply{type="error",message=failure});
    }
    IEnumerator WaitForBoard(){
        var flow=Match3DFlow.Instance;flow.SetDailyPreviewBusy(true);
        float deadline=Time.realtimeSinceStartup+120;
        while(flow.CreationInProgress){if(Time.realtimeSinceStartup>deadline)throw new Exception("开局生成超时，请重新打开试玩");yield return null;}
        yield return null;
    }
    IEnumerator Generate(){
        var source=JsonUtility.FromJson<Match3DDailyLevel>(JsonUtility.ToJson(candidate));source.generatorVersion="daily-v1";source.opening=null;
        var opening=new Match3DDailySnapshot{stages=new Match3DDailyStage[2]};
        for(int stage=1;stage<=2;stage++){
            Emit(new Reply{type="status",message="正在生成第 "+stage+" 关固定开局…"});
            Match3DFlow.Instance.PreviewDailyOpening(source,stage);yield return WaitForBoard();
            yield return new WaitForSecondsRealtime(5);
            opening.stages[stage-1]=Match3DFlow.Instance.CaptureDailyOpening();
        }
        if(!opening.IsValid(candidate.initialCount,23))throw new Exception("生成的开局不完整，请更换候选");
        candidate.opening=opening;candidate.generatorVersion="shared-v2";
        yield return Play(2);
        Emit(new Reply{type="generated",package=new Package{date=candidate.date,tier=candidate.tier,candidateId=candidate.candidateId,seed=candidate.seed,opening=opening,previewImage=CaptureImage()}});
    }
    IEnumerator Play(int stage){
        Emit(new Reply{type="status",message="正在还原第 "+stage+" 关…"});
        Match3DFlow.Instance.PreviewDailyOpening(candidate,stage);yield return WaitForBoard();yield return null;
        Emit(new Reply{type="playing",stage=stage,count=LevelGenerator.Instance.objectsOnScene.Count,message="第 "+stage+" 关试玩中 · 道具免费"});
    }
    static string CaptureImage(){
        var camera=Camera.main;var rt=new RenderTexture(360,640,24);var old=camera.targetTexture;var active=RenderTexture.active;var tex=new Texture2D(360,640,TextureFormat.RGB24,false);
        try{camera.targetTexture=rt;rt.Create();Match3DFlow.Instance.GetComponent<Match3DThemePresentation>().Fit();Match3DTray.Instance.SendMessage("Layout");camera.Render();RenderTexture.active=rt;tex.ReadPixels(new Rect(0,0,360,640),0,0);tex.Apply();return "data:image/jpeg;base64,"+Convert.ToBase64String(tex.EncodeToJPG(65));}
        finally{camera.targetTexture=old;Match3DFlow.Instance.GetComponent<Match3DThemePresentation>().Fit();Match3DTray.Instance.SendMessage("Layout");RenderTexture.active=active;Destroy(tex);rt.Release();Destroy(rt);}
    }
}
#endif
