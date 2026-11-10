using System;
using UnityEngine;

// Model themes are independent of the physical scene and the daily reward identity.
public sealed class Match3DModelThemes : ScriptableObject {
    [Serializable] public sealed class Entry {
        public string id, title;
        public Sprite background;
        public LevelLayoutConfig.Profile profile;
        [Range(.1f,2f)] public float ambientMultiplier=1f,lightMultiplier=1f;
    }
    public Entry[] themes;
    public static Entry Find(string id,GameObject model=null) {
        var catalog=Resources.Load<Match3DModelThemes>("Match3D/ModelThemes");
        if(catalog&&catalog.themes!=null)foreach(var entry in catalog.themes)
            if(entry!=null&&(model?entry.profile!=null&&Array.IndexOf(entry.profile.prefabs,model)>=0:entry.id==id))return entry;
        return null;
    }
    public static string Title(string id,string fallback) {
        var catalog=Resources.Load<Match3DModelThemes>("Match3D/ModelThemes");
        if(catalog&&catalog.themes!=null)foreach(var entry in catalog.themes)if(entry.id==id)return entry.title;
        return fallback;
    }
    public static bool TryGet(string id, string environment, out LevelLayoutConfig.Profile profile) {
        if (string.IsNullOrEmpty(id)) { profile = LevelLayoutConfig.Get(environment); return true; }
        var catalog = Resources.Load<Match3DModelThemes>("Match3D/ModelThemes");
        if (catalog && catalog.themes != null) foreach (var entry in catalog.themes)
            if (entry.id == id && entry.profile != null && entry.profile.environment == environment) {
                profile = entry.profile; return true;
            }
        profile = null; return false;
    }
    public static LevelLayoutConfig.Profile Get(string id, string environment, bool tutorial = false) {
        if (!TryGet(id, environment, out var profile)) throw new InvalidOperationException("Unknown model theme: " + id);
        return tutorial ? LevelLayoutConfig.GetTutorial(profile) : profile;
    }
}
