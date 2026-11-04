using UnityEngine;
using UnityEngine.UI;
public sealed class Match3DMailBadge : MonoBehaviour {
 Match3DInvitations service;Image icon;
 void Start(){icon=GetComponent<Image>();service=Match3DInvitations.Instance;if(service){service.MailChanged+=Refresh;Refresh();}}
 void Refresh(){if(icon)icon.enabled=service&&service.MailAttention;}
 void OnDestroy(){if(service)service.MailChanged-=Refresh;}
}
