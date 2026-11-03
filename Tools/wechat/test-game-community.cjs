const {test}=require('node:test');
const assert=require('node:assert/strict');
const vm=require('node:vm');
const fs=require('node:fs');
const path=require('node:path');
function setup(wx,window){
 const library={},context={LibraryManager:{library},mergeInto:Object.assign,UTF8ToString:x=>x,wx,window,console:{warn(){}}};
 vm.createContext(context);
 vm.runInContext(fs.readFileSync(path.join(__dirname,'../../Assets/GooseReplica/Plugins/WebGL/GooseGameCommunity.jslib'),'utf8'),context);
 context.GooseGameCommunity=library.$GooseGameCommunity;
 return library.GooseGameCommunityOpen;
}
const flush=()=>new Promise(resolve=>setImmediate(resolve));
test('load completes before show; duplicate taps suppressed; instance reused then recreated after destroy',async()=>{
 let release,closed,created=0;const calls=[];
 const open=setup({createPageManager(){created++;return {on(event,cb){assert.equal(event,'destroy');closed=cb;},load(o){calls.push(o.openlink);return new Promise(r=>release=r);},show(){calls.push('show');return Promise.resolve();}};}});
 open('target-circle');open('target-circle');assert.deepEqual(calls,['target-circle']);
 release();await flush();assert.deepEqual(calls,['target-circle','show']);
 open('target-circle');release();await flush();assert.equal(created,1);
 closed();open('target-circle');release();await flush();assert.equal(created,2);
});
test('load/show rejection and synchronous errors report failure and permit retry',async()=>{
 for(const phase of ['create','load','show']){
  let broken=true,shown=0,errors=0;
  const open=setup({showToast(){errors++;},createPageManager(){
   if(broken&&phase==='create')throw Error('create');
   return {on(){},destroy(){},load(){return broken&&phase==='load'?Promise.reject(Error('load')):Promise.resolve();},show(){if(broken&&phase==='show')return Promise.reject(Error('show'));shown++;return Promise.resolve();}};
  }});
  open('target');await flush();assert.equal(errors,1);assert.equal(shown,0);
  broken=false;open('target');await flush();assert.equal(shown,1);
 }
});
test('unsupported platform/API and missing link have visible guidance',()=>{
 let message;
 setup(undefined,{alert(s){message=s;}})('target');assert.match(message,/微信小游戏/);
 setup({showToast(o){message=o.title;}})('target');assert.match(message,/升级微信/);
 setup({createPageManager(){assert.fail('must not create without a target');},showToast(o){message=o.title;}})('');assert.match(message,/暂未配置/);
});
test('late completion of a destroyed page cannot show it or unlock a newer request',async()=>{
 const pages=[];
 const open=setup({createPageManager(){const page={on(e,cb){this.closed=cb;},load(){return new Promise(r=>this.resolve=r);},show(){assert.fail('destroyed or pending page must not show');}};pages.push(page);return page;}});
 open('target');pages[0].closed();open('target');pages[0].resolve();await flush();
 open('target');assert.equal(pages.length,2);
});
