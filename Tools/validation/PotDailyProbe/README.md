# 每日固定开局连消探物回归

运行 `python3 Tools/validation/PotDailyProbe/run.py`，或传入一个未打开的隔离 Unity 项目路径以复用缓存。不会操作主项目编辑器。

2026-11-13：`shared-v2` 每日快照路径绕过 `Spawn`，遗漏 `ComboProbeController.BeginStage`，导致面板未启用且还原物品没有登记为探物目标。测试和正式资格均受影响，本地生成关卡不受影响。现在在校验每日快照后、恢复物品前初始化，录像恢复路径保持原行为。

真实 Play Mode 28 项检查通过：测试权限开关两组、第一/二关显示、全部还原物品登记、手动消除连消、自动吸附不改变连消、重开清空状态、回首页隐藏面板。使用本地生成并捕获的有效快照，关闭账号和记录上传协程，不登录真实账号或发放奖励。此次复用了 `work/pot-probe-activation/project` 隔离项目。

结果：`docs/validation/pot-daily-probe/checks.txt`。本次仅修复和本地验证，未打包上传微信版本。
