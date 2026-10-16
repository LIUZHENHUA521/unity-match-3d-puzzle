# 抓大鹅物理优化参考与当前实现

## 能确认的原版证据

App Store 抓大鹅 1.11.03 的 IL2CPP 元数据在 LevelManager 中保留刚体/可见性优化入口、刚体深度与最大物理物体数字段。下列偏移均为元数据偏移，不是机器码地址。

```json
[
  {
    "name": "_rigidDepth",
    "metadata_offset": "0x6ae08c"
  },
  {
    "name": "_physicCreateTime",
    "metadata_offset": "0x6ae590"
  },
  {
    "name": "_physicFrameCreateCount",
    "metadata_offset": "0x6ae5a8"
  },
  {
    "name": "_isUseRealPhysicSpace",
    "metadata_offset": "0x6ae5c0"
  },
  {
    "name": "_maxPhysicObjCount",
    "metadata_offset": "0x6ae5cc"
  },
  {
    "name": "_shakeOnceDepth",
    "metadata_offset": "0x6ae83c"
  },
  {
    "name": "_shakeMaxDepthCount",
    "metadata_offset": "0x6ae848"
  },
  {
    "name": "CheckRigid",
    "parameters": [
      "unit",
      "index",
      "checkMul"
    ],
    "metadata_offset": "0x27cd08"
  },
  {
    "name": "CheckVisible",
    "parameters": [
      "unit",
      "index"
    ],
    "metadata_offset": "0x27cd28"
  },
  {
    "name": "GetDynamicVisibleHeight",
    "parameters": [
      "index",
      "multi"
    ],
    "metadata_offset": "0x27cd48"
  },
  {
    "name": "LoopOptimization",
    "parameters": [],
    "metadata_offset": "0x27cd68"
  },
  {
    "name": "CheckAllRigidAndVisible",
    "parameters": [],
    "metadata_offset": "0x27cd88"
  }
]
```

函数体仍受 Mach-O 加密保护。不能从字段名确认具体最大数量、休眠阈值、是否用 isKinematic 或动态调整规则，也不能证明所有模式都走同一个实现。

## 开源项目的不同

Yuming0929/goose-catch，固定提交 028e75a21e25e8199385934633b8ff1759f24d59：Experience.tsx 使用 Rapier、零重力、中心吸引、timeStep="vary"；Item.tsx 使用 hull 刚体。它是第三方仿作，所检查源码没有我们的邻域唤醒和动态刚体预算机制，不能当成商业抓大鹅算法。本次没有重新启用此前用户否定的中心吸引生成方案。

## 我们的实现（自主参数）

- 保留 144 个物品、1.5 倍尺寸和原来的生成坐标，所有物品一开始就可见。
- ReplicaPilePhysics 最多允许 16 个动态刚体；其余以 kinematic 碰撞体参与支撑，仍可射线选中。
- 单次活动中连续稳定 0.25 秒且已运行 0.35 秒，或达到 2 秒模拟预算，就停算。2 秒是防止抖动长期占用预算的近似截断，不宣称完全自然静止；仍需真机观察悬停/补位观感。
- 初始按生成顺序排队落位。保持 3 秒准备时间，不等整队处理完才允许选取，避免约 15 秒的等待锁定。
- 选取成功后移出模拟管理，唤醒附近上方可能失去支撑的物品；撤回重新纳入管理；颠锅按预算分批唤醒。
- 不扩大碰撞体、不全局关闭物理、不修改服务器或关卡配额。
- 0.1.5 暂时保留诊断条，sim 显示动态/锅内总数，空闲应归零，运动时不超过 16。

## 验证边界

本机三主题各 144 个放大物品，峰值动态数均为 16；初次全队停算约 15.3–15.7 模拟秒，过程中 3 秒后即可操作。冻结物品拾取、移走唤醒、撤回、颠锅测试通过，没有观察到漏出锅。电脑 CPU 耗时不是 iOS 帧率保证，须在用户 iPhone 17 Pro 复测。
