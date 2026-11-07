# 主题物品编辑器实施计划

已批准设计：../specs/2026-11-07-theme-items-editor-design.md。

1. ThemeItemsEditorService：独立草稿与行角色；引用合法性、第一关与层约束验证；实际配置和 JSON 同步保存；外部冲突检测、备份、异常回滚；不改旧主题池。
2. ThemeItemsEditorWindow：主题选择、缩略图、替换/删除、第一关角色、权重；模型候选搜索与添加、Prefab 拖入；保存/放弃保护、禁用 Play 修改；ModelAdapterWindow 跳转。
3. ThemeItemsEditorValidation：用临时配置副本验证正常保存、非法输入、引用跟随替换、减池层数裁剪、源配置再导入一致、未知字段保留和外部冲突。
4. 使用当前 Unity 编译和测试菜单执行验证；打开主题窗口确认 23 项。比较正式配置的哈希，避免测试污染。

当前可用技能目录未提供 writing-plans 技能和专用审查提示模板，采用上述明确步骤和只读设计审查完成规划。
