# Pot replay controls validation

Implemented the approved replay controls design with the original gameplay pause/settings modal, timer, zoom artwork repurposed for playback speed, and the artist's progress bar. Standalone playback/exit buttons and the pale header panel are removed.

Art source: the user's open Photoshop document `一锅消-回放01.psd`, group `组 3`. Exported individual visible layers from temporary document copies; source unchanged:

| Original layer | Runtime asset | Trimmed pixels |
| --- | --- | --- |
| 矩形 1 拷贝 | PotReplayHandle.png | 34 × 81 |
| img_jindu_2 拷贝 | PotReplayFill.png | 478 × 26 |
| 矩形 1 拷贝 3 | PotReplayTrack.png | 640 × 36 |

The fill stretches to the 630-pixel inner track; the thumb preserves the exported height and shadow. All three assets live in Resources/Match3D/PotReplay.

Validation: `Tools/validation/PotReplayGestures/run.py --replay <existing gzip v2 recording>` passed 44 assertions in an isolated Unity project. The fixture is the previously uploaded recording with canonical ID `c5c100122c7e5153119b2112bf8fc3f000e1702f6a8bce0a60afbc550f115714`.

Coverage includes original hand/halo and tray/tool behavior; visible header and original pause entry; settings freezing playback and gestures, blocking underlying controls, and resuming with physics still frozen; speed steps and boundaries; progress press/release; end-of-recording backward seek; recorded remaining time and a synthetic add-time jump; correct progress geometry; settings exit cleanup and restored time scale. The test harness propagates nested coroutine failures.

Reviewed the generated 720 × 1565 screenshots. Outputs are under `work/pot-replay-gestures/project/work/pot-replay-gestures/`, including `pot-replay-controls.png` and `checks.txt`. Verification is Unity play mode; no physical-device test is claimed.

## Persistent download tip

The replay list now shows the existing ShowTips visual with “下载回放中” until download completion. A dedicated tip instance avoids replacing ordinary transient notices. Duplicate watch clicks are ignored while downloading. Success dismisses the tip and loads the replay; failure dismisses it and reports the error. Leaving the page or changing lists invalidates the pending callback and hides the tip, preventing an old response from entering playback. The existing in-flight request may finish in the background.

The isolated Unity validation passed 47 checks, including persistent visibility beyond 1.5 seconds with time paused, explicit dismissal, and normal timed-tip expiry after persistent use. The staging sync now removes stale copied source files so deleted legacy validators cannot break subsequent runs.
