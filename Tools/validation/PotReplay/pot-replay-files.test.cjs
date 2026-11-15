const {test}=require('node:test');
const assert=require('node:assert/strict');
const fs=require('node:fs'),os=require('node:os'),path=require('node:path'),vm=require('node:vm');
const source=fs.readFileSync(path.resolve(__dirname,'../../../Assets/Match3D/Plugins/WebGL/PotReplayFiles.jslib'),'utf8');
function setup(){
 const root=fs.mkdtempSync(path.join(os.tmpdir(),'pot-files-'));
 const native={accessSync:fs.accessSync,mkdirSync:p=>fs.mkdirSync(p,{recursive:true}),readdirSync:fs.readdirSync,writeFileSync:fs.writeFileSync,readFileSync:fs.readFileSync,statSync:fs.statSync,
 openSync:o=>String(fs.openSync(o.filePath,o.flag)),closeSync:o=>fs.closeSync(Number(o.fd)),
 readSync:o=>({arrayBuffer:o.arrayBuffer,bytesRead:fs.readSync(Number(o.fd),new Uint8Array(o.arrayBuffer),o.offset,o.length,o.position)}),
 writeSync:o=>({bytesWritten:fs.writeSync(Number(o.fd),new Uint8Array(o.data),o.offset,o.length,o.position)})};
 const c={wx:{env:{USER_DATA_PATH:root},getFileSystemManager:()=>native},LibraryManager:{library:{}},mergeInto:Object.assign,UTF8ToString:x=>x,lengthBytesUTF8:x=>Buffer.byteLength(x),_malloc:()=>1,stringToUTF8:s=>{c.result=s}};
 vm.createContext(c);vm.runInContext(source,c);c.PotReplayBase64=c.LibraryManager.library.$PotReplayBase64;
 const call=r=>{c.LibraryManager.library.PotReplayFile(JSON.stringify(r));const result=JSON.parse(c.result);assert.equal(result.ok,true,result.error);return result};
 call({action:'mkdir'});return {c,call,cleanup:()=>fs.rmSync(root,{recursive:true,force:true})};
}
test('native replay bridge round trips binary chunks without wx base64 APIs',()=>{
 const s=setup();try{for(const length of [1,2,3,255,32767,32768]){
 const data=Buffer.alloc(length);for(let i=0;i<length;i++)data[i]=(i*73+19)%256;
 const fd=s.call({action:'open',path:'chunk.bin',target:'w'}).text;
 assert.equal(s.call({action:'writeBytes',fd,position:0,text:data.toString('base64')}).size,length);s.call({action:'close',fd});
 assert.equal(s.call({action:'size',path:'chunk.bin'}).size,length);
 const readFd=s.call({action:'open',path:'chunk.bin',target:'r'}).text;
 assert.equal(s.call({action:'readBytes',fd:readFd,position:0,length}).text,data.toString('base64'));
 if(length>1)assert.equal(s.call({action:'readBytes',fd:readFd,position:1,length}).text,data.subarray(1).toString('base64'));
 s.call({action:'close',fd:readFd});
 }}finally{s.cleanup()}
});
test('invalid base64 is rejected before a file write',()=>{const s=setup();try{for(const x of ['a','!!!!','abc===','a=aa'])assert.throws(()=>s.c.PotReplayBase64.decode(x),/invalid replay base64/)}finally{s.cleanup()}});
