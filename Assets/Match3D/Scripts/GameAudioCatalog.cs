using System;
using System.Collections.Generic;
using UnityEngine;

// Shared 50 ms same-sound gate and three fixed-pitch sources selected by
// consecutive object names. Current audio comes from the configured replacement clips.
public sealed class GameAudioCatalog : MonoBehaviour {
    [Serializable] public class Item {public string name;public int select,combine;public bool usePitch,combineDefault,goose;}
    static Dictionary<string,Item> items;
    static readonly Dictionary<GameObject,Item> bound=new Dictionary<GameObject,Item>();
    readonly BoardSoundSequence sequence=new BoardSoundSequence();
    AudioSource[] effects;AudioClip selectionClip,combineClip;
    public AudioClip SelectionClip=>selectionClip?selectionClip:(selectionClip=Resources.Load<AudioClip>("Match3D/SourceAudio/DM-CGS-32"));
    public int PlayedCount {get;private set;}
    public int LastSound {get;private set;}
    public float LastPitch {get;private set;}
    public static bool Enabled=>Match3DFlow.Instance&&Match3DFlow.Instance.SettingsEffects;
    static void LoadCatalog(){
        if(items!=null)return;
        items=new Dictionary<string,Item>(StringComparer.OrdinalIgnoreCase);
        foreach(var id in AdapterIds())if(ItemCatalog.TryGetAudioProfile(id,out var profile))
            items[id]=new Item{name=string.IsNullOrEmpty(profile.name)?id:profile.name,select=profile.select,combine=profile.combine,usePitch=profile.usePitch,combineDefault=profile.combineDefault,goose=profile.goose};
    }
    static IEnumerable<string> AdapterIds(){return ItemCatalog.GetIds();}
    public static void ClearCache(){items=null;ItemCatalog.ClearCache();}
    public static void Bind(GameObject obj,string sourceName){LoadCatalog();if(items.TryGetValue(sourceName,out var item))bound[obj]=item;}
    public static void Forget(GameObject obj){if(!ReferenceEquals(obj,null))bound.Remove(obj);}
    public static void ResetLevel(){bound.Clear();if(Match3DFlow.Instance){var audio=Match3DFlow.Instance.GetComponent<GameAudioCatalog>();if(audio)audio.sequence.ResetLevel();}}
    static GameAudioCatalog Get(){var flow=Match3DFlow.Instance;if(!flow)return null;var audio=flow.GetComponent<GameAudioCatalog>();return audio?audio:flow.gameObject.AddComponent<GameAudioCatalog>();}
    void Awake(){LoadCatalog();effects=new AudioSource[3];for(int i=0;i<3;i++){effects[i]=gameObject.AddComponent<AudioSource>();effects[i].playOnAwake=false;effects[i].volume=1;effects[i].pitch=1+i*.15f;effects[i].spatialBlend=0;}}
    public static bool TryBoundProfile(GameObject obj,out Item item)=>bound.TryGetValue(obj,out item);
    public static bool TryProfile(string name,out Item item){LoadCatalog();return items.TryGetValue(name,out item);}
    // ID 23 is retained for existing playback diagnostics and throttle sequencing.
    public AudioClip Clip(int id)=>id==23?(combineClip?combineClip:(combineClip=Resources.Load<AudioClip>("Match3D/SourceAudio/unfa_cartoon_pop"))):null;
    public static void Preload(string name){
        var audio=Get();if(!audio||!TryProfile(name,out var item))return;
        if(audio.SelectionClip)audio.SelectionClip.LoadAudioData();
        var clip=audio.Clip(23);if(clip)clip.LoadAudioData();
    }
    public bool PlayEffect(int id,bool usePitch,string objectName)=>PlayEffect(id,usePitch,objectName,null);
    bool PlayEffect(int id,bool usePitch,string objectName,AudioClip overrideClip){
        if(!Enabled)return false;
        // Apply the gate before resolving playback, preserving the event sequence.
        if(!sequence.TryBegin(id,Time.time))return false;
        var clip=overrideClip?overrideClip:Clip(id);if(!clip)return false;
        int index=sequence.SelectSource(usePitch,objectName);effects[index].PlayOneShot(clip);
        PlayedCount++;LastSound=id;LastPitch=effects[index].pitch;return true;
    }
    public static void Select(GameObject obj){
        if(!Enabled||!bound.TryGetValue(obj,out var item))return;var audio=Get();
        // Preserve the configured throttle ID and object-name pitch sequence.
        if(item.select>0)audio.PlayEffect(item.select,item.usePitch,obj.name,audio.SelectionClip);
    }
    public static void Combine(GameObject obj){
        if(!Enabled)return;var audio=Get();
        audio.PlayEffect(23,false,"");
    }
}

public sealed class BoardSoundSequence {
    int lastSound;float lastTime;string lastName;int repeated;
    public bool TryBegin(int sound,float now){if(sound==lastSound&&now-lastTime<.05f)return false;lastSound=sound;lastTime=now;return true;}
    public int SelectSource(bool usePitch,string name){
        if(!usePitch)return 0;
        if(repeated>0&&lastName==name&&repeated<=2){repeated++;return repeated-1;}
        lastName=name;repeated=1;return 0;
    }
    // A new level resets pitch sequencing; the short same-sound gate stays intact.
    public void ResetLevel(){lastName=null;repeated=0;}
}
