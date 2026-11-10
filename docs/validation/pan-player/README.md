# 发布包锅底穿插修复（2026-11-11）

用户确认 0.1.44 仍有巧克力等薄物品被锅面截掉。根因不是单纯调低了物品碰撞体，而是锅模型 `frying-pan.fbx` 的 Read/Write 关闭；`Match3DThemePresentation.AddPanCollision` 在运行时创建 MeshCollider，真实 Player 没有可用于烹制碰撞的 CPU 网格数据。

使用 0.1.44 源码与 Unity 2022.3.62f3c1 构建独立 macOS Player，明确复现 `CollisionMeshData couldn't be created because the mesh has been marked as non-accessible`；锅面碰撞体范围为零、射线不命中，巧克力最低点落到旧底板附近 Y=0.004–0.027。编辑器完整第一关 8 组未复现，说明原有 Editor-only 验证无法覆盖这个发布差异。

## 修改

- 开启这一个锅模型的 Read/Write，保留运行时创建贴合网格碰撞体的现有路径。
- 锅模型重新导入/替换工具保持此标志，防止重建后回退。
- 微信发布前检查所有主题内此锅的网格可读性；不符合时阻止发布。
- 不修改锅或镜头坐标、物品尺寸、形状、重力和物理调度。

## 验证

- 相同独立 Player 对照：锅面碰撞恢复，中心命中 Y≈0.485；前 6 组相同巧克力落下实验最低点为 Y=0.481–0.483。
- 原始对照报告的后 6 组从 Y=22 开始，落在第一关安全顶板上方，不能作为锅底落下验证。最终回归已改成 Y=3/12，均从顶板下方落下。
- 最终真实 Player 回归：巧克力、面包、瓶子 × 3 种尺寸 × 12 组姿态/落点/高度，共 108 组，全部通过。用模型真实顶点检查最低高度，避免旋转瓶子的包围盒空白角点造成误判。
- 在隔离项目把 Read/Write 临时关掉，发布检查确实拒绝；恢复后通过，日志 `PAN_RELEASE_GUARD_REJECTED_UNREADABLE=True`。
- fixtures 与复现步骤见 `Tools/validation/PanPlayer/`，未加入正常游戏 Assets。

报告：[错误日志](baseline-errors.txt)、[修改前](baseline.txt)、[相同条件修改后](fixed-comparison.txt)、[108 组运行包回归](regression.txt)。这是实际 macOS Player 验证，微信/iOS 真机表现仍需更新后确认。

## 微信包

0.1.45 从已提交源码 5f43904 的隔离快照构建，WebGL 导出、包检查及开发版本上传成功。确认构建输入保留 Read/Write，并排除诊断组件和场景。未设为体验版（公众平台受浏览器策略限制），未提审。见 release-0.1.45.json。
