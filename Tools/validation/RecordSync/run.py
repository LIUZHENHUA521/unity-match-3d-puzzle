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
OUT = ROOT / 'work/record-sync/runtime'
STAGE = OUT / 'project'
for folder in ('Assets/Editor', 'Packages', 'ProjectSettings'):
    (STAGE / folder).mkdir(parents=True, exist_ok=True)
shutil.copy(ROOT / 'ProjectSettings/ProjectVersion.txt', STAGE / 'ProjectSettings/ProjectVersion.txt')
deps = json.loads((ROOT / 'Packages/manifest.json').read_text())['dependencies']
(STAGE / 'Packages/manifest.json').write_text(json.dumps({'dependencies': {k: v for k, v in deps.items() if k.startswith('com.unity.modules.')}}))
shutil.copy(ROOT / 'Assets/Match3D/Scripts/Match3DGameRecords.cs', STAGE / 'Assets/Match3DGameRecords.cs')
shutil.copy(ROOT / 'Assets/Match3D/Scripts/Match3DAdTelemetry.cs', STAGE / 'Assets/Match3DAdTelemetry.cs')
shutil.copy(Path(__file__).with_name('FixtureDependencies.cs'), STAGE / 'Assets/FixtureDependencies.cs')
shutil.copy(Path(__file__).with_name('RecordSyncProbe.cs'), STAGE / 'Assets/Editor/RecordSyncProbe.cs')
for name in ('result.txt', 'slow-started'):
    (OUT / name).unlink(missing_ok=True)
counts = {}

class Handler(BaseHTTPRequestHandler):
    def do_POST(self):
        json.loads(self.rfile.read(int(self.headers['Content-Length'])))
        counts[self.path] = counts.get(self.path, 0) + 1
        action = self.path.rsplit('/', 1)[-1]
        code, reply = 200, {'ok': True}
        if action == 'slow':
            (OUT / 'slow-started').touch()
            time.sleep(1)
        if action in ('bad', 'slow'):
            code, reply = 400, {'error': '关卡时间或数量无效'}
        elif action == 'conflict':
            code, reply = 409, {'error': '开局信息冲突'}
        elif action in ('retry', 'auth') and counts[self.path] == 1:
            code, reply = (503 if action == 'retry' else 401), {'error': 'temporary'}
        elif action == 'malformed':
            code = 400
        body = b'not-json' if action == 'malformed' else json.dumps(reply).encode()
        self.send_response(code)
        self.send_header('Content-Type', 'application/json')
        self.send_header('Content-Length', str(len(body)))
        self.end_headers()
        self.wfile.write(body)
    def log_message(self, *args):
        pass

server = ThreadingHTTPServer(('127.0.0.1', 0), Handler)
(OUT / 'endpoint.txt').write_text(f'http://127.0.0.1:{server.server_port}')
threading.Thread(target=server.serve_forever, daemon=True).start()
try:
    subprocess.run(['/Applications/Unity/Unity.app/Contents/MacOS/Unity', '-batchmode', '-nographics', '-projectPath', str(STAGE), '-executeMethod', 'RecordSyncProbe.Run', '-logFile', str(OUT / 'unity.log')], check=True, timeout=240, cwd=STAGE)
    result = (OUT / 'result.txt').read_text()
    print(result)
    assert 'ALL PASSED' in result
    assert counts == {'/records/bad': 2, '/records/conflict': 1, '/records/slow': 1, '/records/retry': 2, '/records/auth': 2, '/records/malformed': 1}, counts
    print('PASS rejected records are not retried; transient/auth failures are retried')
finally:
    server.shutdown()
    (OUT / 'requests.json').write_text(json.dumps(counts, indent=2))
