import {createServer} from 'node:http';
// Deterministic client fixture; production route behavior is covered by server tests.
let reads=0;
createServer(async(req,res)=>{
 res.setHeader('Content-Type','application/json');
 if(req.url!=='/pot-editor/beta-level'||req.method!=='POST'){res.writeHead(404);res.end('{}');return;}
 if(req.headers['x-pot-editor-key']!=='fixture-preview-key'){res.writeHead(401);res.end(JSON.stringify({error:'预览凭据无效'}));return;}
 let body='';for await(const chunk of req)body+=chunk;
 const {tier}=JSON.parse(body);reads++;
 const count=reads===1?30:12;
 res.end(JSON.stringify({ok:true,level:{date:'2026-11-17',sourceDate:'2026-11-17',tier,version:'beta-'+reads,generatorVersion:'daily-v1',modelThemeId:'tasty-kitchen',seed:15937,seconds:90,initialCount:count,limitHeight:7.5,layers:[{count,types:4,height:8}]}}));
}).listen(18097,'127.0.0.1',()=>console.log('READY'));
