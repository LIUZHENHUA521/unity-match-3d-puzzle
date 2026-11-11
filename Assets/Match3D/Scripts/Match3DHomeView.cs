using UnityEngine;

// Artwork and controls are authored in Home/CollectionTab/PetsTab prefabs.
// Safe-area layout preserves authored button positions; entrance motion returns to them.
public sealed class Match3DHomeView : MonoBehaviour {
    public const string BachelorReward="2", EliteReward="10";
    public enum Tab { Home, Collection, Pets }
    public UnityEngine.UI.Button signIn, invite, community;
    public RectTransform artwork;
    Match3DUIView view;
    Rect lastSafe;Vector2 lastSize;Vector2Int lastScreen;
#if UNITY_WEBGL && !UNITY_EDITOR
    static Sprite originalBackground;
    static void RestoreOriginalBackground(Match3DUIView ui){
        if(!originalBackground){
            var jpg=Resources.Load<TextAsset>("Match3D/HomeBackgroundOriginal");
            if(!jpg)return;
            var texture=new Texture2D(2,2,TextureFormat.RGB24,false);
            bool loaded=ImageConversion.LoadImage(texture,jpg.bytes,true);
            Resources.UnloadAsset(jpg);
            if(!loaded){Destroy(texture);return;}
            texture.name="Home background original";
            texture.wrapMode=TextureWrapMode.Clamp;
            texture.filterMode=FilterMode.Bilinear;
            originalBackground=Sprite.Create(texture,new Rect(0,0,texture.width,texture.height),new Vector2(.5f,.5f),100);
        }
        foreach(var image in ui.GetComponentsInChildren<UnityEngine.UI.Image>(true))
            if(image.sprite&&image.sprite.name=="img_Homepage_bg2")image.sprite=originalBackground;
    }
#endif
    public static Match3DHomeView Create(Match3DFlow flow,RectTransform parent,Tab tab){
        var ui=Match3DUIView.Load(tab==Tab.Home?"Home":tab==Tab.Collection?"CollectionTab":"PetsTab",parent);
#if UNITY_WEBGL && !UNITY_EDITOR
        RestoreOriginalBackground(ui);
#endif
        var home=ui.GetComponent<Match3DHomeView>();home.view=ui;
        ui.Bind("Tab Home",()=>flow.ShowHomeTab(Tab.Home));
        ui.Bind("Tab Collection",()=>flow.ShowHomeTab(Tab.Collection));
        ui.Bind("Tab Pets",()=>flow.ShowHomeTab(Tab.Pets));
        if(tab==Tab.Home){
            ui.Bind("Settings",()=>{var existing=home.GetComponentInChildren<Match3DHomeMenu>();if(existing){Destroy(existing.gameObject);return;}var menu=Match3DUIView.Load("HomeMenu",home.transform);var controller=menu.GetComponent<Match3DHomeMenu>();controller.FollowArtwork(home.artwork);controller.Connect(flow);});ui.Bind("Rewards",()=>{if(home.artwork)flow.ShowHomeFeatureNotice("月底抽 iPad 活动尚未接入");else flow.ShowRewardInfo();});
            ui.Bind("Bachelor",()=>flow.BeginReward(0,"bachelor"));ui.Bind("Elite",()=>flow.BeginReward(0,"elite"));
        }
        if(home.signIn){home.signIn.gameObject.SetActive(true);home.signIn.onClick.AddListener(()=>{flow.HomeClick();flow.ShowSignIn();});home.signIn.gameObject.AddComponent<Match3DSignInBadge>();}
        if(home.invite)home.invite.onClick.AddListener(()=>{flow.HomeClick();flow.ShowInvites();});
        if(home.community)home.community.onClick.AddListener(()=>{flow.HomeClick();Match3DGameCommunity.Open();});
        if(!home.artwork)home.Layout();
        if(tab==Tab.Home){
            Match3DHomeButtonMotion.Attach(ui.Get<UnityEngine.UI.Button>("Elite"),.08f,true);
            Match3DHomeButtonMotion.Attach(ui.Get<UnityEngine.UI.Button>("Bachelor"),.18f,true);
            Match3DHomeButtonMotion.Attach(ui.Get<UnityEngine.UI.Button>("Settings"),0,false);
            Match3DHomeButtonMotion.Attach(home.invite,.04f,false);
            Match3DHomeButtonMotion.Attach(home.community,.08f,false);
        }
        return home;
    }
    void LateUpdate(){
        if(!view)return;
        if(((RectTransform)transform).rect.size!=lastSize || Screen.safeArea!=lastSafe || lastScreen!=new Vector2Int(Screen.width,Screen.height))Layout();
    }
    void Layout(){
        if(!view)return;
        // Device Simulator can briefly report editor viewport dimensions during creation.
        if(Screen.width<=0 || Screen.height<=0 || Screen.safeArea.xMax>Screen.width || Screen.safeArea.yMax>Screen.height)return;
        lastSize=((RectTransform)transform).rect.size;lastSafe=Screen.safeArea;
        lastScreen=new Vector2Int(Screen.width,Screen.height);
        float scale=lastSize.x/Mathf.Max(1,Screen.width);
        var safe=view.Get<RectTransform>("Safe content");
        safe.offsetMin=new Vector2(lastSafe.xMin*scale,lastSafe.yMin*scale);
        safe.offsetMax=new Vector2(-(Screen.width-lastSafe.xMax)*scale,-(Screen.height-lastSafe.yMax)*scale);
        if(artwork){
            Canvas.ForceUpdateCanvases();
            float fit=Mathf.Min(1,Mathf.Min(safe.rect.width/artwork.sizeDelta.x,safe.rect.height/artwork.sizeDelta.y));
            artwork.localScale=Vector3.one*Mathf.Max(.001f,fit);
            return;
        }
        var footer=view.Get<RectTransform>("Footer background");
        // Match the authored tab-bar height rather than hard-coding button positions.
        var bar=(RectTransform)view.Get<Transform>("Tab Home").parent;
        footer.sizeDelta=new Vector2(footer.sizeDelta.x,bar.rect.height+lastSafe.yMin*scale);
    }
}
