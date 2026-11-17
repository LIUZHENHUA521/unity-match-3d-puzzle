# 一锅消不完与《抓大鹅》来源风险审查

审查日期：2026-11-17（北京时间）。对象为当前 master 工作区，包含未提交变更。未修改游戏、未构建发布、未验证线上版本。以下为技术证据与风险排查，不是法院侵权认定；是否具有授权、权利归属、可保护表达及实质性相似仍需结合合同和专业法律审查。

## 结论

现状不能说“全部素材仍照搬”，也不能说“已经全部洗清”。旧模型、旧 UI、旧音效已大量清理；最明确的当前残留是机械移植的 Highlight Shader，其次是依据原版 WASM 重建的若干行为模块。新素材的主要问题转为授权链核验，另发现 Carefree 署名记录与当前实现不一致。

## 1. 高优先级：三份机械移植 Shader

- `Assets/Match3D/Resources/Match3D/Highlight/Outline.shader:1`
- `Assets/Match3D/Resources/Match3D/Highlight/Glow.shader:1`
- `Assets/Match3D/Resources/Match3D/Highlight/Overlay.shader:1`

文件头明确记载从保存的 WX GLES 程序机械移植，代码保留 `u_xlat*`。`Tools/LayeredPile/Evidence/highlight-complete/port_highlight_geometry.py:20` 及 `:54` 留存从 HighlightPlus GLES 到 HLSL 的转换过程；`Tools/LayeredPile/highlight-complete-alignment.md:34`、`:43` 也记录该来源。

当前引用链：`Match3DSelectionFeedback.cs:19` → `ItemHighlight.cs:39`、`:73`、`:87`。WebGL 虽禁用了几何描边路径，但 `ItemHighlight.cs:116` 仍绘制 Overlay，不能视为仅历史研究文件。

判断：这是具体程序表达转换的强证据，不只是效果相似。原始标识为 HighlightPlus，权利可能属于第三方插件作者，不应直接全部归于《抓大鹅》厂商。当前检索未见购买凭证，不等于用户绝对没有授权。优先核实自己的合法插件许可，或替换为有清晰来源的独立实现；改名、改色、删除注释不解决来源问题。

## 2. 中高优先级：依据原版 WASM 重建的生产逻辑

| 当前文件 | 证据 | 判断边界 |
|---|---|---|
| `ChallengePlan.cs:26`、`:171` | WX CreateLevel、KFakeRandom 方法映射及分配细节 | 第一关/本地路径仍涉及，不能断言所有每日关卡都走此逻辑 |
| `LevelLayoutConfig.cs:49` | 第一关先消耗随机数再生成与追加目标物 | 属于行为重建证据，单个算法步骤未必受著作权保护 |
| `LayeredItemPile.cs:54`、`:143` | LoopCreateObj、ObjectUnit 方法依据 | 需审查具体结构、非必要细节及表达相似 |
| `BoardTools.cs:8`、`:18`、`:67` | 原版道具方法、选物与时间参数 | 三件移出等抽象规则不能单独认定侵权 |
| `ItemFlightMotion.cs:5` | WX 默认动画时间、曲线及合并时序 | 单个时间参数与整套表达应分开分析 |
| `Assets/Scripts/Game/InputController.cs:107`、`:200` | 触摸/拾取原版方法对应 | 需要进一步做表达层面的比较 |

除特别写明外，上表脚本位于 `Assets/Match3D/Scripts/`。`docs/refactoring/2026-11-04-naming.md:3`、`:15`、`:23` 已明确：命名整理不改变实现来源，商业包是部分数据与行为重建依据。

重要限制：算法、操作方法、数学概念不因被逆向观察就自动构成软件著作权侵权。本次没有完成原 WASM 全程序与 C# 的语义差异鉴定，不能称“全部代码抄袭”或给出总体相似百分比。但现有记录不支持“完全未经参考独立开发”的描述。

当前分流：`Match3DFlow.cs:325` 起，shared-v2 每日局恢复开局快照；`:331` 第一关/本地仍涉及旧生成路径；daily-v1 第二关使用 `Match3DDailyLevel.cs:36` 自有生成。SourceLevelConfig 仍标记 WX 2.1.12 来源，但已更换模型与权重，不能拿旧文档当当前完整配置。

