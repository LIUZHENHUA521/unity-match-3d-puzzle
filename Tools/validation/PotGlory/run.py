from pathlib import Path
from http.server import ThreadingHTTPServer, BaseHTTPRequestHandler
import json,threading,subprocess,time,shutil
ROOT=Path(__file__).resolve().parents[3];OUT=ROOT/'work/pot-glory-validation';OUT.mkdir(parents=True,exist_ok=True)
(OUT/'mode').write_text('ready');requests=[]
class Handler(BaseHTTPRequestHandler):
 def log_message(self,*args):pass
 def do_POST(self):
  body=json.loads(self.rfile.read(int(self.headers['Content-Length'])));mode=(OUT/'mode').read_text();requests.append(self.path)
  code=200
  if self.path.endswith('/honors'):
   if mode=='slow':time.sleep(1)
   if mode=='error':code=503;data={'ok':False,'error':'fixture unavailable'}
   elif mode=='empty':data={'ok':True,'honors':[],'total':0}
   else:
    skip=body.get('skip',0);data={'ok':True,'total':12,'hasMore':skip==0,'honors':[{'certificate':{'certificateKey':'pot:fixture'+str(i),'certificateNo':'POT-fixture'+str(i),'recordId':'record'+str(i),'rewardTier':'elite' if i%2==0 else 'bachelor','dateKey':'2026-11-16','activeDurationMs':208000},'replayId':'a'*64 if i!=2 else ''} for i in range(skip,min(skip+10,12))]}
  else:code=503;data={'ok':False,'error':'fixture download failed'}
  content=json.dumps(data).encode();self.send_response(code);self.send_header('Content-Length',str(len(content)));self.end_headers()
  try:self.wfile.write(content)
  except BrokenPipeError:pass
server=ThreadingHTTPServer(('127.0.0.1',0),Handler);threading.Thread(target=server.serve_forever,daemon=True).start();(OUT/'endpoint').write_text('http://127.0.0.1:'+str(server.server_port))
script=ROOT/'Assets/Match3D/Editor/PotGloryValidation.cs';shutil.copy2(Path(__file__).with_name(script.name),script)
try:
 subprocess.run(['/Applications/Unity/Unity.app/Contents/MacOS/Unity','-batchmode','-projectPath',str(ROOT),'-executeMethod','PotGloryValidation.Run','-logFile',str(OUT/'unity.log')],check=True,timeout=600)
 print((OUT/'checks.txt').read_text());assert requests.count('/pot-replays/download-start')==1
finally:
 server.shutdown();script.unlink(missing_ok=True);script.with_suffix('.cs.meta').unlink(missing_ok=True)
