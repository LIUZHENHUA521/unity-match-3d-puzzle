using System.Collections;
using UnityEngine;

// StartLoadLevel(2) begins while the first MainLevel is active. Resource references
// retain meshes/materials; a bounded inert reserve per type amortizes native cloning.
// Mesh-only offscreen draws also warm the first-use rendering resources.
public sealed class Match3DStagePreloader : MonoBehaviour {
    public const int CopiesPerType=12;
    public const int MaximumPreparedInstances=300;
    public int PreparedInstanceCount {get;private set;}
    public bool Running {get;private set;}
    public bool Ready {get;private set;}
    public int PreparedCount {get;private set;}
    public string Environment {get;private set;}
    Coroutine worker;int version;
    readonly BoardRenderWarmup renderWarmup=new BoardRenderWarmup();
    public int RenderPreparedCount=>renderWarmup.PreparedCount;
    public void Begin(string environment,LevelLayoutConfig.Profile profile=null){
        Cancel();Environment=environment;Ready=false;PreparedCount=0;PreparedInstanceCount=0;Running=true;
        worker=StartCoroutine(Prepare(profile??LevelLayoutConfig.Get(environment),++version));
    }
    IEnumerator Prepare(LevelLayoutConfig.Profile profile,int generation){
        // Let the first visible item render before spending the preload slice.
        yield return null;
        // Instance cloning must not compete with the tutorial's growth/physics.
        while(Match3DFlow.Instance&&Match3DFlow.Instance.CreationInProgress)yield return null;
        foreach(var prefab in profile.prefabs){
            while(Time.timeScale<=0)yield return null;
            if(Time.unscaledDeltaTime>Match3DFlow.SlowCreationFrameSeconds)yield return null;
            while(Time.timeScale<=0)yield return null;
            if(generation!=version)yield break;
            if(BoardItemPool.Prepare(prefab))PreparedInstanceCount++;
            BoardLayoutSettings.ModelScale(prefab.name);
            ItemFlightMotion.ScaleForPrefab(prefab.name,Vector3.one,.71f);
            PreparedCount++;
            yield return null;
            if(Time.unscaledDeltaTime>Match3DFlow.SlowCreationFrameSeconds)yield return null;
            while(Time.timeScale<=0)yield return null;
            if(generation!=version)yield break;
            renderWarmup.Prepare(prefab);
            yield return null;
            while(Time.timeScale<=0)yield return null;
            if(generation!=version)yield break;
            GameAudioCatalog.Preload(prefab.name);
            yield return null;
        }
        // Fill a bounded reserve only after the first board has finished creating.
        // No layout sampling or activation: cached objects cannot affect stage one.
        while(Match3DFlow.Instance&&Match3DFlow.Instance.CreationInProgress)yield return null;
        int copies=Mathf.Min(CopiesPerType,MaximumPreparedInstances/Mathf.Max(1,profile.prefabs.Length));
        for(int copy=2;copy<=copies;copy++)foreach(var prefab in profile.prefabs){
            while(Time.timeScale<=0)yield return null;
            if(Time.unscaledDeltaTime>Match3DFlow.SlowCreationFrameSeconds)yield return null;
            while(Time.timeScale<=0)yield return null;
            if(generation!=version)yield break;
            if(BoardItemPool.Prepare(prefab,copy))PreparedInstanceCount++;
            yield return null;
        }
        Ready=true;Running=false;worker=null;renderWarmup.Dispose();
    }
    public void Cancel(){version++;if(worker!=null)StopCoroutine(worker);worker=null;Running=false;renderWarmup.Dispose();}
    void OnDestroy(){Cancel();}
}
