from http.server import BaseHTTPRequestHandler,HTTPServer
import json
class Handler(BaseHTTPRequestHandler):
 def do_POST(self):
  body=json.loads(self.rfile.read(int(self.headers['Content-Length'])))
  if self.path!='/invites/consume': self.send_error(404); return
  data={'inventory':[0,0,1],'requestId':body['requestId'],'inviteCode':'tool-test','refunded':False}
  self.send_response(200);self.send_header('Content-Type','application/json');self.end_headers();self.wfile.write(json.dumps(data).encode())
 def log_message(self,*args): pass
HTTPServer(('127.0.0.1',18764),Handler).serve_forever()
