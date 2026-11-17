# Pot 独立高亮 Shader（2026-11-17）

已完整替换 Outline、Glow、Overlay 三份 Shader 的实现。保留 Resources 路径及原 meta/GUID，Shader 名称改为 Pot/Highlight/*。使用常规的屏幕空间壳扩张、视角柔化、纹理采样与透明叠色，未继续使用反编译临时变量或原 GLES 转换脚本。

选中保持蓝色、吸附保持青蓝色双目标提示；维持现有脉动、选择取消和工具超时生命周期。Overlay 保留纹理明暗、关闭深度写入并用深度测试保护遮挡物表面。Glow 和既有 ScreenHalo 保留外围 AlwaysOnTop 提示，这不等于整套效果都被前景深度裁掉；遮挡检查只保证覆盖的内部表面不会被染色。编辑器几何路径保留黑色细描边，外层为蓝色柔光；微信路径仍是 Overlay + ScreenHalo。

图像每组三列从左至右：未高亮 / 选中甜甜圈 / 吸附汉堡与披萨。它们是 Unity 使用当前真实模型、生产 ItemHighlight 组件和新 Shader 的渲染结果，不是游戏 UI 截图或微信真机截屏。

- `wechat-compatible-comparison.png`：D16 深度、无 stencil 路径。
- `stencil-comparison.png`：带 stencil 的编辑器路径。
- 同名 `-perspective-` 图：透视投影补充检查。

验证环境 Unity 2022.3.62f3c1 / Apple Metal。44 项断言通过，包括 Shader 支持/编译、正交/透视、两种深度路径、可见高亮与外围光晕、无新增黑色主体和错误粉色、取消/过期无残留、吸附双目标共存及取消不误清工具高亮、前景遮挡物内部保持不变。详见 checks.txt。3 份 meta 与 HEAD 逐字节相同；git diff --check 通过。

复现：`python3 Tools/validation/PotHighlight/run.py`。脚本同步当前工作区到独立临时项目，不占用主编辑器，不访问服务或发布。日志存于临时项目 Temp/pot-highlight-unity.log；本次完整运行日志归档到 work/pot-highlight/unity.log。

第一次基线测试把遮挡采样点放在甜甜圈中空处，混入了原本允许的外围光晕；已改成汉堡实体内部。一次临时编辑器重启在资源回收阶段阻塞，终止的仅为本轮临时进程；限制 job worker 为 2 后两次新实现渲染正常完成。

范围限制：未上传微信、未做真机验收。此次独立重写只覆盖上述三份 Shader，不把 ItemHighlight、Mask、ScreenHalo 等未改模块宣称为本轮原创；也不作整个项目版权风险已清零的结论。历史逆向/来源证据保留，原审查报告中这三份 Shader 的“现存机械移植”描述已由本次替换更新。
