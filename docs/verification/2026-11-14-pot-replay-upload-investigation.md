# 每日回放上传排查（2026-11-14）

## 线上证据

- 约16:30（北京时间）查询 `goose_game_records`：`levelDate=2026-11-14,status=win,stage=2` 共35条；按平台为iOS 12、Android 21、Windows 2。
- `pot_replays` 总共4条，全部属于11月13日；当天回放0条。后台列表直接查询该集合的 `ready:true` 记录，因此问题发生在回放入库之前。
- 分页查询 `pot_replay_chunks` 共1152条，其中16条旧分段、1136条直传会话。1134条会话的当前凭据签发时间在当天；会话并非上传成功的证据。35条胜局中4条存在对应直传会话，其余31条为何未申请上传，本次没有确定。
- 云存储 `pot-replay-incoming/` 列表为空，`pot-replays/` 只有与数据库对应的4个历史文件。
- 16:37左右检索当天存储访问日志：`GetUploadMetadata` 3593次、`BatchGetDownloadUrl` 1878次。抽样能看到同一路径反复申请凭据、查询文件，查询的上游状态为404。这些日志不等于完整的COS请求日志，不能单凭无其他action断言请求从未发出。
- 云托管 `goose-3d-pvp-038` 正常，100%流量，当前部署时间11:48。

## 已确认的配置缺口

16:39通过微信开发者工具重新打开已上传的0.1.75独立发布目录，刷新获得AppID `wx74f2d8722c02a6f4` 的域名配置。request合法域名只有：

- `https://9nanimal.cn`
- `https://cloud1-1g0u68ow59705e3f-1312757046.ap-shanghai.app.tcloudbase.com`
- `https://tcb-api.tencentcloudapi.com`

未包含COS/云存储直传域名；uploadFile合法域名为空。当前上传桥实际使用 `wx.request` 的PUT请求，检查的是request合法域名，不能仅补uploadFile列表。

当前开发者工具控制台明确显示“配置中关闭合法域名、web-view（业务域名）、TLS版本以及HTTPS证书检查”。因此本地成功不能证明正常真机环境能够直传。

配置来源为开发者工具此次刷新产生的 `localstorage_09716a03ce8f6253a2f294d032e80f6b.json` 中 `attr.network`；没有使用10月的旧缓存作结论。

## 上传与重试代码

- `Assets/Match3D/Plugins/WebGL/PotReplayUpload.jslib:13`：直接向服务端ticket.url发出wx.request PUT，未经过已允许的业务域名。
- 同文件第16行丢弃wx.request fail的原始errMsg，统一返回“云存储上传中断”。
- `PotReplayService.cs:109-111` 又将上传结果压缩成bool、显示通用提示，每次尝试结束删除临时压缩包。原始 `.ready` / `.recording` 仅在服务端确认完成后删除；仅凭云端0条无法断定用户本地原始录像也已丢失。

## 结论与边界

已定位到回放文件成功落云存储之前，并确认缺少云存储request合法域名这一阻断项。需要核对服务端实际签发的上传URL主机，补齐精确域名，在开启域名校验的环境完成真实PUT及upload-complete闭环；同时保留原始上传错误，避免再次只能看到泛化提示。

本次没有修改线上域名、服务端或客户端代码，没有发布新包，也没有宣称恢复成功。尚未取得具体失败上传请求的原始errMsg或完整ticket URL，域名问题不能被当成所有未上传胜局的唯一原因。

参考：微信小游戏转换SDK的[网络说明](https://gitee.com/wechat-minigame/minigame-transform/blob/main/Design/UsingNetworking.md)。

## 补充：实际签发的上传域名

2026-11-14 使用服务端已安装的同一 Node SDK，针对同一环境调用 `getUploadMetadata`，仅取得凭证并输出 URL 的协议及主机，没有上传文件。实际返回：

`https://636c-cloud1-1g0u68ow59705e3f-1312757046.cos.ap-shanghai.myqcloud.com`

该主机不在此次开发者工具刷新得到的 RequestDomain 列表中。客户端使用 `wx.request` 的 PUT，因此需要加入 request 合法域名。以上补充确认签发地址，仍不代表已经取得失败真机请求的原始错误或验证修复闭环。

## 用户配置后复核（17:02）

重新打开项目后，开发者工具缓存更新时间为 2026-11-14 17:02:17。上述 COS 域名已经出现在 `UploadDomain` 中，`RequestDomain` 仍为原来的三个域名，未包含 COS 域名。说明本次加到了 uploadFile 类别，当前 `wx.request` PUT 上传所需的 request 类别仍缺失。

复核 `pot_replays` 全集合仍只有昨日的 4 条记录，暂存目录 `pot-replay-incoming/` 为空。本轮没有上传测试文件，也没有验证上传成功；本地临时添加的校验设置已撤回，没有发布代码。

## 第二次配置后复核（17:06–17:07）

- 17:05:41 刷新配置后，COS 主机已同时出现在 `RequestDomain` 和 `UploadDomain`。
- 建立独立的临时小游戏诊断项目（同 AppID，`urlCheck:true`），按回放桥相同的 `wx.request` PUT、ArrayBuffer 和凭证请求头发起上传。
- 17:06:43 正向上传返回 HTTP 200；未列入白名单的 `.invalid` 对照域名同时返回 `request:fail url not in domain list`，确认域名检查实际开启。
- CloudBase 读回 `pot-diagnostics/pot-upload-check-1794647188364.txt`，内容为 `pot upload diagnostic 2026-11-14` 加换行，33 字节，与测试内容一致。仅为独立诊断文件，未创建或冒充玩家录像。
- 再查 `pot_replays` 的正确日期字段 `date=2026-11-14`，仍为 0 条。因此已验证域名配置与小文件上传通路恢复，尚未验证真实录像的上传、校验及入库闭环。
- 临时测试项目已关闭，脚本内短期上传凭证已移除。没有改动或发布游戏业务代码。
