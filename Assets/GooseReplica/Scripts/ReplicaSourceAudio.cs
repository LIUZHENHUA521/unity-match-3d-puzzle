using System;
using System.Collections.Generic;
using UnityEngine;

// WX SoundManager.PlayEffectSound j3492: shared 50 ms same-sound gate and
// three fixed-pitch sources selected by consecutive object names. No random pitch.
public sealed class ReplicaSourceAudio : MonoBehaviour {
    [Serializable] public class Item {public string name;public int select,combine;public bool usePitch,combineDefault,goose;}
    [Serializable] class Catalog {public Item[] items;public string[] names;}
    static Catalog catalog;static Dictionary<string,Item> items;
    static readonly Dictionary<GameObject,Item> bound=new Dictionary<GameObject,Item>();
    readonly Dictionary<int,AudioClip> clips=new Dictionary<int,AudioClip>();
    readonly SourceSoundSequence sequence=new SourceSoundSequence();
    AudioSource[] effects;AudioSource goose;
    public int PlayedCount {get;private set;}
    public int LastSound {get;private set;}
    public float LastPitch {get;private set;}
    public static bool Enabled=>ReplicaFlow.Instance&&ReplicaFlow.Instance.SettingsEffects;
    static void LoadCatalog(){if(catalog!=null)return;catalog=JsonUtility.FromJson<Catalog>(Resources.Load<TextAsset>("Replica/SourceAudioItems").text);items=new Dictionary<string,Item>();foreach(var item in catalog.items)items.Add(item.name,item);}
    public static void Bind(GameObject obj,string sourceName){LoadCatalog();if(items.TryGetValue(sourceName,out var item))bound[obj]=item;}
    public static void Forget(GameObject obj){if(!ReferenceEquals(obj,null))bound.Remove(obj);}
    public static void ResetLevel(){bound.Clear();if(ReplicaFlow.Instance){var audio=ReplicaFlow.Instance.GetComponent<ReplicaSourceAudio>();if(audio)audio.sequence.ResetLevel();}}
    static ReplicaSourceAudio Get(){var flow=ReplicaFlow.Instance;if(!flow)return null;var audio=flow.GetComponent<ReplicaSourceAudio>();return audio?audio:flow.gameObject.AddComponent<ReplicaSourceAudio>();}
    void Awake(){LoadCatalog();effects=new AudioSource[3];for(int i=0;i<3;i++){effects[i]=gameObject.AddComponent<AudioSource>();effects[i].playOnAwake=false;effects[i].volume=1;effects[i].pitch=1+i*.15f;effects[i].spatialBlend=0;}goose=gameObject.AddComponent<AudioSource>();goose.playOnAwake=false;goose.volume=.4f;goose.pitch=1;goose.spatialBlend=0;}
    public static bool TryBoundProfile(GameObject obj,out Item item)=>bound.TryGetValue(obj,out item);
    public static bool TryProfile(string name,out Item item){LoadCatalog();return items.TryGetValue(name,out item);}
    public AudioClip Clip(int id){LoadCatalog();if(id<=0||id>=catalog.names.Length)return null;if(!clips.TryGetValue(id,out var clip)){clip=Resources.Load<AudioClip>("Replica/SourceAudio/"+catalog.names[id]);clips[id]=clip;}return clip;}
    public bool PlayEffect(int id,bool usePitch,string objectName){
        if(!Enabled)return false;
        // Original updates its throttle before checking the loaded clip dictionary.
        if(!sequence.TryBegin(id,Time.time))return false;
        var clip=Clip(id);if(!clip)return false;
        int index=sequence.SelectSource(usePitch,objectName);effects[index].PlayOneShot(clip);
        PlayedCount++;LastSound=id;LastPitch=effects[index].pitch;return true;
    }
    public static void Select(GameObject obj){
        if(!Enabled||!bound.TryGetValue(obj,out var item))return;var audio=Get();
        audio.PlayEffect(item.select,item.usePitch,obj.name);
        // PlaySound invokes goose voice even if the material effect was throttled.
        if(item.goose){var clip=audio.Clip(UnityEngine.Random.Range(31,35));if(clip)audio.goose.PlayOneShot(clip);}
    }
    public static void Combine(GameObject obj){
        if(!Enabled)return;var audio=Get();
        if(obj&&bound.TryGetValue(obj,out var item)&&item.combine!=0){audio.PlayEffect(item.combine,false,"");if(!item.combineDefault)return;}
        audio.PlayEffect(23,false,"");
    }
    public static void Tool(int id){if(Enabled)Get().PlayEffect(id,false,"");}
}

public sealed class SourceSoundSequence {
    int lastSound;float lastTime;string lastName;int repeated;
    public bool TryBegin(int sound,float now){if(sound==lastSound&&now-lastTime<.05f)return false;lastSound=sound;lastTime=now;return true;}
    public int SelectSource(bool usePitch,string name){
        if(!usePitch)return 0;
        if(repeated>0&&lastName==name&&repeated<=2){repeated++;return repeated-1;}
        lastName=name;repeated=1;return 0;
    }
    // Source ResetLevel clears the pitch-name list, not the static last sound/time.
    public void ResetLevel(){lastName=null;repeated=0;}
}
