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
- 输出内容：适配后的 prefab、检查报告 JSON、预览图像/预览数据，以及写入持久 `ModelAdapterCatalog` 的尺寸和碰撞配置。
- 工具默认不覆盖原文件、不自动加入主题或关卡；用户在报告通过后再手动接入主题资源。

## 适配流程

1. 扫描输入模型的 Renderer、MeshFilter/SkinnedMeshRenderer、Transform 层级、Rigidbody、Collider、Tag 和 Layer。
2. 根据所有可见 Renderer 的包围盒计算尺寸、中心、最长边和底部高度。
3. 生成一个适配副本：根节点命名为逻辑 ID并带 `Collectable` tag、Rigidbody 和 `ReplicaSourceIdentity`；第一个子节点固定为 `Visual`，其下放置原始模型，保证运行时 `ReplicaSourceColliders` 的路径约定不变。复用已有碰撞体；缺少碰撞体时标记为 `NeedsCollider`，只有用户明确点击“生成碰撞草稿”后才创建保守 BoxCollider。
4. 按当前导入器使用的标准最长边 `.43` 进行视觉归一化，采用 `Visual/Geometry` 子节点承载中心偏移，让 `Visual.localPosition` 保持为零，避免运行时恢复 pivot 时错位。记录 `normalizedScale`，但不修改原始模型。
5. 按逻辑 ID规则生成 ID：优先读取用户填写的 ID，其次使用合法化后的文件名；运行时 key 使用逻辑 ID，显示名称保存在 `ReplicaSourceIdentity` 和报告中。同名 ID不覆盖已有资源，而是标记冲突并要求用户选择更新。
6. 输出分级检查：`Pass`、`Warning`、`Error`。只有错误清零后才允许“生成适配 prefab”。
7. 生成浏览器预览数据并打开本地 HTML：显示模型、视觉包围盒、原点、拾取碰撞体、实体碰撞体、尺寸数值和检查结果。
8. 校验并补齐根 Rigidbody：`mass=.15`、`drag=.6`、`angularDrag=.8`、Interpolate、ContinuousDynamic、`isKinematic=false`、`detectCollisions=true`；上述设置写入生成 prefab，而不只存在预览副本。

## 逻辑 ID 与现有资源的关系

逻辑 ID独立于显示名称。适配器会在报告中列出：输入路径、逻辑 ID、显示名称、目标主题和是否已经存在 `SourceSizes`/`SourceColliders`/`SourceAudioItems` 映射。为了兼容当前运行时按 `prefab.name` 查询的实现，生成 prefab 的文件名和根节点名使用逻辑 ID；显示名称不参与查表。音效条目也以逻辑 ID作为 `SourceAudioItems.Item.name`，没有映射时报告 Warning，但不会阻止纯视觉换皮生成。

如果输入模型声明了现有逻辑 ID，工具只允许在用户明确选择“更新已有资源”后覆盖适配目录中的 prefab、尺寸、碰撞和音效映射；默认模式只生成新 ID。适配器通过单独的 `ModelAdapterCatalog.json` 保存自身条目，包含 sizes、colliders、audioName 和 prefabPath；运行时在现有 `SourceSizes.json`/`SourceColliders.json`/`SourceAudioItems.json` 之后加载该目录；旧的全量导入器即使重写原始 JSON，也不会删除适配器条目。接入主题或关卡仍需用户在报告通过后确认。

## 检查规则

- **尺寸**：最长边必须大于零；以 `.43` 归一化基准计算 `normalizedScale`。根节点缩放必须是有限的统一缩放；非均匀根缩放为错误，内部节点的非均匀缩放允许但会参与世界空间碰撞检查。
- **原点**：报告视觉中心、底部高度、根节点到视觉中心偏移、坐标轴朝向和 Visual/Geometry 两级路径；倒置、NaN/Infinity、极端缩放为错误。工具不猜测“正面”，提供 up-axis/forward-axis 字段供用户确认。
- **渲染**：至少一个启用 Renderer；每个材质槽非空且 Shader 对当前构建可用；SkinnedMeshRenderer 必须有有效骨骼引用，预览前用 `BakeMesh` 获取稳定 bounds。
- **碰撞**：至少一个启用的拾取 Collider 和一个实体 Collider。配置写入现有 schema：`Profile.items[].name` 使用逻辑 ID，`col`/`realCol` 保存相对 `Visual` 的节点路径，节点保存 `layer`（拾取 7、实体 10）、`enabled`、`trigger`、形状参数和 PhysicMaterial。拾取 Collider 必须集中在 `Pick` 子树，实体 Collider 必须集中在 `Body` 子树，使现有 `SetVisible()` 不会漏隐藏；只支持 Box/Sphere/Capsule；动态 Rigidbody 上的 MeshCollider 必须是 convex，否则为错误；零尺寸、超出视觉包围盒容差或路径不存在为错误。
- **物理**：生成 prefab 根必须有 `Collectable` tag、Rigidbody、`isKinematic=false`、`detectCollisions=true`、ContinuousDynamic、Interpolate；碰撞体层必须是 PickLayer 7 或 BodyLayer 10，且报告当前物理层矩阵和材质。
- **ID**：只允许字母、数字、下划线和短横线；与已有逻辑 ID冲突为错误；大小写按不区分大小写处理。

## 浏览器预览

预览是离线本地 HTML，不依赖网络或浏览器加载 Unity 资源。Unity 编辑器先用 `PreviewRenderUtility` 输出模型静态姿态、拾取碰撞体和实体碰撞体的 PNG/SVG 叠加图，再由 HTML 展示这些本地文件和诊断数据；这样不需要 glTF、CORS 或外部 CDN。页面包含：

- 模型渲染图和正交/侧视/俯视三张预览图，可切换查看。
- 视觉包围盒和原点标记。
- 拾取碰撞体、实体碰撞体的颜色区分。
- 实际归一化尺寸、原点偏移、Collider 数量和逻辑 ID。
- 检查结果筛选和“重新生成报告”提示。

预览数据只描述模型和诊断结果，不执行 Unity 物理，也不替代真机验证。HTML 通过 `Application.OpenURL` 打开生成目录下的 `index.html`。

## 错误处理与安全边界

- 输入模型无法加载、Renderer 全部禁用、层级含循环或尺寸非法时停止生成并保留诊断报告。
- 不删除源模型，不自动修改 `ThemeLibrary`、`SourceLevelCatalog` 或已存在的关卡数据。
- 适配器生成的碰撞体标记为草稿，用户确认后才写入 `ModelAdapterCatalog.json` 的正式条目；原有全局 JSON 不直接被覆盖。
- 资产写入采用 staging 目录：先写临时 prefab、报告和预览，全部检查通过后原子替换目标目录；失败时删除 staging，不留下半成品。
- 所有资产写入完成后调用 `AssetDatabase.SaveAssets` 和 `AssetDatabase.Refresh`，并在报告中记录输出路径。

## 验证方式

1. 使用现有简单模型验证：无碰撞体、BoxCollider、多个子节点 Collider、SkinnedMeshRenderer 各一例。
2. 验证重复 ID、空 Renderer、倒置原点、零尺寸 Collider 等错误报告。
3. 打开浏览器预览，确认包围盒、原点和碰撞体可视化与报告数值一致。
4. 运行适配器专用验证：报告、prefab、`ModelAdapterCatalog`、运行时 key、拾取/实体碰撞路径逐项一致，并在编辑器物理场景中执行一次 raycast 和实体碰撞检查。
5. 生成适配 prefab 后运行现有 Editor 验证工具，确认原有模型导入、对象池和关卡流程没有回归。
