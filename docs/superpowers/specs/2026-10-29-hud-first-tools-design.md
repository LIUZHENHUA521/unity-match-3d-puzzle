# Gameplay header and first-stage tools

User requests lower top controls to avoid iPhone Dynamic Island and working tools in stage one. Move GameHUD's authored header from +40 to -16 design units (720-wide layout); combine Unity safe area with cached WeChat GetWindowInfo safe top/status bar relative to screenTop and windowHeight, using the larger inset, not summing. SDK reads occur at creation/resolution changes. Preserve footer behavior, button size, and gameplay viewport. Unity fallback handles unavailable SDK/safeArea.

Enable the existing source tools in both solo stages. Align visibility, labels, live interactability, badges and inventory callback stage checks; retain free-per-round counts and authoritative inventory flow. Capture current board version for asynchronous paid action so a replaced board cannot receive it. No server changes. PVP remains its existing path.

Validate portrait full screen/notch/island/DPR safe-area math and actual header positions, real first-stage HUD clicks for remove/auto-find/shuffle, counts/reset at second stage, and source first-stage completion. Build local WeChat package; no upload requested.
