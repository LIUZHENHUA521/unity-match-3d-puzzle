# 普通第一关选品与碰撞空间修正（2026-11-04）

## 行为

首关复用第二关的实际 prefab 和类型 ID。限定 AppleGreen、Egg、Broccoli、Cupcake；AppleGreen 固定3件，其余三种随机选2种，各6件。首关普通预算12件加目标3件，目标也参与位置抽样和Y排序，没有强制放在最前排。tutorialNames 第一项为3件目标，其余项为普通候选；配置同步保存于 SourceLevelConfig.json 和 SourceLevelCatalog.asset。

每日关卡与本地首关都走 GenerateFirstStage，每日模式继续使用自己的确定性随机流；第二关的 daily-v1 生成器不变。保留原包目标先消费3次层抽样、再选择普通物品的顺序。

出生高度仍为1.5–6，沿原包高度加权、对应生成框X/Z随机、全体Y升序、创建时随机旋转的公共路径。首批1件，此后按原版每3件及.03秒上限创建，零缩放长大后释放物理；保留已有每帧耗时预算和生长写入上限，因此不声称与原版逐帧一致。

## 新发现的碰撞问题

此前第一关也使用了第二关狭窄底部碰撞空间。实际运行中15件可被挤至Y约9.7，不是单纯选品问题。

重新核对原包 LevelManager.InitLevelCol / j38503：普通单人模式按关卡1/2启用 LevelBGItem.level1Obj / level2Obj，并关闭 customCol。乡村原资源 Lv1Col 底部半径2.5，第二关半径1；第一关为20高、12度壁面，第二关为分段漏斗。

从用户原包配套 wechat-countryside.bundle 导出 CountrysideStoveFirstStage.json：保留共享碰撞和 Lv1Col，排除 level2Obj/customCol，生成框仍沿原版 createBoxPanel，不自行加宽随机出生范围。切换第二关时保留场景和边界根对象，停用并替换碰撞子树。

## 证据与验证

本轮证据和实际运行报告位于 work/first-level-placement-audit/；新增 chain-j38503.wat、first-stage-environment、导出脚本及首关截图。新资源是原包静态配置，物理落点是本地 Unity 模拟，不是原版设备回放。

FirstStagePlacementValidation.Run：1000种子/5003选品断言；本地及每日入口；实际首批1件、零缩放/运动学初态、逐渐长大、15件全部可见、出生高度及Y排序、每日重放、清空5组三消进入第二关。补充验证切关保留边界根、移除Lv1Col、第二关完成300件创建。结果以 work/first-level-placement-audit/validation.txt 为准。

未修改后端、未发布体验版。早期 first-stage-alignment.md 中“两关共享同一碰撞空间”的描述不再适用。
