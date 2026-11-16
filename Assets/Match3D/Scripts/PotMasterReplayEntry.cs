using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public sealed class PotMasterReplayEntry : MonoBehaviour {
    Button button;Match3DResultView result;string owner,date;bool refreshing;float next;
    public void Configure(Match3DResultView view){
        result=view;var retry=(RectTransform)view.primary.transform;
        button=PotMasterReplayView.MakeButton(retry.parent,"Pot master replay entry","大神回放",0,0,282,100,32);
        var rect=(RectTransform)button.transform;rect.anchorMin=retry.anchorMin;rect.anchorMax=retry.anchorMax;rect.pivot=retry.pivot;
        rect.anchoredPosition=retry.anchoredPosition+new Vector2(0,120);
        button.onClick.AddListener(()=>{if(owner==PotMasterReplayAccess.Account()&&date==Match3DFlow.Today){Match3DFlow.Instance.HomeClick();Match3DFlow.Instance.ShowPotMasterReplays(result);}});
        Render(false);
    }
    void Update(){
        if(!button)return;
        string account=PotMasterReplayAccess.Account(),today=Match3DFlow.Today;
        if(account!=owner||today!=date){owner=account;date=today;Render(false);next=0;}
        if(!refreshing&&!string.IsNullOrEmpty(owner)&&Time.realtimeSinceStartup>=next&&PotReplayService.Instance)StartCoroutine(Refresh(owner,date));
    }
    IEnumerator Refresh(string account,string day){
        refreshing=true;
        yield return Match3DStartupRunner.Run(PotReplayService.Instance.Masters(reply=>{
            if(this&&button&&account==PotMasterReplayAccess.Account()&&day==Match3DFlow.Today)Render(PotMasterReplayAccess.Visible(reply,day));
        }),error=>{if(button)Render(false);Debug.LogWarning("Pot master entry: "+error.Message);});
        refreshing=false;next=Time.realtimeSinceStartup+30;
    }
    void Render(bool visible){if(button)button.gameObject.SetActive(visible);if(result)result.SetMasterReplayVisible(visible);}
    void OnDestroy(){if(button)Destroy(button.gameObject);}
}
