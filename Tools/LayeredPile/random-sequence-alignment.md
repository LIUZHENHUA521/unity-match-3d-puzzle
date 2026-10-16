# 随机调用链与创建排序对齐（2026-10-16）

范围：保留的微信 2.1.12 WASM 普通第二关路径。没有发布。证据原函数摘录、源文件 SHA-256 和 icall 字符串地址见 [Evidence/random-sequence/manifest.json](Evidence/random-sequence/manifest.json)。

## 查明的区别与修改

1. **类型初选也是浮点权重抽签。** `j38517` 第 590–715 行建立 `KFakeRandom`，逐项 `AddItem(1,type)`；第 917–946 行每次 `Roll` 后 `RemoveItem`。所以即使等权，原版也没有调用整数 `System.Random.Next(count)`。`j83882` 136–139 行调用 `j1345(0, sum)`，并以累计值严格 `> roll` 命中；`j1345` 在内存地址 1576193 解析出的 icall 是 `UnityEngine.Random::Range(System.Single,System.Single)`。现在源路径按这一调用链抽签，旧第一关路径保持原实现。

2. **原版不会在每层分配后 Fisher–Yates 打乱。** `j65137` 第 1590–1835 行按层索引从 0 递增、按层分配列表原顺序生成坐标描述。第 2298 行 `List.Sort` 的比较器是 `j12039`，读取描述 `offset16`（位置 Y）调用 `j2594`，后者明确是浮点 `CompareTo`；是升序高度排序，并不抽随机数。排序受 `!LevelManager._IsInForceTeach` 条件控制。现在 `GenerateSource` 保留分配插入顺序；完整坐标列表排序由分帧创建流程执行。

3. **普通第二关恢复原版全局随机流。** 生产路径使用 `ReplicaSourceGlobalRandom`，直接调用 `UnityEngine.Random.Range(float,float)`；不设置 seed，不保存/恢复状态，也不隔离其他系统的随机消耗。`ReplicaFlow` 将同一接口传给计划、描述坐标和逐件创建旋转；在各帧之间发生的其他 Unity 抽样会自然推进共享流。原项目 daily seed 不再驱动普通第二关。`ReplicaSourceRandom(seed)` 的独立状态版本仅保留给验证和显式重放，第一关/PVP仍走原项目路径。

4. **高度和横向抽样恢复原运算顺序。** `j16590` 权重相同直接 `Range(min,max)`；不同则 `r=Range(0,(w0+w1)*.5)`，`a=(r+r)/(w1-w0)`，`b=(w0+w0)/(w1-w0)`，结果 `min+(max-min)*(sqrt(a+b*.25*b)-b*.5)`，全部 float32。此前 double 的代数等价式可能改变末位，不再用于源路径。跨区间 `j29517` 先按 `(w0+w1)*heightSpan` 选择区间，再调用上述方法。`j13228` 然后按 X、Z 顺序分别 `Range(-1,1)`，乘边界尺度和 .45。

5. **旋转范围保持原样。** `j13230` 64–94 行按 X、Y、Z 顺序三次 `Range(0,1)`，每项乘 360；不是直接 `Range(0,360)`。旋转发生在按 Y 排序后的逐件创建阶段，不能与之前按分配顺序生成坐标交错。

## 仍然不能声称逐局完全一致的部分

- 原包原生 RNG 内部实现和原会话完整 `Random.State` 尚未恢复。现在是当前 Unity 运行时真实 float Range，不能仅凭同名 API 宣称与原包各平台、各 Unity 版本逐位相同。
- 原版进关前的全局随机消耗、当天线上配置、对象 Awake/动画或其他协程并发抽样还没有完整轨迹。本项目普通第二关已取消每日 seed 驱动；同一天重开不再保证同一个布局。显式重放 seed 只是测试入口，不是恢复出的原版 seed。
- 普通第二关现在按原版使用全局流，但本项目其他系统与原版的抽样调用仍可能不同，不能保证复制原版其他系统插入的随机消耗，也不能根据原版线上某局重放相同局面。
- 相同 Y 时原版 `List.Sort` 不是稳定排序；这里使用同一类排序入口，不额外加入 id tie-breaker。跨 .NET/IL2CPP 版本相同 Y 的排序细节未做原设备逐件轨迹比对。

