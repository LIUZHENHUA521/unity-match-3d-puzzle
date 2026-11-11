mergeInto(LibraryManager.library, {
 $GooseProfile: {
  target:'', generation:0, readVersion:0, button:null,
  emit:function(version,data){if(version===GooseProfile.generation&&GooseProfile.target)Module.SendMessage(GooseProfile.target,'OnWechatProfile',JSON.stringify(data));},
  close:function(){GooseProfile.generation++;GooseProfile.target='';if(GooseProfile.button){try{GooseProfile.button.destroy();}catch(e){}GooseProfile.button=null;}},
  result:function(version,res){
   if(res&&res.userInfo&&!(res.errMsg&&res.errMsg.indexOf(':fail')>=0)){
    var u=res.userInfo;GooseProfile.emit(version,{state:'ready',nickName:typeof u.nickName==='string'?u.nickName:'',avatarUrl:typeof u.avatarUrl==='string'?u.avatarUrl:'',gender:Number(u.gender)||0,country:u.country||'',province:u.province||'',city:u.city||''});
   }else GooseProfile.emit(version,{state:'denied',message:'未获取微信资料，可点击头像旁的相机重试'});
  }
 },
 GooseProfileOpen__deps:['$GooseProfile'],
 GooseProfileOpen:function(target){
  GooseProfile.close();GooseProfile.target=UTF8ToString(target);var version=GooseProfile.generation,readVersion=++GooseProfile.readVersion;
  if(typeof wx==='undefined'){GooseProfile.emit(version,{state:'unavailable',message:'请在微信小游戏中同步资料'});return;}
  try{wx.getSetting({success:function(r){if(version!==GooseProfile.generation||readVersion!==GooseProfile.readVersion)return;
   if(r.authSetting&&r.authSetting['scope.userInfo']){
    if(typeof wx.getUserInfo!=='function'){GooseProfile.emit(version,{state:'unavailable',message:'当前微信暂不支持读取资料'});return;}
    wx.getUserInfo({withCredentials:false,lang:'zh_CN',success:function(res){if(readVersion===GooseProfile.readVersion)GooseProfile.result(version,res);},fail:function(){if(readVersion!==GooseProfile.readVersion)return;GooseProfile.emit(version,{state:'denied',message:'微信资料读取失败，请点击相机重试'});}});
   }else GooseProfile.emit(version,{state:'authorize',message:'点击头像旁的相机，由微信确认授权'});
  },fail:function(){if(readVersion!==GooseProfile.readVersion)return;GooseProfile.emit(version,{state:'authorize',message:'暂未读取到授权状态，可点击相机重试'});}});}catch(e){GooseProfile.emit(version,{state:'unavailable',message:'当前微信暂不支持读取资料'});}
 },
 GooseProfileBounds__deps:['$GooseProfile'],
 GooseProfileBounds:function(x,y,w,h,screenW,screenH){
  if(!GooseProfile.target||typeof wx==='undefined'||w<=0||h<=0||screenW<=0||screenH<=0)return;
  var version=GooseProfile.generation;
  try{
   var info=wx.getWindowInfo?wx.getWindowInfo():wx.getSystemInfoSync();
   var sx=info.windowWidth/screenW,sy=info.windowHeight/screenH;
   if(!(sx>0&&sy>0))return;
   var style={left:x*sx,top:y*sy,width:w*sx,height:h*sy,lineHeight:h*sy,backgroundColor:'rgba(0, 0, 0, 0)',color:'rgba(0, 0, 0, 0)',fontSize:0,borderWidth:0};
   if(GooseProfile.button){for(var k in style)GooseProfile.button.style[k]=style[k];return;}
   if(!wx.createUserInfoButton){GooseProfile.emit(version,{state:'unavailable',message:'当前微信暂不支持资料授权'});return;}
   var button=wx.createUserInfoButton({type:'text',text:'',lang:'zh_CN',withCredentials:false,style:style});
   GooseProfile.button=button;button.onTap(function(res){if(version!==GooseProfile.generation)return;GooseProfile.readVersion++;GooseProfile.result(version,res);});
  }catch(e){GooseProfile.emit(version,{state:'unavailable',message:'微信授权按钮暂不可用，请稍后重试'});}
 },
 GooseProfileClose__deps:['$GooseProfile'],GooseProfileClose:function(){GooseProfile.close();},
 GooseProfileCopy__deps:['$GooseProfile'],GooseProfileCopy:function(value){
  var text=UTF8ToString(value),version=GooseProfile.generation;
  if(typeof wx==='undefined')return;
  wx.setClipboardData({data:text,success:function(){GooseProfile.emit(version,{state:'copied',message:'用户 ID 已复制'});},fail:function(){GooseProfile.emit(version,{state:'copyFailed',message:'复制失败，请重试'});}});
 },
 GooseCustomerServiceOpen:function(link){
  var url=link?UTF8ToString(link):'',corpId='ww18ea30e9ed7eee49';
  if(!/^https:\/\/work\.weixin\.qq\.com\/kf\/[^?#]+\?/.test(url)||url.indexOf('enc_scene=')<0||url.indexOf('scene_param=')<0){
   if(typeof wx!=='undefined'&&typeof wx.showToast==='function')wx.showToast({title:'客服连接已失效，请返回游戏重试',icon:'none'});
   return;
  }
  if(typeof wx==='undefined'){
   if(typeof window!=='undefined'&&typeof window.alert==='function')window.alert('请在微信小游戏中联系客服');
   return;
  }
  var prompted=false;
  function toast(message){if(typeof wx.showToast==='function')wx.showToast({title:message,icon:'none'});}
  function fallback(){
   if(prompted)return;prompted=true;
   if(typeof wx.showModal!=='function'){toast('客服打开失败，请稍后重试');return;}
   wx.showModal({title:'暂时无法打开客服',content:'可以复制一锅消不完客服链接，在微信中打开。',confirmText:'复制链接',
    success:function(res){
     if(!res.confirm)return;
     if(typeof wx.setClipboardData!=='function'){toast('复制失败，请稍后重试');return;}
     try{wx.setClipboardData({data:url,success:function(){toast('客服链接已复制');},fail:function(){toast('复制失败，请稍后重试');}});}catch(e){toast('复制失败，请稍后重试');}
    }
   });
  }
  if(typeof wx.openCustomerServiceChat!=='function'){fallback();return;}
  try{wx.openCustomerServiceChat({extInfo:{url:url},corpId:corpId,fail:function(error){
   if(error&&/cancel/i.test(error.errMsg||''))return;
   fallback();
  }});}catch(e){fallback();}
 }
});
