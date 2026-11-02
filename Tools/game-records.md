# 3D 游戏闯关记录

## 已实现

- 单人每关一条记录，第一关自动切第二关前完成结算；同一次挑战和重试用 runId 关联。
- 通关、槽满/超时失败、回首页和重开分别记录；超 24 小时未结算在后台展示“结果未知”。
- 仅完成三消动画才增加 clearedCount；暂停、切后台不累计 activeDurationMs。
- 免费/库存道具成功使用次数分别记录；第一关不限时以 timeLimitSeconds=0 表示。
- 客户端按已登录账号持久化请求队列，失败退避重传，不阻塞游戏；401 恢复登录，400/409 保留诊断记录，队列上限 500 条（满时告知无法保存新记录）。登录前的事件仅本次进程暂存，首次成功登录后绑定；不跨重启猜测账号。
- 服务端用 token 推导账号，事务去重；finish 可先于 start，终局不可被迟到开局覆盖。属于客户端上报数据，不作为服务端验证的竞技成绩。

## 代码位置

- Unity：ReplicaGameRecords.cs；ReplicaFlow 的准备、可玩、结算、清理入口；InputController 三消完成回调；ReplicaInvitations 的登录恢复。
- Node：crazyCombo/server/services/goose-server/src/records.mjs；共享现有 Invitations 实例与鉴权。
- 查询：crazyCombo/tasks/callFunctions/adminGooseRecords（管理员 token、账号及 /goose-records 角色权限）。
- 后台：crazyCombo/tasks/admin-platform/src/pages/GooseRecords.tsx；菜单“3D 游戏 / 闯关记录”。
- 集合：goose_game_records，ADMINONLY，六组自建索引（另有系统索引）。

## 2026-10-31 部署与验证

- Node 提交 d893ac1 已推送；服务 goose-3d-pvp-008 已 normal、100% 流量。原服务环境变量、0.25 核 / 0.5 GB、固定一实例保留。
- 管理查询及页面提交 df3477f、eeb4b6a 已推送。adminGooseRecords 为 Active，Nodejs18.15。
- 后台以已提交代码构建并通过 MCP 上传 /admin。超级管理员已新增 /goose-records，其他角色不自动扩大权限。
- Node 共 27 项测试通过（包含 PVP、邀请、邮件及 4 项记录测试）；查询 4 项测试通过；Unity 编辑器 15 项生命周期/补传验证通过。
- 线上组合筛选、相同时间游标分页、详情、关联 runId 和 unknown 筛选通过；四条明确标识的部署测试记录已清理。未授权查询被拒绝。
- Unity 编辑器编译无错误；记录组件 WebGL 条件分支使用实际 wx-runtime DLL 编译通过。MCP 通用脚本校验器同时载入 wx-runtime-editor 与 wx-runtime 会产生重复类型误报，已用隔离引用验证，不修改 SDK。

## 客户端发布边界

本轮未构建或上传新的微信小游戏包，也未提交审核。采集代码位于 Unity 当前工作区，现有线上客户端不会自动获得该能力；构建并上传新版后才会产生真实玩家记录。之前的历史通关不能回填。微信真机隐藏/恢复、网络切换仍需新版客户端验收。

Unity 工作区原先已有大量未提交资产和脚本改动（包括本功能依赖的邀请组件），此次保留，未将其混入后端/后台提交。记录客户端验证菜单：Goose Replica / Validation / Game Records，需先启动本地 test/fixture-records.mjs（8099）并进入 Play Mode。验证后恢复本地队列并退出 Play Mode，避免测试账号继续使用。
