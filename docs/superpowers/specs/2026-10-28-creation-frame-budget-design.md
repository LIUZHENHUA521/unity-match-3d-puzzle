# Creation hitch mitigation

The user approved reducing the hitch while the 300 items progressively appear.

## Implementation

- Keep the sorted spawn order, three-item maximum batch, first one-item batch, existing scaled waits and final readiness gates. Split a batch early after an item takes the creation slice past 2.5 ms. This is cooperative scheduling: it cannot interrupt Instantiate or bound GPU/physics time. A split yields a rendered frame and respects pause/cancel/restart.
- Bake source collider profiles and reference/state caches into the level prefabs in the editor. Preserve geometry, layers, material properties, pivot normalization and enabled/trigger flags. Persist generated physical materials as assets. Bake again before WeChat export to cover regenerated assets. Baked prefabs clone directly without a redundant runtime template; legacy unbaked prefabs retain fallback preparation.
- Original and adapter models can share a name but have different hierarchy paths. Select adapter collider profiles only for models carrying ReplicaSourceIdentity; retain original profiles for original models. The bake checks that every target resolves its pick group.
- Grow only the unfinished creation entries. Once full-sized, check height physics and keep only dynamic entries in the creation physics queue; parked entries cannot move during the input-locked creation phase. Continue checking falling dynamic entries until parked and perform the existing full finish/visibility refresh. Remove entries from both queues on detach/clear. Do not remove deep support colliders.

## Validation

Use the production creation iterator and live frame tests for three themes: 300 distinct items, progressive visibility/growth, ordering, final readiness, pause (including a budget split), cancellation and restart. Force a zero budget in editor-only validation to exercise one-item slices deterministically. Compare baked collider data against fresh profile application; test clone references, recycle flags and reuse. Verify full-sized dynamic bodies still freeze below the height threshold. Record editor timings as editor-only observations, not iPhone results.

## Limits

No guarantee of eliminating first GPU uploads or shader compilation. No model count, textures, geometry, or physics parameters are reduced. This request changes code; the previous 0.1.17 upload does not include it.

## Review and implementation plan

Spec review approved the initial design. The existing user approval covers implementation; no new approval is needed. Implement collider serialization/bake and pool fast path, then budget scheduling and growth queues, then validate clone/recycle equivalence, forced budget pause/cancellation and live creation across all themes. Build a local WeChat package after functional checks. Do not claim device performance gains from editor tests.
