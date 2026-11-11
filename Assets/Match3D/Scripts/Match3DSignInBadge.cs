using UnityEngine;

public sealed class Match3DSignInBadge : MonoBehaviour {
    Match3DCircleGraphic dot;Match3DInvitations service;
    void Start(){
        var child=new GameObject("Sign-in available",typeof(RectTransform));child.transform.SetParent(transform,false);
        var rect=(RectTransform)child.transform;rect.anchorMin=rect.anchorMax=new Vector2(1,1);rect.sizeDelta=new Vector2(19,19);rect.anchoredPosition=new Vector2(-12,-12);
        dot=child.AddComponent<Match3DCircleGraphic>();dot.color=new Color32(255,66,96,255);dot.raycastTarget=false;
        service=Match3DInvitations.Instance;if(service)service.SignInChanged+=Refresh;Refresh();
    }
    void Refresh(){if(dot)dot.enabled=service&&service.SignInCurrent&&service.SignInState.canClaim;}
    void OnDestroy(){if(service)service.SignInChanged-=Refresh;}
}
