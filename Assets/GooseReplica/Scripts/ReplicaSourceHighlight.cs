using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

// Active PickHighlight Medium (3) path recovered from the shipped WX package.
// This is deliberately independent of Toon materials: source draws separate stencil/overlay/glow/outline geometry.
[DisallowMultipleComponent, RequireComponent(typeof(Camera))]
public sealed class ReplicaSourceHighlight : MonoBehaviour {
    public const float OutlineWidth=.2f, GlowWidth=.25f, GlowIntensity=1.86f;
    public const float OverlayIntensity=.5f, OverlayMinimum=.5f, AnimationSpeed=1f;
    public static readonly Color OverlayColor=new Color(1,.9215686321f,.01568627544f,1);
    public static readonly Color GlowColor=new Color(1,.9647059441f,.1333333403f,1);
    // OnEnable j85382 indices 4..7, IEEE 0x3f350529, not rounded sqrt(0.5).
    static readonly Vector2[] Directions={new Vector2(-.70711f,.70711f),new Vector2(.70711f,.70711f),new Vector2(.70711f,-.70711f),new Vector2(-.70711f,-.70711f)};
    // Readable static meshes get smoothed normals; skins and GPU-only meshes
    // draw through their renderer without requiring a CPU mesh copy.
    sealed class Part {public Renderer renderer;public Mesh mesh;public bool drawRenderer;public Material[] overlays;}
    sealed class Group {
        public GameObject target;public readonly List<Part> parts=new List<Part>();
        public Material[] glow,outline;
    }
    readonly Group selection=new Group();readonly List<Group> toolGroups=new List<Group>();
    Camera view;CommandBuffer commands;Material mask;Mesh screenQuad;float toolsExpireAt;
    public GameObject Target=>selection.target;
    public int DrawCount {get;private set;}
    public int RendererCount=>selection.parts.Count;
    public int ToolTargetCount=>toolGroups.Count;
    public bool HasCommands=>commands!=null&&commands.sizeInBytes>0;
    public static ReplicaSourceHighlight For(Camera camera){var effect=camera.GetComponent<ReplicaSourceHighlight>();return effect?effect:camera.gameObject.AddComponent<ReplicaSourceHighlight>();}
    public static float OverlayPulse(float scaledTime)=>Mathf.Abs(.5f-Mathf.Repeat(scaledTime%1000*AnimationSpeed,1))*2*(1-OverlayMinimum)+OverlayMinimum;
    public static float GlowPulse(float scaledTime)=>1+.25f*Mathf.Sin(scaledTime*3*AnimationSpeed);
    static Material MaterialFor(string name){var shader=Resources.Load<Shader>("Replica/Highlight/"+name);if(!shader)throw new System.InvalidOperationException("Missing source highlight shader "+name);return new Material(shader){hideFlags=HideFlags.HideAndDontSave};}
    void Awake(){view=GetComponent<Camera>();}
    void Ensure(){
        if(commands!=null)return;if(!view)view=GetComponent<Camera>();mask=MaterialFor("Mask");
        screenQuad=new Mesh{name="Source highlight stencil clear",hideFlags=HideFlags.HideAndDontSave};
        screenQuad.vertices=new[]{new Vector3(-1,-1,0),new Vector3(-1,1,0),new Vector3(1,1,0),new Vector3(1,-1,0)};screenQuad.triangles=new[]{0,1,2,0,2,3};
        commands=new CommandBuffer{name="Source Highlight Medium"};view.AddCommandBuffer(CameraEvent.AfterForwardAlpha,commands);
    }
    public void Select(GameObject item){Clear();if(item)Populate(selection,item,false);}
    // j19876 + j65259: two effect slots, SetTarget/true, restart shared .3 real-time timeout (WaitForSecondsRealtime).
    public void ShowAutoFind(IReadOnlyList<GameObject> items,float duration=.3f){
        ClearTools();if(items==null)return;
        for(int i=0;i<items.Count;i++)if(items[i]){
            int slot=i%2;if(slot<toolGroups.Count){DisposeGroup(toolGroups[slot]);Populate(toolGroups[slot],items[i],true);}
            else{var group=new Group();Populate(group,items[i],true);toolGroups.Add(group);}
        }
        toolsExpireAt=Time.unscaledTime+duration;
    }
    void Update(){TickToolLifetime(Time.unscaledTime);}
    void TickToolLifetime(float unscaledNow){if(toolGroups.Count>0&&unscaledNow>=toolsExpireAt)ClearTools();}
    void Populate(Group group,GameObject item,bool lighting){
        Ensure();group.target=item;group.glow=new Material[4];group.outline=new Material[4];
        var overlayColor=lighting?new Color(.0156862736f,.869899094f,1,1):OverlayColor;
        var glowColor=lighting?new Color(.133333340f,.941176534f,1,1):GlowColor;
        for(int i=0;i<4;i++){
            var glow=group.glow[i]=MaterialFor("Glow");var outline=group.outline[i]=MaterialFor("Outline");
            // Medium .5 factor times serialized pass alpha (.5 Lighting, 1 Pick); pass offset multiplies width.
            glow.SetVector("_Glow",new Vector4(GlowIntensity*.5f*(lighting?.5f:1),GlowWidth/100*(lighting?2:1),.75f,.5f));
            glow.SetVector("_Glow2",new Vector4(OutlineWidth/100,AnimationSpeed,1,0));glow.SetColor("_GlowColor",glowColor);
            outline.SetFloat("_OutlineWidth",OutlineWidth/100);outline.SetColor("_OutlineColor",Color.black);
        }
        foreach(var renderer in item.GetComponentsInChildren<Renderer>(true)){
            var skin=renderer as SkinnedMeshRenderer;var filter=renderer.GetComponent<MeshFilter>();
            Mesh source=skin?skin.sharedMesh:(filter?filter.sharedMesh:null);if(!source)continue;
            bool drawRenderer=skin!=null||!source.isReadable;
            var part=new Part{renderer=renderer,drawRenderer=drawRenderer,mesh=drawRenderer?source:Instantiate(source)};
            if(!part.drawRenderer){part.mesh.name="Source selected geometry";part.mesh.hideFlags=HideFlags.HideAndDontSave;Smooth(part.mesh,Groups(source.vertices));}
            var materials=renderer.sharedMaterials;part.overlays=new Material[source.subMeshCount];
            for(int i=0;i<part.overlays.Length;i++){
                var material=part.overlays[i]=MaterialFor("Overlay");var original=i<materials.Length?materials[i]:null;
                material.SetColor("_OverlayColor",new Color(overlayColor.r,overlayColor.g,overlayColor.b,OverlayIntensity));
                material.SetVector("_OverlayData",new Vector4(AnimationSpeed,OverlayMinimum,1,1));
                if(original&&original.HasProperty("_MainTex")){
                    material.mainTexture=original.GetTexture("_MainTex");material.mainTextureScale=original.GetTextureScale("_MainTex");material.mainTextureOffset=original.GetTextureOffset("_MainTex");
                }
                material.SetColor("_OverlayBackColor",original&&original.HasProperty("_Color")?original.GetColor("_Color"):Color.white);
            }
            group.parts.Add(part);
        }
    }
    static int[][] Groups(Vector3[] vertices){
        var groups=new Dictionary<Vector3,List<int>>();for(int i=0;i<vertices.Length;i++){if(!groups.TryGetValue(vertices[i],out var list))groups.Add(vertices[i],list=new List<int>());list.Add(i);}
        var result=new List<int[]>();foreach(var list in groups.Values)if(list.Count>1)result.Add(list.ToArray());return result.ToArray();
    }
    static void Smooth(Mesh mesh,int[][] groups){var normals=mesh.normals;if(normals.Length!=mesh.vertexCount)return;foreach(var group in groups){var n=Vector3.zero;foreach(int i in group)n+=normals[i];n.Normalize();foreach(int i in group)normals[i]=n;}mesh.normals=normals;}
    void OnPreRender(){Rebuild();}
    public void Rebuild(){
        if(commands==null)return;commands.Clear();DrawCount=0;
        if(selection.target&&selection.target.activeInHierarchy)DrawGroup(selection);else DisposeGroup(selection);
        for(int i=toolGroups.Count-1;i>=0;i--){var group=toolGroups[i];if(group.target&&group.target.activeInHierarchy)DrawGroup(group);else{DisposeGroup(group);toolGroups.RemoveAt(i);}}
    }
    bool Visible(Part p)=>p.renderer&&p.renderer.enabled&&p.renderer.gameObject.activeInHierarchy;
    void Draw(Part p,Material material,int submesh){
        if(p.drawRenderer)commands.DrawRenderer(p.renderer,material,submesh,0);
        else commands.DrawMesh(p.mesh,p.renderer.localToWorldMatrix,material,submesh,0);
        DrawCount++;
    }
    void DrawGroup(Group group){
        commands.DrawMesh(screenQuad,Matrix4x4.identity,mask,0,1);DrawCount++;
        for(int i=0;i<4;i++){
            var d=Directions[i];group.glow[i].SetVector("_GlowDirection",new Vector4(d.x,d.y*view.aspect,0,0));
            group.outline[i].SetVector("_OutlineDirection",new Vector4(d.x*OutlineWidth/100,d.y*view.aspect*OutlineWidth/100,0,0));
        }
        foreach(var p in group.parts)if(Visible(p))for(int s=0;s<p.mesh.subMeshCount;s++)Draw(p,mask,s);
        foreach(var p in group.parts)if(Visible(p))for(int s=0;s<p.mesh.subMeshCount;s++){
            Draw(p,p.overlays[s],s);foreach(var material in group.glow)Draw(p,material,s);foreach(var material in group.outline)Draw(p,material,s);
        }
        commands.DrawMesh(screenQuad,Matrix4x4.identity,mask,0,1);DrawCount++;
    }
    // Clear ordinary selection only: AutoFind stays lit during the pick flight for source _pickMoveTime.
    public void Clear(){DisposeGroup(selection);if(commands!=null){commands.Clear();DrawCount=0;}}
    public void ClearTools(){foreach(var group in toolGroups)DisposeGroup(group);toolGroups.Clear();if(commands!=null){commands.Clear();DrawCount=0;}}
    public void ClearAll(){Clear();ClearTools();}
    static void DisposeGroup(Group group){
        group.target=null;
        foreach(var p in group.parts){if(!p.drawRenderer&&p.mesh)Release(p.mesh);foreach(var m in p.overlays)if(m)Release(m);}group.parts.Clear();
        if(group.glow!=null)foreach(var m in group.glow)if(m)Release(m);
        if(group.outline!=null)foreach(var m in group.outline)if(m)Release(m);
        group.glow=null;group.outline=null;
    }
    static void Release(Object obj){if(Application.isPlaying)Destroy(obj);else DestroyImmediate(obj);}
    void OnDisable(){ClearAll();}
    void OnDestroy(){ClearAll();if(commands!=null){if(view)view.RemoveCommandBuffer(CameraEvent.AfterForwardAlpha,commands);commands.Release();commands=null;}if(mask)Release(mask);if(screenQuad)Release(screenQuad);}
}
