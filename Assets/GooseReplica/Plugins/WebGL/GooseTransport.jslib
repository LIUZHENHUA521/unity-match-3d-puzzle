mergeInto(LibraryManager.library, {
  $GooseTransport: {
    clients: {},
    emit: function(c, kind, data) {
      if (GooseTransport.clients[c.target] !== c) return;
      Module.SendMessage(c.target, 'OnGooseTransportMessage', JSON.stringify({generation:c.generation, kind:kind, data:data || ''}));
    },
    fail: function(c, kind, text) {
      if (c.failed) return;
      c.failed = true;
      GooseTransport.emit(c, kind, text);
    },
    close: function(target) {
      var c = GooseTransport.clients[target];
      delete GooseTransport.clients[target];
      if (!c) return;
      if (c.request && c.request.abort) c.request.abort();
      if (c.socket) { try { c.socket.close(); } catch(e) {} }
    },
    login: function(c, auth) {
      if (GooseTransport.clients[c.target] !== c) return;
      if (!auth || !auth.token || !auth.playerId) {
        GooseTransport.fail(c, 'error', '对战登录失败，请重试'); return;
      }
      GooseTransport.emit(c, 'auth', auth.playerId);
      var url=c.endpoint.replace(/^http/, 'ws')+'/ws?token='+encodeURIComponent(auth.token);
      var open=function() {
        if(GooseTransport.clients[c.target] !== c)return;
        GooseTransport.emit(c, 'open', '');
        GooseTransport.send(c, JSON.stringify({type:c.intent,roomId:c.room}));
      };
      var message=function(e) {
        if (typeof e.data !== 'string' || e.data.length>65536) {
          GooseTransport.fail(c, 'error', '对战消息格式错误'); return;
        }
        GooseTransport.emit(c, 'message', e.data);
      };
      var close=function(){GooseTransport.fail(c,'disconnected','对战连接已断开');};
      var error=function(){GooseTransport.fail(c,'disconnected','对战连接失败，请重新匹配');};
      try {
        if(c.wechat) {
          c.socket=wx.connectSocket({url:url, fail:error});
          c.socket.onOpen(open); c.socket.onMessage(message);
          c.socket.onClose(close); c.socket.onError(error);
        } else {
          c.socket=new WebSocket(url);
          c.socket.onopen=open; c.socket.onmessage=message;
          c.socket.onclose=close; c.socket.onerror=error;
        }
      } catch(e) { error(); }
    },
    send: function(c, data) {
      if(!c || !c.socket || c.failed)return;
      try {
        if(c.wechat)c.socket.send({data:data,fail:function(){GooseTransport.fail(c,'disconnected','发送失败，请重新匹配');}});
        else c.socket.send(data);
      } catch(e) { GooseTransport.fail(c,'disconnected','发送失败，请重新匹配'); }
    }
  },
  GooseConnect__deps: ['$GooseTransport'],
  GooseConnect: function(target, endpoint, intent, room, generation) {
    target=UTF8ToString(target);
    GooseTransport.close(target);
    var c={target:target, endpoint:UTF8ToString(endpoint), intent:UTF8ToString(intent), room:UTF8ToString(room), generation:generation, wechat:typeof wx!=='undefined' && !!wx.connectSocket};
    GooseTransport.clients[target]=c;
    var fail=function(){GooseTransport.fail(c,'error','无法连接对战服务，请检查网络');};
    if(c.wechat) {
      c.request=wx.request({url:c.endpoint+'/session', method:'POST', timeout:10000,
        success:function(r){if(r.statusCode===200)GooseTransport.login(c,r.data);else fail();}, fail:fail});
    } else {
      c.request=new XMLHttpRequest();
      c.request.open('POST',c.endpoint+'/session');c.request.timeout=10000;
      c.request.onload=function(){try{if(c.request.status===200)GooseTransport.login(c,JSON.parse(c.request.responseText));else fail();}catch(e){fail();}};
      c.request.onerror=fail;c.request.ontimeout=fail;c.request.send();
    }
  },
  GooseSend__deps: ['$GooseTransport'],
  GooseSend: function(target, message) { GooseTransport.send(GooseTransport.clients[UTF8ToString(target)],UTF8ToString(message)); },
  GooseDisconnect__deps: ['$GooseTransport'],
  GooseDisconnect: function(target) { GooseTransport.close(UTF8ToString(target)); }
});
