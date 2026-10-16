# 普通选中完整发光描边路径（2026-10-16）

本轮恢复已提取版本普通 `PickHighlight` 实际开启的完整路径：选中目标的 stencil 遮罩、黄色三角波 Overlay、四方向外发光、四方向黑描边，以及取消时清空效果。不是声称复刻 HighlightPlus 插件中所有未启用的模式，也未做原版真机逐像素对照。

## 已核对的原始资源

WX `data.unity3d` 的 HighlightProfile 722/723/724 分别是 LightingHighlight/PickHighlight/TargetHighlight，组件 747/754/766/774/802 分别引用它们。利用原生 IL2CPP 字段序列化 schema 解码，所有组件 556 字节和 Profile 496/500 字节全部消费；iOS `sharedassets0.assets` + `level0` 的三个同名 Profile 全字段相同。`ios-profile-crosscheck.json` 和 `723.json` 保留原始配置、偏移和对象来源。

普通 PickHighlight 有效配置：

| 字段 | 保存值 | 实际作用 |
|---|---:|---|
| outline / outlineColor / outlineWidth | 1 / 黑 / 0.2 | 几何描边 |
| outlineQuality / glowQuality | 3 / 3 | **Medium**，不是 Highest；Highest 的枚举值是 2 |
| glow / glowWidth | 1.86 / 0.25 | 外发光几何扩张 |
| glowPasses | offset=1,alpha=1,RGB=(1,.964705944,.133333340) | Medium 实际使用该颜色，glowHQColor 不参与 |
| overlay / min / animationSpeed | .5 / .5 / 1 | 黄色覆盖层三角波 |
| overlayColor | (1,.921568632,.015686275,1) | 保存的覆盖层颜色 |
| glowAnimationSpeed | 1 | shader 使用 `_Time.w`，即 3×scaledTime |
| normalsOption / constantWidth | Smooth(0) / true | 同坐标顶点平滑法线、恒定屏幕宽度 |
| outlineVisibility / glowVisibility | Normal(0) / AlwaysOnTop(1) | 描边深度测试 LEqual；发光 Always |
| fadeInDuration / fadeOutDuration | 0 / 0 | 立即开启/取消，无项目额外 .12 秒渐入 |
| innerGlow / targetFX / seeThrough | 0 / false / 0 | 普通选中未启用 |

LightingHighlight 是蓝色能力高亮，TargetHighlight 为宽黄色目标提示；不能把它们的颜色/宽度套在普通手指选中上。Lighting 已按 AutoFind 调用链实现独立蓝色高亮，Target 的调用定位为新手教程提示，不接到普通道具。

## 函数和 shader 证据

- `LevelManager.ResetTouchObject j19867 -> HighlightEffect.SetHighlighted j2810(false)`：清选中即时关闭；`fadeOut=0` 无渐退。
- `HighlightEffect.UpdateMaterialPropertiesNow j9989`：Quality 3 的方向索引为 4..7；只有 Quality 2 才会创建 Highest blur 路径。
- `HighlightEffect.OnEnable j85382`：四个对角方向使用精确 float 常量 `0x3f350529 = .7071099877357483`。
- `HighlightEffect.DoOnRenderObject j20234`：Medium glow 总强度乘 .5；每方向 `_Glow=(.93,.0025,.75,.5)`；`_Glow2=(outlineWidth/100,animationSpeed,1)`；方向 Y 乘相机 aspect。描边宽度和方向均除以100。
- `HighlightEffect.GetZTestValue j85386`：Normal=4(LEqual)、AlwaysOnTop=8(Always)、OnlyWhenOccluded=5(Greater)。
- 包内 `HighlightPlus/Geometry/{Overlay,Outline,Glow,Mask}` 的 GLES 程序和渲染状态：移植实际无 alpha clip 的 shader 变体；无另造 rim lighting。
- Overlay 的强度是 `abs(.5-frac((time%1000)*speed))*2*(1-min)+min`，乘 overlay=.5，范围 .25～.5；其 RGB 是原 shader 的纹理/底色与黄色相乘，使用正常 alpha blend。不是原项目的余弦暖白自发光。
- Glow 宽度脉冲 `1+.25*sin(3*time*speed)`，范围 .75～1.25，基于 Unity scaled `_Time`；不是手指按下时重置相位。
- 源渲染状态 `Blend SrcAlpha OneMinusSrcAlpha`，Stencil bit2 对物体实体部分做遮罩，防止外发光/黑描边把整个模型刷亮/刷黑；overlay 自身使用 bit4。

