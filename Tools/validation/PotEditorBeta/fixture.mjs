import {pathToFileURL} from 'node:url';
const {createService}=await import(pathToFileURL(process.argv[2]).href);
let reads=0;
const service=createService({editorBeta:{
 key:()=> 'fixture-preview-key',
 storeFactory:async()=>({transaction:fn=>fn({get:async()=>{
  reads++;return {version:'beta-'+reads,config:{modelThemeId:'tasty-kitchen',seconds:90,layers:[{count:reads===1?30:12,types:4,height:8}]}};
 }})})
}});
service.server.listen(18097,'127.0.0.1',()=>console.log('READY'));
