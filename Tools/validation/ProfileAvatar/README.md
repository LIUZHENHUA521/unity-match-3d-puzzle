# 微信头像加载回归

运行 `python3 Tools/validation/ProfileAvatar/run.py`，使用独立 Unity 2022.3 项目，不关闭或操作主项目编辑器。结果在 `work/profile-avatar-validation/result.txt`。

测试使用生产 `Match3DUserProfileView.cs` 和最小账号/界面依赖桩，通过真实 UnityWebRequestTexture 解码本地 PNG，覆盖 132/640 像素、超限图片、下载失败后的状态和重试、重复点击、取消、缺失地址及过期回调。测试不请求真实微信头像、不登录生产账号。文件 URL 仅由测试直接传入私有下载方法；生产入口仍验证 HTTPS 和 qlogo.cn。

2026-11-11：11 项 Unity 运行检查及 6 项 `Tools/wechat/test-profile.cjs` 桥接测试通过。

已修复失败的 URL 被误当作成功缓存、微信支持的 640×640 头像被 512 上限丢弃，以及下载失败无提示。失败日志前缀为 `[WechatAvatar]`，包含域名、请求结果、HTTP 状态及错误原因，隐藏完整头像 URL。

手机上仍显示默认头像的具体原因尚未取得真机错误；下载域名配置、网络错误、返回地址和响应格式仍需根据新包的真机日志确认。本次未构建或上传微信体验版。

微信头像尺寸说明：https://developers.weixin.qq.com/minigame/dev/api/open-api/user-info/UserInfo.html
