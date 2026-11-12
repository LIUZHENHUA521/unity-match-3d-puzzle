# 连消探物素材来源

原六张 UI PNG 来自用户提供的 `Desktop/GPT图/新home/`。布局修订另复用该目录的 `image_Star.png` 作为稳定亮星；原 Cocos `pre_star.prefab` 用它表示常亮，`image_Star_lighe.png` 仅用于点亮过渡。圆形 Spine 的 JSON、atlas、PNG 来自 `crazyCombo/client/assets/resources/spine/comboFeedback/colorfulCircles_spine.*`。运行时资源在 `Assets/Match3D/Resources/Match3D/ComboProbe/`，原始 JSON 单独保存在 `Original/`，不进入游戏资源包。

## Spine 运行时

`Assets/Spine/` 保留官方 3.8 Unity 包的 runtime/editor 及原始 `.meta`，未包含 Examples，未修改运行时代码。源码文件保留官方版权与运行时许可条款，`Assets/StreamingAssets/Spine-Runtimes-License.txt` 随后续构建一并携带。

- 下载地址：https://esotericsoftware.com/files/runtimes/unity/spine-unity-3.8-2021-11-10.unitypackage
- 包 SHA-256：`ebec10123b018ba9a4c7f4023ac0602a98674e57465657e0b190acd29c923108`
- 原始 JSON SHA-256：`e11b5f82893f1b81fe4268761d50d0a15c57968f7bbdcf37498a83042a020232`

原 JSON 标记为 3.8.75，官方 3.8 reader 对该版本有明确拒绝分支；官方问题记录解释该导出器存在缺陷并建议重新导出（https://github.com/EsotericSoftware/spine-runtimes/issues/2428）。本次没有声称重新导出或通用兼容：仅针对这份无 IK/transform/path 约束的光圈，将导入副本的版本字段规范化为 `3.8.0`，其余数据逐项一致；五色动画均在 Unity Play Mode 加载、播放和截图验证。以后更换导出文件须重新验证，不能套用这个处理到其他骨骼。

校验：`python3 Tools/ComboProbe/normalize_ring.py --check`。需要重建相同导入副本时去掉 `--check`。原文件的哈希不符则脚本拒绝修改。

`ComboProbeAssetBuilder.Build` 导入透明 PNG，创建 `Assets.asset` 及专用 SkeletonGraphic 材质/atlas/skeleton。SkeletonData 缩放为 0.01，SkeletonGraphic 再按 Canvas 的 100 pixels/unit 恢复像素尺寸；光圈图集使用预乘 alpha（PMA）输入：关闭材质 `Straight Alpha Texture`，关闭该图集导入的 `Alpha Is Transparency` 和 `sRGB`；普通 UI PNG 仍使用原来的直通 alpha 导入。不要把它改为静态光圈。

## 验证入口

在项目根目录执行：

```sh
/Applications/Unity/Unity.app/Contents/MacOS/Unity -batchmode -projectPath "$PWD" -executeMethod ComboProbeValidation.Run -logFile "$PWD/work/combo-probe/validation.log"
```

检查结果和真实场景截图在 `work/combo-probe/return-to-bowl-results/`；结束标志为 `checks.txt` 最后一行 `COMPLETE`。测试遮挡夹具故意使用放大的挡板；`showcase-*` 是正常第二关实际物品和容器画面。截图中的连消数和奖励库存由验证器设置，用于验收不同状态。

隔离 WebGL 构建：`python3 Tools/daily-preview/build.py --stage Builds/DailyWebProject`。该命令只生成本地预览产物，不部署。

## 原版布局对照（2026-11-13）

参考 `crazyCombo/client/assets/scenes/game/main.scene` 的 `slot/newCombeNode`、`resources/prefabs/pre_star.prefab`，以及 `ComboSpineFxController.onLoad` 的层级修正。Unity 使用共同的 badge 父节点，统一按 `121/151` 缩放，保留当前圆盘直径。

- 圆盘：151×151，局部原点；Spine 同心，局部缩放 1.24。
- 灰色圆弧：原图 160×134，局部缩放 1.27，中心 `(0.073, 15.811)`。这是下方裁切的圆弧，图片中心与圆盘中心不同。
- 三星坐标：`(-88.646,-30.912)`、`(-1.736,88.811)`、`(87.397,-31.31)`；星星图保留 prefab 的 x=1.47 偏移，尺寸 45.8×45。
- 显示顺序：圆盘 → Spine → 灰色圆弧 → 文案/星星，光圈不会盖住星星。
- 特殊区触摸范围下缘从 310 降到 296，主槽图片范围上缘 292；高度 224→180，减少透明留白。保留锅原先的上移位置及原镜头大小，光圈装饰不参与射线点击。
- 后续微调：蓝色底板、提示、四槽模型和点击区域统一缩小至 90%；蓝色槽位主体（不含顶部提示页签）与红色圆盘在垂直方向居中对齐。整组向右平移 28 个设计像素，补偿缩窄后的宽度差，维持整体水平居中。圆盘及星环大小不变。
- 模型修订：使用 prefab 的纯视觉层级和 `Match3DTrayDisplaySettings.Rotation`，按相机空间的模型包围盒居中取景，不再截取锅中随机翻倒的姿态。底图四槽像素中心为 `(121+103*i,95)`，从 507×152 映射到 558×167 的面板坐标；模型与点击区域同步对齐。删除数量节点和数量文案更新，下层只做 4 像素竖直错位，保留原压栈与取物逻辑。
- 星数反馈：`sunshine.png` 以原版 154×128、1.27 倍尺寸覆盖灰轨道；径向几何裁切还原 Cocos 的 `fillCenter=(.5,.4)`、`fillStart=.62`、顺时针 3/4 圆。0/1/2/3 星对应填充 0/.08/.38/.75，端点跟随左、上、右星位。消耗时 0.22 秒退回，消除后 0.34 秒补满。
- 亮星常态使用 `image_Star.png`；点亮以 `image_Star_lighe.png` 短暂闪光、1.3 倍弹跳，熄灭收缩并淡出至底层灰星。动画从当前值重新衔接，不排队修改逻辑；暂停冻结，重开立即清理。

