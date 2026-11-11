// Disposable localhost fixture using the actual production service and an in-memory transaction store.
import http from 'node:http';
import fs from 'node:fs';
import {Invitations} from '../../../../crazyCombo/goose-server/src/invitations.mjs';
import {SignIn} from '../../../../crazyCombo/goose-server/src/sign-in.mjs';
const output=process.argv[2];let tail=Promise.resolve();const data=new Map();
const store={transaction(fn){const run=tail.then(async()=>{const copy=new Map(structuredClone([...data]));const result=await fn({get:async(t,id)=>copy.get(t+id),set:async(t,id,v)=>copy.set(t+id,structuredClone(v))});data.clear();for(const entry of copy)data.set(...entry);return structuredClone(result);});tail=run.catch(()=>{});return run;}};
let now=Date.parse('2026-11-11T04:00:00Z'),drop=0,reject=false,unauthorized=false;
const api=new Invitations(store,async x=>x),signIn=new SignIn(api,()=>now),requests=[];
const server=http.createServer(async(req,res)=>{
 try{
  let raw='';for await(const chunk of req)raw+=chunk;const body=JSON.parse(raw||'{}');
  const token=(req.headers.authorization||'').replace(/^Bearer /,'');let reply;
  requests.push({path:req.url,body});fs.writeFileSync(output+'/requests.json',JSON.stringify(requests,null,2));
  if(req.url==='/fixture'){
   now+=(body.advanceDays||0)*86400000;if(body.drop!==undefined)drop=body.drop;if(body.reject!==undefined)reject=body.reject;if(body.unauthorized!==undefined)unauthorized=body.unauthorized;
   reply={ok:true};
  }else if(req.url==='/invites/login')reply=await api.login(body.code);
  else if(req.url==='/invites/mail/list')reply={...await api.state(token),mails:[],attention:false,nextCursor:-1};
  else if(req.url.startsWith('/invites/sign-in/')){
   if(unauthorized){unauthorized=false;res.writeHead(401);res.end(JSON.stringify({error:'登录已失效，请重新登录'}));return;}
   if(reject){res.writeHead(503);res.end(JSON.stringify({error:'fixture unavailable'}));return;}
   reply=await signIn.request(token,req.url.split('/').at(-1),body);
   if(req.url.endsWith('/claim')&&drop>0){drop--;req.socket.destroy();return;}
  }else if(req.url==='/invites/state')reply=await api.state(token);
  else if(['/invites/consume','/invites/refund'].includes(req.url))reply=await api.consume(token,body.requestId,body.type,req.url.endsWith('refund'));
  else {res.writeHead(404);res.end('{}');return;}
  res.setHeader('Content-Type','application/json');res.end(JSON.stringify(reply));
 }catch(e){res.writeHead(e.status||500);res.end(JSON.stringify({error:e.message}));}
});
server.listen(0,'127.0.0.1',()=>fs.writeFileSync(output+'/endpoint.txt','http://127.0.0.1:'+server.address().port));
