#!/usr/bin/env python3
"""Compile the game and validate tray-only revival in an isolated Unity copy."""
from pathlib import Path
import shutil
import subprocess

ROOT = Path(__file__).resolve().parents[3]
OUT = ROOT / 'work/pot-tray-revival'
STAGE = OUT / 'project'
STAGE.mkdir(parents=True, exist_ok=True)
for directory in ('Assets', 'Packages', 'ProjectSettings'):
    subprocess.run(['rsync', '-a', str(ROOT / directory) + '/', str(STAGE / directory) + '/'], check=True)
# APFS clones speed up compilation without sharing writable files with the open editor.
if not (STAGE / 'Library').exists() and (ROOT / 'Library').exists():
    subprocess.run(['cp', '-cR', str(ROOT / 'Library'), str(STAGE / 'Library')], check=True)
result_path = STAGE / 'work/revive-ui/validation.txt'
result_path.unlink(missing_ok=True)
subprocess.run(['/Applications/Unity/Unity.app/Contents/MacOS/Unity', '-batchmode',
                '-projectPath', str(STAGE), '-executeMethod', 'Match3DReviveValidation.Run',
                '-logFile', str(OUT / 'unity.log')], check=True, timeout=300, cwd=STAGE)
result = result_path.read_text()
(OUT / 'result.txt').write_text(result)
print(result)
if not result.startswith('PASS ') or 'FAIL' in result:
    raise SystemExit('Pot tray revival validation did not pass')
