# 3D 通关红包实施状态（2026-10-31）

## 已完成并发布

- goose-server 提交 `2754e1b`，CloudRun `goose-3d-pvp-009`：微信身份留存在服务端、在线资格、两关/版本/时间/道具消费凭据校验、按档位额度、事务发券、重复请求返回原券、我的红包查询。
- admin 提交 `063c445`：原红包菜单新增来源/档位/金额/兑换状态、来源账号、关联挑战；闯关详情关联凭证。查询复用 adminGooseRecords，按 `/redenvelops` 或 `/goose-records` 分别鉴权。
- 新集合 goose_reward_runs、goose_reward_claims、goose_reward_catalog、goose_reward_config、game_identity_links 均 ADMINONLY；runs 有 owner/issuedAt、owner/tier/rewardDate 索引。
- goose_reward_config/current：enabled=false，ruleVersion=2026-10-31-v1。尚未发放任何实际凭证。
- 后台：https://cloud1-1g0u68ow59705e3f-1312757046.tcloudbaseapp.com/admin/#/redenvelops

## 本地客户端

本科/清北入口申请资格；普通收藏回玩不申请资格；两关、重试保持档位；消费道具记录请求编号；资格失败提示后普通游玩。独立 RedPackets.prefab、“我的红包”列表、状态和复制凭证，微信复制走 WX.SetClipboardData。没有构建/上传新的小游戏包，Unity 工作区原有大量未提交修改，未混入提交。

## 关键接入阻塞（不能开启发券）

原任务 H5 AuthGate 允许输入 userId 或 URL openid 作为本地身份，不能用于认证收款人。webSession 函数创建的是 web_ 匿名账号，也不能用于微信收款认证。

已向用户询问可复用的真实微信网页授权入口，尚待提供；不以昵称、客户端 openid 或输入 ID 自动绑定。双端确认绑定、自动 unionid 关联尚未实施。现网 `task.voucherGameId=goose3d` 查询为空，3D 游戏圈活动还未配置。

shared/voucherLifecycle 来源白名单/关联/金额校验以及 server 客服策略已有本地准备修改和测试，尚未部署。必须在接入认证后补齐提交时可信身份校验、最终 transferCash 身份映射和商户 AppID 校验、旧公共红包查询隔离、绑定不可变、并发绑定及付款回归，才能部署共享付款链路。不得仅因资格/后台已上线就开启发券。

## 验证

- goose-server 31 个测试通过，其中 4 项新测试覆盖幂等并发、两关顺序、两档额度、越权、过期、消费凭据及退款互斥。
- adminGooseRecords 4 个测试通过，覆盖未登录/过期/禁用/权限不足以及两个菜单权限隔离；线上已授权 vouchers 查询成功。
- 共享 voucherLifecycle + voucherAccess 44 个测试通过；现有客服兑换 11 个回归测试通过（模拟支付，无真实转账）。
- 后台从提交快照构建，通过 MCP 部署；远端 index.html、JS、CSS 的 SHA256 与构建匹配。
- Unity C# 编译完成；RedPackets prefab 引用检查通过；新增 WebGL 分支独立编译无错误。真机登录/兑换未验证。
- 新版服务健康检查200，/rewards/runs 与 /rewards/list 无 token 返回401；发券开关维持关闭。
