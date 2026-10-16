> 后续镜头调用链与阴影移植见 [camera-shadow-alignment.md](camera-shadow-alignment.md)。下文“尚未恢复阴影”和“动态镜头待补”是本次调查前的记录，以新报告为准。

# 主题外观与镜头对齐（2026-10-16）

本次改动作用于 GooseReplica 单人第二关，未发布、未上传体验版。依据是微信版 WX 2.1.12 的场景资源、编译后程序与已保存组件值；没有把静态资源当作真机逐帧验证。

## 已接入主项目

| 当前主题 ID | 原版场景 | 外观资源 |
|---|---|---|
| daily | CountrysideStove | 铁锅、灶台背景、锅盖、碗筷、勺等子场景 |
| toys | GroceryStore | 绿色篮筐、玻璃柜台和商品背景 |
| travel | Barbecue | 烧烤架、炭火、台面、调料、蒲扇等子场景 |

导入 17 个网格、24 份材质记录和贴图，保留原有层级、顶点、法线、切线、局部坐标、旋转、缩放、Sprite 像素比例与锚点。资源在 `Assets/GooseReplica/SourceScenes`；生成的三个 prefab 位于 `Resources/Replica/Scenes`。

外观与上一轮导入的碰撞体均使用原始世界坐标，取消旧锅在第二关的额外放大和旧镜头适配覆盖。隐藏旧锅及把手的 Renderer，继续禁用旧锅碰撞体，外观 prefab 不重复添加 Collider/Rigidbody。退出时恢复旧锅和镜头。

原版自身使用简化空气墙，而非完全贴合锅面网格的碰撞体。因此“同一坐标、同一比例”不代表每个像素轮廓都与物理表面重合；本次没有为追求轮廓重合而改变原版空气墙。

## 镜头证据

`data.unity3d / sharedassets0.assets`：Camera pathID 486，Transform 359，父 MainLevelCenter 316，祖父 RotaCenter 339。记录见 `Evidence/camera-settings-wx.json`。

- MainLevelCenter：位置 `(0, 0, 0.5)`，旋转四元数 `(0.6560590267, 0, 0, 0.7547096610)`，即 X 方向约 82°。
- 相机局部位置 `(0, 0, -30)`。运行时使用父变换合成后的世界位置，避免直接把局部坐标当世界坐标。
- 正交基础尺寸 6.5，near 0.3，far 1000，全屏 viewport；SolidColor、透明黑背景、HDR 关闭、culling mask 503。
- `CameraControl.Start`（sub.wat j92873）调用 `ScreenMa.GetScreenOffset(1,0)`（main.wat j9468），适配公式为 `6.5 × max(1, (9/16)/屏幕宽高比)`。
- 9:16 的尺寸为 6.5，9:19.5 为 7.921875，3:4 为 6.5。窄长屏保持横向范围，宽屏扩展横向视野。

实现：`ReplicaThemePresentation.cs`。进入第二关暂时停用旧 CameraFitter；退出恢复变换、投影参数、背景、剔除层和原组件启用状态。

## 材质：恢复了哪些，仍缺哪些

起初 UnityPy 的 shader 导出只有属性和 pass 元数据。进一步解压包内 LZ4 shader blob，找到了 GLES 编译程序，已保留选中程序于 `Evidence/scene-shaders/*-selected.glsl`。

新增 5 个 shader：ToonLite1、ToonLite2、ToonLite2World、ToonLite2Emissive、ToonLite2EmissiveWorld。片元计算按原版方向光分支机械转换到 HLSL，保留原版颜色空间计算、AO、分段明暗、MatCap 反射、法线、渐变/边缘光，以及炭火的时间变化公式；材质数值与贴图来自原资源。TBN 输入来自原网格法线/切线。使用保存的 Directional Light 朝向，不修改已有道具的光照系统。

尚未完整恢复：原版投射/接收阴影分支、额外光源分支和运行时灯光旋转。BackShadow 只保留 Cull Front 等基本表现，其标准反射光照仍是降级实现；内置 Default-Material 也保留降级材质。这些不计入逐像素还原。

## 验证

结果见 [visual-camera-validation.txt](visual-camera-validation.txt)。在隔离 Unity 项目完成，随后将资源及配套 `.meta` 一起复制回主项目：

- 三主题 67 / 21 / 32 个节点逐一对照原版局部变换，包括同名子节点；全部通过。
- 外观/碰撞共用祖先的世界矩阵一致；无重复物理体。
- 三种屏幕比例投影、相机位置、主题进入/退出、旧锅恢复检查通过。
- 材质在本机 Unity/Metal 可用，无 shader 编译错误；渲染了三主题开局堆叠与空场景。
- 重跑生成、显隐/长大、物理参数、点选/入槽回归，全部通过；每主题仍创建 300 件。

[锅体预览](visual-previews/theme-0-empty.png) · [篮筐预览](visual-previews/theme-1-empty.png) · [烧烤架预览](visual-previews/theme-2-empty.png)

这些是 Unity 相机直接渲染的世界画面，不含完整 UI；并非微信真机截图或设备 FPS 结论。

## 明确保留的差异

- 当前只改第二关。第一关、首页、PVP 和 UI 位置继续使用本项目原有流程。
- CameraControl 的运行时移动/旋转动画（含 ChangeHeightAndRotaAnim）尚未全部映射，本次对齐的是保存的基础姿态与 Start 屏幕适配。
- 物品本身的材质、用户尺寸倍率以及随机数序列并非本次主题外观移植范围，不能据此声称整体画面已经完全一致。
- 未做本次微信 WebGL/真机渲染与触控适配验证；未执行构建或发布命令。

## 复查入口

`SourceSceneImporter.Import()` 从已保存的 visuals.json 生成资源，无需联网。
`SourceVisualValidation.Run()` 在游戏播放状态下验证主题和镜头。
`VisualSceneBatch.Run()` 仅用于隔离项目的 batchmode：导入、进入场景、运行验证、输出预览并退出 Unity。不要在用户正在编辑的主项目调用该批处理入口。

导出与 shader 转换脚本备份于 `Evidence/visual-export-scripts`，保留原研究目录路径；它们不是构建时自动执行的脚本。
