# 五套主题背景

使用内置 image_gen 生成；完整提示词见 prompts.json。统一俯视柔和 3D 卡通桌面，1:2 竖图，装饰集中在角落，中央和下部留白供锅、模型及 UI 使用。无文字、无 UI、无预绘制锅。

- pot-vegetable：田园菜篮，浅绿桌面、蔬菜篮。
- pot-meat：肉肉盛宴，暖米色桌面、烤肉木盘。
- pot-grocery：元气超市，浅蓝桌面、购物袋和购物篮。
- pot-fruit：缤纷果园，奶油黄桌面、水果野餐。
- pot-sweets：甜蜜小屋，粉紫桌面、甜点和彩带。

PNG 为生成原图；manifest.json 对应的 JPEG 为游戏运行时资源，导入设置沿用现有单 Sprite 背景（Clamp、无 mipmap）。五个主题的 background 已分别绑定；再导入食材包时保留对应背景，发布流程覆盖新背景压缩。未部署或上传微信版本。

已检查原图比例、Sprite 导入设置、唯一资源 GUID 及五个主题引用；未进行 Unity 实机画面验收。
