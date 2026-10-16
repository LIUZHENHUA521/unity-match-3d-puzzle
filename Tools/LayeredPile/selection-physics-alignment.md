# 选中碰撞与唤醒：独立源码复核（2026-10-16）

结论：原版是 **选中物变为 kinematic、保留真实碰撞体、用 Update 的 scaled Transform.DOMove 向镜头平移 .4 / .2秒 Linear**。它没有人为搜索周围物体后 WakeUp，也没有切换 ContinuousSpeculative/Interpolate。成功收取不经过“取消选中→恢复动态→再变kinematic”；真正取消才恢复动态并清空两种速度。生产改动与完整 Unity 回归已由主任务完成，最新结果见 [整体报告](selection-remaining-alignment.md)。

## 证据来源

重新扫描 `work/goose-main.wat`、`goose-sub.wat` 和完整 `wechat-all-method-map.json`，提取62个相关函数，给每个直接调用加完整类型/方法名。`Evidence/selection-physics/manifest.json` 列出实际提取数量/调用；`callers.json` 保存 BeSelect、WakeUp、set_interpolation、set_collisionDetectionMode 的调用者扫描。字段偏移从主内存 field offset table 恢复；枚举从 metadata 的压缩整数默认值解码，不依赖枚举名字顺序猜值。源WAT哈希在 provenance.json。

## 输入与状态完整链

- `LevelTouch j38498`：按下保持期间调用 CheckClickRay；按键从有到无时直接 `GetTarget j19867`。
- `CheckClickRay j13231`：切换目标时先旧目标 `BeSelect(false,null)`，再新目标 `BeSelect(true,rayCamera.transform)`；没有切换/失去目标则不重复触发。新目标必须 notBeCatch=true、isHiding=false、isFinishCreate=true。
- `LostTarget j13226`：调用当前目标 `BeSelect(false,null)`、清当前目标引用、关闭普通Highlight。这是取消链。
- 成功收取链：`GetTarget j19867 → GetUnit j13227 → BeClick j12985`。GetTarget在GetUnit后清引用/关Highlight，**没有 BeSelect(false)**。GetUnit先把物品 reparent 到 `_rotaTrans`，传 `_pickMoveTime=.3` 给BeClick，随后维护板面/槽列表和飞槽。
- 元数据没有名为 CancelSelect 的 ObjectUnit 方法；实际取消语义在 BeSelect 的 false 分支。

## BeSelect j6603 精确行为

先检查 `notBeCatch(offset16)`、`_isFinishCreate(offset92)` 为true且 `isStore(offset18)` 为false；否则整体直接返回。随后 `_isSelect(offset19)=selected`。

true 分支：

```csharp
SetRigid(false); // body.isKinematic = true
if (LevelControlType == flatTopView /*2*/ && CurGameType != TeachScene /*2*/ && !IsLockGuidePhysic()) {
    transform.DOKill(false);
    _targetPos = transform.position + cameraTransform.forward * _pickOffset;
    transform.DOMove(_targetPos, _pickAnimTime, snapping:false).SetEase(Ease.Linear);
}
if (_objectAnim) _objectAnim.PlayAnim();
```

- `Init j19263` 从LevelManager读取 objectPickOffset和objectPickAnimTime，覆盖ObjectUnit构造默认值。WX场景保存值为 **-.40000000596** 和 **.20000000298**。所以是 `position - camera.forward*.4`，不是世界Y上移 .4。
- `DOMove j3031` 是 Transform 的 ShortcutExtensions.DOMove；不是 DOTweenModulePhysics.DOMove，也不是 Rigidbody.MovePosition。
- `j1183 → j31214` 将Tween.easeType(offset116)设为参数1，清customEase。metadata枚举确认 Ease.Linear=1、OutQuad=6。原选中上提**不是默认OutQuad**。
- 上提没有 SetUpdate。WX `resources.assets #39 DOTweenSettings` 全部168字节解码：defaultUpdateType=Normal(0)、defaultTimeScaleIndependent=false、timeScale=1、useSmoothDeltaTime=false。因此常规Update/游戏时间推进，timeScale=0时暂停；不能套用飞槽那边的 SetUpdate(true)。
- true分支无velocity/angularVelocity写入，没有碰撞模式/插值模式修改，没有AddForce、MovePosition或WakeUp。

