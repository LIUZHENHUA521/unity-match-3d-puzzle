const fs=require('fs'),vm=require('vm'),assert=require('node:assert/strict');
const telemetry=fs.readFileSync('Assets/Match3D/Plugins/WebGL/GooseAdTelemetry.jslib','utf8');
const adsSource=fs.readFileSync('Assets/Match3D/Plugins/WebGL/GooseReviveAds.jslib','utf8');
const tick=()=>new Promise(setImmediate);
const OWNER='a'.repeat(64),OTHER='b'.repeat(64);
const event=(id='1'.repeat(32),owner=OWNER)=>({schemaVersion:1,eventType:'completed',owner,impressionId:id,recordId:'2'.repeat(32),runId:'3'.repeat(32),stage:2,placement:'tool_magnet',adUnitId:'adunit-test',clientVersion:'0.1',platform:'ios',occurredAt:0});
function fixture(storage=new Map()){
 const requests=[],messages=[],timers=new Map(),hooks={},ads=[];let serial=0,failWrite=false,failRead=false;
 const context={LibraryManager:{library:{}},mergeInto:Object.assign,UTF8ToString:x=>x,console:{warn:()=>{}},Date,Math,Promise,
  setTimeout:f=>{timers.set(++serial,f);return serial},clearTimeout:id=>timers.delete(id),
  Module:{SendMessage:(target,method,data)=>messages.push({target,method,data})},
  wx:{getStorageSync:k=>{if(failRead)throw Error('read');return storage.get(k)||''},setStorageSync:(k,v)=>{if(failWrite)throw Error('full');storage.set(k,v)},
   onShow:f=>hooks.show=f,onNetworkStatusChange:f=>hooks.network=f,request:r=>requests.push(r),
   createRewardedVideoAd:()=>{const closes=new Set(),errors=new Set();let ad={onClose:f=>closes.add(f),offClose:f=>closes.delete(f),onError:f=>errors.add(f),offError:f=>errors.delete(f),load:()=>Promise.resolve(),show:()=>Promise.resolve(),end:x=>[...closes].forEach(f=>f(x))};ads.push(ad);return ad;}}};
 vm.createContext(context);vm.runInContext(telemetry,context);context.GooseAdTelemetry=context.LibraryManager.library.$GooseAdTelemetry;
 vm.runInContext(adsSource,context);context.GooseReviveAds=context.LibraryManager.library.$GooseReviveAds;
 const q=context.GooseAdTelemetry;
 return {q,requests,messages,timers,hooks,ads,storage,context,setWriteFailure:v=>failWrite=v,setReadFailure:v=>failRead=v,
  bind:(owner=OWNER,token='token')=>q.bind({owner,token,endpoint:'https://game.test',target:'telemetry'}),
  retry:()=>{const a=[...timers.entries()];for(const [id,f]of a){timers.delete(id);f();}},
  ack:(i=0)=>requests[i].success({statusCode:200,data:{ok:true,eventType:'completed',impressionId:requests[i].data.impressionId}}),
  show:async(e=event())=>{context.LibraryManager.library.GooseReviveAdShow('view',e.adUnitId,e.impressionId,JSON.stringify(e));await tick();return ads.at(-1);},
  cancel:id=>context.LibraryManager.library.GooseReviveAdCancel(id)};
}
async function run(){
 // A completed native receipt is durable BEFORE Unity receives its callback; no settlement involved.
 let x=fixture();x.bind();let ad=await x.show();ad.end({isEnded:true});
 assert.equal(x.q.items[0].state,'completed');assert.equal(JSON.parse(x.storage.get(x.q.key)).items[0].state,'completed');
 assert.equal(x.requests.length,1);assert.equal(x.messages.filter(m=>m.method==='OnReviveAdReply').length,1);
 assert.equal(x.requests[0].data.placement,'tool_magnet');assert(x.requests[0].data.occurredAt>0);
 assert(!x.storage.get(x.q.key).includes('token'));
 // Process termination before ACK, offline retry and response lost all replay the exact frozen receipt.
 let persisted=x.storage;let y=fixture(persisted);y.bind();assert.equal(y.requests.length,1);assert.deepEqual(JSON.parse(JSON.stringify(y.requests[0].data)),JSON.parse(JSON.stringify(x.requests[0].data)));
 y.requests[0].fail();y.retry();assert.equal(y.requests.length,2);y.ack(1);assert.equal(y.q.items.length,0);y.requests[1].fail();assert.equal(y.q.items.length,0);
 // Prepared but not completed is never inferred as a view on restart.
 x=fixture();x.bind();assert(x.q.prepare(event()));y=fixture(x.storage);y.bind();assert.equal(y.q.items.length,0);assert.equal(y.requests.length,0);
 // Cancel after display only removes the reward callback; telemetry remains active and locks playback.
 x=fixture();x.bind();ad=await x.show();x.cancel(event().impressionId);await x.show(event('4'.repeat(32)));assert(x.context.GooseReviveAds.active);ad.end({isEnded:true});
 assert.equal(x.requests.length,1);assert.equal(x.q.items[0].event.impressionId,event().impressionId);assert.equal(x.messages.filter(m=>m.method==='OnReviveAdReply'&&JSON.parse(m.data).completed).length,0);
 ad.end({isEnded:true});assert.equal(x.requests.length,1);
 // Incomplete close and errors do not count.
 for(const close of [{isEnded:false},undefined]){x=fixture();x.bind();ad=await x.show();ad.end(close);assert.equal(x.q.items.length,0);assert.equal(x.requests.length,0);}
 // Never rebind old records to the new account; late ACK deletes only its original receipt.
 x=fixture();x.bind();x.q.prepare(event());x.q.complete(OWNER,event().impressionId);x.bind(OTHER,'other-token');assert.equal(x.requests.length,1);
 x.q.prepare(event('4'.repeat(32),OTHER));x.q.complete(OTHER,'4'.repeat(32));x.ack();x.retry();assert.equal(x.requests.length,2);assert.equal(x.requests[1].data.owner,OTHER);assert.equal(x.requests[1].header.Authorization,'Bearer other-token');x.ack(1);assert.equal(x.q.items.length,0);
 // An obsolete 401 cannot log out another account; same-session 401 requests refresh.
 x=fixture();x.bind();x.q.prepare(event());x.q.complete(OWNER,event().impressionId);x.bind(OTHER,'new');x.requests[0].success({statusCode:401});assert.equal(x.q.session.token,'new');assert.equal(x.messages.length,0);
 x.bind();assert.equal(x.requests.length,2);x.requests[1].success({statusCode:401});assert.equal(x.q.session.token,'');assert(x.messages.some(m=>m.method==='OnAdTelemetryAuthExpired'));x.bind(OWNER,'fresh');assert.equal(x.requests[2].header.Authorization,'Bearer fresh');
 // Quarantined bad payload does not block valid payloads and remains preserved.
 x=fixture();x.bind();x.q.prepare(event());x.q.complete(OWNER,event().impressionId);x.q.prepare(event('4'.repeat(32)));x.q.complete(OWNER,'4'.repeat(32));x.requests[0].success({statusCode:409});x.retry();assert.equal(x.q.items[0].quarantined,true);assert.equal(x.requests[1].data.impressionId,'4'.repeat(32));x.ack(1);assert.equal(x.q.items.length,1);
 // Network/server/429 and malformed or mismatched ACK never delete an event.
 for(const r of [{statusCode:500},{statusCode:429},{statusCode:200,data:{ok:true,impressionId:'wrong',eventType:'completed'}},{statusCode:200,data:'invalid'}]){x=fixture();x.bind();x.q.prepare(event());x.q.complete(OWNER,event().impressionId);x.requests[0].success(r);assert.equal(x.q.items.length,1);x.retry();assert.equal(x.requests.length,2);}
 // Storage full blocks new playback; failure at completion retains memory and tries network.
 x=fixture();x.bind();x.setWriteFailure(true);await x.show();assert.equal(x.ads.length,0);assert.equal(x.q.items.length,0);
 x=fixture();x.bind();ad=await x.show();x.setWriteFailure(true);ad.end({isEnded:true});assert.equal(x.q.items[0].state,'completed');assert.equal(x.requests.length,1);x.ack();assert.equal(x.q.items.length,1);x.setWriteFailure(false);x.retry();x.ack(1);assert.equal(x.q.items.length,0);
 // Corruption is preserved instead of overwritten by an empty queue.
 x=fixture(new Map([['goose.ads.outbox.v1','broken']]));x.bind();assert(x.q.blocked);assert(!x.q.prepare(event()));assert.equal(x.storage.get(x.q.key),'broken');
 // Invalid native platform/context must fail BEFORE the player watches, not get rejected after completion.
 for(const change of [{platform:'WebGLPlayer'},{recordId:''},{owner:OTHER},{placement:'unknown'}]){x=fixture();x.bind();await x.show({...event(),...change});assert.equal(x.ads.length,0);assert.equal(x.requests.length,0);}
 // Resume and reconnect promptly retry; duplicate complete does not alter its timestamp or send twice.
 x=fixture();x.bind();x.q.prepare(event());x.q.complete(OWNER,event().impressionId);const at=x.q.items[0].event.occurredAt;x.q.complete(OWNER,event().impressionId);assert.equal(x.q.items[0].event.occurredAt,at);assert.equal(x.requests.length,1);x.requests[0].fail();x.hooks.network({isConnected:true});assert.equal(x.requests.length,2);
 console.log('PASS native completion durable before callback; exit/restart; offline/lost ACK; prepared recovery; cancel after show; incomplete; duplicate; owner isolation; stale/auth 401; quarantine; storage failure; corrupt cache; reconnect');
}
module.exports={fixture,event};
if(require.main===module)run().catch(e=>{console.error(e);process.exitCode=1});
