# 顶部安全区与第一关工具（2026-10-29）

- GameHUD HeaderSafeArea 的 anchoredPosition.y 从 +40 调至 -16，整体下移 56 个设计单位；保持按钮尺寸、棋盘和底部布局。
- 顶部同时参考 Unity Screen.safeArea 与微信 GetWindowInfo 的 safeArea.top/statusBarHeight。微信坐标先减 screenTop，再除 windowHeight 转为比例；按 Unity 分辨率换算后取较大值，避免重复叠加或 DPR 放大。创建/分辨率变化时读取 SDK，异常时回退 Unity。
- 第一、二关共用移出、凑齐、打乱的显示、可用性、数量与库存入口。每关各一次免费使用，邀请库存沿用现有流程。异步库存回调检查棋盘 creationVersion，切关后的旧回调不应用到新棋盘。
- `firststagehudcheck`：29 项通过，包含安全区/DPR、实际按钮移出/三消/打乱、扣次数、切关重置和库存回调适用性；不调用真实库存服务。
- 截图：Builds/HudFix/first-stage-tools.png（工具使用后数量为 0）。

编辑器和模拟安全区验证不代替 iPhone 微信真机验收。本次微信 C#/IL2CPP 编译完成，但资源包超限导致导出失败；Builds/Wechat/minigame 仍是之前成功产物，不能作为本次改动的验收包。发布脚本补齐 InviteSkinPS 的既有压缩规则后仍超限，未扩大修改范围或上传。

第一关完整回归复测 67 项通过；与构建同时运行时曾在第三主题的固定 0.85 秒切关断言失败，原代码复测通过，保留该时序波动记录。
