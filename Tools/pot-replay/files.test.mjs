import {test} from 'node:test';import assert from 'node:assert/strict';import fs from 'node:fs';import vm from 'node:vm';
test('WeChat replay journal survives bridge calls and rejects unsafe filenames',()=>{
 const files=new Map(),dirs=new Set(),lib={},native={accessSync:p=>{if(!dirs.has(p)&&!files.has(p))throw Error('missing')},mkdirSync:p=>dirs.add(p),readdirSync:p=>[...files.keys()].filter(k=>k.startsWith(p+'/')).map(k=>k.slice(p.length+1)),writeFileSync:(p,s)=>files.set(p,s),appendFileSync:(p,s)=>{if(!files.has(p))throw Error('missing');files.set(p,files.get(p)+s)},readFileSync:p=>files.get(p),unlinkSync:p=>files.delete(p),renameSync:(p,t)=>{files.set(t,files.get(p));files.delete(p)},statSync:p=>({size:Buffer.byteLength(files.get(p))})};let output;
 const context={LibraryManager:{library:lib},mergeInto:Object.assign,wx:{env:{USER_DATA_PATH:'/native'},getFileSystemManager:()=>native},UTF8ToString:x=>x,lengthBytesUTF8:s=>Buffer.byteLength(s),_malloc:()=>1,stringToUTF8:s=>{output=JSON.parse(s)}};
 vm.runInNewContext(fs.readFileSync(new URL('../../Assets/Match3D/Plugins/WebGL/PotReplayFiles.jslib',import.meta.url),'utf8'),context);
 const call=r=>{lib.PotReplayFile(JSON.stringify(r));return output};assert.equal(call({action:'mkdir'}).ok,true);call({action:'write',path:'/unity/a.recording',text:'初盘\n'});call({action:'append',path:'/unity/a.recording',text:'帧\n'});assert.equal(call({action:'read',path:'a.recording'}).text,'初盘\n帧\n');assert.equal(call({action:'size',path:'a.recording'}).size,11);call({action:'move',path:'a.recording',target:'a.pending'});assert.equal(call({action:'exists',path:'a.recording'}).exists,false);assert.equal(call({action:'exists',path:'a.pending'}).exists,true);assert.equal(call({action:'write',path:'unsafe?name',text:'x'}).ok,false);call({action:'delete',path:'a.pending'});assert.equal(files.size,0);
});

test('native replay streams use bounded binary reads and writes across UTF-8 boundaries',()=>{
 const lib={},contents=new Map(),handles=new Map();let next=0,output,maxRead=0;
 const native={statSync:p=>({size:contents.get(p).length}),openSync:o=>{const fd=String(++next);handles.set(fd,o.filePath);if(o.flag==='w')contents.set(o.filePath,Buffer.alloc(0));return fd;},closeSync:o=>handles.delete(o.fd),
  readSync:o=>{maxRead=Math.max(maxRead,o.length);const source=contents.get(handles.get(o.fd)),count=Math.min(o.length,source.length-o.position);new Uint8Array(o.arrayBuffer).set(source.subarray(o.position,o.position+count));return{arrayBuffer:o.arrayBuffer,bytesRead:count};},
  writeSync:o=>{const path=handles.get(o.fd),old=contents.get(path),next=Buffer.alloc(Math.max(old.length,o.position+o.length));old.copy(next);Buffer.from(o.data).copy(next,o.position,o.offset,o.offset+o.length);contents.set(path,next);return{bytesWritten:o.length};}
 };
 const context={LibraryManager:{library:lib},mergeInto:Object.assign,wx:{env:{USER_DATA_PATH:'/native'},getFileSystemManager:()=>native,arrayBufferToBase64:b=>Buffer.from(b).toString('base64'),base64ToArrayBuffer:s=>Uint8Array.from(Buffer.from(s,'base64')).buffer},UTF8ToString:x=>x,lengthBytesUTF8:s=>Buffer.byteLength(s),_malloc:()=>1,stringToUTF8:s=>{output=JSON.parse(s)}};
 vm.runInNewContext(fs.readFileSync(new URL('../../Assets/Match3D/Plugins/WebGL/PotReplayFiles.jslib',import.meta.url),'utf8'),context);
 const call=r=>{lib.PotReplayFile(JSON.stringify(r));return output};const payload=Buffer.from('回放😀'.repeat(15000));let fd=call({action:'open',path:'stream.bin',target:'w'}).text;
 for(let pos=0;pos<payload.length;pos+=32768)assert.equal(call({action:'writeBytes',fd,position:pos,text:payload.subarray(pos,pos+32768).toString('base64')}).ok,true);
 call({action:'close',fd});fd=call({action:'open',path:'stream.bin',target:'r'}).text;const parts=[];
 for(let pos=0;pos<payload.length;pos+=32768)parts.push(Buffer.from(call({action:'readBytes',fd,position:pos,length:Math.min(32768,payload.length-pos)}).text,'base64'));
 assert.deepEqual(Buffer.concat(parts),payload);assert.ok(maxRead<=32768);assert.equal(call({action:'readBytes',fd,position:0,length:65536}).ok,false);call({action:'close',fd});assert.equal(handles.size,0);
});

test('native upload reads compressed file only and releases task on success or memory cancellation',()=>{
 const lib={},messages=[],reads=[];let request,aborted=false,size=32;
 const context={LibraryManager:{library:lib},mergeInto:Object.assign,UTF8ToString:x=>x,Module:{SendMessage:(target,method,value)=>messages.push(JSON.parse(value))},wx:{env:{USER_DATA_PATH:'/native'},getFileSystemManager:()=>({statSync:()=>({size}),readFileSync:p=>{reads.push(p);return new ArrayBuffer(size);}}),request:o=>{request=o;return{abort:()=>{aborted=true;o.fail();}}}}};
 vm.runInNewContext(fs.readFileSync(new URL('../../Assets/Match3D/Plugins/WebGL/PotReplayUpload.jslib',import.meta.url),'utf8'),context);context.PotReplayNetwork=lib.$PotReplayNetwork;
 const args={receiver:'service',id:'one',path:'/unity/record.upload',ticket:{url:'https://storage.example/file',authorization:'signed',token:'temporary',cosFileId:'id',cloudPath:'incoming/file'}};
 lib.PotReplayUpload(JSON.stringify(args));assert.equal(request.method,'PUT');assert.deepEqual(reads,['/native/pot-replays-v1/record.upload']);request.success({statusCode:200});assert.equal(messages.at(-1).ok,true);assert.equal(context.PotReplayNetwork.task,null);
 lib.PotReplayUpload(JSON.stringify({...args,id:'two'}));lib.PotReplayUploadCancel();assert.equal(aborted,true);assert.equal(messages.at(-1).ok,false);assert.equal(context.PotReplayNetwork.task,null);
 size=16777217;const previous=reads.length;lib.PotReplayUpload(JSON.stringify({...args,id:'large'}));assert.equal(messages.at(-1).ok,false);assert.equal(reads.length,previous);
});
