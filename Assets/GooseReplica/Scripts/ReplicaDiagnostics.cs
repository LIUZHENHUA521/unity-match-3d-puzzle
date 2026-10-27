using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

// Temporary on-device diagnosis. No data is uploaded; switches reset on leaving a board.
public class ReplicaDiagnostics : MonoBehaviour {
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")] static extern int GooseDiagnosticValue(int field);
    static int Native(int field) => GooseDiagnosticValue(field);
#else
    static int Native(int field) => -1;
#endif
    Text label,physicsLabel,modelsLabel;
    readonly Dictionary<Renderer,bool> renderers=new Dictionary<Renderer,bool>();
    bool physicsPaused,modelsHidden,previousSimulation;
    float elapsed,worst;
    int frames,steps;
    public void Show(ReplicaFlow flow,Transform parent) {
        var view=ReplicaUIView.Load("Diagnostics",parent);
        label=view.Get<Text>("Readout");
        physicsLabel=view.Bind("Physics",TogglePhysics).GetComponentInChildren<Text>();
        modelsLabel=view.Bind("Models",ToggleModels).GetComponentInChildren<Text>();
        elapsed=0;frames=steps=0;worst=0;Native(0);
    }
    public void TogglePhysics() {
        if(!physicsPaused){previousSimulation=Physics.autoSimulation;Physics.autoSimulation=false;physicsPaused=true;}
        else {Physics.autoSimulation=previousSimulation;physicsPaused=false;}
        if(physicsLabel)physicsLabel.text=physicsPaused?"Physics OFF":"Physics ON";
    }
    public void ToggleModels() {
        if(modelsHidden){RestoreModels();}
        else {
            foreach(var obj in LevelGenerator.Instance.objectsOnScene)if(obj)
                foreach(var r in obj.GetComponentsInChildren<Renderer>()){renderers[r]=r.enabled;r.enabled=false;}
            modelsHidden=true;
        }
        if(modelsLabel)modelsLabel.text=modelsHidden?"Models OFF":"Models ON";
    }
    void RestoreModels(){foreach(var pair in renderers)if(pair.Key)pair.Key.enabled=pair.Value;renderers.Clear();modelsHidden=false;}
    public void ResetProbe(){if(physicsPaused)Physics.autoSimulation=previousSimulation;physicsPaused=false;RestoreModels();label=null;}
    void OnDestroy(){ResetProbe();}
    void FixedUpdate(){if(label)steps++;}
    void Update(){
        if(!label)return;
        elapsed+=Time.unscaledDeltaTime;worst=Mathf.Max(worst,Time.unscaledDeltaTime);frames++;
        if(elapsed<.5f)return;
        int awake=0,total=0;
        foreach(var obj in LevelGenerator.Instance.objectsOnScene)if(obj){var b=obj.GetComponent<Rigidbody>();if(b&&!b.isKinematic){total++;if(!b.IsSleeping())awake++;}}
        var input=InputController.Instance;
        var pile=ReplicaPilePhysics.Instance;
        label.text=$"0.1.10 | FPS {frames/elapsed:F0} | max {worst*1000:F0}ms | HP {Native(2)}\n"+
            $"touch {Native(0)}/{Native(1)} | input {input.PointerStarts} | hit {input.PointerHits} | pick {input.AcceptedPicks}\n"+
            $"sim {(pile?pile.ActiveCount:awake)}/{(pile?pile.BoardCount:total)} | fixed {steps} | {GameManager.Instance.gameStatus} | {input.LastPickState}";
        elapsed=0;frames=steps=0;worst=0;
    }
}
