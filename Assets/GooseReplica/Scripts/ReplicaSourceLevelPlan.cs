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
        public string[] tutorialNames;
        public int[] ordinaryTypes; // Zero-based prefab indices; target is index zero in recovered first rows.
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

    static ReplicaSourceLevelPlan firstStage;
    public static Profile GetTutorial(string environment){
        if(!firstStage)firstStage=Resources.Load<ReplicaSourceLevelPlan>("Replica/FirstStageCatalog");
        if(!firstStage)throw new InvalidOperationException("Missing FirstStageCatalog; run Import First Stage Plans");
        return firstStage.profiles.First(p=>p.environment==environment);
    }
    public static System.Collections.Generic.List<ChallengeTile> GenerateFirstStage(Profile profile,IReplicaSourceRandom random){
        // j38517 allocates the three targets BEFORE selecting ordinary types.
        // The single-layer KFakeRandom still consumes one float draw per target.
        for(int i=0;i<3;i++)random.Range(0f,1f);
        var tiles=ChallengePlan.GenerateSource(random,profile.ordinaryTypes.Length,profile.layers);
        foreach(var tile in tiles)tile.type=profile.ordinaryTypes[tile.type];
        for(int i=0;i<3;i++)tiles.Add(new ChallengeTile{id=tiles.Count,type=0,layer=0,height=6f});
        return tiles;
    }
}
