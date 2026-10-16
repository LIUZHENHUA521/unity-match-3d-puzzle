# 编辑器可玩版

Unity 2021.3.1f1c1，打开 `Assets/Scenes/GooseReplica.unity` 后 Play。旧 FoodPileChallenge 和 GameScene 保留。

- 首页 → 开始抓鹅：12 个物品 / 2 种 / 120 秒的第一关，通关进入 300 个物品 / 600 秒第二关。
- 第二关自下而上四层：60、36、24、24；类型数 8、6、4、3。物理落下后会混合，不能把配置层理解为不动的平面。
- 普通乡村每日挑战 + 小卖部、烧烤主题可玩；每个主题通关第二关点亮对应大鹅。
- 槽位七格，三同类消除；提供撤回、颠锅、提示和冻结。暂停、失败重试、返回首页均接入。
- 音乐、音效、震动设置和图鉴写入本机 PlayerPrefs。编辑器无手机震动效果。
- 每日种子按北京时间日期 + 主题 + 阶段生成。重试保持本次日期和阶段；不重新抽关。
- PVP 默认连接 `https://goose-3d-pvp-197372-5-1312757046.sh.run.tcloudbase.com`；独立服务在 `/Users/liuzhenhua/Desktop/code/crazyCombo/goose-server`，`npm start` 启动。双方真实客户端匹配或房间号加入后准备。

## 可换皮与调参

`Resources/Replica/ThemeLibrary.asset` 配置主题名、prefab 池、关卡层、音乐和音效。层内 weights 留空为等权，填写时长度须等于 prefab 池且均为正数。每层数量必须是 3 的倍数，且足够容纳各类型至少一组。

`Resources/GooseArt` 是本次临时使用的原版 UI、图鉴图片和音频；`asset-provenance.json` 记录来源。UI 在 `Scripts/ReplicaFlow.cs` 构建，图片按资源名引用，后续可替换同名资源；对象模型已换为原版乡村、小卖部、烧烤模型。

本版复现功能流程并复用原素材。UI 排版由代码重建，并非原版 prefab 的完整还原；三套主题已接入原版物品池。第二关数量与难度曲线为本版可调配置，并非已证明完全等同于原游戏。原版动态难度算法实现尚未恢复。

## 验证

`Goose > Build Replica` 重建场景及默认主题配置（会重置 ThemeLibrary 的手工调参，谨慎使用）。编辑器测试桥通过 Temp/goose-command.txt 接收 test、play、stop、pvptest、home、tutorial、hard、themes、settings、capture:绝对路径。

单机检查 test 需要停止 Play；结果在 Temp/goose-validation.txt。PVP 检查先 Play，启动服务和 `node tools/editor-peer.mjs`，然后 pvptest；结果在 Temp/goose-network-validation.txt。独立 Node peer 仅用于自动测试，不进入正常匹配逻辑。

当前客户端为编辑器联调版，未打包手机、小游戏、WebGL；PVP 已部署。ClientWebSocket 后续须按发布平台选择适配方式；中文字体当前取本机系统字体，发布时需换成字体资产。PVP 采用临时游客会话、单实例内存房间，未接入现有账号与持久化战绩。红包未纳入本版。

## 原版 3D 模型（2026-10-13）

OriginalModels 中导入 71 个原版 prefab、85 个 mesh、102 个材质和 95 张贴图。当前使用 64 个物品：乡村 21、小卖部 18、烧烤 25；7 个 Legacy/Old 物品保留但不进入默认生成池。

层级、UV、局部变换、基本碰撞体从安装包恢复；移除原版脚本，使用本项目 Rigidbody 和三消逻辑。所有物品统一最大尺寸约 0.43，确保适配现有锅和槽位。原版自定义 Shader 的表现由兼容 Shader 重建，保留 BaseColor/AO/颜色参数，不保证光照与原版完全一致。

资源来源见 OriginalModels/provenance.json。Tools/export_goose_models.py 从指定 bundle 目录导出中间数据；菜单 Goose > Import Original 3D Items 重建网格、材质、prefab 并接入三套主题。运行 Goose > Build Replica 会重置主题池，此后应再运行 Import Original 3D Items。

ThemeLibrary.asset 的 pvpEndpoint 是公网默认地址；要调试本机服务，可在好友对战界面填写 http://127.0.0.1:8097。公网验证与部署记录在本任务 outputs/goose-replica。

## 分层生成（2026-10-15）

第二关采用全局三消预算、动态权重跨层逐件分配、满层回退及上宽下窄的物理容器。各层容量为 150 / 60 / 45 / 45，初选种类为 20 / 10 / 8 / 7（模型不足时限制为实际池大小）。详细参数、复刻边界和验证入口见 `Tools/LayeredPile/README.md`。

尺寸与显现修订：第二关恢复各原始 prefab 的相对尺寸，使用早餐铺高度范围作为共享参考，门槛参数 4/5/21/.5，渐显时长 .75 秒；具体适配和未还原项见上述说明的“尺寸与显现修订”。
