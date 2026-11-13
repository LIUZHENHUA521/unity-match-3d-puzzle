using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable] public sealed class PotReplayProbeColumn { public int[] ids=new int[0]; }
[Serializable] public sealed class PotReplayReward { public int id,column;public float age;public bool color; }
[Serializable] public sealed class PotReplayActivation { public int id;public Vector3 origin;public float age; }
[Serializable] public sealed class PotReplayToolEffect { public int key,kind;public Vector3 p,s;public Quaternion r;public float age;public uint seed; }
[Serializable] public sealed class PotReplayPresentation {
    public bool probe,used,firstHint,ringVisible,praiseVisible,storeVisible;
    public int combo,stars,nextColumn,highlightColumn=-1,previewId=-1;
    public int selectedId=-1;public int[] highlightIds=new int[0];public float highlightRemaining,highlightTime;
    public float hintAge,ringTime,ringAlpha,praiseTime,previewTime,storeZ,storeExpansion;
    public string praise="",ring="";public Vector3 previewOrigin;
    public PotReplayProbeColumn[] columns=new PotReplayProbeColumn[0];
    public float[] chance=new float[0];
    public PotReplayReward[] rewards=new PotReplayReward[0];
    public PotReplayActivation[] activations=new PotReplayActivation[0];
    public PotReplayToolEffect[] tools=new PotReplayToolEffect[0];
    public static int Id(GameObject obj)=>obj&&obj.GetComponent<Match3DTile>()?obj.GetComponent<Match3DTile>().id:-1;
    public static PotReplayPresentation Capture(){
        var result=new PotReplayPresentation();var owner=ComboProbeController.Instance;
        if(owner&&owner.Running){
            result.probe=true;result.used=owner.HasUsedProbe;result.combo=owner.State.Combo;result.stars=owner.State.Stars;result.nextColumn=owner.State.NextColumn;
            result.columns=Enumerable.Range(0,4).Select(i=>new PotReplayProbeColumn{ids=owner.State.Column(i).Select(t=>Id(owner.Target(t)?.gameObject)).Where(id=>id>=0).ToArray()}).ToArray();
            if(owner.View)owner.View.CaptureReplay(result);
        }
        var highlight=Camera.main?Camera.main.GetComponent<ItemHighlight>():null;if(highlight)highlight.CaptureReplay(result);
        result.activations=PotProbeActivation.CaptureReplay();BoardToolVisual.CaptureReplay(result);return result;
    }
}
