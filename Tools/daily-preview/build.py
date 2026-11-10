#!/usr/bin/env python3
"""Build the admin WebGL player in a disposable project, leaving the editor open."""
import argparse
import json
from pathlib import Path
import shutil
import subprocess

ROOT = Path(__file__).resolve().parents[2]
parser = argparse.ArgumentParser()
parser.add_argument('--stage', type=Path, default=ROOT / 'Builds/DailyWebProject')
args = parser.parse_args()
stage = args.stage.resolve()
subprocess.run(['python3', str(ROOT / 'Tools/wechat/build.py'), '--stage', str(stage), '--prepare-only'], check=True)
# The WeChat SDK defines a global PlayerPrefs and replaces native WebGL APIs.
# Remove it only from this disposable browser project, including its build entry.
manifest = stage / 'Packages/manifest.json'
packages = json.loads(manifest.read_text())
packages['dependencies'].pop('com.qq.weixin.minigame', None)
manifest.write_text(json.dumps(packages, indent=2) + '\n')
shutil.rmtree(stage / 'Packages/com.qq.weixin.minigame', ignore_errors=True)
for name in ('WechatReleaseBuild.cs', 'WechatReleaseBuild.cs.meta'):
    (stage / 'Assets/Match3D/Editor' / name).unlink(missing_ok=True)
shutil.copytree(ROOT / 'Tools/daily-preview', stage / 'Tools/daily-preview', dirs_exist_ok=True)
log = ROOT / 'Builds/daily-web-build.log'
subprocess.run(['/Applications/Unity/Unity.app/Contents/MacOS/Unity', '-batchmode', '-nographics',
                '-projectPath', str(stage), '-buildTarget', 'WebGL', '-executeMethod',
                'DailyWebPreviewBuild.Run', '-logFile', str(log)], check=True)
output = stage / 'Builds/DailyWebPreview'
if not (output / 'config.json').exists():
    raise SystemExit('Missing WebGL player configuration')
print(f'WebGL player: {output}\nBuild log: {log}')
