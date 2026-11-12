# 分享/广告分渠道上报验证

2026-11-12。结算新增 acquisitionVersion=1、每项道具的 share/ad，以及 reviveUsage.clear/time 的 free/share/ad；rewarded 继续保存 share+ad。统计口径为获取成功且效果实际生效，不代表广告全部展示或微信确认分享送达。旧记录不回填为零。

- 服务端隔离发布源码：93 项测试通过，覆盖普通记录与红包路径、新旧规范化兼容、整数及合计校验、复活一致性、并发幂等和重复发券防护。
- Unity 实际场景 RewardAcquisitionValidation：成功/失败/取消、重复与过期回调、道具/复活独立渠道及换局清零通过；使用本地 Editor 回调，不播放真实广告或分享。
- 独立 Unity RecordSync：20 项运行断言及 HTTP 请求次数检查通过。新增真实结算 JSON 序列化、四种道具渠道与旧合计、两种复活及持久化快照不可变验证。
- 独立 Unity TestPlayerRewards：已有奖励与领取界面回归通过。
- 后台 helper：2 项测试通过，验证道具与复活合计、免费不计入、旧记录/未结算/零次区分；Vite 构建通过。

现有 adminGooseRecords 查询直接返回记录，无需改动云函数、菜单或权限。后台部署按已提交的隔离源码构建，仅上传本轮主应用 assets 和 index，保留线上其他静态内容。

本次不修改手机历史记录，也不构建或上传小游戏包。只有玩家使用新客户端，才会产生分渠道数据；未结算局仍不计入结算统计。

## 部署确认

- 管理后台提交 `a3187bc` 已推送并通过 CloudBase MCP 上传。公网 `/admin/index.html`、主 JS 和 CSS 均 HTTP 200，SHA-256 与隔离构建产物一致。
- 用户明确同意部署接收接口后，发布服务端提交 `daa8495` 的隔离源码。CloudRun `goose-3d-pvp-028` 为 normal、100% 流量，保留 0.25 核 / 0.5 GB、固定一实例、端口及全部原环境变量键。
- 自定义域名与默认 CloudRun 域名的 `/health`、`/invites/health` 均 200，数据库与登录配置 ready；未登录 `/records/finish` 均返回 401，无测试玩家或闯关写入。
- 发布目录与机器可读检查结果位于 `work/acquisition-telemetry/`。并行工作区存在另一项奖励政策变更，本次发布只包含分渠道记录变更；其未提交源码和测试留在原工作区。
- 小游戏客户端提交 `970f0b27` 已推送；尚未构建或上传新的小游戏包。
