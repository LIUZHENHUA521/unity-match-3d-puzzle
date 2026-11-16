using System;
using System.Collections;
using System.IO;

public static class PotReplayCache {
    public static string Packed(string path)=>Path.ChangeExtension(path,".packed");
    public static IEnumerator Prepare(string path,Func<bool> pause){
        string packed=Packed(path),temporary=Path.ChangeExtension(path,".packing");
        if(!PotReplayFiles.Exists(packed)){
            try {yield return PotReplayTransfer.PackFile(path,temporary,pause);
                PotReplayFiles.Move(temporary,packed);
            }finally{if(PotReplayFiles.Exists(temporary))PotReplayFiles.Delete(temporary);}
        }
        if(path.EndsWith(".ready",StringComparison.Ordinal)){
            string raw=Path.ChangeExtension(path,".recording");
            if(PotReplayFiles.Exists(raw))PotReplayFiles.Delete(raw);
        }
    }
    public static void DeleteSynced(string owner,string id){
        foreach(var path in PotReplayFiles.List(owner+"_"+id+".*"))PotReplayFiles.Delete(path);
    }
}
