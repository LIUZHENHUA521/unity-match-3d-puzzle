# 加时道具验证（2026-11-10）

每次使用增加 180 秒，复活保留 300 秒。显示顺序移出、吸附、加时、洗牌；协议 type=3、inventory[3]、toolUsage[3]。素材来自 Photoshop 游戏 PSD 的 img_btn_clock，文字在临时副本中生成，源 PSD 未修改。

- Unity 编译及 18 项运行检查通过：连续加时、未授权拒绝、广告未完成/重复/失效回调、库存成功及失败退款、超时复活独立、结算/不限时状态拒绝、返回主页保留加时记录。
- goose-server 51 项测试通过：旧三项库存和 HTTP 协议、消费退款幂等、三/四项邮件、红包凭证核对、真实剩余时间和旧记录重试。
- adminGooseRecords 17 项测试通过，包含四项并发校验和旧后台三项编辑保留加时库存。
- 管理后台 Vite 构建通过；线上 index-CoN3de0K.js 与本地产物 SHA-256 相同：d839acaa293e946efb92604ff7e6d5988d9c9dbe5e2da281856d9fbec90ceb5b。
- 旧邀请奖励继续只发原三种道具。邮件可发送第四项。新客户端通过 X-Goose-Tool-Count: 4 读取四项库存；旧客户端保持三项响应。
- 记录新增 timeReviveUsed 并保存真实剩余时间；广告和复活沿用客户端上报信任边界，库存次数由后端消费凭证校验。
- 已部署：管理云函数 adminGooseRecords 为 Active（23:03:17 更新），管理后台线上文件与构建一致；游戏服务 goose-3d-pvp-016 为 normal、100% 流量。线上 /health 与 /invites/health 均 HTTP 200，数据库与登录配置 ready。
- 对应提交：Unity 2b80022；tasks ae7a6ef；server 4429d03、de17b9d。后两仓库已推送 master 后发布。

运行：先执行 `python3 Tools/add_time_inventory_mock.py` 启动本地 mock 库存服务（18765，首个 consume 成功、第二个拒绝、refund 返回 settled），在 Unity 停止播放后创建 Temp/run-add-time-check 并刷新。测试使用本地广告回调，不操作真实玩家库存。真实微信广告播放和新版客户端发布仍需小游戏体验版验证，本次未上传小游戏。
