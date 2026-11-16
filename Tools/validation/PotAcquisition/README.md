# Pot 广告与分享规则验证

运行 `python3 Tools/validation/PotAcquisition/run.py`，复制完整当前 Unity 源码到隔离目录，不修改日常编辑器项目。

- 真实计数代码：242 项断言，覆盖 n=1/3/100、共享计数、跨局、分享重置、复活强制广告、开关、远程配置、操作快照、重复回调、取消和账号隔离。
- 实际游戏界面：广告失败重试、第三次广告解锁分享、复活保留分享机会、分享失败重试和旧关卡回调。
- 结果：`work/pot-acquisition/project/work/pot-acquisition-checks.txt`、`work/pot-acquisition/project/docs/validation/reward-acquisition/result.txt`；同目录输出道具广告、复活广告、道具分享截图。

测试使用匿名本地状态和编辑器广告/分享回调，不登录真实玩家，不发放线上奖励。计数状态按唯一测试环境隔离。
