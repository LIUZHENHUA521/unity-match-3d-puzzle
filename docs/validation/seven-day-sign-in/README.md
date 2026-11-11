# 七日签到验证

## 已实现

首页签到入口、可领红点、原 PSD 拆图的签到弹窗、今日高亮、已领蒙层、动态奖励文字、签到成功后的现有道具领取弹窗。

奖励顺序：移出×1；加时×1；吸附×1；移出×2；洗牌×2；吸附×1+加时×2；四种道具各×2。一轮共18个道具（移出5、吸附4、洗牌4、加时5）。累计签到，漏签不重置；北京时间零点刷新，第七天领取后次日开启新一轮。

服务端入口是 `crazyCombo/server/services/goose-server/src/sign-in.mjs`，复用微信身份和现有用户库存。签到记录与库存同一事务写入，无需新增集合。奖励数量由服务端决定；请求只携带日期、轮次、天数，重复请求返回原收据。

## 复现

```sh
cd /Users/liuzhenhua/Desktop/code/crazyCombo/server/services/goose-server
npm test
cd /Users/liuzhenhua/Desktop/code/unity-match-3d-puzzle
python3 Tools/validation/SignIn/run.py
```

Unity 验证应在当前项目未被其他编辑器进程打开时运行。脚本启动随机本地端口，使用生产 `Invitations`、`SignIn` 和内存事务仓库；在空场景中启动生产客户端组件及实际 Home/SevenDaySignIn/RewardReceived 预制体。禁止访问云环境，不发真实奖励，不保存原场景。临时账号仅存在于本地进程。

## 结果

- 服务端89项测试全部通过，其中10项为签到测试：7天奖励和循环、漏签、北京时间零点、跨日幂等、20路并发、鉴权与伪造数据、重启持久化、实际库存消费、库存上限、事务回滚、时钟倒退、HTTP及旧客户端库存形状。
- Unity 2022.3.62f3c1 完整项目编译通过，真实客户端与本地生产服务逻辑联调通过，详见 `result.txt`。覆盖双击、领取成功但响应丢失后的自动重试、请求失败不加库存、重开、完成7天、恢复前台/计时刷新、401及切换账号。
- 截图：`day1-long.png`（720×1560）、`day1-short.png`（720×960）、`day7-ready.png` / `day7-complete.png`（720×1280）。按钮边界和可见文字长度均已验证，并目视检查。
- Photoshop 原稿保持未修改，所有 UI 部件在预制体中可分别编辑。

## 交付边界

服务端已于 2026-11-11 部署为 `goose-3d-pvp-027`，normal、100% 流量；健康、接口鉴权、公网对战回归及部署源码比对通过，见 `deployment.md`。微信小游戏未重新打包/上传，真机签到仍需新版客户端确认。编辑器可预览奖励，真实账号奖励不会由本地 PlayerPrefs 伪造。当前没有新增管理后台功能。
