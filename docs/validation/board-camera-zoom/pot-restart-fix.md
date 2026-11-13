# 重新开局后缩放入口消失修复

原因：LayeredItemPile.Clear 对旧 Match3DThemePresentation 调用 End 并清空自身引用；下一局 Begin 添加新场景组件。BoardCameraZoom 跨局保留，原来仅 Awake 使用 GetComponent 捕获场景，因此仍读取旧组件的 Active=false。Available=false 同时导致按钮隐藏和手势失效。缩放功能没有 AB 分组条件。

修复：每个场景 Begin 显式 BindPresentation(this)，刷新归属并重置倍率；End 使用 ReleasePresentation(this)，仅当前归属可解除绑定，避免旧场景清理影响新一局。

验证：2026-11-13，隔离 Unity 完整编译通过；原有 29 项检查加 10 项生命周期检查，共 39 项通过。覆盖旧场景保留时连续三次重建场景、每局可重新放大、旧场景退出不重置当前倍率。结果见 pot-restart-result.txt。本轮未上传体验版。
