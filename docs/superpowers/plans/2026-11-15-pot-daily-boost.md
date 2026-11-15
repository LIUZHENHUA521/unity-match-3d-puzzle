# 每日配套加量 Implementation Plan

**Goal:** 每日普通/加量固定布局配套发布，通关后 72 小时按动态 n 发放。

**Architecture:** 共用契约校验配套配置；后台生成两版并原子发布；服务端按玩家通关状态选择快照；Unity 还原实际选中的关卡。

**Tech Stack:** Node.js、CloudBase、React/Ant Design、Unity C#。

- [ ] 契约与后台服务：修改 crazyCombo/tasks/callFunctions/adminGooseRecords/dailyLevelContract.js、dailyLevels.js，同步 server/services/goose-server/src/daily-level-contract.cjs。potBoostCount 校验三消倍数和容量；potBoosted 校验首层差值和相同教学布局；原子导入发布。运行 node --test callFunctions/adminGooseRecords/*test.js。
- [ ] 发放与结算：修改 server/services/goose-server/src/daily-levels.mjs、rewards.mjs，新增 pot-daily-boost.mjs；选择时剥离另一版，结算持久化单调最近有效通关时间。覆盖 72 小时精确边界、跨档位、重试幂等及旧局锁定。运行 node --test services/goose-server/test/*.test.mjs。
- [ ] 后台：修改 PotDailyLevels.tsx、PotDailyPreview.tsx 和新增配套生成组件，动态 n 表单校验、两次生成原子回存、两版试玩、完整就绪才发布。运行预览协议测试与 npm run build。
- [ ] Unity：Match3DDailyLevel 增加选中版本信息，编辑器备用生成支持配套文件；真实隔离场景验证普通和加量完整快照、首关共用、重试稳定。微信构建检查与上传。
- [ ] 审查本次差异、提交推送相关仓库；通过 CloudBase MCP 部署云函数、游戏服务和后台；核对部署状态与线上静态入口；记录验证结果。
