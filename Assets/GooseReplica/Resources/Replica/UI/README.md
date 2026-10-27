# 可编辑 UI Prefab

停止运行后，双击这里的 Prefab，在 Prefab 模式编辑布局，保存后再运行即可生效。
不要在 Play 模式里修改后期待 Unity 自动保存。

| Prefab | 用途 |
| --- | --- |
| Home | 首页按钮、底部导航、背景 |
| GameHUD | 游戏内暂停按钮、倒计时、剩余数量、道具按钮 |
| PauseSettings | 暂停／设置弹窗、音乐音效震动、继续游戏、返回首页 |
| Result | 胜负／过关弹窗 |
| Themes | 主题选择 |
| Collection | 大鹅图鉴 |
| CollectionTab / PetsTab | 首页收藏／宠物标签页 |
| Reward | 奖励说明弹窗 |
| ModelSize | 模型大小调节弹窗 |
| PvpLobby | 好友对战大厅，包含连接前后两组按钮 |
| CollectionTray | 游戏中的七格收集槽外观 |
| Notice | 临时提示文字 |
| Diagnostics | 调试面板（普通游戏不显示） |
| UIRoot | Canvas、CanvasScaler 和页面容器 |

## 720 × 1560 预览与适配

页面 Prefab 的根节点使用居中固定锚点，尺寸为 720 × 1560，打开后就是手机设计画布。UIRoot 的 CanvasScaler 参考分辨率同为 720 × 1560，运行时按宽度缩放，页面根容器铺满实际视口。

暂停、结算、设置等完整面板放在 `Content - edit UI here` 下。短屏上 ReplicaUIFitContent 仅缩放这个容器以保证内容完整，按钮本身的位置、尺寸、图片不会被重写。GameHUD 使用顶部／底部锚点，首页使用相对锚点。

CollectionTray 是世界空间的七格收集槽，保留其实际 720 × 159 槽位画布尺寸；它不是一张全屏手机页面。UIRoot 的 Canvas 根尺寸由 Unity 根据 Game 窗口计算，参考分辨率在 CanvasScaler 上查看。

## 编辑方法

- 选中按钮节点，用 Rect Transform 改位置和大小，用 Image 改图片和颜色。
- 展开按钮的 Label 修改字体、字号、颜色、静态文字。
- 可以重命名和调整节点父子关系：ReplicaUIView 使用序列化对象引用，不通过节点名称寻找按钮。
- 不要删除功能节点、Button/Text 等必要组件，或清空根节点 ReplicaUIView 的 Bindings 引用。
- 按钮的点击事件由 ReplicaFlow / ReplicaHomeView 运行时绑定，Inspector 中 On Click 列表为空是正常的。
- 主题标题、倒计时、剩余数量、设置开关状态、结算提示以及随玩法改变的按钮文字属于动态数据，运行时会更新；它们的字体、颜色、图片、位置和尺寸仍来自 Prefab。
- GameHUD 底部道具按钮采用底部锚点；首页按钮采用相对锚点。不同屏幕高度由 Unity 锚点适配。首页仅 Safe content 容器和底部背景高度按设备安全区调整，不再用代码重设按钮位置。
- CollectionTray 是跟随游戏相机的世界空间 Canvas，根节点位置/比例由玩法计算；修改 Frame 和 Slot 子节点外观即可。七个槽位的物品落点由 Slot0…Slot6 子节点决定，显示和飞行动画共用这些坐标。
- 当前 Themes / Collection 保存了项目现有的三个主题。新增主题时，需要同步增加对应的节点和 Bindings。

## 结构与验证

ReplicaUIView.Load 只实例化 Prefab、让页面根容器铺满 Canvas；不会重新创建按钮或覆盖其布局。
ReplicaFlow 负责页面切换和玩法逻辑，ReplicaHomeView 负责首页事件和安全区，ReplicaTray 保留三维槽位定位。

Unity Play 模式下可运行菜单 Goose → UI → Validate prefab UI in Play Mode。
报告写到 Temp/ui-migration/validation.txt。验证涵盖 Prefab 引用、首页导航、暂停期间计时冻结、恢复、返回首页、设置开关、结算下一关、模型滑块和本地对战大厅状态；不发起网络对战。
