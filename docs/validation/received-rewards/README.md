# 领取奖励弹窗

实现于 2026-11-10。可编辑预制体：`Assets/Match3D/Resources/Match3D/UI/RewardReceived.prefab`。

## 调用

必须先完成服务端发放，再调用展示方法；弹窗与确认按钮不会再次写库存或发放红包。

```csharp
// 按项目道具顺序：移出、吸附、洗牌、加时。非正数自动过滤。
Match3DFlow.Instance.ShowReceivedTools(new[] { 2, 3, 1, 4 });

// 同时展示道具和红包。红包金额必须来自服务端。
Match3DFlow.Instance.ShowReceivedRewards(
    new[] { Match3DRewardItem.Tool(0, 2), Match3DRewardItem.RedPacket(10m) },
    buttonText: "返回首页",
    onConfirm: () => Match3DFlow.Instance.Home(),
    receivedAt: serverCreateTimeMilliseconds);
```

按钮默认“领取”，空白字符串也回退为默认值；更改文字不自动改变行为，导航等操作由 `onConfirm` 提供。时间是可选毫秒时间戳，以 UTC+8 显示，未提供就隐藏。不要传入美术稿的示例时间。`Match3DRewardItem.RedPacket(null)` 只展示“红包”，用于服务端已确认凭证但详情暂不可读的情况。

成功邮件领取已接入 `Match3DInvitations.MailOperation`，自动展示邮件的全部奖励；失败和已领取邮件不弹。红包结算成功由 `Match3DGameRecords` 接入，使用凭证 ID 查询实际金额和创建时间，同一账号同一凭证本次运行只提示一次。展示层不会更改发放、资格、兑换或付款规则。

同一批奖励放在一个弹窗；不同批次依次展示。弹窗独立于当前页面，关闭后保留下层邮件或其他弹窗。服务端延迟回包发生在正在进行的游戏/复活流程中时，排队至结算或回首页后展示，避免挡住操作。

## 素材与字体

按钮、红包、光效、标题均来自 PSD，按钮文字动态渲染。46 px 粗体、4 px 蓝色描边；数量 40 px。PSD 标注字体在本机 Photoshop 本身也缺失，使用项目现有中文字体匹配字号和粗细。详见 `RewardSkinPS/README.md`。

## 验证

1. 本地运行 `python3 Tools/reward_popup_mock.py`。
2. 在已打开 Match3D 场景的 Unity 中运行菜单 `Goose Match3D → Validate Received Reward Popup (local fixture)`。
3. 报告和截图输出到 `Temp/reward-popup-validation/`。验证启动的 Play 模式会在结束后退出；仅使用本地模拟接口，不发放真实道具/红包。

本次 Unity 2022.3.62f3c1 编译与 35 项运行验证通过，详见 `validation.txt`。涵盖单红包、2/4/5/9 个奖励、混合奖励、溢出滚动、可配置按钮、重复点击、队列、下层弹窗保留、游戏中延迟展示、真实客户端邮件请求与模拟服务器成功/失败/幂等响应、红包查询与缺失详情回退。截图覆盖 720 × 1560、720 × 1280、720 × 960。

验证使用本地接口，尚未上传微信小游戏包或进行真机领取验证。
