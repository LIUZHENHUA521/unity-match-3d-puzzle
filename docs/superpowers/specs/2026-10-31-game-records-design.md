# 3D 游戏闯关记录设计

日期：2026-10-31。用户已认可单人逐关记录、现有 Node 后端、CloudBase 独立集合和现有管理后台查询的方向。

## 范围和落点

第一版覆盖单人第一关、第二关的开局、通关、失败、主动退出和重开；PVP、操作回放、统计大盘不在本次范围。第二关通过才表示主题通关。

- Unity：Assets/GooseReplica/Scripts 新增独立记录组件，接入 ReplicaFlow 生命周期。
- Node：/Users/liuzhenhua/Desktop/code/crazyCombo/server/services/goose-server，新增记录模块，部署到已有 goose-3d-pvp。复用 Invitations 实例的 token 身份解析，不能另建 token 内存实例。
- 数据库：沿用现有环境，新增 goose_game_records，权限 ADMINONLY；复用 goose_invite_users。现有用户 id 和 inviteCode 相同，文档只存 userId，后台称为“玩家 ID”。
- 管理查询：在现有 tasks/callFunctions 下新增 adminGooseRecords 只读云函数，复用管理员 token 签名、账号禁用校验及角色权限机制；不向浏览器暴露运营密钥。游戏写入仍全部由现有 Node 服务承担。
- 管理页面：tasks/admin-platform 新增“3D 游戏 / 闯关记录”，路径 /goose-records，同步注册菜单、路由和角色权限选项。

## 文档结构

每次进入一关一个 recordId（UUID）；_id 为 sha256(userId + ':' + recordId)。同一次主题挑战共用 runId，第一关到第二关及当前关重试沿用；重新从首页开始挑战生成新 runId。

字段：schemaVersion=1、recordId、runId、userId、mode=solo、themeId、stage（1/2）、levelDate（开始挑战的北京时间日期）、levelVersion、clientVersion、platform、startedAt、endedAt、receivedAt、createdAt、updatedAt、status、endReason、activeDurationMs、timeLimitSeconds、remainingSeconds、initialCount、clearedCount、toolUsage。

startedAt/endedAt 为客户端事件时间；createdAt/updatedAt/receivedAt 为服务端时间，后台详情明确区别。列表时间筛选按 startedAt，默认北京时间最近七天。时间戳使用 UTC 毫秒。客户端时间只做合理性校验，不视为可信服务端判定。

toolUsage 为三种道具的成功使用次数，分别保存 free 和 inventory；使用失败、退款不计成功消耗。activeDurationMs 使用单调时钟累计可玩且未暂停、未切后台的时间，不包含加载和结算界面。clearedCount 仅计完成三消的物品，不把槽位或暂存物品计入；后台进度由 clearedCount / initialCount 计算。

levelVersion 为显式维护的配置版本；不声称 TodaySeed 能重现全局随机生成的棋盘。平台只采集运行平台，不新增微信个人资料采集。

## 写入与补传

POST /records/start 与 POST /records/finish 使用现有微信 Bearer token。请求体白名单、长度、枚举、数字范围和整数约束均由 Node 校验。服务端强制 solo、身份取 token；initialCount > 0，0 <= clearedCount <= initialCount，成功必须 clearedCount == initialCount。所有数据均标注为客户端上报，不用于可信竞技裁定。

start 请求包含不可变开局快照；finish 包含相同开局快照及终局数据，允许在 start 尚未到达时事务性创建终局记录。同一记录重传返回原结果；迟到 start 不覆盖终局；冲突快照或不同终局返回 409，不允许客户端更改已结算结果。并发去重以数据库事务保证。

客户端在本地持久化开始事件和最终结果，按账号隔离队列；只有确认登录身份后才绑定事件。登录尚未完成可在当前进程暂存，但不能在重启后把无身份事件归给另一个账号。上传失败不阻塞游戏，指数退避并在重新登录/网络恢复后补传；401 先恢复登录，400/409 进入本地诊断避免无限重试。队列设置上限 500 条，优先清理已确认事件，达到上限不得静默覆盖未上传结果，显示记录同步异常并保留已有事件。

开始实际游戏时记录 start，HandleResult 结算时先截取道具和进度，再执行场景清理。主动回首页记录 quit/home；重开记录 quit/restart，随后创建新 recordId。时间耗尽记录 fail/timeout，槽位失败记录 fail/tray_full。暂停和切后台不结算；强退不伪造失败。

数据库 status 为 playing/win/fail/quit。查询时对 startedAt 超过 24 小时仍 playing 的记录展示 unknown，属于展示状态而非不可逆写回；仍允许后续 finish 补齐。查询 unknown 和 playing 时须把相同的 24 小时边界转成数据库条件，不能只在分页结果上过滤。

## 查询与权限

adminGooseRecords 提供 list/detail 两个 action。每次核验签名、有效期、admin_accounts 禁用状态，以及角色是否拥有 /goose-records（沿用现有超级管理员规则）。无权限不得读取集合；详情与关联 runId 查询同样校验权限。

列表可组合筛选玩家 ID、开始时间范围、themeId、stage、status、clientVersion，默认 win。使用稳定的 startedAt + _id 倒序游标分页，每页 20、最大 100，返回 hasMore/nextCursor；筛选改变重置游标，旧请求不能覆盖新结果。不做昂贵的全量精确计数。

详情展示所有业务字段、客户端上报标识、服务端接收时间、道具分类，并提供 userId + runId 关联列表（仍分页）。主题名称从客户端主题配置整理为显示映射，未知主题仍显示原始 ID。

基础索引：startedAt + _id；userId + startedAt + _id；themeId + stage + startedAt + _id；status + startedAt + _id；userId + runId + startedAt + _id；clientVersion + startedAt + _id。组合查询需通过目标环境 explain/实际查询验证索引支持，缺失则补匹配索引，不以加载全表到内存过滤代替。

## 验证与交付

- Node 单测：未登录/过期 token、越权身份字段、非法数值、重复/并发上报、finish 先到、迟到 start、冲突终局、数据库失败。
- 客户端验证：两关关联、重开/回首页、槽满/超时、暂停/切后台计时、消除与暂存计数、道具成功/退款、断网重启补传及账号隔离。
- 管理查询测试：未登录/禁用/无权限拒绝、详情权限、筛选组合、unknown 边界、相同时间游标及关联分页；后台生产构建检查。
- 保留工作区已有大量改动，仅提交本任务内容。部署前依项目部署技能完成对应 server/tasks 仓库提交推送。
- 云端操作按 cloudbase-mcp-ops，通过 CloudBase MCP 检查环境、创建集合/权限/索引、部署 Node 和查询函数，再部署 /admin。保留已有服务配置和密钥，不改变 PVP 行为。
- 当前会话未发现 CloudBase MCP 工具，云部署仍为待解决依赖，不能标为已完成。客户端需要构建新版才会采集真实玩家记录；本轮不自行提交微信审核或发布正式版。
- 已部署后做只读健康检查、未授权拒绝、受控测试账号记录写入/后台查询验证；测试记录必须可区分且不污染业务统计。
