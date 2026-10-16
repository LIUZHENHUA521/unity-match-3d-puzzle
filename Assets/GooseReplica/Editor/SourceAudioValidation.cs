using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class SourceAudioValidation {
    static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
    public static string Run(){
        var sequence=new SourceSoundSequence();
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
        var models=SourceModelJson.Read<SourceModelExport>(File.ReadAllText("Assets/GooseReplica/OriginalModels/models.json"));
        int profiles=0,goose=0;
        var holder=new GameObject("Audio validation");var audio=holder.AddComponent<ReplicaSourceAudio>();
        try{
            foreach(var model in models.models){
                Check(ReplicaSourceAudio.TryProfile(model.name,out var p),"audio profile "+model.name);profiles++;if(p.goose)goose++;
                if(p.select>0)Check(audio.Clip(p.select)&&audio.Clip(p.select).samples>0,"decoded select clip "+model.name);
                if(p.combine>0)Check(audio.Clip(p.combine),"decoded custom combine clip "+model.name);
            }
            foreach(int id in new[]{23,24,25,31,32,33,34,35})Check(audio.Clip(id)&&audio.Clip(id).samples>0,"common/goose/tool clip "+id);
            var sources=holder.GetComponents<AudioSource>();Check(sources.Length==4,"three material sources plus independent goose source");
            for(int i=0;i<3;i++)Check(Mathf.Abs(sources[i].pitch-(1+i*.15f))<.0001f&&sources[i].volume==1&&!sources[i].playOnAwake,"source pitch and volume "+i);
            Check(Mathf.Abs(sources[3].volume-.4f)<.0001f&&sources[3].pitch==1,"goose volume .4");
        }finally{UnityEngine.Object.DestroyImmediate(holder);}
        var report="PASS source audio: "+profiles+" original model profiles / "+goose+" goose voices; original decoded clips; fixed 1/1.15/1.3 source pitches, 50ms same-ID gate, object-name reset and level reset, material/combine/tool/goose routing. Editor asset validation, not hardware acoustic equivalence.\n";
        Directory.CreateDirectory("Temp");File.WriteAllText("Temp/goose-source-audio-validation.txt",report);Debug.Log(report);return report;
    }
    [Serializable] class SourceModelExport {public SourceModel[] models;}
    [Serializable] class SourceModel {public string name;}
}
