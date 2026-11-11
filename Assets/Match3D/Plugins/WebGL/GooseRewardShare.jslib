mergeInto(LibraryManager.library, {
 $GooseRewardSharing: {active:null},
 GooseRewardShare__deps:['$GooseRewardSharing'],
 GooseRewardShare:function(targetPtr,requestPtr,minAwaySeconds){
  var target=UTF8ToString(targetPtr),request=UTF8ToString(requestPtr);
  function emit(ok,error){Module.SendMessage(target,'OnReviveAdReply',JSON.stringify({requestId:request,completed:ok,error:error||''}));}
  if(GooseRewardSharing.active){emit(false,'正在分享，请稍候');return;}
  if(typeof wx==='undefined'||!wx.shareAppMessage||!wx.onHide||!wx.onShow||!wx.offHide||!wx.offShow){emit(false,'当前环境不支持分享');return;}
  var state={request:request,finished:false,hiddenAt:null,timer:null};GooseRewardSharing.active=state;
  function cleanup(){
   clearTimeout(state.timer);wx.offHide(onHide);wx.offShow(onShow);
   if(GooseRewardSharing.active===state)GooseRewardSharing.active=null;
  }
  function finish(ok,error){if(state.finished)return;state.finished=true;cleanup();emit(ok,error);}
  function onHide(){if(!state.finished&&state.hiddenAt===null)state.hiddenAt=Date.now();}
  function onShow(){
   if(state.finished)return;
   // WeChat does not attest delivery. This is only a local return/elapsed-time heuristic.
   var elapsed=state.hiddenAt===null?0:Date.now()-state.hiddenAt;
   var accepted=state.hiddenAt!==null&&elapsed>=Math.max(0,minAwaySeconds)*1000;
   finish(accepted,accepted?'':'分享返回过快，请重试');
  }
  state.cancel=function(){if(state.finished)return;state.finished=true;cleanup();};
  try{
   wx.onHide(onHide);wx.onShow(onShow);
   state.timer=setTimeout(function(){finish(false,'分享等待超时，请重试');},120000);
   wx.shareAppMessage({title:'一起来抓大鹅，看看谁能通关！',query:'source=reward',fail:function(){finish(false,'未能打开分享，请重试');}});
  }catch(e){finish(false,'未能打开分享，请重试');}
 },
 GooseRewardShareCancel__deps:['$GooseRewardSharing'],
 GooseRewardShareCancel:function(requestPtr){var s=GooseRewardSharing.active;if(s&&s.request===UTF8ToString(requestPtr))s.cancel();}
});
