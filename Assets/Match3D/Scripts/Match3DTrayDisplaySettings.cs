using System;
using System.Collections.Generic;
using UnityEngine;

// Separate from generated prefabs: adapter regeneration keeps its stable ID.
public sealed class Match3DTrayDisplaySettings : ScriptableObject {
    [Serializable] public sealed class Pose {
        public Vector3 rotation=new Vector3(-90,0,0);
        public float scale=1;
        public Vector3 offset;
        public Pose Copy()=>new Pose{rotation=rotation,scale=scale,offset=offset};
        public bool IsValid=>Finite(rotation)&&Finite(offset)&&!float.IsNaN(scale)&&!float.IsInfinity(scale)&&scale>=.05f&&scale<=4;
        static bool Finite(Vector3 v)=>Finite(v.x)&&Finite(v.y)&&Finite(v.z);
        static bool Finite(float v)=>!float.IsNaN(v)&&!float.IsInfinity(v);
    }
    [Serializable] public sealed class Entry {
        public GameObject prefab;
        public string adapterId;
        public Pose pose=new Pose();
    }
    [Serializable] public sealed class BoardEntry {
        public GameObject prefab;
        public string adapterId;
        public float multiplier=1;
    }
    public List<BoardEntry> boardEntries=new List<BoardEntry>();
    public static bool ValidBoardMultiplier(float value)=>!float.IsNaN(value)&&!float.IsInfinity(value)&&value>=.25f&&value<=2;
    public float BoardMultiplier(GameObject prefab){
        string id=AdapterId(prefab);
        var entry=boardEntries.FindLast(e=>e!=null&&(string.IsNullOrEmpty(id)?string.IsNullOrEmpty(e.adapterId)&&e.prefab==prefab:string.Equals(e.adapterId,id,StringComparison.OrdinalIgnoreCase)));
        return entry!=null&&ValidBoardMultiplier(entry.multiplier)?entry.multiplier:1;
    }
    public void SetBoardMultiplier(GameObject prefab,float value){
        if(!prefab||!ValidBoardMultiplier(value))throw new ArgumentException("Invalid board size");
        RemoveBoardMultiplier(prefab);string id=AdapterId(prefab);
        boardEntries.Add(new BoardEntry{prefab=string.IsNullOrEmpty(id)?prefab:null,adapterId=id,multiplier=value});
    }
    public void RemoveBoardMultiplier(GameObject prefab){
        string id=AdapterId(prefab);
        boardEntries.RemoveAll(e=>e!=null&&(string.IsNullOrEmpty(id)?string.IsNullOrEmpty(e.adapterId)&&e.prefab==prefab:string.Equals(e.adapterId,id,StringComparison.OrdinalIgnoreCase)));
    }
    public List<Entry> entries=new List<Entry>();
    Dictionary<GameObject,Pose> originals;
    Dictionary<string,Pose> adapters;
    static Match3DTrayDisplaySettings cached;
    public static Match3DTrayDisplaySettings Current=>cached?cached:(cached=Resources.Load<Match3DTrayDisplaySettings>("Match3D/TrayDisplaySettings"));
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetCache(){cached=null;}
    void OnEnable(){Invalidate();}
    void OnValidate(){Invalidate();}
    public void Invalidate(){originals=null;adapters=null;}
    public static string AdapterId(GameObject prefab)=>prefab?prefab.GetComponent<BoardItemIdentity>()?.LogicalId:null;
    public bool TryGet(GameObject prefab,out Pose pose){
        pose=null;if(!prefab)return false;
        if(originals==null){
            originals=new Dictionary<GameObject,Pose>();adapters=new Dictionary<string,Pose>(StringComparer.Ordinal);
            foreach(var entry in entries){
                if(entry?.pose==null||!entry.pose.IsValid)continue;
                if(!string.IsNullOrEmpty(entry.adapterId))adapters[entry.adapterId]=entry.pose;
                else if(entry.prefab)originals[entry.prefab]=entry.pose;
            }
        }
        string id=AdapterId(prefab);
        return string.IsNullOrEmpty(id)?originals.TryGetValue(prefab,out pose):adapters.TryGetValue(id,out pose);
    }
    public void Set(GameObject prefab,Pose pose){
        if(!prefab||pose==null||!pose.IsValid)throw new ArgumentException("Invalid tray display pose");
        Remove(prefab);string id=AdapterId(prefab);
        entries.Add(new Entry{prefab=string.IsNullOrEmpty(id)?prefab:null,adapterId=id,pose=pose.Copy()});Invalidate();
    }
    public void Remove(GameObject prefab){
        string id=AdapterId(prefab);
        entries.RemoveAll(e=>e!=null&&(string.IsNullOrEmpty(id)?string.IsNullOrEmpty(e.adapterId)&&e.prefab==prefab:e.adapterId==id));Invalidate();
    }
    public static Quaternion Rotation(Pose pose,Quaternion cameraRotation)=>cameraRotation*Quaternion.Euler(pose?.rotation??new Vector3(-90,0,0));
    public static Vector3 Position(Pose pose,Vector3 slot,Quaternion cameraRotation,float spacing){
        var offset=pose?.offset??Vector3.zero;offset.z=-offset.z;
        return slot+cameraRotation*offset*spacing;
    }
}
