# 回放缓存修复发布 0.1.97 / 服务端 064

2026-11-17 用户授权部署后完成服务端部署和微信客户端上传。

- 服务端 `goose-3d-pvp-064` 已上线：normal、100% 流量、HasTraffic=true、IsReleasing=false。BuildId 2613020698，RunId multi_tenant_1xTAp61v32CkqT。
- 目标服务全部当前源码提交 cf0c07d 已推送；发布输入 82 个文件。线上源码回读核对 81 个文件 SHA-256 全部一致（CloudBase 自动生成的 cloudbaserc.json 单独排除），且与原工作区相同。
- CPU1 / Mem2 / Min1 / Max1 / Port8080、环境变量键集合、公开访问与网络配置核对未改变。环境变量值由部署工具保留合并，未读取或记录明文。
- `https://9nanimal.cn/health` 与 `/invites/health` 均返回 200；服务、数据库和登录配置健康。
- 微信客户端 **0.1.97 上传成功**，AppID wx74f2d8722c02a6f4。开发者工具回执总包 31,458,608 字节：主包 2,463,362；数据分包 22,669,571；代码分包 6,325,675。以微信工具实际成功回执为准。
- 客户端源码提交 fba174d5 已推送 master；完整当前 Assets / Packages / ProjectSettings / Tools/wechat 输入共 6,227 文件记录 SHA-256。329 个运行时代码和插件文件与隔离构建目录逐一一致，隔离 bundleVersion=0.1.97；上传前后全部登记源码不变。
- 当前资源与既有 0.1.96 发布压缩规则一起纳入，没有为本次包体省略回放修复或新增画质降级。Unity WebGL 导出 GOOSE_WECHAT_EXPORT_OK；Brotli 和加载封面无损优化完成后复制独立上传快照，上传前后快照哈希一致。
- 原修复的 175 项服务端、5 项桥接、Unity 长局/缓存/暂停续压/荣耀/首页回放验证已通过；发布脚本 5 项安全校验通过。此次未新增手机真机连续多局验收。
- 没有提交微信审核或发布微信正式版。没有将广告分析 outputs、server/src 另一服务的未提交改动或历史其他 worktree/分支混入发布；本次两个目标的当前工作区有效源码、资源、配置无遗漏。

本次修复改善后续录制与已有完整文件的补传；无法恢复之前根本没录下来的帧。

上传目录 `Builds/Wechat/release-0.1.97-pot-replay-cache`，回执 `Builds/Wechat/upload-info-0.1.97-pot-replay-cache.json`；完整发布证据 `work/pot-replay-release-0.1.97/`，精简记录 `docs/validation/pot-replay-cache-recovery/release-0.1.97.json`。
