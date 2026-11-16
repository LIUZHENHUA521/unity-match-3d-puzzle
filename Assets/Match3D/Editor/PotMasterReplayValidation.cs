using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class PotMasterReplayValidation {
    static void Check(bool ok,string message){if(!ok)throw new Exception(message);Debug.Log("POT MASTER PASS: "+message);}
    public static void Run(){
        try{
            AssetDatabase.Refresh();
            foreach(var guid in AssetDatabase.FindAssets("t:Texture2D",new[]{"Assets/Match3D/Resources/Match3D/PotMasterReplaySkin"})){
                var importer=(TextureImporter)AssetImporter.GetAtPath(AssetDatabase.GUIDToAssetPath(guid));importer.textureType=TextureImporterType.Sprite;importer.spriteImportMode=SpriteImportMode.Single;importer.mipmapEnabled=false;importer.alphaIsTransparency=true;importer.maxTextureSize=2048;importer.textureCompression=TextureImporterCompression.CompressedHQ;importer.SaveAndReimport();
            }
            var access=new PotMasterReplayAccess();int first=access.Begin("a");Check(first>0&&access.Begin("a")==-1,"double click blocked");
            Check(!access.CompleteAd(first,false)&&!access.Paid("a"),"cancelled ad never unlocks");
            int next=access.Begin("a");Check(access.CompleteAd(next,true)&&access.Paid("a"),"complete ad unlocks selected replay");access.Failed(next);
            next=access.Begin("a");Check(access.Paid("a"),"failed download retry reuses ad");access.Failed(next);
            next=access.Begin("b");Check(!access.Paid("b")&&!access.Paid("a"),"different replay requires new ad");access.CompleteAd(next,true);access.Consumed();Check(!access.Paid("b"),"playback consumes unlock");
            next=access.Begin("a");access.Cancel();Check(!access.CompleteAd(next,true)&&!access.Paid("a"),"late ad callback ignored");
            foreach(int count in new[]{0,5,6,10,11}){
                var reply=new PotReplayService.Reply{ok=true,total=count,date="2026-11-16",levelVersion="v1",list=new[]{new PotReplayService.Summary{id="a"}}};
                Check(PotMasterReplayAccess.Visible(reply,"2026-11-16")== (count>5),"threshold "+count);
                Check(!PotMasterReplayAccess.Visible(reply,"2026-11-17"),"cross-day hidden "+count);
            }
            var root=new GameObject("Pot validation canvas",typeof(RectTransform),typeof(Canvas));root.GetComponent<Canvas>().renderMode=RenderMode.ScreenSpaceOverlay;
            var view=PotMasterReplayView.Create(root.transform);Check(view.content&&view.close&&view.retry,"PSD layout controls created");
            Check(view.GetComponentsInChildren<Image>()[1].sprite!=null,"PSD panel imported");UnityEngine.Object.DestroyImmediate(root);
            AssetDatabase.SaveAssets();Directory.CreateDirectory("work/pot-master-replay");File.WriteAllText("work/pot-master-replay/checks.txt","COMPLETE\n");EditorApplication.Exit(0);
        }catch(Exception e){Debug.LogException(e);EditorApplication.Exit(1);}
    }
}
