const test=require('node:test');
const assert=require('node:assert/strict');
const fs=require('node:fs');
const vm=require('node:vm');
const path=require('node:path');
const source=fs.readFileSync(path.join(__dirname,'../../Assets/Match3D/Plugins/WebGL/PotBenefits.jslib'),'utf8');
function read(wx){
 const messages=[],lib={};
 const ctx={wx,LibraryManager:{library:lib},mergeInto:Object.assign,UTF8ToString:x=>x,
  console:{warn(){}},Module:{SendMessage:(target,method,json)=>messages.push(JSON.parse(json))}};
 vm.runInNewContext(source,ctx);ctx.PotBenefitsTarget=lib.$PotBenefitsTarget;
 lib.PotReadBenefits('Home',1,9);
 return messages;
}
test('wait for privacy consent before reading membership and daily metrics',()=>{
 let consent,request;
 const messages=read({requirePrivacyAuthorize:o=>{consent=o;},getGameClubData:o=>{request=o;}});
 assert.equal(request,undefined);assert.equal(messages.length,0);
 consent.success();assert.deepEqual(Array.from(request.dataTypeList,x=>x.type),[1,4,5,6]);
 request.success({encryptedData:'proof',iv:'iv'});
 assert.deepEqual(messages,[{requestId:9,encryptedData:'proof',iv:'iv'}]);
});
test('return a distinct failure for denial, missing API, empty evidence and read failure',()=>{
 for(const [wx,error] of [
  [{},'unsupported'],
  [{getGameClubData:o=>o.fail({errMsg:'getGameClubData:fail auth deny'})},'denied'],
  [{getGameClubData:o=>o.success({})},'empty'],
  [{getGameClubData:o=>o.fail({errMsg:'network error'})},'getGameClubData'],
  [{requirePrivacyAuthorize:o=>o.fail({errMsg:'privacy denied'}),getGameClubData:()=>assert.fail()},'privacy']
 ])assert.deepEqual(read(wx),[{requestId:9,error}]);
});
