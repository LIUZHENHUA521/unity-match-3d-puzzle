using UnityEngine;
using UnityEngine.UI;
public sealed class Match3DHomeMenu : MonoBehaviour {
 public GameObject mailDot;Match3DInvitations service;
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
 public void Connect(Match3DFlow flow){var view=GetComponent<Match3DUIView>();view.Bind("Dismiss",()=>Destroy(gameObject));view.Bind("Settings",()=>{Destroy(gameObject);flow.ShowSettings(false);});view.Bind("Mail",()=>{Destroy(gameObject);flow.ShowMailbox();});view.Bind("Card",()=>{Destroy(gameObject);flow.ShowUserProfile();});view.Bind("Service",()=>{Destroy(gameObject);Match3DCustomerService.Open();});view.Get<Button>("Cash").GetComponentInChildren<Text>().text="我的红包";view.Bind("Cash",()=>{Destroy(gameObject);flow.ShowRedPackets();});service=Match3DInvitations.Instance;if(service){service.MailChanged+=Refresh;Refresh();service.RefreshMail();}}
 void Refresh(){mailDot.SetActive(service&&service.MailAttention);}
 void OnDestroy(){if(service)service.MailChanged-=Refresh;}
}
