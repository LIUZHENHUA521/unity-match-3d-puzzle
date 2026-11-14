# 编辑器内测关卡验证

服务端 `services/goose-server/test/pot-editor-beta.test.mjs` 验证专用凭据、与真实内测玩家配置的一致性、配置变化、协议/档位校验、缺失配置和禁止写库。

Unity：运行 `python3 Tools/validation/PotEditorBeta/run.py`，自动启动 127.0.0.1:18097 测试接口，第一次返回 tasty-kitchen / 30 个物品 / 90 秒，第二次返回 12 个物品。脚本使用隔离项目，不覆盖当前编辑器或预览凭据。

校验首页两档入口、重复点击只发一次、两阶段数量、服务端限时、重试保留当前关卡以及缺失凭据不回落本地关卡。报告：`docs/validation/pot-editor-beta/checks.txt`。
