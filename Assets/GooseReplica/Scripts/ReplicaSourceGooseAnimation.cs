using System.Collections;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

// ObjectUnit.PlayAnim / PlayAnimClip / RandomAnim, using the recovered generic bone clips.
// Source uses an Animator without a controller plus Animancer; this graph preserves that model.
public sealed class ReplicaSourceGooseAnimation : MonoBehaviour
{
    public Animator sourceAnimator;
    public AnimationClip idle, outside, inBox1, inBox2, settlement, mainIdle, mainYell, mainDance;
    PlayableGraph graph;
    AnimationMixerPlayable mixer;
    AnimationClipPlayable[] states;
    AnimationClip[] clips;
    float[] fadeStart;
    float fadeDuration, fadeElapsed;
    int target = -1;
    Coroutine routine;
    bool initialized, visible = true, notBeCatch = true;
    public bool Initialized => initialized;
    // The original name means "not yet caught": true in the pile/store, false after BeClick.
    public bool NotBeCatch => notBeCatch;
    public void SetNotBeCatch(bool value) { notBeCatch = value; }
    public string CurrentClip => target >= 0 && clips != null ? clips[target].name : "";
    public float CurrentTime => target >= 0 && states != null ? (float)states[target].GetTime() : 0;
    public float PendingWait { get; private set; }

    public void InitializeSource()
    {
        if (initialized || !sourceAnimator || !idle) return;
        initialized = true;
        clips = new[] { idle, outside, inBox1, inBox2, settlement, mainIdle, mainYell, mainDance };
        states = new AnimationClipPlayable[clips.Length];
        fadeStart = new float[clips.Length];
        sourceAnimator.enabled = true;
        sourceAnimator.runtimeAnimatorController = null;
        sourceAnimator.applyRootMotion = false;
        sourceAnimator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
        sourceAnimator.updateMode = AnimatorUpdateMode.Normal;
        graph = PlayableGraph.Create("Source goose " + gameObject.GetInstanceID());
        graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
        mixer = AnimationMixerPlayable.Create(graph, clips.Length);
        for (int i = 0; i < clips.Length; i++)
        {
            states[i] = AnimationClipPlayable.Create(graph, clips[i]);
            states[i].SetApplyFootIK(false);
            states[i].SetApplyPlayableIK(false);
            graph.Connect(states[i], 0, mixer, i);
            mixer.SetInputWeight(i, 0);
        }
        var output = AnimationPlayableOutput.Create(graph, "Goose bones", sourceAnimator);
        output.SetSourcePlayable(mixer);
        graph.Play();
        RestartPlayback();
    }

    // Mirrors SetVisible: only actual visibility changes restart/stop the animation coroutine.
    public void SetSourceVisible(bool value)
    {
        if (!initialized || visible == value) return;
        visible = value;
        if (value) { graph.Play(); RestartPlayback(); }
        else
        {
            StopRoutine();
            for (int i = 0; i < states.Length; i++) { mixer.SetInputWeight(i, 0); states[i].SetTime(0); }
            target = -1; graph.Stop();
        }
    }

    // BeClick writes notBeCatch=false before PlayAnim(true), even if already visible.
    public void EnterTray()
    {
        if (!initialized) return;
        notBeCatch = false; visible = true;
        graph.Play(); RestartPlayback();
    }

    // StoreObj writes notBeCatch=true; temporary storage is not the selected tray state.
    public void EnterStore()
    {
        if (!initialized) return;
        notBeCatch = true; visible = true;
        graph.Play(); RestartPlayback();
    }

    public void ReturnToPile()
    {
        if (!initialized) return;
        notBeCatch = true; visible = true;
        graph.Play(); RestartPlayback();
    }

    public void PlaySettlement()
    {
        if (!initialized) return;
        // WinAnim j36721 plays its state without cancelling the existing RandomAnim coroutine.
        graph.Play(); Play(4, .2f, 0);
    }

    void RestartPlayback()
    {
        StopRoutine();
        // Idle GetAnim(type=2, random=false, index=0), then Random.Range(0, clip.length).
        Play(0, .2f, Random.Range(0f, idle.length));
        routine = StartCoroutine(RandomAnimation());
    }

    IEnumerator RandomAnimation()
    {
        while (initialized)
        {
            // Branch is captured before yielding, like <RandomAnim>d__33.MoveNext.
            bool wasNotCaught = notBeCatch;
            PendingWait = wasNotCaught ? Random.Range(5f, 15f) : Random.Range(1f, 5f);
            yield return new WaitForSeconds(PendingWait);
            if (!sourceAnimator) continue;
            // AnimatorMa.GetAnim(random=true) uses the integer Range even for a one-clip list.
            int which = wasNotCaught ? 2 + Random.Range(0, 2) : 1 + Random.Range(0, 1);
            Play(which, .2f, 0);
            PendingWait = clips[which].length * Random.Range(1f, 3f);
            yield return new WaitForSeconds(PendingWait);
            Play(0, .4f, Random.Range(0f, idle.length));
        }
    }

    void Play(int index, float fade, float time)
    {
        for (int i = 0; i < fadeStart.Length; i++) fadeStart[i] = mixer.GetInputWeight(i);
        float total = 0; foreach (float weight in fadeStart) total += weight;
        target = index; fadeDuration = total == 0 ? 0 : fade * Mathf.Abs(1 - fadeStart[index]); fadeElapsed = 0;
        states[index].SetTime(time);
        states[index].SetSpeed(1);
        // Animancer default FadeMode.FixedSpeed adjusts duration by the remaining weight.
        // An empty layer plays its first state immediately (j8462); no bind-pose fade.
        if (fadeDuration == 0)
            for (int i = 0; i < fadeStart.Length; i++) mixer.SetInputWeight(i, i == index ? 1 : 0);
    }

    void Update()
    {
        if (!initialized || !visible || target < 0) return;
        fadeElapsed += Time.deltaTime;
        float t = fadeDuration <= 0 ? 1 : Mathf.Clamp01(fadeElapsed / fadeDuration);
        for (int i = 0; i < fadeStart.Length; i++)
            mixer.SetInputWeight(i, Mathf.Lerp(fadeStart[i], i == target ? 1 : 0, t));
    }

    void StopRoutine() { if (routine != null) StopCoroutine(routine); routine = null; PendingWait = 0; }
    void OnDestroy() { StopRoutine(); if (graph.IsValid()) graph.Destroy(); }
}
