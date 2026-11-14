#!/usr/bin/env python3
"""Render the production avatar with and without a stencil buffer in isolated Unity."""
import json
from pathlib import Path
import shutil
import subprocess

ROOT = Path(__file__).resolve().parents[3]
OUT = ROOT / 'work/pot-avatar-validation'
STAGE = OUT / 'project'
for folder in ('Assets/Editor', 'Packages', 'ProjectSettings'):
    (STAGE / folder).mkdir(parents=True, exist_ok=True)
shutil.copy(ROOT / 'ProjectSettings/ProjectVersion.txt', STAGE / 'ProjectSettings/ProjectVersion.txt')
deps = json.loads((ROOT / 'Packages/manifest.json').read_text())['dependencies']
(STAGE / 'Packages/manifest.json').write_text(json.dumps({'dependencies': {k: v for k, v in deps.items() if k.startswith('com.unity.modules.') or k == 'com.unity.ugui'}}))
shutil.copy(ROOT / 'Assets/Match3D/Scripts/PotAvatarGraphic.cs', STAGE / 'Assets/PotAvatarGraphic.cs')
shutil.copy(Path(__file__).with_name('PotAvatarValidation.cs'), STAGE / 'Assets/Editor/PotAvatarValidation.cs')
(OUT / 'pot-avatar-result.txt').unlink(missing_ok=True)
subprocess.run(['/Applications/Unity/Unity.app/Contents/MacOS/Unity', '-batchmode', '-quit', '-projectPath', str(STAGE), '-executeMethod', 'PotAvatarValidation.Run', '-logFile', str(OUT / 'unity.log')], check=True, timeout=240, cwd=STAGE)
print((OUT / 'pot-avatar-result.txt').read_text())
