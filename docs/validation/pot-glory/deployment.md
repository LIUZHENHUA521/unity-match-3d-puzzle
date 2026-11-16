# 荣耀列表服务端部署

- 日期：2026-11-16（Asia/Shanghai）。
- 服务：goose-3d-pvp；环境：cloud1-1g0u68ow59705e3f。
- 线上版本：058，normal，100% 流量。
- 源码提交：6c098c283ad81f90bf337574379c3e8cb755b741，已推送。
- 输入：目标服务全部当前工作区文件（77 个）。云端上线后源码回读 76 个文件 SHA-256 全部一致，排除工具生成的 cloudbaserc.json；原工作区与部署输入也一致。
- 镜像：ccr.ccs.tencentyun.com/tcb-100026349433-ntfu/ca-hadnxhdq_goose-3d-pvp:goose-3d-pvp-058-20261116215213。
- 摘要：sha256:c7a020866c3f90f732415939dde34f919497dbe4e01dc65d309824493fdde16b。
- 保留原 CPU 1 / 内存 2 GiB / 单实例 / 端口 8080 / 全部环境变量与公开访问配置。
- userCertificates 新增非唯一复合索引 pot_glory_owner_time：gameId(1)、sourceAppid(1)、sourceUserId(1)、claimTime(-1)、certificateKey(-1)。索引列表确认存在；真实数据相同过滤/排序的只读查询成功。既有索引、数据和权限未改动。
- 服务端 167 项测试通过；正式域名和云托管默认域名的 /health、/invites/health 全部 HTTP 200，数据库与登录配置正常；未认证 honors 请求返回 401。
- 未伪造玩家会话，未创建测试奖状或奖励；不声称线上真实通关/发券验收。
- 范围仅目标游戏服务。同仓库另一个服务的未提交改动、其他历史分支/worktree 未纳入本服务部署。没有上传微信包或部署管理后台。
- 客户端底部两个文字入口已移除，Unity 真实 HTTP 夹具与三种屏幕截图验证通过；客户端提交 af918ba8，尚未上传。
