using System.IO;
using UnityEditor;
using UnityEngine;

public static class TrayDisplayPresets {
    public static void ApplyCountrysideDefaults(bool overwrite=false){
        var settings=TrayDisplayWindow.EnsureSettings();
        foreach(var prefab in LevelLayoutConfig.Get("CountrysideStove").prefabs){
            if(!overwrite&&settings.TryGet(prefab,out _))continue;
            var p=new Match3DTrayDisplaySettings.Pose();
            switch(prefab.name){
                case "AppleGreen": p.rotation=new Vector3(-25,0,0);break;
                case "Avocado": p.rotation=new Vector3(-15,180,-10);p.scale=1.1f;break;
                case "Banana": p.rotation=new Vector3(-15,180,-25);p.scale=.52f;break;
                case "Bottle": p.rotation=new Vector3(-10,180,12);p.scale=1.1f;break;
                case "Bread": p.rotation=new Vector3(-35,140,0);p.scale=.9f;break;
                case "Broccoli": p.rotation=new Vector3(-20,0,-10);p.scale=.95f;break;
                case "Burger": p.rotation=new Vector3(-20,0,0);break;
                case "ChickenLeg": p.rotation=new Vector3(-65,0,-25);p.scale=.65f;break;
                case "ChocolateBar": p.rotation=new Vector3(-75,0,-20);p.scale=.9f;break;
                case "Corndog": p.rotation=new Vector3(-15,0,-15);p.scale=1.1f;break;
                case "Croissant": p.rotation=new Vector3(-70,0,-10);p.scale=.9f;break;
                case "Cupcake": p.rotation=new Vector3(-25,0,0);break;
                case "Donut": p.rotation=new Vector3(-60,0,0);break;
                case "Egg": p.rotation=new Vector3(-20,0,-10);break;
                case "Eggplant": p.rotation=new Vector3(-20,0,15);p.scale=.95f;break;
                case "Fries": p.rotation=new Vector3(55,0,0);p.scale=1.1f;break;
                case "Hotdog": p.rotation=new Vector3(-75,0,-35);p.scale=.9f;break;
                case "IceCream": p.rotation=new Vector3(-20,0,-10);break;
                case "Pizza": p.rotation=new Vector3(-70,0,15);break;
                case "Popsicle": p.rotation=new Vector3(-10,0,12);p.scale=1.1f;break;
                case "Pumpkin": p.rotation=new Vector3(-35,0,0);p.scale=1.1f;break;
                case "Soda": p.rotation=new Vector3(-15,0,-10);p.scale=1.1f;break;
                case "Steak": p.rotation=new Vector3(-75,0,-25);break;
                default: continue;
            }
            settings.Set(prefab,p);
        }
        EditorUtility.SetDirty(settings);AssetDatabase.SaveAssets();CaptureConfigured();
    }
    public static void CaptureConfigured(){
        string directory="Builds/TrayDisplay/configured";Directory.CreateDirectory(directory);var settings=TrayDisplayWindow.EnsureSettings();
        using(var preview=new TrayDisplayPreview())foreach(var prefab in LevelLayoutConfig.Get("CountrysideStove").prefabs){
            preview.Select(prefab);settings.TryGet(prefab,out var pose);
            var image=preview.Capture(pose,360,240,true);File.WriteAllBytes(directory+"/"+prefab.name+".png",image.EncodeToPNG());Object.DestroyImmediate(image);
            if(prefab.name=="Bottle"){image=preview.Capture(pose);File.WriteAllBytes("Builds/TrayDisplay/bottle-seven-slots.png",image.EncodeToPNG());Object.DestroyImmediate(image);}
        }
    }
    public static void CaptureCandidates(){
        string directory="Builds/TrayDisplay/candidates";Directory.CreateDirectory(directory);
        var profile=LevelLayoutConfig.Get("CountrysideStove");
        var angles=new[]{new Vector3(-90,0,0),new Vector3(-25,0,0),new Vector3(-25,135,0),new Vector3(-25,180,0)};
        using(var preview=new TrayDisplayPreview())foreach(var prefab in profile.prefabs){
            preview.Select(prefab);
            for(int i=0;i<angles.Length;i++){
                var image=preview.Capture(new Match3DTrayDisplaySettings.Pose{rotation=angles[i]},256,180,true);
                File.WriteAllBytes(directory+"/"+prefab.name+"-"+i+".png",image.EncodeToPNG());Object.DestroyImmediate(image);
            }
        }
        Debug.Log("TRAY_DISPLAY_CANDIDATES "+directory);
    }
    public static void CaptureFries(){
        var prefab=System.Array.Find(LevelLayoutConfig.Get("CountrysideStove").prefabs,p=>p.name=="Fries");
        var angles=new[]{new Vector3(25,0,0),Vector3.zero,new Vector3(25,180,0),new Vector3(90,0,0),new Vector3(0,0,180),new Vector3(-40,180,180)};
        using(var preview=new TrayDisplayPreview()){
            preview.Select(prefab);
            for(int i=0;i<angles.Length;i++){var image=preview.Capture(new Match3DTrayDisplaySettings.Pose{rotation=angles[i]},360,240,true);File.WriteAllBytes("Builds/TrayDisplay/fries-"+i+".png",image.EncodeToPNG());Object.DestroyImmediate(image);}
        }
    }
}
