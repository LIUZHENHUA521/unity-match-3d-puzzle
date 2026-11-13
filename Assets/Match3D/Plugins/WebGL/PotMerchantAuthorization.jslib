mergeInto(LibraryManager.library, {
 GooseMerchantAuthorizationSupported:function(){return typeof wx!=='undefined'&&typeof wx.requestMerchantTransfer==='function'?1:0;},
 GooseMerchantAuthorizationOpen:function(target,json){
  var receiver=UTF8ToString(target),data=JSON.parse(UTF8ToString(json)),finished=false;
  function done(kind){if(finished)return;finished=true;Module.SendMessage(receiver,'OnMerchantAuthorizationBridge',JSON.stringify({requestId:data.requestId,kind:kind}));}
  if(typeof wx==='undefined'||typeof wx.requestMerchantTransfer!=='function'){done('unsupported');return;}
  try{
   if(!data.mchid||!data.appid||!data.packageInfo){done('failed');return;}
   var current=wx.getAccountInfoSync&&wx.getAccountInfoSync();
   if(!current||!current.miniProgram||current.miniProgram.appId!==data.appid){done('failed');return;}
   wx.requestMerchantTransfer({mchId:data.mchid,appId:data.appid,package:data.packageInfo,
    success:function(){done('returned');},
    fail:function(error){done(/cancel/i.test(String(error&&error.errMsg||''))?'cancelled':'failed');},
    cancel:function(){done('cancelled');}});
  }catch(error){done('failed');}
 }
});
