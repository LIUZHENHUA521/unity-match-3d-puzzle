#!/usr/bin/env python3
"""Compare actual Unity download paths against one local replay and controlled latency."""
from pathlib import Path
from http.server import ThreadingHTTPServer, BaseHTTPRequestHandler
import argparse, base64, gzip, hashlib, json, shutil, subprocess, threading, time
ROOT = Path(__file__).resolve().parents[3]
OUT = ROOT / 'work/pot-replay-download'
STAGE = OUT / 'project'
DATA = STAGE / 'work/pot-replay-download'
p = argparse.ArgumentParser()
p.add_argument('--replay', type=Path, required=True)
p.add_argument('--latency-ms', type=int, default=80)
p.add_argument('--baseline', default='f0b9b45d67b0b43ff543112daae69c8fecd8c140')
args = p.parse_args()
payload = args.replay.read_bytes()
recording = json.loads(gzip.decompress(payload))
DATA.mkdir(parents=True, exist_ok=True)
for directory in ('Assets', 'Packages', 'ProjectSettings'):
    subprocess.run(['rsync', '-a', '--delete', str(ROOT / directory) + '/', str(STAGE / directory) + '/'], check=True)
if not (STAGE / 'Library').exists():
    subprocess.run(['cp', '-cR', str(ROOT / 'Library'), str(STAGE / 'Library')], check=True)
shutil.copy(Path(__file__).with_name('PotReplayDownloadValidation.cs'), STAGE / 'Assets/Match3D/Editor/PotReplayDownloadValidation.cs')
# Preserve the old implementation as the benchmark control; call its real Send via reflection.
old = subprocess.check_output(['git', 'show', args.baseline + ':Assets/Match3D/Scripts/PotReplayService.cs'], cwd=ROOT).decode()
old = old[old.index('    public IEnumerator Detail('):old.index('    IEnumerator Send(')]
assert '196608' in old, 'Benchmark control must be the original chunk downloader'
old = old.replace('public IEnumerator Detail(string id,Action<Reply> done)', 'public static IEnumerator Detail(PotReplayService service,string id,Action<PotReplayService.Reply> done)')
old = old.replace('Reply', 'PotReplayService.Reply').replace('PotReplayService.PotReplayService.', 'PotReplayService.')
old = old.replace('Send("download",new RequestBody{id=id,offset=(int)bytes.Length},r=>part=r)', 'Send(service,id,(int)bytes.Length,r=>part=r)')
helper = '''
static IEnumerator Send(PotReplayService service,string id,int offset,Action<PotReplayService.Reply> done){
 var t=typeof(PotReplayService);var body=t.GetNestedType("RequestBody",BindingFlags.NonPublic);
 var value=Activator.CreateInstance(body);body.GetField("id").SetValue(value,id);body.GetField("offset").SetValue(value,offset);
 return (IEnumerator)t.GetMethod("Send",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(service,new object[]{"download",value,done,null,null});
}
'''
(STAGE / 'Assets/Match3D/Editor/PotReplayDownloadBaseline.cs').write_text('using System;using System.IO;using System.Collections;using System.Reflection;using UnityEngine;\npublic static class PotReplayDownloadBaseline {\n'+old+helper+'}\n')
requests = []
lock = threading.Lock()
counts = {}
latency = args.latency_ms / 1000
class Handler(BaseHTTPRequestHandler):
    protocol_version = 'HTTP/1.1'
    def log_message(self, *_): pass
    def mode(self): return (DATA / 'mode').read_text()
    def log_request_data(self, mode):
        with lock:
            requests.append({'path': self.path.split('?')[0], 'mode': mode, 'hasAuthorization': bool(self.headers.get('Authorization'))})
            counts[mode, self.path] = counts.get((mode, self.path), 0) + 1
            return counts[mode, self.path]
    def respond(self, status, body, kind='application/json'):
        if not isinstance(body, bytes): body = json.dumps(body).encode()
        self.send_response(status)
        self.send_header('Content-Type', kind)
        self.send_header('Content-Length', str(len(body)))
        self.end_headers()
        try: self.wfile.write(body)
        except (BrokenPipeError, ConnectionResetError): pass
    def do_POST(self):
        body=json.loads(self.rfile.read(int(self.headers.get('Content-Length', 0))) or '{}')
        mode=self.mode();self.log_request_data(mode)
        time.sleep(latency)
        if self.path.endswith('/download-start'):
            if mode=='denied': return self.respond(404, {'ok': False, 'error': '录像不存在或未公开'})
            ticket={'url': f'http://127.0.0.1:{self.server.server_port}/file', 'format': 'gzip-v1', 'sha256': hashlib.sha256(payload).hexdigest()}
            if mode=='bad-url': ticket['url']='file:///tmp/private'
            if mode=='oversize': ticket['sha256']=hashlib.sha256(b'x'*(16*1024*1024+1)).hexdigest()
            return self.respond(200, {'ok': True, 'download': ticket})
        if self.path.endswith('/download'):
            # Old server fetches the full object before slicing each chunk.
            time.sleep(latency)
            offset=body.get('offset',0)
            return self.respond(200, {'ok': True, 'total': len(payload), 'content': base64.b64encode(payload[offset:offset+196608]).decode()})
        return self.respond(404, {'ok': False})
    def do_GET(self):
        mode=self.mode();count=self.log_request_data(mode)
        time.sleep(latency)
        if mode=='retry' and count==1: return self.respond(403, b'expired')
        if mode=='unavailable': return self.respond(503, b'unavailable')
        if mode=='switch': time.sleep(1)
        data=payload
        if mode=='corrupt': data=payload[:-8]+b'broken!!'
        if mode=='oversize': data=b'x'*(16*1024*1024+1)
        return self.respond(200, data, 'application/octet-stream')
