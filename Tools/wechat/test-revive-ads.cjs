const fs=require('fs'),vm=require('vm'),assert=require('assert');
const source=fs.readFileSync('Assets/GooseReplica/Plugins/WebGL/GooseReviveAds.jslib','utf8');
async function scenario(mode){
 const messages=[];let close,error,timer,shows=0,destroyed=0,resolveLoad;
 const ad={onClose:f=>close=f,onError:f=>error=f,offClose(){},offError(){},destroy(){destroyed++},load:()=>mode==='load-error'?Promise.reject(new Error()):mode==='pending'?new Promise(r=>resolveLoad=r):Promise.resolve(),show:()=>{shows++;return mode==='show-error'?Promise.reject(new Error()):Promise.resolve()}};
 const context={LibraryManager:{library:{}},mergeInto:Object.assign,UTF8ToString:x=>x,Module:{SendMessage:(t,m,v)=>messages.push(JSON.parse(v))},setTimeout:f=>(timer=f,1),clearTimeout(){},Promise,wx:{createRewardedVideoAd:()=>ad}};
 vm.createContext(context);vm.runInContext(source,context);context.GooseReviveAds=context.LibraryManager.library.$GooseReviveAds;
 const lib=context.LibraryManager.library;lib.GooseReviveAdShow('target','adunit-test','one');await new Promise(setImmediate);
 return {messages,lib,close:()=>close,error:()=>error,timeout:()=>timer(),shows:()=>shows,destroyed:()=>destroyed,resolveLoad:()=>resolveLoad&&resolveLoad()};
}
(async()=>{
 let x=await scenario();assert.equal(x.messages.length,0);assert.equal(x.shows(),1);x.close()({isEnded:false});assert.equal(x.messages[0].completed,false);x.close()({isEnded:true});assert.equal(x.messages.length,1);
 x=await scenario();x.close()({isEnded:true});x.error()();assert.equal(x.messages.length,1);assert.equal(x.messages[0].completed,true);assert.equal(x.destroyed(),1);
 x=await scenario();x.close()(undefined);assert.equal(x.messages[0].completed,false);
 for(const kind of ['load-error','show-error']){x=await scenario(kind);assert.equal(x.messages.length,1);assert.equal(x.messages[0].completed,false);}
 x=await scenario('pending');x.timeout();x.resolveLoad();await new Promise(setImmediate);assert.equal(x.shows(),0);assert.equal(x.messages[0].completed,false);
 x=await scenario('pending');x.lib.GooseReviveAdCancel('one');x.resolveLoad();await new Promise(setImmediate);x.close()({isEnded:true});assert.equal(x.messages.length,0);assert.equal(x.shows(),0);
 console.log('PASS complete; incomplete; undefined close; duplicate callbacks; load/show errors; timeout; cancellation; stale promise');
})().catch(e=>{console.error(e);process.exit(1)});
