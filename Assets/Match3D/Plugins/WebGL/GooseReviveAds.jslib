mergeInto(LibraryManager.library, {
 $GooseReviveAds: {active:null},
 GooseReviveAdShow__deps:['$GooseReviveAds'],
 GooseReviveAdShow:function(targetPtr,idPtr,requestPtr){
  var target=UTF8ToString(targetPtr),id=UTF8ToString(idPtr),request=UTF8ToString(requestPtr);
  function emit(ok,error){Module.SendMessage(target,'OnReviveAdReply',JSON.stringify({requestId:request,completed:ok,error:error||''}));}
  if(GooseReviveAds.active){emit(false,'广告正在播放，请稍候');return;}
  if(typeof wx==='undefined'||!wx.createRewardedVideoAd){emit(false,'当前环境不支持激励视频');return;}
  var state={request:request,ad:null,finished:false,timer:null};GooseReviveAds.active=state;
  function cleanup(){
   clearTimeout(state.timer);
   if(state.ad){try{state.ad.offClose(onClose);state.ad.offError(onError);state.ad.destroy();}catch(e){}}
   if(GooseReviveAds.active===state)GooseReviveAds.active=null;
  }
  function finish(ok,error){if(state.finished)return;state.finished=true;cleanup();emit(ok,error);}
  function onClose(res){finish(!!(res&&res.isEnded===true),res&&res.isEnded===true?'':'完整观看视频后才能复活');}
  function onError(){finish(false,'广告暂时不可用，请稍后重试');}
  state.cancel=function(){if(state.finished)return;state.finished=true;cleanup();};
  try{
   state.ad=wx.createRewardedVideoAd({adUnitId:id});state.ad.onClose(onClose);state.ad.onError(onError);
   state.timer=setTimeout(function(){finish(false,'广告加载超时，请重试');},20000);
   // Explicit load makes loading errors recoverable without issuing duplicate show calls.
   Promise.resolve(state.ad.load()).then(function(){if(state.finished)return;return state.ad.show();}).then(function(){if(!state.finished)clearTimeout(state.timer);}).catch(onError);
  }catch(e){onError();}
 },
 GooseReviveAdCancel__deps:['$GooseReviveAds'],
 GooseReviveAdCancel:function(requestPtr){var s=GooseReviveAds.active;if(s&&s.request===UTF8ToString(requestPtr))s.cancel();}
});
