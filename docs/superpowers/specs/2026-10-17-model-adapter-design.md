# 一键模型适配工具设计

## 目标

为 `Assets/GooseReplica/IncomingModels` 下的新模型提供一键适配入口。工具不修改现有关卡生成和运行时消除逻辑，而是把模型转换为可用于抓大鹅玩法的标准物品 prefab，并生成可审阅的 JSON 报告和浏览器预览。

适配结果需要回答四个问题：

1. 模型的视觉尺寸是否在当前物品空间内。
2. 模型原点、朝向和视觉包围盒中心是否合理。
3. 是否存在可用于拾取和实体碰撞的碰撞体，碰撞体是否落在模型范围内。
4. 是否能得到稳定且不与现有关卡冲突的逻辑 ID。

## 入口与输出

- 输入目录：`Assets/GooseReplica/IncomingModels`。
- 支持 Unity 已导入的 prefab；FBX/OBJ 由 Unity 先导入为模型资源后再选择。
- Editor 菜单：`Goose/Model Adapter`。
- 输出目录：`Assets/GooseReplica/Generated/ModelAdapter/<logicalId>/`。
- 输出内容：适配后的 prefab、检查报告 JSON、预览数据 JSON，以及必要的 `SourceSizes` 和 `SourceColliders` 草稿数据。
- 工具默认不覆盖原文件、不自动加入主题或关卡；用户在报告通过后再手动接入主题资源。

## 适配流程

1. 扫描输入模型的 Renderer、MeshFilter/SkinnedMeshRenderer、Transform 层级、Rigidbody 和 Collider。
2. 根据所有可见 Renderer 的包围盒计算尺寸、中心、最长边和底部高度。
3. 生成一个适配副本：保留模型层级和材质，创建标准根节点，复用已有碰撞体；缺少碰撞体时不默默生成可用于正式关卡的碰撞体，而是标记为 `NeedsCollider`，并允许用户在预览中一键生成保守的 BoxCollider 草稿。
4. 按当前导入器使用的标准最长边进行视觉归一化，记录 `normalizedScale`，但不把该值直接写回原始模型。
5. 按逻辑 ID 规则生成 ID：优先读取用户填写的 ID，其次使用合法化后的文件名；同名 ID 不覆盖已有资源，而是标记冲突并要求改名。
6. 输出分级检查：`Pass`、`Warning`、`Error`。只有错误清零后才允许“生成适配 prefab”。
7. 生成浏览器预览数据并打开本地 HTML：显示模型、视觉包围盒、原点、拾取碰撞体、实体碰撞体、尺寸数值和检查结果。

## 逻辑 ID 与现有资源的关系

逻辑 ID 独立于显示名称。适配器会在报告中列出：输入路径、逻辑 ID、显示名称、目标主题和是否已经存在 `SourceSizes`/`SourceColliders`/音效映射。

如果输入模型声明了现有逻辑 ID，工具只允许在用户明确选择“更新已有资源”后覆盖草稿；默认模式只生成新 ID。这样换皮不会意外改变已有关卡数据。

## 检查规则

- **尺寸**：最长边必须大于零；建议范围以当前 `OriginalModelImporter` 的标准尺寸为基准，超出范围只警告，不强行裁剪。
- **原点**：视觉中心与根节点局部原点的偏移、底部距离和非单位缩放分别报告；模型倒置、NaN/Infinity 或极端缩放为错误。
- **渲染**：至少一个启用的 Renderer；材质槽不为空；SkinnedMeshRenderer 必须有有效骨骼引用。
- **碰撞**：至少一个启用 Collider；如果要接入正式玩法，必须同时存在拾取和实体碰撞语义。工具会检查 Collider 是否位于视觉包围盒附近、是否有明显零尺寸、是否使用不支持的 MeshCollider 配置。
- **物理**：适配根节点存在 Rigidbody 时保留；否则生成预览副本时补齐默认 Rigidbody 草稿，报告会明确标记为自动生成。
- **ID**：只允许字母、数字、下划线和短横线；与已有逻辑 ID 冲突为错误。

## 浏览器预览

预览是离线本地 HTML，不依赖网络或游戏运行时。页面包含：

- 3D 模型查看器，可旋转、缩放、重置视角。
- 视觉包围盒和原点标记。
- 拾取碰撞体、实体碰撞体的颜色区分。
- 实际归一化尺寸、原点偏移、Collider 数量和逻辑 ID。
- 检查结果筛选和“重新生成报告”提示。

预览数据只描述模型和诊断结果，不执行 Unity 物理，也不替代真机验证。

## 错误处理与安全边界

- 输入模型无法加载、Renderer 全部禁用、层级含循环或尺寸非法时停止生成并保留诊断报告。
- 不删除源模型，不自动修改 `ThemeLibrary`、`SourceLevelCatalog` 或已存在的关卡数据。
- 适配器生成的碰撞体标记为草稿，用户确认后才写入正式 `SourceColliders` 配置。
- 所有资产写入完成后调用 `AssetDatabase.SaveAssets` 和 `AssetDatabase.Refresh`，并在报告中记录输出路径。

## 验证方式

1. 使用现有简单模型验证：无碰撞体、BoxCollider、多个子节点 Collider、SkinnedMeshRenderer 各一例。
2. 验证重复 ID、空 Renderer、倒置原点、零尺寸 Collider 等错误报告。
3. 打开浏览器预览，确认包围盒、原点和碰撞体可视化与报告数值一致。
4. 生成适配 prefab 后运行现有 Editor 验证工具，确认原有模型导入、对象池和关卡流程没有回归。
