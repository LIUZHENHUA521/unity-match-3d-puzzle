# 五套食材主题

主题与稳定标识：田园菜篮 `pot-vegetable`、肉肉盛宴 `pot-meat`、元气超市 `pot-grocery`、缤纷果园 `pot-fruit`、甜蜜小屋 `pot-sweets`。每套 26 个模型，原有三套主题仍为 23 个。

## 资源与复现

- 用户提供的五个购买包原样保存在 `SourceAssets/PotFoodPacks/`，`manifest.json` 记录 SHA256、模型顺序、中文名称和面数。不修改原始美术资源名称。
- `python3 Tools/prepare_pot_food_packs.py SourceAssets/PotFoodPacks` 提取静态网格和共享 atlas；移除源包展示用平移，转换 glTF 手性、三角形绕序及 UV 原点。遇到不支持的层级/变换直接拒绝。
- Unity 执行 `PotFoodPackImporter.Build` 生成可读原生 Mesh、共享材质、独立适配 Prefab、组合 Body/Pick 碰撞以及入槽姿态。再次导入保留已存资源 GUID。
- 每套共享 3 张 atlas，导入上限 1024；原始贴图与 GLB 保留。全部模型合计 26,209 三角面。运行时不解析 GLB。
- 新主题沿用当前锅、背景与关卡机制；默认第二关第一层覆盖全部 26 个模型，第一关仍为 15 个。持久化物品索引沿用源包顺序，后续不要重排。
- 模型资源目录统一 `PotFoodPacks`，适配 ID 为 `Pot_<pack>_<model>`。历史接口、存档、数据集合、桥接协议保持兼容。

## 接入与验证

Unity `ModelThemes.asset` 共 8 套；测试选择器改为双列，8 套主题及“跟随每日关卡”均可显示。旧主题重建工具保留新增主题。

后台复用原“每日关卡”页面及权限。前后端按所选主题校验类型数量（旧主题 23、新主题 26），开局快照也按该主题校验索引；旧主题不接受 23–25 索引。后台 WebGL 播放器使用版本目录 `20261117-pot-food-themes`。不修改任何实际日期排期。

验证入口：`PotFoodPackValidation.Run`，包含原生资源检查、5 套主题第一关三消、300 个第二关物品、130 个模型入槽、撤回重选、26 类型覆盖、每日同题快照还原。`PotFoodPackPreviews.Build` 生成全部模型联系图。输出保存在 `work/pot-food-packs/`。

## 2026-11-17 检查结果

- Unity Play Mode：714 项断言通过，五套主题全部通过。
- 原有 ModelAdapterCatalog 条目逐项与 HEAD 比较一致，新增恰好 130 条。
- 后台云函数测试 53/53、服务端测试 174/174；五份实际 Unity 开局也通过后台 validateConfig。
- 管理后台生产构建通过；包含 26 类型校验的 Unity WebGL 构建通过。
- 浏览器本地试玩被工具的源站安全策略拒绝（127.0.0.1）；未绕过，未声称浏览器实测通过。
- 本次未打包上传微信版本，也未修改实际日期排期。
