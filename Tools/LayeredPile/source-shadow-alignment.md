> 物品 Toon 与环境光已继续对齐（2026-10-16）：见 [item-toon-ambient-alignment.md](item-toon-ambient-alignment.md)。下文“物品仍为 Lambert、环境光暂缓”的说明已被本轮替代；保留版本、图形后端和运行时 GI 的验证边界。未发布。

# 原版阴影对齐（2026-10-16，未发布）

本次依据 WX 2.1.12 已下载资源和编译后的 GLES 程序；属于包内静态证据，不代表在原版手机抓取了实时画面。

## 灯光与质量

`data.unity3d/globalgamemanagers` QualitySettings 12：当前档位 1（Medium），`shadows=1`（HardOnly）、`shadowResolution=1`（Medium）、`shadowDistance=40`、`shadowProjection=1`（StableFit）、pixelLightCount=1、shadowNearPlaneOffset=3。资源的单级联数值为 1，Unity API 的不切分模式设置为 0。

实际启用的场景级灯是 `level0` Light 56 / Transform 49（SceneChange/LightAngle/Directional Light）：白色、强度 0.38，mask 471，Hard，strength=1，bias=0.05，normalBias=0.4，nearPlane=0.2，local rotation=(-0.076522976,-0.822887182,0.557803869,0.076522991)。父节点身份旋转。

另有 MainScene 下 Light 565（mask503），但其 LightAngle 父节点在资源中 inactive；不擅自启用。MainSceneLight 566 的 mask512 只照 layer9，当前游戏主题节点为 layer0/7，不把首页专用灯混进来。

`ReplicaSourceLighting.Begin/End` 保存并恢复原项目灯光与质量；只在现有主题玩法生命周期挂接。不会把改动永久写入项目质量设置。

## 接收和投射

`export_theme_visuals.py` 现在逐个导出 renderer 的 cast/receive 和 GameObject.layer。Importer 不再统一关闭阴影，也不统一开启：例如某些背景收阴影但不投影，部分装饰双关。三个主题共有 39 个已导入 renderer。

ToonLite1/2/2Emissive（含 World MatCap）由已有 GLES fragment 机械迁移，加上源 `DIRECTIONAL + SHADOWS_SCREEN` 分支的运算位置：

```
cell = clamp((dot(N,L) + BaseCellOffset) / BaseCellSharpness, 0, 1)
cell *= shadowAttenuation
```

之后原版 `_ShadowAddColor`、`_HighLightShadowMask`、`_MetcapShadowMask`、Rim mask 等沿原编译式继续计算。没有把最后像素整体染黑，炭火自发光保留原版组合顺序。Unity 内置 AutoLight 提供当前图形后端的 shadow 采样及距离淡出，主光方向改用运行时实际灯光 uniform，避免阴影方向与固定材质方向不一致。ShadowCaster 保留源 Cull Back、法线偏移。

`BackShadow` 的原版两个 Pass 都是 Cull Front；恢复固定 Standard BRDF（albedo0.5 / metallic0 / smoothness0 / alpha1）。原编译结果的 diffuse=.389541864 和 gamma dielectric=.220916301 与此固定 Standard 参数吻合。内置表面编译器提供平台匹配的 SH、反射探针和阴影，而不是上一版无光照白材质。尚未声称其跨 Unity 版本结果逐像素相同。

## 明确保留的差异

- 原资源的 Flat ambientSkyColor=(1.4148108,1.4148108,1.4148108)，reflectionIntensity=0.46。**暂不全局覆盖**：现有食物仍是 Goose/Original Item Lambert+emission，而不是原版 Toon；直接覆盖会人为使食物过曝。原版 Toon 的自有间接光参数已按材质保留；BackShadow 的 SH/反射仍受项目环境设置影响，需物品 shader 一起迁移后统一收尾。
- 另核实了当前导入的 71 个 iOS 原物品 prefab，共90个 renderer：83个 On/receive，7个特殊玻璃/包装/Shadow 节点。Importer 与现有 prefab 的 flags 按逐节点证据补齐；这是当前模型包的标记，不能把 iOS 标记等同所有 WX 版本。物品 shader 本身仍非原版 Toon。
- 本轮未重建 ForwardAdd 点灯/聚光灯分支：本次三个主题活动光源为一个方向灯，其他分支无活动实例证据。
- 阴影纹理分辨率的实际像素、GPU 精度、图形 API、Unity 版本会影响边缘；不能称逐像素复刻。

证据位于任务 `outputs/goose-analysis/shadow-evidence/` 与 `visual-camera-evidence/programs/`，包含原字段、renderer 列表、导出与迁移脚本。验证入口 `SourceShadowValidation.Run()`：39个环境+90个物品原开关、编译状态、参数恢复、独立遮挡块的阴影开关像素差，以及 receiveShadows=false 是否移除地面投影。验证结果由统一 batch 记录。


本机完整验证已通过，结果见 [camera-shadow-validation.txt](camera-shadow-validation.txt)。阴影投射开关探针改变 247 个地面像素，接收开关清除至少 95%；6 个 shader 编译、39 个环境及 90 个物品 renderer 检查通过。随项目保留的证据位于 `Evidence/shadows`、`Evidence/shadow-programs` 与 `Evidence/lighting-settings-wx.json`。
