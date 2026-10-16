# 首页换皮资源

- 入口：`Assets/GooseReplica/Scripts/ReplicaHomeView.cs`，由 ReplicaFlow.Home 创建；奖励金额在视图常量集中维护。
- 小图源：用户提供的 `/Users/liuzhenhua/Desktop/抓大鹅素材/`，当前使用两枚大按钮、设置、红包及三枚 Tab 图标/底色/顶线；复制到 `Assets/GooseReplica/Resources/Replica/HomeSkin/Icons`。源 `img_ redPacket.png` 重命名为 `redPacket.png`。
- 背景源：`/Users/liuzhenhua/Desktop/code/crazyCombo/client/assets/resources/background/image_Home_bg.jpg`，按用户要求复用，与截图一致。资源保持独立，不打入小图集。
- 首页图集：`HomeUI.spriteatlas`，1024 上限，padding 4，禁旋转/紧密打包，包含 Icons 文件夹。HomeSkinAssets 在编辑器导入及构建前创建缺失图集。原图可单独替换。
- 字体：已有 GooseSans（Noto Sans CJK SC 子集）补充本次文案字形，保留原许可证。
- 功能：本科局 Begin(0)、清北局 ShowThemes；三个底栏页可切换；收藏和宠物暂为占位页。旧收藏方法和本地进度仍存在，结算仍可进入原图鉴。
- 红包：首页两档金额和说明弹窗，仅展示；无余额、结算、提现或服务端写入。
- 验证：HomeSkinValidation.Run 在隔离工程实际进入 PlayMode，验证入口、Tab、弹窗与返回，并通过相机渲染导出 720x1560、720x1280、768x1024 截图；不代表真机帧率验证。

- 0.1.8 底部 Tab：btn_menu_selected / btn_menu_normal 导入配置左右各 4px border，UI Image 使用 Sliced 和 pixelsPerUnitMultiplier=2，中心随布局拉伸、边缘保持细线；修复五 Tab 素材拉宽为三 Tab 后的深色粗边。原始图片未改动。
