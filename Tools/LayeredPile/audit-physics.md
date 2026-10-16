# 整体复核：物理、碰撞、场景边界与尺寸（2026-10-16）

本轮逐项检查现有实现，并追加跨版本资源逐字段比对。原有物理数值没有发现新的确证偏差；主流程查出遗漏 12 个特殊鹅模型后，本分项已补齐它们的原始碰撞 profile。未构建、上传或发布。

## 已有证据与当前实现相符

|范围|本轮复核结果|证据|
|---|---|---|
|普通模式重力|`-19.81`；未启用真实物理创建分支的 `-50`|WX PhysicsManager；LoopCreateObj 条件分支|
|物品刚体|mass `.05`、drag `0`、angularDrag `.05`、Discrete、None 插值|43 个微信物品刚体；当前 `ReplicaSourcePhysics.Apply`|
|全局/实例参数|sleep `.2`、contact `.02`、最大脱离穿透速度 `1`、bounceThreshold `100`、solver `6/1`|WX PhysicsManager；当前 Begin/Apply/End|
|物理步长/最大角速度|`.02` / `7`，项目保存设置已相符|WX TimeManager/PhysicsManager 与 ProjectSettings|
|默认接触材质|动/静摩擦 `.6/.6`、弹性 `0`、combine 枚举 `1/1`；有显式材质的保留|WX 默认 ObjMa；当前 DefaultSurface/碰撞体 Apply|
|碰撞分层|点击层 7 不参与实体碰撞；实体层 10 与 0/6/10 碰撞|WX 32 行碰撞矩阵；ConfigureLayers|
|高度冻结|低于物理门槛、已长大且速度不大于 `.2` 才冻结；无 16 活动上限或 2 秒截断|CheckRigid j5869、HideRigid j4679；当前 RefreshHeightStates|
|三主题边界|保存的碰撞树与 Resources 环境 JSON 逐字段相同|下表与 assets-comparison.json|

三主题不是共用一个圆锅。当前乡村使用 80 个碰撞树节点、33 个 BoxCollider；小卖部与烧烤摊各 31 个节点、10 个 BoxCollider。节点数包含保留层级，不等于围墙边数。第一关/对战专用子树在导出时已排除；运行时统一映射 Wall 层 6 是本项目拾取层兼容处理。

|主题|空气墙截面边数|生成框顶部 X/Z scale|实际单轴随机范围|
|---|---:|---:|---|
|乡村|8|5.85140085|框中心 ± scale × .45|
|小卖部|4|7.10527277|框中心 ± scale × .45|
|烧烤摊|4|7.52322960|框中心 ± scale × .45|

## 本轮新增交叉证据

此前碰撞形状主要来自 iOS 1.11.03，物理行为来自微信 2.1.12，因此本轮直接读取已下载的三个微信物品包逐字段比较：

- **43 个微信物品 profile 与当前 iOS 来源的同名 profile 完全相同**：包括 `col` / `realCol` 层级索引、各节点 layer、Box/Sphere/Capsule 形状与尺寸、启用/trigger 标志、显式物理材质。
- **同批 43 个物品的 299 个层级节点变换也相同**：名称、父子顺序、active、局部位置/旋转/缩放均无差异。
- **71 个已导入模型的尺寸逆归一化均正确**：prefab 视觉根 scale × `SourceSizes.normalizedToSource` = 原始视觉根 scale，最大数值误差约 `4.44e-16`（离线双精度计算）。
- **三个主题的碰撞/出生框资源与已保存的微信导出 JSON 相同**。

这里是包资源/静态计算证据，没有将其说成已观察到线上实时状态。43 个微信物品之外的另 28 个导入物品，仍只依据已取得的 iOS 来源；不能把新增交叉验证范围写成全部 71 个都已微信验证。

可复核文件：[碰撞对照](Evidence/physics-audit/wx-ios-collider-comparison.json)、[变换/尺寸/场景对照](Evidence/physics-audit/assets-comparison.json)。脚本一并保存在同目录，使用已有 research `work/` 原包及 UnityPy，不需重新抓包。

## 仍存在的差异与边界

