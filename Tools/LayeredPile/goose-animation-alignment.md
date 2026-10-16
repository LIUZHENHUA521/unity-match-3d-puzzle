# 鹅骨骼动画：原包证据与本轮恢复

本轮范围：现有三个主题的 12 个 Goose_* 物品，恢复原始骨骼片段与游戏中锅内、隐藏/显示、拿入槽、返回堆的播放逻辑。未发布。

## 原版结构并不是预设 Animator 状态机

iOS `goose-1.11.03.ipa` 的鹅 prefab 在 `…/Goose/e` 或 `…/e` 上含 Animator，`m_Controller=0`、`m_Avatar=0`、`m_UpdateMode=0`、`m_CullingMode=0`、`m_ApplyRootMotion=false`；旁边的 MonoBehaviour 指向该 Animator，字段为 Animancer 的 `_Animator`、`_ActionOnDisable`。ObjectUnit `_anim` 指向这个 AnimancerComponent。

微信 WASM `ObjectUnit.PlayAnim`（j7955）、`PlayAnimClip`（j7956）调用 `AnimancerComponent.Play`（j3101）、`AnimancerState.set_Time`（j2185）；没有凭空恢复一个 AnimatorController。实现使用 Unity Playables 在原骨骼上播放原始 AnimationClip，保留相同的无 Avatar、无 Root Motion 结构。

## 实际恢复的资源

从 iOS 两个 `anim_assets` bundle 提取 8 个鹅片段：

| 片段 | 秒 | Loop | 用途 |
| --- | ---: | --- | --- |
| Level_Idle | 1.3333334 | 是 | 关卡待机 |
| Level_Outside | 1.3333334 | 是 | 飞槽/槽内间歇动作（notBeCatch=false） |
| Level_InBox1 | 1.3333334 | 否 | 锅内/暂存间歇动作变体 1（notBeCatch=true） |
| Level_InBox2 | 1.3333334 | 否 | 锅内/暂存间歇动作变体 2（notBeCatch=true） |
| Settlement_Struggle | 0.33333334 | 是 | 原 WinAnim 使用的挣扎片段 |
| MainScene_Idle | 4.8000002 | 是 | 原主页待机，资源已恢复 |
| MainScene_Yell | 2.4000001 | 否 | 原主页鸣叫，资源已恢复 |
| MainScene_Dance | 10.400001 | 否 | 原主页舞蹈，资源已恢复 |

每个片段 147 个 Transform 浮点通道，共 1,176 通道、19,914 个关键帧。不是根据视频手工模拟动作：从 `m_StreamedClip` 的三次多项式系数恢复值、进出切线，从 `m_ConstantClip` 恢复常量通道；这些片段没有 dense 通道。每段在五个内部采样点反验 Hermite 插值，最大误差 < 1.4e-7。所有骨骼路径通过原 `genericBindings.path` CRC32 与真实骨骼层级对应。

另一个 `HandCatch` 绑定的是手模型，未错误套到鹅骨骼。8 个鹅片段的 `m_Events` 全部为空，不能凭片段名称加上未经证实的音效事件。

## 游戏中的播放时序

证据：`j19263` Init、`j7955` PlayAnim、`j7956` PlayAnimClip、`j65003` RandomAnim.MoveNext、`j6602` SetVisible、`j12985` BeClick、`j19259` StoreObj。

