> 物品 Toon 与环境光已继续对齐（2026-10-16）：见 [item-toon-ambient-alignment.md](item-toon-ambient-alignment.md)。下文“物品仍为 Lambert、环境光暂缓”的说明已被本轮替代；保留版本、图形后端和运行时 GI 的验证边界。未发布。

> 2026-10-16 门闩复核纠正：本文旧行为/旧通过记录中的“后续显示默认 .75 秒长大”结论失效。普通 WX 对象的 `_changeObjScale` 为非序列化 private bool，未发现开启路径，默认不执行重显长大；开局创建增长仍保留。最新证据与实现见 [reveal-growth-gate-alignment.md](reveal-growth-gate-alignment.md)。

# 镜头动态与阴影核对（2026-10-16）

范围：微信 WX 2.1.12 保存的代码/场景，移植到本项目单人第二关；未发布。静态调用链与本机 Unity 验证分开记录。

## 镜头：修正上一轮的“待补运动”判断

本次找到的默认路径是 **flatTopView 固定俯视**。不能因为类中存在镜头动画函数，就把它接到三消或堆高下降事件。

证据位于 `Evidence/camera-motion`：

1. 元数据 `LevelControlType` 枚举值为 freeRotaAndMove=0、circleRota=1、flatTopView=2（v29 默认值压缩整数，见 control-mode-enum.json）。
2. `LevelManager..ctor` j83392 将实例 `_levelType`（offset176）设为 2。
3. `.cctor` j83393 同样将静态模式设为 2；`Awake` j83431 再从实例 `_levelType` 拷贝至静态模式（static+8）。不能只看静态构造而漏掉 Awake 的覆盖。
4. `CameraControl.Start` j92873 在 mode=2 分支设置 `_canMoveUp=false`、`_canRota=false`；同时对托盘 Content 的位置做屏幕比例适配。
5. `LevelManager.Update` j83394 → `LevelTouch` j38498 走点选/抬手流程。`OtherControlType` j83404 入口也排除 mode=2。
6. 对 main/sub WASM 函数体做直接调用和常量表调用检索：找到 OtherControlType → AddCameraOffset；未找到 SetHeightLimit/GetCameraHeight/ChangeHeightAndRotaAnim 的直接/常量表调用。未把这一有边界的检索扩张为“任何反射或动态路径都绝不可能调用”。

保留函数本身做什么：

- `AddCameraOffset` j43052 在升降开关开启时，用 `-deltaY × _yMoveRate × 0.01` 更新高度并限制在 heightLimit；旋转开关开启时按 `deltaX × _xRotaRate × 0.1` 改变绕 Y 轴角度。保存的 rates 为 0.5 / 1。
- `ChangeHeightAndRotaAnim` j92876 接收 animTime、targetHeight、rotaAnim，创建位移与旋转 tween。没有确认到默认玩法触发点、固定时长或“消除到某高度自动调用”的证据。

**实现决定：默认相机保持静止，只做已确认的屏幕比例适配；不新增自动下沉、绕锅旋转或点击摇镜头。** `SourceControlMode=2` 标记原版默认值，并补上降低整堆物品高度后镜头仍固定的回归检查。上一轮“镜头动态尚未对齐”的表述应由上述结论替代。

## 阴影：已接入的来源配置

- WX 保存的当前质量档为 Medium：HardOnly、Medium 分辨率、StableFit、阴影距离 40、nearPlaneOffset 3、pixelLightCount 1。原版序列化值为单 cascade（1）；设置 `shadowCascades=0` 表示不分割，本机 Unity 2022.3 将其归一化回读为 1，实测其他质量值均符合来源。没有开启两级或四级阴影。
- 默认 SceneChange 方向光：强度 0.38、白色、culling mask 471、硬阴影，strength 1、bias 0.05、normalBias 0.4、nearPlane 0.2，保持保存的方向。
- MainScene 的另一个同名方向光挂在默认未激活的 LightAngle 下，不能因为资源里存在它就叠加开启。MainSceneLight 的 mask=512 只照另一层，也不冒充关卡光源。
- 三个主题的 39 个 Renderer 按源记录逐个恢复 layer、castShadows、receiveShadows；没有全场景一律开投影/接收。
- 当前导入的 71 个物品（iOS 原 prefab）共有 90 个 Renderer，另核实并恢复了其投射/接收开关。其中 7 个特殊节点包括不投不收的瓶壳/玻璃、仅接收的包装内部，以及 ShadowsOnly 的独立投影体。其余 83 个为投射并接收。来源版本与 WX 场景证据分开记录。
- 5 个 Toon 场景 shader 补上 ForwardBase 实际主光与阴影采样，阴影参与原版分段明暗系数，继续作用于原版高光/MatCap 的 shadow mask；并补 ShadowCaster。不是在最终画面上简单叠一个黑色透明层。
- BackShadow 恢复标准材质光照与 Cull Front；这一反面材质不是屏幕空间阴影特效。
- 进入单人第二关时使用这些设置，退出恢复原灯光启用状态、sun 和质量设置，避免影响首页、第一关和 PVP。

详细阴影证据与限制见 [source-shadow-alignment.md](source-shadow-alignment.md)。

实现见 `ReplicaSourceLighting.cs`、`SourceSceneImporter.cs` 与 `Shaders/Source`。来源记录包含 `lighting-settings-wx.json`、质量档和灯光祖先层级、选中的原版 GLES shader 程序；均随项目保存于 Evidence。

## 仍然不能宣称“完整视觉一致”的部分

物品还在使用本项目的 Lambert 简化着色器，而原版物品使用 Toon 材质。原资源的环境光颜色为 HDR 1.4148108；现在直接全局套用会使简化物品材质过曝。因此本轮保留本项目环境光，并明确保留该差异；BackShadow 的环境光部分也受此影响。额外光源分支、运行时灯光旋转尚没有完整触发路径证据，未自行添加。

静态保存配置不等于所有设备运行时质量档，也不等于真机逐像素比较。本轮验证的是本机 Unity 的行为与渲染链路；不能据此承诺手机性能或每个阴影边缘完全一致。

## 验证结果

完整结果见 [camera-shadow-validation.txt](camera-shadow-validation.txt)，本机 Unity 2022.3 图形 batch 退出码为 0：

- 三个主题各降低全部 300 件物品 8 个世界单位，执行 12 次显隐检查；高度阈值确实下降，而相机位置、旋转、正交大小保持不变。
- 39 个环境 Renderer、90 个物品 Renderer 的原版投射/接收标记校验通过，6 个 shader 编译无错误。
- 独立遮挡块探针排除遮挡块自身屏幕范围后，投影开关改变 247 个地面像素；关闭地板接收阴影后移除至少 95% 的这些投影像素。图像见 visual-previews/shadow-probe-on.png、shadow-probe-off.png、shadow-probe-no-receive.png。
- 三主题外观与碰撞共用祖先坐标、9:16 / 9:19.5 / 3:4 镜头适配，以及退出还原检查通过。
- 生成坐标、物理参数、300 件保留、1 秒显隐、0.75 秒 OutQuad 长大、点选反馈和 300ms 入槽回归检查通过。

[乡村预览](visual-previews/theme-0-pile.png)、[小卖部预览](visual-previews/theme-1-pile.png)、[烧烤预览](visual-previews/theme-2-pile.png) 为本机相机直接渲染的世界画面，未包含 UI，不是真机运行截图。未构建或发布体验版。
