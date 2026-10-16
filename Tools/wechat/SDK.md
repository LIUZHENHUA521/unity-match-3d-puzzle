# 微信 SDK 与字体来源

官方仓库：https://github.com/wechat-miniprogram/minigame-tuanjie-transform-sdk
固定提交：a09d4b29daa1dd8358b09b5b5639554ab08cfdc2
嵌入目录：Packages/com.qq.weixin.minigame

Git 传输较慢，因此按该提交的 Git tree 下载官方 raw 文件；651 个文件均逐个核对 Git blob SHA-1 一致后导入。SDK 代码未修改；MiniGameConfig.asset 配置为当前游戏的 AppID、输出目录和竖屏分包构建。Editor/Node/node_modules 是 SDK 自带构建依赖，应保留。

中文字体：Noto Sans CJK SC Regular，来源 https://github.com/notofonts/noto-cjk ，SIL Open Font License 随字体放在 Assets/GooseReplica/Resources/Replica/Fonts/LICENSE.txt。GooseSans.otf 只保留当前源码和主题文案用到的字符；主题名称的字形覆盖已检查。添加新文案时需补充字体子集。
