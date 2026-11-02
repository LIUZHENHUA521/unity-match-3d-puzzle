# ShowTips 消息提示

参考疯狂动物消 `client/assets/resources/scripts/Tips.ts` 和 `pre_tips.prefab`，采用居中半透明黑底（alpha 108/255）、35 号白色粗体文字。显示 1.5 秒，连续调用替换内容并从最后一次调用重新计时，空白消息忽略。

调用：`ReplicaFlow.Instance.ShowTips("操作成功")`。

旧的 `Notice` 和 `ShowHomeFeatureNotice` 自动转到同一组件，现有功能未开放、道具异常、分享、对战、记录同步和红包提示继续有效；进入最后一关的提示也统一接入。

`ReplicaTips` 懒创建并复用 Notice 预制体的字体。独立 Canvas 排序为 100，覆盖普通弹窗；文字和背景不接收射线，不改变游戏暂停状态。短消息最小宽 350，长消息按屏幕宽度自动换行。使用真实时间判断隐藏，不依赖 Flow 协程，切换页面时清理旧提示。

## 验证

Unity 编辑器菜单：`Goose Replica / Validate ShowTips`。验证会进入 Play Mode，在结束时退出；在已运行的 Play Mode 中执行则保留运行状态。

本次 Unity Play Mode 验证通过：显示消息、无点击拦截、连续替换重新计时、空白忽略、暂停及停止 Flow 协程后隐藏、长文字换行、弹窗上层、页面切换清理及旧入口兼容。实际 Game View 截图确认长提示在用户资料弹窗上方可见。

结果：`docs/validation/show-tips/result.txt`；截图：`docs/validation/show-tips/preview.png`。

未重新打包或上传微信体验版，尚未进行手机真机验证。
