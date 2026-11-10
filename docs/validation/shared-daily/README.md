# 每日同题验证（2026-11-11）

隔离工程 DailyOpeningWorkshop.RunValidation 通过：使用真实吃货派对模型生成 15 / 300 个物品，保存后重建两关，逐物品核对 ID、类型、位置、朝向、缩放、可见状态；检查静置稳定、失败重试、首次选择恢复物理、试玩不上传正式记录和分享日期/档位。导出的约 126 KB 文件通过服务器布局校验。

验证基于提交版本的 InputController 和 BoosterManager，不纳入工作区其他未提交修改。第一次冷启动在 Unity 打开场景时挂起，改为编辑器初始化后再打开场景，后续两次完整验证成功。预览图按竖屏画幅重新排布槽位。

后台相关 Node 测试 18 项通过，游戏服务 Node 测试 54 项通过，包括共享关两阶段红包结算、旧协议拒绝时无新局写入、并发多人同题、日切和候选校验。共用后台从已提交基线加本次文件的干净目录 Vite 构建通过。

以上不等于微信/iOS 真机验证，未验证跨设备物理运行逐帧一致；承诺的是同一开局快照，操作后允许物理路线分岔。

## 部署核验

- Unity 功能提交 `c8c70fe`，服务端提交 `a7a63c9`，管理端/云函数提交 `7614ac4`，均已推送。
- `adminGooseRecords` 更新后为 Active，更新时间 2026-11-11 01:07:07；未登录调用新候选动作返回 unauthorized。
- 专用服务 `goose-3d-pvp-017` 已承接 100% 流量。`/health` 与 `/invites/health` 均返回 200，数据库与登录配置正常。下载在线版本核对 daily-levels、contract、rewards、records 均与提交内容一致；原环境变量、CPU、内存、实例数和端口保留。
- 后台 index.html 和引用的 index-BcPnxZF9.js 已部署，HTTPS 获取内容与干净构建逐字节一致。未覆盖其他项目的静态编辑器资产。
- Unity WebGL 构建通过 GOOSE_WECHAT_EXPORT_OK；AppID、方向、资源分包验证通过。微信开发版本 0.1.46 上传成功，总包 31,249,111 字节，回执 `Builds/Wechat/upload-info-0.1.46-shared-daily.json`，独立包 `Builds/Wechat/release-0.1.46-shared-daily`。
- 未提交审核，未确认设置为体验版；没有自动创建或启用正式 shared-v2 排期。正式客户端可用后，后台确认客户端就绪再发布未来关卡。原有两个用户未提交源码改动保留，未纳入本次构建和提交。
