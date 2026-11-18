mergeInto(LibraryManager.library, {
 $GooseReviveAds: {
  active:null,ads:{},recent:null,hooksInstalled:false,
  // Diagnostics must never affect rewards, callback delivery or native lifecycle.
  trace:function(state,event,detail){
   try{
    if(!state)return;
    var row={tag:'PotAdDiagnostics',event:event,requestId:state.request,adUnitId:state.id,
     placement:state.placement,clientVersion:state.clientVersion,platform:state.platform,
     at:Date.now(),elapsedMs:Date.now()-state.started,showRequested:!!state.showRequested,
     finished:!!state.finished,cancelled:!!state.cancelled,detail:detail||''};
    var line=JSON.stringify(row),g=typeof GameGlobal!=='undefined'?GameGlobal:{};
    try{if(g.realtimeLogManager)g.realtimeLogManager.info('[PotAdDiagnostics]',line);}catch(e){}
    try{if(g.logmanager)g.logmanager.info('[PotAdDiagnostics]',line);}catch(e){}
    try{if(typeof console!=='undefined'&&console.info)console.info('[PotAdDiagnostics]',line);}catch(e){}
   }catch(e){}
  },
  install:function(){
   var q=GooseReviveAds;if(q.hooksInstalled)return;q.hooksInstalled=true;
   function record(event,detail){
    var s=q.active||q.recent;
    if(s&&(q.active===s||Date.now()-(s.finishedAt||s.started)<120000))q.trace(s,event,detail);
   }
   function hook(name,event){try{if(wx[name])wx[name](function(){record(event);});}catch(e){}}
   hook('onHide','app_hide');hook('onShow','app_show');hook('onMemoryWarning','memory_warning');
   hook('onError','runtime_error');hook('onUnhandledRejection','unhandled_rejection');
   // Observe only: do not preventDefault or force a graphics-context restore.
   try{if(typeof canvas!=='undefined'&&canvas.addEventListener){
    canvas.addEventListener('webglcontextlost',function(){record('context_lost');});
    canvas.addEventListener('webglcontextrestored',function(){record('context_restored');});
   }}catch(e){}
  }
 },
 PotAdDiagnosticMark__deps:['$GooseReviveAds'],
 PotAdDiagnosticMark:function(requestPtr,eventPtr,detailPtr){
  var q=GooseReviveAds,id=UTF8ToString(requestPtr),s=q.active;
  if(!s||s.request!==id)s=q.recent;
  if(s&&s.request===id)q.trace(s,UTF8ToString(eventPtr),UTF8ToString(detailPtr));
 },
 GooseReviveAdShow__deps:['$GooseReviveAds','$GooseAdTelemetry'],
 GooseReviveAdShow:function(targetPtr,idPtr,requestPtr,contextPtr){
  var target=UTF8ToString(targetPtr),id=UTF8ToString(idPtr),request=UTF8ToString(requestPtr);
  var state={request:request,id:id,started:Date.now(),ad:null,finished:false,timer:null,showRequested:false,cancelled:false};
  function trace(event,detail){GooseReviveAds.trace(state,event,detail);}
  function emit(ok,error){
   trace('unity_dispatch',ok?'completed':error);
   try{Module.SendMessage(target,'OnReviveAdReply',JSON.stringify({requestId:request,completed:ok,error:error||''}));trace('unity_dispatch_returned');}
   catch(e){trace('unity_dispatch_failed');throw e;}
  }
  trace('request');
  if(GooseReviveAds.active){emit(false,'广告正在播放，请稍候');return;}
  if(typeof wx==='undefined'||!wx.createRewardedVideoAd){emit(false,'当前环境不支持激励视频');return;}
  var context;try{context=JSON.parse(UTF8ToString(contextPtr));}catch(e){emit(false,'广告记录初始化失败，请重试');return;}
  if(!context||context.impressionId!==request||context.adUnitId!==id||!GooseAdTelemetry.prepare(context)){emit(false,'广告记录暂无法保存，请联网并检查设备存储后重试');return;}
  state.placement=context.placement;state.clientVersion=context.clientVersion;state.platform=context.platform;
  GooseReviveAds.active=state;GooseReviveAds.recent=state;GooseReviveAds.install();trace('prepared');
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
   if(state.finished)return;state.finished=true;state.finishedAt=Date.now();
   if(ok)GooseAdTelemetry.complete(context.owner,request);else GooseAdTelemetry.discard(context.owner,request);
   cleanup();trace('finished',ok?'completed':error);if(!state.cancelled)emit(ok,error);
  }
  function onClose(res){trace('native_close',res&&res.isEnded===true?'completed':res&&res.isEnded===false?'incomplete':'missing_result');finish(!!(res&&res.isEnded===true),res&&res.isEnded===true?'':'请完整观看视频后领取奖励');}
  function onError(error){
   trace('native_error',error&&typeof error.errCode==='number'?String(error.errCode):'no_error_code');
   if(state.finished)return;
   if(typeof console!=='undefined')console.warn('[GooseReviveAds]',id,error);
   finish(false,'广告暂时不可用，请稍后重试');
  }
  state.cancel=function(){
   trace('cancel');if(state.finished)return;state.cancelled=true;
   // Native close is still authoritative after the Unity view has gone away.
   if(!state.showRequested)finish(false,'');
  };
  try{
   state.ad=GooseReviveAds.ads[id];
   if(!state.ad){state.ad=wx.createRewardedVideoAd({adUnitId:id});GooseReviveAds.ads[id]=state.ad;}
   state.ad.onClose(onClose);state.ad.onError(onError);
   state.timer=setTimeout(function(){trace('load_timeout');finish(false,'广告加载超时，请重试');},20000);
   // Explicit load makes loading errors recoverable without issuing duplicate show calls.
   trace('load_start');
   Promise.resolve(state.ad.load()).then(function(){trace('load_resolved');if(state.finished)return;state.showRequested=true;clearTimeout(state.timer);trace('show_start');return Promise.resolve(state.ad.show()).then(function(){trace('show_resolved');});}).catch(onError);
  }catch(e){onError(e);}
 },
 GooseReviveAdCancel__deps:['$GooseReviveAds'],
 GooseReviveAdCancel:function(requestPtr){var s=GooseReviveAds.active;if(s&&s.request===UTF8ToString(requestPtr))s.cancel();}
});
