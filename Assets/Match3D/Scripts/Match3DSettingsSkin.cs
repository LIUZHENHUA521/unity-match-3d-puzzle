using UnityEngine;
using UnityEngine.UI;

// The editable checkmarks reflect the persisted settings, not the PSD's sample state.
public sealed class Match3DSettingsSkin : MonoBehaviour {
    public GameObject musicMark, effectsMark, vibrationMark;
    public Button support;
    public Text supportStatus;
    public void SetState(string key,bool enabled){
        var mark=key=="Music"?musicMark:key=="Effects"?effectsMark:vibrationMark;
        if(mark)mark.SetActive(enabled);
    }
    public void BindSupport(bool ingame=false){
        support.onClick.RemoveAllListeners();
        if(ingame){
            support.image.sprite=Resources.Load<Sprite>("Match3D/SettingsSkinPS/exit-game");
            support.image.preserveAspect=true;
            support.onClick.AddListener(()=>{Match3DFlow.Instance.HomeClick();Match3DFlow.Instance.Home();});
            return;
        }
        support.onClick.AddListener(()=>{
            Match3DFlow.Instance.HomeClick();
            Match3DCustomerService.Open();
        });
    }
}
