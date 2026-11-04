using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
// A real native ClientWebSocket against a second independent Node client.
public static class Match3DNetworkValidation {
    static Match3DNetwork network;static Action<Match3DMessage> original;static bool started,moved,ended,readied;static double deadline,due;static int step;static string report;
    public static void Run(){
        if(!EditorApplication.isPlaying)throw new Exception("Enter Play mode first");
        Match3DFlow.Instance.ShowPvp();network=Match3DFlow.Instance.GetComponent<Match3DNetwork>();original=network.Received;started=moved=ended=readied=false;step=0;report="";
        network.Received=m=>{original(m);if(m.type=="room"&&m.players.Length==2&&!readied){readied=true;network.Send(new Match3DMessage{type="ready"});}if(m.type=="start")started=true;if(m.type=="move"&&m.playerId==network.PlayerId)moved=true;if(m.type=="result")ended=true;};
        deadline=EditorApplication.timeSinceStartup+35;due=0;EditorApplication.update+=Tick;network.Connect(Match3DFlow.Instance.library.pvpEndpoint,"match");
    }
    static void Tick(){
        try{
            if(EditorApplication.timeSinceStartup>deadline)throw new Exception("Native PVP timed out at step "+step);
            if(EditorApplication.timeSinceStartup<due)return;
            if(step==0&&started){if(LevelGenerator.Instance.objectsOnScene.Count!=96)throw new Exception("Expected 96 items");report+="PASS native guest login, matchmaking, prepare/load/start with 96 items\n";InputController.Instance.AddObjectOnStack(LevelGenerator.Instance.objectsOnScene.First());step=1;}
            else if(step==1&&moved){due=EditorApplication.timeSinceStartup+1;step=2;}
            else if(step==2){if(InputController.Instance.objectsOnStack.Count!=1)throw new Exception("Server echo did not apply pick");report+="PASS server-authorized pick reaches Unity stack\n";step=3;}
            else if(step==3&&ended&&!string.IsNullOrEmpty(Match3DFlow.Instance.LastResult)){if(Match3DFlow.Instance.LastResult!="对战胜利！")throw new Exception("Wrong result "+Match3DFlow.Instance.LastResult);report+="PASS peer surrender produces Unity victory screen\n";Finish(null);}
        }catch(Exception e){Finish(e);}
    }
    static void Finish(Exception e){EditorApplication.update-=Tick;if(network)network.Received=original;File.WriteAllText("Temp/goose-network-validation.txt",report+(e==null?"ALL CHECKS PASSED":"FAIL "+e));}
}
