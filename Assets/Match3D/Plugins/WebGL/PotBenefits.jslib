mergeInto(LibraryManager.library,{
 $PotBenefitsTarget:{name:''},
 PotReadBenefits__deps:['$PotBenefitsTarget'],
 PotReadBenefits:function(targetPtr,club,requestId){
  var target=UTF8ToString(targetPtr),result={requestId:requestId},sent=false;
  PotBenefitsTarget.name=target;
  function done(){if(!sent){sent=true;Module.SendMessage(target,'OnPotBenefits',JSON.stringify(result));}}
  try{
   if(typeof wx==='undefined'){done();return;}
   if(club){
    if(!wx.getGameClubData){done();return;}
    function read(){wx.getGameClubData({dataTypeList:[{type:4},{type:5},{type:6}],success:function(r){result.iv=r.iv;result.encryptedData=r.encryptedData;},complete:done});}
    if(wx.authorize)wx.authorize({scope:"scope.gameClubData",success:read,fail:done});else read();
   }else{
    var options=wx.getEnterOptionsSync?wx.getEnterOptionsSync():wx.getLaunchOptionsSync();
    result.scene=Number(options.scene);
    if(wx.checkIsAddedToMyMiniProgram)wx.checkIsAddedToMyMiniProgram({success:function(r){result.added=r.added===true;},complete:done});else done();
   }
  }catch(e){done();}
 }
});
