# 主题物品编辑器

用户已批准在 Unity 内新增 Goose / 主题物品编辑器，查看缩略图、添加、替换、移除物品、编辑相对权重，并从 Model Adapter 跳转。

## 数据与范围

运行时实际读取 ThemeLibrary.sourceLevelCatalog 中按 sourceEnvironment 对应的 LevelLayoutConfig.Profile。旧 ThemeEntry.prefabs 不做修改。编辑器选择主题后加载实际 Profile 为独立草稿，保存同时更新 SourceLevelConfig.json 对应 environment 的 names、prefabPaths、weights、tutorialNames 和 layers.types；保留 JSON 的其他字段与其他主题。当前源格式仅支持每物品一组全层共享权重；遇到各层不同权重时拒绝加载并说明，避免悄悄丢失数据。

## 操作

列表显示缩略图、Prefab、第一关角色、相对数量权重、替换和移除。候选区从适配目录中搜索已生成的 Prefab 并添加；也可以拖入 Prefab。拒绝原始 FBX、场景实例、重复名称、缺失物理组件或未登记且无旧版碰撞配置的模型。替换时保留权重及第一关角色，移除第一关物品后必须重新选择目标与至少两种普通候选。新增权重为 1。权重解释为选中该类型后的数量预算比例，不保证抽中概率。

第一关目标单选，普通候选可多选且与目标互斥。至少一个目标、两个普通候选。删除后各层类型数仅下调到池大小，上限还须符合每层三消预算；新增不自动提高难度。层数、数量、时间、高度保持原设置。

## 保存与保护

草稿可放弃；切换主题和关闭窗口保护未保存修改，编译/Play 模式禁止保存。保存前验证引用、名称与运行时 ID 一致性、权重正数有限、第一关完整性、层预算有效。检测源文件与配置在编辑期间的外部变化，拒绝覆盖并提示重新加载。写入前保留两文件备份；写入失败恢复原数据。备份放在工作目录 work/theme-items-backups，成功提示备份位置和下次开局生效。不修改运行中局面，不主动换掉现有 23 件素材。

## 验证与实施顺序

1. 实现独立数据服务：草稿、验证、JSON 保留字段更新、双文件保存/回滚与备份。
2. 实现编辑窗口与 Model Adapter 跳转。
3. 临时隔离资产验证：替换第一关引用、添加/删除权重对齐、类型数裁剪、重复和非法权重拒绝、JSON 未知字段保留、外部冲突拒绝、保存后重导入一致性。
4. Unity 实际编译并打开窗口，确认当前 23 件正确展示且未改正式配置。

## 审查补充

- 带 BoardItemIdentity 的 Prefab 仅允许 ItemCatalog 内同 ID、同 prefabPath 且碰撞路径可解析的模型；不得借用旧版同名碰撞记录。没有 Identity 的模型才校验 SourceColliders 分支。
- 当前导入器写入固定 SourceLevelCatalog.asset。编辑窗口若发现 ThemeLibrary 指向非默认 catalog，则拒绝编辑并提示，避免保存与重导入指向不同资产；测试服务可使用独立临时副本。
