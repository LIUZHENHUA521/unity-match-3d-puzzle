# 厨房主题背景

厨房背景作为未启用的备选素材保留。乡村运行时已恢复加载 `CountrysideStove.prefab`，并恢复原相机视野。

- 备选背景（当前未加载）：`Assets/GooseReplica/Resources/Replica/Scenes/Kitchen.prefab`。
- 启动场景仍是 `Assets/Scenes/GooseReplica.unity`。
- `Goose > Kitchen > Rebuild Kitchen Background` 可重新按布局代码生成背景（会覆盖此背景的手工摆放）。
- 外观使用 Polytus Cookware 的黑色 `wok.fbx`，Kenney Furniture Kit 的水槽、橱柜、灶台、冰箱、小电器和植物；台面和灶台底座由基础网格搭建。
- 两套资源均为 CC0。出处及下载文件校验值见 `asset-provenance.json`；Kenney 原许可随包保存。
- 装饰不带碰撞体，不进入 Collectable 层；沿用当前关卡的独立物理边界。锅体开口约 8 单位，底部 y=-3.18、口沿接近 y=0；以现有堆叠区域为基准摆放。
- 试作时的厨房镜头放宽逻辑已撤回；当前所有主题沿用原正交视野。
