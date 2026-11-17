#!/usr/bin/env python3
"""Compile the game and validate record completion in an isolated Unity copy."""
from pathlib import Path
import shutil
import subprocess

ROOT = Path(__file__).resolve().parents[3]
OUT = ROOT / 'work/pot-victory-flow'
STAGE = OUT / 'project'
STAGE.mkdir(parents=True, exist_ok=True)
for directory in ('Assets', 'Packages', 'ProjectSettings'):
    subprocess.run(['rsync', '-a', '--delete', str(ROOT / directory) + '/', str(STAGE / directory) + '/'], check=True)
# APFS clones speed up compilation without sharing writable files with the open editor.
if not (STAGE / 'Library').exists() and (ROOT / 'Library').exists():
    subprocess.run(['cp', '-cR', str(ROOT / 'Library'), str(STAGE / 'Library')], check=True)
shutil.copy(Path(__file__).with_name('PotVictoryFlowValidation.cs'), STAGE / 'Assets/Match3D/Editor/PotVictoryFlowValidation.cs')
(OUT / 'result-artifacts').mkdir(exist_ok=True)
(OUT / 'result-artifacts/result.txt').unlink(missing_ok=True)
subprocess.run(['/Applications/Unity/Unity.app/Contents/MacOS/Unity', '-batchmode',
                '-projectPath', str(STAGE), '-executeMethod', 'PotVictoryFlowValidation.Run',
                '-logFile', str(OUT / 'unity.log')], check=True, timeout=300, cwd=STAGE)
result = (OUT / 'result-artifacts/result.txt').read_text()
print(result)
if 'ALL CHECKS PASSED' not in result:
    raise SystemExit('Record completion validation did not pass')
