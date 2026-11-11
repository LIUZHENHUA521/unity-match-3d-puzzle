# 七日签到后端部署记录

- 日期：2026-11-11（北京时间）。用户明确授权部署签到后端。
- 仓库：crazyCombo/server，提交 `5b799d2745402be88eab05f3961d0d98ac757b35` 已推送 `origin/master`。
- 环境：`cloud1-1g0u68ow59705e3f`，CloudRun 服务 `goose-3d-pvp`。
- 版本：`goose-3d-pvp-027`；BuildId `2612155812`；RunId `multi_tenant_1xRA2zo8taPBSZ`；状态 normal，100% 流量。
- 上一版本：`goose-3d-pvp-026`。

通过本机 CloudBase MCP 读取线上配置、下载旧源码、部署 Git 提交快照，并下载已部署源码复核。生产源码差异仅 `src/invitations.mjs` 接口注册及新增 `src/sign-in.mjs`；Dockerfile、依赖清单不变。现有环境变量、0.25 CPU / 0.5 GB、固定单实例、8080 端口、网络访问与扩缩容配置均保持一致。

## 验证

- 部署前完整后端89项测试通过。
- 已部署 src、Dockerfile、package.json、package-lock.json 与提交快照逐文件一致。
- `https://9nanimal.cn` 及 CloudRun 默认域名的 `/health`、`/invites/health` 均200，数据库和微信登录配置就绪。
- 两个域名的 `/invites/sign-in/state`、`/invites/sign-in/claim` 均拒绝无凭证请求（401），确认新路由生效且需要登录。
- 公网 WSS 两客户端私有房间、相同牌组、同步开局、出牌、重复出牌拒绝、撤销次数及断线结算检查通过。

未使用真实微信账号发放签到奖励；实际领取与跨自然日体验仍需新版客户端真机验收。本轮只部署后端，未重新打包、上传或提审小游戏。

完整原始部署记录在本机 `work/seven-day-sign-in-deploy/`，其中配置快照包含服务端私密配置，不纳入版本库。
