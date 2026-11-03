using System;
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

    public static Profile GetTutorial(string environment){
        var challenge=Get(environment);
        // Both stages use the exact same theme prefabs, including adapted models.
        return new Profile {
            environment=environment,sourceIndex=challenge.sourceIndex,seconds=0,limitHeight=4,
            prefabs=challenge.prefabs,
            layers=new[]{new ChallengeLayer(15,3,6f)}
        };
    }
    public static System.Collections.Generic.List<ChallengeTile> GenerateFirstStage(Profile profile,IReplicaSourceRandom random){
        // Select three distinct theme types; equal triple budgets preserve 6/6/3 objects.
        return ChallengePlan.GenerateSource(random,profile.prefabs.Length,profile.layers);
    }
}
