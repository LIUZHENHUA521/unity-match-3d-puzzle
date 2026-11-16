# 大神回放服务端部署（2026-11-16）

- 目标：CloudBase `cloud1-1g0u68ow59705e3f` / `goose-3d-pvp`。
- 源码：`1a8135a`，已推送；部署完整 `services/goose-server` 工作区快照，78 个文件。
- 生效版本：`goose-3d-pvp-059`，normal，100% 流量；保留 1 CPU / 2 GiB、单实例、8080 端口及原环境变量。
- 镜像摘要：`sha256:f8b4e9fae312ca45721bd562cc8634f67aa3068c5a0fdaf7f2e00f8e2666cd74`。
- 云端源码回读：排除部署工具重写的 `cloudbaserc.json`，77 个文件与发布输入 SHA-256 一致；原工作区输入未变化。
- 新建并通过 listIndexes 回读确认 `pot_master_replay_fastest` 复合索引，覆盖筛选条件与 duration/id 升序。
- 169 项服务端测试通过；正式域名及默认域名的健康、数据库及登录就绪检查通过。
- 查询当天已公开且符合资源/格式条件的本科回放返回 0 条；没有修改录像公开状态，入口仍按超过 5 条的规则显示。
- 其他历史 worktree 和疯狂动物消独立 server/src 服务不属于本次目标，未部署。

发布输入清单、云端回读源码及健康检查原始结果保存在 `Builds/pot-master-release/`。
