using System;
using System.Linq;
using UnityEngine;

// The packaged WX 2.1.12 ordinary MainLevel / level 2 rows (indices 1, 4, 5).
// This is a separate catalog: it must not reorder the legacy tutorial/PVP type IDs.
public sealed class ReplicaSourceLevelPlan : ScriptableObject {
    [Serializable] public class Profile {
        public string environment;
        public int sourceIndex,seconds;
        public float limitHeight;
        public GameObject[] prefabs;
        public ChallengeLayer[] layers;
    }
    public Profile[] profiles;
    static ReplicaSourceLevelPlan catalog;
    public static Profile Get(string environment){
        if(!catalog){
            var library=Resources.Load<ReplicaTheme>("Replica/ThemeLibrary");
            if(library&&library.sourceLevelCatalog)catalog=library.sourceLevelCatalog;
        }
        if(!catalog)catalog=Resources.Load<ReplicaSourceLevelPlan>("Replica/SourceLevelCatalog");
        if(!catalog)throw new InvalidOperationException("Missing SourceLevelCatalog; run SourceLevelPlanImporter.Import");
        foreach(var profile in catalog.profiles)if(profile.environment==environment)return profile;
        throw new InvalidOperationException("No source level profile for "+environment);
    }

    // WX LevelData's first MainLevel row is a separate tutorial profile. It is
    // deliberately kept out of SourceLevelCatalog because that catalog is the
    // ordinary second-stage (150/60/45/45) plan. The names and geometry below
    // come directly from the packaged first rows: 12 items, two selected types,
    // height 6, limit height 4, and no countdown.
    public static Profile GetTutorial(string environment){
        var ordinary=Get(environment);
        string[] names;
        switch(environment){
            case "CountrysideStove": names=new[]{"Goose_Tomato","Potato","Orange","SaltedEgg"};break;
            case "GroceryStore": names=new[]{"Goose_ChiliSauce","Banana","GlassCola","WahahaMilk"};break;
            case "Barbecue": names=new[]{"Goose_Pancake","BBQSkewers2","GrilledCorn"};break;
            default: throw new InvalidOperationException("No source tutorial profile for "+environment);
        }
        var prefabs=new GameObject[names.Length];
        for(int i=0;i<names.Length;i++){
            prefabs[i]=ordinary.prefabs.FirstOrDefault(p=>p&&p.name==names[i]);
            if(!prefabs[i])throw new InvalidOperationException("Missing source tutorial prefab "+environment+"/"+names[i]);
        }
        return new Profile{
            environment=environment,sourceIndex=ordinary.sourceIndex,seconds=0,
            limitHeight=4f,prefabs=prefabs,
            layers=new[]{new ChallengeLayer(12,2,6f){weights=Enumerable.Repeat(1f,prefabs.Length).ToArray()}}
        };
    }
}
