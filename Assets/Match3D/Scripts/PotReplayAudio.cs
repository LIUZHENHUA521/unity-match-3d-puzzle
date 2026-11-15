using System;
using System.Collections.Generic;
using UnityEngine;

// Recorded events drive audio independently of the frozen live board and its Time.time gate.
public sealed class PotReplayAudio : IDisposable {
    sealed class Cue {public float time,pitch=1,volume=1;public AudioClip clip;}
    sealed class Voice {public AudioSource source;public Cue cue;public bool paused;}
    readonly List<Cue> cues=new List<Cue>();
    readonly List<Voice> voices=new List<Voice>();
    readonly GameObject root;
    int cursor;float previous;
    public int PlayedCount {get;private set;}
    public AudioClip LastClip {get;private set;}
    public float LastPitch {get;private set;}
    public PotReplayAudio(PotReplayData data,Dictionary<int,GameObject> objects){
        var flow=Match3DFlow.Instance;
        root=new GameObject("Pot replay audio");root.transform.SetParent(flow.transform,false);
        var catalog=flow.GetComponent<GameAudioCatalog>()??flow.gameObject.AddComponent<GameAudioCatalog>();
        var sequence=new BoardSoundSequence();
        foreach(var e in data.events){
            var cue=new Cue{time=e.t};
            if(e.type=="pick"){
                // Magnet pickups explicitly use playSound=false in live gameplay; probe picks do not.
                if(e.value==1||!objects.TryGetValue(e.id,out var obj)||!obj||!GameAudioCatalog.TryBoundProfile(obj,out var profile)||profile.select<=0||!sequence.TryBegin(profile.select,e.t))continue;
                cue.clip=catalog.SelectionClip;cue.pitch=1+sequence.SelectSource(profile.usePitch,obj.name)*.15f;
            }else if(e.type=="match"){
                if(!sequence.TryBegin(23,e.t))continue;
                cue.clip=catalog.Clip(23);
            }else if(e.type=="tool"){cue.clip=flow.library.clickSound;cue.volume=.65f;}
            else if(e.type=="finish"){
                cue.clip=data.status=="win"?flow.library.winSound:data.status=="fail"?flow.library.loseSound:null;cue.volume=.65f;
            }else continue;
            if(cue.clip)cues.Add(cue);
        }
    }
    public void Seek(float time){
        Stop();previous=time;
        // Skip past events without emitting them, including when seeking straight to the end.
        int low=0,high=cues.Count;
        while(low<high){int middle=(low+high)/2;if(cues[middle].time<=time)low=middle+1;else high=middle;}
        cursor=low;
    }
    public void Apply(float time,bool playing,float speed){
        if(time<previous)Seek(time);
        previous=time;
        if(!GameAudioCatalog.Enabled){Seek(time);return;}
        if(!playing){foreach(var voice in voices)if(voice.cue!=null&&!voice.paused){voice.source.Pause();voice.paused=true;}return;}
        while(cursor<cues.Count&&cues[cursor].time<=time){
            var cue=cues[cursor++];
            // A stalled frame must not burst-play every sound from the skipped interval.
            if((time-cue.time)*cue.pitch>=cue.clip.length)continue;
            Voice voice=voices.Find(v=>v.cue==null);
            if(voice==null){
                if(voices.Count>=12)continue;
                var source=root.AddComponent<AudioSource>();source.playOnAwake=false;source.spatialBlend=0;
                voice=new Voice{source=source};voices.Add(voice);
            }
            voice.cue=cue;voice.paused=false;voice.source.clip=cue.clip;voice.source.volume=cue.volume;
            voice.source.pitch=Mathf.Clamp(cue.pitch*speed,.1f,3);
            voice.source.time=Mathf.Max(0,(time-cue.time)*cue.pitch);voice.source.Play();
            PlayedCount++;LastClip=cue.clip;LastPitch=voice.source.pitch;
        }
        foreach(var voice in voices){
            if(voice.cue==null)continue;
            float position=(time-voice.cue.time)*voice.cue.pitch;
            if(position>=voice.cue.clip.length){voice.source.Stop();voice.cue=null;voice.paused=false;continue;}
            voice.source.pitch=Mathf.Clamp(voice.cue.pitch*speed,.1f,3);
            if(Mathf.Abs(voice.source.time-position)>.05f)voice.source.time=Mathf.Max(0,position);
            if(voice.paused){voice.source.UnPause();voice.paused=false;}
        }
    }
    void Stop(){foreach(var voice in voices){voice.source.Stop();voice.cue=null;voice.paused=false;}}
    public void Dispose(){Stop();if(root)UnityEngine.Object.Destroy(root);}
}
