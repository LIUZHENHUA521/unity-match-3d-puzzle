# Pot 发布期名称混淆（试验开关）

仅改写发布副本的玩家程序集。源码、编辑器程序集、组件类型、字段、公开接口与协议保持原样；不增加控制流、加密、运行时解密或垃圾指令。本版仅覆盖 methods.txt 中 15 个私有方法，**不是整个游戏的全面混淆**。类型、编译器生成的协程/闭包名称与字符串仍可能提供算法线索。

```sh
python3 Tools/wechat/build.py --stage Builds/PotProtectedProject --obfuscate
python3 Tools/wechat/obfuscation/test.py
python3 Tools/wechat/obfuscation/compare.py Builds/PotObfuscationProject Builds/PotProtectedProject
```

默认发布流程仍不启用试验开关。未执行上传、提审或正式发布。不要手工把这些 C# 工具放入主工程 Assets。

## 构建方式

- build.py 从完整当前 Assets/Packages/ProjectSettings 创建独立快照。
- 仅在该快照中安装 IPostBuildPlayerScriptDLLs 回调。用本机 Unity 2022.3.62f3c1 附带的 Mono 与 Unity.Cecil 编译工具，无需商业插件或额外 NuGet 包。
- 回调只处理 BuildReport 指定的独立副本内唯一玩家 Assembly-CSharp.dll（微信 SDK 使用 Temp/StagingArea/Data/Managed），在 IL2CPP 前重命名明确列出的私有方法及引用。
- 缺少/重复目标、重载歧义、虚方法、接口覆盖、泛型方法、PInvoke、特殊方法、编译器生成方法、常见 Unity 回调、程序集内字符串引用及资源中明确的字符串/UnityEvent 引用会使构建失败。动态拼接的反射名称无法静态证明安全；扩展名单前必须审查调用方式。
- 写回前比较方法指令、局部变量、异常处理区、类型/字段/属性/事件和方法签名等，除指定名称和引用外有变化即失败。PDB 同步重写。重复处理同一 DLL 会失败，重新构建需使用干净输入。
- 映射及回执保存于构建副本的 BuildInputs/PotObfuscation，SDK 诊断符号也在构建完成后移入此目录。此目录在被忽略的 Builds 下，不进入小游戏包。保留每版对应的映射与符号用于排查。
- BuildInputs/pot-input-hashes.json 记录构建前输入；pot-package-sizes.json 记录文件大小。packageBytes 按 SDK packOptions 排除项计算本地待上传文件总量，不等于微信平台最终压缩包的官方计费/限制口径。

## 验证与维护

test.py 覆盖直接调用、嵌套调用、委托、迭代器、IL 等价，以及不安全/缺失/字符串引用目标和重复执行的拒绝。PotSeedProbe.cs 可运行实际游戏程序集的 Generate 与 GenerateSource（注入相同伪随机流），比较 36 组关卡输出及随机调用次数。它不模拟 Unity 原生物理或微信设备。

扩展保护范围应单独审查名单，不需要修改业务源码。需在微信真机验证三消、满槽、道具、复活、回放及性能后再考虑默认启用。名称混淆增加阅读成本，无法阻止算法还原。

本次构建与体积结果见 `docs/validation/pot-release-obfuscation.md`。
