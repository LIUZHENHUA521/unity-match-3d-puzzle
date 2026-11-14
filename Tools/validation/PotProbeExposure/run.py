#!/usr/bin/env python3
"""Validate exposure retirement against real Unity picking, UI and replay state."""
from pathlib import Path
import shutil
import subprocess

ROOT = Path(__file__).resolve().parents[3]
OUT = ROOT / 'work/pot-probe-exposure'
STAGE = OUT / 'project'
STAGE.mkdir(parents=True, exist_ok=True)
for directory in ('Assets', 'Packages', 'ProjectSettings'):
    subprocess.run(['rsync', '-a', str(ROOT / directory) + '/', str(STAGE / directory) + '/'], check=True)
if not (STAGE / 'Library').exists() and (ROOT / 'Library').exists():
    subprocess.run(['cp', '-cR', str(ROOT / 'Library'), str(STAGE / 'Library')], check=True)
shutil.copy(Path(__file__).with_name('PotProbeExposureValidation.cs'), STAGE / 'Assets/Match3D/Editor/PotProbeExposureValidation.cs')
result = STAGE / 'work/pot-probe-exposure/checks.txt'
result.unlink(missing_ok=True)
subprocess.run(['/Applications/Unity/Unity.app/Contents/MacOS/Unity', '-batchmode',
                '-projectPath', str(STAGE), '-executeMethod', 'PotProbeExposureValidation.Run',
                '-logFile', str(OUT / 'unity.log')], check=True, timeout=360, cwd=STAGE)
report = result.read_text()
print(report)
if not report.endswith('COMPLETE\n'):
    raise SystemExit('Probe exposure validation failed')
