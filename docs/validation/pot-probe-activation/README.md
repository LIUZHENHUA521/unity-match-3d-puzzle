# 探物关联激活动效验证

2026-11-13，在 Unity 2022.3.62f3c1 独立验证工程运行通过。

表现：探物卡片位置聚光 0.08s，沿定位弧线传光 0.28s，目标原地透视高亮及缩放回弹 0.20s，之后复用出锅 0.20s、飞入收集槽 0.30s 和落槽轻弹。长按定位沿用原有表现。

- `regression-checks.txt`：完整 ComboProbeValidation 玩法、物理接触、UI 像素与布局回归，包含新传光/激活阶段和暂停断言。
- `activation-checks.txt`：连续探物及普通点击引发重排时保留独立时钟；探物完成三消时等待所有飞行；移入暂存立即取消；池 token 变化时清理；重开不残留；真实物堆正常入槽。
- `pot-probe-before/light/awaken/lift/landed.png`：Unity 真实满锅场景分阶段截图，已人工检查传光可见性和目标透视激活。

运行：

```sh
python3 Tools/validation/PotProbeActivation/run.py
python3 Tools/validation/PotProbeActivation/run.py --focused
```

验证副本位于 `work/pot-probe-activation/project`。生产工程的新运行时代码仅新增 PotProbeActivation，并接入现有取物时间轴；不变更存档、协议和奖励规则。验证未覆盖微信真机性能；本次未上传微信体验版。
