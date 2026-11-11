// Loopback-only Unity validation. No CloudBase or WeChat calls and no payments.
const http=require('node:http');
const prepared=new Set();
http.createServer((req,res)=>{
 const token=String(req.headers.authorization||'').replace('Bearer ','');
 const status=req.url==='/rewards/authorization/status',prepare=req.url==='/rewards/authorization/prepare';
 res.setHeader('Content-Type','application/json');
 if(!status&&!prepare){res.writeHead(404);res.end('{}');return;}
 const send=()=>{
  if(token==='status-error'){res.writeHead(503);res.end(JSON.stringify({error:'模拟网络失败'}));return;}
  const authorized=token==='authorized'||(status&&token==='success'&&prepared.has(token));
  if(prepare)prepared.add(token);
  res.end(JSON.stringify({ok:true,authorized,state:authorized?'TAKING_EFFECT':'WAIT_USER_CONFIRM',appid:'wx74f2d8722c02a6f4',mchid:'1234567890',...(prepare?{packageInfo:'fixture-package'}:{})}));
 };
 setTimeout(send,(token==='delayed'||(token==='slow-prepare'&&prepare))?500:10);
}).listen(18766,'127.0.0.1',()=>console.log('Local authorization fixture on 18766'));
