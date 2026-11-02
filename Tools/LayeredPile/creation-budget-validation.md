# Creation frame-budget validation — 2026-10-28

## goose-creation-budget-validation

```text
PASS forced budget: one item per frame, pause at null yield, cancellation/restart, 300 distinct items and readiness; all prefab caches clone/reset with equivalent collider profiles; growing queue drains and falling full-size bodies still park. Editor only.
```

## goose-source-creation-validation

```text
PASS three themes: production iterator; bounded slices within batches 1/3/.../2; ascending Y; initial kinematic rigid; +.1/.02 creation growth; depth-based freezing; waits .02 then .5/.2/.3; FinishCreate before play.
```

## goose-source-creation-frames-validation

```text
PASS live creation coroutine: first batch only; progress across frames; scaled-time pause; home cancels pending work; all three themes progressively reveal/grow before creation completes; restart creates 300 distinct items and enables play only after creation. This is editor timing, not device frame equivalence.
Theme 0: first visible 3.398s at 169 objects; finished 7.440s; progressive growth observed. Editor only.
Theme 1: first visible 3.873s at 166 objects; finished 8.626s; progressive growth observed. Editor only.
Theme 2: first visible 4.030s at 166 objects; finished 8.663s; progressive growth observed. Editor only.
```

## Additional checks

- 66 source-level prefabs baked; physical materials persisted as assets.
- Highlight validation passes for 23 countryside models, GPU-only mesh and original goose.
- WeChat IL2CPP/WebGL export and package validation pass. Local output: `Builds/Wechat/minigame`. No new upload.
- Spec reviewed and approved; implementation additionally separates original/adapter collider profiles for shared model names.

## Interpretation

The 2.5 ms budget stops additional creation work after the current indivisible item. It cannot guarantee a whole-frame bound or interrupt Instantiate, GPU upload or physics. Frame-observer tests scan all objects and are not a performance benchmark. Editor observations above are not iPhone FPS or comparable device loading measurements. Previously uploaded 0.1.17 does not include this change.
