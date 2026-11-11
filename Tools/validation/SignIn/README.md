# 七日签到本地联调

在 Unity 编辑器关闭时运行 `python3 Tools/validation/SignIn/run.py`。

`server.mjs` 只绑定随机 localhost 端口，调用实际 goose-server 签到实现，以内存事务和固定服务器日期替代云数据库/微信认证。`/fixture` 是这个独立验证进程的控制接口，不存在于生产服务。

`Match3DSignInValidation` 在空场景禁用 Flow 的真实启动流程，显式连接本地账号服务，验证真实首页入口、签到视图、库存请求和领奖弹窗。结果与截图见 `docs/validation/seven-day-sign-in/`。
