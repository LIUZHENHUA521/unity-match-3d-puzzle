# 物品 Toon 与配套环境光（2026-10-16，未发布）

本轮把当前项目 71 个物品 prefab 的简化 Lambert 材质替换为其来源包的 Toon 着色流程，并恢复此前暂缓的场景环境光。物品证据来自 iOS 1.11.03，场景环境光来自 WX 2.1.12；不把两个发行版本当成同一份真机录制结果。

## 来源和实现

- 71 模型 / 85 mesh / 102 材质，按 `shaderSourceId` 追溯到对应 bundle 的 shader。25 份逐 bundle shader 记录保留了原始 parsed 状态、关键字、Metal 程序与 blob 索引。
- 直接转换原 iOS Metal 顶点与片段程序，不再依赖“贴图乘一个颜色”的 Lambert 近似。普通物品 8 个 ToonLite1/2 变体保留 World MatCap、Single Color、FadeY、Custom Invert 的实际编译分支。相同变体在多份来源 shader 中的片段哈希一致后才合并。
- 玻璃、反面玻璃与鸡蛋分别使用 ToonGlass、ToonGlassSingle、ToonGlassWorld、ToonGlassBack 和 ToonEffectFull。保留透明混合、剔除、深度写入与颜色通道写入状态；不把透明包装改成不透明 Toon。
- 恢复源材质已声明的数值、颜色向量、贴图和 UV 缩放/偏移、渲染队列。源保存字段中不被该 shader 声明的旧 Standard 参数单独计数，不当成有效着色参数。
- 213 张贴图按原色彩空间、mipmap 开关、wrap、filter、aniso、bias 导入。原法线 X 存在 alpha、Y 存在 green；以普通 RGBA 纹理导入，避免 Unity NormalMap 二次编码破坏通道。PNG 为原压缩贴图解码结果，本轮使用不再二次压缩的导入设置；这并不等于原 ASTC 存储格式或内存占用。
- 85 个 mesh 的原始切线恢复，用于原法线/TBN 计算。只更新同 GUID 的 mesh 切线与材质资源，不重建物品几何、坐标或物理配置。
- 继续使用原版方向光阴影衰减的位置，并保留已有投射/接收特例。项目已有点选弱光与选择描边作为单独兼容层保留，不把这层兼容逻辑说成新发现的原版 shader 源码。

主要实现：`OriginalModelImporter.ApplySourceMaterials()`、`Shaders/SourceItems`、`ReplicaSourceLighting`。材质/mesh 资源修改复用现有 GUID，prefab 的引用不需要替换。

## 环境光

重新解包 WX `data.unity3d / level0 / RenderSettings 55`，与之前的证据文件一致：Flat（枚举 3）、HDR sky=(1.4148107767, 1.4148107767, 1.4148107767)、ambient intensity=1、reflection intensity=0.46；原 equator/ground/subtractive、无 skybox/custom reflection、反射分辨率128/bounces1及关闭雾的原参数一并恢复。

物品 Toon 的主表面使用其 `_IndirectColor` 等自有明暗参数，不再走旧 Lambert 的环境光乘法，所以不需要为了防过曝私自压低原环境色。BackShadow 等标准光照路径则使用场景环境。

设置只在来源场景的现有生命周期内生效；结束或销毁时恢复先前环境、反射纹理、灯光/质量、雾和 SH。包内序列化 ambientProbe 的零值不代表运行时应强制零 SH，未添加无证据的 DynamicGI 更新。

## 验证与边界

本机 Unity 2022.3 图形 batch 退出码 0，结果见 [item-toon-ambient-validation.txt](item-toon-ambient-validation.txt)：

- 102 份材质的 shader/队列、3053 项数值、1099 项向量、612 个贴图槽/ST 通过来源校验，2264 个未声明的历史保存字段不计入有效参数。
- 213 张贴图导入设置、85 个 mesh / 47352 个原始切线、71 个 prefab / 90 个 Renderer 引用均通过。
- 单独渲染全部 71 个物品（包含玻璃和鸡蛋），无着色器错误。点选状态改变 136160 个像素，取消后差异像素为 0；这个诊断布局不用于衡量真机视觉强弱。
- 环境光原值、重复 Begin/End、销毁后非默认反射纹理与 27 个 SH 系数恢复通过。
- 三主题镜头/外观、投射和接收阴影、生成坐标、物理、显隐长大、点选/入槽回归通过。

预览：[全部物品](item-toon-previews/all-items-toon.png)、[点选效果](item-toon-previews/all-items-selected.png)、[乡村](item-toon-previews/theme-0-pile.png)、[小卖部](item-toon-previews/theme-1-pile.png)、[烧烤](item-toon-previews/theme-2-pile.png)。它们是本机直接渲染的世界画面/诊断排布，不是真机截图；主题图未包括 UI。全物品顺序见同目录 CSV。

编译过程中仅修复了 Metal 单参数向量构造的 Cg 写法与一个阴影插值槽冲突，没有为通过检查而调整原材质数值。源码、生成器与逐 bundle 程序记录随项目保留在 `Evidence/item-toon-port`。

未发布、未上传体验版。

当前场景只有已确认的单方向光，ForwardAdd 迁移其方向光分支；没有添加未使用的 Point/Spot/Cookie 变体。GPU 精度、Unity 版本、阴影采样、运行时 GI、压缩纹理解码/mipmap 重建仍可能造成像素差异，尚无原版真机逐帧对照。不声称完整游戏所有材质或所有光源组合逐像素一致。

详细导出字段见 [item-material-source-evidence.md](item-material-source-evidence.md)。
