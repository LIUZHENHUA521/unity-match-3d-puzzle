# Pot highlight validation

Run `python3 Tools/validation/PotHighlight/run.py` from the repository. Uses the current workspace in a disposable copy and the installed Unity editor with graphics enabled. Does not enter the main editor, access network gameplay services, save scenes, or publish.

Optional `--stage` and `--output` choose the disposable project and rendered evidence directory. Tests actual production ItemHighlight with current Party food prefabs on both stencil and depth-only render targets, orthographic and perspective cameras. Three-frame images show plain / selection / two-target absorption. See docs/validation/pot-highlight/README.md for scope and device limitations.
