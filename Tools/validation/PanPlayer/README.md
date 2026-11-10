# 锅面碰撞的 Player 回归

只能在隔离 Unity 项目运行；不要对正在编辑的主项目执行。

1. 将 `PanPlayerProbe.cs` 复制到隔离项目 `Assets/` 下，`PanPlayerProbeBuild.cs` 复制到 `Assets/Editor/` 下。
2. Unity 2022.3.62f3c1 以 `-batchmode -nographics -buildTarget StandaloneOSX -executeMethod PanPlayerProbeBuild.Run` 构建。产物为隔离项目上级 `pan-investigation/regression.app`。
3. 执行应用内 `Contents/MacOS/<productName> -batchmode -nographics --report <绝对路径> -logFile <绝对路径>`。退出码非零即失败。
4. `-executeMethod PanPlayerProbeBuild.CheckReleaseGuard` 临时关闭锅模型 Read/Write，验证发布检查确实拒绝，并在 finally 恢复。只可在隔离副本运行。

真实 Player 与编辑器的网格可读性不同：编辑器能读取未开启 Read/Write 的导入网格，不能仅用 Play Mode 检查动态 MeshCollider。

用实际生产锅模型和 AddPanCollision 创建碰撞面，再加入第一关边界；验证锅中心向下射线的命中高度。巧克力、面包和瓶子各覆盖 0.5/1/1.25 三种尺寸、12 组角度/落点/高度，共 108 组，每组模拟 12 秒。高度取 3 与 12，避免从第一关安全顶板上方投放。检查实际网格顶点最低点，不使用旋转物体包围盒的空白角点；顶点在构建测试场景时序列化，不依赖道具网格在 Player 可读。允许 0.04 的接触误差。

此测试验证打包后的碰撞创建与支撑，不等同于微信/iOS 真机验收。测试脚本留在 Tools，正常游戏发布不包含这些组件或场景。
