const fs=require('fs'),vm=require('vm'),assert=require('assert');
const source=fs.readFileSync('Assets/Match3D/Plugins/WebGL/GooseRewardShare.jslib','utf8');
function fixture(mode){
 const messages=[];let now=1000,hide,show,timer,options,removed=0;
 const wx={onHide:f=>hide=f,onShow:f=>show=f,offHide:f=>{assert.equal(f,hide);removed++;},offShow:f=>{assert.equal(f,show);removed++;},shareAppMessage:o=>{options=o;if(mode==='throw')throw Error('fail');}};
 const context={LibraryManager:{library:{}},mergeInto:Object.assign,UTF8ToString:x=>x,Module:{SendMessage:(t,m,v)=>messages.push(JSON.parse(v))},setTimeout:f=>(timer=f,1),clearTimeout(){},Date:{now:()=>now},Math,wx:mode==='unsupported'?undefined:wx};
 vm.createContext(context);vm.runInContext(source,context);context.GooseRewardSharing=context.LibraryManager.library.$GooseRewardSharing;
 const lib=context.LibraryManager.library;lib.GooseRewardShare('target','one',3);
 return {messages,lib,hide:()=>hide(),show:()=>show(),advance:ms=>now+=ms,timeout:()=>timer(),fail:()=>options.fail(),removed:()=>removed};
}
let x=fixture();assert.equal(x.messages.length,0);x.hide();x.advance(3000);x.show();x.show();assert.equal(x.messages.length,1);assert.equal(x.messages[0].completed,true);assert.equal(x.removed(),2);
x=fixture();x.show();assert.equal(x.messages[0].completed,false);
x=fixture();x.hide();x.advance(2999);x.show();assert.equal(x.messages[0].completed,false);
x=fixture();x.hide();x.lib.GooseRewardShareCancel('one');x.advance(4000);x.show();x.timeout();assert.equal(x.messages.length,0);assert.equal(x.removed(),2);
x=fixture();x.fail();x.hide();x.advance(4000);x.show();assert.equal(x.messages.length,1);assert.equal(x.messages[0].completed,false);
x=fixture();x.timeout();assert.equal(x.messages[0].completed,false);
for(const mode of ['unsupported','throw']){x=fixture(mode);assert.equal(x.messages[0].completed,false);}
x=fixture();x.lib.GooseRewardShare('target','two',3);assert.equal(x.messages[0].requestId,'two');assert.equal(x.messages[0].completed,false);x.hide();x.advance(4000);x.show();assert.equal(x.messages[1].requestId,'one');assert.equal(x.messages[1].completed,true);
console.log('PASS share return threshold, no hide, fast return, cancellation, duplicate callbacks, failure, timeout, unsupported, exception, concurrent requests and listener cleanup');
