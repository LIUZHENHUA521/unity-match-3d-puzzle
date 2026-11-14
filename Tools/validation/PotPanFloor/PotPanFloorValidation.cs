using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class PotPanFloorValidation {
    static readonly List<string> lines=new List<string>();
    static void Check(bool ok,string message){if(!ok)throw new Exception(message);lines.Add("PASS "+message);}
    static GameObject Item(string name,float scale){
        var prefab=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Match3D/Generated/ModelAdapter/"+name+"/"+name+".prefab");
        var obj=UnityEngine.Object.Instantiate(prefab);obj.transform.localScale*=BoardLayoutSettings.ModelScale(prefab)/BoardLayoutSettings.ModelSizeMultiplier*scale;
        obj.transform.position=SoftTransitionContainer.Origin;obj.transform.rotation=Quaternion.identity;return obj;
    }
    static Vector3[] Vertices(GameObject obj)=>obj.GetComponentsInChildren<MeshFilter>().SelectMany(m=>m.sharedMesh.vertices.Select(v=>obj.transform.InverseTransformPoint(m.transform.TransformPoint(v)))).ToArray();
    public static void Run(){
        bool pass=false;
        try{
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
            Physics.autoSimulation=false;BoardPhysicsSettings.Begin();ItemColliderGeometry.ConfigureLayers();BoardEnvironment.Select("CountrysideStove");
            var pan=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Match3D/Scenes/CountrysideStove"));Match3DThemePresentation.AddPanCollision(pan);
            SoftTransitionContainer.Create();
            Check(PotPanFloor.TryHeight(SoftTransitionContainer.Origin,out float floor)&&floor>.45f,"actual pan floor="+floor);
            // Reproduce the old fitting rule on the model from the reported theme.
            var binding=pan.GetComponentInChildren<PotPanFloor>();binding.enabled=false;
            var baseline=Item("Kitchen_pizza",.5f);var baselineBody=baseline.GetComponent<Rigidbody>();var baselineVertices=Vertices(baseline);
            baseline.transform.position+=Vector3.up*(.04f-SoftTransitionContainer.SolidPoints(baseline).Min(p=>p.y));
            SoftTransitionContainer.Fit(baseline);baselineBody.isKinematic=false;BoardPhysicsSettings.Apply(baselineBody);Physics.SyncTransforms();
            for(int step=0;step<600;step++)Physics.Simulate(.02f);
            float baselineTop=baselineVertices.Max(v=>baseline.transform.TransformPoint(v).y);
            Check(baselineTop<floor,"baseline Kitchen_pizza remains completely under pan after 12s; top="+baselineTop);
            UnityEngine.Object.DestroyImmediate(baseline);binding.enabled=true;PotPanFloor.Bind(binding.transform);
            foreach(string name in new[]{"Party_Pizza","Party_Waffle","Party_PancakesStack","Kitchen_pizza"})foreach(float scale in new[]{.5f,.9f,1f,1.25f}){
                var obj=Item(name,scale);var rb=obj.GetComponent<Rigidbody>();rb.isKinematic=true;var vertices=Vertices(obj);
                float bottom=SoftTransitionContainer.SolidPoints(obj).Min(p=>p.y);
                obj.transform.position+=Vector3.up*(.04f-bottom);
                SoftTransitionContainer.Fit(obj);
                Check(SoftTransitionContainer.SolidPoints(obj).Min(p=>p.y)>=floor+.039f,name+" "+scale+" fit above visible floor");
                var before=obj.transform.position;Check(!PotPanFloor.Recover(rb)&&obj.transform.position==before,"valid placement unchanged");
                // Force the whole collider below the pan, including a sleeping/kinematic body.
                obj.transform.position+=Vector3.up*(floor-.05f-SoftTransitionContainer.SolidPoints(obj).Max(p=>p.y));
                Check(PotPanFloor.Recover(rb),name+" "+scale+" stranded body rescued");
                Check(SoftTransitionContainer.SolidPoints(obj).Min(p=>p.y)>=floor+.039f,"rescued full geometry clears pan");
                rb.isKinematic=false;BoardPhysicsSettings.Apply(rb);Physics.SyncTransforms();
                for(int step=0;step<600;step++){Physics.Simulate(.02f);if(step%50==49)PotPanFloor.Recover(rb);}
                float top=vertices.Max(v=>obj.transform.TransformPoint(v).y);
                Check(top>floor+.01f&&PotPanFloor.TryHeight(obj.transform.position,out _),name+" "+scale+" remains visible after 12s top="+top);
                UnityEngine.Object.DestroyImmediate(obj);
            }
            // Live visibility refresh must rescue the last hidden item without a booster.
            UnityEngine.Object.DestroyImmediate(pan);
            var owner=new GameObject("Pot floor integration");owner.AddComponent<InputController>();var physics=owner.AddComponent<ItemPhysicsController>();var pile=owner.AddComponent<LayeredItemPile>();
            physics.SendMessage("Awake");pile.SendMessage("Awake");
            pile.Begin(false,limitHeight:1);
            var last=Item("Kitchen_pizza",1f);var body=last.GetComponent<Rigidbody>();body.isKinematic=true;
            last.transform.position=new Vector3(.05f,.03f,-.04f);body.position=last.transform.position;
            physics.Register(body,true);pile.Register(body);physics.HoldDailyOpening();
            var held=last.transform.position;pile.RefreshVisibility();Check(last.transform.position==held,"held daily snapshot remains exact");
            physics.ReleaseDailyOpening();pile.RefreshVisibility();
            Check(last.transform.position.y>floor&&pile.CanPick(body)&&last.GetComponentsInChildren<Renderer>().Any(r=>r.enabled),"tail refresh rescues and reveals last pizza");
            pile.Detach(body);physics.RemoveForPick(body);last.transform.position=held;body.position=held;pile.RefreshVisibility();
            Check(last.transform.position==held,"picked/tray item is excluded");
            UnityEngine.Object.DestroyImmediate(last);pile.Clear();UnityEngine.Object.DestroyImmediate(owner);
            Check(!PotPanFloor.TryHeight(Vector3.zero,out _),"presentation cleanup clears floor binding");
            pass=true;
        }catch(Exception e){lines.Add("FAIL "+e);}
        Directory.CreateDirectory("../results");File.WriteAllLines("../results/pot-pan-floor.txt",lines);EditorApplication.Exit(pass?0:1);
    }
    public static void AuditSnapshot(){
        var level=JsonUtility.FromJson<Match3DDailyLevel>(File.ReadAllText("/tmp/pot-under-pan-level.json"));
        var profile=Match3DModelThemes.Get(level.modelThemeId,"CountrysideStove");
        var pan=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Match3D/Scenes/CountrysideStove"));Match3DThemePresentation.AddPanCollision(pan);
        var report=new List<string>();int hidden=0;int count=0;
        foreach(var stage in level.opening.stages)foreach(var item in stage.items){
            var obj=UnityEngine.Object.Instantiate(profile.prefabs[item.type]);obj.transform.SetPositionAndRotation(item.position,item.rotation);obj.transform.localScale=item.scale;
            if(PotPanFloor.TryHeight(item.position,out float floor)){
                var verts=Vertices(obj);float top=verts.Max(v=>obj.transform.TransformPoint(v).y),bottom=verts.Min(v=>obj.transform.TransformPoint(v).y);
                count++;if(top<floor)hidden++;
                if(top<floor||obj.name.ToLowerInvariant().Contains("pizza"))report.Add("stage="+stage.stage+" id="+item.id+" model="+obj.name+" p="+item.position+" top="+top+" bottom="+bottom+" floor="+floor+" hidden="+(top<floor));
            }
            UnityEngine.Object.DestroyImmediate(obj);
        }
        report.Add("items over pan="+count+"; fully below pan="+hidden);
        File.WriteAllLines("../results/pot-snapshot-audit.txt",report);EditorApplication.Exit(0);
    }
}
