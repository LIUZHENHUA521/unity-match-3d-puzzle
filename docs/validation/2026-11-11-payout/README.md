# 注册玩家打款与收款授权标题（2026-11-11）

授权标题被蓝色背景带遮挡：修正 CommonDialog 的兄弟层级，并在生成器及运行时防止背景带覆盖文字。启动授权 PlayMode 检查全部通过，见 authorization-result.txt；startup-prompt.png 为编辑器渲染，真机观感仍需复测。

微信开发版本 0.1.52 上传成功，总包 30,466,794 字节；Unity GOOSE_WECHAT_EXPORT_OK、AppID、分包校验通过。源码 8fe2aca，独立发布包 Builds/Wechat/release-0.1.52-payout-title，回执 Builds/Wechat/upload-info-0.1.52.json。排除工作区尚未提交的个人资料修改。未提审。公众平台受既有浏览器策略限制，尚未设为体验版，需用户在版本管理手动选择 0.1.52。

共用管理后台 /admin/#/goose-users 增加“打款”：当前商户授权校验、0.30–200.00 元、备注、二次确认、原单查询和最近 20 笔记录。管理员必须同时具备 /goose-users 与 /payouts 权限。通过现有 adminAuth 服务验证登录，当前角色在服务端复核，不复制脱敏密钥。查询玩家授权使用 transferCash 的服务端接口，避免客户端授权函数用调用者身份替换目标玩家。

后台源码位于 crazyCombo/tasks，独立提交快照构建。部署 adminGoosePayments、transferCash 及静态后台。新增 goose_admin_payouts，权限 ADMINONLY，userId/createdAt 复合索引。16 项自动测试通过；线上无效登录被拒绝，真实玩家只读授权查询通过；静态入口与 JS/CSS 文件哈希与发布包一致。未实际打款，也未验证完整真钱到账。浏览器工具被 Work 设置禁用，未完成登录后的点击验收。

发布过程中 transferCash 首次包遗漏 waterVoucherSource.cjs，配置调用失败；已从修改前原线上 ZIP 补回原文件并重发，恢复配置/授权查询。没有改动该依赖的业务逻辑。
