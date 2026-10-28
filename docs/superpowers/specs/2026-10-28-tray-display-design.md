# Per-model tray display

User approved an editor window with per-model camera-relative Euler rotation, scale multiplier, slot-relative offset, live seven-slot preview, save/reset, runtime flight/reflow integration and countryside presets.

- A separate Resources ScriptableObject stores settings. Adapter identities use their stable logical ID (survives adapter prefab regeneration); originals use prefab references, so identical names do not collide. Runtime binding happens during spawn, before the instance is renamed to type_N. Unconfigured objects use existing behavior.
- Runtime source flight uses the configured rotation, offset (X right, Y up, Z toward viewer, in slot spacings) and scale multiplier. Reflow uses the same target; merge timing stays unchanged. A per-instance binding remembers board scale/rotation before pickup and resets when a pooled object is rebound. Stored/reselected items must not compound scaling; undo restores board pose.
- The editor window lists source-level prefabs, previews a copy with the actual tray prefab, source orthographic camera orientation, slot spacing and common runtime target math. Preview rendering must not change scene objects, physics, assets or global lighting permanently. Save is explicit, dirty edits protected, reset removes that model's override. No auto-fit during ordinary preview; it would conceal clipping/oversize.
- Inspect 23 countryside models and save initial readable poses. Validate settings roundtrip/reset, adapter ID preservation and duplicate names, configured/unconfigured pick/reflow, store/reselect, undo and pool reuse, cancellation/merge. Verify camera-relative target and displayed preview agree. Build locally after checks; no upload requested.

Plan: data/binding and flight integration; editor preview/window; contact sheets and curated countryside presets; runtime and persistence validation; local WeChat build.
