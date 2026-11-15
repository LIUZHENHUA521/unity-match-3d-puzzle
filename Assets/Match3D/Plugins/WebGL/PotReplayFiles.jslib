mergeInto(LibraryManager.library, {
  $PotReplayBase64: {
    alphabet: 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/',
    encode: function(buffer) {
      var bytes=new Uint8Array(buffer), alphabet=PotReplayBase64.alphabet, out='';
      for(var i=0;i<bytes.length;i+=3){
        var a=bytes[i], b=i+1<bytes.length?bytes[i+1]:0, c=i+2<bytes.length?bytes[i+2]:0;
        out+=alphabet[a>>2]+alphabet[((a&3)<<4)|(b>>4)]+(i+1<bytes.length?alphabet[((b&15)<<2)|(c>>6)]:'=')+(i+2<bytes.length?alphabet[c&63]:'=');
      }
      return out;
    },
    decode: function(text) {
      if(text.length%4||!/^(?:[A-Za-z0-9+/]{4})*(?:[A-Za-z0-9+/]{2}==|[A-Za-z0-9+/]{3}=)?$/.test(text))throw new Error('invalid replay base64');
      var padding=text.endsWith('==')?2:text.endsWith('=')?1:0;
      var bytes=new Uint8Array(text.length/4*3-padding), alphabet=PotReplayBase64.alphabet, offset=0;
      for(var i=0;i<text.length;i+=4){
        var a=alphabet.indexOf(text[i]), b=alphabet.indexOf(text[i+1]), c=alphabet.indexOf(text[i+2]), d=alphabet.indexOf(text[i+3]);
        bytes[offset++]=(a<<2)|(b>>4);
        if(offset<bytes.length)bytes[offset++]=((b&15)<<4)|(c>>2);
        if(offset<bytes.length)bytes[offset++]=((c&3)<<6)|d;
      }
      return bytes.buffer;
    }
  },
  PotReplayFile__deps: ['$PotReplayBase64'],
  PotReplayFile: function (json) {
    var result;
    try {
      var r = JSON.parse(UTF8ToString(json));
      var fs = wx.getFileSystemManager(), root = wx.env.USER_DATA_PATH + '/pot-replays-v1';
      var path = function (p) { var name = (p || '').split('/').pop(); if (!/^[a-zA-Z0-9_.-]+$/.test(name)) throw new Error('invalid replay filename'); return root + '/' + name; };
      result = { ok: true };
      if (r.action === 'mkdir') { try { fs.accessSync(root); } catch (_) { fs.mkdirSync(root, true); } }
      else if (r.action === 'list') result.files = fs.readdirSync(root);
      else if (r.action === 'write') fs.writeFileSync(path(r.path), r.text, 'utf8');
      else if (r.action === 'append') fs.appendFileSync(path(r.path), r.text, 'utf8');
      else if (r.action === 'read') result.text = fs.readFileSync(path(r.path), 'utf8');
      else if (r.action === 'open') result.text = fs.openSync({filePath:path(r.path),flag:r.target});
      else if (r.action === 'close') fs.closeSync({fd:r.fd});
      else if (r.action === 'readBytes') {
        if (!Number.isInteger(r.length) || r.length < 1 || r.length > 32768) throw new Error('invalid replay read size');
        var read = fs.readSync({fd:r.fd,arrayBuffer:new ArrayBuffer(r.length),offset:0,length:r.length,position:r.position});
        result.text = PotReplayBase64.encode(read.arrayBuffer.slice(0,read.bytesRead));
      }
      else if (r.action === 'writeBytes') {
        if (r.text.length > 43692) throw new Error('invalid replay write size');
        var bytes = PotReplayBase64.decode(r.text);
        result.size = fs.writeSync({fd:r.fd,data:bytes,offset:0,length:bytes.byteLength,position:r.position}).bytesWritten;
      }
      else if (r.action === 'exists') { try { fs.accessSync(path(r.path)); result.exists = true; } catch (_) { result.exists = false; } }
      else if (r.action === 'delete') fs.unlinkSync(path(r.path));
      else if (r.action === 'move') fs.renameSync(path(r.path), path(r.target));
      else if (r.action === 'size') result.size = fs.statSync(path(r.path)).size;
      else throw new Error('invalid replay file action');
    } catch (error) { result = { ok: false, error: String(error.message || error) }; }
    var text = JSON.stringify(result), length = lengthBytesUTF8(text) + 1, pointer = _malloc(length);
    stringToUTF8(text, pointer, length); return pointer;
  }
});
