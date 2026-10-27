using UnityEngine;
using UnityEngine.UI;

// The editable checkmarks reflect the persisted settings, not the PSD's sample state.
public sealed class ReplicaSettingsSkin : MonoBehaviour {
    public GameObject musicMark, effectsMark, vibrationMark;
    public Button support;
    public Text supportStatus;
    public void SetState(string key,bool enabled){
        var mark=key=="Music"?musicMark:key=="Effects"?effectsMark:vibrationMark;
        if(mark)mark.SetActive(enabled);
    }
    public void BindSupport(){
        support.onClick.RemoveAllListeners();
        support.onClick.AddListener(()=>{
            ReplicaFlow.Instance.HomeClick();
            supportStatus.text="客服功能尚未接入";
        });
    }
}
