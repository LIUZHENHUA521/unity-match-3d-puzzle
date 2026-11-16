from pathlib import Path
import json, gzip, hashlib, subprocess, threading, time, shutil, argparse
parser=argparse.ArgumentParser()
parser.add_argument("--replay",type=Path,required=True,help="A compatible local gzip replay fixture")
args=parser.parse_args()
from http.server import ThreadingHTTPServer, BaseHTTPRequestHandler
ROOT=Path(__file__).resolve().parents[3]
STAGE=ROOT/'work/pot-master-replay/project'
OUT=STAGE/'work/pot-master-replay'
OUT.mkdir(parents=True,exist_ok=True)
for name in ('Assets','Packages','ProjectSettings'):
 subprocess.run(['rsync','-a','--delete',str(ROOT/name)+'/',str(STAGE/name)+'/'],check=True)
shutil.copy(Path(__file__).with_name('PotMasterReplayIntegration.cs'),STAGE/'Assets/Match3D/Editor/PotMasterReplayIntegration.cs')
source=args.replay
recording=json.loads(gzip.decompress(source.read_bytes()))
day=time.strftime('%Y-%m-%d',time.gmtime(time.time()+8*3600))
recording['status']='win';recording['level'].update(date=day,tier='bachelor',version='pot-fixture')
payload=gzip.compress(json.dumps(recording).encode())
requests=[]
class Handler(BaseHTTPRequestHandler):
 def log_message(self,*args): pass
 def do_POST(self):
  body=json.loads(self.rfile.read(int(self.headers['Content-Length'])))
  mode=(OUT/'mode').read_text() if (OUT/'mode').exists() else 'five'
  requests.append({'path':self.path,'mode':mode,'body':body})
  status=200;response={'ok':True}
  if self.path.endswith('/daily'):response={'ok':True,'level':recording['level']}
  elif self.path.endswith('/list'):
   if body.get('kind')!='pot-masters':response={'ok':True,'list':[]}
   elif mode=='error':status=503;response={'ok':False,'error':'fixture unavailable'}
   else:
    total=5 if mode=='five' else 11
    rows=[{'id':('a'*64 if i==0 else format(i,'064x')),'status':'win','stage':2,'date':day,'tier':'bachelor','levelVersion':'pot-fixture','resourceVersion':'pot-replay-resources-2','nickName':'挑战者'+str(i+1),'duration':150+i} for i in range(min(total,10))]
    response={'ok':True,'date':day,'levelVersion':('pot-replaced' if mode=='version' else 'pot-fixture'),'total':total,'list':rows}
  elif self.path.endswith('/download-start'):
   time.sleep(.8)
   if mode=='success':response={'ok':True,'download':{'url':f'http://127.0.0.1:{self.server.server_port}/file','format':'gzip-v1','sha256':hashlib.sha256(payload).hexdigest()}}
   else:status=503;response={'ok':False,'error':'fixture download failed'}
  else:status=404;response={'ok':False,'error':'fixture unknown route'}
  data=json.dumps(response).encode();self.send_response(status);self.send_header('Content-Type','application/json');self.send_header('Content-Length',str(len(data)));self.end_headers()
  try:self.wfile.write(data)
  except (BrokenPipeError,ConnectionResetError):pass
 def do_GET(self):
  self.send_response(200);self.send_header('Content-Length',str(len(payload)));self.end_headers();self.wfile.write(payload)
server=ThreadingHTTPServer(('127.0.0.1',0),Handler)
threading.Thread(target=server.serve_forever,daemon=True).start()
(OUT/'endpoint').write_text(f'http://127.0.0.1:{server.server_port}')
try:
 result=subprocess.run(['/Applications/Unity/Unity.app/Contents/MacOS/Unity','-batchmode','-projectPath',str(STAGE),'-executeMethod','PotMasterReplayIntegration.Run','-logFile',str(ROOT/'work/pot-master-replay/integration.log')],timeout=600)
 print((OUT/'integration.txt').read_text() if (OUT/'integration.txt').exists() else 'No report')
 (OUT/'requests.json').write_text(json.dumps(requests,indent=2))
 if result.returncode:raise SystemExit(result.returncode)
 assert (OUT/'integration.txt').read_text().endswith('COMPLETE\n')
finally:server.shutdown()
