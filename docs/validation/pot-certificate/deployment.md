# 证书功能服务端部署记录

- 日期：2026-11-16（Asia/Shanghai）
- 环境：cloud1-1g0u68ow59705e3f
- 服务：goose-3d-pvp
- 线上版本：057，normal，100% 流量
- 源码提交：2f9b117d56028d0300b10d5da020570d64e3e6b5（已推送）
- 范围：services/goose-server 完整当前源码，包含通关证书签发及广告分享频次配置。
- 测试：165 项通过。
- 云端源码回读：74 个文件 SHA-256 一致（排除工具生成的 cloudbaserc.json）。发布后再次确认原工作区源码无变化。
- 镜像：ccr.ccs.tencentyun.com/tcb-100026349433-ntfu/ca-hadnxhdq_goose-3d-pvp:goose-3d-pvp-057-20261116213230
- 镜像摘要：sha256:0ae1facc55c43d5a9dc81e0533106daaa38e440b6bd34e9118de08b382027f29
- 配置：保留现有 1 核 / 2 GiB、单实例、8080 端口及环境变量。
- 正式域名 9nanimal.cn 与云托管默认域名的 /health、/invites/health 均 HTTP 200；ok、ready、database、loginConfigured 均为 true。
- 未上传微信版本，未部署同仓库其他服务。未通过创建真实红包或奖励验证接口。
