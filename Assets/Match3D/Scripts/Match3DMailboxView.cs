using System;
using UnityEngine;
using UnityEngine.UI;
public sealed class Match3DMailboxView : MonoBehaviour {
 public RectTransform list;public GameObject rowTemplate;public Text status;public Button more;public Sprite unread,read;
 Match3DInvitations service;Match3DCommonDialog dialog;
 public void Connect(Match3DInvitations value){service=value;service.MailChanged+=Render;service.Changed+=UpdateStatus;more.onClick.AddListener(()=>service.RefreshMail(true));Render();service.RefreshMail();}
 void UpdateStatus(){if(!service)return;more.interactable=!service.Busy;status.text=service.Busy?"正在加载…":!string.IsNullOrEmpty(service.Status)?service.Status:service.Mails.Count==0?"暂时没有邮件":"";}
 void Render(){
  if(!service)return;for(int i=list.childCount-1;i>=0;i--)if(list.GetChild(i).gameObject!=rowTemplate){list.GetChild(i).gameObject.SetActive(false);Destroy(list.GetChild(i).gameObject);}
  foreach(var mail in service.Mails){var row=Instantiate(rowTemplate,list,false);row.SetActive(true);var ui=row.GetComponent<Match3DUIView>();row.GetComponent<Image>().sprite=mail.read?read:unread;
   ui.Text("Title",mail.title+(mail.read?"（已读）":"（未读）"));ui.Text("Date",DateTimeOffset.FromUnixTimeMilliseconds(mail.sentAt).ToOffset(TimeSpan.FromHours(8)).ToString("MM-dd HH:mm"));
   ui.Text("Summary",mail.claimed?"道具已领取。":mail.expired?"邮件已过期":mail.HasRewards?"您收到了一份道具奖励，点击查看。":mail.body.Replace("\n"," "));
   ui.Bind("Open",()=>Open(mail));
  }
  more.gameObject.SetActive(service.MailNextCursor>=0);UpdateStatus();
 }
 void Open(Match3DMail mail){
  if(service.Busy||dialog)return;
  var ui=Match3DUIView.Load("CommonDialog",transform);dialog=ui.GetComponent<Match3DCommonDialog>();
  bool canClaim=mail.HasRewards&&!mail.claimed&&!mail.expired;
  string content=mail.body+(mail.HasRewards?"\n\n附件：\n"+mail.RewardText:"");
  dialog.Configure(canClaim?"道具领取":mail.title,content,"返回邮件",canClaim?"确认领取":mail.claimed?"已领取":mail.expired?"已过期":"确定",CloseDetail,()=>{
   if(!canClaim){CloseDetail();return;}if(service.Busy)return;
   dialog.SetBusy(true);service.ClaimMail(mail.id,ok=>{if(!this||!dialog)return;dialog.SetBusy(false);if(ok){CloseDetail();}else{dialog.status.text=service.Status;dialog.right.interactable=service.Ready;}});
  });
  if(mail.claimed||mail.expired)dialog.right.interactable=false;
  service.ReadMail(mail.id);
 }
 void CloseDetail(){if(dialog){dialog.gameObject.SetActive(false);Destroy(dialog.gameObject);dialog=null;}}
 void OnDestroy(){if(service){service.MailChanged-=Render;service.Changed-=UpdateStatus;}}
}
