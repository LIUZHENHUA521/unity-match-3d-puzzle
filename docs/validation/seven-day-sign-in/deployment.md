# 七日签到部署记录

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

未使用真实微信账号发放签到奖励；实际领取与跨自然日体验仍需新版客户端真机验收。后续小游戏上传结果见下方。

完整原始部署记录在本机 `work/seven-day-sign-in-deploy/`，其中配置快照包含服务端私密配置，不纳入版本库。

## 微信小游戏 0.1.56

用户随后明确要求发布小游戏体验版。源码提交 `1edcb5ce1e1c960aa7775cac1cf929abb5ce6e85` 已推送 `origin/master`，从该提交的独立快照构建，未纳入工作区其他未提交改动。

- AppID：`wx74f2d8722c02a6f4`（一锅消不完）。
- Unity WebGL 构建返回 `GOOSE_WECHAT_EXPORT_OK`；AppID、竖屏、资源分包校验通过，签到预制体及 9 张美术资源进入构建，签到脚本保留在 IL2CPP 输出。发布脚本 5 项测试通过。
- 微信开发者工具 `upload` 成功，版本 `0.1.56`；上传说明：新增七日累计签到，登录领取每日道具奖励，支持断签续领和防重复领取。
- 回执：总包 30,779,701 字节；主包 2,518,727 字节，资源分包 22,126,595 字节，程序分包 6,134,379 字节。
- 发布包：本机 `Builds/Wechat/release-0.1.56-seven-day-sign-in/`；原始回执：`Builds/Wechat/upload-info-0.1.56.json`；构建和上传日志：`work/seven-day-sign-in-release/`。
- **已上传开发版本，尚未设为体验版。** 本轮访问微信公众平台时，浏览器工具明确返回管理员策略禁止访问 `mp.weixin.qq.com`，未绕过。需用户在公众平台 → 版本管理 → 开发版本 `0.1.56` → 设为体验版。
- 未提交审核，未发布正式版；微信真机登录、实际领取及跨自然日体验仍待真机验收。

## 首页入口修复版本 0.1.57

- 源码：`32e11888294fa50788bdb3ecd7bf44e6a587939c`，已推送远程 master。
- 修复签到和邀请入口同坐标导致的遮挡：签到在首页左侧上方，邀请在下方。三种屏幕比例的边界、点击命中、独立弹窗及完整签到流程验证通过。
- 从提交快照构建；`GOOSE_WECHAT_EXPORT_OK`、发布包校验通过，微信开发者工具上传成功。
- 总包 30,776,419 字节；主包 2,518,727 字节，资源分包 22,123,313 字节，程序分包 6,134,379 字节。
- 发布包：`Builds/Wechat/release-0.1.57-sign-in-entry-fix/`；回执：`Builds/Wechat/upload-info-0.1.57.json`；日志：`work/sign-in-entry-fix-release/`。
- 已上传开发版本。受本会话已确认的公众平台浏览器管理员策略限制，未操作设为体验版；需用户在版本管理将 **0.1.57** 设为体验版。未提审，后端无需重新部署。
