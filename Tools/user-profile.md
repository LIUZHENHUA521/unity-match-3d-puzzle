# 微信用户资料展示

## 行为

首页左上菜单第一项「个人名片」打开 UserProfile。沿用美术PSD，去掉相机和编辑按钮，只有关闭、同步微信资料、复制ID三个操作。

昵称、头像、性别和地区使用微信小游戏 `getSetting` / `getUserInfo` / `createUserInfoButton`。已授权自动读取；未授权由用户实际点击原生透明授权区域，拒绝不会阻止游戏或账号资料展示。微信未返回字段显示“微信玩家”“未提供”和美术默认头像，不根据头像猜测性别，不自动定位。

用户ID为现有服务器账号 inviteCode，显示缩略但复制完整值，不展示原始openid。注册时间使用服务端users.createdAt，旧服务器未返回时显示“未提供”。它表示当前账号系统中的创建时间，不追溯未被记录的旧版游玩时间。

微信个人资料只保存在当前弹窗内存；不上传服务器，不写PlayerPrefs，不影响鉴权/奖励。每次重新打开重新检查授权。关闭销毁微信原生按钮、头像请求和纹理，旧弹窗回调不会污染新界面。头像下载仅允许HTTPS qlogo.cn及其子域，不跟随重定向，失败保留默认头像。

## 编辑及验证

- `Assets/GooseReplica/Resources/Replica/UI/UserProfile.prefab`：720×1280可编辑布局，短屏缩放。
- `Assets/GooseReplica/Resources/Replica/ProfileSkinPS`：原PSD临时副本导出，9张PNG共352579 bytes（约344KiB），最大655×722。没有导出相机、编辑图标或示例资料。
- `Goose Replica/UI/Build User Profile`：重建Prefab，会覆盖该Prefab手工布局；普通调整直接编辑Prefab。
- `Tools/wechat/test-profile.cjs`：模拟授权、拒绝、首次点击、坐标转换、复制、页面销毁与迟到回调竞争。
- `ReplicaProfileValidation.Run`：在Play Mode和本地8098 fixture验证展示及截图，结果在work/profile-validation。不退出编辑器；测试结束重启Play恢复生产配置。

当前微信官方网页读取不可用，桥接依据项目内微信SDK `Packages/com.qq.weixin.minigame/Runtime/wechat-default/unity-sdk/userinfo.js` 的原生按钮实现。不同微信版本或授权状态可能限制返回字段，不保证每位用户都能返回真实昵称、性别、地区。

上线需部署后端新增createdAt返回，并构建新客户端；头像域名需符合小游戏下载域名配置。微信真机授权、拒绝及图片下载仍需实测，模拟测试不能替代真机授权验收。本次没有部署，也没有获取或上传真实用户资料。

2026-10-30 本地验证：Unity 展示及交互 15 项通过，微信桥接模拟测试 6 项通过，后端测试 23 项通过。720×1280 截图在 `work/profile-validation`，截图中的授权资料为测试数据。


## 后端部署完成（2026-10-30）

服务 `goose-3d-pvp` 的版本 `goose-3d-pvp-007` 已 normal、100% 流量；部署代码提交 `fe91e7c`（已推送 crazyComboServer）。三个邮件集合已创建并复核 ADMINONLY；GOOSE_MAIL_ADMIN_KEY 已配置在服务端，保留原微信/数据库凭据及 0.25 核、0.5 GB、固定 1 实例规格。

23 项本地测试通过；公网健康检查、数据库与登录配置就绪，未登录读邮件返回 401，无运营凭据发放返回 403；正确运营凭据搭配无效空内容返回 400（仅验证配置，不发邮件、不发奖励）。公网双人对战建房、同步、重复点击拒绝、撤销和断线结果回归通过。

此次仅更新后端，未构建或上传微信客户端；真实微信资料授权和邮件领取仍需新版客户端真机验收。临时运营密钥文件已清理，密钥只保留于云端服务配置。
