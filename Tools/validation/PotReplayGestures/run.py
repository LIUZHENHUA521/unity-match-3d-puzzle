#!/usr/bin/env python3
"""Compile and exercise real replay UI in an isolated Unity project."""
from pathlib import Path
import subprocess
import shutil
import argparse

ROOT = Path(__file__).resolve().parents[3]
parser = argparse.ArgumentParser()
parser.add_argument('--replay', type=Path, required=True, help='Existing gzip v2 replay fixture')
args = parser.parse_args()
OUT = ROOT / 'work/pot-replay-gestures'
STAGE = OUT / 'project'
STAGE.mkdir(parents=True, exist_ok=True)
for directory in ('Assets', 'Packages', 'ProjectSettings'):
    subprocess.run(['rsync', '-a', '--delete', str(ROOT / directory) + '/', str(STAGE / directory) + '/'], check=True)
if not (STAGE / 'Library').exists():
    subprocess.run(['cp', '-cR', str(ROOT / 'Library'), str(STAGE / 'Library')], check=True)
shutil.copy(Path(__file__).with_name('PotReplayGesturesValidation.cs'), STAGE / 'Assets/Match3D/Editor/PotReplayGesturesValidation.cs')
output = STAGE / 'work/pot-replay-gestures'
output.mkdir(parents=True, exist_ok=True)
shutil.copy(args.replay, output / 'fixture.json.gz')
(output / 'checks.txt').unlink(missing_ok=True)
subprocess.run(['/Applications/Unity/Unity.app/Contents/MacOS/Unity', '-batchmode', '-projectPath', str(STAGE),
                '-executeMethod', 'PotReplayGesturesValidation.Run', '-logFile', str(OUT / 'unity.log')],
               check=True, timeout=600, cwd=STAGE)
report = (output / 'checks.txt').read_text()
print(report)
if not report.endswith('COMPLETE\n'):
    raise SystemExit('Replay UI validation failed')
