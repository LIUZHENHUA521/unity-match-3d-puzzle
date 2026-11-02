# 复活弹窗

资源从 Photoshop「普通框-复活移出物体.psd」「普通框-复活延长时间.psd」临时副本导出透明 PNG。源文件保持不变，原图 720×1564；Unity 按 720×1280 重新排布，切图不拉伸。

Prefab：`Replica/UI/ReviveClear`、`Replica/UI/ReviveTime`。
重建菜单：Goose Replica → UI → Build Revive Screens。
验证入口：`ReplicaReviveValidation.Run`；真实关卡验证及 720×1280 截图输出到 `work/revive-ui/`。

满槽先提供移出全部槽内物体的复活，转入原暂存区而非删除；超时先提供 +300 秒复活。两种复活每关各一次，完整看完微信激励视频后生效。提前关闭、加载/播放失败不扣次数，可重试。关闭复活或相应次数耗尽，进入失败结算；失败页「继续」「重新挑战」仍然重新开始当前关。第一关自动晋级及 PVP 保持原流程。

弹窗用非缩放时间淡入和回弹，暂停状态下正常播放。入场和点击后禁用按钮，防止重复处理；复活沿用当前关卡及游戏记录。

广告位配置：`Assets/GooseReplica/Resources/Replica/RewardedVideoConfig.json` 的 `clearAdUnitId` / `timeAdUnitId`，可填写同一个合法微信激励视频广告位 ID。当前两个入口均配置为 `adunit-29119d7476389ec0`；缺少 ID 时不会发放复活。编辑器默认不模拟成功；验证脚本仅在 UNITY_EDITOR 下显式注入模拟广告结果。

广告桥接验证：`node Tools/wechat/test-revive-ads.cjs`，覆盖完整观看、提前关闭、缺失结果、重复回调、加载/播放失败、加载超时、取消及过期回调。最终实际广告展示需用本小游戏广告位在微信真机检查。