建议：按模块列出保留的功能需求，剥离原程序表达，用独立设计实现；为生成预算、工具交互、动画和关卡节奏形成自己的设计依据。不是随意改几个常量便能获得法律保证。

## 3. 美术现状：旧资源已清理，新资源仍需凭证

当前 `Resources/GooseArt`、`OriginalModels`、`SourceScenes` 已不存在。旧清理记录中的 821 张图片、7 个音频以及另一轮 27 个旧材质/鹅叫/道具音频，不能继续列为当前存在资源。构建脚本也对部分旧目录回归设有拦截。

当前主题与锅已有新的来源记录；但“来自用户 PSD”只说明导入来源，并不等于取得完整商用许可。例如：

- `Assets/Match3D/Resources/Match3D/HomeSkinPS/README.md:3`：主界面 PSD。
- `Assets/Match3D/Resources/Match3D/GameplaySkin/README.md:3`：游戏界面 PSD。
- `Assets/Match3D/Kitchen/Art/Grab3D/README.md:3`：Grab3D 平底锅，记录 CC0。
- `Tools/validation/PotComboPraise/README.md:5`：连消 Spine 来自 crazyCombo。

实际 `ModelThemes.asset` 为 8 套主题，每套 26 个 prefab，共 208 个配置引用，不能沿用旧文档“每套 23 个”。TastyKitchen/FoodieParty 的 provenance 记录原 23 个模型分别为 Kenney Food Kit 和 Quaternius/poly.pizza 的 CC0 资源；它们不能自动证明新增条目的来源。五套食材包在 `docs/validation/pot-food-themes/README.md:7` 记录为用户提供购买包，但本轮未找到订单及许可文本；玩具初始模型来自原仓库 Assets/Models，也未找到覆盖它们的明确许可。上述项目应补证，不据此认定来自抓大鹅。新增五背景在 `SourceAssets/PotThemeBackgrounds/README.md:3` 有生成来源记录。

本次未取得证据证明这些 PSD 是抓大鹅素材，故不认定其抄袭；需补原创作者/外包合同/购买订单/素材许可，以及其中字体、装饰素材的权利链。自己另一个游戏里的资源，也应查最初许可是否涵盖两个游戏。

仓库还有旧原型授权缺口：`Assets/FoodPile/MODEL_SOURCES.md:3`、`:9` 明确 8 个模型来自第三方 goose-catch，未建立素材许可证。它不是商业《抓大鹅》本身的代码来源证明。主场景仍留有旧设置引用，但 `WechatReleaseAssets.cs:57` 起正式发布准备会清掉旧依赖；正常主玩法也不走旧生成。将其列为仓库/其他构建路径风险，不断言当前微信包正在使用。

## 4. 音频：原版音效风险已下降，另有许可落实缺口

- 当前拾取 `DM-CGS-32.wav` 为 Dustyroom CC0；三消 `unfa_cartoon_pop.wav` 为 unfa CC0，当前 `GameAudioCatalog.cs:13`、`:34` 可确认绑定。
- `PotRewardReceived.mp3` 与 crazyCombo 的 `LevelUp.mp3` SHA-256 完全一致：`8358e2a679e914de0a81623691f89d9570fcd86edbe2bc73e18ee718d386c382`。这证明复用来源，不证明其最初许可。
- 当前 `S_ButtonClick.wav` 与清理前 GooseArt 同名文件相同，但 `docs/click-audio.md:34` 记录该旧路径早已被替换为 CC0 Zippo click；因此不能因路径历史将其当原版音效残留。
- 探物发射记录为 Kenney CC0；印章音效来源页面标记 CC0；胜利音效来自 crazyCombo，仍需保留原始授权。

