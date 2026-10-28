# 分帧创建与开局时序对齐（2026-10-16）

## 2026-10-28：修复开局全隐藏与额外等待

- `CreateBoardItem` 把出生描述的高度传给 `ReplicaLayeredPile.Register`，由该高度决定初始显隐。对象池模板停放在 Y=-1000，关闭自动 Transform 同步时不能立即读取其克隆刚体的旧位置作为出生高度。上层物品现在正确进入从零比例增长分支。
- `RunSourceCreation` 每批只等待原有 `WaitForSeconds`，移除其后的额外 `yield return null`；保留首批 1 件、其后 3 件、末批 2 件，以及 .5/.2/.3 秒尾部等待。
- 当前增长仍由 `LateUpdate` 累计 .02 秒后推进 .1；下文旧说明中“增长也由独立协程驱动”不再代表当前实现。没有补帧追赶循环，不保证不同设备逐帧等价。
- 创建测试已改为从 `Begin` 直接进入第二关，沿真实对象池路径检查尚未物理同步时的初始显隐，以及真实帧中的提前显现、逐步长大、暂停、取消、重开和完成后才开放操作。
- 编辑器实测三个主题在创建到 175 / 172 / 160 件时首次出现可见物品，约 1.95 / 2.10 / 2.12 秒；完整创建约 4.79 / 5.35 / 4.78 秒。三主题验证通过。这是编辑器观测，不是 iOS 性能承诺，也不是与原版真机的等时对照。
- 报告：`work/ios-performance-audit/creation-alignment-validation.txt`。本次未构建或发布。

> 2026-10-16 门闩复核纠正：本文旧行为/旧通过记录中的“后续显示默认 .75 秒长大”结论失效。普通 WX 对象的 `_changeObjScale` 为非序列化 private bool，未发现开启路径，默认不执行重显长大；开局创建增长仍保留。最新证据与实现见 [reveal-growth-gate-alignment.md](reveal-growth-gate-alignment.md)。

范围：本项目三个主题的基础第二关、普通非强制教程创建路径；先不发布。证据为微信 2.1.12 WASM 控制流、iOS 1.11.03 保存的 LevelManager 数值；不是抓取线上当前运行状态。原函数和 SHA-256 位于 [Evidence/frame-creation](Evidence/frame-creation)。

## 恢复的执行顺序

1. `LoopCreateObj / j65137` 先展开所有出生描述：层从底到顶，每层按已分配列表顺序采样位置。每件先高度抽签、再 X/Z。`_layerMinDistance` 保存值为 0，第一层下界 1.5。
2. 非 `_IsInForceTeach` 分支调用 `List.Sort`；比较器 `j12039` 比较描述 Y 升序，不是随机洗牌。旋转在后续 `CreateUnitObj / j13230` 里才抽，按排序后的创建顺序消耗同一随机流。
3. 普通参数为 `_level2CreateCompeletTime=2`、`_frameMaxCreateCount=3`、`_maxCreateInterval=.03`。等待量 `min(2 / 描述数量 * 3, .03)`；300 件时是 .02 秒。
4. `j65137` 约 2710 行以 **零基 i % 3 == 0** 判定 yield。因此首轮是第 0 件后等待，再每次 3 件；300 件的最后批次为 2 件。并非每轮固定先生成 3 件。100 次 .02 秒等待是计划值，Unity WaitForSeconds 按实际帧恢复，不能把其解释成任意设备必定 2 秒完工。
5. 全部实例完成后继续等待 .5 秒，停止 `LoopCrowObjSize`，等待 .2 秒，再等待 .3 秒，然后所有对象 `FinishCreate(true)` 强制恢复完整比例。特殊真实物理分支在 .2 秒后恢复重力；本项目不启用保存值为 false 的该分支。
6. `FinishCreate` 后检查堆高。源协程等待所属游戏 UI 存在，再 `GameManager.SetGameState(gaming)`，启动 `LoopOptimization` 和 `CountDownTime`。本项目自己的 HUD 已同步存在，完成创建后才切 PLAY；倒计时和输入不会在未创建完时启动。

## 纠正上一轮“开局直接完整大小”的结论

上一轮只追了 `ObjectUnit.Init → SetVisible → FinishCreate`，遗漏 `CreateUnitObj` 中 **Init 之前的 `_growUpInCreate` 分支**。

- `LevelManager..ctor / j83392` offset552 将 `_growUpInCreate` 置 true；该字段未包含在保存的序列化字段列表中。对 LevelManager 方法中此偏移的直接写扫描只找到该构造函数，仍不把这等同于排除了所有外部运行时覆盖。
- `CreateUnitObj / j13230`：初始可见描述先设比例 0；初始隐藏描述设比例 1。然后 Init、SetVisible。
- `LoopCrowObjSize / j65133` 每个 `TimerManager.WaitFor0_02Second` 周期：比例 X < .99 时加 `Vector3.one * .1`。这不是按秒插值，也不是 .75 秒 OutQuad。达到完整比例后，下一次该循环才进行 `CheckRigid`。
- `Init / j19263` 调 `j6539(true)`；解析其 icall 字符串证实为 **Rigidbody.set_isKinematic(true)**，并不是激活动态物理。增长中的物品保持初始运动学状态；达到完整比例后的高度检查才决定解除。
- 创建循环的 `CheckRigid` depth 参数是 1；开局完成后的正常优化循环仍是 .5。原“刚注册就按普通门槛启用动态刚体”已移除。
- 后续 `DelayToGrowScale` 内虽有 .75 秒 OutQuad，但 `_changeObjScale` 默认 false 且未发现开启路径，普通隐藏→显示不执行。该门闩与开局已开启的创建增长不同。

实现使用真正的 Unity 协程：实例化协程保留每个原等待，初始增长循环另用 WaitForSeconds(.02)。Home/重启取消当前创建及增长，旧 iterator 也受版本检查保护，离场不继续生成。

## 验证接口

- `SourceCreationValidation.Run()` 驱动生产 `SourceCreationSteps` 同一 iterator，检查三个主题批次、Y 顺序、起始运动学状态、初始增长与高度冻结、尾部等待及完成后才 PLAY。
- `TakeOverSourceCreationForValidation()` 仅 Editor 可用；停止 coroutine wrapper 并返回已经 yield 的原 iterator，Current 是首个等待秒数。旧几何测试使用这个步骤器完成 fixture，没有另写同步生成器。
- 主任务另设 `SourceCreationFramesValidation` 在真实 Unity 帧运行，覆盖暂停、取消、重启和正常完成。统一批处理结果由主任务填写；本文不把尚未执行的测试算作通过。

## 尚未宣称对齐

- 上述参数来自保存场景，与微信线上分支开关/热更新是否相同未做运行时观测。`_isUseRealPhysicSpace=true` 下的 -50 重力、70 件窗口及3.5秒策略不是普通路径，本轮没有启用。
- 源加载资源、教程、活动 UI 和在线模式分支未移植到本项目自己的页面流程。
- 已追加核对额外物理唤醒增长：`HideRigid / j4679` 虽有 `!LevelManager.IsMoreVisible && _isFinishCreate` 时调用 DelayToGrowScale 的分支，但 `LevelManager..cctor / j83393` 将 IsMoreVisible 置 true，Awake/Start 的相关路径未覆盖它。当前普通路径保留 SetVisible 增长，不误启用此分支；这仍不是线上所有外部覆盖的运行时排除。对应完整静态扫描见随机审计。
- 源全局随机还可能被其他系统消耗；同一个关卡种子逐件跨游戏一致不能凭局部随机接口宣称。详见随机序列审计。
