# 槽位 UI、暂存列与回槽布局对齐记录

更新时间：2026-10-16

本轮只使用已提取的微信 `data.unity3d` 与 `goose-main.wat` 证据，未发布体验版。

## 原版几何证据

从 `goose-wechat-data/extracted/data.unity3d` 恢复的 `LevelManager` 主场景：

- `Content` localScale 为 `(1.4,1.4,1.4)`，localPosition `(0,-5.04,2)`，旋转为 X=-90°。
- `ContentPointList` 的 7 个点 local X 为 `-2.13,-1.42,-.71,0,.71,1.42,2.13`，所以局部间距为 `0.71`，经 Content 缩放后源世界间距为 `0.994`。
- `StorePointList` 的 3 个点 local X 为 `-.83,.01,.83`，位于 local `(0,-.46,1.55)`。
- `LevelManager` 序列化数值：`_pickAnimTime=.3`、`_pickMaxScale=1.4`、`_storeObjPosYOffset=.75`、`_storeObjIndexOffset=.1`、`_storeContentShowZ=1.44`、`_storeContentHideZ=-3`、`_storeAnimTime=.3`。

WASM `j19862`（`PlayStoreAnim/SetStoreContentPos` 路径）确认：暂存物品按三列目标点定位；同列按 `.75 * (1-count+index)` 沿暂存点 `up` 方向叠放；非首个物品交替使用 `.1` 的小偏移；X/Y/Z 分量都在 `0.3` 秒内独立 tween。`j9876` 负责暂存底板显示/隐藏位置，显示与隐藏 Z 使用上述资源值。

## 项目改动

- `Assets/GooseReplica/Scripts/ReplicaTray.cs`：源模式槽位目标改用 `0.71 * 1.4 = .994` 的源世界间距，仍保留安全区屏幕锚点并投影到 Content 深度 `z=1.58`。
- `Assets/GooseReplica/Scripts/ReplicaSourceTools.cs`：`StorePosition` 使用源三列 X（乘 1.4）、源点高度 `1.55*1.4`、Content 深度 `2 + .46*1.4 = 2.644`；同列叠放间距为 `.75*1.4=1.05`，交错偏移为 `.1*1.4=.14`；现有 X/Y/Z 三条 `.3` 秒曲线保持与 WAT 的 `OutSine/InBack/OutBack` 配置一致。

这些改动只对齐槽位/暂存目标坐标和时序，不把当前 world-space Canvas 宣称为原版 Content 预制体的逐像素复刻。原版 tray 的 prefab 层级和点旋转已恢复为数值证据；项目仍以安全区适配 Canvas 承载槽位外观。

## 验证边界

静态验证应检查：`SlotPosition(0)` 到 `SlotPosition(1)` 距离约 `.994`；同一暂存列两项目标沿相机 forward 的间距约 `1.05`；三列横向相邻目标间距分别约 `1.176` 和 `1.148`（`(.01-(-.83))*1.4`、`(.83-.01)*1.4`）。`SourceToolsFramesValidation` 已覆盖 `.3` 秒三轴曲线和回槽后再次拾取；统一验证时应复跑 `VisualSceneBatch.FrameChecks`。未发布。