## 项目落地

- `ReplicaSourceHighlight`：仅在当前被选中物体上生成效果几何，四个方向逐个绘制。Camera 的 `AfterForwardAlpha` CommandBuffer 重建对应原 OnRenderObject 的覆盖时机；没有为300件物品各创建模糊 render texture，也没有常驻额外300个Renderer。
- 使用复制的几何做平滑法线，原 mesh、原 Toon material、原材质属性不被替换；SkinnedMeshRenderer 按 j20234 的 RendererType=1 分支直接 CommandBuffer.DrawRenderer，使用 Unity 当前骨骼矩阵；不再用 BakeMesh+renderer.localToWorldMatrix。原版只对静态复制 mesh 平滑法线，skin 不额外平滑。
- Resources 下四份独立 Shader 保证实际构建引用，不依赖 `Shader.Find` 偶然保留；Outline/Glow/Overlay 是保存 GLES 程序的机械 HLSL 移植。针对 Metal/D3D 做 clip-Z 与贴面深度偏移方向适配。
- `ReplicaSelectionFeedback` 调用独立 source renderer，旧 `_SelectionGlow/_SelectionPulse` 始终置0，避免 Toon 中旧近似选中效果与新效果叠加；去掉旧 1.4Hz 自定义脉冲与 .12 秒软起步。
- 取消、选中目标切换、对象失活、Home/Restart 调用 Cancel、相机禁用/销毁都会清空绘制命令；所有临时 mesh/material 随清理销毁；每台相机只有一个效果 buffer。
- 本次不变更已存在的选中物理上提、邻近唤醒策略；它们不是 HighlightPlus 渲染实现的一部分。

## 验证和边界

`SourceHighlightValidation.Run()` 覆盖原波形采样、完整12次单submesh绘制、四shader支持、真实Camera渲染截图、切换/隐藏/取消、原资产不变和反复选中后buffer不增殖。另用真实 `Goose_Pancake` 骨骼模型验证选中像素变化和取消后无残留。输出 `Temp/goose-source-highlight-validation.txt`、`visual-captures/source-selection-highlight.png` 与 `source-goose-highlight-{off,on,cleared}.png`。旧 `SelectionFeedbackValidation` 已更新为检查新source renderer及旧叠亮为0。最终运行结果由主任务统一记录。

- 这是使用当前 Unity/图形后端的实现移植，不能等同于原版本/真机驱动逐像素一致。
- 源插件其他质量档、Highest模糊链、see-through、TargetFX、命中圆形传播等普通Pick未开启的分支不引入。
- TargetHighlight 由 NewGuide/NewGuideClickObj（新手引导）控制；当前项目没有这套引导流程，因此没有把它套到 Store/AutoFind 上。

## AutoFind 蓝色高亮及皮肤专项补齐

- `LevelManager.CreateAutoFindLighting j19876` 按 itemIndex % `_toolHighlights.Length` 取 effect（场景数组两项），对自动找到的物品 SetTarget、SetHighlighted(true)。重启 FingHighlight 协程。
- `FingHighlight.MoveNext j65259` 等待 `_pickMoveTime` = .3000000119 秒（WaitForSecondsRealtime；本轮重查类型索引后纠正前轮误读），然后遍历 `_toolHighlights` 全部 SetHighlighted(false)。
- `ReplicaSourceHighlight.ShowAutoFind(items,duration=.3)` 对应两个独立蓝色组；普通手指 Select/Clear 不干扰这个 .3 秒时窗。ClearTools/ClearAll、相机禁用、目标失活或销毁清理剩余绘制。
- Lighting 的 overlay=(.0156862736,.869899094,1)*.5；glowPass color=(.133333340,.941176534,1)，offset=2,alpha=.5。Medium 最终 `_Glow=(.465,.005,.75,.5)`；黑描边仍宽 .2。普通 Pick 数值保持不变。
- `CreateThrowLight j38515` 调用 ToolReturnLight.SetPos(item,_storeAnimTime)，没有直接启动 HighlightEffect；不能把蓝 Lighting 当作 Store 的完整视觉。
- `_guideHighlight` offset824 的调用仅定位到 `NewGuide j9880`、`NewGuideClickObj j38494`：教程指向第一个/下一个同类，结束时设 null/false。对应宽黄色 TargetProfile，不属于本轮基础三个道具。

