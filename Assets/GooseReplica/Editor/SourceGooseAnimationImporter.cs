using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class SourceGooseAnimationImporter
{
    const string Root = "Assets/GooseReplica/OriginalModels";
    [Serializable] public class Export { public Model[] models; public Clip[] clips; }
    [Serializable] public class Model { public string model, group, path; }
    [Serializable] public class Clip { public string name; public float length, sampleRate; public bool loop; public Curve[] curves; }
    [Serializable] public class Curve { public string path, property; public Key[] keys; }
    [Serializable] public class Key { public float time, value, inTangent, outTangent; }
    public static Export Read() => SourceModelJson.Read<Export>(File.ReadAllText(Root + "/Animations/source-clips.json"));
    public static void Import()
    {
        var data = Read();
        foreach (var source in data.clips)
        {
            string path = Root + "/Animations/" + source.name + ".anim";
            var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
            if (!clip) { clip = new AnimationClip { name = source.name }; AssetDatabase.CreateAsset(clip, path); }
            clip.ClearCurves(); clip.legacy = false; clip.frameRate = source.sampleRate;
            foreach (var curve in source.curves)
            {
                var keys = curve.keys.Select(k => new Keyframe(k.time, k.value, k.inTangent, k.outTangent)).ToArray();
                AnimationUtility.SetEditorCurve(clip, EditorCurveBinding.FloatCurve(curve.path, typeof(Transform), curve.property), new AnimationCurve(keys));
            }
            var settings = AnimationUtility.GetAnimationClipSettings(clip);
            settings.startTime = 0; settings.stopTime = source.length;
            settings.loopTime = source.loop; settings.loopBlend = false;
            AnimationUtility.SetAnimationClipSettings(clip, settings);
            EditorUtility.SetDirty(clip);
        }
        AssetDatabase.SaveAssets();
        foreach (var model in data.models)
        {
            string path = Root + "/Prefabs/" + model.group + "/" + model.model + ".prefab";
            var root = PrefabUtility.LoadPrefabContents(path);
            try
            {
                var transform = root.transform.Find(model.path);
                if (!transform) throw new Exception("Missing source Animator root " + model.path);
                var animator = transform.GetComponent<Animator>();
                if (!animator) animator = transform.gameObject.AddComponent<Animator>();
                animator.runtimeAnimatorController = null; animator.avatar = null;
                animator.applyRootMotion = false; animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
                animator.updateMode = AnimatorUpdateMode.Normal;
                var animation = root.GetComponent<ReplicaSourceGooseAnimation>();
                if (!animation) animation = root.AddComponent<ReplicaSourceGooseAnimation>();
                animation.sourceAnimator = animator;
                animation.idle = Load("Level_Idle"); animation.outside = Load("Level_Outside");
                animation.inBox1 = Load("Level_InBox1"); animation.inBox2 = Load("Level_InBox2");
                animation.settlement = Load("Settlement_Struggle"); animation.mainIdle = Load("MainScene_Idle");
                animation.mainYell = Load("MainScene_Yell"); animation.mainDance = Load("MainScene_Dance");
                PrefabUtility.SaveAsPrefabAsset(root, path);
            }
            finally { PrefabUtility.UnloadPrefabContents(root); }
        }
        AssetDatabase.SaveAssets();
        Debug.Log("SOURCE_GOOSE_ANIMATION_IMPORTED " + data.clips.Length + " clips / " + data.models.Length + " goose prefabs");
    }
    static AnimationClip Load(string name) => AssetDatabase.LoadAssetAtPath<AnimationClip>(Root + "/Animations/" + name + ".anim");
}
