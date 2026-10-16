using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

// Native particle prefabs + WX j31483/65504, j31485/65498, j19860/9876.
// No gameplay RNG is used here: particle autoRandomSeed belongs to the native particle system.
public sealed class ReplicaSourceToolVisual : MonoBehaviour {
    public const float FindDurationMultiplier=.7f,FindUnitSize=.0209f,FindWidth=.15f;
    public const float StoreShowZ=1.44f,StoreHideZ=-3f,StoreDepth=6.998f;
    static ReplicaSourceToolVisual instance;
    readonly List<GameObject> effects=new List<GameObject>();
    // ObjectPoolMa.GetObjByName/RemoveObj keeps transient effects by prefab name.
    // Keep this pool limited to tool VFX; gameplay ObjectUnit instances use ReplicaSourceObjectPool.
    readonly Dictionary<string,Stack<GameObject>> pooledEffects=new Dictionary<string,Stack<GameObject>>();
    GameObject background;float backgroundZ=StoreHideZ;Tween backgroundTween;
    public static int ActiveEffectCount=>instance?instance.effects.FindAll(x=>x).Count:0;
    public static int PooledEffectCount=>instance?instance.pooledEffects.Values.Sum(bucket=>bucket.Count):0;
    public static bool HasBackground=>instance&&instance.background&&instance.background.activeSelf;
    public static float BackgroundPosition=>instance?instance.backgroundZ:StoreHideZ;
    static ReplicaSourceToolVisual Ensure(){if(!instance&&ReplicaFlow.Instance)instance=ReplicaFlow.Instance.gameObject.AddComponent<ReplicaSourceToolVisual>();return instance;}
    void Awake(){instance=this;}
    void OnDestroy(){Clear();DestroyPooled();if(instance==this)instance=null;}
    public static void Reset(){if(instance)instance.Clear();}
    void Clear(){StopAllCoroutines();if(backgroundTween!=null)backgroundTween.Kill();backgroundTween=null;while(effects.Count>0)Recycle(effects[effects.Count-1]);if(background){backgroundTween=null;backgroundZ=StoreHideZ;background.SetActive(false);}backgroundZ=StoreHideZ;}
    void DestroyPooled(){foreach(var bucket in pooledEffects.Values)while(bucket.Count>0){var obj=bucket.Pop();if(obj)Destroy(obj);}pooledEffects.Clear();if(background)Destroy(background);background=null;}
    GameObject Spawn(string name){var prefab=Resources.Load<GameObject>("SourceTools/"+name);if(!prefab){Debug.LogError("Missing imported source tool visual "+name);return null;}
        GameObject obj=null;if(pooledEffects.TryGetValue(name,out var bucket))while(bucket.Count>0&&!obj){var candidate=bucket.Pop();if(candidate)obj=candidate;}
        if(!obj)obj=Instantiate(prefab);obj.name=name;obj.transform.SetParent(null,false);obj.SetActive(true);
        var ps=obj.GetComponent<ParticleSystem>();if(ps){ps.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);ps.Clear(true);}effects.Add(obj);return obj;}
    void Recycle(GameObject obj){effects.Remove(obj);if(!obj)return;var ps=obj.GetComponent<ParticleSystem>();if(ps)ps.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);obj.transform.SetParent(transform,false);obj.SetActive(false);string key=obj.name;if(!pooledEffects.TryGetValue(key,out var bucket))pooledEffects.Add(key,bucket=new Stack<GameObject>());bucket.Push(obj);}
    public static void Return(GameObject item){var owner=Ensure();if(owner&&item)owner.StartCoroutine(owner.FollowReturn(item));}
    IEnumerator FollowReturn(GameObject item){
        var fx=Spawn("FX_Tool_ReturnBack");if(!fx)yield break;fx.transform.position=item.transform.position;fx.transform.localScale=Vector3.one;fx.GetComponent<ParticleSystem>().Play(true);
        float elapsed=0;
        do{if(!item||!item.activeInHierarchy)break;fx.transform.position=item.transform.position;elapsed+=Time.unscaledDeltaTime;yield return null;}while(elapsed<ReplicaSourceTools.StoreSeconds);
        // WX j1511 is unscaledDeltaTime: follow continues while gameplay is paused.
        // Original then waits two realtime seconds before pool removal and ParticleSystem.Stop.
        yield return new WaitForSecondsRealtime(2);Recycle(fx);
    }
    public static void AutoFind(GameObject item,Vector3 slot){var owner=Ensure();if(owner&&item)owner.StartCoroutine(owner.FindBeam(item,slot+Vector3.forward*.5f));}
    IEnumerator FindBeam(GameObject item,Vector3 origin){
        var fx=Spawn("FX_Tool_AutoFind");if(!fx)yield break;fx.transform.position=origin;fx.transform.localScale=Vector3.zero;fx.GetComponent<ParticleSystem>().Play(true);
        float elapsed=0;
        do{if(!item||!item.activeInHierarchy)break;var delta=item.transform.position-fx.transform.position;if(delta.sqrMagnitude>0)fx.transform.LookAt(item.transform.position,Vector3.up);fx.transform.localScale=new Vector3(FindWidth,FindWidth,delta.magnitude*FindUnitSize);elapsed+=Time.unscaledDeltaTime;yield return null;}while(elapsed<ReplicaSourceFlight.MoveSeconds*FindDurationMultiplier);
        Recycle(fx);
    }
    public static void MessUp(){var owner=Ensure();if(owner)owner.StartCoroutine(owner.ShuffleParticles());}
    IEnumerator ShuffleParticles(){
        var fx=Spawn("FX_Tool_Messup");if(!fx)yield break;var camera=Camera.main;if(!camera){Recycle(fx);yield break;}
        // WX Odin _screenVFXPoint[ToolMessUp=2] references Transform417 MessToolPoint,
        // child of Camera359: local position (0,0,1), rotation identity, scale .1.
        // Collapse that identity anchor into the effect transform; source effect itself has local TRS 0/0/1.
        fx.transform.SetParent(camera.transform,false);fx.transform.localPosition=Vector3.forward;fx.transform.localRotation=Quaternion.identity;fx.transform.localScale=Vector3.one*.1f;
        var ps=fx.GetComponent<ParticleSystem>();ps.Play(true);
        // The source prefab is non-looping; preserve all child durations and tail particles.
        yield return null;while(fx&&ps.IsAlive(true))yield return null;Recycle(fx);
    }
    public static void SetStoreVisible(bool visible){var owner=Ensure();if(!owner)return;if(!owner.background){if(!visible)return;var prefab=Resources.Load<GameObject>("SourceTools/StoreContentBG");if(!prefab){Debug.LogError("Missing imported store background");return;}owner.background=Instantiate(prefab);owner.background.name="Source StoreContentBG";}
        owner.background.SetActive(true);
        if(owner.backgroundTween!=null)owner.backgroundTween.Kill();owner.backgroundTween=DOTween.To(()=>owner.backgroundZ,x=>{owner.backgroundZ=x;owner.LayoutBackground();},visible?StoreShowZ:StoreHideZ,ReplicaSourceTools.StoreSeconds).SetEase(Ease.OutQuad).OnComplete(()=>{if(!visible&&owner.background)owner.background.SetActive(false);});owner.LayoutBackground();
    }
    void LateUpdate(){if(background)LayoutBackground();}
    void LayoutBackground(){
        var camera=Camera.main;if(!camera||!background)return;var center=ReplicaTray.Instance?ReplicaTray.Instance.SlotPosition(3):camera.transform.TransformPoint(0,-5.04f,1.58f);
        float ratio=ReplicaTray.Instance?Vector3.Distance(ReplicaTray.Instance.SlotPosition(0),ReplicaTray.Instance.SlotPosition(1))/.71f:1.4f;
        var p=center+camera.transform.up*(backgroundZ*ratio);var screen=camera.WorldToScreenPoint(p);screen.z=StoreDepth;background.transform.position=camera.ScreenToWorldPoint(screen);
        background.transform.rotation=camera.transform.rotation;background.transform.localScale=new Vector3(2.67f,1.076612949f,.861290395f)*ratio;
    }
}
