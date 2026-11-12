# 分享卡片配置

用户已确认：使用以下三条文案，每次分享等概率随机选一条；所有文案共用一张已提供的分享图。

1. 这一锅不拼手速，拼的是眼力和取舍，敢来试试吗？
2. 名字叫一锅消不完，你能替我争口气吗？😂
3. 看着挺简单，一消就上头！这一锅你能消完吗？

imageUrl: https://mmocgame.qpic.cn/wechatgame/OwVPicIFUBQ9L8hmSjicNSfv0sW7zyTmEfPZ25bUTUMJMAksojPuD9ViaOhoWZRawC4/0

imageUrlId: Mg3EGffMTCuIXpg24LrIIw==

## 实现

- 客户端新增共享 WebGL 分享卡片配置，邀请、奖励和结算分享使用统一的选取函数。
- 每次调用时重新随机，允许连续抽到相同文案。
- 保留邀请 inviter、奖励 source、每日关卡 dailyDate/dailyTier 参数和原有失败回调、奖励返回判定。
- 结算标题不再加日期、档位、胜负前缀；日期和档位保留在 query 中。
- 不涉及后台配置、服务端和发布操作。

## 验证

用 Node VM 加载真实 jslib，控制随机数覆盖三条文案以及连续分享重新选取，验证图地址/ID、邀请和每日关卡参数。运行原有邀请和分享奖励回归脚本，检查异常、取消、超时和回调清理仍然有效。编辑器每日关卡校验改为检查分享 query。
