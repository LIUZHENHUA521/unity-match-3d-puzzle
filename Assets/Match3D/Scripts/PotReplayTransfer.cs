using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;

// The upload ticket is scoped to one encrypted object; no CloudBase login secrets enter the client.
public static class PotReplayTransfer {
    public const int MaxFileBytes=16*1024*1024;
    public static byte[] Pack(string json,string key,string iv){
        byte[] bytes=Encoding.UTF8.GetBytes(json);
        if(bytes.Length>PotReplayService.MaxJsonBytes)throw new IOException("录像展开大小超过64MiB");
        using(var output=new MemoryStream()){
            using(var aes=new AesManaged()){
                aes.Key=Convert.FromBase64String(key);aes.IV=Convert.FromBase64String(iv);
                aes.Mode=CipherMode.CBC;aes.Padding=PaddingMode.PKCS7;
                using(var cipher=new CryptoStream(output,aes.CreateEncryptor(),CryptoStreamMode.Write,true))
                using(var gzip=new GZipStream(cipher,CompressionLevel.Fastest))gzip.Write(bytes,0,bytes.Length);
            }
            if(output.Length>MaxFileBytes)throw new IOException("录像压缩大小超过16MiB");
            return output.ToArray();
        }
    }
    public static string Unpack(byte[] bytes){
        using(var input=new MemoryStream(bytes))using(var gzip=new GZipStream(input,CompressionMode.Decompress))using(var output=new MemoryStream()){
            var buffer=new byte[32768];int count;
            while((count=gzip.Read(buffer,0,buffer.Length))>0){if(output.Length+count>PotReplayService.MaxJsonBytes)throw new IOException("录像展开大小超过64MiB");output.Write(buffer,0,count);}
            return Encoding.UTF8.GetString(output.ToArray());
        }
    }
}
