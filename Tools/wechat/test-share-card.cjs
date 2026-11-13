const {test}=require('node:test');
const assert=require('node:assert/strict');
const fs=require('node:fs');
const path=require('node:path');
const vm=require('node:vm');

const titles=[
 '这一锅不拼手速，拼的是眼力和取舍，敢来试试吗？',
 '名字叫一锅消不完，你能替我争口气吗？😂',
 '看着挺简单，一消就上头！这一锅你能消完吗？'
];
const imageUrl='https://mmocgame.qpic.cn/wechatgame/OwVPicIFUBQ9L8hmSjicNSfv0sW7zyTmEfPZ25bUTUMJMAksojPuD9ViaOhoWZRawC4/0';

function fixture(){
 const library={},calls=[],messages=[];
 let random=0;
 const math=Object.create(Math);math.random=()=>random;
 const context={
  LibraryManager:{library},mergeInto:Object.assign,UTF8ToString:value=>value,Math:math,
  Module:{SendMessage:(target,method,json)=>messages.push(JSON.parse(json))},
  setTimeout:()=>1,clearTimeout(){},
  wx:{shareAppMessage:options=>calls.push(options),onHide(){},onShow(){},offHide(){},offShow(){}}
 };
 vm.createContext(context);
 for(const name of ['PotShareCard','PotInvites','PotRewardShare']){
  vm.runInContext(fs.readFileSync(path.join(__dirname,'../../Assets/Match3D/Plugins/WebGL',name+'.jslib'),'utf8'),context);
 }
 // Resolve only declared dependencies, as the WebGL linker does.
 function bind(symbol){
  for(const dependency of library[symbol+'__deps']||[]){
   assert.ok(dependency in library,'Missing WebGL dependency '+dependency);
   bind(dependency);
   context[dependency.slice(1)]=library[dependency];
  }
 }
 function call(symbol,...args){bind(symbol);return library[symbol](...args);}
 return {calls,messages,call,setRandom:value=>random=value};
}

for(const entry of ['invite','reward','result']){
 test(entry+' reselects all three titles with the approved image and original query',()=>{
  const f=fixture();
  // Include both sides of the interval boundaries and repeated selections.
  for(const [random,index] of [[0,0],[1/3-Number.EPSILON,0],[1/3,1],[2/3-Number.EPSILON,1],[2/3,2],[0.999999,2],[0,0]]){
   f.setRandom(random);
   let query;
   if(entry==='invite'){
    query='inviter=2222222222222222';f.call('GooseInvitesShare','2222222222222222');
   }else if(entry==='reward'){
    query='source=reward';f.call('GooseRewardShare','target','request',3);
   }else{
    query='dailyDate=2026-11-12&dailyTier=bachelor';f.call('GooseShareResult',query);
   }
   const card=f.calls.at(-1);
   assert.equal(card.title,titles[index]);
   assert.equal(card.imageUrl,imageUrl);
   assert.equal(card.imageUrlId,'Mg3EGffMTCuIXpg24LrIIw==');
   assert.equal(card.query,query);
   if(entry==='reward')f.call('GooseRewardShareCancel','request');
  }
  assert.equal(f.calls.length,7);
  assert.equal(f.messages.length,0,'Sharing alone must not fabricate reward callbacks');
 });
}

test('invitation failure still reaches the Unity callback',()=>{
 const f=fixture();f.call('GooseInvitesShare','2222222222222222');
 f.calls[0].fail();assert.equal(f.messages[0].error,'未能打开分享，请重试');
});

test('non-daily result shares without a daily query',()=>{
 const f=fixture();f.call('GooseShareResult','');assert.equal(f.calls[0].query,'');
});
