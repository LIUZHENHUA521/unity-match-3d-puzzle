# 抓大鹅机制移植：当前入口

当前核对范围是**单人第二关，乡村、小卖部、烧烤三个主题**。来源是微信 2.1.12 包内代码/配置及 iOS 1.11.03 资源；不代表当前线上热更新值，也不等于整款游戏完全一致。未发布。

最新 [整体复核与选中物理对齐](selection-remaining-alignment.md) 覆盖选中碰撞/自然唤醒、默认重显门闩、手动高度记录及鹅/工具时序纠正。

前轮发光描边、鹅动画、三道具与音效复核见 [feedback-animation-tools-alignment.md](feedback-animation-tools-alignment.md)。随机序列、分帧创建、飞槽复核见 [sequence-creation-flight-alignment.md](sequence-creation-flight-alignment.md)；整体范围及剩余差异见 [overall-alignment-audit.md](overall-alignment-audit.md)。 对象池、槽位 UI 与结算边界见 [settlement-objectpool-slot-alignment.md](settlement-objectpool-slot-alignment.md)。

- [生成与显隐复核](audit-generation.md)：原表类型池/权重、组预算、跨层单件分配、初始显示与下层首次显现。
- [交互复核](audit-interaction.md)：单射线、按住换选、松手取当前目标、撤回碰撞体状态。
- [物理与资源复核](audit-physics.md)：源刚体/形状、微信与 iOS 交叉检查、新增鹅的骨架和实体支撑。
- [物品 Toon 与环境光](item-toon-ambient-alignment.md)、[镜头与阴影](camera-shadow-alignment.md)、[主题场景](visual-camera-alignment.md)：上轮完成项及平台验证边界。

## 运行配置

`Assets/GooseReplica/Resources/Replica/SourceLevelConfig.json` 保存第二关原表；`SourceLevelCatalog.asset` 绑定实际 prefab。第一关与 PVP 保留 `ThemeLibrary.asset` 原有索引。

三主题四层容量都是 150 / 60 / 45 / 45，高度 14 / 18 / 20 / 22。乡村/小卖部初选 22 / 10 / 8 / 7，烧烤 21 / 10 / 8 / 7。生成位置使用各主题原离散矩形框及高度抽样，真实物理会继续改变位置；不是四块硬隔板，也不是理想圆锥。

尺寸滑杆为用户要求保留的扩展；1.00× 才是资源原尺寸。全部 300 个实例在开局分帧创建，下层显示不会新增物品。开局可见物品用独立的创建增长循环，每 .02 游戏秒增加 .1 尺度；隐藏物品保留实体支撑。后续普通隐藏→显示保持原尺度；.75 秒 OutQuad 分支受默认关闭的 `_changeObjScale` 门闩控制，不会自动执行。

普通第二关使用 Unity 全局随机流，不再按每日 seed 重置；类型/分层/坐标/旋转按源调用顺序消耗。描述按 Y 升序创建，无额外随机洗牌。飞槽使用已提取包 Awake 配置确定的旧动画分支，固定 .3 秒，三消先释放逻辑槽位再完成视觉动画。

## 验证入口

`VisualSceneBatch.Run()` 在隔离的 Unity 项目快照里导入并运行生成、交互、全物品渲染、材质、骨架/碰撞、场景、光照、镜头、阴影和显隐测试。使用 `Tools/wechat/build.py --prepare-only` 只准备快照，不发布。验证报告存于 [overall-validation.txt](overall-validation.txt)。

旧实现、已删除的额外保护以及历次参数记录见 [implementation-history.md](implementation-history.md)，其中过时数值不可再用于解释当前行为。
