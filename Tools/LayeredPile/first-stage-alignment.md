# 第一关对齐记录

依据 `outputs/goose-analysis/wechat-LevelData.json` 的 `MainLevel / _level=1 / _index=1` 行，第一关使用主题自己的原始模型池，12 个物品、2 种类型、目标高度 6、限制高度 4、场景 `CountrysideStove`，`_limittime=[0]`。

运行时现在按主题取四个（烧烤主题为三个）原始候选模型，用同一套 `ChallengePlan.GenerateSource` 类型选择、`ReplicaSourceEnvironment.Position(0,6)` 坐标采样、原始模型归一化、原始碰撞体、质量/重力/阻力和主题场景边界。第一关没有第二关的分层显隐优化、对象池飞槽或高度扫描，但保留原版的 3 秒准备阶段和三消流程。

主题映射：

- CountrysideStove：`Goose_Tomato`、`Potato`、`Orange`、`SaltedEgg`
- GroceryStore：`Goose_ChiliSauce`、`Banana`、`GlassCola`、`WahahaMilk`
- Barbecue：`Goose_Pancake`、`BBQSkewers2`、`GrilledCorn`

`limittime=0` 被解释为不启用倒计时；第一关 HUD 不显示 `00:00`，第二关及对战仍按各自时间配置运行。

验证：隔离副本 `ReplicaValidation.Run` 全部通过，包含 12 件、2 种类型、无倒计时、主题场景选择、物理落稳、第一关三消通关、进入第二关及原有第二关回归检查。未发布。
