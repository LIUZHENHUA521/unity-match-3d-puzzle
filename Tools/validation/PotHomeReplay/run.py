#!/usr/bin/env python3
"""Exercise home replay UI against an isolated local HTTP fixture, never a production account."""
from pathlib import Path
import json
import subprocess
import shutil
import threading
import time
import gzip
import hashlib
import argparse
from http.server import ThreadingHTTPServer, BaseHTTPRequestHandler

ROOT = Path(__file__).resolve().parents[3]
OUT = ROOT / 'work/pot-home-replay'
STAGE = OUT / 'project'
BASE = subprocess.check_output(['git', 'rev-parse', 'HEAD'], cwd=ROOT).decode().strip()
DATA = STAGE / 'work/pot-home-replay'
parser = argparse.ArgumentParser()
parser.add_argument('--replay', type=Path, required=True)
args = parser.parse_args()
payload = args.replay.read_bytes()
recording = json.loads(gzip.decompress(payload))
STAGE.mkdir(parents=True, exist_ok=True)
# Include the full current source tree so new replay dependencies are compiled.
for folder in ('Assets', 'Packages', 'ProjectSettings'):
    subprocess.run(['rsync', '-a', '--delete', str(ROOT / folder) + '/', str(STAGE / folder) + '/'], check=True)
if not (STAGE / 'Library').exists():
    subprocess.run(['cp', '-cR', str(ROOT / 'Library'), str(STAGE / 'Library')], check=True)
shutil.copy(Path(__file__).with_name('PotHomeReplayValidation.cs'), STAGE / 'Assets/Match3D/Editor/PotHomeReplayValidation.cs')
DATA.mkdir(parents=True, exist_ok=True)
(DATA / 'checks.txt').unlink(missing_ok=True)
(DATA / 'api-mode').write_text('pending')
requests = []

class Handler(BaseHTTPRequestHandler):
    def log_message(self, *_): pass
    def do_POST(self):
        body = json.loads(self.rfile.read(int(self.headers['Content-Length'])))
        mode = (DATA / 'api-mode').read_text()
        requests.append({'path': self.path, 'skip': body.get('skip', 0), 'mode': mode})
        (DATA / 'requests.json').write_text(json.dumps(requests))
        status, response = 200, {'ok': True, 'list': [], 'hasMore': False}
        today = time.strftime('%Y-%m-%d', time.gmtime(time.time() + 8 * 3600))
        def row(tier, result='win', key='a'):
            return {'id': key*64, 'recordId': key*32, 'date': today, 'tier': tier, 'status': result,
                    'stage': 2, 'resourceVersion': 'pot-replay-resources-2', 'createdAt': int(time.time()*1000)}
        if self.path.endswith('/list') and mode in ('ready', 'download-fail', 'download-ok', 'download-cancel', 'slow'):
            if mode == 'slow': time.sleep(2)
            if body.get('skip', 0) == 0:
                response = {'ok': True, 'list': [row('bachelor', 'fail', 'f') for _ in range(20)], 'hasMore': True}
            else:
                response = {'ok': True, 'list': [row('bachelor'), row('elite', key='b')], 'hasMore': False}
        elif self.path.endswith('/download-start'):
            time.sleep(3)
            if mode not in ('download-ok', 'download-cancel'):
                status, response = 503, {'ok': False, 'error': 'fixture download failed'}
            else:
                response = {'ok': True, 'download': {'url': 'http://127.0.0.1:' + str(self.server.server_port) + '/file', 'format': 'gzip-v1', 'sha256': hashlib.sha256(payload).hexdigest()}}
        elif self.path.endswith('/upload-complete'):
            if mode == 'already-ready': status, response = 404, {'ok': False}
            elif mode == 'upload-fail': status, response = 503, {'ok': False}
            else: response = {'ok': True, 'ready': True, 'id': 'a'*64}
        elif self.path.endswith('/upload-start'):
            response = {'ok': True, 'ready': True, 'id': 'a'*64}
        encoded = json.dumps(response).encode()
        self.send_response(status)
        self.send_header('Content-Type', 'application/json')
        self.send_header('Content-Length', str(len(encoded)))
        self.end_headers()
        try: self.wfile.write(encoded)
        except (BrokenPipeError, ConnectionResetError): pass

    def do_GET(self):
        time.sleep(.03)
        self.send_response(200)
        self.send_header('Content-Type', 'application/octet-stream')
        self.send_header('Content-Length', str(len(payload)))
        self.end_headers()
        try: self.wfile.write(payload)
        except (BrokenPipeError, ConnectionResetError): pass

server = ThreadingHTTPServer(('127.0.0.1', 0), Handler)
threading.Thread(target=server.serve_forever, daemon=True).start()
(DATA / 'endpoint').write_text('http://127.0.0.1:' + str(server.server_port))
try:
    subprocess.run(['/Applications/Unity/Unity.app/Contents/MacOS/Unity', '-batchmode', '-projectPath', str(STAGE),
                    '-executeMethod', 'PotHomeReplayValidation.Run', '-logFile', str(OUT / 'unity.log')],
                   check=True, timeout=600, cwd=STAGE)
    report = (DATA / 'checks.txt').read_text()
    print(report)
    failures = [r for r in requests if r['path'].endswith('/download-start') and r['mode'] == 'download-fail']
    assert len(failures) == 1, 'Repeated clicks must not start duplicate downloads'
    if not report.endswith('COMPLETE\n'): raise SystemExit('Home replay validation failed')
finally:
    server.shutdown()
