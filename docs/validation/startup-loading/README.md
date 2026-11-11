# 一锅消不完：启动加载页

## 素材来源

- 玩具主题主图：本次确认的 `output/imagegen/yi-guo-xiao-bu-wan-loading-toys-v3.png`，已复制到版本管理目录 `Assets/Match3D/Resources/Match3D/Startup/ToyLoading.png`。
- 8+：原样复制疯狂动物消 `client/assets/resources/images/playAgeTip.png` 到 `Startup/Age8.png`，SHA-256 `aba178cb2f10ce77acccf794e095600690579cfe8c34559d6207918f7119307d`。未重绘图标。
- 忠告：疯狂动物消 `client/assets/scenes/loading.scene` 的两段原文，按句分成四行。额外展示标题“健康游戏忠告”。文字使用项目自带中文字体，不依赖网络字体或图片生成。

## 接入

`Match3DStartupBuilder` 生成可编辑的 `UI/StartupLoading.prefab`，再用 Unity UI 渲染导出 `Assets/Match3D/Art/Startup/WechatCover.png`。微信导出器和本地 SDK 导出配置使用同一封面；SDK 加载进度置于忠告上方。发布副本将封面转为 JPEG（质量 90），并将 Unity 内不透明的主图按 1024px/DXT1 导入，控制包体；源图与原版 8+ 图标保留。

Unity 的 Awake 首先显示启动页，不再提前创建不可操作的主页。SDK 第二帧交接到相同风格的 Unity 加载页；SDK、字体、基础服务、微信账号、未完成道具恢复及首批邮件、主页资源准备后再进入主页。编辑器和独立每日关卡 Web 预览保留免微信登录路径。其他主题模型沿用原有按需预加载。

启动协程错误和 SDK/账号等待超时会显示重试；重试复用已创建的服务，进度不会倒退。使用安全区域内等比适配，确保图标、标题、忠告完整显示。

## 重建与验证

- Unity 菜单 `Goose/UI/Build startup loading`，或写入 `Temp/build-startup-loading`。先等待脚本编译完成再触发。
- `Temp/check-startup-loading-assets`：素材/字形、嵌套异常、重试回调、进度、微信导出配置检查，以及三种分辨率渲染。
- `Temp/check-startup-home`：真实 Play Mode 首帧 Loading、初始化结束进入唯一主页、输入恢复、教程开局、返回主页。
- `Temp/check-startup-webgl`：编译 WebGL Player 脚本，覆盖编辑器条件编译外的微信登录初始化代码。

验证报告与截图保存在此目录。微信真机登录、弱网重试和 SDK 封面交接仍需打包后在设备上验证；编辑器验证不代表真机网络验证。已于 2026-11-11 成功上传开发版本 0.1.48；微信公众平台的“设为体验版”操作因浏览器管理员策略禁止访问 mp.weixin.qq.com，需由用户完成。未提交审核或发布线上版。

## 0.1.48 上传记录

- 构建源码：`ddd17b8`，构建前已推送 `origin/master`。
- 使用 `Tools/wechat/publish.py` 独立副本构建并通过微信开发者工具 CLI 上传，退出码 0，返回 `✔ upload`。
- AppID：`wx74f2d8722c02a6f4`，场景 `Assets/Scenes/Match3D.unity`。
- 上传快照：`Builds/Wechat/release-0.1.48-20261111-155522-244267`。
- 原始回执：同路径加 `-upload.json`；可复核大小摘要见 `upload-0.1.48.json`。
- 总包体 31,444,145 字节，主包 1,652,696 字节。微信上传已接受；总包体接近 30 MiB，后续增加资源须留意。
- 下一步：微信公众平台 → 版本管理 → 开发版本 0.1.48 → 设为体验版，再真机验证。
