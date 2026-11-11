#!/usr/bin/env python3
"""Compile and exercise actual client components against actual server logic on localhost."""
from pathlib import Path
import subprocess
import time
ROOT = Path(__file__).resolve().parents[3]
OUT = ROOT / 'docs/validation/seven-day-sign-in'
OUT.mkdir(parents=True, exist_ok=True)
(OUT / 'endpoint.txt').unlink(missing_ok=True)
server = subprocess.Popen(['node', str(Path(__file__).with_name('server.mjs')), str(OUT)], cwd=ROOT)
try:
    for _ in range(100):
        if (OUT / 'endpoint.txt').exists(): break
        if server.poll() is not None: raise RuntimeError('Fixture failed')
        time.sleep(.05)
    subprocess.run(['/Applications/Unity/Unity.app/Contents/MacOS/Unity', '-batchmode',
                    '-projectPath', str(ROOT), '-executeMethod', 'Match3DSignInValidation.Run',
                    '-logFile', str(ROOT / 'Logs/sign-in-validation.log')], check=True, timeout=240, cwd=ROOT)
    result = (OUT / 'result.txt').read_text()
    print(result)
    assert 'ALL PASSED' in result
finally:
    server.terminate()
    server.wait(timeout=10)
    (OUT / 'endpoint.txt').unlink(missing_ok=True)
