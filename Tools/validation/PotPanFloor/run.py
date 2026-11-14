#!/usr/bin/env python3
"""Run pan-floor editor and macOS Player checks in an existing isolated project."""
import argparse
import plistlib
from pathlib import Path
import shutil
import subprocess

ROOT = Path(__file__).resolve().parents[3]
parser = argparse.ArgumentParser()
parser.add_argument('--stage', required=True, type=Path)
args = parser.parse_args()
stage = args.stage.resolve()
if stage == ROOT or not (stage / 'ProjectSettings').is_dir():
    parser.error('--stage must name an existing disposable Unity copy, not the main project')
output = stage.parent / 'results'
output.mkdir(exist_ok=True)
for directory in ('Assets', 'Packages', 'ProjectSettings'):
    subprocess.run(['rsync', '-a', str(ROOT / directory) + '/', str(stage / directory) + '/'], check=True)
editor = stage / 'Assets/Match3D/Editor'
shutil.copy(Path(__file__).with_name('PotPanFloorValidation.cs'), editor)
shutil.copy(ROOT / 'Tools/validation/PanPlayer/PanPlayerProbeBuild.cs', editor)
shutil.copy(ROOT / 'Tools/validation/PanPlayer/PanPlayerProbe.cs', stage / 'Assets')
unity = '/Applications/Unity/Unity.app/Contents/MacOS/Unity'
for method, report in [('PotPanFloorValidation.Run', 'pot-pan-floor'), ('PanPlayerProbeBuild.Run', 'pot-player-build')]:
    subprocess.run([unity, '-batchmode', '-nographics', '-buildTarget', 'StandaloneOSX',
                    '-projectPath', str(stage), '-executeMethod', method,
                    '-logFile', str(output / (report + '.log'))], cwd=stage, check=True, timeout=600)
app = stage.parent / 'pan-investigation/regression.app'
with (app / 'Contents/Info.plist').open('rb') as file:
    executable = plistlib.load(file)['CFBundleExecutable']
subprocess.run([str(app / 'Contents/MacOS' / executable), '-batchmode', '-nographics',
                '--report', str(output / 'pot-player.txt'), '-logFile', str(output / 'pot-player.log')], check=True, timeout=120)
print((output / 'pot-pan-floor.txt').read_text())
print((output / 'pot-player.txt').read_text().splitlines()[-1])
