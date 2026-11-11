using System;
using UnityEngine;

// Local acquisition order only. Permanent inventory remains server-authorized.
public static class Match3DRewardAcquisition {
    public enum Entry { Tool, Revive }
    public enum Route { Unavailable, Free, Share, Ad }
    [Serializable] public sealed class Rules {
        public int freeUsesPerRound;
        public string[] initial={"share","ad"},loop={"share","ad"};
    }
    [Serializable] public sealed class Configuration {
        public bool adsEnabled=true,sharesEnabled=true;
        public float minShareAwaySeconds=3;
        public Rules tool=new Rules(),revive=new Rules();
    }
    public sealed class Offer {
        public Entry entry;public Route route;
        internal string owner,key;internal int count,freeUsed,round;
    }
    static Configuration config;
    static readonly int[] freeUsed=new int[2];
    static int round;
    public static Configuration Config {
        get {
            if(config==null){var asset=Resources.Load<TextAsset>("Match3D/RewardAcquisitionConfig");
                try{config=asset?JsonUtility.FromJson<Configuration>(asset.text):null;}catch(Exception e){Debug.LogWarning("Invalid acquisition config: "+e.Message);}
                if(config==null)config=new Configuration();
            }
            return config;
        }
    }
    // Can be replaced by a runtime configuration provider without changing the consumers.
    public static void Configure(Configuration value){config=value;}
    static string Owner=>Match3DInvitations.Instance&&Match3DInvitations.Instance.Ready?Match3DInvitations.Instance.State.inviteCode:"local";
    public static string CounterKey(Entry entry,string owner)=>"goose.acquisition.v1."+owner+"."+entry;
    public static void ResetRound(){round++;Array.Clear(freeUsed,0,freeUsed.Length);}
    public static Route Resolve(Configuration settings,Entry entry,int completed,int usedFree){
        var rules=(entry==Entry.Tool?settings.tool:settings.revive)??new Rules();
        if(usedFree<Math.Max(0,rules.freeUsesPerRound))return Route.Free;
        var first=rules.initial??new string[0];var loop=rules.loop??new string[0];
        completed=Math.Max(0,completed);
        string step=completed<first.Length?first[completed]:loop.Length>0?loop[(completed-first.Length)%loop.Length]:null;
        var requested=step=="share"?Route.Share:step=="ad"?Route.Ad:Route.Unavailable;
        if(requested==Route.Share)return settings.sharesEnabled?Route.Share:settings.adsEnabled?Route.Ad:Route.Unavailable;
        if(requested==Route.Ad)return settings.adsEnabled?Route.Ad:settings.sharesEnabled?Route.Share:Route.Unavailable;
        return Route.Unavailable;
    }
    public static Offer Next(Entry entry){
        string owner=Owner,key=CounterKey(entry,owner);int count=Math.Max(0,PlayerPrefs.GetInt(key,0)),used=freeUsed[(int)entry];
        return new Offer{entry=entry,route=Resolve(Config,entry,count,used),owner=owner,key=key,count=count,freeUsed=used,round=round};
    }
    public static bool IsCurrent(Offer offer)=>offer!=null&&offer.round==round&&offer.owner==Owner&&offer.count==Math.Max(0,PlayerPrefs.GetInt(offer.key,0))&&offer.freeUsed==freeUsed[(int)offer.entry]&&offer.route==Resolve(Config,offer.entry,offer.count,offer.freeUsed);
    public static void Commit(Offer offer){
        if(!IsCurrent(offer)||offer.route==Route.Unavailable)return;
        if(offer.route==Route.Free)freeUsed[(int)offer.entry]++;
        else {PlayerPrefs.SetInt(offer.key,offer.count+1);PlayerPrefs.Save();}
    }
}
