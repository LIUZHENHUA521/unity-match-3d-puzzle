using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;

// Frozen pre-refactor fingerprints include every output field and every RNG call.
public static class ChallengePlanEquivalenceValidation {
    const string Fixture = "Tools/validation/challenge-plan-baseline.txt";
    sealed class Trace : IBoardRandom {
        readonly BoardReplayRandom random;
        readonly BinaryWriter writer;
        public Trace(int seed, BinaryWriter writer) { random = new BoardReplayRandom(seed); this.writer = writer; }
        public float Range(float min, float max) {
            float value = random.Range(min, max);
            writer.Write(min); writer.Write(max); writer.Write(value);
            return value;
        }
    }
    static string Fingerprint(Action<BinaryWriter> action) {
        using(var memory = new MemoryStream()) {
            using(var writer = new BinaryWriter(memory, System.Text.Encoding.UTF8, true)) {
                try { action(writer); writer.Write("OK"); }
                catch(ArgumentException e) { writer.Write(e.GetType().FullName); writer.Write(e.Message); }
            }
            using(var sha = SHA256.Create()) return BitConverter.ToString(sha.ComputeHash(memory.ToArray())).Replace("-", "");
        }
    }
    static void WriteTiles(BinaryWriter writer, List<ChallengeTile> tiles) {
        writer.Write(tiles.Count);
        foreach(var tile in tiles) { writer.Write(tile.id); writer.Write(tile.type); writer.Write(tile.layer); writer.Write(tile.height); }
    }
    public static void Run() {
        var saved = UnityEngine.Random.state;
        try {
            var rows = new List<string>();
            var seeds = Enumerable.Range(0, 200).Concat(new[]{-1, int.MinValue, int.MaxValue, 1926537}).ToArray();
            foreach(int pool in new[]{1, 5, 22, 31}) {
                foreach(bool weighted in new[]{false, true}) {
                    var layers = pool == 1 ? new[]{new ChallengeLayer(12, 1, 6)} : ChallengePlan.DefaultLayers(pool);
                    if(weighted) foreach(var layer in layers) layer.weights = Enumerable.Range(0, pool).Select(i => (i % 5 + 1) * .17f).ToArray();
                    foreach(int seed in seeds) {
                        rows.Add($"shared/{pool}/{weighted}/{seed} " + Fingerprint(w => WriteTiles(w, ChallengePlan.GenerateSource(new Trace(seed, w), pool, layers))));
                        rows.Add($"legacy/{pool}/{weighted}/{seed} " + Fingerprint(w => WriteTiles(w, ChallengePlan.Generate(seed, pool, layers))));
                    }
                }
            }
            foreach(var weights in new[]{new[]{1f,1f},new[]{1f,1f,1f,1f,1f},new[]{.01f,100f,.03f},new[]{0f},new[]{float.NaN},new float[0]})
                foreach(int groups in new[]{-1,0,2,3,5,100})
                    rows.Add("budget " + Fingerprint(w => { foreach(int n in ChallengePlan.SourceGroupBudgets(groups, weights)) w.Write(n); }));
            var invalid = new ChallengeLayer[][] {null, new ChallengeLayer[0], new ChallengeLayer[]{null}, new[]{new ChallengeLayer(4,1,2)}, new[]{new ChallengeLayer(3,2,2)}, new[]{new ChallengeLayer(6,1,float.NaN)}, new[]{new ChallengeLayer(6,1,2){weights=new[]{0f,1f}}}, new[]{new ChallengeLayer(6,1,2),new ChallengeLayer(5,1,3)}};
            foreach(var layers in invalid)
                rows.Add("invalid " + Fingerprint(w => WriteTiles(w, ChallengePlan.GenerateSource(new Trace(53,w),2,layers))));
            if(!saved.Equals(UnityEngine.Random.state)) throw new Exception("Validation changed production RNG state");
            string actual = string.Join("\n", rows) + "\n";
            if(Environment.GetEnvironmentVariable("MATCH3D_CAPTURE_BASELINE") == "1") {
                if(File.Exists(Fixture)) throw new Exception("Refusing to overwrite frozen baseline");
                Directory.CreateDirectory(Path.GetDirectoryName(Fixture)); File.WriteAllText(Fixture,actual);
            } else if(File.ReadAllText(Fixture) != actual) {
                File.WriteAllText("Temp/challenge-plan-actual.txt",actual);
                throw new Exception("Output/RNG fingerprints differ from pre-refactor baseline");
            }
            string report = "PASS " + rows.Count + " pre-refactor fingerprints: exact tile order/fields, float bits, RNG bounds/values/order, budget rounding and invalid input behavior. Production RNG state preserved.\n";
            Directory.CreateDirectory("docs/refactoring"); File.WriteAllText("docs/refactoring/2026-11-04-algorithm-validation.txt",report); Debug.Log(report);
            EditorApplication.Exit(0);
        } catch(Exception e) { Debug.LogException(e); EditorApplication.Exit(1); }
        finally { UnityEngine.Random.state = saved; }
    }
}
