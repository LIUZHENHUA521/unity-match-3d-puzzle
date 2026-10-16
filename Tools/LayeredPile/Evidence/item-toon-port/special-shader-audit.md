# Special item shader evidence (2026-10-16)

Extraction `work/extract_special_item_metal.py`, port `work/port_special_item_shaders.py`.
The matching iOS `2021.3.14f1` model bundles contain original Metal source in the compiled blob. The parsed pass's `progVertex/progFragment.m_PlayerSubPrograms[0].m_BlobIndex` identifies the correct program; `m_KeywordIndices` indexes the parsed form's `m_KeywordNames`. Parameter blobs also occupy the program table and must not be treated as shader programs. All selected MSL and matching pass states, bundle/path IDs are saved in `metal/`.

| Reconstructed shader | Source variant | Source passes |
|---|---|---|
| ToonGlass | DIRECTIONAL | ForwardBase, ForwardAdd; Cull Back; ZWrite Off; RGB only; SrcAlpha/OneMinusSrcAlpha base, SrcAlpha/One add; Transparent+1 |
| ToonGlassSingle | _ISSINGLECOLOR_ON + DIRECTIONAL | same |
| ToonGlassWorld | _ISUSEWORLDMATCAP_ON + DIRECTIONAL | same |
| ToonGlassBack | DIRECTIONAL | ForwardBase, ForwardAdd; Cull Front; ZWrite Off; RGB only; same blends; Transparent-1 |
| ToonEffectFull | DIRECTIONAL | source outline (Cull Front), base, add, ShadowCaster; opaque |

All special fragments have their original base tint, AO, normal alpha/green unpack, cell threshold, inverse light (EffectFull), shadow add color, high light, MatCap, roughness, rim and glass alpha calculations preserved. Vertex UV/TBN calculations are mechanically translated too. Transparent sources have no ShadowCaster pass; no fake caster is added.

EffectFull's source outline expands vertices by `GlobalOutlineWidth * _OutlineW` and outputs `_OutlineColor` (alpha 0). Its source global width is retained, not replaced with guessed numbers. EffectFull receives native directional attenuation in the original cell-term position (Metal SHADOWS_SCREEN fragment confirms it). The Unity native ShadowCaster pass is reused for the opaque shader.

The normal packed texture sample is `.yw` then `.yx`, i.e. alpha/green for x/y. Preserve original texture RGBA and do not run Unity's normal import repacking.

None of the selected special fragments reads ambient spherical harmonics or Unity ambient colors. They use their own `_IndirectColor`, plus source directional-light presence/direction. Therefore restoring the scene's high-valued ambient does not introduce Lambert washout for these migrated materials.

Project compatibility addition: `_SelectionGlow`, `_SelectionPulse`, `_OutlinePixels`, existing black selection-outline pass, and the existing weak rim glow. This is marked separately and does not change unselected output.

Scope: ForwardAdd's direction-light variant is recovered. Additional point/spot/cookie lighting variants are not ported because the evidenced level-2 rig uses one directional light. Exact real-device pixel equivalence has not been demonstrated; Metal-to-Cg compilation may vary precision. The Unity shader compiler and visual tests are performed by the parent integration task.

## Additional goose variants (2026-10-16 audit)

The newly included goose models use `ToonEffectFullSingle` and `ToonEffectFullWorld`. Both now have their exact source outline/base/add passes ported by the same mechanical generator, with native shadow attenuation and the existing separately marked selection overlay. `GreenPepperMa` uses source shader `5f13ea99433469a9`; `ColaTexMa` and `InstantNoodlesTexMa` use `a8f2be5a89a42a84`. The six Single vertex/fragment programs in both source bundles are byte-identical (verified SHA-256), so one Single shader is valid for both. The World base/add fragment programs are distinct and are not replaced with the plain Full variant.

`ios-exact-programs/index.json` was appended, retaining every earlier entry (56 source shader records after append). `special-programs/goose-variant-manifest.json` records all 18 exact source-to-selected-program references, comprising 12 unique vertex/fragment programs. The ToonLite generator was also rerun across all 102 ToonLite materials: all eight named variants have byte-identical matching vertex and fragment programs for both base/add passes across their source bundles; `toon-lite-cross-bundle-audit.json` records all four hashes per material. These checks found no new unsupported ToonLite keyword combination.

These shaders do not implement goose skeletal or clip animation: imported static/skinned geometry, runtime animation components and source clip playback are separate audit scopes. Shader compilation and all-item render verification remain the parent integration task's responsibility; no Unity instance or release was started here.
