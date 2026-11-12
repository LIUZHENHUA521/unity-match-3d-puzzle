# 连消探物素材来源

原六张 UI PNG 来自用户提供的 `Desktop/GPT图/新home/`。布局修订另复用该目录的 `image_Star.png` 作为稳定亮星；原 Cocos `pre_star.prefab` 用它表示常亮，`image_Star_lighe.png` 仅用于点亮过渡。圆形 Spine 的 JSON、atlas、PNG 来自 `crazyCombo/client/assets/resources/spine/comboFeedback/colorfulCircles_spine.*`。运行时资源在 `Assets/Match3D/Resources/Match3D/ComboProbe/`，原始 JSON 单独保存在 `Original/`，不进入游戏资源包。

## Spine 运行时

`Assets/Spine/` 保留官方 3.8 Unity 包的 runtime/editor 及原始 `.meta`，未包含 Examples，未修改运行时代码。源码文件保留官方版权与运行时许可条款，`Assets/StreamingAssets/Spine-Runtimes-License.txt` 随后续构建一并携带。

- 下载地址：https://esotericsoftware.com/files/runtimes/unity/spine-unity-3.8-2021-11-10.unitypackage
- 包 SHA-256：`ebec10123b018ba9a4c7f4023ac0602a98674e57465657e0b190acd29c923108`
- 原始 JSON SHA-256：`e11b5f82893f1b81fe4268761d50d0a15c57968f7bbdcf37498a83042a020232`

原 JSON 标记为 3.8.75，官方 3.8 reader 对该版本有明确拒绝分支；官方问题记录解释该导出器存在缺陷并建议重新导出（https://github.com/EsotericSoftware/spine-runtimes/issues/2428）。本次没有声称重新导出或通用兼容：仅针对这份无 IK/transform/path 约束的光圈，将导入副本的版本字段规范化为 `3.8.0`，其余数据逐项一致；五色动画均在 Unity Play Mode 加载、播放和截图验证。以后更换导出文件须重新验证，不能套用这个处理到其他骨骼。

校验：`python3 Tools/ComboProbe/normalize_ring.py --check`。需要重建相同导入副本时去掉 `--check`。原文件的哈希不符则脚本拒绝修改。

`ComboProbeAssetBuilder.Build` 导入透明 PNG，创建 `Assets.asset` 及专用 SkeletonGraphic 材质/atlas/skeleton。SkeletonData 缩放为 0.01，SkeletonGraphic 再按 Canvas 的 100 pixels/unit 恢复像素尺寸；使用直通 alpha 输入。不要把它改为静态光圈。

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
