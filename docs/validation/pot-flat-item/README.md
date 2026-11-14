# 当前模型平放测试（2026-11-14）

使用当前工作区 Assets / Packages / ProjectSettings 的隔离副本，在 Unity 编辑器执行生产锅面碰撞和第二关容器，未改动游戏逻辑或锅的位置。

27 组：Party_Pizza、Party_Waffle、Party_PancakesStack × 0.5/1/1.25 尺寸倍率 × 从 Y=3 落下、贴锅面平放、隐藏底板附近放置后执行 SoftTransitionContainer.Fit。每组模拟 600 × 0.02 秒。截图为测试相机和测试灯光，不是完整游戏 HUD。

锅面 Y=0.48466。0.5 倍披萨从上方落下/锅面平放均保持可见，最终最低点 0.48434，最高点 0.53045。人为放到隐藏底板附近并 Fit 后，最终范围 0～0.04534，完全被锅面遮住。0.5 倍华夫饼/松饼也能在这类人为低位放置后完全隐藏。

限制：这证明存在可容纳薄物体的遮挡区，并未证明真实对局通过何种操作进入该区域；非微信真机/发布 Player 验证。正常放置组的大尺寸物品可能受到狭窄容器挤压而倾斜，1.25 倍披萨贴锅面组存在部分穿插，不能描述为全部通过。pick 列仅为独立 PickLayer 射线检测，不代表实际可见网格点击成功。

脚本：Tools/validation/PotFlatItem/PotFlatItemValidation.cs，仅复制到隔离项目 Assets/Match3D/Editor 后以 -executeMethod PotFlatItemValidation.Run 执行。输出在隔离项目上级 results。
