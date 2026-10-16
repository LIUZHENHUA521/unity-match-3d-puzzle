# 选择、消除与道具音效（2026-10-16）

仅本地修改，未发布。运行控制依据微信 2.1.12 WASM 与主场景，音频波形/83模型字段来自已取得的 iOS 1.11.03 资源。

## 运行路径

- `LevelManager.GetUnit(j13227)` 先 BeClick，仅 `playSound=true` 时调用 `ObjectUnit.PlaySound(j19260)`。按住选中 `BeSelect(j6603)` 没有这条播放调用，因此不是按下就响；有效收取才响。
- 普通物品按 `_effectSoundType` 选面粉、肉、蔬菜、塑料、玻璃、木材、金属、瓷器等原声音，不再统一播放项目原 `tile_click.mp3`。`none=0` 保留静音，不以默认点击声顶替。
- `SoundManager.PlayEffectSound(j3492)` 先检查音效开关；相邻同声音ID的间隔若小于 .05 游戏秒，则丢弃这次材质声。不同ID不会被这道门槛拦截。
- `_soundUsePitch=true` 时按物品名字维护连续列表：同名第1/2/3次分别使用音高1/1.15/1.3，第4次从1重新开始；换物品名也从1开始。不同物品即使共用材质声音，也不会被当成同一种物品。非pitch的消除/道具音不清这个列表。`ResetLevel(j33490)` 清名字列表，不重置静态最后音效与时间。
- `SoundManager` 主场景组件 path_id746 完整解析208 bytes；三路effect volume均1、pitch1/1.15/1.3。鹅独立声道volume .4/pitch1。
- `_isGoose` 另从31/32/33/34四个叫声随机选一段叠加；这次整数Random.Range推进全局随机状态。原鹅骨骼clips无音效事件，未给动作擅自添加叫声。
- `EndCombineObjAnim(j19872)` 调 `ObjectUnit.OnCombine(j36725)`：自定义 `_endSoundType` 若非0则播放；只有标记 `_endSoundCombie` 时另叠通用消除声，否则无自定义时用23=`S_Effect_Combine`。挂接在现有每组三消+.7秒完成点，替换原 `merge_sound.mp3`。
- 三基础工具：移出24，凑齐25；凑齐内部GetUnit的playSound=false，不能再为自动收取叠普通点击声。打乱没有被证明的独立音效调用，不把35=`S_ToolShake`混作打乱声。

## 资源与边界

43个已取得的微信物品完整组件按其字段布局解析耗尽全部字节，音效字段与iOS逐项相等；其余40个仅有iOS字段证据，未冒充微信直接交叉校验。

实际导出27段原始音频为解码WAV，保留采样率/声道/长度；原来源、path ID、SHA-256见 `Evidence/source-audio/audio-export-manifest.json`。音效开关继续由用户现有设置控制。第一关/PVP仍走原项目兼容流程，不把第二关修改称为所有模式一致。

静态资源解析不能证明线上热更音频相同，编辑器播放也不能证明手机扬声器响度相同。原音频压缩编码到WAV解码是平台适配，不是原压缩字节直接复用。

## 验证

`SourceAudioValidation.Run()` 检查83模型路由、12鹅配置、必要音频可加载且有采样、三档pitch与volume、50ms同ID门槛、换类型重置和跨消除保留连续音高。实际工具与飞槽验证覆盖调用时序。最终统一验证输出见本轮总报告。
