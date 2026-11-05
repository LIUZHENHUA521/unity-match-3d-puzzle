# 微信 SDK 与字体来源

官方仓库：https://github.com/wechat-miniprogram/minigame-tuanjie-transform-sdk
固定提交：a09d4b29daa1dd8358b09b5b5639554ab08cfdc2
嵌入目录：Packages/com.qq.weixin.minigame

Git 传输较慢，因此按该提交的 Git tree 下载官方 raw 文件；651 个文件均逐个核对 Git blob SHA-1 一致后导入。SDK 代码未修改；MiniGameConfig.asset 配置为当前游戏的 AppID、输出目录和竖屏分包构建。Editor/Node/node_modules 是 SDK 自带构建依赖，应保留。

字体：启动后优先通过 `WX.GetWXFont` 使用微信系统字体，并开启 `PreloadWXFont`。所有 ReplicaUIView（含隐藏子节点）在实例化时统一应用选定字体，动态文字继承所属 UI 的字体。

当前固定版本 SDK 明确不支持 iOS 的 common font 获取；iOS、不支持该接口的平台、加载失败或超过 8 秒时使用随包的常用中文字体 `DefaultChinese.otf`（Noto Sans CJK SC Regular），编辑器也使用此字体。字库包含 GB2312 常用字符及项目现有文案；少见昵称字可能不覆盖。新增文案超出字库时需重新生成。保留原资源 GUID 以迁移已有 Prefab/场景引用。

备用字体来自 https://github.com/notofonts/noto-cjk/blob/main/Sans/OTF/SimplifiedChinese/NotoSansCJKsc-Regular.otf ，SIL Open Font License 位于同目录 LICENSE.txt。完整字体约 15.7 MiB，直接打包超出发布限制。使用 `python3 Tools/wechat/subset-default-font.py --source /path/to/NotoSansCJKsc-Regular.otf` 生成发布字库，并检查压缩后包体大小。

验证：`node Tools/wechat/test-default-font.cjs` 检查平台选择；Unity 执行 `ReplicaDefaultFontValidation.Run` 检查脚本文案汉字覆盖和全部 UI Prefab 字体绑定。真机仍需检查 Android / iOS 的字体外观、换行和昵称显示。

## 游戏启动页

加载页复用首页 HomeSkinPS/img_Homepage_bg2.jpg，发布构建显式设置 bgImageSrc。SDK 的 Runtime/wechat-default/game.js 隐藏额外 Unity 图标，并使用游戏加载文案及绿色进度条；升级 SDK 时保留此定制。Unity 2022 Personal 仍保留引擎启动标志并使用同款背景，有 Pro 授权的发布构建自动关闭引擎启动画面。需要重新构建微信包后生效。

build.py 另将首页原始 JPG 保存为 Resources/Match3D/HomeBackgroundOriginal.bytes。WebGL 运行时由 Match3DHomeView 首次显示时解码并缓存 Sprite，替换首页及同类页签的背景，保留 720×1565 原始细节，不经过纹理降采样或 DXT 压缩。使用 JPG 存储可避免直接打包无压缩纹理超出分包限制；解码后关闭 CPU 可读副本。原图集保留作为缺失资源时的回退，其他 UI 沿用发布压缩设置。
