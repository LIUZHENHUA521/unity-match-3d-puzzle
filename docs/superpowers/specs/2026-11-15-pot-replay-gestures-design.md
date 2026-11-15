# Pot replay UI and operation gestures

User approved restoring the tray/tool visuals and showing an animated hand for recorded operations, using crazyCombo's actual replay icon and its 0.92 tap scale.

Keep gameplay HUD footer visible in replay, hide the normal header, and disable all HUD interaction without changing recorded outcomes or current account inventory. Hide quantity badges because old recordings do not contain historical inventory. Show tray in game and replay states and lay it out after replay camera application.

Build a presentation-only gesture timeline from existing events: manual `pick` (value 0), `tool` (tool index), and `probe_use` (column). Skip magnet picks, probe-generated picks, automatic exposure and reward generation to avoid duplicate/false clicks. Probe preview presses may indicate a long hold; derive the column from the pre-event presentation when available and pair release events without duplicating a subsequent use. Fall back to the use event if old data is insufficient.

Resolve manual picks from the last recorded pre-pick pose and recorded camera. Resolve UI targets from live tool/probe RectTransforms. Draw a non-interactive overlay hand using the existing crazyCombo replay sprite, respecting its fingertip anchor. Move to each target over up to 0.14 replay seconds, then scale to 0.92 over 0.06 and back over 0.08 (quad-out), matching the source. Use absolute replay time, not independent tweens, so pause, speed, backward seek and jumps are deterministic. Do not alter the recorded timeline. Hide during idle gaps and outside replay.

Validation: real Unity compilation and play-mode checks of tray/footer visibility, disabled interaction, manual/magnet/probe filtering, tap scale and seek determinism, screenshot review. Cover old event format without backend or schema changes. Commit/push and use the existing WeChat publishing workflow after validation, preserving unrelated work.
