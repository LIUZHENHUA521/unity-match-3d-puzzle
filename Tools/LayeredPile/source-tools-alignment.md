# 原版道具逻辑对齐（2026-10-16）

范围：WX 2.1.12 包内默认普通单人第二关；iOS 1.11.03 序列化参数作为交叉证据。代码只恢复本地道具执行，不请求广告、付费或在线资源。没有发布。

## 本轮确认并实现

| 道具 | 原版证据 | 实现 |
|---|---|---|
| 移出（ToolReturnBack，0） | `LevelManager.UseTool j38482` 从 `_contentUnitList` 取前 `min(3,count)`；不是最后一次点选。constructor `j83392` 的 `_UseObjStore=true`，全直接字段写入扫描只有该 LevelManager 写入；源 GameToolData 描述为移到暂存栏。 | 当前槽前三件移到三列暂存，不重新投入锅，不删除关卡剩余物品。暂存可重新点选进入槽。 |
| 凑齐（ToolAutoFind，1） | `CanUseTool j13232` 槽空禁止；六格时必须已有相邻对子；`UseTool j38482` 普通分支去重当前槽类型，整数 `Random.Range(0,类型数)` 选择一个类型，倒序搜索 `_boxObjList` 补够三件。六格分支选择首个相邻对子，只补一件、不抽随机数。 | 修掉原项目“总挑数量最多类型”和“空槽拿三件”的行为；不做字符串 Contains 模糊匹配。工具 GetUnit 的 `_playSound=0`，避免为自动取物播放用户点选音效。 |
| 打乱（ToolMessUp，2） | `MessObjRotaAnim.MoveNext j65101`：排除暂存、选当前最高位置以下七单位内（严格 `>`）；随机移到末尾 N 次，对原位置按 Y 升序排序。每件速度 `Range(540,720)`，按离轴距离乘 `Lerp(1.2,.15,d/maxD)`，旋转一秒后 .3 秒归位。 | 替换第二关“颠锅”按钮的近似物理冲量。过程禁输入、暂停计时/高度物理优化；保留物品、类型与总数，改变物品到原位置的对应关系。完成后按高度恢复可见与刚体。 |

三类工具 `GameToolData._limitcount=1`。`LevelToolMa.UseTool j8062` 成功后增加使用数，普通分支与配置 offset24 比较，达到上限从允许工具集合移除。`CanUse/MarkUsed/Reset` 对本地第二关保存这一限制；特殊活动、在线热配置覆盖及背包购买来源不在本轮范围。

## 暂存动画与空间

- `SetObjIntoStore j19860`：三列轮流放入，复用同列列表；拿走后由 `SetObjOutStore j38487` 重排该列。
- `PlayStoreAnim j19862`：X/Y/Z 分别 .3 秒，缓动 OutSine / InBack / OutBack（2026-10-16 复核原metadata默认值后纠正之前错误的enum名称）。
- `StoreObj j19259`：保持运动学刚体，重新启用 col 与 realCol 所在对象；不是只开放拾取体。恢复保存的组件 enabled 标志，避免启用原来被禁用的真实碰撞体。
- 缩放 .2 秒 InBack 至 `min(Content.localScale.x=1.4, _pickScale*1.5)`，通过原模型归一化比例换算。
- WX StorePointList 子节点顺序 X 为 -.83 / .01 / .83，根 local y=-.46,z=1.55。Content 相机深度2、旋转X=-90°、scale1.4，因此存储物品基准相机深度2.644。
- 多次存储同列按 `.75*第一二列距离` 后移，除首件外世界X ±.1 交错。
- **UI适配边界**：继续沿用项目当前槽的屏幕位置/间距换算；已经恢复原 StoreContentBG 图、网格和滑入/滑出，尚未重建整个源 Canvas 排版，不声称截图逐像素一致。

## 打乱空间、时间与物理

三个主题的旋转中心均是 `Lv2AirWall` 原点，轴为其 **up**（`j4551 = Transform.get_up`）：早餐/烧烤旋转X=-7°，商店X=-6.5°。不是笼统世界Y转圈。每帧绕轴旋转后，世界Y按 `Lerp(currentY,targetY,deltaTime*2)` 向目标收敛。

原 `ObjectUnit.InitRota j36727` 禁用两碰撞对象；`MoveToTargetPos j36726` 再启用并以 .3秒 DOMove（SetUpdate true）回目标。转圈用缩放时间，最后移动用不缩放时间，等待仍为缩放时间。结束强制尺寸完整并执行可见/刚体检查。`ReplicaLayeredPile.RestoreToolScale` 处理项目导入模型的归一化尺寸。

原版随机算法不是 Fisher–Yates：循环 N 次，每次从 [0,N) 随机挑一个元素，RemoveAt 后 Add 到末尾。已原样保留随机调用次数与固定范围。

