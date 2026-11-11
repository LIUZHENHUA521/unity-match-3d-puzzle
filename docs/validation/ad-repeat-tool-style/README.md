# 重复激励视频与道具文字样式修复（2026-11-12）

旧广告桥接每次完成/失败/取消时销毁实例；微信同广告位返回单例，重复创建可能再次拿到已销毁实例。现按广告位缓存实例，所有入口共用，清理仅解绑本次 close/error 回调和加载定时器，保留请求 ID 防重复与旧回调隔离。错误同时记录广告位和原始 SDK 信息，便于真机定位缺货等平台错误。

`node Tools/wechat/test-revive-ads.cjs` 使用带销毁状态的单例模拟，覆盖连续三次播放、道具切换复活、未看完、错误和重试、超时/取消后新请求、旧回调与 Promise、并发拒绝、不同广告位以及监听清理。`node Tools/wechat/test-reward-share.cjs` 验证现有分享链路。

加时说明使用与其他道具相同的 Image 路径；新透明 PNG 沿用洗牌原稿文字图层描边，本机缺失原微软雅黑粗体，采用苹方 Semibold，字体来源和尺寸见素材目录 README。动态按钮加粗为 32 号白字，增加不透明蓝色 2px 描边，分享/免费切换保留样式，并避免复活切换时重复创建按钮文字。

Unity 场景验证命令：

```sh
/Applications/Unity/Unity.app/Contents/MacOS/Unity -batchmode -projectPath /Users/liuzhenhua/Desktop/code/unity-match-3d-puzzle -executeMethod RewardAcquisitionValidation.Run -logFile Logs/ad-repeat-tool-style-validation.log
```

广告和分享使用本地模拟；微信真实广告填充及真机连续观看仍需体验版验证。本次未上传小游戏。

结果：Unity 批处理退出码 0，33 项检查全部通过（见 `unity-result.txt`），加时弹窗实际截图见 `add-time-ad.png`。截图已人工检查：白色说明与蓝色描边完整、无裁切，按钮文字完整居中且描边可见。`git diff --check` 通过。

## 0.1.58 上传（2026-11-12）

源码提交 `a95c9d28` 已推送。独立 Unity WebGL 构建成功，发布包 AppID/方向/资源分包校验通过，微信开发者工具 `upload` 成功且回执有效，总包 30,805,451 字节。219 个运行脚本、桥接与相关资源逐文件核对，见 `source-0.1.58.json`；上传回执与构建文件哈希见同目录 JSON。

公众平台 `mp.weixin.qq.com` 被浏览器管理员策略拒绝访问，未尝试绕过。当前仅确认开发版本上传，须用户在公众平台「版本管理」将 0.1.58 设为体验版。未提审，未发布正式版。
