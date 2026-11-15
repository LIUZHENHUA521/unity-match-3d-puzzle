#!/usr/bin/env python3
from pathlib import Path
import subprocess, shutil, argparse
ROOT=Path(__file__).resolve().parents[3]
OUT=ROOT/'work/pot-replay-audio'
STAGE=OUT/'project'
parser=argparse.ArgumentParser()
parser.add_argument('--replay',type=Path,required=True)
args=parser.parse_args()
STAGE.mkdir(parents=True,exist_ok=True)
base=subprocess.check_output(['git','rev-parse','HEAD'],cwd=ROOT).decode().strip()
for folder in ('Assets','Packages','ProjectSettings'):
    if (STAGE/folder).exists():shutil.rmtree(STAGE/folder)
archive=subprocess.Popen(['git','archive',base,'Assets','Packages','ProjectSettings'],cwd=ROOT,stdout=subprocess.PIPE)
subprocess.run(['tar','-xf','-','-C',str(STAGE)],stdin=archive.stdout,check=True)
archive.stdout.close()
if archive.wait():raise SystemExit('snapshot failed')
for name in ('PotReplayAudio','PotReplayPlayer'):
    for ext in ('.cs','.cs.meta'):
        relative=Path('Assets/Match3D/Scripts')/(name+ext)
        shutil.copy(ROOT/relative,STAGE/relative)
if not (STAGE/'Library').exists():subprocess.run(['cp','-cR',str(ROOT/'Library'),str(STAGE/'Library')],check=True)
shutil.copy(Path(__file__).with_name('PotReplayAudioValidation.cs'),STAGE/'Assets/Match3D/Editor/PotReplayAudioValidation.cs')
output=STAGE/'work/pot-replay-audio';output.mkdir(parents=True,exist_ok=True)
shutil.copy(args.replay,output/'fixture.json.gz')
(output/'checks.txt').unlink(missing_ok=True)
subprocess.run(['/Applications/Unity/Unity.app/Contents/MacOS/Unity','-batchmode','-projectPath',str(STAGE),'-executeMethod','PotReplayAudioValidation.Run','-logFile',str(OUT/'unity.log')],check=True,timeout=600,cwd=STAGE)
report=(output/'checks.txt').read_text();print(report)
if not report.endswith('COMPLETE\n'):raise SystemExit('Replay audio validation failed')
