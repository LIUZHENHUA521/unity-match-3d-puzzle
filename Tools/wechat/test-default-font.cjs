const fs = require('node:fs');
const vm = require('node:vm');
const assert = require('node:assert/strict');
const source = fs.readFileSync('Assets/Match3D/Plugins/WebGL/PotDefaultFont.jslib', 'utf8');
function check(platform, supported, expected) {
  const library = {};
  const context = { LibraryManager: { library }, mergeInto: Object.assign,
    wx: { getSystemInfoSync: () => ({platform}) },
    GameGlobal: { manager: {font: supported ? {getCommonFont(){}} : {}} } };
  vm.runInNewContext(source, context);
  assert.equal(library.GooseCanLoadSystemFont(), expected);
}
check('ios', true, 0);
check('android', true, 1);
check('android', false, 0);
check('windows', false, 0);
console.log('Default font platform checks passed');
