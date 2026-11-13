#!/usr/bin/env python3
"""Verify daily snapshot combo integration in an isolated Unity project."""
from pathlib import Path
import subprocess
import shutil
import sys
ROOT = Path(__file__).resolve().parents[3]
OUT = ROOT / 'work/pot-daily-probe'
STAGE = Path(sys.argv[1]).resolve() if len(sys.argv)>1 else OUT / 'project'
STAGE.mkdir(parents=True, exist_ok=True)
OUT.mkdir(parents=True, exist_ok=True)
for directory in ('Assets', 'Packages', 'ProjectSettings'):
    subprocess.run(['rsync', '-a', str(ROOT / directory) + '/', str(STAGE / directory) + '/'], check=True)
if not (STAGE / 'Library').exists() and (ROOT / 'Library').exists():
    subprocess.run(['cp', '-cR', str(ROOT / 'Library'), str(STAGE / 'Library')], check=True)
method = 'PotDailyProbeValidation.Run'
shutil.copy(Path(__file__).with_name('PotDailyProbeValidation.cs'), STAGE / 'Assets/Match3D/Editor/PotDailyProbeValidation.cs')
result = STAGE / 'work/pot-daily-probe/checks.txt'
result.unlink(missing_ok=True)
subprocess.run(['/Applications/Unity/Unity.app/Contents/MacOS/Unity', '-batchmode',
                '-projectPath', str(STAGE), '-executeMethod', method,
                '-logFile', str(OUT / 'unity.log')], check=True, timeout=360, cwd=STAGE)
report = result.read_text()
print(report)
if not report.endswith('COMPLETE\n'):
    raise SystemExit('Daily probe validation failed')
