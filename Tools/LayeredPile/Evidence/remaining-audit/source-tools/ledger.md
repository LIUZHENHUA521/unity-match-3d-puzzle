# SourceTools / Input / flight / audio 余差复核（2026-10-16）

范围：WX 2.1.12 已取 WAT 与资源默认值；原生 API 使用完整 IL2CPP method map 核对。未发布；本子任务不启动 Unity。生产修改经主任务后续授权，验证由主任务统一执行。

| ID | 结论与证据 | 动作 / 状态 |
|---|---|---|
| T01 | 原暂存移动 X/Y/Z 读取 LevelManager 680/688/684，其资源值 3/26/27；WX metadata 默认值表真实解码为 OutSine / InBack / OutBack。StoreObj j19259 缩放常量26=InBack。先前报告错误标为 OutQuad / OutElastic / InOutElastic。 | 已修 SourceTools 4处曲线，补enum数字断言及真实移动中点采样。待统一Unity验证。 |
| T02 | GetUnit j13227：SetParent → BeClick（内部 SetVisible、SetRigid、PlayAnim）→ PlaySound → box移除/content插入 → RefreshContentDisplay → SetObjOutStore。此前选择音效早于鹅动画，影响共享随机序列。 | 主任务整合 Input 先BeClick动画后音效。音效参数、50ms节流及三档pitch本轮未发现数值误差。 |
| T03 | 原 SetObjOutStore j38487 只从列移除、对剩余列重排、必要时收背景，不kill取走对象。当前提前ReleaseStored并kill与source顺序不同。 | 已删ReleaseStored被取走obj的DOKill；主任务Input在BeClick前杀旧tween并将ReleaseStored后移，避免新飞槽被杀。 |
| T04 | Shuffle j65101 先CheckObjMaxHeight j5870→j19885，仅当新峰值低于记录maxHeight才更新，随后以记录maxHeight-7选择，严格>。项目此前重算瞬时峰值，物品向上弹时候选带偏小。 | Tools 已调用新增Layered.RefreshSourceMaxHeight()并传记录值；静态fixture覆盖向上弹后保持带宽。接口由主任务实现。 |
| T05 | Shuffle末尾 j65101 每候选设置scale=one→CheckVisible j9885→CheckRigid j5869(player=1,depth=1)。项目此前全盘RefreshVisibility（含再更新高度）、再全盘正常depth=.5物理。 | Tools 已改为逐候选FinishSourceShuffle()，移除额外全盘刷新；接口、depth1和growth运行验证由主任务负责。 |
| T06 | 静音鹅随机分支：ObjectUnit.PlaySound j19260 material之后调用j17949；j17949自己在Random.Range之前检查effects开关。 | 当前静音early return正确，不作“仍需随机抽声”的假修复。 |
| T07 | 消除中心缩小 j65313 DOScale(0,.2)→Ease26，真实为InBack。 | 旧飞槽当前InBack正确。本轮早期曾沿用错误工具enum注解误报OutElastic，已立即撤回，未作错误代码修改。 |
| T08 | UseTool j38482→j38495分配3443544(type23105)=MessObjRotaAnim d__292→main65101。虽然CreateInfo写isVisible，整个main65101不读取该字段、不调用SetVisible、不置scale0。另一分支sub65103 MessObjPosAnim才有visibility随位置移植和scale0长大。 | 不向默认旋转打乱移植另一分支；两位独立调查员已交叉确认。 |

## 已核对 API，避免按名字猜

`verified-method-map.json`来自完整WX方法映射：j4551=get_up、j1028=position、j1387=localScale、j1161=set_localScale；j5721/9406/12483=DOMoveX/Y/Z；j9885=CheckVisible、j5869=CheckRigid、j5870=CheckObjMaxHeight。
`wx-ease-enum.json`保存真实fieldIndex、默认值entry/file offset、压缩字节和解码值；`decode_ease.py`可在研究workspace重放。正整数枚举用metadata v29压缩有符号编码（unsigned >> 1），不是按字段位置猜数。

## 仍有明确边界，不能声称完全一致

- 暂存/飞槽屏幕锚点与间距继续适配项目安全区，模型大小按源pickScale转换；不是整套原Canvas排版。
- 工具与物品效果使用本地重建资源；工具粒子来源iOS1.11.03，尚无WX对应粒子包逐字节比较。模型83份中43份有WX相同参数交叉证据，另外40份仍为iOS证据。
- AutoFind遇到损坏自定义关卡、缺足够同类时项目拒绝部分执行，是故意保留的防损坏门槛；正常三倍数组数关卡不触发。Shuffle空集合、无Layered兼容fallback保留，不作为原版代码证据。
- 原对象池复用仍以本地实例清理代替；不声称相同分配开销。Home/Reset取消旧协程和tween属于项目生命周期要求。
- 原失败救援、广告/付费获得工具、特殊id5/6、活动工具次数覆盖、双人/第一关分支不扩展。
- 当前旧飞槽是已取包 NetInitManager.isUseNewAnim=false 默认，不等于所有线上AB配置。新飞槽另有完整证据但没有强启。

此次没有把“源码里存在一个字段/另分支”直接当成默认运行行为。没有发布或声称真机性能已验证。

## 限定追加：Visual时钟与手动触摸高度

- T09：ReturnLight j65504与FindBeam j65498均累加**j1511=Time.unscaledDeltaTime**，不是j1414=deltaTime。原Return跟随.3秒后WaitForSecondsRealtime(2)，Find为.3*.7=.21秒。已修helper两处错误scaled累加，补timeScale=0实际跟随、停止、回收测试。
- T10：背景j9876调用DOLocalMoveZ(.3)没有SetEase或SetUpdate，库默认OutQuad、scaled；当前保持正确。原10个粒子系统useUnscaledTime=false、simulationSpeed=1，粒子时钟本身仍scaled；不能因为光带跟随unscaled就把粒子一并改掉。ShuffleFX原父Camera/MessToolPoint局部(0,0,1) scale.1继续保持；新增暂停粒子time回归。
- T11：GetUnit(playSound=true)会在BeClick前用手动拾取物品的worldY调用SetMinTouchHeight j9878(height,false,player1)。若height<touchHeight，touchHeight=max(height,touchHeight-.5)；不写maxHeight、不立即显示扫描。自动凑齐playSound=false不调用。原offset584是_pickAnimTime=.3，BeClick将它用于缩放动画时长，不是高度。主任务已接手RecordSourceTouchHeight接口和Input门控。

具体API、WaitForSecondsRealtime元数据type与原PS模块时钟摘要见visual-clock-audit.json；暂停回归待主任务统一运行。

## 最终整合状态

主任务 `goose-selection-audit3` Unity 退出 0，27 套检查全部通过。表中待统一验证项已完成：暂存曲线实际中点、历史峰值、逐候选depth1、手动touchHeight、飞行中道具并用和暂停时钟均通过。粒子暂停fixture先越过进入暂停当帧再取基线，未改生产粒子的scaled时钟。最终汇总见 `Tools/LayeredPile/selection-remaining-alignment.md`。
