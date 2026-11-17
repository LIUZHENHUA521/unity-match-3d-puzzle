# Pot 本局缩放解锁

状态：用户已确认；已接入代码，66 项 Unity 检查通过，尚未发布。

- 保留现有缩放入口，不增加广告图标。
- 未解锁时，点击放大或在棋盘内进行有效双指缩放，复用 ReviveClear 道具弹窗；不直接播放广告。
- 标题：自由缩放。说明：观看视频，本局可自由放大缩小。按钮：看视频解锁。
- 弹窗和广告期间暂停游戏计时与棋盘操作；关闭后恢复原游戏速度。
- 仅完整观看激励视频后解锁，本局不限次数；下一局重新锁定。不走免费或分享获取路线。
- 取消、未看完或播放失败不解锁；失败可以重试。手势触发后须松手重新操作，避免穿透拾取和画面跳变。
- 回到 1.0 倍或复活不重新锁定。新对局清除授权，旧广告回调不能解锁新对局。
- 回放速度按钮与 PVP 不受影响。
- 复用现有工具广告位配置；为缩放增加独立统计用途时，需核对客户端及服务端用途白名单，不冒用移出等道具用途。

## 图标

原图：pot-zoom-icon.png。游戏资源：Assets/Match3D/Resources/Match3D/GameplaySkin/pot-zoom-icon.png（96×96，约 12 KB）。通过内置 image_gen 进行裁剪整理，再以 sips 导出项目所需像素尺寸。使用内置 image_gen 生成，透明底，金黄色边框、蓝色镜面和手柄、白色加号、亮面卡通放大镜。

生成提示词：Generate a single game power-up icon: a chunky toy magnifying glass with a bold white plus sign in its blue lens, rounded golden yellow rim and short blue handle. Casual mobile puzzle game 3D cartoon render, glossy plastic, simple readable silhouette, front view slightly angled, soft bevels and bright highlights. Match the visual simplicity and polish of a small red-blue toy magnet power-up icon. Centered isolated object filling 80% of square canvas, transparent background, no text, no button frame, no video/ad symbol, no extra objects. This is the zoom power-up icon for Pot match 3D puzzle.

## 实现后验证

覆盖按钮与双指入口、弹窗去重、取消及广告失败重试、成功后反复缩放、复活保留授权、下一局重新锁定、旧回调隔离、计时恢复和回放隔离。运行现有 BoardCameraZoom 验证并补充广告门禁案例；真实微信端验证激励视频回调。

验证结果：`python3 Tools/validation/BoardCameraZoom/run.py` 通过 66 项检查；`node Tools/wechat/test-ad-telemetry.cjs` 通过；`POT_TEST_AD_PLACEMENT=pot_zoom node Tools/wechat/test-ad-telemetry-integration.cjs` 通过。真实微信端激励视频仍需设备验证。服务端 `services/goose-server/src/ad-events.mjs` 已增加 pot_zoom 用途白名单，尚未部署。
