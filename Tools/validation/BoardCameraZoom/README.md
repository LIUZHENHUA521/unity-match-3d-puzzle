# Camera zoom validation

Run `python3 Tools/validation/BoardCameraZoom/run.py` on the project's Mac with Unity 2022.3.62f3c1 installed. The runner copies the project into `work/board-camera-zoom/project` and compiles the real game scripts, leaving the open editor alone. No gameplay startup, login or network services run.

The editor fixture exercises real camera projection, mesh picking, foreground transforms, the touch recognizer and HUD callbacks. Its synthetic contacts enter the same `ProcessContacts` method as the live `Input.GetTouch` path. Output: `work/board-camera-zoom/result.txt`, `unity.log`, `controls.png` (empty-pot HUD fixture).

Real WeChat/iOS/Android touch delivery and perceived pinch sensitivity still require device testing. The fixture does not substitute for it.
