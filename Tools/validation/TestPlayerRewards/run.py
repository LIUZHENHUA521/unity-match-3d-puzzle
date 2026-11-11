#!/usr/bin/env python3
"""Isolated Unity runtime checks using production rewards, records and receipt UI; no cloud calls."""
import json
from pathlib import Path
import re
import shutil
import subprocess
import threading
from http.server import BaseHTTPRequestHandler, ThreadingHTTPServer

ROOT = Path(__file__).resolve().parents[3]
OUT = ROOT / 'work/test-player-rewards/runtime'
STAGE = OUT / 'project'
for folder in ('Assets/Editor', 'Packages', 'ProjectSettings'):
    (STAGE / folder).mkdir(parents=True, exist_ok=True)
shutil.copy(ROOT / 'ProjectSettings/ProjectVersion.txt', STAGE / 'ProjectSettings/ProjectVersion.txt')
deps = json.loads((ROOT / 'Packages/manifest.json').read_text())['dependencies']
(STAGE / 'Packages/manifest.json').write_text(json.dumps({'dependencies': {k: v for k, v in deps.items() if k.startswith('com.unity.modules.') or k == 'com.unity.ugui'}}))

def copy(path):
    target = STAGE / path.relative_to(ROOT)
    target.parent.mkdir(parents=True, exist_ok=True)
    shutil.copy(path, target)
    meta = Path(str(path) + '.meta')
    if meta.exists():
        shutil.copy(meta, str(target) + '.meta')

names = ['Rewards', 'GameRecords', 'TestPrivileges', 'RewardPopup', 'RewardPresenter', 'RewardItem', 'RewardItemView', 'UIView', 'UIFitContent', 'DefaultFont', 'TextStroke', 'CustomerService', 'Flow.ReceivedRewards']
for name in names:
    copy(ROOT / f'Assets/Match3D/Scripts/Match3D{name}.cs')
# Preserve actual prefab references and artwork rather than reproducing its layout.
assets = {}
for meta in (ROOT / 'Assets/Match3D').rglob('*.meta'):
    match = re.search(r'^guid: (\w+)', meta.read_text(), re.M)
    if match:
        assets[match[1]] = Path(str(meta)[:-5])
prefab = ROOT / 'Assets/Match3D/Resources/Match3D/UI/RewardReceived.prefab'
copy(prefab)
for guid in set(re.findall(r'guid: (\w+)', prefab.read_text())):
    path = assets.get(guid)
    if path and path.suffix != '.cs':
        copy(path)
shutil.copy(Path(__file__).with_name('TestPlayerRewardsProbe.cs'), STAGE / 'Assets/Editor/TestPlayerRewardsProbe.cs')
shutil.copy(Path(__file__).with_name('FixtureDependencies.cs'), STAGE / 'Assets/FixtureDependencies.cs')
requests = []
class Handler(BaseHTTPRequestHandler):
    def do_POST(self):
        body = json.loads(self.rfile.read(int(self.headers.get('Content-Length', 0))) or '{}')
        requests.append((self.path, body))
        code = 200
        if self.path == '/rewards/runs':
            if body['tier'] == 'denied':
                code, result = 409, dict(ok=False, error='fixture quota exhausted')
            else:
                result = dict(ok=True, runId='fixture-qualified-run', rewardTier=body['tier'], ruleVersion='fixture-rule', testToolsEnabled=body['tier']=='bachelor', level=dict(date='2026-11-11', version='fixture-level', tier=body['tier'], seed=123, generatorVersion='daily-v1'))
        elif self.path == '/rewards/list':
            result = dict(ok=True, nextCursor=-1, vouchers=[dict(id='fixture-voucher', money=2, createTime=1794355200000)])
        else:
            code, result = 404, dict(ok=False, error='unexpected fixture route')
        data = json.dumps(result).encode()
        self.send_response(code)
        self.send_header('Content-Type', 'application/json')
        self.send_header('Content-Length', str(len(data)))
        self.end_headers()
        self.wfile.write(data)
    def log_message(self, *args): pass
server = ThreadingHTTPServer(('127.0.0.1', 0), Handler)
(OUT / 'endpoint.txt').write_text(f'http://127.0.0.1:{server.server_port}')
threading.Thread(target=server.serve_forever, daemon=True).start()
(OUT / 'result.txt').unlink(missing_ok=True)
try:
    subprocess.run(['/Applications/Unity/Unity.app/Contents/MacOS/Unity', '-batchmode', '-projectPath', str(STAGE), '-executeMethod', 'TestPlayerRewardsProbe.Run', '-logFile', str(OUT / 'unity.log')], check=True, timeout=240, cwd=STAGE)
    result = (OUT / 'result.txt').read_text()
    print(result)
    assert 'ALL PASSED' in result
    starts = [body for path, body in requests if path == '/rewards/runs']
    assert len(starts) == 3 and all(x['levelProtocol'] == 3 for x in starts)
    assert all(path.startswith('/rewards/') for path, _ in requests)
    print('PASS all three entries use formal reward endpoint and protocol 3')
finally:
    server.shutdown()
    (OUT / 'requests.json').write_text(json.dumps(requests, indent=2))
