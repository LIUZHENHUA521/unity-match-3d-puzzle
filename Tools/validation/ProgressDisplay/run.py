from pathlib import Path
import shutil,subprocess,json
root=Path(__file__).resolve().parents[3];out=root/'work/progress-ab/runtime';stage=out/'project'
for d in ['Assets/Editor','Packages','ProjectSettings']:(stage/d).mkdir(parents=True,exist_ok=True)
shutil.copy(root/'ProjectSettings/ProjectVersion.txt',stage/'ProjectSettings/ProjectVersion.txt')
(stage/'Packages/manifest.json').write_text('{"dependencies":{}}')
shutil.copy(root/'Assets/Match3D/Scripts/Match3DProgressDisplay.cs',stage/'Assets/Match3DProgressDisplay.cs')
shutil.copy(Path(__file__).with_name('ProgressProbe.cs'),stage/'Assets/Editor/ProgressProbe.cs')
subprocess.run(['/Applications/Unity/Unity.app/Contents/MacOS/Unity','-batchmode','-nographics','-projectPath',str(stage),'-executeMethod','ProgressProbe.Run','-logFile',str(out/'unity.log')],check=True,timeout=240)
print((out/'result.txt').read_text())
