#!/usr/bin/env python3
"""Run probe integration and framebuffer checks in an isolated, APFS-cloned Unity project."""
from pathlib import Path
import subprocess
import shutil
import sys
ROOT = Path(__file__).resolve().parents[3]
OUT = ROOT / 'work/pot-probe-activation'
STAGE = OUT / 'project'
STAGE.mkdir(parents=True, exist_ok=True)
for directory in ('Assets', 'Packages', 'ProjectSettings'):
    subprocess.run(['rsync', '-a', str(ROOT / directory) + '/', str(STAGE / directory) + '/'], check=True)
if not (STAGE / 'Library').exists() and (ROOT / 'Library').exists():
    subprocess.run(['cp', '-cR', str(ROOT / 'Library'), str(STAGE / 'Library')], check=True)
focused = '--focused' in sys.argv
method = 'PotProbeActivationValidation.Run' if focused else 'ComboProbeValidation.Run'
shutil.copy(Path(__file__).with_name('PotProbeActivationValidation.cs'), STAGE / 'Assets/Match3D/Editor/PotProbeActivationValidation.cs')
result = STAGE / ('work/pot-probe-results/checks.txt' if focused else 'work/combo-probe/return-to-bowl-results/checks.txt')
result.unlink(missing_ok=True)
subprocess.run(['/Applications/Unity/Unity.app/Contents/MacOS/Unity', '-batchmode',
                '-projectPath', str(STAGE), '-executeMethod', method,
                '-logFile', str(OUT / ('focused.log' if focused else 'unity.log'))], check=True, timeout=360, cwd=STAGE)
report = result.read_text()
print(report)
if not report.endswith('COMPLETE\n'):
    raise SystemExit('Probe activation validation failed')
