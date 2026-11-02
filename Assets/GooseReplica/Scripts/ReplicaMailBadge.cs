using UnityEngine;
using UnityEngine.UI;
public sealed class ReplicaMailBadge : MonoBehaviour {
 ReplicaInvitations service;Image icon;
 void Start(){icon=GetComponent<Image>();service=ReplicaInvitations.Instance;if(service){service.MailChanged+=Refresh;Refresh();}}
 void Refresh(){if(icon)icon.enabled=service&&service.MailAttention;}
 void OnDestroy(){if(service)service.MailChanged-=Refresh;}
}
