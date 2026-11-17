# 编辑器玩家独立配置交付

2026-11-17。

- 管理后台新增每日关卡第三个标签「编辑器玩家」，两档入口共用层数组，只保存 count/types。
- 使用既有 `/goose-daily-levels` 权限及 `adminGooseRecords`，独立文档 `goose_daily_levels/pot-editor-current`，不会变更每日发布数据。
- Unity 测试启动配置新增「后台编辑器配置」，本地选主题，第一关保持原状；第二关覆盖层数、物品数与类型数。类型数不自动裁剪。已有层保留高度和权重，新增层补高度并等权。
- 新开局读取配置，重试不重新请求；本局主题冻结。保留内测每日关卡与纯本地来源。凭据沿用忽略目录中的编辑器只读凭据。

## 验证

- `python3 Tools/validation/PotEditorBeta/run.py`：274 PASS，COMPLETE。覆盖 8 个主题的两档入口、第一关、第二关、重试、主题冻结、增层高度、权重匹配、凭据缺失、原有内测及纯本地模式。
- `node --test callFunctions/adminGooseRecords/*.test.js`：60/60。
- 独立游戏服务 `npm test`：176/176。
- 管理后台 `npm run build`：通过，保留已有大包提示。

## 部署核对

- tasks commit：`d5cd055`，已推送。
- server commit：`f02222d`，已推送。
- 云函数 `adminGooseRecords`：Active，更新时间 2026-11-17 22:24:18，线上入口代码含 potEditorLevels。
- CloudRun `goose-3d-pvp-065`：normal，流量 100%；远程配置及环境变量保持。
- 线上 health 返回 ok；编辑器配置接口错误凭据返回 401，正确凭据返回「请先在后台编辑器玩家页保存关卡配置」（尚未初始化，不擅自写入默认参数）。
- 通过 CloudBase 托管下载线上文件并核对 SHA-256：
  - admin/index.html：`d45aca3e80ce0f6925ce27e6987590c2236c18fa6d8ecb8198b1494265ac0ac2`
  - admin/assets/index-DjkIgaUW.js：`9715ff904d9dee642307412b33f1cc82a80f6e8c69a040b8312b5519cb3a44d7`
- 部署输入为当前工作区完整 admin-platform、adminGooseRecords 与 services/goose-server，部署前提交推送，目标目录无遗漏。其他项目和其他 worktree 不在此次发布范围内；未上传微信版本。
- 浏览器安全策略禁止访问后台域名，未完成线上浏览器点击验证。CloudBase MCP 连接中断后，改用现有 CLI 和本地 CloudBase MCP 查询部署状态及托管文件，未绕过浏览器策略。
