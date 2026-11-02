# 邮件系统

## 编辑界面

- `Assets/GooseReplica/Resources/Replica/UI/Mailbox.prefab`：信封、可滚动邮件列表、刷新与更多。
- `CommonDialog.prefab`：通用两按钮弹窗。`ReplicaCommonDialog.Configure` 可设置标题、正文、两个按钮文字和动作。邮件中点确认才领取，返回及关闭只退出详情。
- `HomeMenu.prefab`：左上角菜单。邮件在第三项，设置在第二项。个人名片、客服、提现沿用未接入提示，本次未实现这些业务。
- `MailSkinPS/`：从三份用户 PSD 直接导出，原文档未修改。设计容器720×1280，单张最大578×940，PNG像素无损重压缩。
- `Goose Replica/UI/Build Mail System` 重建上述三个 Prefab，会覆盖其美术手工调整；普通调整直接编辑 Prefab。

## 领取和数据

道具数组顺序为 `[移出, 吸附, 洗牌]`，复用邀请系统库存。领取与库存加法同一服务端事务，凭证按用户和邮件唯一；断线、重复点击、重新登录不重复到账。取消不发奖，打开详情只标为已读。

所有登录、邮件和道具请求通过 ReplicaInvitations 串行执行。401 后需关闭详情并点击刷新重新微信登录；网络失败可原邮件重试。编辑器默认只显示提示，不伪造真实微信身份；fixture 验证另行显式启动。

未读或有效未领取附件触发红点，服务端汇总全部分页。每页20条。全服邮件仅发送时已注册账号可见；定向用户必须已注册。到期后不能首次领取，仍可读；过期30天后发送新邮件时会从目录清理。目录上限200封（含保留的过期邮件），达到上限明确拒绝新发放；历史模板和领取 ID 不重用。

## 运营发放

服务端位于 `/Users/liuzhenhua/Desktop/code/crazyCombo/server/services/goose-server`。路由均在 `/invites/mail/`：`list`、`read`、`claim` 使用现有微信 Bearer token；`send` 使用独立 `X-Mail-Admin-Key`。不要把密钥放入客户端。

服务器配置 `GOOSE_MAIL_ADMIN_KEY`（至少32字符）。CloudBase需要以下ADMINONLY集合，加上已有的users/uses：

- `goose_invite_mail_catalog`
- `goose_invite_mail_messages`
- `goose_invite_mail_receipts`

发送示例 JSON：

```json
{"id":"launch-gift-20261030","target":"all","title":"官方道具礼包","body":"感谢支持！确认后即可领取。","rewards":[2,3,1],"expiresAt":1796054400000}
```

`expiresAt` 为毫秒时间戳，必须处于未来一年内。`target` 可为 `all` 或玩家 inviteCode；公告将 rewards 设为 `[0,0,0]`。同一 id/内容重试成功，不重复投递；相同 id 不同内容返回409。

在服务端目录运行 `node tools/send-mail.mjs https://服务地址 /绝对路径/邮件.json`，密钥从进程环境变量读取。此工具会实际发放邮件，测试使用本地 fixture；本次未向线上玩家发信。

## 验证

- `node --test test/*.test.mjs`：邀请、邮件、PVP 回归。
- `node test/fixture-mail.mjs`：只监听127.0.0.1:8099，独立内存数据库，无生产凭据。
- Unity `ReplicaMailValidation.Run`：本地 fixture 登录、菜单红点、列表、取消、领取、重复领取、分页、已领取禁用、关闭输入恢复及空列表；720×1280截图在work/mail-validation。

上线顺序：创建受保护集合、设置服务端密钥、部署服务，再构建客户端；需要微信真机双账号验证。此文档不是线上已部署证明。

## 本次验收（2026-10-30）

22项服务端测试全部通过；Unity编译、Prefab构建和15项本地HTTP/UI联调通过，包含长正文滚动、取消不发奖、确认入库、重复领取不重发。截图已逐张检查并修复智能对象裁切导致的按钮缺角。

16张透明PNG共663042 bytes（约648 KiB），最大578×940，Unity默认导入上限1024、关闭mipmap。图片只做临时副本渲染、裁切及压缩，没有修改原PSD。

本轮未创建生产数据库集合、未设置生产运营密钥、未部署服务、未发送线上邮件、未上传微信体验版；新版客户端实际联网依赖后端部署，微信双账号真机验收尚未执行。


## 后端部署完成（2026-10-30）

服务 `goose-3d-pvp` 的版本 `goose-3d-pvp-007` 已 normal、100% 流量；部署代码提交 `fe91e7c`（已推送 crazyComboServer）。三个邮件集合已创建并复核 ADMINONLY；GOOSE_MAIL_ADMIN_KEY 已配置在服务端，保留原微信/数据库凭据及 0.25 核、0.5 GB、固定 1 实例规格。

23 项本地测试通过；公网健康检查、数据库与登录配置就绪，未登录读邮件返回 401，无运营凭据发放返回 403；正确运营凭据搭配无效空内容返回 400（仅验证配置，不发邮件、不发奖励）。公网双人对战建房、同步、重复点击拒绝、撤销和断线结果回归通过。

此次仅更新后端，未构建或上传微信客户端；真实微信资料授权和邮件领取仍需新版客户端真机验收。临时运营密钥文件已清理，密钥只保留于云端服务配置。
