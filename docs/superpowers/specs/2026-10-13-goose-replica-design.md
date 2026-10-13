# 3D daily challenge implementation design

User approved the feature scope and requested implementation on 2026-10-13. No red-envelope/payment feature in this scope.

## Scope and acceptance
- A new launch scene with Chinese home UI, daily normal challenge, selectable theme challenges, collection and settings. Reuse extracted Goose graphics/audio as explicitly requested; resource references isolated for later reskin. Reconstruct screens from available assets; do not claim recovered original application logic or perfect layout without visual comparison.
- Keep existing FoodPileChallenge and GameScene playable and preserve all pre-existing uncommitted edits.
- Normal and theme runs each have a short tutorial stage followed by a layered challenge. Stable daily identity uses Asia/Shanghai calendar date, mode/theme and config version. Replay uses the same initial generation configuration. Physics simulation is not assumed deterministic.
- Layer generation allocates complete triples with exact count validation and distinct type limits, weighted selection, seeded shuffle, and named object identity. Use project's existing food prefabs plus available original models where validated. Do not silently claim 22 types while using eight.
- A successful hard stage unlocks the corresponding goose once; restart and repeated completion cannot duplicate collection rewards. Collection/settings persist locally for this prototype, reset only by explicit developer action.
- Settings: independent music, effects, vibration. Pause menu blocks game input/timer; returning home cancels countdowns and pending generation.
- A separate service under crazyCombo/goose-server provides quick match and private rooms, ready/start, server-owned deck/seed, validated tile-pick and undo, progress, terminal outcome, leave/disconnect, heartbeat and room cleanup. No existing CloudBase collections or live service changes. Service is prepared for a single instance, not advertised as horizontally scalable.
- Unity native ClientWebSocket client connects to configurable endpoint, queues callbacks to Unity main thread, reports network failures and never substitutes an unlabelled bot. Server owns terminal result; no client-declared win.
- UI uses imported sprite/audio inventory and a replaceable theme manifest. Match/lobby/result actions have actual handlers. Settings and collection need functional back navigation.

## Boundaries
Original binary remains encrypted. Exact original solver, random sequence, all live operations/events, and original PVP rules are not recovered. Implement a documented race-to-clear duel. No rewards service, advertisements, production authentication or deployment without further instruction. On-device native/WebGL build depends on target platform; editor test is mandatory.

## Validation
Compile through installed Unity editor; deterministic generator tests for triples, layer counts, seed stability and invalid configs; play-mode smoke for home, modes, first-to-second stage, settings, collection and return; preserve legacy 96-item tests. Run two real WebSocket clients against isolated local server to test ready/start, invalid/duplicate picks, progression, victory, disconnect, timeout and duplicate queue safety. Check visible UI at portrait resolution and capture screenshots. Record verified coverage and remaining deployment/platform limitations.
