mergeInto(LibraryManager.library, {
 $GooseReviveAds: {active:null,ads:{}},
 GooseReviveAdShow__deps:['$GooseReviveAds','$GooseAdTelemetry'],
 GooseReviveAdShow:function(targetPtr,idPtr,requestPtr,contextPtr){
  var target=UTF8ToString(targetPtr),id=UTF8ToString(idPtr),request=UTF8ToString(requestPtr);
  function emit(ok,error){Module.SendMessage(target,'OnReviveAdReply',JSON.stringify({requestId:request,completed:ok,error:error||''}));}
  if(GooseReviveAds.active){emit(false,'广告正在播放，请稍候');return;}
  if(typeof wx==='undefined'||!wx.createRewardedVideoAd){emit(false,'当前环境不支持激励视频');return;}
  var context;try{context=JSON.parse(UTF8ToString(contextPtr));}catch(e){emit(false,'广告记录初始化失败，请重试');return;}
  if(!context||context.impressionId!==request||context.adUnitId!==id||!GooseAdTelemetry.prepare(context)){emit(false,'广告记录暂无法保存，请联网并检查设备存储后重试');return;}
  var state={request:request,ad:null,finished:false,timer:null,showRequested:false,cancelled:false};GooseReviveAds.active=state;
  function cleanup(){
   clearTimeout(state.timer);
   // WeChat reuses rewarded-video instances. Destroying one after a close can
   // leave the next request holding the same, now unusable native instance.
   if(state.ad){
    try{state.ad.offClose(onClose);}catch(e){}
    try{state.ad.offError(onError);}catch(e){}
   }
   if(GooseReviveAds.active===state)GooseReviveAds.active=null;
  }
  function finish(ok,error){
   if(state.finished)return;state.finished=true;
   if(ok)GooseAdTelemetry.complete(context.owner,request);else GooseAdTelemetry.discard(context.owner,request);
   cleanup();if(!state.cancelled)emit(ok,error);
  }
  function onClose(res){finish(!!(res&&res.isEnded===true),res&&res.isEnded===true?'':'请完整观看视频后领取奖励');}
  function onError(error){
   if(state.finished)return;
   if(typeof console!=='undefined')console.warn('[GooseReviveAds]',id,error);
   finish(false,'广告暂时不可用，请稍后重试');
  }
  state.cancel=function(){
   if(state.finished)return;state.cancelled=true;
   // Native close is still authoritative after the Unity view has gone away.
   if(!state.showRequested)finish(false,'');
  };
  try{
   state.ad=GooseReviveAds.ads[id];
   if(!state.ad){state.ad=wx.createRewardedVideoAd({adUnitId:id});GooseReviveAds.ads[id]=state.ad;}
   state.ad.onClose(onClose);state.ad.onError(onError);
   state.timer=setTimeout(function(){finish(false,'广告加载超时，请重试');},20000);
   // Explicit load makes loading errors recoverable without issuing duplicate show calls.
   Promise.resolve(state.ad.load()).then(function(){if(state.finished)return;state.showRequested=true;clearTimeout(state.timer);return state.ad.show();}).catch(onError);
  }catch(e){onError();}
 },
 GooseReviveAdCancel__deps:['$GooseReviveAds'],
 GooseReviveAdCancel:function(requestPtr){var s=GooseReviveAds.active;if(s&&s.request===UTF8ToString(requestPtr))s.cancel();}
});
