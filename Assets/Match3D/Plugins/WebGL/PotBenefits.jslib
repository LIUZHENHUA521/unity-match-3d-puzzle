mergeInto(LibraryManager.library,{
 $PotBenefitsTarget:{name:''},
 PotReadBenefits__deps:['$PotBenefitsTarget'],
 PotReadBenefits:function(targetPtr,club,requestId){
  var target=UTF8ToString(targetPtr),result={requestId:requestId},sent=false;
  PotBenefitsTarget.name=target;
  function done(){if(!sent){sent=true;Module.SendMessage(target,'OnPotBenefits',JSON.stringify(result));}}
  function failed(stage,error){
   var message=String(error && (error.errMsg||error.message)||'').slice(0,240);
   console.warn('[PotBenefits]',stage,message);
   result.error=stage;
   if(/privacy|privacy_api/i.test(message))result.error='privacy';
   else if(/auth deny|auth denied|authorize:fail auth|permission denied/i.test(message))result.error='denied';
   done();
  }
  function readClub(){
   try{
    wx.getGameClubData({
     dataTypeList:[{type:1},{type:4},{type:5},{type:6}],
     success:function(r){
      if(!r||!r.encryptedData||!r.iv){failed('empty');return;}
      result.iv=r.iv;result.encryptedData=r.encryptedData;done();
     },
     fail:function(e){failed('getGameClubData',e);}
    });
   }catch(e){failed('getGameClubData',e);}
  }
  try{
   if(typeof wx==='undefined'){failed('unsupported');return;}
   if(club){
    if(typeof wx.getGameClubData!=='function'){failed('unsupported');return;}
    if(typeof wx.requirePrivacyAuthorize==='function'){
     wx.requirePrivacyAuthorize({success:readClub,fail:function(e){failed('privacy',e);}});
    }else readClub();
   }else{
    var options=wx.getEnterOptionsSync?wx.getEnterOptionsSync():wx.getLaunchOptionsSync();
    result.scene=Number(options.scene);
    if(wx.checkIsAddedToMyMiniProgram)wx.checkIsAddedToMyMiniProgram({success:function(r){result.added=r.added===true;},complete:done});else done();
   }
  }catch(e){failed('bridge',e);}
 }
});
