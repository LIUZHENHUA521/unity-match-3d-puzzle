# 共用 AB 接入验证与发布（2026-11-13）

- Unity 源码已接入 `Match3DABExperiments`，登录后自动发现本游戏实验，失败使用现状，缓存一小时且按账号隔离。
- 共用后台继续使用 `/admin/#/ab-tests`，选择“一锅消不完 · Unity 3D”。游戏名登记已修正。没有创建业务实验或修改真实玩家分组。
- goose-server 101 项测试、AB 云函数 31 项测试通过。隔离 Unity PlayMode 的 11 项检查通过：默认值、登录、分组读取、换账号、停止实验、缓存过期、网络失败、迟到响应、401。完整微信 WebGL 构建成功。
- `hitABTest`、`createABTest`、`listABTests`、`listABTestRecords`、`updateABTest` 已部署并逐一确认 Active。
- 线上云函数使用临时验证账号检查有效凭证成功返回、伪造凭证拒绝；验证账号已删除，没有产生业务实验或命中记录。
- `goose-3d-pvp-030` 状态 normal、100% 流量。下载云端源码，与发布快照的 src、依赖清单和 Dockerfile 逐文件相同。原环境变量、实例规格、端口、网络配置完整保留。公网 health/invites/health 返回 200，新 AB 路由无 token 返回 401。
- 分析服务器增加本游戏专用签名凭据，仅允许 `ab_hits`。保留原凭据与所有既有环境变量；线上签名空数组请求通过鉴权后被业务校验拒绝，跨游戏请求被 403 拒绝，没有写分析数据。
- 共用后台源码构建通过。因线上已有无法从当前 HEAD 完整复现的历史发布，使用固定 SHA-256 的在线 JS 作为基础，按脚本逐项替换 8 处 AB 相关片段；反向替换验证其他字节不变。脚本保存在 tasks 的 `admin-platform/scripts/patch-unity-ab-release.mjs`。新脚本与入口已上传，CloudBase 下载及公网 HTTP 200 内容哈希均与发布包相同。
- 微信开发版本 `0.1.64` 上传成功，总包 31,408,517 字节，主包 2,527,572 字节。未提交审核，也未设为正式版。未进行双账号真机操作和登录后后台点击验收。

发布源码分支：三个仓库均为 `codex/unity-shared-ab-20261113`。Unity 实现提交 `a4fffd19`；goose-server 发布快照 `8abc6c58`；tasks 实现快照 `1fb0fc4d`，补充在线后台发布脚本 `b27737f2`。后两个仓库保留原工作区与其他任务未提交修改，没有切换其当前分支。

3D 玩法指标需通过 `ab_test_records.userId` 关联 `goose_game_records.userId`。现有动物消专用玩法、广告和留存统计不能直接代表 3D 效果；后台已说明这一范围。详细接口、业务接入和数据口径见 [接入文档](../../ab-experiments.md)。