## 局内提示

提示与“连消数”均加粗、居中。提示按原图顶部页签中心定位，宽 260 设计像素，字号 19，必要时缩至 15，保持单行。优先显示最后一星提醒；有探物时显示首次使用说明或常规操作提示；无探物时显示首次消除说明或实际剩余取物次数。

- 首次消除前：`先消一组，点亮三星`。
- 已激活、无探物：`3次取物内再消除，获得探物`，数字跟随剩余星数。
- 尚未使用过探物且有模型：`点击模型，取出被挡物品`，首次奖励所在槽轻亮两次。
- 成功使用探物后：`点击取物 · 长按定位`。长按、拖出取消、无效点击不视为成功使用。
- 最后一星：`最后1次取物，消除可续连`。

成功使用记录仅在本次运行内保留，换关/重开不会重复首次操作教学；不写入永久存档。槽位提示动画随暂停冻结，重开清理。

## 光圈黑边修正（2026-11-13）

原先误将光圈图集按直通 alpha 输入，shader 再次乘以 alpha，导致半透明柔光边缘变暗。原 PNG 的 256,306 个半透明像素全部满足 RGB 各通道不超过 alpha，白色光环的 RGB 随 alpha 衰减，符合 PMA 数据。修复只改材质与导入设置，不修改 PNG、骨骼动画或全局色彩空间（当前项目为 Gamma）。配置由 `ComboProbeAssetBuilder.Build` 保持，避免重建资源时回退。官方设置说明：[Spine Unity Assets](https://esotericsoftware.com/spine-unity-assets)。

验证器在同一动画帧用临时材质重现旧的二次 alpha，输出 `ring-double-alpha-before.png`；`showcase-combo-1.png` 为相同姿态的修正版，其余四种颜色也分别截图。临时材质在 finally 中恢复，不写回资源。

## 进度、宽度与模型光照修正（2026-11-13）

- `ComboProbeArcGraphic` 直接生成剩余扇区的顶点和原图 UV，零进度不生成三角形，不再使用 `Mask`/模板缓冲。保留原图圆心、起始角和 0/.08/.38/.75 星位阈值，以及亮灭动画。针对不提供 stencil 的宿主，验证 0/1/2/3/0 状态的实际输出像素；depth=0 与 depth=24 结果一致。
- 特殊区内容整体放大至原来的 1.15 倍，居中微调 4 个设计像素；不再随短屏压缩宽度。布局矩形为 720×210、下缘仍是 296，四种画幅下装饰矩形宽度均约为视口的 97.3%，四槽点击区域与模型同步缩放。锅的相机比例与构图算法不变。
- 模型缩略图移除独立的 0.8 强度、不同方向的灯光，捕获期间临时将预览层加入实际 `RenderSettings.sun` 的照射范围，finally 恢复原 mask；沿用场景环境光和原材质。换关/重开清空缩略图缓存，避免沿用旧主题光照。

结果见 `work/combo-probe/region-fixes-results/`：`chance-empty.png`（零星无黄条）、`chance-one-star.png`、`chance-two-stars.png`、`chance-refilled.png` 和 `showcase-tray-comparison.png`（特殊区与主槽同款模型）。

## 探物取物与发奖反馈（2026-11-13）

- 探物取物在接收这一帧就将目标设为 kinematic，并关闭刚体检测与全部子 Collider，随后才执行入槽动画。普通取物保持原时序。原碰撞开关保存在回锅快照中，移出/复活时正常恢复；不冻结整锅或修改邻近物品。
- 奖励即时确定真实目标与轮转槽位；`ComboProbeRewardFeedback` 使用用户的 `image_MeteorLight.png` 从圆盘顶部星位沿弧线飞行 0.44 秒。抵达时显示模型、短暂闪光，并在 0.28 秒内从 0.25 倍放大至 1.15 倍后回到 1 倍。下层模型保留原姿态。
- 同槽奖励排队飞入，其他槽可并行。飞行中不展示/消费新模型，落地只改变显示，不再次发奖；目标被普通取物/磁吸拿走时取消对应动画。暂停冻结，重开/换关清理。
- 5 连消起，亮星和飞行星头随彩色光圈使用 `image_Star_color.png`。熄灭显示灰星，断连/重开恢复普通星皮肤。
- 参考疯狂动物消 `LayoutRoot._runNextStarAnim` 的命中后生卡顺序与 `StarLight` 的定向飞行，结合本项目实例绑定和四列叠放；参考项目未改动。
