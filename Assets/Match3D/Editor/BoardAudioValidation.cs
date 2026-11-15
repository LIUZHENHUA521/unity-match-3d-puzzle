using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class BoardAudioValidation {
    static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
    public static string Run(){
        var sequence=new BoardSoundSequence();
        Check(sequence.TryBegin(4,1),"first material sound accepted");
        Check(sequence.SelectSource(true,"apple")==0,"first apple source pitch 1");
        Check(!sequence.TryBegin(4,1.02f),"same sound blocked inside 50ms");
        Check(sequence.TryBegin(4,1.06f)&&sequence.SelectSource(true,"apple")==1,"second accepted apple pitch 1.15");
        Check(sequence.TryBegin(4,1.12f)&&sequence.SelectSource(true,"apple")==2,"third accepted apple pitch 1.3");
        Check(sequence.TryBegin(4,1.18f)&&sequence.SelectSource(true,"apple")==0,"fourth apple restarts pitch sequence");
        Check(sequence.TryBegin(23,1.19f)&&sequence.SelectSource(false,"")==0,"different ID bypasses same-ID gate and nonpitch does not advance names");
        Check(sequence.TryBegin(4,1.2f)&&sequence.SelectSource(true,"apple")==1,"combine leaves material-name run intact");
        Check(sequence.SelectSource(true,"pear")==0,"different object resets even if material sound is shared");
        sequence.ResetLevel();Check(sequence.SelectSource(true,"pear")==0,"level reset clears pitch run");
        int profiles=0;
        var holder=new GameObject("Audio validation");var audio=holder.AddComponent<GameAudioCatalog>();
        try{
            foreach(var id in ItemCatalog.GetIds()){
                Check(GameAudioCatalog.TryProfile(id,out var p),"audio profile "+id);profiles++;
                Check(p.select>0&&p.combine==0&&!p.goose,"current profile uses replacement selection/default combine: "+id);
            }
            var themes=Resources.Load<Match3DModelThemes>("Match3D/ModelThemes");
            foreach(var theme in themes.themes){
                foreach(var prefab in theme.profile.prefabs)Check(GameAudioCatalog.TryProfile(prefab.name,out _),"theme audio bound "+prefab.name);
                foreach(var prefab in LevelLayoutConfig.GetTutorial(theme.profile).prefabs)Check(GameAudioCatalog.TryProfile(prefab.name,out _),"tutorial audio bound "+prefab.name);
            }
            Check(audio.SelectionClip&&audio.SelectionClip.name=="DM-CGS-32"&&audio.SelectionClip.samples==8253,"selection clip and original sample count preserved");
            Check(audio.Clip(23)&&audio.Clip(23).name=="unfa_cartoon_pop"&&Mathf.Abs(audio.Clip(23).length-8254f/48000f)<.002f,"combine clip decodes with unchanged duration");
            foreach(int id in new[]{1,4,16,24,25,31,32,33,34,35})Check(!audio.Clip(id),"retired clip route absent "+id);
            Check(!GameAudioCatalog.TryProfile("Goose_Tomato",out _),"retired source-model mapping absent");
            var reward=Resources.Load<AudioClip>("Match3D/SourceAudio/PotRewardReceived");Check(reward&&reward.samples>0,"reward clip decodes");
            var sources=holder.GetComponents<AudioSource>();Check(sources.Length==3,"only three current pitch sources, no goose source");
            for(int i=0;i<3;i++)Check(Mathf.Abs(sources[i].pitch-(1+i*.15f))<.0001f&&sources[i].volume==1&&!sources[i].playOnAwake,"source pitch and volume "+i);
        }finally{UnityEngine.Object.DestroyImmediate(holder);}
        var report="PASS current audio: "+profiles+" adapter profiles; selection/combine/reward clips; fixed pitches, 50ms gate and reset; retired routes absent.\n";
        Directory.CreateDirectory("Temp");File.WriteAllText("Temp/pot-audio-validation.txt",report);Debug.Log(report);return report;
    }
}
