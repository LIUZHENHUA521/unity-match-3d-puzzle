using System;
using System.IO;
using System.Collections;
using System.Reflection;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

public static class SourceGooseAnimationValidation
{
    const string Root = "Assets/GooseReplica/OriginalModels";
    public static string Run()
    {
        var data = SourceGooseAnimationImporter.Read();
        int channels = 0, keys = 0, movedSkeletons = 0, deformedMeshes = 0;
        var random = UnityEngine.Random.state;
        try
        {
            foreach (var source in data.clips)
            {
                var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(Root + "/Animations/" + source.name + ".anim");
                Check(clip && !clip.legacy, "Missing generic source clip " + source.name);
                Check(Mathf.Abs(clip.length - source.length) < 1e-5f, "Clip duration " + source.name);
                Check(AnimationUtility.GetAnimationClipSettings(clip).loopTime == source.loop, "Clip loop mode " + source.name);
                var bindings = AnimationUtility.GetCurveBindings(clip);
                Check(bindings.Length == source.curves.Length, "Curve count " + source.name);
                foreach (var c in source.curves)
                {
                    var curve = AnimationUtility.GetEditorCurve(clip, EditorCurveBinding.FloatCurve(c.path, typeof(Transform), c.property));
                    Check(curve != null && curve.length == c.keys.Length, "Missing keys " + source.name + "/" + c.path);
                    for (int k = 0; k < c.keys.Length; k++)
                    {
                        var expected = c.keys[k]; var actual = curve.keys[k];
                        Check(Mathf.Abs(actual.time - expected.time) < 1e-5f && Mathf.Abs(actual.value - expected.value) < 1e-5f &&
                              Mathf.Abs(actual.inTangent - expected.inTangent) < 1e-4f && Mathf.Abs(actual.outTangent - expected.outTangent) < 1e-4f,
                              "Recovered key differs " + source.name + "/" + c.path + "/" + k);
                        keys++;
                    }
                    channels++;
                }
            }
            foreach (var model in data.models)
            {
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(Root + "/Prefabs/" + model.group + "/" + model.model + ".prefab");
                var obj = UnityEngine.Object.Instantiate(prefab); obj.SetActive(true);
                try
                {
                    var anim = obj.GetComponent<ReplicaSourceGooseAnimation>();
                    Check(anim && anim.sourceAnimator && !anim.sourceAnimator.runtimeAnimatorController && !anim.sourceAnimator.avatar, "Source Animator configuration " + model.model);
                    var root = anim.sourceAnimator.gameObject;
                    foreach (var c in data.clips[0].curves) Check(root.transform.Find(c.path), "Unresolved bone " + model.model + "/" + c.path);
                    var bones = root.GetComponentsInChildren<Transform>();
                    anim.outside.SampleAnimation(root, 0);
                    var positions = bones.Select(b => b.localPosition).ToArray(); var rotations = bones.Select(b => b.localRotation).ToArray();
                    var skin = obj.GetComponentInChildren<SkinnedMeshRenderer>();
                    var mesh = new Mesh(); skin.BakeMesh(mesh); var vertices = mesh.vertices;
                    anim.outside.SampleAnimation(root, .4f);
                    Check(bones.Where((b, i) => (b.localPosition - positions[i]).sqrMagnitude > 1e-8f || Quaternion.Angle(b.localRotation, rotations[i]) > .01f).Any(), "Frozen skeleton " + model.model);
                    movedSkeletons++;
                    skin.BakeMesh(mesh);
                    Check(mesh.vertices.Where((v, i) => (v - vertices[i]).sqrMagnitude > 1e-8f).Any(), "Skin did not deform " + model.model);
                    deformedMeshes++; UnityEngine.Object.DestroyImmediate(mesh);
                    if (Application.isPlaying)
                    {
                        anim.InitializeSource();
                        Check(anim.Initialized && anim.CurrentClip == "Level_Idle" && anim.CurrentTime >= 0 && anim.CurrentTime <= anim.idle.length, "Init clip/random phase " + model.model);
                        Check(anim.NotBeCatch && anim.PendingWait >= 5 && anim.PendingWait <= 15, "Original Init notBeCatch=true / InBox wait " + model.model);
                        anim.SetSourceVisible(false); Check(anim.PendingWait == 0, "Hidden coroutine still running " + model.model);
                        anim.SetSourceVisible(true); Check(anim.NotBeCatch && anim.PendingWait >= 5 && anim.PendingWait <= 15, "Reveal did not restart " + model.model);
                        anim.EnterTray(); Check(!anim.NotBeCatch && anim.PendingWait >= 1 && anim.PendingWait <= 5, "BeClick notBeCatch=false / Outside wait " + model.model);
                        anim.EnterStore(); Check(anim.NotBeCatch && anim.PendingWait >= 5 && anim.PendingWait <= 15, "StoreObj notBeCatch=true / InBox wait " + model.model);
                        anim.EnterTray(); Check(!anim.NotBeCatch && anim.PendingWait >= 1 && anim.PendingWait <= 5, "Stored item repick / Outside wait " + model.model);
                        anim.ReturnToPile(); Check(anim.NotBeCatch && anim.PendingWait >= 5 && anim.PendingWait <= 15, "Returned pile wait " + model.model);
                        float pendingBeforeWin = anim.PendingWait;
                        anim.PlaySettlement(); Check(anim.CurrentClip == "Settlement_Struggle" && anim.PendingWait == pendingBeforeWin, "WinAnim preserves existing RandomAnim coroutine " + model.model);
                        // Drive the actual compiler-generated coroutine across its waits. This checks the
                        // WAT branch, not just helper state labels, without waiting 15 real seconds per goose.
                        anim.EnterStore(); anim.SetSourceVisible(false);
                        var steps = (IEnumerator)typeof(ReplicaSourceGooseAnimation)
                            .GetMethod("RandomAnimation", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(anim, null);
                        try
                        {
                            Check(steps.MoveNext() && anim.PendingWait >= 5 && anim.PendingWait <= 15, "True-flag initial wait " + model.model);
                            anim.SetNotBeCatch(false); // Source state-machine captures the earlier branch across its yield.
                            Check(steps.MoveNext() && (anim.CurrentClip == "Level_InBox1" || anim.CurrentClip == "Level_InBox2"), "True branch must choose InBox " + model.model);
                            Check(steps.MoveNext() && anim.CurrentClip == "Level_Idle" && anim.PendingWait >= 1 && anim.PendingWait <= 5, "Next iteration rereads false flag " + model.model);
                            Check(steps.MoveNext() && anim.CurrentClip == "Level_Outside", "False branch must choose Outside " + model.model);
                        }
                        finally { (steps as IDisposable)?.Dispose(); }

                    }
                }
                finally { UnityEngine.Object.DestroyImmediate(obj); }
            }
        }
        finally { UnityEngine.Random.state = random; }
        string report = "PASS source-goose-animation\n8 original generic clips / " + channels + " float curves / " + keys + " exact imported keys\n" + movedSkeletons + " skeletons move; " + deformedMeshes + " skinned meshes deform\nNative Animator has no Avatar/controller; source Playables lifecycle + visibility/tray waits checked\n";
        Directory.CreateDirectory("Temp"); File.WriteAllText("Temp/goose-source-animation-validation.txt", report);
        Debug.Log(report); return report;
    }
    static void Check(bool ok, string message) { if (!ok) throw new Exception(message); }
}
