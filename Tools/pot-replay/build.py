#!/usr/bin/env python3
"""Build the immutable replay player using the existing isolated WebGL pipeline."""
import subprocess
import shutil
from pathlib import Path
root = Path(__file__).resolve().parents[2]
stage = root / 'Builds/PotReplayWebProject'
subprocess.run(['python3', str(root / 'Tools/daily-preview/build.py'), '--stage', str(stage)], check=True)
output = stage / 'Builds/DailyWebPreview'
for name in ('index.html', 'bridge.js'):
    shutil.copy2(root / 'Tools/pot-replay' / name, output / name)
print('Pot replay player:', output)
