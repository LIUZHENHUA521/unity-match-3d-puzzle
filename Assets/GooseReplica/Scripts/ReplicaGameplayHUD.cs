using UnityEngine;
using UnityEngine.UI;

// Authored PSD layout. Only safe-area containers and live quantities change at runtime.
public sealed class ReplicaGameplayHUD : MonoBehaviour {
    public RectTransform header,footer;
    public Text[] quantities;
    Vector2 headerPosition,footerPosition;
    void Awake(){headerPosition=header.anchoredPosition;footerPosition=footer.anchoredPosition;}
    void LateUpdate(){
        var size=((RectTransform)transform).rect.size;
        float units=size.x/Mathf.Max(1,Screen.width);
        header.anchoredPosition=headerPosition+Vector2.down*(Screen.height-Screen.safeArea.yMax)*units;
        footer.anchoredPosition=footerPosition+Vector2.up*Screen.safeArea.yMin*units;
        bool limited=ReplicaFlow.Instance&&!ReplicaFlow.Instance.IsPvp&&ReplicaFlow.Instance.Stage==2;
        for(int i=0;i<quantities.Length;i++){
            quantities[i].transform.parent.gameObject.SetActive(limited);
            if(limited)quantities[i].text=Mathf.Max(0,1-ReplicaSourceTools.UsedCount(i)).ToString();
        }
    }
}
