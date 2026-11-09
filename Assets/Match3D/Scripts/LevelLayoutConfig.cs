using System;
using System.Linq;
using UnityEngine;

// Theme-specific first- and second-stage layouts. Keep this catalog separate
// so legacy tutorial/PVP type IDs retain their ordering.
// Data provenance: docs/refactoring/2026-11-04-naming.md.
public sealed class LevelLayoutConfig : ScriptableObject {
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
    static LevelLayoutConfig catalog;
    public static Profile Get(string environment){
        if(!catalog){
            var library=Resources.Load<Match3DTheme>("Match3D/ThemeLibrary");
            if(library&&library.sourceLevelCatalog)catalog=library.sourceLevelCatalog;
        }
        if(!catalog)catalog=Resources.Load<LevelLayoutConfig>("Match3D/SourceLevelCatalog");
        if(!catalog)throw new InvalidOperationException("Missing SourceLevelCatalog; run BoardLevelPlanImporter.Import");
        foreach(var profile in catalog.profiles)if(profile.environment==environment)return profile;
        throw new InvalidOperationException("No source level profile for "+environment);
    }

    public static Profile GetTutorial(string environment){
        return GetTutorial(Get(environment));
    }
    public static Profile GetTutorial(Profile challenge){
        string environment=challenge.environment;
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
    public static System.Collections.Generic.List<ChallengeTile> GenerateFirstStage(Profile profile,IBoardRandom random){
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
