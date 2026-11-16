const assert = require('node:assert/strict');
const fs = require('node:fs');
const vm = require('node:vm');
const source = fs.readFileSync('Packages/com.qq.weixin.minigame/Runtime/wechat-default/plugins/check-update.js', 'utf8');
function fixture() {
    const callbacks = {}, modals = [];
    let applied = 0;
    const manager = {
        onUpdateReady: fn => { callbacks.ready = fn; },
        onUpdateFailed: fn => { callbacks.failed = fn; },
        applyUpdate: () => { applied++; },
    };
    vm.runInNewContext(source, { wx: { getUpdateManager: () => manager, showModal: modal => modals.push(modal) } });
    return { callbacks, modals, applied: () => applied };
}
const f = fixture();
assert.equal(f.modals.length, 0, 'No prompt before an update is ready');
f.callbacks.ready();
f.callbacks.ready();
assert.equal(f.modals.length, 1, 'Do not stack update prompts');
assert.equal(f.modals[0].confirmText, '立即更新');
assert.equal(f.modals[0].cancelText, '稍后');
f.modals[0].success({ cancel: true });
f.modals[0].complete();
assert.equal(f.applied(), 0, 'Cancel must keep the current game running');
f.callbacks.ready();
f.modals[1].success({ confirm: true });
f.modals[1].complete();
f.callbacks.ready();
assert.equal(f.applied(), 1);
assert.equal(f.modals.length, 2, 'Do not prompt again while restarting');
const failed = fixture();
failed.callbacks.failed();
failed.callbacks.failed();
assert.equal(failed.modals.length, 1);
assert.equal(failed.modals[0].showCancel, false);
assert.equal(failed.applied(), 0, 'A failed download must never restart');
failed.modals[0].complete();
failed.callbacks.ready();
assert.equal(failed.modals.length, 2, 'A subsequent successful download can prompt');
vm.runInNewContext(source, {});
vm.runInNewContext(source, { wx: {} });
assert.match(fs.readFileSync('Packages/com.qq.weixin.minigame/Editor/MiniGameConfig.asset', 'utf8'), /needCheckUpdate: 1/);
assert.match(fs.readFileSync('Assets/Match3D/Editor/WechatReleaseBuild.cs', 'utf8'), /config\.ProjectConf\.needCheckUpdate\s*=\s*true;/);
console.log('PASS: ready, cancel, confirm, duplicate events, failed download, recovery, unsupported platform and build configuration');
