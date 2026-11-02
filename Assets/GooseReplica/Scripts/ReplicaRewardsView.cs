using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public sealed class ReplicaRewardsView : MonoBehaviour {
 public RectTransform list;public GameObject rowTemplate;public Text status;public Button refresh,more;
 int cursor=-1;bool busy;ReplicaCommonDialog dialog;
 public void Connect(){refresh.onClick.AddListener(()=>StartCoroutine(Load(false)));more.onClick.AddListener(()=>StartCoroutine(Load(true)));StartCoroutine(Load(false));}
 IEnumerator Load(bool append){
  if(busy)yield break;busy=true;refresh.interactable=more.interactable=false;status.text="我的红包 · 正在加载…";
  ReplicaRewards.Reply reply=null;yield return ReplicaRewards.Instance.Request("list","{\"cursor\":"+(append?cursor:0)+"}",r=>reply=r);
  if(reply!=null&&reply.ok){
   if(!append)for(int i=list.childCount-1;i>=0;i--)if(list.GetChild(i).gameObject!=rowTemplate){list.GetChild(i).gameObject.SetActive(false);Destroy(list.GetChild(i).gameObject);}
   foreach(var voucher in reply.vouchers??new ReplicaRewards.Voucher[0]){var row=Instantiate(rowTemplate,list,false);row.SetActive(true);var ui=row.GetComponent<ReplicaUIView>();ui.Text("Title",(voucher.rewardTier=="bachelor"?"本科":"清北")+"红包 · "+voucher.money+" 元");ui.Text("Date",DateTimeOffset.FromUnixTimeMilliseconds(voucher.createTime).ToOffset(TimeSpan.FromHours(8)).ToString("MM-dd HH:mm"));ui.Text("Summary",State(voucher.status));ui.Bind("Open",()=>Open(voucher));}
   cursor=reply.nextCursor;status.text="我的红包 · 仅服务端确认的凭证可兑换";
  }else status.text=reply!=null?reply.error:"加载失败，请重试";
  busy=false;refresh.interactable=more.interactable=true;more.gameObject.SetActive(cursor>=0);
 }
 static string State(string s){switch(s){case "binding":return "待绑定兑换账号";case "available":return "待兑换";case "reviewing":return "审核中";case "paying":return "付款处理中";case "redeemed":return "已兑换";default:return "已过期或失效";}}
 void Open(ReplicaRewards.Voucher v){if(dialog)return;var ui=ReplicaUIView.Load("CommonDialog",transform);dialog=ui.GetComponent<ReplicaCommonDialog>();string guide=v.status=="binding"?"兑换账号认证入口接入中，暂不可兑换。":v.rewardTier=="bachelor"?"请联系现有红包客服兑换。":"完成本游戏对应的游戏圈发帖任务，审核通过后兑换。";dialog.Configure("红包凭证",State(v.status)+"\n"+v.money+" 元\n"+v.id+"\n有效期至 "+DateTimeOffset.FromUnixTimeMilliseconds(v.expiresAt).ToOffset(TimeSpan.FromHours(8)).ToString("MM-dd HH:mm")+"\n"+guide,"返回","复制凭证",Close,()=>Copy(v.id));}
 void Copy(string id){
#if UNITY_WEBGL && !UNITY_EDITOR
  WeChatWASM.WX.SetClipboardData(new WeChatWASM.SetClipboardDataOption{data=id,success=_=>{if(dialog)dialog.status.text="凭证已复制";},fail=_=>{if(dialog)dialog.status.text="复制失败，请重试";}});
#else
  GUIUtility.systemCopyBuffer=id;if(dialog)dialog.status.text="凭证已复制";
#endif
 }
 void Close(){if(dialog)Destroy(dialog.gameObject);dialog=null;}
}
