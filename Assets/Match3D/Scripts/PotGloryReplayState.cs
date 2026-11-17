public static class PotGloryReplayState {
    public static string Resolve(PotReplayService.Honor honor,string owner){
        if(!string.IsNullOrEmpty(honor?.replayId))return "ready";
        if(honor==null)return "pending";
        var local=string.IsNullOrEmpty(owner)||string.IsNullOrEmpty(honor.certificate?.recordId)?null:PotReplayLocalState.Get(owner,honor.certificate.recordId);
        // Local queue may have recovered since the last diagnostic reached the server.
        if(local?.state=="failed")return "unavailable";
        if(local?.state=="uploading")return "uploading";
        if(local?.state=="queued"||local?.state=="synced")return "pending";
        return honor.replayState=="unavailable"||honor.replayState=="uploading"?honor.replayState:"pending";
    }
    public static string Caption(string state)=>state=="unavailable"?"本局未保存":state=="uploading"?"回放同步中":"回放待同步";
    public static string Explanation(string state)=>state=="unavailable"?"本局没有可用的完整回放，通关结果和奖状不受影响。":"回放正在等待同步，请保持联网并在首页稍候。";
}
