# 锅下薄物品回归

复用隔离 Unity 副本（不能是正在编辑的主项目）：

```sh
python3 Tools/validation/PotPanFloor/run.py --stage work/pot-flat-item/project
```

脚本同步当前源码与资源，在隔离项目运行 Editor 物理测试，再构建并执行真正的 macOS Player。结果写入隔离项目上级的 `results/`；测试脚本保留在 Tools，不进入正常发布包。

- 关闭新锅面绑定，复现 Kitchen_pizza 被旧 Y=0 底板支撑、完全被可见锅面遮住。
- Kitchen_pizza、Party_Pizza、华夫饼、松饼 × 4 种尺寸：真实锅面 fitting、锅下恢复、正常位置不动、12 秒模拟后仍可见。
- 最后一个 Kitchen_pizza 经生产可见性刷新恢复并可拾取；保持每日开局锁定，排除已取走物品，验证场景清理。
- Player 使用 7 个模型 × 3 种尺寸 × 12 种落点/旋转/高度，以及每个模型/尺寸的动态、运动学锅下恢复，共 294 组。

`AuditSnapshot` 是可选的只读诊断入口：将取得的关卡快照写入 `/tmp/pot-under-pan-level.json` 后，用 Unity `-executeMethod PotPanFloorValidation.AuditSnapshot` 运行。它检查可见网格顶点，不修改或上传关卡数据。
