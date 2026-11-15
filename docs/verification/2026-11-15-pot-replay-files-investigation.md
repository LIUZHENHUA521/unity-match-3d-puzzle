# 2026-11-15 回放文件桥排查

## 云端证据（约14:35–14:37，北京时间）

- 第二关当日通关记录68条，其中5条有对应 `_upload` 会话。
- `pot_replays` 中 date 为11月14日、15日的记录均为0；暂存目录为空。
- 当日暂存路径日志 GetUploadMetadata 9272次、BatchGetDownloadUrl 4822次。凭据请求不代表文件上传成功。

## 可重复的失败

用当前 `PotReplayFiles.jslib` 原样放进同AppID独立小游戏诊断项目，微信开发者工具RC 1.06.2402021、基础库3.18.0：mkdir、write、size、open均成功，readBytes返回：

`{"ok":false,"error":"wx.arrayBufferToBase64 is not a function"}`

该接口在此小游戏运行环境中不存在；原桥还依赖 `wx.base64ToArrayBuffer`。录像恢复和压缩要经过这座文件桥，失败发生在PUT之前。昨日直接上传诊断文件绕过了文件桥，不能覆盖这一缺陷。0.1.75导出代码也含上述调用。

## 修改与验证

- 文件桥改为自带Base64编码/解码，保持C#与JS协议、文件路径和二进制内容兼容，不引入整份录像内存拷贝。
- 新增 `Tools/validation/PotReplay/pot-replay-files.test.cjs`：在wx不提供Base64接口时，通过真实磁盘验证1、2、3、255、32767、32768字节二进制及偏移读取；验证无效Base64拒绝写入。2项测试通过。
- 修复后微信运行时readBytes返回 `aGVsbG8gcmVwbGF5`，writeBytes返回size=5，关闭后stat为5；实际文件桥读写复测通过。
- `git diff --check` 通过。

## 边界

这是已复现的客户端上传前阻断缺陷；尚未取得每位玩家的错误日志，不能将其解释为68条通关全部缺失的唯一原因，尤其63条没有上传会话。
本轮没有发布新包，没有声称完整Unity打包链或真实玩家录像的压缩、上传、校验、入库闭环已通过。已有本地录像是否保留须按实际设备确认，不保证全部恢复。
