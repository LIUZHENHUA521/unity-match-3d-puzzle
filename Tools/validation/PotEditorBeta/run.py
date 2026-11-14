from pathlib import Path
import subprocess, shutil, json
ROOT=Path(__file__).resolve().parents[3]
OUT=ROOT/'work/pot-editor-beta'
STAGE=OUT/'project'
STAGE.mkdir(parents=True,exist_ok=True)
for folder in ('Assets','Packages','ProjectSettings'):
    subprocess.run(['rsync','-a',str(ROOT/folder)+'/',str(STAGE/folder)+'/'],check=True)
if not (STAGE/'Library').exists(): subprocess.run(['cp','-cR',str(ROOT/'Library'),str(STAGE/'Library')],check=True)
shutil.copy(Path(__file__).with_name('PotEditorBetaValidation.cs'),STAGE/'Assets/Match3D/Editor/PotEditorBetaValidation.cs')
(STAGE/'UserSettings').mkdir(exist_ok=True)
(STAGE/'UserSettings/PotEditorPreview.json').write_text(json.dumps({'key':'fixture-preview-key'}))
server=ROOT.parent/'crazyCombo/server/services/goose-server/src/server.mjs'
fixture=subprocess.Popen(['node',str(Path(__file__).with_name('fixture.mjs')),str(server)],stdout=subprocess.PIPE,text=True)
try:
    if fixture.stdout.readline().strip()!='READY': raise RuntimeError('Fixture did not start')
    subprocess.run(['/Applications/Unity/Unity.app/Contents/MacOS/Unity','-batchmode','-projectPath',str(STAGE),'-executeMethod','PotEditorBetaValidation.Run','-logFile',str(OUT/'unity.log')],check=True,timeout=300,cwd=STAGE)
    report=(STAGE/'work/pot-editor-beta/checks.txt').read_text()
    print(report)
    if not report.endswith('COMPLETE\n'): raise RuntimeError('Validation incomplete')
finally:
    fixture.terminate()
    fixture.wait(timeout=10)
