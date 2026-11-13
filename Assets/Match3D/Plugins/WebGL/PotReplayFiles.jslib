mergeInto(LibraryManager.library, {
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
        result.text = wx.arrayBufferToBase64(read.arrayBuffer.slice(0,read.bytesRead));
      }
      else if (r.action === 'writeBytes') {
        if (r.text.length > 43692) throw new Error('invalid replay write size');
        var bytes = wx.base64ToArrayBuffer(r.text);
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
