# 邀请得道具（2026-10-29）

## 编辑资源

- 弹窗：`Assets/GooseReplica/Resources/Replica/UI/InviteRewards.prefab`
- 图片：`Assets/GooseReplica/Resources/Replica/InviteSkinPS/`
- 原 PSD：`弹窗-邀请获得奖励.psd` 的可见 `03` 组。导出时使用临时副本，未修改原文件。
- 6 张透明 PNG，最大 592×878，合计 300625 bytes；裁掉透明边缘并无损压缩。没有重新导出主界面背景。
- Prefab 根节点沿用 720×1560，Content 是 720×1280 安全编辑范围；短屏整体缩放。按钮、背景、标题、奖励图示、说明和动态文本独立。
- 普通美术调整直接编辑 Prefab。`Goose Replica/UI/Build Invitation Popup` 会重建初始布局，**会覆盖该 Prefab 的手工调整**，仅用于重建。

## 运行规则

首页邀请按钮打开弹窗。微信分享携带 `inviter` 参数，服务器使用 `wx.login` 的 code 换取真实微信身份。新人首次注册时事务绑定邀请人、增加人数并发放三种道具各 1。已注册用户、自邀、重复打开、分享取消不会发奖。

新人定义为该邀请系统启用后首次登记微信身份。旧版游戏只有匿名 PVP 身份，无法事后识别旧游客是否玩过，不能据此承诺排除所有旧版玩家。

每局先使用原有免费次数，再扣永久库存。库存消费由服务端确认；待处理编号按用户保存，失败取消、重试、退款均幂等。网络异常最多两轮请求后解除冻结，未完成取消的编号在下次微信登录恢复，期间禁止新的库存消费。客户端不存可自行增加的奖励数。

编辑器只预览，不能模拟真实邀请发奖。`ReplicaInviteValidation.Run()` 仅用于本地 fixture 测试，会临时使用 127.0.0.1:8098；退出 Play 恢复生产配置。

## 服务端配置

服务端源码在 `crazyCombo/server/services/goose-server`（`crazyCombo/goose-server` 是软链接）。

服务端配置 `WECHAT_APP_ID`、`WECHAT_APP_SECRET`、`CLOUDBASE_ENV_ID`、`CLOUDBASE_APIKEY`。AppSecret 不能放入 Unity、分享链接或代码仓库。运行账号需要目标环境的数据库权限。

数据库集合 `goose_invite_users`、`goose_invite_uses`，只允许服务端访问，客户端读写规则应关闭。邀请奖励和消费使用 CloudBase 事务；数据库不可用时拒绝发奖，不降级成内存库存。生产配置完成后，应验证真实数据库并发事务及两个微信账号的首次进入发奖流程。

接口：POST `/invites/login`（code、inviter），`/invites/state`，`/invites/consume`，`/invites/refund`（requestId、type=0/1/2）。除 login 外均要求本服务签发的 Bearer token。原 `/session` 和 `/ws` 不受影响。

取消先到时写已退款终态，不增加库存；迟到的相同消费返回已取消。相同编号不同道具拒绝，消费凭证按认证用户隔离。已退款响应绝不能执行道具效果。

上线需先部署服务端及集合/配置，再构建微信客户端；真机验证分享、首次登录、返回刷新、扣库存和断网恢复。源码 PNG 大小不等于微信资源包增量，重新打包后仍需检查体积。

## 本地验证结果

- Node 服务端 16 项测试通过（含原 PVP 回归、HTTP 登录鉴权、并发幂等、先取消后迟到消费）。
- `node --test Tools/wechat/test-invitations.cjs`：冷启动/热启动邀请参数与分享桥测试通过。
- Unity 编译通过；本地 HTTP fixture 联调通过登录、新人奖励、免费优先、禁止绕过扣款、库存消费、无效效果退款、pending 恢复和关闭弹窗。
- `Temp/invitation-tests/unity-result.txt` 保存本轮结果。真实数据库与部署验证见下方记录；尚未进行双账号真机分享和新版微信打包验证。

## 后端部署验证（2026-10-29）

- CloudBase 环境：`cloud1-1g0u68ow59705e3f`；服务：`goose-3d-pvp`。
- 生效版本：`goose-3d-pvp-006`，状态 normal，100% 流量。镜像：`goose-3d-pvp-004-20261029014223`。
- 微信登录配置和专用数据库凭据已写入服务端环境变量；两个邀请集合均为 ADMINONLY。
- 公网 `/health`、`/invites/health` 返回 200，数据库事务读取与登录配置均就绪。无凭据库存请求及无效微信 code 均返回 401。
- 16 项本地测试通过；公网双人对战建房、同步牌组、出牌、重复出牌拒绝、撤销额度及断线结果回归通过。
- 未执行真实微信用户的新邀请发奖、数据库并发写入真机测试；需要新版客户端双账号验收。此次仅部署后端，未重新打包/上传微信体验版。
- 部署使用的本机临时密钥文件已清理；凭据未写入源码。