## 音效证据

`UseTool j38482` 移出和全部移出救援先调用 `SoundManager.PlayEffectSound(24)`；凑齐为25；另一个冲击道具 ToolShake 为35。打乱分支本体没有独立 PlayEffectSound 调用，本轮不自行绑定冲击声。工具颗粒/VFX组件可能含独立声音，不据此宣称完全无声。音效接入由统一 SourceAudio 实现。

## 仍保留的差异与边界

- 原三工具初始库存来自存档/经济系统；本地体验直接提供每局一次使用，不接广告、金币与分享。
- 原满槽与超时救援窗口、ToolCleanBoxObject（抓取两组三件）和 ToolShake（冲击）不是目前三个普通按钮，不把它们混进打乱。
- 旧项目“冻结时间5秒”不属于这份原版工具配置，第二关不再显示。
- 粒子来自原 iOS 1.11.03 的三个完整 prefab，运行时序来自 WX WAT；尚未拿到 WX 对应粒子包做逐字节资源比对，不将跨平台资源断言为完全相同。
- `CanUseTool j13232` 没有飞槽/三消忙锁；已移除项目额外 `IsBusy` 限制，保留本轮已确认的游戏状态、工具次数和打乱状态门槛。
- 自定义不完整三件关卡额外防护：若不足以补齐，工具不消耗次数、不部分塞槽；这是项目保护，不是原版普通关卡算法。

## 证据与验证

`Evidence/source-tools/` 保留 WAT、GameToolData、三主题轴矩阵、暂存层级、原版参数与摘要。原 iOS 字段完整解码后，在 WX 原始 LevelManager 组件内找到**唯一、连续、逐字节相同的72字节工具参数块**（iOS offset2720 / WX offset1696），覆盖throw缓动、store .3秒、打乱7/1/540/720/1.2/.15；相邻暂存参数也逐字节一致，见 `wx-ios-tool-settings-compare.json`。没有将字符串搜索直接冒充源码。

`SourceToolsValidation.Run` 覆盖：取前三件、类型去重抽签、六格对子禁随机、空槽/不完整三件边界、倒序板上查找、精确类型匹配、N次移尾随机、严格七单位高度带、主题旋转轴。运行集成测试由主任务统一执行，结果写 `Temp/goose-source-tools-validation.txt`；本子任务未单独启动 Unity。


## 原专用视觉资源恢复

资源不是手画替代：`particleprefab_assets_part1` 中 `FX_Tool_ReturnBack` / `FX_Tool_AutoFind` / `FX_Tool_Messup` 分别有 1 / 3 / 6 个 ParticleSystem，保留全部原生模块 JSON、曲线、梯度、burst、原层级位置/旋转/缩放、renderer模式/mesh/排序/顶点流。导出 8 个材质（包括背景）、原 PNG 和 Cylinder001 mesh；粒子自带 autoRandomSeed，不向关卡 Unity.Random 抽数。导入器 `SourceToolVisualImporter.Build()` 通过 `EditorJsonUtility.FromJsonOverwrite` 恢复原生组件并另存序列化回读，禁止以自拟时长/发射率代替。

- **移出光带**：`ToolReturnLight.SetPos j31483` 在物品当前位置激活粒子，scale=1；`<Anim>d__5.MoveNext j65504` 每帧跟随物品位置 `.3s`，随后等待 **2秒 realtime** 才归还对象池/Stop。项目用本地实例销毁代替源对象池，运动/等待不变，Home 立即取消全部跟随与粒子。
- **凑齐闪电**：`CreateAutoFindLighting j19876` 原点为 GetUnit 返回的目标槽位置 + `Vector3.forward*.5`；`ToolFindLighting.SetPos j31485` 乘 prefab `_minTime=.7`，在 `.3*.7=.21s` 内逐帧 LookAt 物品（world up），scale=(.15,.15,距物品*.0209)。注意 `.0209/.7` 是 prefab 序列化覆盖，不是 constructor 的 `.0205/.9`。跟踪结束立即归还/Stop。两个 Lighting profile 同时持续 `.3s`，由 `ReplicaSourceHighlight` 恢复，普通选中取消不抹掉工具高亮。
- **打乱**：`UseTool j38482` 先将原 prefab 挂到 `_screenVFXPoint[2]`，再重置 **local** position0、localEulerAngles0、localScale1，最后 ParticleSystem.Play；WX Odin 引用表证明字典[2] → Transform417 `MessToolPoint`，父为 Main Camera359，local position(0,0,1)、rotation identity、scale .1；项目将同一局部矩阵直接折叠到粒子根并挂相机。六个系统保持原非循环时长及尾粒子，helper 在所有子系统结束后销毁，不自行缩短到旋转1秒。Home/Reset 提前终止。
- **扭曲材质**：`Unlit/NiuQu_UV_Alpha` 从原 Metal 顶点/片元与 Pass 状态机械移植，保留 Noise 滚动、Noise.r*V 的 UV 扭曲、Tex*Color*Mask*particleColor*ColorScale、SrcAlpha/OneMinusSrcAlpha、ZWrite Off/Cull Off。其余直接使用同名 Unity 内建 shader，不以泛用加法材质替换。
- **暂存背景**：WX `StoreContentBG` 原内建网格 fileID10210（Quad，导入时断言ID）、原 `Unlit/Transparent Cutout` 材质与贴图。材质残留 `_Color=.3962264` 不属于该 shader 的有效属性，不错误添加灰色乘色。原 Content BG localZ 从 -3 → 1.44，`.3s` OutQuad；首件暂存显示，末件取走隐藏，Reset直接清除。原 local scale (2.67,1.076612949,.861290395)，原深度6.998与暂存物品深度2.644分别保留；屏幕锚点/间距仍依项目安全区适配。

