const {test}=require('node:test');
const assert=require('node:assert/strict');
const vm=require('node:vm');
const fs=require('node:fs');
const path=require('node:path');
function setup(wx){
 const library={},events=[];
 vm.runInNewContext(fs.readFileSync(path.join(__dirname,'../../Assets/Match3D/Plugins/WebGL/GooseMerchantAuthorization.jslib'),'utf8'),{LibraryManager:{library},mergeInto:Object.assign,wx,UTF8ToString:x=>x,Module:{SendMessage:(target,method,json)=>events.push({target,method,...JSON.parse(json)})}});
 return {library,events,open:(patch={})=>library.GooseMerchantAuthorizationOpen('Game',JSON.stringify({requestId:'request1',appid:'wx-game',mchid:'1234567890',packageInfo:'package',...patch}))};
}
function api(requestMerchantTransfer){return {getAccountInfoSync:()=>({miniProgram:{appId:'wx-game'}}),requestMerchantTransfer};}
test('calls official API with exact keys and reports returned, never authorized',()=>{
 let options;const s=setup(api(o=>options=o));assert.equal(s.library.GooseMerchantAuthorizationSupported(),1);s.open();
 assert.equal(options.appId,'wx-game');assert.equal(options.mchId,'1234567890');assert.equal(options.package,'package');assert.equal(options.appid,undefined);
 options.success();options.fail({errMsg:'late error'});assert.equal(s.events.length,1);assert.equal(s.events[0].kind,'returned');assert.equal(s.events[0].authorized,undefined);assert.equal(s.events[0].requestId,'request1');
});
test('cancel is distinguishable from failure and callbacks are delivered once',()=>{
 for(const cancel of [o=>o.fail({errMsg:'requestMerchantTransfer:fail cancel'}),o=>o.cancel()]){
  const s=setup(api(o=>{cancel(o);o.success();}));s.open();assert.equal(s.events.length,1);assert.equal(s.events[0].kind,'cancelled');
 }
});
test('unsupported runtime returns guidance without calling other APIs',()=>{
 for(const wx of [undefined,{}]){const s=setup(wx);assert.equal(s.library.GooseMerchantAuthorizationSupported(),0);s.open();assert.equal(s.events[0].kind,'unsupported');}
});
test('wrong game AppID and incomplete payload never launch authorization',()=>{
 const s=setup(api(()=>assert.fail('must not launch')));
 for(const patch of [{appid:'wx-other'},{packageInfo:''},{mchid:''}]){s.open(patch);assert.equal(s.events.at(-1).kind,'failed');}
});
test('native errors are handled without leaking provider details',()=>{
 const s=setup(api(()=>{throw Error('private error');}));s.open();assert.equal(s.events[0].kind,'failed');assert.equal(JSON.stringify(s.events).includes('private'),false);
});
