# 模型表面选取修复（2026-11-10）

用户要求：点击范围贴合模型，射线只检测物品点击层。

## 实现

- InputController 仅查询 PickLayer=7。使用可扩容的无分配射线缓冲区，按物品去重，再按实际网格命中距离决定最前面的物品。
- ItemMeshPicking 将可见 MeshRenderer 的三角形作为最终点击形状，按网格共享缓存与空间树裁剪检测。SkinnedMeshRenderer 使用当前姿态烘焙结果。模型空隙允许选中后排，真实表面仍阻挡后排；最近可见物品的可选状态最后判断，避免穿透暂不可选物品。
- 在原有点击节点下添加覆盖可见几何的候选 BoxCollider，避免原点击框偏小而漏掉模型露出的部分。这个框仅作候选筛选，不决定最终命中。动画物品更新候选范围。
- 原有实体碰撞几何不变；新候选框位于不参与物理碰撞的 PickLayer。隐藏、收取和对象池复用沿用原生命周期并恢复新增框。
- 为旧关卡生成路径增加 PickLayer 候选框；模型适配器和构建预处理确保 CPU 网格数据可读。69 个模型导入配置仅将 isReadable 从 0 改为 1。该设置会增加玩家端网格 CPU 内存占用；没有改贴图读取设置。

## 验证范围

独立 Unity 2022.3.62f3c1 项目副本执行 `ItemMeshPickingValidation.Run`，结果见 `report.txt`。覆盖：实体遮挡、包围盒空白、多个模型间的真实空隙、物品点击层过滤、网格距离排序、不可选前景阻挡、隐藏/禁用、旋转及负数非均匀缩放、点击框外可见几何、复用恢复、按住滑动换选、空白取消、70 候选缓冲扩容、全部适配模型及四种带骨骼鹅模型。

性能采样是桌面编辑器结果，不能代表手机帧耗时。当前完成代码与编辑器验证，未发布微信体验版，也未进行真机触摸验证。精确范围来自三角形几何，不包含贴图透明像素或 shader 顶点位移的逐像素拾取。

## 复现

```sh
/Applications/Unity/Unity.app/Contents/MacOS/Unity -batchmode -nographics \
  -projectPath <独立项目副本> -executeMethod ItemMeshPickingValidation.Run \
  -logFile <日志路径>
```

退出前报告保存在副本的 `ValidationOutput/item-mesh-picking-validation.txt`。
