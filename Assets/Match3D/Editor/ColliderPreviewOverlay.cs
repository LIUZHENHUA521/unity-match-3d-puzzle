using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

// Editor-only overlay. Capture authored solid colliders before previews disable simulation.
public sealed class ColliderPreviewOverlay {
    readonly Collider[] colliders;
    public int Count=>colliders.Length;
    public ColliderPreviewOverlay(GameObject item){
        colliders=item.GetComponentsInChildren<Collider>(true).Where(c=>c.enabled&&!c.isTrigger&&c.gameObject.activeInHierarchy&&c.gameObject.layer==ItemColliderGeometry.BodyLayer).ToArray();
    }
    public void Draw(Rect rect,Camera camera){
        if(Event.current.type!=EventType.Repaint)return;
        GUI.BeginGroup(rect);Handles.BeginGUI();var previous=Handles.color;Handles.color=new Color(.25f,1,.45f,.9f);
        try{foreach(var collider in colliders)if(collider)DrawCollider(collider,(a,b)=>{
            var p=camera.WorldToViewportPoint(a);var q=camera.WorldToViewportPoint(b);
            if(p.z<=0||q.z<=0)return;
            Handles.DrawAAPolyLine(1.5f,new Vector3(p.x*rect.width,(1-p.y)*rect.height),new Vector3(q.x*rect.width,(1-q.y)*rect.height));
        });}finally{Handles.color=previous;Handles.EndGUI();GUI.EndGroup();}
    }
    static void Circle(Vector3 center,Vector3 a,Vector3 b,float radius,Action<Vector3,Vector3> line){
        Arc(center,a,b,radius,0,Mathf.PI*2,line);
    }
    static void Arc(Vector3 center,Vector3 a,Vector3 b,float radius,float start,float end,Action<Vector3,Vector3> line){
        Vector3 last=center+(a*Mathf.Cos(start)+b*Mathf.Sin(start))*radius;
        for(int i=1;i<=32;i++){float angle=Mathf.Lerp(start,end,i/32f);var next=center+(a*Mathf.Cos(angle)+b*Mathf.Sin(angle))*radius;line(last,next);last=next;}
    }
    static void DrawCollider(Collider collider,Action<Vector3,Vector3> line){
        var t=collider.transform;var scale=t.lossyScale;scale=new Vector3(Mathf.Abs(scale.x),Mathf.Abs(scale.y),Mathf.Abs(scale.z));
        if(collider is BoxCollider box){
            var corners=new Vector3[8];for(int i=0;i<8;i++)corners[i]=t.TransformPoint(box.center+Vector3.Scale(box.size*.5f,new Vector3((i&1)==0?-1:1,(i&2)==0?-1:1,(i&4)==0?-1:1)));
            for(int i=0;i<8;i++)for(int bit=1;bit<=4;bit*=2)if((i&bit)==0)line(corners[i],corners[i|bit]);
        }else if(collider is SphereCollider sphere){
            var center=t.TransformPoint(sphere.center);float radius=sphere.radius*Mathf.Max(scale.x,Mathf.Max(scale.y,scale.z));
            Circle(center,t.right,t.up,radius,line);Circle(center,t.up,t.forward,radius,line);Circle(center,t.forward,t.right,radius,line);
        }else if(collider is CapsuleCollider capsule){
            int d=capsule.direction;var axis=t.rotation*(d==0?Vector3.right:d==1?Vector3.up:Vector3.forward);
            var a=t.rotation*(d==0?Vector3.up:Vector3.right);var b=Vector3.Cross(axis,a);
            float radius=capsule.radius*Mathf.Max(scale[(d+1)%3],scale[(d+2)%3]);float half=Mathf.Max(0,capsule.height*scale[d]*.5f-radius);
            var center=t.TransformPoint(capsule.center);var top=center+axis*half;var bottom=center-axis*half;
            Circle(top,a,b,radius,line);Circle(bottom,a,b,radius,line);
            foreach(var tangent in new[]{a,b}){
                line(top+tangent*radius,bottom+tangent*radius);line(top-tangent*radius,bottom-tangent*radius);
                Arc(top,tangent,axis,radius,0,Mathf.PI,line);Arc(bottom,tangent,-axis,radius,0,Mathf.PI,line);
            }
        }
    }
}
