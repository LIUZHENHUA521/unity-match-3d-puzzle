mergeInto(LibraryManager.library, {
 $GooseInvites: {target:'',inviter:'',emit:function(data){Module.SendMessage(GooseInvites.target,'OnInvitationBridge',JSON.stringify(data));},capture:function(options){var value=options&&options.query&&options.query.inviter;if(typeof value==='string'&&/^[a-zA-Z0-9_-]{16,80}$/.test(value))GooseInvites.inviter=value;}},
 GooseInvitesInit__deps:['$GooseInvites'],
 GooseInvitesInit:function(target){GooseInvites.target=UTF8ToString(target);if(typeof wx==='undefined')return;GooseInvites.capture(wx.getLaunchOptionsSync());if(!GooseInvites.listener){GooseInvites.listener=function(o){GooseInvites.capture(o);GooseInvites.emit({refresh:true});};wx.onShow(GooseInvites.listener);}},
 GooseInvitesLogin__deps:['$GooseInvites'],
 GooseInvitesLogin:function(){if(typeof wx==='undefined'){GooseInvites.emit({error:'请在微信小游戏中邀请好友'});return;}wx.login({timeout:10000,success:function(r){GooseInvites.emit({code:r.code,inviter:GooseInvites.inviter});},fail:function(){GooseInvites.emit({error:'微信登录失败，请重试'});}});},
 GooseInvitesShare__deps:['$GooseInvites'],
 GooseInvitesShare:function(code){if(typeof wx==='undefined'){GooseInvites.emit({error:'请在微信小游戏中邀请好友'});return;}wx.shareAppMessage({title:'一起来抓大鹅，看看谁能通关！',query:'inviter='+encodeURIComponent(UTF8ToString(code)),fail:function(){GooseInvites.emit({error:'未能打开分享，请重试'});}});}
});
