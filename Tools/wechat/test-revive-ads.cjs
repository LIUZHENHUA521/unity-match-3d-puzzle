const fs=require('fs'),vm=require('vm'),assert=require('assert');
const source=fs.readFileSync('Assets/Match3D/Plugins/WebGL/PotReviveAds.jslib','utf8');
const flush=()=>new Promise(setImmediate);
function fixture(options={}){
 const logs=[],hooks={},canvasHooks={};
 const logger={info:(tag,line)=>{if(options.failLogging)throw Error("logger unavailable");logs.push(JSON.parse(line));}};
 const messages=[],ads=new Map(),timers=new Map();let serial=0,creates=0;
 const context={GameGlobal:{realtimeLogManager:logger,logmanager:logger},
  canvas:{addEventListener:(name,fn)=>{canvasHooks[name]=fn;}},LibraryManager:{library:{}},mergeInto:Object.assign,UTF8ToString:x=>x,
  GooseAdTelemetry:{prepare:()=>true,complete:()=>{},discard:()=>{}},
  Module:{SendMessage:(target,method,value)=>messages.push({target,...JSON.parse(value)})},
  setTimeout:f=>{timers.set(++serial,f);return serial},clearTimeout:id=>timers.delete(id),Promise,
  wx:{...Object.fromEntries(['onShow','onHide','onMemoryWarning','onError','onUnhandledRejection'].map(name=>[name,fn=>{(hooks[name]??=[]).push(fn);}])),createRewardedVideoAd:({adUnitId})=>{
   creates++;if(ads.has(adUnitId))return ads.get(adUnitId); // Native singleton survives create calls, even after destroy.
   const close=new Set(),error=new Set();
   const ad={close,error,shows:0,loads:0,destroyed:0,mode:'ok',resolveLoad:null,
    onClose:f=>close.add(f),onError:f=>error.add(f),offClose:f=>close.delete(f),offError:f=>error.delete(f),
    destroy(){this.destroyed++},
    load(){this.loads++;if(this.destroyed||this.mode==='load-error')return Promise.reject(new Error('load failed'));
     return this.mode==='pending'?new Promise(r=>this.resolveLoad=r):Promise.resolve()},
    show(){this.shows++;return this.destroyed||this.mode==='show-error'?Promise.reject(new Error('show failed')):Promise.resolve()},
    end:value=>[...close].forEach(f=>f(value)),fail:()=>[...error].forEach(f=>f({errCode:1004}))};
   ads.set(adUnitId,ad);return ad;
  }}};
 vm.createContext(context);vm.runInContext(source,context);context.GooseReviveAds=context.LibraryManager.library.$GooseReviveAds;
 const lib=context.LibraryManager.library;
 return {logs,hooks,canvasHooks,messages,ads,timers,lib,creates:()=>creates,state:context.GooseReviveAds,
  async show(request,id='adunit-test',target='tool'){lib.GooseReviveAdShow(target,id,request,JSON.stringify({owner:'test-owner',impressionId:request,adUnitId:id}));await flush();return ads.get(id)}};
}
function clean(x,ad){assert.equal(x.state.active,null);assert.equal(ad.close.size,0);assert.equal(ad.error.size,0);assert.equal(x.timers.size,0);assert.equal(ad.destroyed,0)}
(async()=>{
 let x=fixture(),ad=await x.show('one');let oldClose=[...ad.close][0],oldError=[...ad.error][0];
 assert.equal(ad.shows,1);ad.end({isEnded:true});oldClose({isEnded:true});oldError();
 assert.equal(x.messages.length,1);assert.equal(x.messages[0].completed,true);clean(x,ad);
 // Regression: second and third watch, including tool -> revival on the shared unit.
 for(const request of ['two','three']){await x.show(request,'adunit-test','revive');oldClose({isEnded:true});oldError();
  assert.equal(x.messages.length,request==='two'?1:2);ad.end({isEnded:true});clean(x,ad)}
 assert.equal(x.creates(),1);assert.equal(ad.shows,3);assert.deepEqual(x.messages.map(m=>m.requestId),['one','two','three']);
 for(const value of [{isEnded:false},undefined]){x=fixture();ad=await x.show('cancelled-watch');ad.end(value);assert.equal(x.messages[0].completed,false);clean(x,ad);await x.show('retry');ad.end({isEnded:true});assert.equal(x.messages[1].completed,true)}
 for(const mode of ['load-error','show-error','event-error']){
  x=fixture();ad=await x.show('warmup');ad.end({isEnded:true});ad.mode=mode;await x.show('failure');if(mode==='event-error')ad.fail();
  assert.equal(x.messages.length,2);assert.equal(x.messages[1].completed,false);clean(x,ad);
  ad.mode='ok';await x.show('retry');ad.end({isEnded:true});assert.equal(x.messages[2].completed,true);assert.equal(x.creates(),1);
 }
 for(const finish of ['timeout','cancel']){
  x=fixture();ad=await x.show('warmup');ad.end({isEnded:true});ad.mode='pending';await x.show('pending');
  const staleClose=[...ad.close][0],staleError=[...ad.error][0],resolve=ad.resolveLoad;
  if(finish==='timeout')[...x.timers.values()][0]();else x.lib.GooseReviveAdCancel('pending');
  clean(x,ad);const count=x.messages.length,shows=ad.shows;
  ad.mode='ok';await x.show('next');resolve();staleClose({isEnded:true});staleError();await flush();
  assert.equal(ad.shows,shows+1);assert.equal(x.messages.length,count);ad.end({isEnded:true});assert.equal(x.messages.length,count+1);clean(x,ad);
 }
 x=fixture();ad=await x.show('active');await x.show('busy');assert.equal(x.messages[0].completed,false);assert.equal(ad.shows,1);
 x.lib.GooseReviveAdCancel('wrong-id');assert(x.state.active);ad.end({isEnded:true});clean(x,ad);
 const other=await x.show('other','adunit-other');other.end({isEnded:true});clean(x,other);await x.show('original');ad.end({isEnded:true});assert.equal(x.creates(),2);
 x=fixture();ad=await x.show('cancel-after-show');x.lib.GooseReviveAdCancel('cancel-after-show');assert(x.state.active);await x.show('blocked');assert.equal(x.messages.length,1);ad.end({isEnded:true});assert.equal(x.messages.length,1);clean(x,ad);
 // Diagnostics distinguish native close, bridge delivery, Unity callback and a resumed frame.
 x=fixture();ad=await x.show('diagnostic');
 x.hooks.onHide[0]();x.hooks.onShow[0]();x.hooks.onMemoryWarning[0]();
 x.canvasHooks.webglcontextlost();x.canvasHooks.webglcontextrestored();
 ad.end({isEnded:true});
 x.lib.PotAdDiagnosticMark('diagnostic','unity_reply','matched');
 x.lib.PotAdDiagnosticMark('diagnostic','unity_next_frame','frame=123');
 for(const event of ['prepared','load_start','load_resolved','show_start','show_resolved','app_hide','app_show','memory_warning','context_lost','context_restored','native_close','finished','unity_dispatch','unity_dispatch_returned','unity_reply','unity_next_frame']){
  assert(x.logs.some(row=>row.event===event&&row.requestId==='diagnostic'),event);
 }
 assert(x.logs.every(row=>!('owner' in row)&&!('token' in row)&&row.elapsedMs>=0));
 const count=x.logs.length;x.lib.PotAdDiagnosticMark('unrelated','unity_reply','');assert.equal(x.logs.length,count);
 await x.show('diagnostic-next');ad.end({isEnded:true});assert(Object.values(x.hooks).every(list=>list.length===1));clean(x,ad);
 // Broken log sinks must not prevent completion or leave a native singleton locked.
 x=fixture({failLogging:true});ad=await x.show('broken-logger');ad.end({isEnded:true});
 assert.equal(x.messages[0].completed,true);clean(x,ad);
 console.log('PASS diagnostic lifecycle correlation; one-time listeners; Unity markers; no credentials; throwing log sinks');
 console.log('PASS repeated watches; shared tool/revive unit; no destroyed singleton reuse; listener cleanup; incomplete/undefined close; duplicate/stale callbacks; load/show/event errors and retry; timeout/cancel followed by new request; busy and separate units');
})().catch(e=>{console.error(e);process.exit(1)});