false 分支：

```csharp
transform.DOKill(false); // 停在当前位置，不跳到终点、不返回按下前的位置
SetRigid(true);          // 常规关卡 body.isKinematic=false
body.velocity = Vector3.zero;
body.angularVelocity = Vector3.zero;
```

两分支末尾均调用可选 `_ObjectCustomMod.OnSelect()`；包内 ObjCustomModBase.OnSelect映射j889空实现，唯一派生ObjFireworkVFXMod只覆写OnInit/OnComine，没有隐藏碰撞/唤醒逻辑。可选ObjectAnim.PlayAnim是对子Transform的DOPunchScale(.05,.5秒,vibrato10,elasticity1)，不操作刚体；不能把它当作所有物品通用物理脉冲。

## SetRigid / CheckRigid / HideRigid

`SetRigid j7954(enabled)` 实际写 `body.isKinematic = !enabled`。启用动态时有三种阻止条件：已在Store、TeachScene、IsLockGuidePhysic；关闭动态不受这三项阻止。IsLockGuidePhysic为 `_IsInForceTeach && ABPackManager.NewPlayerGuideABType==0`。

`CheckRigid j5869` 按物品高度和scale≈1得出 hide，调用 HideRigid(hide,false)。`HideRigid j4679`：

1. 已经是所需isKinematic状态则return。
2. **_isSelect=true直接return**；notBeCatch=false也return。
3. 如果要启动态且isStore，则return。
4. 非force且速度模长>.2，只拒绝转kinematic；转动态不受速度阈值阻止。
5. `SetRigid(!hide)`；从隐藏刚体转动态还可能触发既有DelayToGrowScale路径。

所以选中标志必须让每秒高度/刚体优化跳过该物品。无需从板面注册字典移除，更不能以移除为由唤醒半径邻居。原版刚体和碰撞产生的自然接触/自然唤醒仍保留。

## BeClick与碰撞关闭

BeClick j12985在notBeCatch为true时：DOKill(false)、SetVisible(true)、_isSelect=false、notBeCatch=false、SetRigid(false)，然后启动 DelayToHideCol。

`<DelayToHideCol>d__47.MoveNext j65349`：等待 **WaitForSeconds(.05)**，若物品仍处于被收取状态（notBeCatch=false），将 `col` 和 `realCol` 两个GameObject SetActive(false)。等待前的.05秒碰撞体保留，刚体保持kinematic；没有“短暂切回dynamic再清速度”的阶段。等待类型3440336由metadata usage解码为type20795 WaitForSeconds。

## 对项目旧实现的逐项修法

| 复核时实现 | 原版证据 / 应改 |
|---|---|
| FixedUpdate + Rigidbody.MovePosition + OutQuad | Update/scaled Transform.DOMove + Linear；.4、.2数值原本正确 |
| Begin先清线/角速度 | 删除显式清零；按源仅SetRigid(false) |
| Begin设ContinuousSpeculative和Interpolate | 删除；保留导入的Discrete/原插值 |
| Begin RemoveForPick + WakeSelectionNeighbours | 选中标志/优化排除，删除两次邻域唤醒 |
| RemoveForPick自身WakeNear | 收取只注销管理，不附带WakeNear |
| Cancel Register+WakeNear | 取消直接动态+线角速度0；删除邻域激活/唤醒；不以高度门限立即重新冻结 |
| Input释放先Cancel，AddStack又Cancel/短暂dynamic清速度 | 加独立Commit结束反馈/上提，不调用取消恢复链；成功收取连续保持kinematic |
| WakeNear按半径.9和高度-.35提前Activate邻居 | 原选择链没有该算法；不能把未激活深层作为“碰一下”提前唤醒 |

