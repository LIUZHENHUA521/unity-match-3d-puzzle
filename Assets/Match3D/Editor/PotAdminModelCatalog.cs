using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

// Export from the live prefab order: admin selections use these exact zero-based indices.
public static class PotAdminModelCatalog {
    public static void Run() { EditorApplication.delayCall += Export; }
    static void Export() {
        try {
            var root = "work/pot-model-catalog"; Directory.CreateDirectory(root);
            var names = JObject.Parse(File.ReadAllText("Assets/Match3D/Resources/Match3D/ModelAdapterCatalog.json"))["items"]
                .GroupBy(x => (string)x["id"]).ToDictionary(g => g.Key, g => (string)g.First()["displayName"]);
            var catalog = Resources.Load<Match3DModelThemes>("Match3D/ModelThemes");
            if(catalog.themes.Length != 8) throw new Exception("Expected eight themes");
            var output = new JObject();
            foreach(var theme in catalog.themes) {
                var profile = theme.profile;
                if(profile.prefabs.Length != 26 || profile.prefabs.Any(p => !p)) throw new Exception("Incomplete theme " + theme.id);
                if(profile.layers.Any(l => l.weights != null && l.weights.Length > 0 && (l.weights.Length != 26 || l.weights.Any(w => w <= 0 || float.IsNaN(w) || float.IsInfinity(w))))) throw new Exception("Invalid weights " + theme.id);
                var models = new JArray(); Directory.CreateDirectory(root + "/" + theme.id);
                using(var preview = new ThemeItemGamePreview()) {
                    for(int i=0;i<profile.prefabs.Length;i++) {
                        var prefab=profile.prefabs[i];var thumbnail=preview.Thumbnail(prefab);
                        File.WriteAllBytes(root + "/" + theme.id + "/" + i + ".jpg", thumbnail.EncodeToJPG(85));
                        UnityEngine.Object.DestroyImmediate(thumbnail);
                        models.Add(new JObject { ["index"]=i, ["name"]=names.TryGetValue(prefab.name,out var name)?name:prefab.name, ["prefab"]=prefab.name });
                    }
                }
                output[theme.id]=models;
            }
            File.WriteAllText(root+"/catalog.json",output.ToString());
            Validate();
            File.WriteAllText(root+"/checks.txt","PASS 8 themes x 26 model indices, thumbnails, weights and daily fixed/random generation\n");
            EditorApplication.Exit(0);
        } catch(Exception e) { Debug.LogException(e);EditorApplication.Exit(1); }
    }
    static void Validate() {
        foreach(int seed in Enumerable.Range(1,100)) {
            foreach(var selected in new[]{new int[0],new[]{25},new[]{0,23,25},Enumerable.Range(0,26).ToArray()}) {
                var row=new ChallengeLayer(156,selected.Length==0?23:selected.Length,14){potModelIndices=selected};
                var plan=Match3DDailyLevel.Generate(26,new[]{row},new Match3DDailyRandom(seed));
                if(plan.Count!=156 || plan.GroupBy(x=>x.type).Any(g=>g.Count()%3!=0) || plan.Select(x=>x.type).Distinct().Count()!=row.types)throw new Exception("Invalid budgets");
                if(selected.Length>0&&!plan.Select(x=>x.type).Distinct().OrderBy(x=>x).SequenceEqual(selected.OrderBy(x=>x)))throw new Exception("Selection mismatch");
                if(selected.Length==0) {
                    row.potModelIndices=null;
                    var old=Match3DDailyLevel.Generate(26,new[]{row},new Match3DDailyRandom(seed));
                    if(!old.Select(x=>x.type).SequenceEqual(plan.Select(x=>x.type)))throw new Exception("Empty changes RNG");
                }
            }
        }
        foreach(var selected in new[]{new[]{0,0},new[]{-1,1},new[]{0,26},new[]{0}})
            if(Match3DDailyLevel.PotModelsValid(new ChallengeLayer(30,2,14){potModelIndices=selected},26))throw new Exception("Invalid selection accepted");
    }
}
