# Pot 缩放广告服务端部署（2026-11-18）

- 服务 goose-3d-pvp，环境 cloud1-1g0u68ow59705e3f。
- 提交 bac2b08 已推送；当前 services/goose-server 全部工作区内容纳入，83 个文件。
- 线上版本 goose-3d-pvp-067，normal，100% 流量；BuildId 2613074953。
- 保持 1 CPU / 2 GiB、单实例、8080 端口、原有环境变量及公网访问配置。
- 云端生效源码回读后，82 个文件 SHA-256 与发布输入一致；仅排除工具重写的 cloudbaserc.json。发布源工作区未漂移。
- 176 项服务端测试及 pot_zoom 客户端到 HTTP 入库联调通过；切流后正式/默认域名 health 和 invites/health 均返回 200，就绪正常。
- 前端未构建、上传或部署。同仓库独立 server/src 服务、其他历史 worktree 不属于本次目标，未发布。
- 证据位于 Builds/pot-zoom-release/。
