# 物理参数与主题场景对齐（2026-10-15）

> 2026-10-16 门闩复核纠正：本文旧行为/旧通过记录中的“后续显示默认 .75 秒长大”结论失效。普通 WX 对象的 `_changeObjScale` 为非序列化 private bool，未发现开启路径，默认不执行重显长大；开局创建增长仍保留。最新证据与实现见 [reveal-growth-gate-alignment.md](reveal-growth-gate-alignment.md)。

本次已修改本地实现并完成独立 Unity Play Mode 批处理验证。没有构建、上传或发布体验版。

## 物理参数

单人关卡（含第一关和第二关）进入时启用来源配置，退出/重开/切换模式/销毁游戏流程时恢复进入前的全局值。PVP不启用本轮物理配置。自制模型也采用同一玩法物理参数，但其模型/碰撞形状仍保留项目版本。

|参数|此前项目值|当前值|证据|
|---|---:|---:|---|
|重力Y|-9.81|-19.81|微信2.1.12 PhysicsManager|
|质量|.15|.05|微信三主题物品prefab，43个刚体交叉核对；已有iOS71个刚体同值|
|线性阻力|.6|0|同上|
|角阻力|.8|.05|同上|
|碰撞检测|ContinuousDynamic|Discrete|同上|
|刚体插值|Interpolate|None|同上|
|休眠阈值|.005|.2|微信PhysicsManager；同时写入实例|
|接触偏移|.01|.02|微信PhysicsManager；同时写入碰撞体|
|最大脱离穿透速度|原Unity默认|1|微信PhysicsManager；同时写入实例|
|弹跳阈值|2|100|微信PhysicsManager|
|位置/速度求解迭代|6/1|6/1|保持与微信一致|

微信全局默认物理材质指向ObjMa：动/静摩擦均.6、弹性0、两种combine枚举均为1。对没有显式材质的碰撞体现在也应用该默认材质；带显式材质的保留原资源配置。固定物理步长的来源保存值为.02，本项目已一致，本轮未改变。

重力-50不能直接拿来替换普通模式：WX LoopCreateObj(j65137)的两次重力写入都受_isUseRealPhysicSpace控制，分别读取_gravitySpeed的第二/第一分量；相关保存场景值是(-19.81,-50)，开关为false。新实现对齐普通模式的-19.81，不启用特殊真实物理创建分支。开关值的保存场景证据来自之前iOS提取，不声称已经抓取微信线上运行值。直接调用扫描未找到mass/drag/angularDrag等setter的调用，不将该扫描结果等同于排除了所有间接或内联覆盖。

实现：ReplicaSourcePhysics.cs；ReplicaFlow负责进入、实例应用与离开恢复。原先没有活动上限/计时强制冻结的高度调度保持不变。

## 三个主题不再共用早餐铺碰撞空间

按ThemeLibrary每项显式指定sourceEnvironment：daily→CountrysideStove、toys→GroceryStore、travel→Barbecue。toys/travel是项目原有ID，没有改动每日种子和主题身份。

|主题|二关围墙截面边数|wallWidthSize|生成框baseSize|最高保存框X/Z Scale|
|---|---:|---:|---:|---:|
|乡村|8|.65|.70|5.85140085|
|小卖部|4|1.50|.85|7.10527277|
|烧烤摊|4|1.50|.90|7.52322960|

上述Scale不是半径；实际单轴采样半范围仍乘.45。乡村与烧烤摊最高保存框世界Y≈23.82111，Z≈-2.92486；小卖部为Y≈23.84572，Z≈-2.71688，来源层级实际倾角不同。各主题均保存9个框与changeAngleHeight=[12]。分别恢复世界中心、局部scale及空气墙/底部的真实BoxCollider层级；乡村还带有多块锅底碰撞和共享底面，不能用早餐铺一块底面替代。

导出按LevelBGItem的level1Obj/customCol引用排除第一关及对战专用碰撞树，保留level2Obj与共享有效碰撞树，保留碰撞体启用/触发器标记。本轮资源里的物理碰撞体均可用BoxCollider表示。运行时依旧映射到Wall层6以配合项目拾取射线；原资源材质参数保留。

小卖部同包还有GroceryStoreSimple变体。本项目明确使用完整GroceryStore，不声称已恢复原版按设备/模式选择Simple的分支。主题视觉背景、镜头、锅外观没有替换；这次替换的是玩法空间和碰撞配置，仍可能与项目当前锅外观不完全贴合。

## 验证与边界

physics-theme-validation.txt记录所有通过结果：
- 三主题各自场景资源和最高框宽度；300个物品的质量、阻力、碰撞模式、接触偏移。
- 单独自由落体1个.02秒步：速度变化与-19.81重力一致，且没有线性阻尼；每个主题退出后全局设置完整恢复。
- 原有生成、显隐和动画回归：10,000坐标、300件不增生、1秒检查、.75秒OutQuad、隐藏时动画继续、只长大一次、24件可动态、无2秒冻结、点击反馈和300ms入槽。
- 三主题各3秒模拟无物品越出测试场景边界、拾取网格可点到物品。不是长时间完整通关，也不是手机性能结果。

仍未宣称完全一致：System.Random和分组预算移植、线上运行时特殊分支、Simple场景切换、主题视觉及镜头、长大期间点选锁、邻近唤醒和颠锅/撤回等项目逻辑。没有为追求低耗时重新加活动上限或强制冻结。

## 可复核证据

任务目录outputs/goose-analysis/physics-theme-evidence包含：
- physics-settings-and-bodies.json：微信PhysicsManager、TimeManager、默认材质，以及43个微信物品刚体原始数据。
- CountrysideStove/GroceryStore/Barbecue.json与对应-source-settings.json：提取的碰撞/生成框及原始LevelBGItem、空气墙构建参数。
- theme-comparison.json：主题差异。
- gravity-branch-evidence.txt：原WASM条件分支与字段对应。

可复现导出脚本：work/export_theme_environments.py。资源取自已取得的微信2.1.12目录和游戏公开CDN；下载地址记录work/theme-downloads.json，相关包名由wechat-catalog.json定位。小卖部通过Addressables依赖bucket6776定位到levelscenemultiprefab_assets_part2_085cb43d2b98b55e05d4e5c7e8a36824.bundle，非凭名称猜包。
