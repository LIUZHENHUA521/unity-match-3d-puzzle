using System;
using UnityEngine;

// A local win only enables the pending caption. Playable IDs always come from the server.
public static class PotHomeReplayState {
    public static bool IsTier(string tier)=>tier=="bachelor"||tier=="elite";
    static string Key(string owner,string tier)=>"pot.home-replay.win."+owner+"."+tier;
    public static bool Won(string owner,string date,string tier)=>!string.IsNullOrEmpty(owner)&&PlayerPrefs.GetString(Key(owner,tier),"")==date;
    public static void MarkWin(string owner,Match3DGameRecords.Record record){
        if(string.IsNullOrEmpty(owner)||record==null||record.status!="win"||record.stage!=2||record.levelSeed==0||!IsTier(record.challengeTier)||string.IsNullOrEmpty(record.levelDate))return;
        PlayerPrefs.SetString(Key(owner,record.challengeTier),record.levelDate);PlayerPrefs.Save();
    }
    public static bool Matches(PotReplayService.Summary row,string date,string tier)=>row!=null&&!string.IsNullOrEmpty(row.id)&&row.status=="win"&&row.stage==2&&row.date==date&&row.tier==tier&&(row.resourceVersion==PotReplayData.Resources||row.resourceVersion=="pot-replay-resources-1");
}
