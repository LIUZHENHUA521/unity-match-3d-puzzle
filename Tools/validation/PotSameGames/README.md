# 同款游戏验证

首页右侧入口打开 `PotSameGames.prefab`，弹窗只展示疯狂动物消和一锅消不完。疯狂动物消跳转正式版 `wx85d24d5dbf72b423`（来自 crazyCombo/wechatGame/project.config.json）；一锅消不完的按钮文字为“当前游戏”，点击后提示“当前游戏，无法跳转”；最后一行“尽情期待”。

共用现有 `InviteSkinPS/弹窗背景.png` 和关闭图标，不重新导入。新增 `PotSameGamesSkin` 的入口来自“一锅消-主界面-排版02已命名.psd”，两张卡片和标题来自“一锅消-弹窗-同款游戏.psd”；源 PSD 未修改。卡片文字、标签、按钮沿用 PSD 的原始视觉；一锅消不完的按钮在临时副本中改为“当前游戏”，缩小字号后保持居中，点击区域仅覆盖卡片右侧按钮。

- 编辑器菜单 `Pot/UI/Build Same Games` 可重建弹窗及首页入口。
- `node Tools/validation/PotSameGames/navigation.test.cjs` 验证跳转目标、防连点、取消、失败、超时、旧回调和异常恢复。
- Unity batchmode 执行 `PotSameGamesValidation.Run` 验证首页入口、两条游戏、提示、关闭重开和三种屏幕比例，并在 `work/pot-same-games-validation/` 输出截图与报告。
- 微信真机跨小游戏跳转需要在微信环境验证；编辑器展示平台提示，不尝试跳转。
