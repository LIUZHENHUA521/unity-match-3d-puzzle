> 后续修正：[container-framing.md](container-framing.md) 保留本报告的固定镜头/锅，进一步修复实际物品堆偏上。空锅验证不能证明物品取景正确。

# 切关时锅与镜头保持稳定

第一关约 82°、第二关 90° 的镜头角度切换，会改变锅沿、锅柄和背景的投影。
上一版虽然对齐锅口中心，仍会看到轻微跳动。

现在两关全程复用第一关的完整镜头姿态和锅 Transform。删除第二关专用镜头位置、
旋转和切关重定位调用；重开第二关同样使用第一关镜头。第一关原碰撞不变，第二关
仍为竖直（0°）的短段圆滑容器，碰撞体方向与镜头角度相互独立。

## 验证结果

在独立 Unity 2022.3.62f3c1 项目快照中运行 `StageTransitionViewValidation.Run`，
编译成功、批处理退出码 0。通过五次实际三消自然切关，等待第二关创建协程完成，
再验证第二关重开。

- 连续观察 470 帧，其中第二关 176 帧，锅及其渲染器全程保持启用。
- 相机视图矩阵、投影矩阵、锅世界变换矩阵的最大变化均为 0。
- 720×1280 渲染中，第一关与刚进入第二关、第二关创建完成时的锅/背景图像，
  差异像素数和最大 RGB 差值均为 0。
- 第二关实际生成 300 个物品，容器方向保持竖直；重开保留相同镜头矩阵。

为隔离锅和背景，截图时临时隐藏物品等非场景视觉渲染器并立即恢复；测试期间暂停
自动物理模拟，但切关和第二关创建运行实际协程。这是编辑器中的镜头/视觉回归，
不代表微信真机全画面或性能测试。此前物理回归见历史报告，本次没有重复运行。

[原始报告](stable-view/validation.json) · [第一关](stable-view/first-stage.png) ·
[进入第二关首帧](stable-view/first-second-stage-frame.png) ·
[第二关创建完成](stable-view/second-stage-ready.png)

退出 Play 后重新进入以加载新代码。本次未发布或上传。

## 复现

仅在独立快照运行（需要图形渲染，不使用 `-nographics`）：

```sh
/Applications/Unity/Unity.app/Contents/MacOS/Unity -batchmode \
  -projectPath <snapshot> -executeMethod StageTransitionViewValidation.Run \
  -logFile <validation-log>
```

报告和截图写入快照上级 `results/stable-view/`，完成后退出该快照编辑器。
