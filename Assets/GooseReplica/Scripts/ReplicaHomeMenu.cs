using UnityEngine;
using UnityEngine.UI;
public sealed class ReplicaHomeMenu : MonoBehaviour {
 public GameObject mailDot;ReplicaInvitations service;
 RectTransform artwork,content;
 public void FollowArtwork(RectTransform source){
  if(!source)return;
  artwork=source;
  content=new GameObject("Menu artwork layout",typeof(RectTransform)).GetComponent<RectTransform>();
  content.SetParent(transform,false);
  // Keep the authored menu controls in artwork coordinates, but the dismiss shade full-screen.
  for(int i=transform.childCount-1;i>=0;i--){var child=transform.GetChild(i);if(child!=content)child.SetParent(content,false);}
  FollowLayout();
 }
 void LateUpdate(){FollowLayout();}
 void FollowLayout(){
  if(!artwork||!content)return;
  content.pivot=artwork.pivot;content.sizeDelta=artwork.rect.size;
  content.position=artwork.position;content.rotation=artwork.rotation;
  var parentScale=transform.lossyScale;var scale=artwork.lossyScale;
  content.localScale=new Vector3(scale.x/parentScale.x,scale.y/parentScale.y,1);
 }
 public void Connect(ReplicaFlow flow){var view=GetComponent<ReplicaUIView>();view.Bind("Dismiss",()=>Destroy(gameObject));view.Bind("Settings",()=>{Destroy(gameObject);flow.ShowSettings(false);});view.Bind("Mail",()=>{Destroy(gameObject);flow.ShowMailbox();});view.Bind("Card",()=>{Destroy(gameObject);flow.ShowUserProfile();});view.Bind("Service",()=>flow.ShowHomeFeatureNotice("客服功能尚未接入"));view.Get<Button>("Cash").GetComponentInChildren<Text>().text="我的红包";view.Bind("Cash",()=>{Destroy(gameObject);flow.ShowRedPackets();});service=ReplicaInvitations.Instance;if(service){service.MailChanged+=Refresh;Refresh();service.RefreshMail();}}
 void Refresh(){mailDot.SetActive(service&&service.MailAttention);}
 void OnDestroy(){if(service)service.MailChanged-=Refresh;}
}
