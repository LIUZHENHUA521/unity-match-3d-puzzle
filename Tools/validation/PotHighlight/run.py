"""Render production selection/tool highlighting in a disposable Unity project."""
from pathlib import Path
import argparse
import os
import shutil
import subprocess

ROOT = Path(__file__).resolve().parents[3]
parser = argparse.ArgumentParser()
parser.add_argument('--stage', type=Path, default=ROOT / 'work/pot-highlight/project')
parser.add_argument('--output', type=Path, default=ROOT / 'docs/validation/pot-highlight')
args = parser.parse_args()
stage, output = args.stage.resolve(), args.output.resolve()
if stage == ROOT or stage in ROOT.parents or stage == output or stage in output.parents:
    parser.error('stage must be a disposable copy separate from source and output')
stage.mkdir(parents=True, exist_ok=True)
output.mkdir(parents=True, exist_ok=True)
for folder in ('Assets', 'Packages', 'ProjectSettings'):
    subprocess.run(['rsync', '-a', '--delete', str(ROOT / folder) + '/', str(stage / folder) + '/'], check=True)
if not (stage / 'Library').exists():
    subprocess.run(['cp', '-cR', str(ROOT / 'Library'), str(stage / 'Library')], check=True)
shutil.copy2(Path(__file__).with_name('PotHighlightValidation.cs'), stage / 'Assets/Match3D/Editor/PotHighlightValidation.cs')
env = dict(os.environ, POT_HIGHLIGHT_OUTPUT=str(output))
subprocess.run(['/Applications/Unity/Unity.app/Contents/MacOS/Unity', '-batchmode', '-job-worker-count', '2',
                '-projectPath', str(stage), '-executeMethod', 'PotHighlightValidation.Run',
                '-logFile', str(stage / 'Temp/pot-highlight-unity.log')], env=env, check=True, timeout=600)
report = (output / 'checks.txt').read_text()
print(report)
if not report.endswith('COMPLETE\n'):
    raise RuntimeError('Incomplete highlight validation')