`SourceToolVisualValidation.Run()` 对照原原生模块与shader，`RunFrames()` 走真实导入关卡模型的移出、再点回槽、凑齐、打乱及Home销毁，生成 `visual-captures/source-tool-*.png`。最终执行结果由主任务统一记录；此子任务不自行启动Unity。


### 完整回读与渲染回归发现的修正

初次直接写入 UnityPy JSON 时，Unity 的 EditorJsonUtility 原生class wrapper与serializedVersion缺失会静默保留默认模块。已修为读取当前Unity原生schema、保留每层版本后合并原payload；预置一条临时burst仅为取得burst/countCurve schema，随即被源burst数组完整覆盖。导入器现在逐字段断言，renderer的0/1与bool作等价比较；唯独四个运行包专用lightmap bookkeeping字段不出现在Editor schema，明确排除。全20个模块/renderer JSON回读核对 **18,750 个标量字段，0差异**（不含重新绑定的对象引用），完整曲线、梯度、burst、尺寸/速度/颜色模块均纳入。

真实打乱截图发现先前两处调用识别错误，已据方法映射修复：`j4551` 实为 `Transform.get_up`，不是forward；`j1271/j1725` 是局部位置/欧拉角，不是世界重置。现在物体绕锅的up轴旋转，旋风在相机前的源MessToolPoint上；不再将物体转出画面或把旋风放成世界空间横条。三主题源up分别为早餐/烧烤 `(0,.99254615,-.12186936)`、商店 `(0,.99357186,-.11320321)`，旋转中心均为原点。证据 `mess-rotation-and-screen-anchor-wx.json` 保存方法映射、WX字典entry byte offset/hex、Unity引用与完整锚点祖先链。

增加三主题0.2秒/0.55秒的物品视锥可见数检查和源相机锚点矩阵断言，记录 `source-tool-shuffle-visibility.txt` 及三张独立主题图；不只以“动作完成/数量没丢失”判断视觉正确。


### 最终统一验证结果（tools9）

主任务运行 `/tmp/goose-feedback-tools9.log`，基础工具、真实飞行中使用、源VFX导入/static/runtime均PASS；没有发布。导入后的全20份模块回读再次比对，18,750标量字段无差异。三主题打乱视锥中可见候选数（开始 / 0.2秒 / 0.55秒）：早餐119 / 119 / 119，商店117 / 117 / 117，烧烤124 / 124 / 124。三张截图均已目视：物品保留在容器视野内，原紫白旋风围绕物堆呈环状；此前空锅和横穿屏幕平面条带已消失。凑齐高亮的遮罩修复由独立高亮实现完成，最新图保留物品轮廓与纹理，不再是纯青色遮挡块。

这些是本地Unity渲染/运行回归；不宣称微信真机FPS或与未取得的WX粒子bundle逐字节一致。素材来源、原WAT控制流和UI安全区适配边界如上。

## 2026-10-16 余差复核修正（本轮测试待主任务汇总）

已按真实WX metadata枚举值修正暂存X/Y/Z为OutSine/InBack/OutBack，scale为InBack；不是弹性Elastic。补真实中点曲线验证。Shuffle候选阈值使用只降不升的源maxHeight记录；结束逐候选CheckVisible后按depth1检查刚体，不再额外全盘刷新。ReleaseStored不杀新飞槽tween，Input前置kill与后移release由主任务统一整合。完整审计和明确保留边界见 `Evidence/remaining-audit/source-tools/ledger.md`。另一位置洗牌分支的visibility移植/scale0不误加到默认旋转洗牌。

限定追加复核：原Return/Find控制协程使用j1511=unscaledDeltaTime，已纠正helper并补timeScale0测试；Return尾部仍2秒realtime，Find持续.21秒；背景补间与原粒子系统仍scaled，未一律改为unscaled。
