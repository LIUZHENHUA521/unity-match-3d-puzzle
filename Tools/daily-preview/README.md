# 每日候选 WebGL

`python3 Tools/daily-preview/build.py` 在 `Builds/DailyWebProject` 中构建，产物为该目录下的 `Builds/DailyWebPreview`。使用标准 Unity WebGL 与 `DAILY_WEB_PREVIEW`，不会覆盖微信发布目录。

构建阶段复用微信资源压缩配置，但只在临时工程移除微信 SDK 及微信专用构建入口。微信 SDK 的全局 PlayerPrefs 和原生 JS 桥不能用于浏览器。源工程及当前编辑器不受影响。

后台按版本引用 `/admin/goose-preview/<version>/index.html`。部署 `index.html`、`bridge.js`、`config.json`、`Build/`，先上传游戏资源，再部署引用该版本的后台。开启 gzip 解压回退，不依赖服务器 Content-Encoding 配置。默认加载超时 3 分钟，可关闭重开。

桥接协议 `goose-daily-preview-v1`：iframe hash 的随机 session 绑定父窗口。parent→child：load(level)、play(stage)。child→parent：ready、status、playing、generated(package)、error。精确 origin、source、session 校验；不传管理员凭据。生成的布局只经后台显式保存，写接口再次验证候选身份与打开时 revision。此页面是同源可信代码，不是第三方脚本隔离沙箱。

运行 `node --test Tools/daily-preview/bridge.test.mjs` 验证子页消息边界；后台 `node --test scripts/tests/goose-preview.test.mjs` 验证父页边界。真实 WebGL 测试和部署记录见 `docs/validation/daily-web-preview/README.md`。
