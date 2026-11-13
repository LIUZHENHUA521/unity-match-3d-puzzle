// Runs the production native outbox against the production HTTP router with an isolated transactional store.
const assert=require('node:assert/strict'),path=require('node:path'),{pathToFileURL}=require('node:url');
const {fixture,event}=require('./test-ad-telemetry.cjs');
(async()=>{
 const {createService}=await import(pathToFileURL(path.resolve(__dirname,'../../../crazyCombo/server/services/goose-server/src/server.mjs')));
 const rows=new Map();let tail=Promise.resolve();
 const store={transaction(fn){const p=tail.then(async()=>{const snapshot=new Map(structuredClone([...rows]));const result=await fn({get:async(t,id)=>snapshot.get(t+id),set:async(t,id,v)=>snapshot.set(t+id,v)});rows.clear();for(const [k,v]of snapshot)rows.set(k,v);return result;});tail=p.catch(()=>{});return p;}};
 const service=createService({invites:{storeFactory:async()=>store,authenticate:async code=>code}});
 await new Promise(resolve=>service.server.listen(0,'127.0.0.1',resolve));const base='http://127.0.0.1:'+service.server.address().port;
 try{
  const user=await (await fetch(base+'/invites/login',{method:'POST',body:JSON.stringify({code:'telemetry-fixture-only'})})).json();
  let x=fixture();x.bind(user.inviteCode,user.token);x.q.session.endpoint=base;
  const ad=await x.show(event('c'.repeat(32),user.inviteCode));ad.end({isEnded:true});
  const deliver=async request=>{const r=await fetch(request.url,{method:'POST',headers:request.header,body:JSON.stringify(request.data)});return {statusCode:r.status,data:await r.json()};};
  const committed=await deliver(x.requests[0]);assert.equal(committed.statusCode,200);assert.equal(committed.data.ok,true);
  // Kill the client before it receives the successful ACK, then replay its disk cache in a fresh VM.
  const y=fixture(x.storage);y.bind(user.inviteCode,user.token);y.q.session.endpoint=base;
  // Bind attempted delivery immediately; point that captured request at the same local fixture.
  y.requests[0].url=base+'/ads/events';const replay=await deliver(y.requests[0]);y.requests[0].success(replay);
  assert.equal(y.q.items.length,0);assert.equal([...rows.keys()].filter(k=>k.startsWith('ad_events')).length,1);
  assert.equal([...rows.keys()].filter(k=>k.startsWith('game_records')).length,0);
  const stored=[...rows.values()].find(v=>v.eventType==='completed');assert.equal(stored.userId,user.inviteCode);assert.equal(stored.placement,'tool_magnet');
  const changed={...x.requests[0],data:{...x.requests[0].data,placement:'tool_remove'}};assert.equal((await deliver(changed)).statusCode,409);
  console.log('PASS production JS -> HTTP -> transaction: completion without settlement; terminate before ACK; restart/replay exactly once; conflicting replay denied');
 }finally{await service.close();}
})().catch(e=>{console.error(e);process.exitCode=1});