**明确待修：Carefree 的署名记录与实际实现不一致。** `Assets/Match3D/Audio/Licensed/README.md:11` 声称设置/暂停页面通过 AddMusicCredit 展示署名，但当前 Assets 的 C#、Prefab、Asset、JSON 搜索没有找到该方法或 Kevin/许可文本，ThemeLibrary 仍绑定 Carefree，Match3DFlow 仍播放。按仓库采用的 CC BY 4.0 应在合理位置提供作者、许可链接等；若已有另购免署名许可，应提供凭证。这是第三方音乐合规风险，不特指抓大鹅。本次未运行 UI，结论是静态证据下署名疑似缺失，需实际页面确认。

## 5. 玩法及整体表现

七格、三消、堆叠、摇锅等抽象规则相似，不足以单独判断著作权侵权。但如果对具体规则体系、数值、节奏、界面组织整体进行全面仿制，即使换了素材，仍可能涉及不正当竞争。当前存在独立 UI、主题、每日快照等变化，需综合评估，不能仅凭“换皮”标签定性。

内部 goose/Source 标识本身不是侵权证明。对外名称、图标、宣传是否引人误认为与抓大鹅有关另行审查；本次未检查平台商店页、广告投放、商标注册和全部线上服务。

## 范围与复核方法

用户指定 `goose-wechat-extracted` 目录共 29 个文件，主要是 JS/WASM 和 17 张 PNG，**不是完整模型、贴图、音频素材库**。对当前 Assets 2514 个非 meta 文件及 SourceAssets/Tools 做 SHA-256 比较，除空文件伪命中外无整文件相同项；对当前 Assets 330 张可解码图片与该目录 17 张 PNG 做尺寸+RGBA 全像素哈希比较也无相同项。这只是有限样本的完全一致性检查，不能排除裁切、重画、转码或其他包内资产来源。

另对清理提交 08d2653e 的父版本中四个旧来源目录的 966 个唯一媒体 Git blob 与当前媒体做哈希比对，仅命中上述已被 CC0 替换的按钮音频；该比较不覆盖近似/衍生素材，也不代表已对旧版本发布责任作出判断。

REA 已对用户指定目录进行静态 JS 分析，证据 ID 和限制见 `rea-summary.json`。game.js/subContext.js 的语义阶段受堆内存限制，图覆盖不完整；没有执行原游戏或全面反编译 WASM。主要结论依靠当前文件、运行调用链、保存的提取/转换证据和来源记录交叉核验。没有把未发现当作不存在。

本次未重新构建、未检查最新线上包。代码及素材变更由其他会话持续进行，结论对应本轮读取时状态。旧上传包和历史发行的风险不因当前删除自动消失，应另核对发行范围；保留来源与整改记录，不以删除证据方式整改。

## 法律与许可依据（2026-11-17 查阅）

- [著作权法：作品类别与复制、改编等权利](https://www.ncac.gov.cn/xxfb/flfg/flfg_532/202103/t20210309_50530.html)。
- [计算机软件保护条例：第六条不保护思想、处理过程、操作方法、数学概念](https://www.cac.gov.cn/2013-02/08/c_12648744.htm)。
- [最高法转载人民法院报：游戏玩法与换皮的不正当竞争边界](https://www.court.gov.cn/zixun/xiangqing/463511.html)。个案说明路径，不等于本项目必然构成侵权。
- [2025 修订反不正当竞争法：第二条、第七条](https://www.samr.gov.cn/zw/zfxxgk/fdzdgknr/fgs/art/2023/art_3737890d856a4e44a8ea07c50c90c116.html)。
- [CC BY 4.0 署名要求](https://creativecommons.org/licenses/by/4.0/)、[Incompetech 许可说明](https://incompetech.com/music/royalty-free/licenses/)。
- [Dustyroom CC0](https://dustyroom.com/free-casual-game-sounds/)、[unfa Cartoon Pop CC0](https://freesound.org/people/unfa/sounds/245645/)、[印章音效 CC0](https://sfxmint.com/sounds/checkmark-10)。

处理优先序：①高亮 Shader 合法替换/许可核验；②恢复音乐署名或核实免署名许可；③PSD/模型/跨项目资源建立授权台账；④对原版重建模块逐项做独立表达与整体设计审查；⑤对最终发布包核对整改覆盖。以上均为建议，本轮没有实施替换或发布。
