# Pot replay gestures validation

Run `python3 Tools/validation/PotReplayGestures/run.py --replay /absolute/path/to/recording.json.gz` with an existing v2 gzip replay. The runner copies the project into `work/pot-replay-gestures/project`, compiles it with Unity, loads the actual recording, and checks replay UI, non-interaction, filtered operation events, tap scale, pause, speed, seek and cleanup. Screenshots and checks are under the staged project's `work/pot-replay-gestures/` directory. Player recordings are not committed.

The probe screenshot uses a synthetic gesture at a real recorded probe column when the supplied recording contains no probe clicks; the timeline checks separately cover cancelled previews and successful probe-use deduplication.

The hand asset is copied without visual changes from crazyCombo:
- `client/assets/scenes/game/main.scene`: `movefinger/Node`, sprite UUID `74bce41a-9345-475d-a546-6ac182af1262@e5aa6`.
- `client/assets/resources/newSpriteAsset/coreicons.plist`: `Move Hand`, unrotated rectangle `(1516, 1, 286, 304)` in `coreicons.png`.
- Scene size `(143,152)`, local offset `(34.941,-45.246)` and scale `0.8` are retained.
- `LayoutRoot.ts`: 0.14 s move, 0.06 s shrink to 0.92, 0.08 s restore, quad-out easing.

The replay hand is driven by absolute recording time. Tool gestures point downward and right-edge item gestures mirror horizontally so the palm remains visible. No new recording protocol or server support is required.
