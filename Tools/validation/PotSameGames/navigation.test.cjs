const assert = require('node:assert/strict');
const fs = require('node:fs');
const vm = require('node:vm');
const source = fs.readFileSync('Assets/Match3D/Plugins/WebGL/PotSameGames.jslib', 'utf8');
function fixture(wx) {
    const library = {}, timers = new Map(); let id = 0;
    const context = { wx, LibraryManager: { library }, mergeInto: Object.assign,
        UTF8ToString: x => x, setTimeout: f => { timers.set(++id,f); return id; }, clearTimeout: n => timers.delete(n) };
    vm.runInNewContext(source, context);
    context.PotSameGamesNavigation = library.$PotSameGamesNavigation;
    return { open: () => library.PotSameGamesOpen('wx85d24d5dbf72b423'), timers };
}
let calls=[], notices=[];
const f=fixture({navigateToMiniProgram: o=>calls.push(o),showToast:o=>notices.push(o.title)});
f.open();f.open();assert.equal(calls.length,1);
assert.equal(calls[0].appId,'wx85d24d5dbf72b423');assert.equal(calls[0].envVersion,'release');
assert.equal(calls[0].extraData.source,'pot');
calls[0].fail({errMsg:'navigateToMiniProgram:fail cancel'});calls[0].complete();
assert.equal(notices.length,0);f.open();assert.equal(calls.length,2);
calls[1].fail({errMsg:'network failure'});assert.equal(notices.pop(),'暂时无法打开，请稍后再试');
f.open();for(const timer of [...f.timers.values()])timer();f.open();assert.equal(calls.length,4);
calls[2].complete();f.open();assert.equal(calls.length,4,'stale callback cannot unlock new request');
calls[3].complete();f.open();assert.equal(calls.length,5);
fixture(undefined).open();
fixture({showToast:o=>notices.push(o.title)}).open();assert.equal(notices.pop(),'请在微信小游戏中打开疯狂动物消');
const throwing=fixture({navigateToMiniProgram:()=>{throw Error('unsupported');},showToast:o=>notices.push(o.title)});
throwing.open();throwing.open();assert.equal(notices.length,2);
console.log('PASS navigation target, duplicate taps, cancel, failure, timeout, stale callback, unsupported API and exceptions');