验证扩展：逐个隔离真实 Goose_Pancake 的 SkinnedMeshRenderer，关闭其他 renderer 后分别比对前后像素（每个必须 >100 像素变化），避免仅煎饼变亮让整图断言误通过。另检查真实鹅的 Lighting 蓝色像素、双槽并存、普通取消不清道具、unscaled deadline 到达后统一清理、ClearAll 无残留。截图 `source-goose-skin-{n}-{off,on}.png` 与 `source-autofind-highlight-{off,on}.png`。这些新增运行检查由主任务统一执行；本子任务未启动 Unity 或发布。

### 统一 tools4 运行结果

主任务执行后的 `/tmp/goose-feedback-tools4.log` 已记录新增测试 PASS：真实鹅整图选中变化 7554 像素，清除残留 0；隔离皮肤 `e` 自身变化 19002 像素；AutoFind 蓝色变化 6301 像素；两槽并存与超时清理通过。人工查看 `source-goose-highlight-on.png`、`source-autofind-highlight-on.png` 确认鹅身体和静态煎饼都收到对应黄/蓝效果。子任务仅读取结果和截图，没有另起 Unity 或发布。

## 堆叠场景遮罩修正（tools6 目视复核）

真实两目标 AutoFind 截图出现大面积不透明青色，不能仅以孤立模型“像素有变化”认定正确。本次找到并修复实际移植错误，没有减少原亮度：

- `j20234` 行 554–711：读取 glowVisibility(offset204)=1、glow(offset156)>0，并在非 Highest 分支把 glowVisibility 传到 local22，令 local15=true。
- 同函数行 1775–1784：`RenderMask(this,rendererIndex,mesh,local15,...)`。
- `j39438` 行 49–76：根据该布尔参数选择 `_ZTest=8 Always`，否则为 4 LEqual。因此实际 Pick/Lighting 都需要 **Always silhouette mask**。此前固定 LEqual 是错误的；被其它物体挡住或深度浮点不一致时，mask 未覆盖实体部分，四方向 Always glow 在内部重复填色，结果接近不透明青色（每方向alpha=.465，四次叠加等效覆盖 `1-(1-.465)^4≈91.8%`）。
- 原 `HighlightPlus_Geometry_Mask/1.glsl` 还给 clipZ 加 -0.0001；此前Mask遗漏此贴面偏移，本次同步恢复并适配 reversed-Z。
- Overlay 强度 .25～.5、Lighting glow .465 与两倍offset、Outline宽 .2、绘制顺序都保持原证据数值，不以美观调暗。

新增回归：目标球体中心被另一个不高亮Cube遮住，在相机距离3/5/9分别渲染 AutoFind前后。中心15×15区域必须完全不被蓝色填充，外围仍必须存在真实高亮像素。保存 `source-autofind-occluded-{distance}-{off,on}.png`。此检查能覆盖之前孤立模型测试未覆盖的遮挡情形。主任务 tools8 已运行通过：三个距离遮挡中心均0像素污染；真实堆叠截图的青色实体填充消失，前景西瓜/蒜材质保持可见。仍可穿过前景看见目标外轮廓，这是原 glowVisibility=AlwaysOnTop 的行为；overlay 为LEqual，不覆盖挡住目标的前景物。

## AutoFind暂停计时纠正（选中物理复核）

本轮不是根据字段名字推断等待类型：主内存地址3440340保存encoded usage `0x2000a27f`，去usage标签后右移一位得到IL2CPP typeIndex20799；WX metadata的byvalTypeIndex20799明确为`UnityEngine.WaitForSecondsRealtime`，含waitTime与m_WaitUntilTime两字段；FingHighlight j65259确实写m_WaitUntilTime=-1。对照延迟关碰撞的3440336编码0x2000a277→typeIndex20795则为WaitForSeconds。详见 `Evidence/selection-physics/wait-types.json`。

因此 ShowAutoFind deadline改为Time.unscaledTime，Update通过同一独立时钟清理。shader脉冲仍保留原Unity scaled _Time，不修改发光波形。暂停回归在timeScale=0时检查.299 real秒仍保留两组、.301 real秒清除，以及实际Update对过期deadline的清理。EditMode同步检查通过私有TickToolLifetime传入独立时钟模拟时间边界，并未声称同步方法期间Unity PlayerLoop已推进。.3秒寿命的此前scaled描述是错误记录，已在正文纠正。
