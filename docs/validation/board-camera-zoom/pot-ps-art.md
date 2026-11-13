# Photoshop 缩放控件美术接入

2026-11-13，从当前 Photoshop 文档「游戏中-野餐背景-蓝色已命名1(1).psd」导出独立图层：

- img_btn_reduce：56×56，含减号。
- img_btn_add：56×56，含加号。
- img_whiteLine_cube02：透明边缘裁剪后 110×56，仅倍率底板。

导出通过临时文档隔离图层完成，保留透明度；源文档的图层、选中状态和保存状态未改变。原始美术名称保留，Unity 资源目录为 Assets/Match3D/Resources/Match3D/PotCameraZoomSkin。

中间的 `1.0 x` 不导出为图片，由 Unity Text 以白色粗体实时显示倍率。使用美术原始尺寸和横向间距，沿用现有 HUD 的安全区位置。边界禁用和双指操作规则保持。

完整项目在隔离副本中编译成功，已有 29 项相机交互检查通过。pot-ps-art-controls.png 为真实 Unity HUD 空锅夹具渲染，已目视确认按钮及文字对齐。本轮未打包上传。
