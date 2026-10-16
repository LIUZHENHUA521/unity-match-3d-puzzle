# 飞槽与三消时序复核（2026-10-16）

未发布。主证据为微信小游戏 2.1.12 的 WASM 与同包 `data.unity3d`。资源字段来自安装包，不等于已经抓到线上账号运行时分支。

## 先纠正新旧分支判断

不能只看 `LevelManager..cctor(j83393)` 把 `IsUseNewAnimManager` 初始化为 true：`LevelManager.Awake(j83431)` 随后用 `NetInitManager.Instance.isUseNewAnim` 覆盖它。WX 包 `level0/NetInitManager path_id145` 的这个字段为 **false**，`isUseNewAnimAB` 也为 false。字段解析完整消费 880 bytes；序列与 WX IL2CPP 字段访问标记及 SerializeField 属性独立核对，非只按旧 iOS 字段猜偏移。`isUseNewAnim` 序列化 bytes 848–851，对应运行时偏移 156；AB 开关 bytes 808–811，对应运行时偏移 135。

`CanUseNewAnim(j5871)` 还排除双人模式。本轮据此恢复**该包默认旧动画路径**，没有强行启用新的 `LevelObjAnimManager.ClickObj`。

扫描两份 WAT 已映射游戏函数，static offset81 的直接写入只见 cctor/Awake；没有发现 NetInitManager offset156 的后续直接写入或新动画 AB getter `j96762` 的调用。该负面结论限于已映射的静态函数，不能证明服务器反射、动态配置或其他版本永不覆盖。

## 实际修改

第二关单人使用下列恢复路径；第一关、PVP 保留原项目集成。

| 时刻/阶段 | 原版证据 | 本项目落地 |
| --- | --- | --- |
| 抬手 | GetTarget j19867→GetUnit j13227→BeClick j12985→RefreshContentDisplay j9879 | 抬手即开始位移和缩放，不插额外前摇 |
| 0–0.3 秒飞槽 | WX LevelManager 保存值 `_pickAnimTime=.3`，XZ Ease1=Linear，Y Ease6=OutQuad，rotation Ease5=InQuad | 各轴单独补间；已在槽内的物品也是 .3 秒，不再套 .2 秒移位近似 |
| 0–0.1 秒缩放 | UseTwoScaleCurve=true，peak1.4，.1 秒 OutQuad | 从释放时尺寸放大到1.4倍 |
| 0.1–0.3 秒缩放 | j63999，目标是 ObjectUnit._pickScale，剩余 .2 秒 InSine | 恢复83个模型各自 pickScale；按现槽距与原本地槽距.71换算，移除统一包围盒大小上限 |
| 落地 | j63997：起始尺寸×.03，.2秒，vibrato1/elasticity1 DOPunchScale | 恢复微小落地弹动 |
| 点击后 .05 秒 | DelayToHideCol.MoveNext j65349 | 保留短碰撞窗口，到期才关闭拾取及实际碰撞形状 |
| 第三件进入逻辑列表 | DelayToCheckContent.MoveNext j65369 在第一个 yield 前就检测连续三件，并从槽与关卡逻辑列表移除 | 立即释放3个逻辑槽；视觉物品单独保留，允许继续点下一件 |
| 第三件点击 +.3 秒 | 旧分支 DelayToCheckContent 等待 .3 后启动 DestroyAnim j65313 | 两边物品向中央汇聚 .2秒，默认 OutQuad |
| +.5 秒 | j65313，TimerManager.WaitFor0_2Second | 移除两边；中央用 .2秒 InBack 缩到0，不是原项目三件一起 .18秒缩小 |
| +.7 秒 | EndCombineObjAnim j19872→CheckLevelState | 执行该批完成检查，实际 GameManager 在此后才允许最后三件胜利；并行三消不必全部结束 |
| +.75 秒 | j65313 的 WaitForSeconds(.05) 后移除中央并 RefreshContentDisplay | 清理最后视觉物品并启动剩余槽位 .3秒重排 |
| 第七件无三消 | j65369 未匹配时直接 PrepareLose j13225，普通模式设置 IsPrepareLose=true并清选中 | 立即锁住额外点击并调用项目既有失败窗口。原版可用工具检查/救援窗口仍是剩余差异 |

