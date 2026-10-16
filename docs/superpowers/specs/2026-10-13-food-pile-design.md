# Food pile challenge

User-approved scope: use the existing Unity project, import eight food models from the downloaded goose-catch project, and build a 96-item (12 of each) physics matching prototype. This is an experimental level, not a claim about the original game's daily challenge configuration.

Keep GameScene and its authored levels. Add FoodPileChallenge scene, a prefab-based settings asset, and an optional LevelGenerator path using that asset. Reuse the existing seven-slot matching UI and boosters. Convert the eight uncompressed GLBs offline into Unity mesh/material/prefab assets, retaining original UVs and textures. Models should be normalized and colliders fitted; model names remain stable match IDs for compatibility with the existing boosters.

Create a visible tray with containment colliders and staggered, non-overlapping spawn layers. Add a shake button and Space shortcut, bounded impulses and a cooldown. Keep collected objects outside physics. Fix camera framing for both portrait and wide editor previews. Prevent input through UI, overfilling during animations, stale tweens after restart, and overlapping match resolution. Preserve original authored levels.

Validation: compile/import in Unity 2021.3; inspect converted assets, eight types and twelve copies each; run physics settling and containment; verify selection, triple removal, undo, seven-slot failure, restart and completion; visually inspect scene with all seven slots visible. Record limitations and model provenance. No commercial asset-license claim is made.

## Requested correction
Replace the flat tray with an open circular cooking pot, inward sloping collision surface, rim and two handles. Concentrate the 96 spawns into a narrower vertical pile. Bind each pointer gesture to the rigidbody hit on press, disable all collection physics before lifting, and consume release once. Verify a selected object reaches its slot while unrelated transforms remain unchanged with simulation paused, then run normal-physics playthrough.
