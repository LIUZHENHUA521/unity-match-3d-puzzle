# 萌趣玩具替换验证（2026-11-09）

用户批准复用已有 23 种玩具，替换当前食物池。使用独立 Toy* 适配 prefab；原玩具和食物资源保留。

- Unity 2022.3.62f3c1 实机编辑器验证：198 条断言通过。
- 23 个模型材质及拾取/实体碰撞有效；第一关三消通关；第二关 300 个玩具正常生成。
- 全部 23 种玩具完成入槽验证，小熊额外验证撤回及重新选择。
- SourceLevelConfig 除 names/prefabPaths/tutorialNames 外的内容与迁移前完全一致；全部旧 ModelAdapterCatalog 条目保持一致。
- 原始玩具 192–2660 三角面；未更改其共享网格、贴图或材质。
- 修改仅涉及新模型适配资源、物品池引用和新模型入槽/大小配置；没有修改运行时游戏逻辑、UI、场景或音效资源。
- 模型外形变化会改变局部堆叠形态，未声称难度完全相同；未构建或发布微信包。

截图：`work/toy-theme-20261109/first-stage.png`、`second-stage.png`、`tray.png`。
完整迁移前配置备份：`work/toy-theme-20261109/before/`。
映射清单：`mapping.json`；结果：`result.txt`。

编辑器验证入口：Play 完成初始化后向 `Temp/toy-theme-command` 写入 `validate`。验证运行期间临时禁用 InputController 的用户输入，完成后恢复；建议验证期间不要触发重新编译或切换 Play 状态。
