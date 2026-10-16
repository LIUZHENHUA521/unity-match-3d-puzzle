# 鹅动画 / 分层生命周期复核账本

范围：ObjectUnit.Init / SetVisible / BeClick / StoreObj / ThrowBack，RandomAnim 状态机，LayeredPile 创建完成、显示和洗牌期间的物理优化。先静态复核；除随后明确授权的动画 helper、动画验证与报告纠正外，不改其他生产模块，不运行 Unity、不发布。

## A. 已确认且已修 helper：notBeCatch 解释反向

**影响：真实行为差异，高优先级；也会通过定时随机调用影响后续全局随机序列。**

原字段 offset16 的写入 / 读取：

| 源方法 | 字段值/要求 |
| --- | --- |
| Init j19263 | 写 true |
| BeClick j12985 | 写 false |
| StoreObj j19259 | 写 true |
| ThrowAnim.MoveNext j64881 | 先等 0.3 秒，然后写 true |
| CanBeSelect j80063 | 要求 `notBeCatch && !isHiding && _isFinishCreate` |

所以含义是“尚未被拿取”，不是“禁止拿取”。RandomAnim j65003 的 true 分支等 5–15 秒，PlayAnimClip j7956 选 enum3→Level_InBox1/2；false 分支等 1–5 秒，选 enum4→Level_Outside。Init 后在锅里与 Store 后暂存都走 true；点击进槽后走 false。

此前 helper 默认 false、EnterTray=true，完全反向；旧测试仅核对了被误读的标签，因此 PASS 不足以证明行为正确。本轮已纠正 `ReplicaSourceGooseAnimation` 的默认值、EnterTray、EnterStore、ReturnToPile 和 NotBeCatch 命名；新增测试直接推进生产 RandomAnimation IEnumerator，检验两分支实际片段和跨 yield 捕获的分支。

集成要求：InputController.MoveSourceItemsToStore 中改调 `EnterStore()`，普通 SourcePick 仍调 `EnterTray()`。不得把存储和点击复用为同一状态。

## B. 已确认：隐藏鹅由自动找物收取时漏一次可见性动画重启

**影响：最终 Idle 相位和全局 RNG 顺序。**

原 BeClick j12985 顺序是 `SetVisible(true)` → `notBeCatch=false` → `PlayAnim(true)`。如果先前隐藏，SetVisible j6602 的 renderer 状态变化会先调用一次 PlayAnim(true)，此时字段还为 true，因此先抽 Idle 相位和 5–15 秒等待；末尾再抽 Idle 相位和 1–5 秒等待。

复核时 `ReplicaLayeredPile.Detach` 直接启用 renderer/collider，没有调用动画可见性接口，只有 SourcePick.EnterTray 重启一次。自动找物没有过滤隐藏对象，可以走到这个分支。建议 Detach 在移除 entry 前通知 `SetSourceVisible(true)`；helper 自身可见性门控能避免给原本已显示的鹅多抽随机数。

## C. 不应套用到默认分支：位置洗牌的可见性移植

`<MessObjPosAnim>d__288.MoveNext` j65103 确实保存 `CreateInfo.isVisible`，随后 `SetVisible(targetInfo.isVisible)` 并把可见者缩放置零。但本轮道具负责人确认当前默认分支是 `<MessObjRotaAnim>d__292.MoveNext` j65101。重新提取 j65101 后确认：**没有 j6602 SetVisible 调用、没有零缩放调用；末尾才全量恢复 scale=one、j9885 CheckVisible、j5869 CheckRigid。**

因此此条只是非默认分支差异，不能给当前默认旋转洗牌新增按 target visibility 显隐/从零长大。虽都使用 CreateInfo 结构，也不能假定默认分支使用了所有字段。此前中间消息提出的默认移植建议已经撤回，主任务与道具负责人均已收到纠正。

方法映射同时确认：j9885=LevelManager.CheckVisible，j5869=LevelManager.CheckRigid；两者名字不能反写。

## D. 条件分支：暂停优化扫描不等于暂停已经开始的长大 tween

**后续门闩复核：普通对象 `_changeObjScale=false`，不会启动这个 tween。以下只针对显式开启门闩的休眠分支，不是普通关卡默认行为，见 [独立核查](reveal-growth-gate-alignment.md)。**

`ObjectUnit.DelayToGrowScale` j19265 创建独立 `DOScale(1,.75)`，没有把它绑到优化协程；`LoopOptimization` j65121 在 LevelManager offset60=true 时跳过检查但仍等待下一轮。两种洗牌 j65101/j65103 都设置 offset60=true，暂停的是这类优化/交互检查，不是 Time.timeScale；没有全局 KillAll/PauseAll 动作。

复核时 LayeredPile.StepVisibility 因 Shuffling 在最开头 return，连正在进行的 reveal growth 也停止。应让已存在的 growth 按缩放游戏时间继续，再跳过高度/刚体/可见性扫描。洗牌候选自身可有源程序明确的缩放重置，不应由这个扫描门控扩大到其他物品。

## E. 兼容撤销入口漏重置动画状态（非原版第一道具）

普通第二关按钮已经用 Store 替代 Undo，但 InputController.UndoLast 仍被调试/消息和现有验证调用。其飞回后重注册 LayeredPile，并没有调用 ReturnToPile；原先收取状态可能保留。若保留这个项目兼容入口，应在返回完成后调用 ReturnToPile（true），不要把此兼容动画飞回描述为原 ThrowBack（源先等 .3 秒再加力）。

## F. 已修 helper：WinAnim 不取消既有 RandomAnim

原 WinAnim j36721 只播放 Settlement_Struggle 并设时间0，没有 StopCoroutine。原helper额外StopRoutine，已移除；新测试要求该入口保留既有 PendingWait。此入口目前尚未接入完整胜利抓鹅表演，不把资源入口修正宣称为整段UI演出完成。

## 已核对但不新增修改

- 原 CanBeSelect 确实检查 `_isFinishCreate`，不能因名称误读而取消成长期间禁选。
- j1387 由完整方法表确认为 Transform.get_localScale；j35148 是 Rigidbody.get_velocity；j6539 是 Rigidbody.set_isKinematic。CheckRigid j5869 以实际 localScale.x 是否约等于1决定深层物品冻结，和创建/后续长大的阶段耦合是真实来源。
- 原 SetVisible 只在主 renderer 或 skin renderer 实际改变 enabled 时重新播放动画；现 helper 的同值可见性门控对正常生命周期是需要的。
- Store 的 isStore=true 会阻止 SetVisible(false) / SetRigid(true)。现有暂存物品从 LayeredPile/物理管理集合移除，正常高度循环不会再隐藏或唤醒它们。
- 8 个原片段曲线、骨骼、蒙皮内容不重复重建；本次问题在状态与调用链，不在资源曲线。
- 本范围不含选中物体的 get_up/forward 和加力方向；已明确交由选择物理负责人。

## 证据与验证状态

[源状态表](Evidence/goose-animation-lifecycle/source-state-table.json)，同目录保存本次使用的 WAT 摘录；原方法完整映射来自研究目录 `work/wechat-all-method-map.json`，字段偏移来自 `work/wechat-field-offsets.json`。

本复核不启动 Unity。动画 helper 的新分支验证与其他集成修复由主任务统一运行后记录；不要沿用上一轮反向生命周期断言的 PASS。
