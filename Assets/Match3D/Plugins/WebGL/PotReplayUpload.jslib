mergeInto(LibraryManager.library, {
  $PotReplayNetwork: {task:null,id:null},
  PotReplayUpload__deps: ['$PotReplayNetwork'],
  PotReplayUpload: function(json) {
    var r=JSON.parse(UTF8ToString(json)),finished=false;
    function done(ok,error) {if(finished)return;finished=true;if(PotReplayNetwork.id===r.id){PotReplayNetwork.task=null;PotReplayNetwork.id=null;}Module.SendMessage(r.receiver,'OnPotReplayUpload',JSON.stringify({id:r.id,ok:ok,error:error||''}));}
    try {
      var name=(r.path||'').split('/').pop();if(!/^[a-zA-Z0-9_.-]+$/.test(name))throw new Error('invalid replay upload filename');
      if(!/^https:\/\//.test(r.ticket.url))throw new Error('invalid replay upload URL');
      var fs=wx.getFileSystemManager(),path=wx.env.USER_DATA_PATH+'/pot-replays-v1/'+name;
      if(fs.statSync(path).size>16777216)throw new Error('compressed replay too large');
      // Only the compressed ciphertext enters wx.request. Raw replay JSON never crosses WASM/JS.
      PotReplayNetwork.id=r.id;PotReplayNetwork.task=wx.request({url:r.ticket.url,method:'PUT',data:fs.readFileSync(path),timeout:180000,
        header:{'Content-Type':'application/octet-stream',Signature:r.ticket.authorization,Authorization:r.ticket.authorization,'x-cos-security-token':r.ticket.token,'x-cos-meta-fileid':r.ticket.cosFileId,key:encodeURIComponent(r.ticket.cloudPath)},
        success:function(res){done(res.statusCode>=200&&res.statusCode<300,'云存储返回 '+res.statusCode);},
        fail:function(){done(false,'云存储上传中断，录像已保留');}
      });
    } catch(e) {done(false,String(e.message||e));}
  },
  PotReplayUploadCancel__deps: ['$PotReplayNetwork'],
  PotReplayUploadCancel: function() {var task=PotReplayNetwork.task;PotReplayNetwork.task=null;if(task)task.abort();}
});
