using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public sealed class PotMasterReplayEntry : MonoBehaviour {
    Button button;string owner,date;bool refreshing;float next;
    public void Configure(Match3DUIView view){
        var bachelor=view.Get<Button>("Bachelor").GetComponent<RectTransform>();
        button=PotMasterReplayView.MakeButton(bachelor.parent,"Pot master replay entry","大神\n回放",0,0,140,90,25);
        var rect=(RectTransform)button.transform;rect.anchorMin=bachelor.anchorMin;rect.anchorMax=bachelor.anchorMax;rect.pivot=bachelor.pivot;
        rect.anchoredPosition=bachelor.anchoredPosition+new Vector2(266,-15);
        button.onClick.AddListener(()=>{if(owner==PotMasterReplayAccess.Account()&&date==Match3DFlow.Today){Match3DFlow.Instance.HomeClick();Match3DFlow.Instance.ShowPotMasterReplays();}});
        button.gameObject.SetActive(false);
    }
    void Update(){
        if(!button)return;
        string account=PotMasterReplayAccess.Account(),today=Match3DFlow.Today;
        if(account!=owner||today!=date){owner=account;date=today;button.gameObject.SetActive(false);next=0;}
        if(!refreshing&&!string.IsNullOrEmpty(owner)&&Time.realtimeSinceStartup>=next&&PotReplayService.Instance)StartCoroutine(Refresh(owner,date));
    }
    IEnumerator Refresh(string account,string day){
        refreshing=true;
        yield return Match3DStartupRunner.Run(PotReplayService.Instance.Masters(reply=>{
            if(this&&button&&account==PotMasterReplayAccess.Account()&&day==Match3DFlow.Today)button.gameObject.SetActive(PotMasterReplayAccess.Visible(reply,day));
        }),error=>{if(button)button.gameObject.SetActive(false);Debug.LogWarning("Pot master entry: "+error.Message);});
        refreshing=false;next=Time.realtimeSinceStartup+30;
    }
    void OnDestroy(){if(button)Destroy(button.gameObject);}
}
