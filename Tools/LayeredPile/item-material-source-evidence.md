# 原版物品材质来源证据（2026-10-16）

本记录只证明提取到的源数据；运行时渲染和验证结果由本轮对齐报告记录。**物品参数来自 iOS 1.11.03 的 model-bundles，不能直接称为微信版参数。** 本轮同时恢复了这些 iOS bundle 内 Metal 文本，可以用同一来源核对公式，减少跨平台拼接的推断。

## 覆盖范围

- 71 个模型，85 个 Mesh，102 个实际引用的 Material，213 个实际引用的 Texture2D。
- 85 个 Mesh 全部保留原始 tangent.xyzw，数量与 vertex 一致。无重新估算法线/切线。
- MeshRenderer 的原始开启、投射阴影、接收阴影标志保持不变。
- 材质保留 shader 名、具体 shaderSourceId、有效/无效 keywords、renderQueue、所有 float/int/vector、所有 texture（包含空引用）、各自 ST、instancing、GI、tag、disabled passes。
- 原有模型、mesh、material 的 ID 和贴图文件名稳定；新增字段不更改现有预制体几何/物理。

## 实际 shader 分支

| Shader | 有效局部关键字 | 材质数 |
| --- | --- | ---: |
| ToonLite1 | _ISUSEWORLDMATCAP_ON | 10 |
| ToonLite1 | 无 | 62 |
| ToonLite2 | 无 | 15 |
| ToonLite1 | _CUSTOMINVERTLIGHTDIR_ON | 1 |
| ToonLite1 | _FADECOLORDIRY_ON | 2 |
| ToonGlass | _ISUSEWORLDMATCAP_ON | 1 |
| ToonEffectFull | 无 | 1 |
| ToonLite1 | _ISSINGLECOLOR_ON,_ISUSEWORLDMATCAP_ON | 1 |
| ToonGlass | _ISSINGLECOLOR_ON | 1 |
| ToonGlass | 无 | 3 |
| ToonLite2 | _ISUSEWORLDMATCAP_ON | 2 |
| ToonLite2 | _ISSINGLECOLOR_ON | 1 |
| ToonGlassBack | 无 | 1 |
| BackShadow | 无 | 1 |

特殊材质：LatiaoPackFront 为 ToonGlass；LatiaoPackBack 为 ToonGlassBack；玻璃杯 GlassOutside 是 SingleColor 分支；手电筒 FlashlightGrass 是 WorldMatcap 分支；EggMa 1 使用 ToonEffectFull。不能全部替换为普通不透明 ToonLite。

## 法线贴图与采样设置

导出的 PNG 是原 ASTC 解码后的 RGBA，未转为普通 RGB 法线。iOS ToonLite1 片段先采样 `.yw`，再 `.yx * 2 - 1`，即用 **alpha、green** 还原 X/Y。因此应保持 `TextureImporterType.Default`，按源 `m_ColorSpace` 设置 sRGB，不让 Unity 二次做 NormalMap 打包。所有 85 个网格都有原 tangent 可供 TBN 转换。

源贴图设置：180 张 Bilinear/Repeat、32 张 Bilinear/Clamp、1 张 Point/Repeat；Aniso 全为 1。211 张只有 1 级 mip，两张分别有 8 和 9 级。每张原始 colorSpace、format、filter、wrapU/V/W、aniso、mipBias、mipCount 见 `Evidence/item-materials/textures-source.json`。这些是原运行时 Texture2D 设置，不是臆测 importer 默认值；导入器应按它们重建。

`m_SavedProperties` 包含已切换 shader 后遗留的 Standard 字段；“导出完整”不意味着每个遗留字段都参与原 shader 渲染。实际属性和公式须由对应 Shader 的 m_PropInfo / 程序确认。尤其不能因为遗留 `_IsUseNormalMap=0` 就省略已编译片段的法线采样。

## 可复核原始证据与重现

- `Assets/GooseReplica/OriginalModels/models.json`：用于导入的全部数据。
- `Tools/LayeredPile/Evidence/item-materials/material-parameters.json`：102 材质可读参数副本。
- 研究目录 `outputs/goose-analysis/item-material-evidence/materials-source.json`：完整原始材质 typetree。
- `outputs/goose-analysis/item-material-evidence/ios-exact-programs/`：25 个按来源 ID 分开的 Shader，分别保存 parsed.json、program.raw、程序 `.metal` 和 variants.json（blob index、pass、keywords 一一对应）。同名 Shader 在不同 bundle 的裁剪变体不同，禁止跨文件混用索引。
- `work/export_goose_models.py` 重新导出模型/材质/贴图；`work/export_item_shader_evidence.py` 重新导出逐来源 Metal 证据。

旧的 `shader-parsed-ios.json` 按 shader 名聚合，只可查属性，不能与按首个同名 shader 导出的 `ios-programs` 索引配对；准确对应以 `ios-exact-programs` 和每个材质 `shaderSourceId` 为准。
