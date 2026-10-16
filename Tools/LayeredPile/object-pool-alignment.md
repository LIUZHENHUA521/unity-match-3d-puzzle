# 对象池生命周期对齐

复核对象：官方微信小游戏 2.1.12 的 `LevelObjectPool` 元数据与 WASM 依赖，以及当前 Unity 分层关卡运行时。

## 原版证据

从 `outputs/goose-analysis/creation-visibility-evidence.json` 和
`outputs/goose-analysis/elimination-callback-candidates.json` 恢复到的
`LevelObjectPool` 类型有两个字段：`_objectUnitPool`、`_tempObjSet`。它公开了：

- `OnInit()`
- `Get(objName)`
- `Recycle(obj)`
- `Clear()`、`Clear(nameSet)`
- `ClearByList(nameList)`
- `ClearByLevel(indexType)`
- `ClearByExclusion(nameSet)`
- `ClearWithoutLevel(indexType)`

这组接口说明 `ObjectUnit` 的生命周期是“按对象名取用、三消后回收、按关卡/名称清理”，而不是每次消除都销毁 GameObject。`LevelManager` 还分别维护场景实例列表、当前锅内列表和暂存列表；回收对象应先离开这些逻辑列表，再交给池管理。

## Unity 对齐

`ReplicaSourceObjectPool` 按源 prefab 名维护桶：

1. `ReplicaFlow.CreateBoardItem` 创建分层物品时先 `Get(prefab,name)`，池为空才 Instantiate。
2. `InputController.RemoveSourceVisual` 在完整三消演出结束后调用 `Recycle`，先清理 DOTween、音频/飞槽绑定、动画、刚体、碰撞体和 Renderer，再停用并挂到池节点。
3. 暂存区清理、返回首页和重开关卡也走 `Recycle`，避免暂存物品脱离 `objectsOnScene` 后泄漏。
4. 复用时恢复源 prefab 缩放、位置/旋转由关卡创建流程写入；`ReplicaSourceColliders.Apply` 复用已有组件，避免同一 ObjectUnit 重复添加碰撞管理组件。
5. `Clear` 仅销毁池内停用实例，活动关卡物品仍由 `LevelGenerator.objectsOnScene` 管理。

## 验证

`SourceObjectPoolValidation` 在 Unity 图形批处理里实际验证了：

- `Get → Recycle → Get` 返回同一个 GameObject instance ID；
- 回收后对象停用，刚体恢复为运动学、无碰撞、零速度；
- 缩放恢复到 prefab 初始值；
- `Clear` 释放池内实例。

报告：`Builds/WechatProject/Temp/goose-source-object-pool-validation.txt`。

## 边界

WASM/元数据没有暴露池容量上限、淘汰策略或跨主题共享桶的运行时数值，因此 Unity 没有臆造容量或 LRU 行为。当前池只接入分层第二关；第一关和 PVP 继续使用原有 Destroy 路径。