## 验证

新增 `SourceRandomValidation.Run()`：生产全局包装与外部直接抽样交错执行120次，验证和原生Unity直接抽样逐项相等且推进同一个全局状态、不重置不隔离；另对显式重放版本120次逐项对比当前 Unity 原生 `Random.Range`，检查构造/每次抽样/整条计划与坐标流程均不污染外部全局状态；用可控随机接口断言类型初选 float 范围、6 件计划恰好 8 次抽样而没有额外 shuffle、分配顺序、同 seed 连续计划→坐标→旋转重放、旋转三次 0–1 范围。结果由主任务统一 Unity 批处理记录；本文件不把未运行验证写成通过。

旧纯 Mono 的计划测试不再足以验证 Unity 序列。可以用 `IReplicaSourceRandom` 注入测试替身测纯算法，但不可把替身结果作为原生随机序列证据。

## 追加只读复核：IsMoreVisible 与解冻长大

`j4679 / ObjectUnit.HideRigid` 的确还包含一条长大入口：刚体状态真正切为动态后，只有 `!LevelManager.IsMoreVisible && _isFinishCreate` 才进入 `j19265 / DelayToGrowScale`。这不意味着普通关卡应开启该入口。

已追到的启动链：

- `LevelManager..cctor / j83393` 第 16–20 行将静态 offset4 的 `IsMoreVisible` 写为 true。
- 完整 `Awake / j83431` 覆盖静态 offset81 的 `IsUseNewAnimManager`，数据来自 NetInit 实例 offset156；它没有覆盖 offset4。
- `Start / j83398` 及 `<Start>b__85_0 / j83395` 没有写入该字段。
- 微信启动场景保存的 WXNetInit 配置完整解码结果位于 [保存配置](Evidence/random-sequence/net-init-wx.json)（研究输出 `outputs/goose-analysis/flight-timing/net-init-wx.json`）；其中 `isUseNewAnim=false` 影响的是 offset81；完整字段列表中没有 `IsMoreVisible` 或长大开关。
- 对 main/sub WAT 中引用唯一 LevelManager TypeInfo 句柄 3434312 且存在 `store8 offset=4` 的所有函数进行了候选写入扫描（[记录](Evidence/random-sequence/is-more-visible-write-scan.json)）。逐项追溯地址，只有 cctor 是该字段的写入；其余是 GameManager 等其他类。TypeInfo 原始编码 `0x200074f9` 在保留主内存只有该一个对齐地址；静态指针后加4的候选也未发现。

因此，按本包普通三主题路径，`IsMoreVisible=true`，解冻长大分支被条件排除；本轮不应为了补齐代码分支而打开这项效果。此结论是上述静态调用链和保存配置范围内的判断，不覆盖任意外部热补丁、反射写入或调试器修改。

随机入口也再次核对：`j1345` 仅缓存 icall 函数地址并将两个 float 边界直接传给原生 Range，没有附加 managed seed 参数或重置操作。保留的 `UnityEngine.Random` 元数据方法表没有 `InitState`、`set_seed`、`get_state`、`set_state`，主内存 icall 字符串也没有对应入口，见 [字符串记录](Evidence/random-sequence/random-icall-strings.json)。这支持“本包这条 managed 路径不自行设种子”，不能据此恢复 Unity native 自动初始化的状态。Range 本身会推进原生 RNG 状态，不能把它描述成无状态/只读数学函数。

独立检查已完成：`ChallengePlan.cs + ReplicaSourceRandom.cs` 以 Unity CoreModule/netstandard 引用编译成功。通过注入 `System.Random` 驱动的浮点测试替身运行原 `SourcePlanChecks` 的 3,000 个种子：三主题容量、全局三倍数、ID、重放及跨层拆组全部通过（split=3000）；这是算法回归，不能作为 Unity 原生序列验证。
