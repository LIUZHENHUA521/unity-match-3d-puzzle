# 广告返回黑屏诊断日志

日志标记：`PotAdDiagnostics`。通过 SDK 已初始化的 `GameGlobal.realtimeLogManager` 与 `GameGlobal.logmanager` 写入微信实时日志和反馈日志，同时输出控制台。需构建并上传包含本改动的客户端才能采集；本次未发布。

按 `requestId` 关联一次广告。包含广告位、用途、客户端版本、平台、时间、耗时及请求状态，不记录 token、用户标识或完整业务上下文。设备详情及原始全局异常沿用 SDK 启动日志和错误日志，本次仅追加关联标记。

正常链路：`request` → `prepared` → `load_start` → `load_resolved` → `show_start` → `show_resolved` → `native_close` → `finished` → `unity_dispatch` → `unity_reply` → `unity_callback_returned` → `unity_dispatch_returned` → `unity_next_frame`。原生事件与 Promise 完成时序可能交错；不能仅按固定顺序判断失败。

- `native_error` / `load_timeout`：原生广告错误（记录数字错误码）或加载超时。
- `app_hide` / `app_show`：前后台切换；不推断广告已看完。
- `memory_warning` / `context_lost` / `context_restored`：平台支持对应监听时记录内存及图形上下文事件。不会强制恢复或改变默认事件行为。
- `runtime_error` / `unhandled_rejection`：同一时段发生全局异常，结合 SDK 原始错误日志排查。
- `unity_dispatch_failed` / `unity_callback_failed`：桥接调用或 Unity 业务回调抛错。
- `unity_next_frame`：Unity 协程继续执行，包含帧号、timeScale 和焦点状态；不代表已经成功渲染可见画面。组件销毁、停用或日志丢失也可能导致没有此记录，缺少日志不能单独证明卡死。

全局监听每个运行会话只注册一次，只关联当前广告或结束后两分钟内最近一次广告。不增加轮询、奖励推断、广告等待超时或自动重启。日志接口失败不会中断广告正常回调。

验证：`node Tools/wechat/test-revive-ads.cjs`、`node Tools/wechat/test-ad-telemetry.cjs`、`git diff --check` 通过。覆盖重复观看、生命周期关联、监听不重复、日志接口抛错、奖励回调及广告统计回归。尚未执行 Unity 构建或真机验证。
