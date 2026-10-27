# 第一关初始化碰撞复核（2026-10-28）

## 确认并修复

第一关 `ReplicaFlow.Spawn(sourceTutorial=true)` 在创建原版主题边界时，只通过 `ReplicaThemePresentation.Begin` 关闭旧锅 Renderer，没有关闭旧锅 Collider。旧场景包含 40 个 `Pot containment` BoxCollider 和 1 个 `Cooking Pot` MeshCollider。物品 BodyLayer=10 忽略围挡 Layer=2，但允许与锅体 Layer=0 碰撞。因此不能将 40 个围挡都称作实际接触源；旧锅体仍然提供了原版主题之外的不可见碰撞表面。

现在 `ReplicaThemePresentation` 统一保存并关闭旧锅及子节点的 Collider（含 inactive 节点），退出时逐个恢复原状态。第二关不再重复保存/恢复同一组 Collider，第一关到第二关复用同一个 presentation 时，旧碰撞体持续禁用。

回归发现：`ReplicaLayeredPile.Clear` 原先保留 presentation 引用。在后续第一关复用组件后，再次进入第二关时 Begin 内的 Clear 可能结束当前活跃 presentation。现在 Clear 释放该引用，避免旧所有者关闭下一轮场景。

## 尚未对齐，纠正此前“完全对齐”的表述

- 原版 WX 2.1.12 `wechat-LevelData.json` MainLevel 第一关（index 1/4/5）有 `_wintype=[1]`、`_wincount=[3]` 和排除类型 1 的 `_cancreatetype`。当前 `GetTutorial` + `GenerateSource` 将完整模型列表作为普通候选，没有独立 WinType/WinCount 流程。最终总数、目标物品位置和教学分支须继续追踪代码，不能仅凭字段推断。
- 当前第一关同步创建全部物品，位置与旋转交替抽样、立即开启刚体，再等待 3 秒；没有走第二关已有的描述排序、分帧创建与生长流程。没有证据证明这 3 秒是原版第一关行为。
- `GetTutorial.limitHeight=4` 已写入配置，但第一关路径没有消费该字段来建立显隐阈值。写入配置不等于已实现对应行为。
- 第一关仍用 `[0,6]` 采样；已有普通生成流程实现使用 1.5 作为底部起点。第一关是否走同一路径仍须核实，不在此次碰撞修复中假定。
- `UsesSourceFlight`、音效绑定和鹅动画初始化仍主要限制在 Stage 2，第一关仍有旧交互分支。

本次不改随机预算、计时或位置范围，避免把尚未核实的推断一起作为“原版修复”。修复仅针对确定的旧碰撞体泄漏和场景所有权问题，未发布。

## 验证

`LegacyBoundaryValidation.Run`：隔离 Unity 项目 Play Mode 验证三个主题的旧 Collider 禁用、原版边界启用、第一关到第二关的组件/边界交接、混合启用状态恢复、重复清理与重开。无需截图渲染。具体结果见 `legacy-boundary-validation.txt`。该检查覆盖组件生命周期，不代表已经复现用户设备上的全部初始碰撞轨迹。
