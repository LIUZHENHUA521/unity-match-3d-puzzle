# 吃货派对导入（2026-11-09）

来源：用户提供的「已选模型 (7).zip」，23 个 Quaternius 食物 OBJ。包内来源、授权及模型清单原样保存在 IncomingModels/FoodieParty。

- 原始资源：Assets/Match3D/IncomingModels/FoodieParty。
- 适配资源：Assets/Match3D/Generated/ModelAdapter/Party_*。
- 当前 CountrysideStove 关卡物品池切换为吃货派对，主题标题同步更新，原场景与玩法沿用。此操作未新增主题切换入口。
- 已有 23 个玩具和 23 个美味厨房适配资源及目录条目完整保留。
- 关卡数量、权重、类型数、高度、时间与导入前一致；源 JSON 与正式关卡资产同步更新。
- 每个模型 120–1278 三角面，1–6 个材质；沿用原包颜色与几何，使用基础拾取/实体碰撞及新模型独立 ID。
- Unity 编辑器验证通过 198 条断言：材质与碰撞、第一关三消、300 物品第二关、23 种模型入槽、首个模型撤回重选和关卡重试。
- 未打包或发布微信版本；形状变化可能影响局部堆叠形态。

备份：work/foodie-party/before。
截图：work/foodie-party/first-stage.png、second-stage.png、tray.png。
验证入口：Play 初始化完成后向 Temp/foodie-party-command 写入 validate。
