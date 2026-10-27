using UnityEngine;

// Slot anchors follow the artist's seven cells, shared by rendering and item flight.
public sealed class ReplicaTraySkin : MonoBehaviour {
    public RectTransform[] slots;
    public RectTransform footerBackground;
    Vector2 footerSize,footerPosition;
    void Awake(){if(footerBackground){footerSize=footerBackground.sizeDelta;footerPosition=footerBackground.anchoredPosition;}}
    public void ApplyBottomInset(float inset){
        if(!footerBackground)return;
        footerBackground.sizeDelta=footerSize+Vector2.up*inset;
        footerBackground.anchoredPosition=footerPosition-Vector2.up*inset*.5f;
    }
    public float screenCenterFromBottom=237.5f;
}
