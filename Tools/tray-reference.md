# 底部收纳槽参考与实现

参考用户提供的 Unity demo：Assets/Scenes/TripleMatchCanoe.unity 的 MatchingPad，由一个 Image.Type.Sliced 背景及七个 Cell Image 构成；MatchingPad.cs 把物品送到各 Cell 的 RectTransform 中心。

本项目只借用槽位结构和 rectangle_mini_red.png / rectangle_mini.png 素材（单独复制为 Replica/Tray/TrayBase、TrayCell，保留原始文件）。ReplicaTray 创建世界空间 Canvas，让现有 3D 道具显示在槽位前方，无需改成 demo 的 2D 飞行图标。隐藏旧 Stack 的贴图平面及碰撞器，单底板 + 七格，整体宽 680、格距 90、格子 86x88（720 逻辑宽）。以屏幕底部 310 点和安全区定位。InputController 从同一槽位组件读取落点和尺寸比例，维持原有三消与撤回逻辑。

TrayValidation 实际 PlayMode 验证旧平面隐藏、八个 UI Image、不拦截点选、入槽中心、撤回、三消、七格不同物品判负、回主页隐藏及清场，全部通过。预览在 Builds/Wechat/TrayPreview。测试截图为编辑器渲染，不代表 iOS 真机帧率。
