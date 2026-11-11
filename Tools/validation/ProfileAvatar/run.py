#!/usr/bin/env python3
"""Run the real profile image loader in a disposable Unity editor project."""
import json
from pathlib import Path
import shutil
import subprocess

ROOT = Path(__file__).resolve().parents[3]
OUT = ROOT / 'work/profile-avatar-validation'
STAGE = OUT / 'project'
for folder in ('Assets/Editor', 'Packages', 'ProjectSettings'):
    (STAGE / folder).mkdir(parents=True, exist_ok=True)
shutil.copy(ROOT / 'ProjectSettings/ProjectVersion.txt', STAGE / 'ProjectSettings/ProjectVersion.txt')
dependencies = json.loads((ROOT / 'Packages/manifest.json').read_text())['dependencies']
dependencies = {k: v for k, v in dependencies.items()
                if k.startswith('com.unity.modules.') or k == 'com.unity.ugui'}
(STAGE / 'Packages/manifest.json').write_text(json.dumps({'dependencies': dependencies}, indent=2))
shutil.copy(ROOT / 'Assets/Match3D/Scripts/Match3DUserProfileView.cs', STAGE / 'Assets/Match3DUserProfileView.cs')
shutil.copy(Path(__file__).with_name('ProfileAvatarProbe.cs'), STAGE / 'Assets/Editor/ProfileAvatarProbe.cs')
# No production services or account data are used by these image-loader tests.
(STAGE / 'Assets/ProfileStubs.cs').write_text('''using System;
using UnityEngine;
public class Match3DInvitations:MonoBehaviour {
 public event Action Changed; public bool Ready,Busy; public string Status;
 public ProfileState State=new ProfileState(); public void Refresh(){} }
public class ProfileState { public string inviteCode; public long createdAt; }
public class Match3DFlow:MonoBehaviour { public static Match3DFlow Instance; public void HomeClick(){} }
''')
(OUT / 'result.txt').unlink(missing_ok=True)
subprocess.run(['/Applications/Unity/Unity.app/Contents/MacOS/Unity', '-batchmode', '-nographics',
                '-projectPath', str(STAGE), '-executeMethod', 'ProfileAvatarProbe.Run',
                '-logFile', str(OUT / 'unity.log')], check=True, timeout=240, cwd=STAGE)
result = (OUT / 'result.txt').read_text()
print(result)
if 'ALL PASSED' not in result:
    raise SystemExit('Avatar validation did not finish')
