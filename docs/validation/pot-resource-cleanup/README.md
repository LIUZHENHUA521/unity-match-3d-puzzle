# Pot 旧素材与 PVP 清理（2026-11-15）

删除整个 `Assets/Match3D/Resources/GooseArt`（821 张图片、7 个音频及对应 meta），删除 PvpLobby/PvpGameHUD 和已停用的 Result/Themes 页面；删除原生/WebGL PVP 传输组件、专用桥接及其测试。历史素材来源记录和已上传快照保留。

GameHUD、ModelSize、Diagnostics、Collection、Reward 中旧图片引用换为现有 SettingsSkinPS 与主题背景；收藏页运行时加载当前食物主题图。菜单点击复用 CrazyCombo/tile_click，胜负旧音效停用，现有背景音乐、选择/三消音效保留。其他目录（如 SourceAudio、OriginalModels）的来源审查不属于本轮清理范围。

`pvpEndpoint` 是现有序列化字段，登录、奖励、记录和回放仍共用此服务地址，因此保持兼容。服务端、接口、本地存档和其他桥接协议未迁移。部分单机模块保留 IsPvp 防护判断，但已没有进入 PVP 的入口及网络实现。

构建脚本在旧素材目录或 PVP 页面重新出现时直接失败，不再使用旧图片保留名单，也不再压缩已删的旧 BGM。发布工具的 npm test 改为现有的发布流程测试。

## 验证

- 删除资源的 839 个 GUID 在当前 Assets 序列化资源中无残留引用。
- 发布流程单元测试 5 项通过；build.py 语法检查通过。
- 独立 Unity 副本运行检查通过：资源/组件引用、首页、收藏、奖励、设置、第一关、第二关、模型大小弹窗、单机结算和返回首页，详见 runtime-validation.txt。
- 微信 WebGL 构建成功，资源分包加载开启；构建清单及解压后的数据包均无 GooseArt/PVP 页面。产物位于 `Builds/PotResourceCleanupProject/Builds/Wechat/minigame`。本轮不上传，不替换既有发布快照。
