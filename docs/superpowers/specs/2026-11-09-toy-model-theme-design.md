# 萌趣玩具模型替换

用户已确认：复用项目现有 23 种玩具，替换当前 23 种食物；只改模型，其他保持不变。

## 范围

沿用 CountrysideStove 现有入口与主题标识，不新增主题 UI。不修改场景、锅、背景、声音、运行时玩法、关卡数量、权重、层类型数、高度或时间。旧食物与原始玩具资源保留。

玩具：Bear、Bunny、Sheep、Horse、Duck、Fish、Bee、Dinosaur、Car、Truck、Minibus、Plane、Helicopter、Boat、Rocket、Robot、Doll、WoodToy、WaterGun、Bucket、BasketBall、RugbyBall、RollerSkate。

## 实现

从 Assets/Resources/CollectableObjects 的原始玩具 prefab 提取视觉资源，生成独立适配 prefab；旧 prefab 的 MeshCollider 不直接用于当前运行时，使用当前适配器支持的基础碰撞形状。材质、模型均来自本项目。

使用现有 ModelAdapterService 和 ThemeItemsEditorService 进行适配和逐槽替换，保留每个槽位的数量权重和第一关角色。新模型沿用被替换槽位的大小基准与音效配置，调整模型本身的碰撞以适合新外形。正式 SourceLevelCatalog 与 SourceLevelConfig 同步更新，避免重新导入撤销更改。

替换前备份当前配置（包含用户已有未提交修改）；不覆盖其他在途修改。生成失败时不切换物品池；切换前检查 23 个 prefab 均有效。

## 验证

比较替换前后关卡非模型参数；检查 23 个模型和材质引用、碰撞路径、第一关引用及适配目录。Unity 实际运行第一关和第二关，检查模型可见、堆叠、点击、三消和入槽，保存截图与验证结果。模型形状变化会影响局部堆叠，无法保证难度完全一致。
