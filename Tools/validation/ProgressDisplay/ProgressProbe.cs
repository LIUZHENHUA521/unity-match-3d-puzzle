using System;
using System.IO;
using UnityEditor;
public static class ProgressProbe {
 static int count;
 static void Check(bool ok,string name){if(!ok)throw new Exception(name);count++;}
 static bool Near(float a,float b)=>Math.Abs(a-b)<.00001f;
 public static void Run(){try{
  var p=new Match3DProgressDisplay();
  string[] groups={"A","B","C","D"};float[] expected={.2f,1f/3f,.2f/.76f,.2f/.44f};
  for(int i=0;i<4;i++){
   p.BeginRun(groups[i]);Check(Near(p.Sample(100,80,80),expected[i]),"four group formula "+groups[i]);
   Check(Near(p.Sample(100,80,0),expected[i]),"reveal cannot decrease "+groups[i]);
   p.ResetStage();Check(p.Value==0&&p.Group==groups[i],"retry resets progress preserves assignment");
   Check(Near(p.Sample(100,80,0),.2f),"tray/storage remain full weight");
   Check(p.Sample(100,0,0)==.99f,"not won capped99");Check(p.Sample(100,0,0,true)==1,"win100");
  }
  p.BeginRun("D");Check(p.Sample(100,100,100)==0,"all hidden before removal still0");
  Check(p.Sample(0,0,0)==0,"empty invalid board stays0");
  Check(p.Sample(100,120,-10)==0,"bad counts clamp");
  p.BeginRun(null);Check(p.Group=="A"&&Near(p.Sample(100,80,80),.2f),"missing assignment defaults real");
  p.BeginRun("unknown");Check(p.Group=="A","unknown group defaults real");
  Check(p.BestKey("food",2)=="goose.result.best.food.2","real preserves legacy best");
  var keys=new System.Collections.Generic.HashSet<string>();foreach(var g in groups){p.BeginRun(g);keys.Add(p.BestKey("food",2));}
  Check(keys.Count==4,"algorithm bests isolated");
  File.WriteAllText(Path.GetFullPath("../result.txt"),"ALL PASSED: "+count);EditorApplication.Exit(0);
 }catch(Exception e){File.WriteAllText(Path.GetFullPath("../result.txt"),e.ToString());EditorApplication.Exit(1);}}
}
