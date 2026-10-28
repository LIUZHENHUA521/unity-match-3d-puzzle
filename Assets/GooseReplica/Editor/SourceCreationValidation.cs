using System;
using System.IO;
using System.Linq;
using UnityEngine;

// Drives the very same suspended iterator used by RunSourceCreation. Timing on real Unity frames
// (including pause and cancellation) is covered separately by SourceCreationFramesValidation.
public static class SourceCreationValidation {
    static void Check(bool value,string message){if(!value)throw new Exception(message);}
    public static void Run(){
        bool auto=Physics.autoSimulation;var flow=ReplicaFlow.Instance;
        try{
            Physics.autoSimulation=false;
            for(int theme=0;theme<3;theme++){
                flow.Begin(theme);
                var pile=ReplicaLayeredPile.Instance;var objects=LevelGenerator.Instance.objectsOnScene;
                Check(flow.CreationInProgress&&pile.Creating,"creation must remain suspended after first batch");
                Check(objects.Count==1,"source zero-based remainder yields after first one item");
                Check(objects[0].GetComponent<Rigidbody>().isKinematic,"Init sets kinematic before creation height checks");
                Check(GameManager.Instance.gameStatus==GameManager.GameStatus.PREPARING,"no input/countdown while creating");
                var steps=flow.TakeOverSourceCreationForValidation();int batches=0,tail=0,previous=0;float waits=0,highest=float.NegativeInfinity;
                bool sawInitialGrow=false,sawPark=false;
                do {
                    float wait=(float)steps.Current;waits+=wait;
                    if(Mathf.Abs(wait-.02f)<.00001f){
                        int delta=objects.Count-previous;
                        Check(delta==(batches==0?1:3),"source batch size/index mismatch");batches++;
                    } else {
                        float[] tails={.5f,.2f,.3f};Check(tail<tails.Length&&Mathf.Abs(wait-tails[tail++])<.00001f,"tail wait sequence");
                    }
                    foreach(var obj in objects.Skip(previous)){
                        Check(obj.transform.position.y>=highest,"source descriptions sort by ascending Y");highest=obj.transform.position.y;
                        Check(!pile.CanPick(obj.GetComponent<Rigidbody>()),"FinishCreate required before selecting");
                        bool expectedVisible=obj.transform.position.y>=pile.Threshold;
                        Check(obj.GetComponentsInChildren<Renderer>(true).Any(r=>r.enabled)==expectedVisible,"initial visibility uses spawn height before physics sync");
                        if(expectedVisible&&!sawInitialGrow)Check(obj.transform.localScale==Vector3.zero,"visible new items start at zero scale");
                        if(!sawInitialGrow&&obj.transform.localScale==Vector3.zero){
                            sawInitialGrow=true;
                            int type=int.Parse(obj.name.Substring(5));
                            var prefab=ReplicaSourceLevelPlan.Get(flow.library.themes[theme].sourceEnvironment).prefabs[type];
                            var full=prefab.transform.localScale*ReplicaSourceProfile.ModelScale(prefab.name);
                            pile.StepVisibility(.02f);
                            Check(Vector3.Distance(obj.transform.localScale,full*.1f)<.0001f,"creation grows linearly +.1 each .02 tick");
                        }
                    }
                    // No physics simulation: retain sampled coordinates to check sorted order. Advance
                    // scaled creation-growth ticks, testing its real depth checks and finite phases.
                    for(float t=0;t+1e-6f<wait;t+=.02f)pile.StepVisibility(Mathf.Min(.02f,wait-t));
                    sawPark|=objects.Any(x=>x.GetComponent<Rigidbody>().isKinematic);
                    previous=objects.Count;
                }while(steps.MoveNext());
                Check(batches==100&&tail==3&&Mathf.Abs(waits-3)<.00001f,"300-item source schedule totals 100*.02+.5+.2+.3");
                Check(objects.Count==300&&!flow.CreationInProgress&&!pile.Creating,"FinishCreate completes all items");
                Check(sawInitialGrow&&sawPark,"creation growth and height-based rigid freezing exercised");
                Check(GameManager.Instance.gameStatus==GameManager.GameStatus.PLAY,"input/countdown enable after finish");
                flow.Home();
            }
            File.WriteAllText("Temp/goose-source-creation-validation.txt","PASS three themes: true source iterator; batches 1/3/.../2; ascending Y; initial kinematic rigid; +.1/.02 creation growth; depth-based freezing; waits .02 then .5/.2/.3; FinishCreate before play.\n");
        }finally{flow.Home();Physics.autoSimulation=auto;}
    }
}
