# 每日同题实施计划

目标：实现已确认的每日同题、固定开局、后台候选试玩发布和带日期档位的分享。
架构：后台管理完整候选；Unity 生成/还原真实布局；服务端按日期档位锁定发布快照。

- [ ] 在 tasks/callFunctions/adminGooseRecords/dailyLevelContract.js 与 server/services/goose-server/src/daily-level-contract.cjs 增加 shared-v2 校验，保持契约逐字一致。
- [ ] 在 dailyLevels.js 增加候选创建、导入、试玩确认及发布锁定；GooseDailyLevels.tsx 接入同页操作，沿用权限。
- [ ] 在 daily-levels.mjs 与 rewards.mjs 接入协议 3、公共日期快照，保留旧局结算与旧模式日切迁移。
- [ ] Unity 新增 Match3DDailySnapshot 数据与还原，在 Match3DFlow 内接入，分享携带日期档位；新增编辑器候选导入、生成快照、试玩与导出工具。
- [ ] 添加共享关服务端/后台回归及 Unity 快照还原验证；运行相关 Node suites、管理端 Vite 构建和隔离 Unity 验证。
- [ ] 分仓提交推送；使用可用 CloudBase MCP 部署并核对云函数、专用服务和后台；构建上传微信开发版本并记录实际结果。