`CheckLevelState(j19881)` 在每次 `EndCombine` 时检查未拾取的 `_boxObjList`，不统计其他仍在播放的三消动画。因此连续两组三消只相隔 .2 秒时，若第一组在 +.7 秒回调时盘面已经清空，原版不等第二组的 +.9 秒。本项目已移除“等待全部匹配完成”的额外条件，以每次完成回调作为判胜检查点。当前项目关卡列表仍在配成三件时移除，而原版有独立的未拾取列表；正常每类型三倍数的清盘路径已对齐，自定义非三倍数关卡与工具制造的残余槽位不是本轮覆盖范围。

输入不再因为存在普通飞槽/三消动画而一律禁点。工具（撤回等）仍保留项目动画忙锁；这不是宣称原版完整道具时序已经恢复。

### 时钟不能全部统一

`RefreshContentDisplay` 各轴和旋转、`BeClick` 两段缩放调用的 `j1281(...,1,MethodInfo3502500)`，泛型方法元数据解析为 `TweenSettingsExtensions.SetUpdate(t,isIndependentUpdate)`，即 `SetUpdate(true)`，不随 `timeScale`。三消 `WaitForSeconds`、汇聚、中央缩小、落地 punch 与延迟关闭碰撞仍按各自原调用使用游戏时间。测试覆盖暂停期间飞槽继续、.05秒碰撞协程等待的混合行为。

### 槽位空间适配边界

原 `ContentPointList` 相邻本地 X 距离 .71，父 Content scale1.4，所以原世界间距 .994。目标点在主相机局部深度 **1.58**：Content localZ2 + RotateX(-90)作用于PointList localY.3×1.4。原目标世界旋转是相机旋转再乘 RotateX(-90)。

本项目此前把槽中物品强制放在世界 y=.3 地面，虽然屏幕落点相近，Y曲线的实际运动空间完全不同。本轮保留项目现有安全区与屏幕槽位排版，把目标投影回原深度1.58，并恢复目标旋转；底板Canvas也恢复到Content的相机深度2，与相机对齐，防止原地面底板被锅遮挡。槽位UI美术、屏幕位置、当前模型归一化中心仍是适配，并没有声称整套原 Canvas 已搬过来。

## 新动画也已恢复证据，但不激活

`ObjectAnimConfig3`：WX sharedassets0 pid668 与 iOS pid766 从name后到结尾字段字节完全一致，完整解析460 bytes。新分支时长为 `clamp(.12 + .01*(abs(UI dy)*.028 + abs(UI dx)*.015), .26, .35)`；source UI参考分辨率1080×1920。`ClickObj` 返回飞行+.05，CombieAnim再等+.05，汇聚.15、中央放大.10、.05后移除；重排等待返回时长+.35。这套数据保存在证据中，**没有用它替换默认旧路径，也没有加入人为切换UI**。

## 资源范围与尚未对齐

- 83个 pickScale 来自当前用于模型重建的 iOS 资源；其中43个有已取得WX对应资源，逐项数值相等。其余40个尚未做WX交叉校验，不能都标为同版本直接证据。
- 原版 `ObjectUnit.PlayAnim/OnCombine` 自定义鹅动画、模型特殊消除粒子/音效与 HighlightPlus 仍未完整恢复。
- 原失败后工具机会、撤回投掷协程、PVP双人槽位、第一关引导保持项目既有流程。
- 本轮不把“每次飞到不同槽都会重新采样距离”的新分支逻辑强行加进旧分支；包默认旧分支时长确为固定.3秒。

## 验证

`SourceFlightTimingValidation.Run()` 为实际PlayMode逐帧协程，检查轴曲线、缩放峰值、碰撞窗口、七槽三消立即释放、连点、视觉阶段、实际最后三件胜利链、七槽无匹配输入锁、撤回忙锁的生命周期、时间暂停与Home取消；补测并行三消判胜及六个真实原模型的入槽比例、竖屏槽位、相机深度和底板位置，并保存实际渲染。报告 `Temp/goose-source-flight-timing-validation.txt` 由主任务统一运行后记录。第一关现有 SelectionFeedbackValidation 和 SourceInteractionValidation 保持兼容。未做原版真机逐帧录屏对照。
