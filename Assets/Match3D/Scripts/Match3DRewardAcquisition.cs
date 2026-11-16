using System;
using UnityEngine;

// Local acquisition order only. Permanent inventory remains server-authorized.
public static class Match3DRewardAcquisition {
    public enum Entry { Tool, Revive }
    public enum Route { Unavailable, Free, Share, Ad }
    [Serializable] public sealed class Rules {
        public int freeUsesPerRound;
        // Retained for legacy config deserialization; cadence now uses adsBeforeShare.
        public string[] initial={"share","ad"},loop={"share","ad"};
    }
    [Serializable] public sealed class Configuration {
        public bool adsEnabled=true,sharesEnabled=true;
        public float minShareAwaySeconds=3;
        public int adsBeforeShare=3;
        public Rules tool=new Rules(),revive=new Rules();
    }
    public sealed class Offer {
        public Entry entry;public Route route;
        internal string owner,key;internal int count,freeUsed,round,revision,configVersion;
        internal bool started,consumed;
    }
    static Configuration config;
    static readonly int[] freeUsed=new int[2];
    static int round,revision,configVersion;
    static string environment="local";
    static Offer active;
    [Serializable] public sealed class PotRemoteConfiguration { public int adsBeforeShare; }
    static bool ValidThreshold(int n)=>n>=1&&n<=100;
    static string ConfigKey=>"pot.acquisition.config.v1."+environment;
    public static void SetEnvironment(string endpoint){
        string next=Uri.EscapeDataString(endpoint??"local");
        if(environment!=next){environment=next;revision++;}
        var settings=Config;settings.adsBeforeShare=PlayerPrefs.GetInt(ConfigKey,3);
        if(!ValidThreshold(settings.adsBeforeShare))settings.adsBeforeShare=3;
        configVersion++;
    }
    public static void ApplyRemote(PotRemoteConfiguration value){
        if(value==null||!ValidThreshold(value.adsBeforeShare))return;
        Config.adsBeforeShare=value.adsBeforeShare;configVersion++;
        PlayerPrefs.SetInt(ConfigKey,value.adsBeforeShare);PlayerPrefs.Save();
    }
    public static Configuration Config {
        get {
            if(config==null){var asset=Resources.Load<TextAsset>("Match3D/RewardAcquisitionConfig");
                try{config=asset?JsonUtility.FromJson<Configuration>(asset.text):null;}catch(Exception e){Debug.LogWarning("Invalid acquisition config: "+e.Message);}
                if(config==null)config=new Configuration();
            }
            return config;
        }
    }
    // Local/test configuration. Login updates only the remote cadence field via ApplyRemote.
    public static void Configure(Configuration value){config=value;configVersion++;}
    static string Owner=>Match3DInvitations.Instance&&Match3DInvitations.Instance.Ready?Match3DInvitations.Instance.State.inviteCode:"local";
    public static string CounterKey(Entry entry,string owner)=>"pot.acquisition.v2."+environment+"."+owner;
    public static void ResetRound(){round++;Array.Clear(freeUsed,0,freeUsed.Length);}
    public static Route Resolve(Configuration settings,Entry entry,int completed,int usedFree){
        if(entry==Entry.Revive)return settings.adsEnabled?Route.Ad:Route.Unavailable;
        var rules=settings.tool??new Rules();
        if(usedFree<Math.Max(0,rules.freeUsesPerRound))return Route.Free;
        int threshold=ValidThreshold(settings.adsBeforeShare)?settings.adsBeforeShare:3;
        if(completed>=threshold&&settings.sharesEnabled)return Route.Share;
        return settings.adsEnabled?Route.Ad:Route.Unavailable;
    }
    public static Offer Next(Entry entry){
        string owner=Owner,key=CounterKey(entry,owner);int count=Math.Max(0,PlayerPrefs.GetInt(key,0)),used=freeUsed[(int)entry];
        return new Offer{entry=entry,route=Resolve(Config,entry,count,used),owner=owner,key=key,count=count,freeUsed=used,round=round,revision=revision,configVersion=configVersion};
    }
    public static bool IsCurrent(Offer offer)=>offer!=null&&!offer.consumed&&offer.round==round&&offer.revision==revision&&offer.owner==Owner&&offer.key==CounterKey(offer.entry,Owner)&&offer.count==Math.Max(0,PlayerPrefs.GetInt(offer.key,0))&&offer.freeUsed==freeUsed[(int)offer.entry]&&(offer.started||(offer.configVersion==configVersion&&offer.route==Resolve(Config,offer.entry,offer.count,offer.freeUsed)));
    public static bool TryBegin(Offer offer){
        if(active!=null||!IsCurrent(offer)||offer.route==Route.Unavailable)return false;
        active=offer;offer.started=true;return true;
    }
    public static void End(Offer offer){if(active==offer)active=null;if(offer!=null)offer.started=false;}
    public static void Commit(Offer offer){
        if(!IsCurrent(offer)||offer.route==Route.Unavailable)return;
        if(offer.route==Route.Free)freeUsed[(int)offer.entry]++;
        else {PlayerPrefs.SetInt(offer.key,offer.route==Route.Share?0:offer.count==int.MaxValue?int.MaxValue:offer.count+1);PlayerPrefs.Save();}
        offer.consumed=true;revision++;
    }
}
