#!/usr/bin/env python3
"""Validate streamed gzip uploads with the real Unity serializer in an isolated project."""
from pathlib import Path
import shutil
import subprocess

ROOT = Path(__file__).resolve().parents[3]
OUT = ROOT / 'work/pot-replay-reliability'
STAGE = OUT / 'project'
STAGE.mkdir(parents=True, exist_ok=True)
for directory in ('Assets', 'Packages', 'ProjectSettings'):
    subprocess.run(['rsync', '-a', '--delete', str(ROOT / directory) + '/', str(STAGE / directory) + '/'], check=True)
if not (STAGE / 'Library').exists() and (ROOT / 'Library').exists():
    subprocess.run(['cp', '-cR', str(ROOT / 'Library'), str(STAGE / 'Library')], check=True)
shutil.copy(Path(__file__).with_name('PotReplayReliabilityValidation.cs'),
            STAGE / 'Assets/Match3D/Editor/PotReplayReliabilityValidation.cs')
shutil.copy(Path(__file__).with_name('PotReplayCacheValidation.cs'),
            STAGE / 'Assets/Match3D/Editor/PotReplayCacheValidation.cs')
log = OUT / 'unity.log'
subprocess.run(['/Applications/Unity/Unity.app/Contents/MacOS/Unity', '-batchmode',
                '-projectPath', str(STAGE), '-executeMethod', 'PotReplayReliabilityValidation.Run',
                '-logFile', str(log)], check=True, timeout=600, cwd=STAGE)
result = next((line for line in log.read_text().splitlines() if line.startswith('POT_REPLAY_RELIABILITY_OK')), None)
if not result:
    raise SystemExit(f'Replay validation did not pass; inspect {log}')
print(result)
