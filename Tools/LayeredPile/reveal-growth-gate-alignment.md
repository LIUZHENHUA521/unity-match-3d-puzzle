# `_changeObjScale` 独立核查（2026-10-16）

结论：当前微信发布包的普通 ObjectUnit 路径中，没有证据支持默认执行“重新显示时 0 → 1、0.75 秒长大”。相反，原代码的门闩 `_changeObjScale` 是默认 false 的非序列化私有 bool；已恢复的对象及创建路径没有开启它。`DelayToGrowScale` 存在不等于普通关卡会执行它。复刻不应无条件执行此分支。

## 直接证据

- 微信元数据版本 29，ObjectUnit 为 type definition 2042，byvalTypeIndex 16096；字段序号 30，token `0x04000a11`，typeIndex 10969。
- 原 WASM 注册数据 fieldOffsets 指针表地址 3336976，本类型表地址 `0xb1eb0`，第 30 字段实际偏移 128。不是将 iOS 的 64 位字段偏移套到 WASM。
- 原 WASM types 指针表 184576，typeIndex 10969 指向 `0x2f6388`，bits `0x80020001`：字段访问属性 1（private），类型码 2（bool）。
- 直接解析微信 Assembly-CSharp 自定义属性原表，`0x04000a11` 没有任何属性；邻近 col、realCol、_render、_anim 都能正常解析出 SerializeField，排除属性解析器全部漏读。无 SerializeField 私有 bool 不属于 Unity 普通序列化字段。
- ObjectUnit 父类 MonoBehaviour；33 个方法中无 Awake、Start、OnEnable。构造 j80060 和 Init j19263 未设置 `_changeObjScale`。
- j19265 首段在 manager 存在检查后读取 this+128 的一个字节，为 0 时直接离开。只有通过此守卫和 `!IsRealPhysicsSpace` 后才执行 scale=0、DOScale(1,0.75)。

## 扫描边界及排误

独立扫描 main/sub 两份完整 WAT：所有 i32/i64/f32/f64/v128 store（含 store8/16/32）覆盖字节 128 的情况，以及所有 `i32.const 128` 地址计算候选。保留 7719 个原始候选于 JSON，按方法映射过滤 ObjectUnit 与它的四个嵌套类型后只有 RotaAnim j36724 两处。

j36724 的两处基址为 local2；函数开头 local2=global stack pointer−160，明确是临时向量的栈空间，而非 this(local0)。所以不能误算为字段写。构造与 Init 也没有宽写覆盖这个字段。四个嵌套类型为 RandomAnim、DisplayClass44_0、DelayToHideCol、ThrowAnim。微信字符串常量表没有 `_changeObjScale`，未见按此名字反射设置的常规路径。

这是对现有完整 WASM 和已映射生命周期的静态结论，不声称穷尽任意外部 native 修改、按动态拼接名字反射等没有证据的路径。

## iOS 包版本边界

独立解析 iOS 1.11.03 自己的元数据：ObjectUnit type790，字段 token `0x040011ad`、typeIndex14124，原 Il2CppType `(3429,1,2)` 同样是 private bool，且没有自定义属性。iOS 的字段偏移和原生执行控制流不使用本次 WX WAT 结论替代。本结论中“普通执行路径关闭”直接依据微信 WASM；iOS 此处只有字段/序列化佐证。

## 对前次 growth/shuffle 审计的修正

此前“Shuffle 的暂停标志只停止优化循环，不停止 DOTween”仍是该代码路径的独立事实，但必须加前提：只有 `_changeObjScale` 真正开启并启动了 tween，才存在需要在 shuffle 中继续推进的长大过程。不能因此在普通对象默认状态下创建原本未启动的长大动画。

原始注册数据、散列、完整扫描结果与函数位于 [Evidence/reveal-growth-gate](Evidence/reveal-growth-gate)。

## 实现与验证范围

`ReplicaLayeredPile.Entry.changeObjScale` 默认为 false；`Register(body, sourceChangeObjScale=false)` 保存每个对象的门闩。普通游戏调用不传 true，显隐仅切换 renderer/点击对象，保留实体尺度与完成状态，不再增加 .75 秒禁选。仅明确传 true 的 fixture 执行休眠分支。创建阶段 `_growUpInCreate` 的 .02 秒/.1 步进及 FinishCreation 恢复完整尺寸保留。

已修改 RevealTimingValidation（普通重复显示与显式门闩分支分别验证）、SourceAlignmentValidation（三主题普通显隐）、HeightPhysicsValidation（实体尺度不变）、LayeredPileValidation（完成创建的可见物品可选）。主任务统一 audit3 Unity 运行 exit 0，27 套验证全部通过，其中包含普通显示门闩和显式休眠分支；完整最终记录以 [overall-validation.txt](overall-validation.txt) 为准。未发布。
