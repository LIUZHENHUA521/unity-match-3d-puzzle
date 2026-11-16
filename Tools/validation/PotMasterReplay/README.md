# 大神回放本地集成验证

运行 `python3 Tools/validation/PotMasterReplay/run.py --replay /absolute/path/to/fixture.json.gz`。

需要兼容当前资源版本的本地录像和 `/Applications/Unity/Unity.app`。脚本复制完整当前 Assets、Packages、ProjectSettings，启动隔离本地 HTTP 服务，模拟每日关卡、列表与下载授权。微信广告使用 Unity 编辑器回调替身。不会上传录像或修改生产数据。

测试结果和截图：`work/pot-master-replay/project/work/pot-master-replay/`。

状态机与资源导入验证：Unity `-executeMethod PotMasterReplayValidation.Run`。
