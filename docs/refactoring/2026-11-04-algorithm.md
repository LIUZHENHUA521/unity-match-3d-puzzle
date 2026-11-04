# 关卡生成职责拆分

将 `ChallengePlan.GenerateInternal` 拆为配置校验、类型选择、配额分配、逐类型放置、可用层查找、输出与旧关卡打乱几个职责清楚的私有函数。公开入口、算法来源和规则不变；未调整游戏难度、资源、物理或动画。

保留逐层校验时机、类型降序遍历、候选插入顺序、单精度/双精度表达式、舍入规则、随机调用顺序和数量。`FrequencyPool` 的概率更新实现未改动。来源说明仍在代码和已有来源文档中。

重构前使用原实现生成 `Tools/validation/challenge-plan-baseline.txt`，重构后用同一套输入对比。3308 组指纹覆盖 204 个种子、4 种物品池大小、等权/非等权、共享随机流/旧生成路径、配额边界和非法配置。SHA-256 输入包括每次随机调用的上下界与结果，以及生成物品每个字段的二进制值。对比通过，测试期间生产 RNG 状态保持不变。

复跑命令：

```sh
/Applications/Unity/Unity.app/Contents/MacOS/Unity -batchmode -nographics -projectPath "$PWD" -executeMethod ChallengePlanEquivalenceValidation.Run -logFile /tmp/match3d-algorithm-equivalence.log
```

基线捕获模式会拒绝覆盖现有基线，正常验证不会更新预期结果。结果见 `2026-11-04-algorithm-validation.txt`。这些用例证明被测输入一致，不代表跨 Unity 版本或所有可能输入的形式化证明。

发布目标：微信开发版本 0.1.26，包含本轮重构和此前命名迁移。上传后需在公众平台设为体验版；浏览器管理员策略阻止当前助手访问该站点，不能自动执行此步骤。
