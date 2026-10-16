# 结算、对象池与槽位剩余差异复核

更新时间：2026-10-16

本轮只记录能从已提取的微信 WASM / 方法表 / 已恢复资源直接证明的行为，未发布。

## 结算入口

微信方法表确认以下调用链：

- `GameSceneMa.SetGetGoose (j39869)` 是胜利结算的抓鹅入口。
- `UIWinPop.GooseLoadCompelet (j30249)` 在鹅资源加载完成后调用 `SetGetGoose`。
- `ObjectUnit.WinAnim (j36721)` 只播放 `Settlement_Struggle` 状态，使用 `0.2` 秒淡入并从 `0` 秒开始；WASM 没有停止既有 `RandomAnim` 协程的调用。
- `LevelManager.EndCombineObjAnim (j19872)` 在每批三消动画收尾后调用 `CheckLevelState (j19881)`，所以判胜检查点在三消回调，而不是任意点击后立即检查。

项目已恢复 `ReplicaSourceGooseAnimation.PlaySettlement` 的 `WinAnim` 语义，并保持随机动作协程；当前项目没有原版 `GameSceneMa` 的主鹅场景、手部模型及 `UIWinPop` 资源绑定，因此结果弹窗仍是项目 UI。没有把现有 2D 弹窗宣称为完整抓鹅场景。

## 对象池

方法表确认原版 `ObjectPoolMa` 的接口：`GetObjByName (j5150)`、`GetObj (j5812)`、`RemoveObj (j2538)`、`CleanAllUnActiveObj (j80103)`、`CleanUnActiveObjList (j7960)`；`LevelManager.CreateUnitObj (j13230)` 和多个工具/粒子路径会调用按名称取用或移除。

项目现已将第二关主物品按 prefab 名称接入 `ReplicaSourceObjectPool`，回收时停止 tween、清速度、冻结刚体、关闭碰撞和渲染，再放入 inactive 桶；下次按原 prefab 名称取出并恢复基础缩放。第一关/PVP 仍保留原有实例路径，避免跨模式状态污染。

工具粒子和暂存底板由 `ReplicaSourceToolVisual` 使用同样的按名称池：粒子 `Stop + Clear` 后禁用，按 prefab 名称复用；`StoreContentBG` 隐藏后留在池内，销毁组件时再清理。验证只针对短生命周期 VFX，不推断原版池的预热数量或容量。

## 槽位 UI

`LevelManager.RefreshContentDisplay (j9879)` 按内容列表索引逐项更新入槽/重排位置和旋转；`SetStoreContentPos (j9876)` 使用 Content 底板与方向变换计算暂存位置。旧分支的飞槽时间、曲线、三消释放时序已按资源配置恢复。

项目 `ReplicaTray` 继续使用七个独立槽格和安全区适配的 world-space Canvas，并把飞行目标投影到源 Content 深度；这保证当前竖屏布局下的可见落点和碰撞深度一致，但没有声称复制原版 Content 预制体层级或未恢复的新动画分支。

## 验证

`SourceToolVisualValidation` 继续覆盖三个工具粒子层级、十个 native ParticleSystem、缩放/时钟、暂停行为和 Home 清理，并新增：

- 三个暂存返回特效结束后按名称进入池；
- 两个自动凑齐光束结束后进入池；
- 清理流程不留下 active 特效或可见暂存底板。

完整主鹅场景、手部演出、结算 UI 资源加载和原版 Content Canvas 的逐像素布局仍是明确边界；本轮未发布体验版。

本轮 Unity 编译通过。整套运行批次仍报告既有的 `SourceToolVisualValidation` 主题 1 可见候选失败；该失败发生在分层显示验证，当前没有把它归因到结算或池化，也没有把整套批次标成全绿。
