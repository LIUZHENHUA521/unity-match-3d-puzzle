# 本科局大神回放验证（2026-11-16）

## 行为

- 本科局旁新增独立入口；服务端按当前日期、关卡版本、资源版本，筛选已公开、已就绪、第二关完整通关、gzip 可下载录像。总数至少 6 条时显示，最多展示耗时升序的十条，耗时相同按录像 ID 排序。
- 使用 Photoshop「一锅消-弹窗-录像.psd」导出的面板、关闭、行底板和按钮。保留原 PSD；列表支持滚动与屏幕适配。
- 一次完整激励广告只解锁所选录像的一次播放；下载失败可在当前列表重试同一条，切换录像、退出列表、换账号、跨天或关卡版本更新清除资格。
- 广告结束后重新查询当前本科局，避免播放已替换关卡的录像。播放成功后消耗资格，退出回到列表。原个人回放保持免费。
- 广告使用现有 toolAdUnitId，placement 为 `pot_master_replay`，公共统计使用兼容的 `bonus` 分组；首页广告使用独立浏览请求 ID，不伪造实际对局。
- 用户已授权同步的昵称、头像通过 `/pot-replays/profile` 保存为公开回放显示资料；没有资料时显示「微信玩家」和默认头像。列表不返回账号 ID、openid、token。昵称不解析富文本，头像只接受 HTTPS qlogo.cn 域名。

## 验证

- 服务端完整测试：169 项通过（筛选/总数/前十排序、资料边界与独立广告归因包含在内）。
- Unity 编辑器编译、资源导入和状态单元验证通过；`Match3DDefaultFontValidation.CompileWebGL` 编译通过。
- `integration.txt` 记录真实本地 HTTP + Unity Play Mode 验证，覆盖入口、十条列表、取消/重复广告、失败重试、账号切换、关闭页面的迟到回调、实际录像播放、退出返回、重看再次广告、广告期间关卡版本变化及错误重试。
- 原个人回放完整回归通过，包含开始挑战原样、个人上传状态、下载提示、取消下载、实际播放与退出；见 `personal-replay-regression.txt`。
- 原生 JS 广告回调、持久化队列和 HTTP 路由测试通过；测试使用隔离存储与公共统计转发替身，不调用生产广告统计。
- `home.png`、`list-short.png` 和 `list-tablet.png` 为 Unity 渲染结果。列表行高 112、行间距 12，排列步长 124；首屏完整显示七行，后续通过滚动查看。已修正原先步长 100 导致的相邻底板重叠，并重新通过完整集成验证。

## 运行

`python3 Tools/validation/PotMasterReplay/run.py --replay /absolute/path/to/compatible-fixture.json.gz`

脚本复制当前 Assets、Packages 和 ProjectSettings 到 work/pot-master-replay/project，只使用本地 HTTP fixture。首次运行可使用本项目 Library 的 APFS 克隆加快导入。纯状态验证入口为 `PotMasterReplayValidation.Run`。

服务端源码位于 crazyCombo/server/services/goose-server。新增 `kind=pot-masters` 使用 `pot_replays` 的相同筛选条件进行 count 与 duration/id 排序查询，保持旧接口、标识及发布权限兼容。未自动公开任何历史录像。

本轮交付为开发和本地验证，未部署服务端、未上传微信版本。真实微信激励视频及真机表现尚未实测，线上启用需要配套发布服务端与客户端，并确认新列表查询索引可用。
