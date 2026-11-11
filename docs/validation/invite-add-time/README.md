# 邀请奖励适配加时道具（2026-11-11）

邀请弹窗使用现有四种道具图标和名称，显示顺序为移出、吸附、加时、洗牌。服务端新邀请事务对四项库存各加 1，旧三项库存自动补齐；历史邀请不补发。

验证完成：
- goose-server `npm test`：79/79 通过，包含并发重复邀请、新旧客户端响应、旧库存升级、加时消费与幂等退款。
- `node --test Tools/wechat/test-invitations.cjs`：1/1 通过。
- Unity 2022.3.62f3c1 主项目编译及 `Match3DInviteValidation.BuildAndCheck` 通过；已重建实际 InviteRewards.prefab，检查四项图标、名称、顺序和按钮引用，并渲染 720×1280、720×960 截图人工检查。
- 本地验证不写真实玩家库存。未进行双微信账号真机验证。

产物：`invite-1280.png`、`invite-960.png`、`layout-result.txt`。Unity 日志：`Logs/invite-add-time-build.log`。

发布状态：后端提交 `ffaa246` 已推送并通过 CloudBase MCP 发布为 `goose-3d-pvp-026`，normal / 100% 流量；下载线上源码及 Dockerfile、依赖清单共 15 个文件，与提交快照逐文件一致。运行配置与凭据保持不变。自定义域名和默认域名的健康/登录依赖检查均通过，未登录库存请求正确返回 401。完整无敏感信息证据见 `deployment.json`。

客户端 Prefab 已更新但尚未构建/上传新版小游戏。此功能不涉及管理后台改动。
