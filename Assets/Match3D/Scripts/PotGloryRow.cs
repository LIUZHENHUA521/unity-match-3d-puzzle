using System;
using UnityEngine;
using UnityEngine.UI;

public sealed class PotGloryRow : MonoBehaviour {
    public Text tier,date,duration,replayState;
    public GameObject badge;
    public Button certificate,replay;
    void BuildThumbnail(PotCertificateData data){
        // Reuse the actual certificate card so every field and the deterministic seal match the full view.
        var prefab=Resources.Load<GameObject>("Match3D/UI/PotCertificate");
        var preview=Instantiate(prefab,transform,false).GetComponent<PotCertificateView>();
        preview.gameObject.SetActive(false);preview.Apply(data);
        var card=preview.card;card.SetParent(transform,false);card.name="Certificate thumbnail";
        card.anchorMin=card.anchorMax=new Vector2(0,1);card.pivot=new Vector2(.5f,.5f);
        card.anchoredPosition=new Vector2(158,-102);card.localScale=Vector3.one*(224f/590f);
        preview.stamp.localRotation=Quaternion.Euler(0,0,-12);
        foreach(var graphic in card.GetComponentsInChildren<Graphic>())graphic.raycastTarget=false;
        Destroy(preview.gameObject);
    }
    public void Configure(PotReplayService.Honor honor,bool fresh,Action showCertificate,Action showReplay){
        var data=honor.certificate;tier.text=data.rewardTier=="elite"?"清北局":"本科局";
        date.text=(data.dateKey??"").Replace('-','.');
        long seconds=Math.Max(0,data.activeDurationMs/1000);duration.text=$"{seconds/60:00}:{seconds%60:00}";
        BuildThumbnail(data);
        badge.SetActive(fresh);
        certificate.onClick.AddListener(()=>{if(Match3DFlow.Instance)Match3DFlow.Instance.HomeClick();showCertificate();});
        replay.onClick.AddListener(()=>{if(Match3DFlow.Instance)Match3DFlow.Instance.HomeClick();showReplay();});
        bool ready=!string.IsNullOrEmpty(honor.replayId);replayState.transform.parent.gameObject.SetActive(!ready);
        replayState.text="回放待同步";replay.image.color=ready?Color.white:new Color(.65f,.65f,.65f,1);
    }
}
