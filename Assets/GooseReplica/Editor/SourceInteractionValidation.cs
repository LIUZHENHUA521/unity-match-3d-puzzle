using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

// Runtime interaction contract checks use isolated small colliders in front of the camera.
public static class SourceInteractionValidation {
    static void Check(bool value,string message){if(!value)throw new Exception(message);}
    static void Invoke(InputController input,string name,Vector2 position){
        typeof(InputController).GetMethod(name,BindingFlags.Instance|BindingFlags.NonPublic).Invoke(input,new object[]{position});
    }
    public static void Run(){
        var flow=ReplicaFlow.Instance;bool auto=Physics.autoSimulation;string report="";
        var created=new List<GameObject>();
        var uiRaycasters=new Dictionary<GraphicRaycaster,bool>();
        try{
            Physics.autoSimulation=false;flow.Begin(0);GameManager.Instance.gameStatus=GameManager.GameStatus.PLAY;
            // These foreground world probes test hit/hold/release, not the production HUD layout.
            // Keep actual pointer code intact; disable UI raycasters only for this isolated fixture.
            foreach(var raycaster in UnityEngine.Object.FindObjectsOfType<GraphicRaycaster>(true)){
                uiRaycasters[raycaster]=raycaster.enabled;raycaster.enabled=false;
            }
            foreach(var obj in LevelGenerator.Instance.objectsOnScene)obj.SetActive(false);
            var input=InputController.Instance;var feedback=input.GetComponent<ReplicaSelectionFeedback>();var camera=Camera.main;
            var a=Create(camera,new Vector2(.3f,.55f),"Pointer A",created);
            var b=Create(camera,new Vector2(.7f,.55f),"Pointer B",created);
            var aPos=(Vector2)camera.WorldToScreenPoint(a.transform.position);var bPos=(Vector2)camera.WorldToScreenPoint(b.transform.position);
            var blank=(Vector2)camera.ViewportToScreenPoint(new Vector3(.5f,.55f,0));Physics.SyncTransforms();
            Check(input.PickAt(aPos)==a&&input.PickAt(bPos)==b,"single ray hits pick collider");
            var edge=aPos+new Vector2(camera.pixelWidth*.018f,0);
            Check(input.PickAt(edge)==null,"no invented near-miss ray fan");
            Invoke(input,"BeginPointer",aPos);Check(feedback.Selected==a,"press A; state="+input.LastPickState+" busy="+input.IsBusy);
            Invoke(input,"UpdatePointer",aPos);Check(feedback.Selected==a,"held kinematic object remains pickable");
            var savedA=a.transform.position;var savedB=b.transform.position;
            a.transform.position=camera.ViewportToWorldPoint(new Vector3(.2f,.8f,camera.nearClipPlane+1f));b.transform.position=savedA;Physics.SyncTransforms();
            Invoke(input,"UpdatePointer",aPos);Check(feedback.Selected==a,"stationary pointer does not switch as physics changes hit");
            a.transform.position=savedA;b.transform.position=savedB;Physics.SyncTransforms();
            Invoke(input,"UpdatePointer",bPos);Check(feedback.Selected==b&&!a.GetComponent<Rigidbody>().isKinematic,"moving held pointer switches and restores previous body");
            Invoke(input,"UpdatePointer",blank);Check(!feedback.Selected,"held blank clears target");
            Invoke(input,"EndPointer",blank);Check(input.objectsOnStack.Count==0,"release blank collects nothing");
            Invoke(input,"BeginPointer",blank);Invoke(input,"UpdatePointer",aPos);Check(feedback.Selected==a,"pointer can acquire after initial miss");
            Invoke(input,"UpdatePointer",bPos);Invoke(input,"EndPointer",bPos);
            Check(input.objectsOnStack.Count==1&&input.objectsOnStack[0]==b,"release collects last held target, no drag-distance cancellation");
            // Exercise actual undo callback after restoring the resolved tray state.
            var resolution=typeof(InputController).GetField("resolution",BindingFlags.Instance|BindingFlags.NonPublic);
            input.StopAllCoroutines();resolution.SetValue(input,null);input.arrivedObject.Add(b);
            input.UndoLast();DOTween.Complete(b.transform,true);
            var shapes=b.GetComponents<Collider>();Check(shapes[0].enabled&&!shapes[1].enabled,"undo preserves originally disabled collider");
            report="PASS exact ray and edge miss; held kinematic hit; stationary target retention; moved target switch; empty cancellation/reacquisition; release current target; undo original collider flags\n";
        }catch(Exception e){report="FAIL "+e+"\n";}
        finally{
            InputController.Instance.CancelSelection();InputController.Instance.ResetStack();
            foreach(var obj in created)if(obj){obj.transform.DOKill();UnityEngine.Object.DestroyImmediate(obj);}
            flow.Home();
            foreach(var saved in uiRaycasters)if(saved.Key)saved.Key.enabled=saved.Value;
            Physics.autoSimulation=auto;
            File.WriteAllText("Temp/goose-source-interaction-validation.txt",report);Debug.Log(report);
        }
    }
    static GameObject Create(Camera camera,Vector2 viewport,string name,List<GameObject> created){
        var obj=GameObject.CreatePrimitive(PrimitiveType.Cube);obj.name=name;obj.tag="Collectable";obj.layer=ReplicaSourceColliders.PickLayer;
        obj.transform.position=camera.ViewportToWorldPoint(new Vector3(viewport.x,viewport.y,camera.nearClipPlane+1f));obj.transform.rotation=camera.transform.rotation;
        obj.transform.localScale=Vector3.one*(camera.orthographicSize*2*camera.aspect*.02f);
        obj.AddComponent<Rigidbody>().useGravity=false;obj.AddComponent<SphereCollider>().enabled=false;
        created.Add(obj);return obj;
    }
}
