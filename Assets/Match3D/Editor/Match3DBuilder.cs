using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
public static class Match3DBuilder {
    public const string Scene="Assets/Scenes/Match3D.unity";
    [MenuItem("Goose/Build Match3D")]
    public static void Build(){
        if(EditorApplication.isPlaying)throw new Exception("Stop play before building");
        AssetDatabase.Refresh();
        foreach(string path in Directory.GetFiles("Assets/Match3D/Resources/GooseArt/Sprites","*.png")){
            var imp=AssetImporter.GetAtPath(path) as TextureImporter;if(imp==null)continue;
            if(imp.textureType!=TextureImporterType.Sprite){imp.textureType=TextureImporterType.Sprite;imp.spriteImportMode=SpriteImportMode.Single;imp.alphaIsTransparency=true;imp.maxTextureSize=2048;imp.SaveAndReimport();}
        }
        Directory.CreateDirectory("Assets/Match3D/Resources/Match3D");
        var library=ScriptableObject.CreateInstance<Match3DTheme>();
        var food=new[]{"cheese","ice-cream","sandwich","coockie-man","toast","pancake-big","sandwich-toast","hotdog"}.Select(x=>AssetDatabase.LoadAssetAtPath<GameObject>("Assets/FoodPile/Generated/"+x+".prefab")).ToArray();
        var toys=new[]{"Bear","Bunny","Dinosaur","Doll","Duck","Robot","Sheep","Bee"}.Select(NormalizeToy).ToArray();
        var vehicles=new[]{"Boat","Car","Helicopter","Horse","Minibus","Plane","Rocket","Truck"}.Select(NormalizeToy).ToArray();
        library.themes=new[]{Theme("daily","每日厨房","每天一道挑战，清空整锅食物","Goose_Potato_2_Icon",food),Theme("toys","玩具总动员","找齐玩具，收藏专属大鹅","Goose_Eggplant_2_Icon",toys),Theme("travel","出发去旅行","车船飞机，一起踏上旅途","Goose_GreenPepper_2_Icon",vehicles)};
        string audio="Assets/Match3D/Resources/GooseArt/Audio";
        Func<string,AudioClip> clip=name=>Directory.GetFiles(audio).Where(x=>!x.EndsWith(".meta")&&Path.GetFileNameWithoutExtension(x)==name).Select(x=>AssetDatabase.LoadAssetAtPath<AudioClip>(x)).FirstOrDefault();
        library.homeMusic=AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Match3D/Audio/Licensed/Carefree.mp3");library.gameMusic=library.homeMusic;library.pvpMusic=clip("BGM_PVP");library.clickSound=clip("S_ButtonClick");library.selectSound=AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Match3D/Audio/CrazyCombo/tile_click.mp3");library.matchSound=AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Match3D/Audio/CrazyCombo/merge_sound.mp3");library.winSound=clip("S_Win");library.loseSound=clip("S_Lose");
        var old=AssetDatabase.LoadAssetAtPath<Match3DTheme>("Assets/Match3D/Resources/Match3D/ThemeLibrary.asset");if(old){EditorUtility.CopySerialized(library,old);UnityEngine.Object.DestroyImmediate(library);library=old;}else AssetDatabase.CreateAsset(library,"Assets/Match3D/Resources/Match3D/ThemeLibrary.asset");
        library.sourceLevelCatalog=AssetDatabase.LoadAssetAtPath<LevelLayoutConfig>("Assets/Match3D/Resources/Match3D/SourceLevelCatalog.asset");
        EditorSceneManager.OpenScene("Assets/Scenes/FoodPileChallenge.unity");
        var flow=new GameObject("Match3D Flow").AddComponent<Match3DFlow>();flow.library=library;
        Match3DFlow.HideLegacyPresentation();
        EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene(),Scene);
        var prior=EditorBuildSettings.scenes.Where(x=>x.path!=Scene).ToList();prior.Insert(0,new EditorBuildSettingsScene(Scene,true));EditorBuildSettings.scenes=prior.ToArray();
        AssetDatabase.SaveAssets();Debug.Log("GOOSE_REPLICA_BUILD_OK");
    }
    static ThemeEntry Theme(string id,string title,string subtitle,string sprite,GameObject[] prefabs){
        if(prefabs.Any(x=>!x))throw new Exception("Missing prefabs: "+id);
        return new ThemeEntry{id=id,title=title,subtitle=subtitle,gooseSprite=sprite,prefabs=prefabs,layers=ChallengePlan.DefaultLayers(prefabs.Length)};
    }
    static GameObject NormalizeToy(string name){
        var src=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Resources/CollectableObjects/"+name+".prefab");if(!src)throw new Exception("Missing "+name);
        var obj=UnityEngine.Object.Instantiate(src);obj.name=name;obj.tag="Collectable";
        var renderers=obj.GetComponentsInChildren<Renderer>();var bounds=renderers[0].bounds;foreach(var r in renderers)bounds.Encapsulate(r.bounds);
        float max=Mathf.Max(bounds.size.x,bounds.size.y,bounds.size.z);if(max>0)obj.transform.localScale*=.42f/max;
        var body=obj.GetComponent<Rigidbody>();if(!body)body=obj.AddComponent<Rigidbody>();body.mass=.15f;body.drag=.6f;body.angularDrag=.8f;body.isKinematic=false;body.interpolation=RigidbodyInterpolation.Interpolate;body.collisionDetectionMode=CollisionDetectionMode.ContinuousDynamic;
        var prefab=PrefabUtility.SaveAsPrefabAsset(obj,"Assets/Match3D/Resources/Match3D/"+name+".prefab");UnityEngine.Object.DestroyImmediate(obj);return prefab;
    }
}
[InitializeOnLoad] public static class Match3DEditorBridge {
    static Match3DEditorBridge(){EditorApplication.update+=Poll;}
    static void Poll(){
        if(EditorApplication.isCompiling||EditorApplication.isUpdating)return;
        string request="Temp/goose-command.txt";if(!File.Exists(request))return;
        string cmd=File.ReadAllText(request).Trim();File.Delete(request);
        try {
            if(cmd=="build")Match3DBuilder.Build();
            else if(cmd=="models")OriginalModelImporter.Import();
            else if(cmd=="apple")HamsterAppleImporter.Build();
            else if(cmd=="apple-preview")HamsterAppleImporter.Preview();
            else if(cmd=="play"){EditorSceneManager.OpenScene(Match3DBuilder.Scene);EditorApplication.isPlaying=true;}
            else if(cmd=="stop")EditorApplication.isPlaying=false;
            else if(cmd=="spawncapture")SpawnCapture.Run();
            else if(cmd=="test")Match3DValidation.Run();
            else if(cmd=="skinfinalize")Match3DGameplaySkinValidation.FinalizeAssets();
            else if(cmd=="skincheck")Match3DGameplaySkinValidation.Run();
            else if(cmd=="pauseexitcheck")Match3DGameplaySkinValidation.ValidatePauseExit();
            else if(cmd=="uicheck")Match3DUIPrefabValidation.Run();
            else if(cmd=="layoutcheck")Match3DUILayoutValidation.Run();
            else if(cmd=="gameplayskin")Match3DGameplaySkinBuilder.Build();
            else if(cmd=="picnicbuild")PicnicBackdropBuilder.Build();
            else if(cmd=="picniccapture")PicnicBackdropBuilder.Capture();
            else if(cmd=="kitchenbuild")KitchenSceneBuilder.Build();
            else if(cmd=="kitcheninspect")KitchenSceneBuilder.Inspect();
            else if(cmd=="modeladaptercheck")ModelAdapterValidation.RunMenu();
            else if(cmd=="layercheck")LayeredPileValidation.Run();
            else if(cmd=="selectionpreview")SelectionFeedbackValidation.Preview();
            else if(cmd=="heightphysicscheck")HeightPhysicsValidation.Run();
            else if(cmd=="pileoptimizationcheck")PileOptimizationValidation.Run();
            else if(cmd=="creationcheck")BoardCreationValidation.Run();
            else if(cmd=="firststageimport")BoardLevelPlanImporter.ImportFirstStage();
            else if(cmd=="hiddenstartupcheck")HiddenStartupValidation.Start();
            else if(cmd=="firststagehudcheck")FirstStageHudValidation.Start();
            else if(cmd=="firststagecheck")FirstStageValidation.Start();
            else if(cmd=="creationprepare")BoardPrefabPreparation.Bake();
            else if(cmd=="creationbudgetcheck")CreationBudgetValidation.Start();
            else if(cmd=="traydisplaycandidates")TrayDisplayPresets.CaptureCandidates();
            else if(cmd=="traydisplaydefaults")TrayDisplayPresets.ApplyCountrysideDefaults();
            else if(cmd=="traydisplayopen")TrayDisplayWindow.Open();
            else if(cmd=="traydisplayfries")TrayDisplayPresets.CaptureFries();
            else if(cmd=="traydisplayrefine")TrayDisplayPresets.ApplyCountrysideDefaults(true);
            else if(cmd=="traydisplaycheck")TrayDisplayValidation.Start();
            else if(cmd=="creationframescheck")BoardCreationFramesValidation.Start();
            else if(cmd=="selectioncheck")SelectionFeedbackValidation.Run();
            else if(cmd=="highlightcheck")BoardHighlightValidation.Run();
            else if(cmd=="pvptest")Match3DNetworkValidation.Run();
            else if(cmd=="home")Match3DFlow.Instance.Home();
            else if(cmd=="tutorial")Match3DFlow.Instance.Begin(0);
            else if(cmd=="hard")Match3DFlow.Instance.NextStage();
            else if(cmd=="themes")Match3DFlow.Instance.ShowThemes();
            else if(cmd=="settings")Match3DFlow.Instance.ShowSettings(false);
            else if(cmd=="size")Match3DFlow.Instance.ShowModelSize();
            else if(cmd.StartsWith("capture:"))ScreenCapture.CaptureScreenshot(cmd.Substring(8));
            File.WriteAllText("Temp/goose-command-result.txt","OK "+cmd);
        }catch(Exception e){File.WriteAllText("Temp/goose-command-result.txt","ERROR "+e);Debug.LogException(e);}
    }
}