`callers.json` 的直接调用扫描中，游戏选择链无 Rigidbody.WakeUp、set_interpolation、set_collisionDetectionMode调用；也没有OverlapSphere等邻居搜索。结论是取消**人为邻域唤醒**，不是禁用PhysX自然唤醒。Unity引擎内部在isKinematic/接触变化时如何处理速度与睡眠不等同于脚本显式操作，测试应区分二者。

## 建议验证

- .2秒上提在.1秒恰好走一半；不同相机倾角都沿-camera.forward；timeScale=0不推进。
- Selected物在跨过每秒高度检查后仍kinematic，碰撞体保持开着。
- 真取消不回到起始点，随后dynamic且线/角速度0；任意额外邻居的深层kinematic状态不变。
- 收取不能经Cancel，不出现dynamic中间状态；.05秒scaled以后才关两套Collider。
- 无接触的睡眠邻居保持睡眠；实际碰撞接触的动态物体可以自然被推开/唤醒，不能因删除WakeNear而把真实碰撞一并关闭。

## 顺带纠正前轮AutoFind计时

FingHighlight j65259的等待全局3440340编码0x2000a27f，解码为type20799 **WaitForSecondsRealtime**，且显式m_WaitUntilTime=-1。故蓝Lighting的.3秒寿命应以unscaledTime推进；前轮误写成scaled，已在ReplicaSourceHighlight改为unscaled deadline，并补暂停时钟回归；尚待主任务统一Unity验证。它与选中上提scaled、BeClick碰撞延迟scaled、飞槽SetUpdate(true)是不同的时钟，不可统一套用。

## 追加限定核查：可选ObjectAnim与解冻增长

`object-anim-bindings.json` 对当前83个实际导入物品逐项记录源码prefab来源、对象pathID与引用，其中包括12鹅；所有 `_objectAnim` 都为null。三个已获得WX物品bundle中43个对应物品也全部完整解码，引用均为null。因此虽然BeSelect支持ObjectAnim.PlayAnim的可选DOPunchScale，这批游戏物品没有启动该分支，无需增加通用punch或为鹅补绑定。

HideRigid→DelayToGrowScale入口不由isHiding决定：要求这次刚体真正从kinematic变动态，`!LevelManager.IsMoreVisible`、`_isFinishCreate=true`。本轮独立扫描4个“引用LevelManager TypeInfo且store8 offset4”候选，确认只有cctor j83393写该字段true；其它候选分别对不同TypeInfo写offset4。已把4份候选WAT保存。因此当前普通包不应新增“刚体解冻就长大”，走原SetVisible入口。

但SetVisible入口仍受内部守卫：j19265要求LevelManager实例存在、`ObjectUnit._changeObjScale(offset128)=true`、`LevelManager._isUseRealPhysicSpace(offset500)=false`，然后才finishCreate=false、DOKill、scale=zero、DOScale(1,.75)、完成时finishCreate=true。内部没有检查isHiding，也没有scale<1判断；外层SetVisible是renderer状态发生变化且开启、IsMoreVisible=true、finishCreate=true。

当前Layered.SetVisible已表达renderer显示转换和finishCreate门槛，却未表达 `_changeObjScale` 开关。这一字段不在83/43个物品序列化布局，ObjectUnit构造/Init未见赋true；本轮直接写入扫描也未见ObjectUnit相关写入。该潜在缺口已发主任务并交另一证据分工独立核查，不能仅因SetVisible有动画代码就宣称原包必定执行它；本子任务未改这一生产行为。

## 最终整合验证

`goose-selection-audit3` 27 套检查全部通过。自然碰撞/无接触睡眠、选中优化排除、取消与成功收取分离、.05 秒碰撞窗口及各时钟均有真实帧验证。 `_changeObjScale` 独立扫描最终支持默认 false，普通重新显示已取消误接的 .75 秒长大，条件分支另行测试。此前“尚待整合”文字记录取证阶段，最终状态以本段及整体报告为准。
