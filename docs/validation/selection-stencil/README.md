# 选中物品变黑排查（2026-11-05）

## 复现与结论

使用当前 CountrysideStove 的 Donut、实际游戏相机旋转、正交投影与 .3–1000 裁剪范围，在 Metal 离屏渲染中对比有/无 stencil 的目标。

- D24+S8（本机实际 D32+S8）：旧描边无新增纯黑像素。
- D16（无 stencil）：旧描边使 30,373 个原本可见像素变为纯黑，出现与用户截图相似的黑色主体和蓝色碎纹。
- 故障机制：Mask 无法写入 stencil 时，Outline 的 NotEqual 测试无法保护模型内部，随后不透明黑色描边覆盖模型。Glow 也进入模型内部。
- 尚未读取用户设备上的 framebuffer，故不能声称已确认该设备丢失 stencil 的具体原因。

## 兼容处理

ItemHighlight 在 WebGLPlayer 默认只绘制现有蓝色表面 Overlay，跳过依赖 stencil 的 Mask、Glow、Outline。其他平台保持原路径。此兼容处理牺牲微信/WebGL 端外发光和黑色描边，保留选中蓝色反馈，不为此增加全屏中间渲染目标。

## 验证

SelectionStencilValidation.Run 在两种深度格式下分别验证：选中像素发生变化、新增纯黑像素为 0、取消后残留像素为 0、AutoFind 反馈可见。结果见 report.txt；*-on.png 是旧路径，*-compatible.png 是修复路径。

运行：

```sh
/Applications/Unity/Unity.app/Contents/MacOS/Unity -batchmode -projectPath /Users/liuzhenhua/Desktop/code/unity-match-3d-puzzle -executeMethod SelectionStencilValidation.Run -quit -logFile /tmp/match3d-stencil-fix.log
```

本轮未构建、上传微信包，也未完成真机验证。

旧 BoardHighlightValidation 在编辑器调用中缺少 Rebuild/Awake 初始化，后续还引用已移除的 Goose_Pancake；排查时临时校正初始化以检查当前食物模型，之后恢复原文件。本轮以新的独立渲染对照测试为验收依据，不宣称旧验证全量通过。
