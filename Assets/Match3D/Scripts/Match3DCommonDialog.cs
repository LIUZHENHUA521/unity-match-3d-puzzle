using System;
using UnityEngine;
using UnityEngine.UI;
// Reusable two-action dialog: copy, labels and callbacks are supplied by the caller.
public sealed class Match3DCommonDialog : MonoBehaviour {
 public Text title,body,leftLabel,rightLabel,status;public Button left,right,close;
 Action cancel,confirm;
 public void Configure(string heading,string content,string leftText,string rightText,Action onCancel,Action onConfirm){
  var ribbon=title.transform.parent.Find("Title ribbon");
  if(ribbon&&ribbon.GetSiblingIndex()>title.transform.GetSiblingIndex())ribbon.SetSiblingIndex(title.transform.GetSiblingIndex());
  title.text=heading;body.text=content;leftLabel.text=leftText;rightLabel.text=rightText;cancel=onCancel;confirm=onConfirm;
  left.onClick.AddListener(()=>{Match3DFlow.Instance.HomeClick();cancel?.Invoke();});close.onClick.AddListener(()=>{Match3DFlow.Instance.HomeClick();cancel?.Invoke();});right.onClick.AddListener(()=>{Match3DFlow.Instance.HomeClick();confirm?.Invoke();});
 }
 public void SetBusy(bool value){left.interactable=right.interactable=close.interactable=!value;status.text=value?"正在领取，请稍候…":"";}
}
