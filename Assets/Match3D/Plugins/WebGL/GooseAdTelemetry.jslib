mergeInto(LibraryManager.library, {
 $GooseAdTelemetry: {
  key:'goose.ads.outbox.v1',items:[],loaded:false,blocked:false,session:null,inflight:null,timer:null,delay:1000,
  notify:function(message){
   if(typeof console!=='undefined')console.warn('[GooseAdTelemetry]',message);
   var s=GooseAdTelemetry.session;
   if(s&&s.target)try{Module.SendMessage(s.target,'OnAdTelemetryWarning',message);}catch(e){}
  },
  save:function(){
   var q=GooseAdTelemetry;if(q.blocked)return false;
   try{wx.setStorageSync(q.key,JSON.stringify({version:1,items:q.items}));return true;}
   catch(e){q.notify('广告记录暂时无法保存，请检查设备存储');return false;}
  },
  init:function(){
   var q=GooseAdTelemetry;if(q.loaded)return;q.loaded=true;
   if(typeof wx==='undefined'){q.blocked=true;q.notify('请在微信小游戏中使用广告');return;}
   try{
    var raw=wx.getStorageSync(q.key),data=raw?JSON.parse(raw):{version:1,items:[]};
    if(data.version!==1||!Array.isArray(data.items)||data.items.some(function(x){return !x||!x.event||!x.event.owner||!x.event.impressionId||['prepared','completed'].indexOf(x.state)<0;}))throw Error('invalid outbox');
    // Interrupted prepared requests have no native completion receipt. Never count them.
    q.items=data.items.filter(function(x){return x.state==='completed';});
    if(q.items.length!==data.items.length)q.save();
   }catch(e){q.blocked=true;q.notify('广告记录缓存无法读取，已保留原数据，请联系客服');}
   if(wx.onShow)wx.onShow(function(){q.wake();});
   if(wx.onNetworkStatusChange)wx.onNetworkStatusChange(function(x){if(x.isConnected)q.wake();});
  },
  bind:function(session){
   var q=GooseAdTelemetry;q.session=session;q.init();q.delay=1000;q.wake();
  },
  prepare:function(event){
   var q=GooseAdTelemetry;q.init();
   if(q.blocked||!q.session||!q.session.token||q.session.owner!==event.owner)return false;
   var id=function(v){return typeof v==='string'&&/^[a-zA-Z0-9_-]{16,80}$/.test(v);};
   var text=function(v,max){return typeof v==='string'&&v.length>0&&v.length<=max&&!/[\u0000-\u001f\u007f]/.test(v);};
   // Validate before the player watches, not after the receipt would be quarantined.
   if(event.schemaVersion!==1||event.eventType!=='completed'||!(/^[a-f0-9]{64}$/).test(event.owner)||!id(event.impressionId)||!id(event.recordId)||!id(event.runId)||[1,2].indexOf(event.stage)<0||
    ['tool_remove','tool_magnet','tool_shuffle','tool_time','revive_clear','revive_time'].indexOf(event.placement)<0||!text(event.adUnitId,128)||!text(event.clientVersion,80)||
    ['ios','android','windows','mac','iOS','Android'].indexOf(event.platform)<0){q.notify('广告记录信息尚未准备好，请重新进入关卡后重试');return false;}
   if(q.items.length>=1000){q.notify('广告记录缓存已满，请联网同步后重试');return false;}
   if(q.items.some(function(x){return x.event.owner===event.owner&&x.event.impressionId===event.impressionId;}))return false;
   var item={state:'prepared',event:JSON.parse(JSON.stringify(event))};q.items.push(item);
   if(q.save())return true;
   q.items.splice(q.items.indexOf(item),1);return false;
  },
  complete:function(owner,id){
   var q=GooseAdTelemetry,item=q.items.find(function(x){return x.event.owner===owner&&x.event.impressionId===id;});
   if(!item||item.state==='completed')return;
   item.state='completed';item.event.occurredAt=Date.now();
   q.save();q.wake(); // Keep the in-memory completion even if storage is temporarily full.
  },
  discard:function(owner,id){
   var q=GooseAdTelemetry,index=q.items.findIndex(function(x){return x.state==='prepared'&&x.event.owner===owner&&x.event.impressionId===id;});
   if(index>=0){var item=q.items.splice(index,1)[0];if(!q.save())q.items.splice(index,0,item);}
  },
  schedule:function(){
   var q=GooseAdTelemetry;if(q.timer||q.blocked)return;
   q.timer=setTimeout(function(){q.timer=null;q.flush();},Math.min(60000,q.delay+Math.floor(Math.random()*q.delay*0.2)));
   q.delay=Math.min(60000,q.delay*2);
  },
  wake:function(){var q=GooseAdTelemetry;if(q.timer){clearTimeout(q.timer);q.timer=null;}q.flush();},
  flush:function(){
   var q=GooseAdTelemetry,s=q.session;if(q.blocked||q.inflight||!s||!s.token)return;
   var item=q.items.find(function(x){return x.state==='completed'&&!x.quarantined&&x.event.owner===s.owner;});
   if(!item)return;
   q.save(); // Retry a failed durable write before networking.
   var flight={item:item,session:s,done:false};q.inflight=flight;
   function finish(response){
    if(flight.done)return;flight.done=true;if(q.inflight===flight)q.inflight=null;
    var code=response&&response.statusCode,body=response&&response.data;
    if(typeof body==='string')try{body=JSON.parse(body);}catch(e){body=null;}
    if(code===200&&body&&body.ok===true&&body.impressionId===item.event.impressionId&&body.eventType==='completed'){
     var index=q.items.indexOf(item);if(index>=0){q.items.splice(index,1);if(!q.save())q.items.splice(index,0,item);}
     q.delay=1000;q.schedule();return;
    }
    if(code===401){
     // A response for an old account/session must not invalidate a newer login.
     if(q.session===s){q.session={owner:s.owner,token:'',target:s.target,endpoint:s.endpoint};
      try{Module.SendMessage(s.target,'OnAdTelemetryAuthExpired',s.owner);}catch(e){q.notify('广告记录等待重新登录后同步');}}
     q.schedule();return;
    }
    if(code===400||code===409){item.quarantined=true;item.rejectedStatus=code;q.save();q.notify('有广告记录未能同步，已保留，请联系客服');q.delay=1000;}
    q.schedule();
   }
   try{wx.request({url:s.endpoint+'/ads/events',method:'POST',timeout:10000,
    header:{'Content-Type':'application/json','Authorization':'Bearer '+s.token},data:JSON.parse(JSON.stringify(item.event)),
    success:finish,fail:function(){finish(null);}});}catch(e){finish(null);}
  }
 },
 GooseAdTelemetryBind__deps:['$GooseAdTelemetry'],
 GooseAdTelemetryBind:function(target,endpoint,owner,token){
  GooseAdTelemetry.bind({target:UTF8ToString(target),endpoint:UTF8ToString(endpoint),owner:UTF8ToString(owner),token:UTF8ToString(token)});
 }
});
