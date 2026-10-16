using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class SourceToolsValidation {
    static void Check(bool value,string message){if(!value)throw new Exception(message);}
    public static void Run(){
        var objects=new List<GameObject>();string report="";
        Func<string,GameObject> make=name=>{var obj=new GameObject(name);objects.Add(obj);return obj;};
        try{
            Check((int)ReplicaSourceTools.StoreXEase==3&&(int)ReplicaSourceTools.StoreYEase==26&&(int)ReplicaSourceTools.StoreZEase==27&&(int)ReplicaSourceTools.StoreScaleEase==26,"store ease numeric values match WX enum metadata, including InBack not OutElastic");
            var a=make("A");var a2=make("A");var b=make("B");var c=make("C");
            var stack=new List<GameObject>{a,a2,b,c};
            Check(ReplicaSourceTools.ReturnCandidates(stack).SequenceEqual(new[]{a,a2,b}),"return must take first three slot entries, not last pickup");
            Check(ReplicaSourceTools.ReturnCandidates(new[]{a}).SequenceEqual(new[]{a}),"return fewer than three");
            var b1=make("B");var b2=make("B");var aa=make("AA");var a3=make("A");
            var board=new List<GameObject>{a,a2,b,c,b1,b2,aa,a3};int draws=0;
            var picked=ReplicaSourceTools.AutoFindCandidates(stack,board,(low,high)=>{draws++;Check(low==0&&high==3,"draw distinct tray types once");return 1;});
            Check(draws==1&&picked.SequenceEqual(new[]{b2,b1}),"auto-find uses selected distinct type and reverse board order, not most numerous group");
            Check(ReplicaSourceTools.AutoFindCandidates(new GameObject[0],board).Count==0,"empty tray cannot auto-find");
            Check(!ReplicaSourceTools.CanAutoFind(new GameObject[0]),"empty tray button disabled");
            var six=new List<GameObject>{a,a2,b,c,make("D"),make("E")};
            picked=ReplicaSourceTools.AutoFindCandidates(six,board,(x,y)=>{throw new Exception("six-slot pair path must not draw RNG");});
            Check(picked.SequenceEqual(new[]{a3}),"six-slot pair fills one matching item");
            six[1]=make("F");Check(!ReplicaSourceTools.CanAutoFind(six),"six different types have no safe completion");
            Check(ReplicaSourceTools.AutoFindCandidates(new[]{a},new[]{aa}).Count==0,"matching uses exact type and refuses malformed incomplete triples");
            var sequence=new List<int>{0,1,2,3};int steps=0;
            ReplicaSourceTools.ReorderForShuffle(sequence,(x,y)=>{steps++;Check(x==0&&y==4,"shuffle draw bounds remain fixed");return 0;});
            Check(steps==4&&sequence.SequenceEqual(new[]{0,1,2,3}),"source reorder is N remove-append operations, not Fisher-Yates");
            a.transform.position=new Vector3(0,20,0);b.transform.position=new Vector3(0,13,0);c.transform.position=new Vector3(0,13.01f,0);
            Check(ReplicaSourceTools.ShuffleCandidates(new[]{a,b,c},new GameObject[0]).SequenceEqual(new[]{a,c}),"shuffle strict seven-unit top band");
            Check(ReplicaSourceTools.ShuffleCandidates(new[]{a,b,c},new GameObject[0],19f).SequenceEqual(new[]{a,b,c}),"source historical maxHeight retains a wider band after pile bounces upward");
            Check(Vector3.Distance(ReplicaSourceTools.ShuffleAxis("CountrysideStove"),new Vector3(0,.99254615f,-.12186936f))<.00001f,"breakfast shuffle axis source transform");
            Check(Vector3.Distance(ReplicaSourceTools.ShuffleAxis("GroceryStore"),new Vector3(0,.99357186f,-.11320321f))<.00001f,"shop shuffle axis source transform");
            report="PASS source tools: first-three removal selection; distinct tray-type random completion; six-slot pair/no-RNG path; empty and malformed gates; reverse board search; exact type matching; N remove-append shuffle draws; strict top-seven-unit band and source per-theme axis. Store/animation runtime lifecycle verified separately.\n";
        }catch(Exception e){report="FAIL "+e+"\n";}
        finally{foreach(var obj in objects)if(obj)UnityEngine.Object.DestroyImmediate(obj);File.WriteAllText("Temp/goose-source-tools-validation.txt",report);Debug.Log(report);}
    }
}
