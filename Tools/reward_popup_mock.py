#!/usr/bin/env python3
"""Local-only fixture for Match3DRewardPopupValidation. No production services or grants."""
import json
from http.server import BaseHTTPRequestHandler, HTTPServer

inventory = [0, 0, 0, 0]
claimed = False

def state():
    return dict(token='reward-popup-fixture', inviteCode='reward-popup-fixture', invited=0,
                inventory=inventory, nextCursor=-1, attention=not claimed,
                mails=[dict(id='fixture-tools', title='道具奖励', body='四种道具一起领取',
                            sentAt=1794268800000, expiresAt=1994268800000,
                            rewards=[2, 3, 1, 4], read=True, claimed=claimed, expired=False),
                       dict(id='fixture-failed', title='失败测试', body='模拟领取失败',
                            sentAt=1794268800000, rewards=[1, 0, 0, 0], claimed=False, expired=False)])

class Handler(BaseHTTPRequestHandler):
    def do_POST(self):
        global claimed, inventory
        body = json.loads(self.rfile.read(int(self.headers.get('Content-Length', 0))) or '{}')
        status = 200
        if self.path == '/invites/login':
            inventory, claimed = [0, 0, 0, 0], False
            result = state()
        elif self.path == '/invites/mail/claim':
            if body.get('id') == 'fixture-failed':
                status, result = 409, dict(error='fixture-failure')
            else:
                if not claimed:
                    inventory = [2, 3, 1, 4]
                    claimed = True
                result = state()
        elif self.path in ('/invites/mail/list', '/invites/mail/read'):
            result = state()
        elif self.path == '/rewards/list':
            result = dict(ok=True, nextCursor=-1, vouchers=[dict(
                id='fixture-voucher', money=10, createTime=1794268800000, status='available')])
        else:
            status, result = 404, dict(error='unknown-fixture-route')
        data = json.dumps(result, ensure_ascii=False).encode()
        self.send_response(status)
        self.send_header('Content-Type', 'application/json')
        self.send_header('Content-Length', str(len(data)))
        self.end_headers()
        self.wfile.write(data)
    def log_message(self, *args):
        pass

if __name__ == '__main__':
    print('Reward popup fixture listening on 127.0.0.1:8107', flush=True)
    HTTPServer(('127.0.0.1', 8107), Handler).serve_forever()
