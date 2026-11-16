using System;
using UnityEngine;
using UnityEngine.UI;

public sealed class PotGloryRow : MonoBehaviour {
    public Text tier,date,duration,replayState;
    public GameObject badge;
    public Button certificate,replay;
    public void Configure(PotReplayService.Honor honor,bool fresh,Action showCertificate,Action showReplay){
        var data=honor.certificate;tier.text=data.rewardTier=="elite"?"清北局":"本科局";
        date.text=(data.dateKey??"").Replace('-','.');
        long seconds=Math.Max(0,data.activeDurationMs/1000);duration.text=$"{seconds/60:00}:{seconds%60:00}";
        badge.SetActive(fresh);certificate.onClick.AddListener(()=>showCertificate());replay.onClick.AddListener(()=>showReplay());
        bool ready=!string.IsNullOrEmpty(honor.replayId);replayState.transform.parent.gameObject.SetActive(!ready);
        replayState.text="回放待同步";replay.image.color=ready?Color.white:new Color(.65f,.65f,.65f,1);
    }
}
