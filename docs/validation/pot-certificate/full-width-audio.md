# 奖状满宽与音效（2026-11-16）

通关与历史奖状共用按可用宽度 100% 缩放的布局，保持图片比例；移除按钮上下的小字，返回操作保留为右上角关闭图标。领取后的等待或错误通过现有提示反馈。

通关成功沿用游戏 Result 音效入口，绑定疯狂动物消实际 LevelModal.victorySound 使用的 Win.mp3（原 UUID d94c8d24-86c7-4274-9c59-d78275aed8ff），复制为 pot-victory.mp3，未修改音频。历史查看不重复播放通关成功音效。

盖章使用 SFXMint Rubber Stamp Checkmark 10：
- 来源：https://sfxmint.com/sounds/checkmark-10
- 下载：https://sfxmint.com/dl/checkmark-10.mp3
- 页面标注 CC0 1.0，可用于商业游戏，无需署名；2026-11-16 下载，保存为 pot-stamp.mp3，未修改音频。
- 印章落下至第 1 秒时播放一次，跳过动画时同步播放且不重复；遵循游戏音效开关，暂停不影响动画时序。

验证：Unity 奖状测试 87 项断言通过，包含三种屏幕尺寸的满宽和按钮边界、音频绑定及播放、暂停落章、重复跳过防重、奖励回执和账号隔离。截图在 work/pot-certificate-validation。本次仅修改客户端，未部署或上传。

我的荣耀回归测试通过，包含打开对应奖状、返回列表、回放入口与账号切换清理。
