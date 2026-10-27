# 微信小游戏发布配置

## Unity 发布菜单（macOS）

打开 **工具 → 微信小游戏 → 发布**，填写版本号和更新说明，点击 **保存、构建并上传开发版本**。
菜单使用 `release.config.json` 指定的场景（窗口会显示），并非当前打开的任意场景。
先在微信开发者工具登录对应账号，并在设置 → 安全设置开启服务端口；本入口不需要 CI 上传私钥。

发布会保存资源、要求保存已修改场景，在 `Builds/WechatMenuProject` 构建独立副本，校验 AppID、方向和资源分包，
再复制到带版本号和时间戳的独立目录，通过开发者工具 CLI 上传。
上传成功后仍需在公众平台「版本管理」设为体验版；本工具不提审、不发布正式版。
发布期间可关闭窗口再打开，但请勿退出 Unity；为避免中断，脚本程序集重载会延迟到发布结束。

窗口提供发布日志、Unity 构建日志和公众平台入口。日志分别位于 `Builds/Wechat/menu-publish.log` 和
`Builds/wechat-build.log`，上传回执与独立发布目录同在 `Builds/Wechat`。
脚本安全检查可运行 `python3 Tools/wechat/test-publish.py`，不会实际构建或上传。

AppID：`wx74f2d8722c02a6f4`，由用户从 sandBlast 分配给当前 Unity 游戏。

- `release.config.json`：AppID、目标场景、竖屏、输出目录和 PVP 地址。
- `.local.json`：仅本机保存上传私钥路径，已加入 gitignore。引用 sandBlast 原有私钥，没有复制到 Assets 或构建包。
- `upload.cjs`：校验小游戏产物和 AppID，上传开发版本或生成预览。默认不提审、不发布正式版。
- AppSecret 不是代码上传密钥。当前上传流程不需要它，不写入客户端；后续服务端接入 wx.login 时再通过服务端环境变量配置。

## 当前进度（2026-10-14）

- Unity 2022.3.62f3c1（1623fc0bbb97）的 WebGL Build Support 已安装，官方包校验通过，Rosetta 已安装。独立最小 WebGL 构建成功、错误数为 0；已打开的 Unity 编辑器需重启以刷新平台列表。
- 微信转换 SDK 已完整接入并核对 Git 文件哈希，固定版本见 `SDK.md`。
- 完整游戏构建成功，产物位于 `Builds/Wechat/minigame`。资源通过小游戏分包加载，数据包约 14.5 MB；构建快照只纳入当前界面实际引用的 15 张参考 PNG，主工程全部参考素材保留。
- 已接入中文字体、微信振动和网络接口，PVP 传输的回归测试以及双客户端建房、加入、准备流程验证通过。
- 上传前本地检查通过。微信开发者工具可加载首页，但模拟器按钮触控尚未验证通过，不能视为完整可玩或真机验收通过。
- 微信开发版本 0.1.0 已于 2026-10-14 通过本机已登录的开发者工具上传成功（总包 21,524,621 字节，主包 1,384,984 字节）。未设为体验版，未提交审核。当前 origin 仍指向模板作者，源码远程备份尚待配置。
- CI 私钥上传当前受微信 IP 白名单限制；本次使用开发者工具 CLI 完成上传，未修改白名单。结果记录在 `Builds/Wechat/upload-info.json`。
- `Builds/Wechat/input-probe` 是临时触控诊断包，不是发布目录。

## 构建与验证

在项目根目录执行：

```sh
python3 Tools/wechat/build.py
node Tools/wechat/test-transport.cjs
node Tools/wechat/upload.cjs --check
```

构建脚本创建独立快照，避免与正在打开的 Unity 项目争用；日志位于 `Builds/wechat-build.log`。小游戏输出为 `Builds/Wechat/minigame`。

## 上传命令

完成运行验收及源码提交推送后执行：

```sh
cd Tools/wechat
npm ci --ignore-scripts
npm run check
npm run preview
npm run upload -- --version 0.1.0
```

`npm run upload` 上传开发版本后，需要在微信公众平台版本管理设为体验版。脚本不会提交审核。

可通过 `GOOSE_WECHAT_PRIVATE_KEY_PATH` 覆盖本机私钥路径。不要提交 `.local.json`、AppSecret、私钥或 `.env` 文件。

## 横向适配与拾取修正（2026-10-14）

以下修正已随微信开发版本 0.1.1 上传：

