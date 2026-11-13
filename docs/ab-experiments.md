# 共用 AB 系统接入

Unity 游戏“一锅消不完”的 AppID 为 `wx74f2d8722c02a6f4`。复用疯狂动物消的 `ab_tests`、`ab_test_records`、`hitABTest` 和管理后台 `/admin/#/ab-tests`，不另建分组算法或实验后台。

## 开实验

在共用后台新建实验，生效游戏明确选择“一锅消不完 · Unity 3D”。建议 key 使用 `goose_<feature>_ab`，A 为现状，B 为新方案。选择本游戏时默认注册起点为当前时间；若明确覆盖老玩家，可清空注册起点。流量 100 表示全部参与，组间由现有确定性算法分配。配置激活后下次拉取自动发现，无需在 Unity 维护实验清单。

历史空 appids 实验不会分给 Unity 用户。不同游戏建议使用独立实验 key。暂停/结束后，下次成功刷新会清除客户端分组；已保存命中记录保留。当前系统保留已有用户分组，放量策略主要影响新命中，不能用它假定已分组玩家全部切组。

后台“命中记录”支持填写 Unity 玩家 ID（邀请系统 ID）或真实微信 openid 补命中，服务端解析并验证用户；新记录带本游戏 appid。指定组修改和清除继续使用原后台流程。测试账号手工命中应从分析 cohort 排除。本次接入没有创建业务实验，也没有替真实玩家改组。

## Unity 使用

```csharp
if (Match3DABExperiments.GetABGroup("goose_example_ab") == "B") {
    // 新方案
} else {
    // A / 未命中 / 未登录 / 网络失败均使用现状
}
```

组件由 `Match3DFlow` 启动，观察微信登录完成后 POST `/invites/ab`，不阻塞游戏启动。初次拉取完成前走默认行为。业务如需开局一致性，应在开局读取一次并保存到该局状态，避免局内因刷新改变规则。可通过 `Ready` / `Changed` 等待结果，`Refresh()` 请求提前刷新。

缓存仅保存在当前会话内，最长一小时，切账号或登录失效后立刻不可读，过期后回默认并重试。失败间隔至少 30 秒，每次请求超时 15 秒。服务端、环境地址和账号之间不复用落盘缓存。编辑器未登录时保持默认行为。

## 身份与记录

游戏服务端先验证原有 Bearer 登录 token，从 `goose_invite_users` 取得真实微信身份。使用随机 256 位短时凭证，把 SHA-256 摘要和一分钟有效期写入该用户的 `abProof`，原文仅通过服务端 CloudBase SDK 传给 `hitABTest`。云函数核对摘要、过期时间、AppID 和用户文档，忽略客户端伪造的 openid、注册时间与实验选择。用户集合保持仅服务端可写。

复用原 AB 批量命中流程，真实注册时间来自 `goose_invite_users.createdAt`。记录 appid 为本游戏，openid 为真实 `sourceOpenid`，userId 为 Unity 玩家 ID。记录查询和确定性文档 ID 按 AppID 隔离；旧动物消文档 ID 保持兼容。手工分组、父子实验、注册范围、流量、暂停与时间窗口沿用现有规则。

ClickHouse 同步使用专门的 `GOOSE_INTERNAL_INGEST_KEY_ID` / `GOOSE_INTERNAL_INGEST_SECRET`，分析服务器凭据范围限定为本游戏 `ab_hits`，不会使用动物消的签名身份。CloudBase 命中记录是分组来源；ClickHouse 同步失败不阻断游戏，也不改变分组。

## 玩法效果分析

现有动物消 AB 页面专用的玩法、广告、留存指标不代表 Unity 游戏效果。Unity 对局已保存在 `goose_game_records`，无需为 AB 增加重复埋点。分析时通过 `ab_test_records.userId = goose_game_records.userId` 关联，限定本游戏 AppID、实验 ID、首次命中后的对局，并排除 `manualOverride=true`。按玩家聚合后再比较组间指标，避免高活跃玩家放大权重。广告次数可从已上报的 `toolUsage[].ad` 和 `reviveUsage` 计算；未上报对应版本字段的对局不能视为零广告。成熟留存须另有可靠的每日活跃口径，不能用无对局记录推断未登录。

## 验证

- `node --test crazyCombo/server/services/goose-server/test/*.test.mjs`
- `node --test crazyCombo/tasks/callFunctions/__tests__/*AB*.test.js`
- `python3 Tools/validation/ABExperiments/run.py`：隔离 Unity PlayMode + 本地 HTTP fixture，覆盖登录、换账号、迟到响应、暂停清缓存、过期、网络失败和 401。
- 完整微信构建验证组件编译及启动接线；后台从隔离发布快照构建，线上检查服务健康与鉴权。
