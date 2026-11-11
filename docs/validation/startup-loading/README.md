# 一锅消不完：启动加载页

## 当前版本：确认后的长屏新图

- 用户确认的新图已原样保存为 `Assets/Match3D/Art/Startup/ToyLoadingLong.png`，原始分辨率 841×1870（约 9:20）。此前 v3 的资源已替换。
- `ToyLoadingLong.bytes` 是该图原尺寸 JPEG（质量 95），加载时解码成原分辨率纹理；不再通过 WebGL 1024px/DXT1 缩小和块压缩启动图。源 PNG 位于 Resources 外，避免重复打包。
- Unity 背景采用等比 cover，随视口适量裁切，文字和图标单独适配安全区；不拉伸，不留黑边。
- SDK 封面由同一 Unity UI 渲染为 841×1870 和 841×1495 两种构图，根据手机窗口比例选择，再以 NO_BORDER 等比填满。保留原版 8+ 标识和真实字体忠告。
- 发布时只缩小主页中已被原图 JPEG 同步替换的备用精灵，以腾出新封面的包体空间；主页实际展示的原图 JPEG 不变。此优化只作用于隔离构建副本。
- 验证报告与四种屏幕截图已更新。下方 0.1.48/0.1.49 段落为旧版历史记录。

### 0.1.50 上传结果

- 源码提交 `540c28d` 推送后完成隔离构建，微信 CLI 上传成功，退出码 0。
- 总包体 30,458,453 字节；主包 2,517,270 字节；数据包 21,812,271 字节。
- 发布快照：`Builds/Wechat/release-0.1.50-20261111-184945-214838`，回执为同路径加 `-upload.json`。
- 实际导出文件确认 NO_BORDER、两种封面均存在且分别为 841×1870、841×1495。运行时 JPEG 和主页原图字节与源文件一致。
- 69 项断言和 Play Mode 流程通过；手机上的 SDK 交接及安全区表现仍待用户真机复看。
- 公众平台访问受已确认的浏览器管理员策略限制，需用户将 0.1.50 设为体验版。未提审或发布线上版。

## 素材来源

- 玩具主题主图：本次确认的 `output/imagegen/yi-guo-xiao-bu-wan-loading-toys-v3.png`，已复制到版本管理目录 `Assets/Match3D/Resources/Match3D/Startup/ToyLoading.png`。
- 8+：原样复制疯狂动物消 `client/assets/resources/images/playAgeTip.png` 到 `Startup/Age8.png`，SHA-256 `aba178cb2f10ce77acccf794e095600690579cfe8c34559d6207918f7119307d`。未重绘图标。
- 忠告：疯狂动物消 `client/assets/scenes/loading.scene` 的两段原文，按句分成四行。额外展示标题“健康游戏忠告”。文字使用项目自带中文字体，不依赖网络字体或图片生成。

## 接入

`Match3DStartupBuilder` 生成可编辑的 `UI/StartupLoading.prefab`，再用 Unity UI 渲染导出 `Assets/Match3D/Art/Startup/WechatCover.png`。微信导出器和本地 SDK 导出配置使用同一封面；SDK 加载进度置于忠告上方。发布副本将封面转为 JPEG（质量 90），并将 Unity 内不透明的主图按 1024px/DXT1 导入，控制包体；源图与原版 8+ 图标保留。

Unity 的 Awake 首先显示启动页，不再提前创建不可操作的主页。SDK 第二帧交接到相同风格的 Unity 加载页；SDK、字体、基础服务、微信账号、未完成道具恢复及首批邮件、主页资源准备后再进入主页。编辑器和独立每日关卡 Web 预览保留免微信登录路径。其他主题模型沿用原有按需预加载。

启动协程错误和 SDK/账号等待超时会显示重试；重试复用已创建的服务，进度不会倒退。背景独立于安全区，四边拉伸铺满屏幕，保留完整标题；Unity 前景图标和文字按安全区等比适配，并在长屏上靠近底部地毯空白处。微信 SDK 封面使用 EXACT_FIT，避免 SHOW_ALL 在长屏上产生上下黑边。

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

## 0.1.49 全屏适配修复

- 原因：SDK 的 SHOW_ALL 对 720×1280 封面保留完整比例，在长屏上产生上下黑边；Unity 背景也随安全区内容一起缩小。
- 修复：SDK 改用 EXACT_FIT；Unity 背景移至根节点并四边拉伸，前景文字/图标单独按安全区等比适配，长屏底部位置随地毯空白调整。标题烘焙在主图中，采用拉伸以保留标题两侧；主图会随屏幕比例纵向伸展。
- 63 项断言通过，预览覆盖 720×1280、720×1560、750×1334、720×1600。Play Mode 启动到主页及教程/返回流程通过。尚未代替真机验证。
- 源码提交 `128c374` 已推送后构建。实际导出的 game.js 已确认 EXACT_FIT，AppID 正确。
- 微信 CLI 成功上传 0.1.49，总包体 31,443,294 字节。回执 `Builds/Wechat/release-0.1.49-20261111-163326-957752-upload.json`；摘要见 `upload-0.1.49.json`。
- 受先前已确认的浏览器管理员策略限制，未操作公众平台；用户需将 0.1.49 设为体验版。未提交审核。
