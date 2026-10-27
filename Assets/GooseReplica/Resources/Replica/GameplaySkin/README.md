# 野餐游戏 UI 美术

来源：用户在 Photoshop 打开的《游戏中-野餐背景-蓝色已命名.psd》，画布 720 × 1564。
由 Photoshop 在临时文档副本中导出，源 PSD 未保存或修改；隐藏的篮子和占位图没有导入游戏。

- PNG 保留原始图层名称、透明边缘和图层效果。
- layout-source.json 记录导出时的原稿坐标，便于追溯。游戏 Prefab 按 720 × 1560 设计，顶部/底部锚点适配实际屏幕和安全区。
- image_number_circle-background.png 是数量徽章智能对象去掉数字层后的临时导出；真实数字由 Unity Text 显示。
- 静态“移出／吸附／洗牌”文字使用 PS 导出图片；倒计时使用 Unity Text。
- image_add_circle.png 保留美术原图。目前没有补充道具逻辑，正式关卡显示实际剩余次数 1/0，不显示无行为的“＋”。教程不限次数，不显示次数徽章，计时器显示 ∞。
- img_Blue_box1 / img_GreyBlue_box 是原稿中的叠放备选，当前使用最上层 img_Blue_box，它已经包含七个格子。

## 修改位置

- UI/GameHUD.prefab：设置、暂停、计时器、三个道具按钮。Header 和 Tools 容器仅负责安全区偏移，子控件位置可直接改。
- UI/CollectionTray.prefab：蓝色七格和底部工具栏背景。Slot0..Slot6 是物品落点，位置与画面中的七格一致。调整格子布局时同步调整这些 RectTransform。
- Scenes/CountrysideStove.prefab：野餐背景图片和原有黑锅。背景使用本次从 PSD 导出的裁切结果。
- UI/PvpGameHUD.prefab：保留原对战 HUD 的认输和对手进度。

不需要再运行导入菜单；直接编辑 Prefab 即可。导入器是这次迁移的工具，不会在运行时重新摆放 UI。
