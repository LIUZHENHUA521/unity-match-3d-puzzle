using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public sealed class PotHomeReplayEntry : MonoBehaviour {
    readonly string[] tiers={"bachelor","elite"};
    readonly string[] ids=new string[2];
    PotHomeReplayButton[] buttons;
    PotReplayService service;
    string owner,date;int generation;bool refresh=true,querying,opening;float nextCheck;
    public void Configure(Match3DUIView view){
        buttons=new[]{new PotHomeReplayButton(view.Get<Button>("Bachelor"),false),new PotHomeReplayButton(view.Get<Button>("Elite"),true)};
        view.Bind("Bachelor",()=>Click(0));view.Bind("Elite",()=>Click(1));
        service=PotReplayService.Instance;if(service)service.Uploaded+=OnUploaded;
        StartCoroutine(Watch());
    }
    void OnUploaded(string account){if(account==owner)refresh=true;}
    void OnDestroy(){if(service)service.Uploaded-=OnUploaded;}
    static string Account(){var a=Match3DInvitations.Instance;return a&&a.Ready?a.State.inviteCode:null;}
    bool Current(int version)=>this&&isActiveAndEnabled&&version==generation&&owner==Account()&&date==Match3DFlow.Today;
    IEnumerator Watch(){
        while(true){
            string account=Account(),today=Match3DFlow.Today;
            if(account!=owner||today!=date){owner=account;date=today;generation++;ids[0]=ids[1]=null;opening=false;refresh=true;Render();}
            if(!querying&&!string.IsNullOrEmpty(owner)&&(refresh||Time.realtimeSinceStartup>=nextCheck)){
                refresh=false;querying=true;int version=generation;
                yield return Match3DStartupRunner.Run(Refresh(version),e=>Debug.LogWarning("Pot home replay: "+e.Message));
                querying=false;nextCheck=Time.realtimeSinceStartup+30;
            }
            yield return new WaitForSecondsRealtime(1);
        }
    }
    IEnumerator Refresh(int version){
        if(!service)yield break;
        var found=new string[2];int skip=0;
        long dayStart=new DateTimeOffset(DateTime.ParseExact(date,"yyyy-MM-dd",System.Globalization.CultureInfo.InvariantCulture),TimeSpan.FromHours(8)).ToUnixTimeMilliseconds();
        while(Current(version)){
            PotReplayService.Reply reply=null;yield return service.List(false,skip,null,r=>reply=r);
            if(!Current(version)||reply==null||!reply.ok)yield break;
            var rows=reply.list??new PotReplayService.Summary[0];
            foreach(var row in rows)for(int i=0;i<2;i++)if(found[i]==null&&PotHomeReplayState.Matches(row,date,tiers[i]))found[i]=row.id;
            // The endpoint orders by startedAt/createdAt descending. Older starts cannot be today's daily run.
            bool older=rows.Length>0&&rows[rows.Length-1].createdAt>0&&rows[rows.Length-1].createdAt<dayStart;
            if(!reply.hasMore||rows.Length==0||older||found[0]!=null&&found[1]!=null)break;
            skip+=rows.Length;if(skip>10000)yield break;
        }
        if(!Current(version))yield break;
        for(int i=0;i<2;i++)ids[i]=found[i];Render();
    }
    void Render(){for(int i=0;i<2;i++)buttons[i].Render(PotHomeReplayState.Won(owner,date,tiers[i]),ids[i]);}
    void Click(int index){
        if(opening)return;
        var flow=Match3DFlow.Instance;
        if(owner!=Account()||date!=Match3DFlow.Today){refresh=true;flow.Notice("正在更新今日回放，请稍候…");return;}
        if(!string.IsNullOrEmpty(ids[index])){opening=true;StartCoroutine(Open(ids[index],generation));return;}
        if(PotHomeReplayState.Won(owner,date,tiers[index])){refresh=true;flow.Notice("已通关，回放尚未同步完成，请稍候…");return;}
        flow.BeginReward(0,tiers[index]);
    }
    IEnumerator Open(string id,int version){
        var flow=Match3DFlow.Instance;flow.Notice("正在加载回放…");
        PotReplayService.Reply reply=null;
        yield return Match3DStartupRunner.Run(service.Detail(id,r=>reply=r),e=>reply=new PotReplayService.Reply{error=e.Message});
        if(!Current(version))yield break;
        opening=false;
        if(reply==null||!reply.ok){refresh=true;flow.Notice(reply?.error??"录像下载失败，请重试");yield break;}
        try{var player=flow.GetComponent<PotReplayPlayer>()??flow.gameObject.AddComponent<PotReplayPlayer>();player.Load(reply.detail);}
        catch(Exception e){flow.Notice("回放加载失败："+e.Message);}
    }
}