1. **尺寸滑杆是用户要求保留的适配。** 单人第二关 `100%` 才对应原始 prefab 尺寸；`75%` 是每轴缩为 `.75`，碰撞体随同缩小，体积约变为 `.422`。质量仍 `.05`，不会自动按体积重算。已有 PlayerPrefs 值不会被复核偷偷重置。因此实玩时仍可能不是 100%，需以设置面板为准。原始 prefab 保存尺寸与默认普通生成分支可核对，不代表已排除所有线上模式改尺寸的分支。
2. **第一关/PVP 不在完整二关还原路径内。** 它们仍使用项目原有排布、旧锅与统一 `BoardItemScale=1.5`（相对于 `.43` 归一化尺寸），不能把“二关三主题已对齐”概括为整个游戏所有模式已还原。
3. **邻域主动唤醒与颠锅力仍是项目实现。** `WakeNear` 的半径 `.9`、高度差 `.35` 和 `ApplyShake` 的向心 `.3`、上抬 `1.9`、随机扭矩 `2` 不应被描述为已经找到了同值原版证据。撤回落点也仍属项目逻辑。本分项没有用猜测值替换这些值。
4. **场景变体仍未完整恢复。** 使用完整 GroceryStore，尚未核实原版何时选 GroceryStoreSimple；没有凭设备性能自行启用变体。
5. **物理引擎与平台结果不能保证逐帧相同。** 当前移植使用项目 Unity/PhysX 环境，原版是对应包版本/平台的 Unity 运行时。相同参数、碰撞形状与调度不保证随机堆叠轨迹和逐帧接触解算一致；尚无同种子、同输入的双端实机轨迹对照。
6. **跨版本证据范围仍需注明。** 当前三个主题环境与行为取微信 2.1.12；完整物品库主要取 iOS 1.11.03。新增的 43 个一致性验证缩小了证据缺口，没有消除全部平台/版本差异。

## 本轮补齐：12 个特殊鹅的碰撞

主流程在 iOS 1.11.03 原包中发现先前漏导出的 12 个 Goose 物品。本分项已向 SourceColliders.json 追加它们，profile 总数由 71 变为 83；原有 71 项和三个主题边界均保留。

新增的 Farmer、SteelCup、Watermelon、Tomato、ChiliSauce、ColaCup、Egg、Noodles、Pancake、Eggplant、GreenPepper、Potato 共包含 156 个原始碰撞形状。每个 profile 按 ObjectUnit 的真实 col/realCol 引用定位点击/支撑层级，保留部件形状、层、启用/trigger 与材质；没有把复杂鹅模型缩成一个包围盒。它们的刚体也是 mass .05、drag 0、angularDrag .05、Discrete，适用已有物理参数。

证据：[新增碰撞 profile](Evidence/physics-audit/goose-collider-profiles-ios.json)、[12 个刚体保存数据](Evidence/physics-audit/goose-rigidbodies-ios.json)。新增 SourceGooseColliderValidation 逐个检查导入 prefab 与 profile 的层级/形状/材质、点击显隐不关闭实体支撑。Unity 是否通过以主流程本轮批处理记录为准，当前报告不预先宣称执行结果。上文 71 个尺寸逆归一化验证指新增前样本，新 12 个尺寸由本轮模型导入与汇总验证另行覆盖。


### 特殊鹅身体与骨骼补齐

新增 12 个鹅的身体使用 SkinnedMeshRenderer；此前只处理 MeshRenderer 的导出器会漏掉鹅身体，只剩帽子、食品等附件。本轮修复通用导出/导入，保留身体网格、每顶点骨骼权重、bindposes、骨架完整层级，以及精确 bones/rootBone 引用；恢复 renderer 的本地剔除范围与相关设置。不是用一个静态配件代替鹅。

[骨骼导出自检记录](Evidence/physics-audit/goose-skin-bindings.json) 验证所有骨骼引用可解析、顶点与权重数量相符、权重和约等于 1。SourceItemMaterialsValidation 进一步比较导入网格权重/bindposes 与原包数据；SourceGooseColliderValidation 进行 BakeMesh 非空检查。**Animator 状态机与动画片段尚未还原，现在保留资源静止姿态，不宣称原版鹅的动作已经恢复。**

同时发现材质导入 DTO 使用 enableInstancing，而导出字段是 enableInstancingVariants，导致丢失原实例化开关；本轮已修正，127 份材质中 114 份源开关为 true。保存的 lightmapFlags 全为 4，现显式写回；disabledShaderPasses 与 stringTagMap 在全部记录中为空，没有额外猜测处理。

导入验证还暴露了原骨架层级超过 JsonUtility 深度上限 10 的问题，导致深层节点数组被截断。本轮改用已有版本 Newtonsoft JSON（3.2.1）读取完整模型树，明确允许深度 256；导入、材质/骨骼验证、渲染验证与尺寸导入共用该解析器。由于独立验证快照会移除 UnityMCP，现将原本仅由 MCP 间接引入的同版 JSON 包声明为直接依赖，避免验证快照缺程序集。没有缩短源骨架来绕过报错。
