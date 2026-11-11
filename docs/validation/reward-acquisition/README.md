# 分享／广告交替获取验证

2026-11-12，Unity 2022.3.62f3c1 批处理场景验证成功退出，`result.txt` 为本轮完整结果。

```sh
/Applications/Unity/Unity.app/Contents/MacOS/Unity -batchmode \
  -projectPath /Users/liuzhenhua/Desktop/code/unity-match-3d-puzzle \
  -executeMethod RewardAcquisitionValidation.Run \
  -logFile Logs/reward-acquisition-validation.log
node Tools/wechat/test-reward-share.cjs
node Tools/wechat/test-revive-ads.cjs
```

覆盖前置与循环、免费优先、关闭广告/分享的分支、实际道具弹窗与按钮、跨道具共用序列、复活独立序列、清槽与加时复活共用序列、失败重试、双击及重复回调、跨关卡旧回调、暂停恢复、免费不扣库存与不推进序列、重新加载配置保留次数。微信桥接使用 Node VM 模拟 API，覆盖分享前后台返回、停留阈值、异常、超时、并发、取消与监听清理；视频覆盖完整观看、提前关闭、加载/播放失败与旧异步回调。

Unity 验证使用临时独立账号，关闭后台客服和账号轮询，分享和视频由 Editor override 注入；没有调用真实微信分享或广告播放，没有上传体验版。分享的 3 秒返回判定不证明分享送达，需要真机检查前后台事件行为。

截图：`tool-share.png`、`tool-ad.png`、`revive-share.png`。