- CameraFitter 使用锅体的实际宽度适配全屏，取消 9:16 横向留白；模型的屏幕显示随之放大，物理尺寸保持原样。
- InputController 支持原生 Touch 的按下、结束、取消，保留鼠标支持且避免重复消费；拖动阈值随渲染宽度缩放。
- 精确射线优先，边缘最多容错 24 / 720 屏宽；每条射线仍只使用首个碰撞体，避免穿透拾取。
- 不可见围栏使用 Ignore Raycast 层，物理防漏功能保留。
- Unity 编辑器验证通过：三种宽高比的锅宽、前后遮挡、近边缘命中、远处空白不命中。结果见 `Builds/Wechat/viewport-picking-validation.txt`。真机触摸手感尚待验收。

## 放大试玩版本 0.1.1

已通过微信开发者工具上传成功。锅内模型与碰撞体统一按原 prefab 的 1.5 倍生成；入槽缩放补偿，保持七格布局，撤回恢复锅内尺寸。生成位置、关卡数量保持原样。版本同时包含上述适配和点击修正。总包 21,530,216 字节，上传记录 `Builds/Wechat/upload-info-0.1.1.json`。尚未提交审核；真机手感由试玩确认。

## 音效版本 0.1.2

已通过微信开发者工具上传开发版本。选择音效为 crazyCombo 的 tile_click.mp3，消除音效为 merge_sound.mp3，文件哈希与来源一致，Unity 构建报告确认两段音频打入包中。选中成功路径播放选择音，三消完成播放消除音，均遵守音效开关。上传使用 `Builds/Wechat/release-0.1.2` 独立快照，规避开发者工具对替换目录的旧文件索引缓存；结果为 `Builds/Wechat/upload-info-0.1.2.json`。未提交审核。

## iOS 性能版本 0.1.3

已上传微信开发版本，测试反馈设备为 iPhone 17 Pro。保留 1.5 倍物品、144 个第二关物品及原有物理参数（50 Hz / ContinuousDynamic / 原睡眠阈值）。30 Hz、推测式/离散碰撞候选在编辑器对比中出现更慢或漏出锅的问题，已全部排除，记录 `Builds/Wechat/rejected-physics-candidate.txt` 中的 optimized=True 是未采用的候选，不是发布版。

发布改动：冻结屏幕特效仅在 FrostAmount>0 时启用；WebGL 关闭实时阴影、HDR、MSAA；官方微信 SDK iOS 像素倍率设为 2；HUD 文字仅在数值改变时更新；目标帧率 60 不等于保证达到 60 FPS。

完整 WebGL 构建成功，冻结特效启停检查通过，生成包的 iOS 倍率已验证。上传快照 `Builds/Wechat/release-0.1.3`，结果 `Builds/Wechat/upload-info-0.1.3.json`。未取得 iOS 真机性能采样，改善幅度需复测；画面阴影与精细度会有所降低。未提审。

## 卡顿诊断版 0.1.4

用户报告 0.1.3 在 iPhone 17 Pro 第二关仍非常卡、点不动，尚未解决。已上传 0.1.4 开发诊断版（非正式发布）。单人关卡顶部显示 FPS、最大帧间隔、HP 运行标记（0 普通/1 高性能/2 Plus；编辑器 -1），微信原生 touch 开始/结束、Unity 输入、命中及选中累计次数、未休眠物体数与游戏状态。数据只在本地显示，不上传遥测。

Physics ON/OFF 暂停/恢复物理模拟；Models ON/OFF 隐藏/恢复道具渲染，用于分离瓶颈。离开或重开关卡恢复开关。隐藏模型仅用于诊断。原生桥计数/模式测试通过，WebGL 构建、包检查和上传通过；仍需用户点道具后提供真机诊断栏截图。暂未取得真机 FPS 或 HP 结果，不能宣称已修复或断言未开通高性能模式。

## 物理预算版本 0.1.5

微信开发版本上传成功。原版元数据线索和开源项目对比见 `Tools/goose-physics-reference.md`；未恢复加密原生函数，本实现及其 16 个活动刚体上限是自主方案。

ReplicaPilePhysics 管理所有锅内物品：停算物品保持可碰撞、可选择；动态数最多 16；移走物品触发附近上方的局部唤醒；撤回重新登记；颠锅按预算分批处理。初始化仍全部显示，并在原有 3 秒准备后开放输入，全队落位可继续进行。每次活动最多模拟 2 秒以截断抖动，是近似机制；须真机检查补位及悬停观感。1.5 倍尺寸、数量与服务器不变。

三主题物理验证及完整教程/难关/撤回/七格失败/通关收藏/回首页检查通过。报告：`Builds/Wechat/bounded-pile-validation.txt` 和 `bounded-gameplay-validation.txt`。WebGL 构建、上传检查通过，上传记录 `upload-info-0.1.5.json`。真机 FPS 尚待用户复测，未宣称卡顿已解决。诊断栏 `sim` 显示活动数/总锅内数；测试保持 Physics ON、Models ON。

