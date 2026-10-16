const fs=require('node:fs'), vm=require('node:vm'), assert=require('node:assert/strict'), path=require('node:path');
const events=[], requests=[], sockets=[];
const wx={
  request(options){requests.push(options);return{abort(){}};},
  connectSocket(){
    const handlers={};
    const socket={handlers,sent:[],onOpen:f=>handlers.open=f,onMessage:f=>handlers.message=f,
      onClose:f=>handlers.close=f,onError:f=>handlers.error=f,
      send(options){socket.sent.push(JSON.parse(options.data));},close(){handlers.close();}};
    sockets.push(socket);return socket;
  }
};
const library={};
const context={LibraryManager:{library},mergeInto:Object.assign,UTF8ToString:s=>s,
  Module:{SendMessage:(target,method,json)=>events.push(JSON.parse(json))},wx};
vm.createContext(context);
vm.runInContext(fs.readFileSync(path.join(__dirname,'../../Assets/GooseReplica/Plugins/WebGL/GooseTransport.jslib'),'utf8'),context);
context.GooseTransport=library.$GooseTransport;
library.GooseConnect('test','https://example.invalid','match','',1);
library.GooseDisconnect('test');
requests[0].success({statusCode:200,data:{token:'old',playerId:'old'}});
assert.equal(sockets.length,0,'cancelled session must not open a socket');
library.GooseConnect('test','https://example.invalid','join','123456',2);
requests[1].success({statusCode:200,data:{token:'new',playerId:'player'}});
sockets[0].handlers.open();
assert.deepEqual(sockets[0].sent,[{type:'join',roomId:'123456'}]);
sockets[0].handlers.message({data:'{"type":"room"}'});
assert.equal(events.at(-1).kind,'message');
library.GooseDisconnect('test');
const count=events.length;
sockets[0].handlers.message({data:'{"type":"room"}'});
assert.equal(events.length,count,'late events must not reach a new game');
library.GooseConnect('test','https://example.invalid','match','',3);
requests[2].success({statusCode:503,data:{}});
requests[2].fail();
assert.equal(events.length,count+1,'one failure notification per connection');
assert.equal(events.at(-1).kind,'error');
console.log('PASS: cancellation, room join, messages, stale callbacks, HTTP errors');
