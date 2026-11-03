# 微信 SDK 与字体来源

官方仓库：https://github.com/wechat-miniprogram/minigame-tuanjie-transform-sdk
固定提交：a09d4b29daa1dd8358b09b5b5639554ab08cfdc2
嵌入目录：Packages/com.qq.weixin.minigame

Git 传输较慢，因此按该提交的 Git tree 下载官方 raw 文件；651 个文件均逐个核对 Git blob SHA-1 一致后导入。SDK 代码未修改；MiniGameConfig.asset 配置为当前游戏的 AppID、输出目录和竖屏分包构建。Editor/Node/node_modules 是 SDK 自带构建依赖，应保留。

字体：启动后优先通过 `WX.GetWXFont` 使用微信系统字体，并开启 `PreloadWXFont`。所有 ReplicaUIView（含隐藏子节点）在实例化时统一应用选定字体，动态文字继承所属 UI 的字体。

当前固定版本 SDK 明确不支持 iOS 的 common font 获取；iOS、不支持该接口的平台、加载失败或超过 8 秒时使用随包的完整标准中文字体 `DefaultChinese.otf`（Noto Sans CJK SC Regular），编辑器也使用此字体。不再使用 GooseSans 文案子集，无需随新增文案手工补字。保留原资源 GUID 以迁移已有 Prefab/场景引用。

备用字体来自 https://github.com/notofonts/noto-cjk/blob/main/Sans/OTF/SimplifiedChinese/NotoSansCJKsc-Regular.otf ，SIL Open Font License 位于同目录 LICENSE.txt。完整字体约 15.7 MiB，发布时需要重新检查压缩后包体大小。

验证：`node Tools/wechat/test-default-font.cjs` 检查平台选择；Unity 执行 `ReplicaDefaultFontValidation.Run` 检查脚本文案汉字覆盖和全部 UI Prefab 字体绑定。真机仍需检查 Android / iOS 的字体外观、换行和昵称显示。

## 游戏启动页

加载页复用首页 HomeSkinPS/img_Homepage_bg2.jpg，发布构建显式设置 bgImageSrc。SDK 的 Runtime/wechat-default/game.js 隐藏额外 Unity 图标，并使用游戏加载文案及绿色进度条；升级 SDK 时保留此定制。Unity 2022 Personal 仍保留引擎启动标志并使用同款背景，有 Pro 授权的发布构建自动关闭引擎启动画面。需要重新构建微信包后生效。
