# 3D 游戏共用红包链路：需求核对稿

状态：用户已确认暂时保持相同难度，按入口区分档位且完成两关发券。完整设计见 2026-10-31-red-packet-integration-design.md；本稿保留前期核对记录。

## 已确认规则

- 复用疯狂动物消现有红包管理、客服兑换、游戏圈任务审核、核销和付款链路。
- 本科局：2 元，沿用 easyPass，客服直接兑换。
- 清北局：10 元，沿用 pass，先完成游戏圈发帖活动并提交审核，再由现有任务付款链路核销。
- 现有兑换策略的凭证有效期为 7 天；沿用现有失效、付款状态查询和防重复核销行为，不另造独立余额或支付系统。
- 管理后台继续在 crazyCombo/tasks/admin-platform 中扩展已有红包页面，验证后部署。

## 代码核对结论

1. ReplicaHomeView 本科按钮调用 Begin(0)，清北按钮打开 ShowThemes；主题按钮仍调用 Begin(index)。当前 ThemeLibrary 仅有 daily 主题。因此两个入口最终都进入同一套第一关、第二关，并不存在已独立配置的本科/清北难度。
2. goose_game_records 只有 mode=solo、themeId、stage，不能从历史记录可靠反推 rewardTier。接入后必须显式记录档位和奖励规则版本；历史客户端上报记录不自动补发现金凭证。
3. 现有 redEnvelop 存有 type、money、openid、appid、createTime。新来源需补 gameId、源账号标识、recordId、runId、rewardTier、ruleVersion 与不可变奖励金额，保持旧凭证兼容。
4. 客服兑换目前从 kf_external_users.unionid 查询旧 user 表得到 openid；任务核销也直接比较 user.openid 与 voucher.openid。3D 登录目前仅保存 hash(appid + ':' + openid)，尚无可供该链路使用的原始身份映射。必须补充服务端可信身份关联，不接受客户端自报 openid，也不能把不同 AppID 下的 openid 直接混用。
5. 现有红包列表展示小游戏筛选器，但 listRedEnvelop 调用未带 appid；扩展时需真正贯通游戏筛选、用户查询和详情关联。
6. 旧 addRedEnvelope 的本科分支限制北京时间当天一次；清北分支检查具体关卡的通关记录。新游戏的奖励资格键需要结合已确认档位/关卡定义设计，使用事务保证重试不重复发券，不能照搬客户端请求或非原子的先查后写。
7. 通关记录目前是客户端统计上报，不能作为现金资格的唯一可信依据。独立发券模块必须绑定服务端签发的开局和奖励规则，检查结算及资格；结构校验不能被描述成完整防作弊验证。

## 待用户选择

- 推荐：本科、清北各有独立难度，各自完成整套挑战获得对应凭证。
- 或保留相同挑战，按入口确定奖励档位（需要用户明确接受相同难度对应不同金额）。
- 或第一关算本科、第二关算清北（会改变当前第一关为热身关的意义）。

确认此项后，补齐具体关卡资格、跨应用身份与支付映射、游戏圈任务来源校验、接口及测试设计；再按 brainstorming 流程审查实施设计。

## 只读核对来源

- Unity：ReplicaHomeView.cs、ReplicaFlow.cs、ThemeLibrary.asset。
- server/src/services/voucher-redemption.ts、voucher-redemption-policy.ts、wechat-kf-identity.ts。
- tasks/callFunctions/validateSubmission/voucherLifecycle.js。
- wechatGame/sheepServer/addRedEnvelope/index.js、transferCash/voucherPayoutGuard.js。
- tasks/docs/cloud-functions-live.md（2026-10-26 快照：上述相关函数记录为 active；实施部署前再通过 MCP 复核）。
