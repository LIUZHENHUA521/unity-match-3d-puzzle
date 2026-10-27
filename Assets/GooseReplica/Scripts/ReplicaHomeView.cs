using UnityEngine;

// Artwork and controls are authored in Home/CollectionTab/PetsTab prefabs.
// Only the safe-area container follows device insets; button transforms stay authored.
public sealed class ReplicaHomeView : MonoBehaviour {
    public const string BachelorReward="2", EliteReward="10";
    public enum Tab { Home, Collection, Pets }
    public UnityEngine.UI.Button signIn, invite, community, challenge;
    public RectTransform artwork;
    ReplicaUIView view;
    Rect lastSafe;Vector2 lastSize;Vector2Int lastScreen;
    public static ReplicaHomeView Create(ReplicaFlow flow,RectTransform parent,Tab tab){
        var ui=ReplicaUIView.Load(tab==Tab.Home?"Home":tab==Tab.Collection?"CollectionTab":"PetsTab",parent);
        var home=ui.GetComponent<ReplicaHomeView>();home.view=ui;
        ui.Bind("Tab Home",()=>flow.ShowHomeTab(Tab.Home));
        ui.Bind("Tab Collection",()=>flow.ShowHomeTab(Tab.Collection));
        ui.Bind("Tab Pets",()=>flow.ShowHomeTab(Tab.Pets));
        if(tab==Tab.Home){
            ui.Bind("Settings",()=>flow.ShowSettings(false));ui.Bind("Rewards",()=>{if(home.artwork)flow.ShowHomeFeatureNotice("月底抽 iPad 活动尚未接入");else flow.ShowRewardInfo();});
            ui.Bind("Bachelor",()=>flow.Begin(0));ui.Bind("Elite",flow.ShowThemes);
        }
        if(home.signIn)home.signIn.onClick.AddListener(()=>{flow.HomeClick();flow.ShowHomeFeatureNotice("七日签到功能尚未接入");});
        if(home.invite)home.invite.onClick.AddListener(()=>{flow.HomeClick();flow.ShowHomeFeatureNotice("邀请有奖功能尚未接入");});
        if(home.community)home.community.onClick.AddListener(()=>{flow.HomeClick();flow.ShowHomeFeatureNotice("游戏圈功能尚未接入");});
        if(home.challenge)home.challenge.onClick.AddListener(()=>{flow.HomeClick();flow.ShowThemes();});
        if(!home.artwork)home.Layout();return home;
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