1. **字段语义纠正**：`notBeCatch` 不是“禁止拿取”，而是“尚未被拿取”。Init j19263 写 true，BeClick j12985 写 false，StoreObj j19259 写 true；CanBeSelect j80063 也要求它为 true。这四条写入/读取共同确认含义，不能根据字段英文猜反。
2. Init 调用 PlayAnim(true)：播放 Level_Idle，将播放时间置为 `Random.Range(0, clip.length)`。锅内 `notBeCatch=true`：等待 `Random.Range(5f,15f)` 秒，随机选 Level_InBox1/2，以 0.2 秒淡入从时间 0 播放；等待 `clip.length * Random.Range(1f,3f)` 后，以 0.4 秒淡入随机相位的 Level_Idle。
3. 普通拿取 BeClick 将 `notBeCatch=false`，重启 Idle；此后等待 `Random.Range(1f,5f)` 秒，动作选 Level_Outside（列表只有 1 个片段，原版仍调用整数 Range）。移出槽并暂存 StoreObj 则写 true，回到 5–15 秒 / InBox1/2 分支。暂存与普通收取不可复用同一个 flag 更新。RandomAnim 在 yield 前选分支，该次等待期间字段变化不会改变已进入的分支；下一次循环才重新判断。
4. 隐藏：停止动画状态和 RandomAnim；实际再次显示时重启 Idle 随机相位与等待。每次高度扫描但可见性不变时，不重复抽随机数。
5. 混合使用源 Animancer 的默认 FixedSpeed。`j8462/j44658` 显示首次空层直接播放；后续目标仍有权重时，淡入时长按 `duration * abs(1-currentWeight)` 缩短。`j6079` 的进度为线性权重变化。
6. Animator/RandomAnim 都使用缩放游戏时间；暂停游戏时也暂停。
7. ThrowBack 的协程先等 0.3 秒，再设置 notBeCatch=true、恢复物理、加力并 PlayAnim(true)：返回后也是 5–15 秒 / InBox 分支。当前 ReturnToPile 是项目原撤回按钮的兼容入口，恢复 true；SetNotBeCatch 可同步源字段，不能再以反义的 CatchBlocked 命名。
8. ObjectUnit.OnCombine 本身没有调用另一个鹅骨骼动作；不附加自造“消除扑腾”。

微信 `data.unity3d` 的 level0/pathID 762 是 840 字节的 AnimatorMa。Odin 字典 `_animList` 解出 AnimType 0…7 的数量为 `1,1,1,2,1,1,1,1`。`j95667 StartLoad` 的多片段分支按 index 从 0 开始、拼接枚举名称与 `index+1` 加载，因此 InBox1/2 并非按名字猜的额外动画。原异步加载完成顺序、运行时在线资源覆盖仍不等于静态包证据。

## 代码与验证

- `SourceGooseAnimationImporter`：生成原曲线 `.anim`，为 12 个现有 prefab 补回原 Animator 根和播放 helper；不重建模型、材质或碰撞体。
- `ReplicaSourceGooseAnimation`：等价 Playables 运行层，连接原有创建/显示/拿取生命周期。
- `SourceGooseAnimationValidation`：逐关键帧比较值/时间/切线、loop/长度、全部骨骼路径；给每只鹅采样 Level_Outside，验证骨骼变化且 BakeMesh 顶点真正变形；Play 模式检查随机相位、隐藏重启、锅内/槽内/暂存等待和恢复；并驱动真实 RandomAnimation IEnumerator 穿过 yield，直接核对 true→InBox、false→Outside 和等待期间捕获分支。
- 原资源验证已通过：8 clips / 1,176 通道 / 19,914 关键帧、12 个骨骼运动、12 个蒙皮变形。先前生命周期测试也跟随了被误读的语义，因此不能把旧 PASS 当作该语义正确的证据；本轮已纠正生产分支与断言，并增加真实协程分支检查，新的统一 Unity 验证结果由主任务记录。

## 证据边界

动画控制逻辑是已提取的微信 WASM；曲线与 12 只鹅资源来自 iOS 1.11.03。微信基础 `data.unity3d` 本身没有 AnimationClip，不能声称已经逐字节比对两端下载动画资源。跨 Unity 版本的最终蒙皮像素仍需真机对比。

主页舞台、胜利抓鹅展示、对应手模型和舞蹈随音乐节拍属于单独场景。这里恢复了其鹅片段和 PlaySettlement 入口，但当前项目未复刻这些完整场景编排，不将“片段已导入”说成所有主页/结算表演已经完成。

原始 clip typetree、SHA256、数量/误差与方法摘录见 [Evidence/goose-animation](Evidence/goose-animation/inventory.json)。
