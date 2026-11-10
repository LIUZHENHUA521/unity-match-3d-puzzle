from http.server import BaseHTTPRequestHandler,HTTPServer
import json
class Handler(BaseHTTPRequestHandler):
 calls=0
 def do_POST(self):
  body=json.loads(self.rfile.read(int(self.headers['Content-Length'])))
  assert body['type']==3
  assert self.headers.get('X-Goose-Tool-Count')=='4'
  if self.path=='/invites/consume':
   Handler.calls+=1
   if Handler.calls==2:
    self.send_response(409);self.end_headers();self.wfile.write(b'{}');return
  data={'inventory':[0,0,0,1],'requestId':body['requestId'],'inviteCode':'add-time-test','refunded':self.path.endswith('/refund')}
  self.send_response(200);self.send_header('Content-Type','application/json');self.end_headers();self.wfile.write(json.dumps(data).encode())
 def log_message(self,*args):pass
HTTPServer(('127.0.0.1',18765),Handler).serve_forever()
