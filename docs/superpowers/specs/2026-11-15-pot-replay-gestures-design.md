# Pot replay UI and operation gestures

User approved restoring the tray/tool visuals and showing an animated hand for recorded operations, using crazyCombo's actual replay icon and its 0.92 tap scale.

Keep the gameplay HUD header and footer visible in replay. Reuse the original pause/settings entry and timer; disable footer tool interaction without changing recorded outcomes or current account inventory. Hide quantity badges because old recordings do not contain historical inventory. Show tray in game and replay states and lay it out after replay camera application.

Build a presentation-only gesture timeline from existing events: manual `pick` (value 0), `tool` (tool index), and `probe_use` (column). Skip magnet picks, probe-generated picks, automatic exposure and reward generation to avoid duplicate/false clicks. Probe preview presses may indicate a long hold; derive the column from the pre-event presentation when available and pair release events without duplicating a subsequent use. Fall back to the use event if old data is insufficient.

Resolve manual picks from the last recorded pre-pick pose and recorded camera. Resolve UI targets from live tool/probe RectTransforms. Draw a non-interactive overlay hand using the existing crazyCombo replay sprite, respecting its fingertip anchor. Move to each target over up to 0.14 replay seconds, then scale to 0.92 over 0.06 and back over 0.08 (quad-out), matching the source. Use absolute replay time, not independent tweens, so pause, speed, backward seek and jumps are deterministic. Do not alter the recorded timeline. Per user correction, keep the hand visible at its last target during idle gaps and after the last operation. Keep the source orientation without rotation or mirroring. Add the source circle below the hand, with the original looping 1 → 1.1 → 1 pulse. Hide only outside replay or when no operation exists.

Validation: real Unity compilation and play-mode checks of tray/footer visibility, disabled interaction, manual/magnet/probe filtering, tap scale and seek determinism, screenshot review. Cover old event format without backend or schema changes. Commit/push and use the existing WeChat publishing workflow after validation, preserving unrelated work.

## Approved replay controls revision

The original upper-left pause button opens the existing settings modal and suspends the replay clock. Closing settings resumes the previous playback state; live simulation remains frozen (`Time.timeScale = 0`). The settings exit entry goes through `PotReplayPlayer.Exit` to clean up visuals and restore normal game time. There are no separate play/pause or exit buttons on the replay screen.

Reuse the existing camera zoom artwork and layout for replay speed: minus and plus change speed by 0.5, bounded to 0.5–3; the centre pill only displays speed. In normal gameplay these controls retain camera zoom behavior. Show recorded frame remaining time in the original timer, including add-time jumps and backward seeking.

Export the visible progress track, fill and thumb from the artist's `一锅消-回放01.psd` group `组 3`. Position the progress control below the multiplier inside the safe-area header. Pressing or dragging the progress temporarily suspends playback; releasing restores playback, including seeking backward from the completed final frame. Settings blocks speed and progress interaction. Keep the original hand and halo behavior.
