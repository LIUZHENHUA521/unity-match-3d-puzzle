from pathlib import Path
import subprocess, shutil
ROOT=Path(__file__).resolve().parents[3]
OUT=ROOT/'work/pot-acquisition'
STAGE=OUT/'project'
STAGE.mkdir(parents=True,exist_ok=True)
for d in ('Assets','Packages','ProjectSettings'):
 subprocess.run(['rsync','-a',str(ROOT/d)+'/',str(STAGE/d)+'/'],check=True)
if not (STAGE/'Library').exists():subprocess.run(['cp','-cR',str(ROOT/'Library'),str(STAGE/'Library')],check=True)
shutil.copy(Path(__file__).with_name('PotAcquisitionValidation.cs'),STAGE/'Assets/Match3D/Editor/PotAcquisitionValidation.cs')
for method in ['PotAcquisitionValidation.Run','RewardAcquisitionValidation.Run']:
 subprocess.run(['/Applications/Unity/Unity.app/Contents/MacOS/Unity','-batchmode','-projectPath',str(STAGE),'-executeMethod',method,'-logFile',str(OUT/(method+'.log'))],check=True,timeout=300,cwd=STAGE)
print((STAGE/'work/pot-acquisition-checks.txt').read_text())
print((STAGE/'docs/validation/reward-acquisition/result.txt').read_text())
