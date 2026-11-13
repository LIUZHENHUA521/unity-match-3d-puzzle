mergeInto(LibraryManager.library, {
 $GooseShareCard: {
  titles: [
   '这一锅不拼手速，拼的是眼力和取舍，敢来试试吗？',
   '名字叫一锅消不完，你能替我争口气吗？😂',
   '看着挺简单，一消就上头！这一锅你能消完吗？'
  ],
  create: function(query,fail){
   var options={
    title:GooseShareCard.titles[Math.floor(Math.random()*GooseShareCard.titles.length)],
    imageUrl:'https://mmocgame.qpic.cn/wechatgame/OwVPicIFUBQ9L8hmSjicNSfv0sW7zyTmEfPZ25bUTUMJMAksojPuD9ViaOhoWZRawC4/0',
    imageUrlId:'Mg3EGffMTCuIXpg24LrIIw==',
    query:query||''
   };
   if(fail)options.fail=fail;
   return options;
  }
 },
 GooseShareResult__deps:['$GooseShareCard'],
 GooseShareResult:function(queryPtr){
  if(typeof wx!=='undefined'&&wx.shareAppMessage)wx.shareAppMessage(GooseShareCard.create(UTF8ToString(queryPtr)));
 }
});
