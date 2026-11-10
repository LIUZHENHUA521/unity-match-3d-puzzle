using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class ColliderPreviewValidation {
    [MenuItem("Goose/Validation/Collider preview interaction")]
    public static void Run(){
        try{Check();}catch(Exception e){Directory.CreateDirectory("docs/validation/collider-preview");File.WriteAllText("docs/validation/collider-preview/report.txt",e.ToString());throw;}
    }
    static void Check(){
        var window=EditorWindow.GetWindow<TrayDisplayWindow>();
        var flags=BindingFlags.Instance|BindingFlags.NonPublic;
        var preview=(BoardSizePreview)typeof(TrayDisplayWindow).GetField("boardPreview",flags).GetValue(window);
        if(preview==null)throw new Exception("Open preview first");
        var before=preview.Rotation;bool dirty=window.hasUnsavedChanges;
        var p=new Vector2(450,450);
        window.SendEvent(new Event{type=EventType.MouseDown,button=0,mousePosition=p});
        window.SendEvent(new Event{type=EventType.MouseDrag,button=0,mousePosition=p+new Vector2(80,40),delta=new Vector2(80,40)});
        window.SendEvent(new Event{type=EventType.MouseUp,button=0,mousePosition=p+new Vector2(80,40)});
        if(Quaternion.Angle(before,preview.Rotation)<1)throw new Exception("Board drag did not rotate");
        if(window.hasUnsavedChanges!=dirty)throw new Exception("Preview drag changed saved-pose dirty state");
        var start=preview.Rotation;
        window.SendEvent(new Event{type=EventType.MouseDown,button=0,mousePosition=p});
        window.SendEvent(new Event{type=EventType.MouseDrag,button=0,mousePosition=p+new Vector2(30,0),delta=new Vector2(30,0),shift=true});
        if(Quaternion.Angle(start,preview.Rotation)<1)throw new Exception("Shift drag did not rotate");
        window.SendEvent(new Event{type=EventType.KeyDown,keyCode=KeyCode.Escape});
        if(Quaternion.Angle(start,preview.Rotation)>.01f)throw new Exception("Escape did not restore rotation");
        int models=0,colliders=0;
        foreach(var model in (GameObject[])typeof(TrayDisplayWindow).GetField("models",flags).GetValue(window)){
            int count=0;var clone=UnityEngine.Object.Instantiate(model);
            try{foreach(var c in clone.GetComponentsInChildren<Collider>(true)){
                if(!c.enabled||c.isTrigger||!c.gameObject.activeInHierarchy||c.gameObject.layer!=ItemColliderGeometry.BodyLayer)continue;
                if(!(c is BoxCollider)&&!(c is SphereCollider)&&!(c is CapsuleCollider))throw new Exception("Unsupported solid collider: "+model.name+" / "+c.GetType().Name);
                count++;
            }}finally{UnityEngine.Object.DestroyImmediate(clone);}
            if(count==0)throw new Exception("Missing solid collider: "+model.name);
            models++;colliders+=count;
        }
        Directory.CreateDirectory("docs/validation/collider-preview");
        File.WriteAllText("docs/validation/collider-preview/report.txt","PASS: board drag rotates; Shift drag rotates; Escape restores; no pose dirty state changes; "+models+" models / "+colliders+" supported solid colliders.\n");
        window.Repaint();Debug.Log("PASS collider preview interaction");
    }
}