## 实时阴影版本 0.1.6

恢复 ReplicaFlow 在编辑器和运行时的实时灯光投影：ShadowQuality.All、Medium 分辨率、20 单位距离、无级联，沿用场景已有柔和方向光与模型投射/接收阴影。撤销 0.1.3 的全局关闭阴影；保留 0.1.5 物理预算、1.5 倍尺寸及闲置冻结后处理关闭。WebGL 构建成功（GOOSE_WECHAT_EXPORT_OK），包检查通过，微信开发者工具确认 upload 成功，总包 21569529 字节；快照 release-0.1.6，记录 upload-info-0.1.6.json。尚未验证 iOS 真机阴影观感和恢复投影后的 FPS，未提审。

## 精简主页换皮 0.1.7

用户提供 UI 素材并确认本科局接普通玩法、清北局接主题玩法。新增 ReplicaHomeView：疯狂动物消兔子背景，蓝色本科局/黄色清北局及 2 元/10 元奖励展示；底部按主页、收藏、宠物排列，后两者占位。红包说明只展示规则待开放，不接结算服务。首页小图使用独立 HomeUI 图集，背景独立，补齐 GooseSans 文案字形。旧玩法、收藏进度、物理预算、阴影与 1.5 倍尺寸保留。

HomeSkinValidation 的 720x1560、720x1280、768x1024 布局及 Tab/弹窗/本科局/主题入口/返回检查全部通过，预览与报告在 `Builds/Wechat/HomePreview`。WebGL 构建与上传检查通过，微信工具明确返回 upload 成功；0.1.7 开发包总 23217595 字节，快照 `release-0.1.7`，记录 `upload-info-0.1.7.json`，未提审。新首页尚待真机视觉复测。资源来源及替换位置见 `Tools/home-skin-assets.md`。

## 底部 Tab 细分隔线 0.1.8

修复首页 Tab 底图横向拉伸导致相邻深色边缘变粗：左右 4px sprite border，Image.Sliced，pixelsPerUnitMultiplier=2。原图保留，图标、文字和交互不变。三尺寸截图及现有首页导航检查通过，预览在 HomePreview；WebGL 构建、包检查、微信开发版本 upload 成功，总包 23211837 字节。快照 release-0.1.8，上传记录 upload-info-0.1.8.json。未提审。

## 独立七格收纳槽 0.1.9

按用户提供 demo 的 MatchingPad 结构重做底部槽：单块九宫格圆角底板 + 七个独立浅色格子，隐藏旧 Stack 平面纹理；保留现有 3D 入槽物品，用 ReplicaTray 统一 UI 位置、物品落点及缩放比例。来源和结构见 Tools/tray-reference.md。

PlayMode 验证入槽、七格落点、撤回、三消、七格失败、主页隐藏及清场全部通过，报告/截图在 TrayPreview。WebGL 构建、包检查及微信开发版本 upload 成功，总包 23227320 字节；快照 release-0.1.9，结果 upload-info-0.1.9.json。未提审，预览为编辑器渲染，真机观感待用户复测。

## 当前修改重新构建上传 0.1.10（2026-10-15）

按用户请求从当前源码快照构建，包含分层生成、模型尺寸调节、选中反馈等最新修改；源码哈希记录 source-snapshot-0.1.10.json。检测到另一个并行构建，发布快照直接从本次隔离 staging 导出目录复制，避免共用 minigame 产物混淆。Unity 构建退出 0，AppID/资源分包校验通过，开发工具确认 upload 成功，总包 23254318 字节。快照 release-0.1.10，上传记录 upload-info-0.1.10.json。

已上传开发版本，未确认设为体验版。尝试打开已登录的公众平台版本管理页面时被浏览器站点安全策略拒绝，未绕过；需用户在公众平台对 0.1.10 手动设为体验版。不提审，未宣称真机功能测试已完成。

## 最新修改上传 0.1.11（2026-10-15）

从当前保存内容重新构建，包含 ReplicaPilePhysics、ReplicaLayeredPile 最新变化。源码记录 source-snapshot-0.1.11.json，上传时再次核对，无快照后变更。本轮未修改游戏逻辑；游戏内诊断标签仍为源码中的 0.1.10，公众平台上传版本为 0.1.11。Unity GOOSE_WECHAT_EXPORT_OK、包/AppID 校验及开发工具 upload 均通过，总包 23259622 字节，快照 release-0.1.11，记录 upload-info-0.1.11.json。已上传开发版本，因既有公众平台浏览器安全策略拦截，未重试该页面或绕过限制，仍需用户手动设为体验版；未提审。
