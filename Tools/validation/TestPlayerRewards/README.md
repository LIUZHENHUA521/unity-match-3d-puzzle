# 测试账号红包回归

运行 `python3 Tools/validation/TestPlayerRewards/run.py`。使用独立 Unity 项目和随机本地端口，不操作主项目编辑器，不登录真实账号、不发放真实红包。输出位于 `work/test-player-rewards/runtime/`。

被测生产组件：Match3DRewards、Match3DGameRecords、Match3DTestPrivileges、奖励弹窗及队列、联系客服入口。复制实际 RewardReceived 预制体、字体及素材，检查按钮完整显示并输出截图。

FixtureDependencies.cs 仅替代账号、关卡、场景/棋盘和动画依赖；BeginQualified 在此验证入口参数，真实完整场景的启动与两阶段玩法不在此模拟环境运行。服务端发奖规则由 goose-server 的 rewards.test.mjs 验证。真实微信打开客服由现有 Tools/wechat/test-customer-service.cjs 桥接测试覆盖，仍需新包真机验证。
