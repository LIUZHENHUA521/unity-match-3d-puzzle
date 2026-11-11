const {test}=require('node:test');
const assert=require('node:assert/strict');
const vm=require('node:vm');
const fs=require('node:fs');
const path=require('node:path');
const url='https://work.weixin.qq.com/kf/test?enc_scene=signed&scene_param='+ 'a'.repeat(64);
function setup(wx,window){
 const library={};
 vm.runInNewContext(fs.readFileSync(path.join(__dirname,'../../Assets/Match3D/Plugins/WebGL/GooseProfile.jslib'),'utf8'),{LibraryManager:{library},mergeInto:Object.assign,UTF8ToString:value=>value,wx,window});
 return (link=url)=>library.GooseCustomerServiceOpen(link);
}
test('user tap opens the dedicated game account synchronously',()=>{
 let called=false;
 setup({openCustomerServiceChat(o){called=true;assert.equal(o.extInfo.url,url);assert.equal(o.corpId,'ww18ea30e9ed7eee49');},showModal(){assert.fail('unexpected fallback');}})();
 assert.ok(called);
});
test('failed launch waits for confirmation before copying the same account link',()=>{
 let modal,copied,toast;
 setup({openCustomerServiceChat(o){o.fail({errMsg:'permission denied'});},showModal(o){modal=o;},setClipboardData(o){copied=o.data;o.success();},showToast(o){toast=o.title;}})();
 assert.equal(copied,undefined);modal.success({confirm:false});assert.equal(copied,undefined);
 modal.success({confirm:true});assert.equal(copied,url);assert.equal(toast,'客服链接已复制');
});
test('missing API and synchronous errors offer fallback',()=>{
 for(const api of [undefined,()=>{throw Error('unsupported');}]){
  let shown=0;setup({openCustomerServiceChat:api,showModal(){shown++;}})();assert.equal(shown,1);
 }
});
test('user cancellation does not show an error modal',()=>{
 setup({openCustomerServiceChat(o){o.fail({errMsg:'openCustomerServiceChat:fail cancel'});},showModal(){assert.fail('cancel should be quiet');}})();
});
test('clipboard failure is visible and unsupported browser shows guidance',()=>{
 let message;
 setup({showModal(o){o.success({confirm:true});},setClipboardData(o){o.fail();},showToast(o){message=o.title;}})();
 assert.equal(message,'复制失败，请稍后重试');
 setup(undefined,{alert(s){message=s;}})();assert.equal(message,'请在微信小游戏中联系客服');
});

test('rejects missing, untrusted and unassociated links without opening chat',()=>{
 for(const link of ['', 'https://evil.example/?enc_scene=x&scene_param=y', 'https://work.weixin.qq.com/kfid/old']){
  let notice;setup({openCustomerServiceChat(){assert.fail('must not open')},showToast(o){notice=o.title}})(link);assert.match(notice,/失效/);
 }
});
