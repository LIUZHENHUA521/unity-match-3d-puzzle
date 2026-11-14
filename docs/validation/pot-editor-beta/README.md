# 编辑器读取内测共享关卡

每次在 Unity 编辑器首页点击本科局/清北局，调用 POST /pot-editor/beta-level，读取 goose_daily_levels/beta-current。使用与真实内测玩家相同的 playerDailyLevel 配置归一化逻辑；每次预览生成随机种子，不绑定某个真实玩家的当日种子。

请求使用独立的只读预览凭据，存放在已忽略的 UserSettings/PotEditorPreview.json（key 字段），不进入 Assets 或发布包。服务器 POT_EDITOR_PREVIEW_KEY 仅授权这个入口；不会创建玩家、关卡进度、红包资格或奖励。接口失败不会偷偷回落本地关卡。后台主题覆盖本地工具主题选择。

原始 Begin 本地工具和候选关卡工作坊保留。当前局的重试和第二关保持已读取配置，回首页新开局重新读取。

验证：服务端全套 122 项通过；独立 Unity 编译与 Play Mode 13 项检查通过，见 checks.txt。

部署：后端提交 8bdcdd4 已推送。以线上 037 下载快照为基础，仅叠加已提交的预览入口，保留并行发布的广告相关逻辑和环境变量。通过 CloudBase MCP 发布 038，状态 normal，100% 流量。

公网验证：两档均读取 beta-461e6f0d7584e008e8d5fb45 / tasty-kitchen / 294 个 / 600 秒，与数据库 beta-current 一致；无凭据返回 401；服务存活和数据库就绪检查均为 200。详情见 live.json。未改动内测配置，也未创建真实玩家或红包资格。
