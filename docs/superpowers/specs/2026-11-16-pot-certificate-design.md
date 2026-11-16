# 一锅消通关奖状与红包衔接

## 已确认需求

本科局、清北局最终通关后先展示奖状，玩家点击「领取红包」后进入现有红包兑换流程。用户已批准手动确认方案，并允许复用疯狂动物消素材及数据库表。本文件细化复用边界，尚未实现或部署。

## 核对结果

- Unity 入口：`Match3DFlow.FinishSoloResult/Result`；第一关成功直接 NextStage，第二关目前展示 ResultSuccess。
- 红包：`Match3DGameRecords.Upload` 收到 voucherId 后调用 `Match3DRewards.NotifyReceivedVoucher`，后者异步查询金额并直接展示奖励，尚无奖状等待机制。
- Cocos 参考：`client/assets/resources/prefabs/Pre_certificate_big_item.prefab`、`PreCertificateBigItem.ts`、`LevelModal.ts`。已有卡片、横幅、盖章、按钮入场及动画跳过行为。
- 图集引用：`resources/images/Certificate.plist`、`0314.plist`、`resources/newSpriteAsset/newCertificate.plist`、`newhomeIcons.plist`。应只导入实际依赖图片，正确处理图集旋转、裁边、原始尺寸，不导入整个素材库。
- 2026-11-16 CloudBase 只读核验：环境 cloud1-1g0u68ow59705e3f；saveCertificate/getMyCertificates 均 Active，最近修改为 2026-11-15。
- 共享表 userCertificates 已存在 certificateKey 唯一索引、openid + claimTime 索引。旧查询只按 openid 过滤，旧录像关联 replay_records。
- 游戏服务端位于 crazyCombo/goose-server；rewards.mjs 已按 Bearer 身份、开局快照、道具凭据校验结算，发券、限额、记录均有幂等逻辑。存储映射尚未包含 userCertificates，需显式映射，不能落入默认 goose_invite_ 前缀。

## 推荐方案与替代方案

推荐复用图片与 userCertificates，由 Unity 重建奖状 UI，由一锅消服务端签发自己的证书。可复用既有存储，又保留账号、对局和录像边界。

仅复用图片、本地展示也能满足顺序，但无法可靠恢复奖状。新建独立证书表更易隔离，但在用户允许共享且已有唯一索引的情况下没有必要。Cocos prefab/TypeScript 无法直接作为 Unity 组件使用。

## UI 与顺序

1. 本科/清北第二关获胜，立即显示对应通关奖状；显示昵称、档位、通关日期、用时。服务器未确认时不伪造正式证书编号。
2. 复用奖状底图、横幅、对应印章；重建卡片入场和盖章动画。按钮「领取红包」，动画期间首次点击可快进，后续确认只处理一次。
3. 通关上报仍立即发生，红包发放时机及限额规则不变。当前对局的到账通知先缓存，不能顶掉奖状。
4. 玩家确认且服务端结算已完成时，展示本局红包及「联系客服兑换」。确认早于响应时展示等待状态；允许返回首页，结算队列继续重试，不让玩家无限卡住。
5. 无奖励额度等情况下保留有效通关奖状，显示服务端原因；无效对局不签发正式证书。断网显示待同步状态，不宣称奖励已到账。
6. 异步结果按 owner + runId + recordId 匹配。换号清理展示状态；旧对局回执不得覆盖新奖状；历史待处理红包保留现有「我的红包」访问能力。
7. 热身第一关、失败、PVP、回放不展示正式奖状。编辑器本地预览可以展示样式，但明确为预览且不写证书、不发奖励。

## 共享数据与兼容

- userCertificates 中的一锅消记录使用现有持久化业务标识 gameId=goose3d，并保存 sourceAppid、sourceUserId、sourceOpenid、runId、recordId、档位、日期、耗时及展示字段。内部新增代码采用 Pot/pot 命名。
- 一锅消证书不填写疯狂动物消使用的顶层 openid，真实身份放在 sourceOpenid；因此现有按 openid 查询的收藏列表不会读到新证书。所有一锅消读取必须同时限定服务端确认的游戏和账号。历史字段及数据不迁移。
- certificateKey 采用带 pot 命名空间、AppID、服务端用户 ID、runId、recordId 的稳定哈希；文档 ID 同样确定化。同一局重复上报返回同一证书，不覆盖其他局或旧游戏证书。
- 证书由现有通关结算事务生成，资格/关卡/时序/道具校验通过且第二关胜利才签发；发券额度不足不抹去有效通关荣誉。资格过期或其他永久校验拒绝显示「未通过验证」的本地结果，不继续显示待同步、不签发正式证书；仅可重试的网络失败显示待同步。
- 在发券配额判定前完成证书资格判断；结算事务统一提交证书、记录及发券结果。响应增加可选 certificate 字段，旧客户端忽略即可。结算重试的提前返回分支也必须返回已签发证书。
- 新实现仅为上线后的新结算签发证书，不自动补历史证书。预览、回放和旧 terminal 记录不补造奖状。
- 不直接让 Unity 调用 saveCertificate，也不接受客户端指定的 sourceUserId、sourceOpenid 或游戏来源作为鉴权依据。
- 不复用疯狂动物消 replay_records 关联逻辑。一锅消只保存本局精确记录关联，未来收藏入口可通过 pot_replays 解析；本次不增加收藏页、跨游戏奖状合并或后台菜单。

## 实现边界

- Unity：新增 Pot 奖状视图/构建器和对应资源，调整结果展示、结算回执数据及红包展示协调。沿用现有 UI 缩放和模态动画体系。
- 游戏服务端：新增 pot 证书构建模块、显式集合映射，接入 rewards 结算及幂等响应。不调整红包金额、额度、兑换账号绑定、现有接口路径或业务标识。
- 疯狂动物消：素材只读复用，旧云函数原则上无需修改。实施前检查共享表安全规则及其他写入者，确认不会覆盖带命名空间的证书。
- 已发现另一写入者 `wechatGame/sheepServer/elementaryReward/index.js`，使用 elementary-certificate 文档 ID 命名空间和 elementary_ 证书键。回归验证须覆盖小学局证书，并验证旧 openid 查询不返回 pot 证书。
- 本次需求不是发布指令，不自动上传微信或部署服务；开发完成后明确客户端、服务端生效所需的发布步骤。

## 验证

- 服务端：本科/清北、第一关/失败不签发、同局并发重试唯一证书、重复响应证书稳定、额度耗尽有证书无红包、无效对局不签发、跨账号拒绝、旧证书不覆盖、旧客户端兼容。
- 客户端：快慢网络两种顺序、重复点击、断网重试、切账号、返回首页、旧回执到达新局、无奖励原因、回放隔离。
- Unity 编译、Play Mode 检查与奖状截图；核对透明边缘、文字、档位印章、按钮及常用屏幕比例。最终以真实结算契约验证，不把本地 UI 预览视为线上领奖成功。

## 设计审查

2026-11-16 独立 spec reviewer 审查通过；补充明确永久拒绝状态及小学局证书写入兼容性。待用户审阅后进入实施。