server=ThreadingHTTPServer(('127.0.0.1',0),Handler)
threading.Thread(target=server.serve_forever,daemon=True).start()
(DATA/'endpoint').write_text(f'http://127.0.0.1:{server.server_port}')
(DATA/'expected').write_text(str(len(recording['frames'])))
(DATA/'mode').write_text('idle')
try:
    subprocess.run(['/Applications/Unity/Unity.app/Contents/MacOS/Unity','-batchmode','-projectPath',str(STAGE),'-executeMethod','PotReplayDownloadValidation.Run','-logFile',str(OUT/'unity.log')],check=True,timeout=600,cwd=STAGE)
    (DATA/'requests.json').write_text(json.dumps(requests,indent=2))
    requests=[r for r in requests if r['path'] in ('/pot-replays/download','/pot-replays/download-start','/file')]
    checks=(DATA/'checks.txt').read_text();print(checks)
    assert checks.endswith('COMPLETE\n')
    assert not any(r['hasAuthorization'] for r in requests if r['path']=='/file'), 'Account token leaked to storage'
    for i in range(3):
        assert sum(r['mode']==f'direct-{i}' for r in requests)==2
        assert sum(r['mode']==f'baseline-{i}' for r in requests)==(len(payload)+196607)//196608
    assert sum(r['mode']=='retry' for r in requests)==4
    assert sum(r['mode']=='unavailable' for r in requests)==4
    assert not any(r['path']=='/file' and r['mode'] in ('denied','bad-url') for r in requests)
    timings=json.loads((DATA/'timings.json').read_text())
    report={'fixtureBytes':len(payload),'baselineRevision':args.baseline,'baselineRequests':(len(payload)+196607)//196608,'directRequests':2,'frames':len(recording['frames']),'latencyMsPerHop':args.latency_ms,'environment':'Unity Editor, localhost HTTP; controlled latency, not production or phone bandwidth','timings':timings}
    (OUT/'report.json').write_text(json.dumps(report,indent=2));print(json.dumps(report,indent=2))
finally:
    server.shutdown()
