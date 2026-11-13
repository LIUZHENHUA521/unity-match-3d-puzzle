#!/usr/bin/env python3
"""Compile the game and validate camera interaction in an isolated Unity copy."""
from pathlib import Path
import shutil
import subprocess

ROOT = Path(__file__).resolve().parents[3]
OUT = ROOT / 'work/board-camera-zoom'
STAGE = OUT / 'project'
STAGE.mkdir(parents=True, exist_ok=True)
for directory in ('Assets', 'Packages', 'ProjectSettings'):
    subprocess.run(['rsync', '-a', str(ROOT / directory) + '/', str(STAGE / directory) + '/'], check=True)
# APFS clones speed up compilation without sharing writable files with the open editor.
if not (STAGE / 'Library').exists() and (ROOT / 'Library').exists():
    subprocess.run(['cp', '-cR', str(ROOT / 'Library'), str(STAGE / 'Library')], check=True)
shutil.copy(Path(__file__).with_name('BoardCameraZoomValidation.cs'), STAGE / 'Assets/Match3D/Editor/BoardCameraZoomValidation.cs')
(OUT / 'result.txt').unlink(missing_ok=True)
subprocess.run(['/Applications/Unity/Unity.app/Contents/MacOS/Unity', '-batchmode',
                '-projectPath', str(STAGE), '-executeMethod', 'BoardCameraZoomValidation.Run',
                '-logFile', str(OUT / 'unity.log')], check=True, timeout=300, cwd=STAGE)
result = (OUT / 'result.txt').read_text()
print(result)
if not result.startswith('PASS '):
    raise SystemExit('Camera zoom validation did not pass')
