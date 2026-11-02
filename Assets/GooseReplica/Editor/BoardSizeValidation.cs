using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public static class BoardSizeValidation {
    const string Key="board-size-validation";
    static double deadline;
    static BoardSizeValidation(){EditorApplication.playModeStateChanged+=Changed;}
    static void Check(bool value,string message){if(!value)throw new Exception(message);}
    public static void Run(){
        try{
            ModelAdapterSizing.MigrateExisting();
            Check(ModelAdapterValidation.Run().StartsWith("PASS"),"adapter validation");
            Check(ModelAdapterSizing.Suggest("Egg",Vector3.one).longest<ModelAdapterSizing.Suggest("Bottle",Vector3.one).longest,"small/large classes");
            Check(ModelAdapterSizing.Suggest("UnknownAsset",Vector3.one).reason.Contains("未识别"),"unknown category labelled");
            var settings=TrayDisplayWindow.EnsureSettings();
            var models=ReplicaSourceLevelPlan.Get("CountrysideStove").prefabs;
            var original=ReplicaSourceLevelPlan.Get("GroceryStore").prefabs.First(p=>p.name=="Banana");
            var adapter=models.First(p=>p.name=="Banana");
            settings.SetBoardMultiplier(adapter,.7f);settings.SetBoardMultiplier(original,1.2f);
            Check(settings.BoardMultiplier(adapter)==.7f&&settings.BoardMultiplier(original)==1.2f,"same-name isolation");
            Check(Mathf.Abs(ReplicaSourceProfile.BaseModelScale(original)/ReplicaSourceProfile.ModelSizeMultiplier-4.8105235f)<.001f,"original Banana source scale isolated");
            var pose=settings.TryGet(adapter,out var saved)?saved.Copy():new ReplicaTrayDisplaySettings.Pose();
            EditorUtility.SetDirty(settings);AssetDatabase.SaveAssets();AssetDatabase.ImportAsset(TrayDisplayWindow.SettingsPath,ImportAssetOptions.ForceUpdate);
            Check(settings.BoardMultiplier(adapter)==.7f&&settings.TryGet(adapter,out var after)&&after.scale==pose.scale,"board and tray persistence");
            settings.RemoveBoardMultiplier(adapter);Check(settings.TryGet(adapter,out after)&&after.scale==pose.scale,"board reset keeps tray");
            settings.SetBoardMultiplier(adapter,.8f);settings.Remove(adapter);Check(settings.BoardMultiplier(adapter)==.8f,"tray reset keeps board");settings.Set(adapter,pose);
            Check(!ReplicaTrayDisplaySettings.ValidBoardMultiplier(float.NaN)&&!ReplicaTrayDisplaySettings.ValidBoardMultiplier(0),"invalid board values rejected");
            settings.RemoveBoardMultiplier(original);
            Reimport(settings);
            foreach(var model in models)settings.SetBoardMultiplier(model,.75f);
            EditorUtility.SetDirty(settings);AssetDatabase.SaveAssets();
            SessionState.SetBool(Key,true);EditorSceneManager.OpenScene("Assets/Scenes/GooseReplica.unity");EditorApplication.isPlaying=true;
        }catch(Exception e){Finish(e);}
    }
    static void Reimport(ReplicaTrayDisplaySettings settings){
        string catalog=File.ReadAllText(ModelAdapterService.CatalogPath);
        const string id="BoardSizeProbe";
        var source=ModelAdapterService.InputAssets().First(p=>Path.GetFileNameWithoutExtension(p)=="Egg");
        GameObject first=null;
        try{
            var result=ModelAdapterService.Generate(source,id,id,false,true);Check(!result.HasErrors,"generate fixture");
            first=AssetDatabase.LoadAssetAtPath<GameObject>(result.generatedPrefabPath);
            settings.SetBoardMultiplier(first,.6f);settings.Set(first,new ReplicaTrayDisplaySettings.Pose{scale=1.3f});
            result=ModelAdapterService.Generate(source,id,id,true,true);Check(!result.HasErrors,"reimport fixture");
            var second=AssetDatabase.LoadAssetAtPath<GameObject>(result.generatedPrefabPath);
            Check(settings.BoardMultiplier(second)==.6f&&settings.TryGet(second,out var pose)&&pose.scale==1.3f,"reimport keeps manual and tray settings");
            settings.RemoveBoardMultiplier(second);settings.Remove(second);
            Check(ReplicaSourceCatalog.TryGet(id,out var item)&&item.trayBaseScale>0&&!string.IsNullOrEmpty(item.sizeTier),"new catalog independent baseline");
        }finally{
            File.WriteAllText(ModelAdapterService.CatalogPath,catalog);AssetDatabase.ImportAsset(ModelAdapterService.CatalogPath);ReplicaSourceProfile.ClearCache();
            AssetDatabase.DeleteAsset(ModelAdapterService.OutputRoot+"/"+id);
        }
    }
    public static void Capture(){
        Directory.CreateDirectory("Builds/BoardSizePreview");
        var models=ReplicaSourceLevelPlan.Get("CountrysideStove").prefabs;
        using(var board=new BoardSizePreview())using(var tray=new TrayDisplayPreview()){
            foreach(string name in new[]{"Egg","Bottle","AppleGreen"}){
                var prefab=models.First(p=>p.name==name);
                foreach(float multiplier in new[]{.6f,1f}){
                    var texture=board.Render(new Rect(0,0,720,340),prefab,multiplier);
                    var rt=RenderTexture.GetTemporary(720,340,0);var previous=RenderTexture.active;
                    try{Graphics.Blit(texture,rt);RenderTexture.active=rt;var image=new Texture2D(720,340,TextureFormat.RGB24,false);image.ReadPixels(new Rect(0,0,720,340),0,0);image.Apply();File.WriteAllBytes("Builds/BoardSizePreview/"+name+"-"+multiplier.ToString("F1",System.Globalization.CultureInfo.InvariantCulture)+".png",image.EncodeToPNG());UnityEngine.Object.DestroyImmediate(image);}
                    finally{RenderTexture.active=previous;RenderTexture.ReleaseTemporary(rt);}
                }
                tray.Select(prefab);ReplicaTrayDisplaySettings.Current.TryGet(prefab,out var pose);var capture=tray.Capture(pose);
                File.WriteAllBytes("Builds/BoardSizePreview/"+name+"-tray.png",capture.EncodeToPNG());UnityEngine.Object.DestroyImmediate(capture);
            }
        }
    }
    static void Changed(PlayModeStateChange state){
        if(state!=PlayModeStateChange.EnteredPlayMode||!SessionState.GetBool(Key,false))return;
        EditorApplication.delayCall+=()=>{
            try{RuntimeChecks();File.Delete("Temp/goose-tray-display-validation.txt");new GameObject("Interrupted store test").AddComponent<SourceValidationDriver>().StartCoroutine(InterruptedStore());deadline=EditorApplication.timeSinceStartup+150;EditorApplication.update+=Poll;}
            catch(Exception e){Finish(e);}
        };
    }
    static void RuntimeChecks(){
        QualitySettings.vSyncCount=0;Application.targetFrameRate=60;
        var flow=ReplicaFlow.Instance;flow.Home();flow.Begin(0);flow.NextStage();SourceCreationTestSupport.FinishPending(flow);
        var settings=ReplicaTrayDisplaySettings.Current;
        var create=typeof(ReplicaFlow).GetMethod("CreateBoardItem",BindingFlags.Instance|BindingFlags.NonPublic);
        int index=0;
        foreach(var prefab in ReplicaSourceLevelPlan.Get("CountrysideStove").prefabs){
            settings.TryGet(prefab,out var pose);if(pose!=null)settings.Remove(prefab); // Also exercise objects without a tray Pose.
            Vector3 tray=Vector3.zero,store=Vector3.zero;
            foreach(float multiplier in new[]{.5f,1.5f}){
                settings.SetBoardMultiplier(prefab,multiplier);
                create.Invoke(flow,new object[]{new ChallengeTile{id=19000+index,type=19000+index++},prefab,new Vector3(0,25,0),Quaternion.identity,true,false});
                var obj=LevelGenerator.Instance.objectsOnScene.Last();
                Check(Vector3.Distance(obj.transform.localScale,prefab.transform.localScale*ReplicaSourceProfile.BaseModelScale(prefab)*multiplier)<.001f,"production root scale "+prefab.name);
                var renderers=obj.GetComponentsInChildren<Renderer>();var bounds=renderers[0].bounds;foreach(var r in renderers)bounds.Encapsulate(r.bounds);
                Physics.SyncTransforms();
                var collider=obj.GetComponentsInChildren<Collider>().First(c=>c.gameObject.layer==ReplicaSourceColliders.BodyLayer);
                Check(Vector3.Distance(collider.bounds.size,bounds.size)<.02f,"body bounds scale with visuals "+prefab.name);
                var pick=ReplicaSourceFlight.PickScale(obj,obj.transform.localScale);
                var stored=ReplicaSourceFlight.StoreScale(obj,obj.transform.localScale);
                if(multiplier==.5f){tray=pick;store=stored;}else{Check(Vector3.Distance(tray,pick)<.0001f,"tray independent "+prefab.name);Check(Vector3.Distance(store,stored)<.0001f,"store independent "+prefab.name);}
                ReplicaSourceFlight.Forget(obj);obj.SetActive(false);
            }
            if(pose!=null)settings.Set(prefab,pose);settings.SetBoardMultiplier(prefab,.75f);
        }
        flow.Home();
    }
    static IEnumerator InterruptedStore(){
        var settings=ReplicaTrayDisplaySettings.Current;
        var prefab=ReplicaSourceLevelPlan.Get("CountrysideStove").prefabs.First(p=>p.name=="AppleGreen");
        settings.TryGet(prefab,out var pose);settings.Remove(prefab);
        GameObject obj=null;Vector3 expected=Vector3.zero;Exception error=null;
        try{
            var flow=ReplicaFlow.Instance;flow.Begin(0);flow.NextStage();SourceCreationTestSupport.FinishPending(flow);
            typeof(ReplicaFlow).GetMethod("CreateBoardItem",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(flow,new object[]{new ChallengeTile{id=29000,type=29000},prefab,new Vector3(0,25,0),Quaternion.identity,true,false});
            obj=LevelGenerator.Instance.objectsOnScene.Last();expected=ReplicaSourceFlight.PickScale(obj,obj.transform.localScale);
            InputController.Instance.AddObjectOnStack(obj,false);
            Check(InputController.Instance.MoveSourceItemsToStore(1),"immediate store accepted");
        }catch(Exception e){error=e;}
        if(error==null){yield return new WaitForSeconds(.6f);try{Check(obj&&Vector3.Distance(obj.transform.localScale,expected)<.001f,"immediate store uses independent baseline without Pose");}catch(Exception e){error=e;}}
        ReplicaFlow.Instance.Home();if(pose!=null)settings.Set(prefab,pose);
        if(error!=null){Finish(error);yield break;}
        TrayDisplayValidation.Start();
    }
    static void Poll(){
        const string path="Temp/goose-tray-display-validation.txt";
        if(File.Exists(path)){var result=File.ReadAllText(path);Finish(result.StartsWith("PASS")?null:new Exception(result));}
        else if(EditorApplication.timeSinceStartup>deadline)Finish(new Exception("Tray regression timed out"));
    }
    static void Finish(Exception error){
        EditorApplication.update-=Poll;SessionState.SetBool(Key,false);
        string result=error==null?"PASS board sizes: migration; classifications; original isolation; settings persistence/reset; actual reimport; all 23 production visual/body scales; independent tray/store with no Pose; existing production pickup, undo, merge, reuse regression.":"FAIL board sizes: "+error;
        Directory.CreateDirectory("Builds");File.WriteAllText("Builds/board-size-validation.txt",result);Debug.Log(result);EditorApplication.Exit(error==null?0:1);
    }
}
