#!/usr/bin/env python3
"""Exercise production record component in isolated Unity with a local HTTP fixture."""
import json
from pathlib import Path
import shutil
import subprocess
import threading
import time
from http.server import BaseHTTPRequestHandler, ThreadingHTTPServer

ROOT = Path(__file__).resolve().parents[3]
OUT = ROOT / 'work/ab-integration/runtime'
STAGE = OUT / 'project'
for folder in ('Assets/Editor', 'Packages', 'ProjectSettings'):
    (STAGE / folder).mkdir(parents=True, exist_ok=True)
shutil.copy(ROOT / 'ProjectSettings/ProjectVersion.txt', STAGE / 'ProjectSettings/ProjectVersion.txt')
deps = json.loads((ROOT / 'Packages/manifest.json').read_text())['dependencies']
(STAGE / 'Packages/manifest.json').write_text(json.dumps({'dependencies': {k: v for k, v in deps.items() if k.startswith('com.unity.modules.')}}))
shutil.copy(ROOT / 'Assets/Match3D/Scripts/Match3DABExperiments.cs', STAGE / 'Assets/Match3DABExperiments.cs')
shutil.copy(Path(__file__).with_name('FixtureDependencies.cs'), STAGE / 'Assets/FixtureDependencies.cs')
shutil.copy(Path(__file__).with_name('ABProbe.cs'), STAGE / 'Assets/Editor/ABProbe.cs')
for name in ('result.txt', 'slow-started'):
    (OUT / name).unlink(missing_ok=True)
counts = {}
class Handler(BaseHTTPRequestHandler):
    def do_POST(self):
        assert self.path == '/invites/ab'
        assert self.headers.get('X-Goose-Progress-Display') == '1'
        assert self.headers.get('X-Pot-Combo-Praise') == '1'
        assert json.loads(self.rfile.read(int(self.headers['Content-Length']))) == {}
        token = self.headers.get('Authorization', '').replace('Bearer ', '')
        counts[token] = counts.get(token, 0) + 1
        code, owner, group = 200, {'token-a':'owner-a','token-b':'owner-b','token-c':'owner-c','empty':'owner-b','slow':'owner-b'}.get(token, 'owner-b'), {'token-a':'B','token-b':'C','token-c':'D','slow':'OLD'}.get(token)
        if token == 'slow':
            (OUT / 'slow-started').touch()
            time.sleep(1)
        if token == 'failure': code = 503
        if token == 'unauthorized': code = 401
        body=json.dumps({'ok':code==200,'owner':owner,'experiments':[{'testKey':'demo','group':group}] if group else []}).encode()
        self.send_response(code)
        self.send_header('Content-Type','application/json')
        self.send_header('Content-Length',str(len(body)))
        self.end_headers()
        self.wfile.write(body)
    def log_message(self,*args): pass

server = ThreadingHTTPServer(('127.0.0.1', 0), Handler)
(OUT / 'endpoint.txt').write_text(f'http://127.0.0.1:{server.server_port}')
threading.Thread(target=server.serve_forever, daemon=True).start()
try:
    subprocess.run(['/Applications/Unity/Unity.app/Contents/MacOS/Unity', '-batchmode', '-nographics', '-projectPath', str(STAGE), '-executeMethod', 'ABProbe.Run', '-logFile', str(OUT / 'unity.log')], check=True, timeout=240, cwd=STAGE)
    result = (OUT / 'result.txt').read_text()
    print(result)
    assert 'ALL PASSED' in result
    assert all(t in counts for t in ['token-a','token-b','token-c','empty','failure','slow','unauthorized']), counts
finally:
    server.shutdown()
    (OUT / 'requests.json').write_text(json.dumps(counts, indent=2))
