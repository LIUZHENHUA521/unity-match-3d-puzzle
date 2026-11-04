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

    public static Profile GetTutorial(string environment){
        var challenge=Get(environment);
        var names=challenge.tutorialNames;
        if(names==null||names.Length<3||names.Distinct().Count()!=names.Length)
            throw new InvalidOperationException("First stage needs a target and distinct ordinary candidates: "+environment);
        var types=names.Select(name=>Array.FindIndex(challenge.prefabs,p=>p&&p.name==name)).ToArray();
        if(types.Any(type=>type<0))throw new InvalidOperationException("First-stage model missing from theme: "+environment);
        // Both stages use the exact same theme prefabs, including adapted models.
        return new Profile {
            environment=environment,sourceIndex=challenge.sourceIndex,seconds=0,limitHeight=4,
            prefabs=challenge.prefabs,
            tutorialNames=names,ordinaryTypes=types.Skip(1).ToArray(),
            layers=new[]{new ChallengeLayer(12,2,6f)}
        };
    }
    public static System.Collections.Generic.List<ChallengeTile> GenerateFirstStage(Profile profile,IReplicaSourceRandom random){
        // WX j38517: allocate three targets first, even for a single layer.
        // Keep IDs in the shared second-stage prefab array for pooling and tray bindings.
        for(int i=0;i<3;i++)random.Range(0f,1f);
        var tiles=ChallengePlan.GenerateSource(random,profile.ordinaryTypes.Length,profile.layers);
        foreach(var tile in tiles)tile.type=profile.ordinaryTypes[tile.type];
        int target=Array.FindIndex(profile.prefabs,p=>p&&p.name==profile.tutorialNames[0]);
        for(int i=0;i<3;i++)tiles.Add(new ChallengeTile{id=tiles.Count,type=target,layer=0,height=6f});
        return tiles;
    }
}
