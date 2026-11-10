using System;
[Serializable] public sealed class Match3DMail {
 public string id,title,body;public long sentAt,expiresAt;public int[] rewards;public bool read,claimed,expired;
 public bool HasRewards {get {if(rewards!=null)foreach(int n in rewards)if(n>0)return true;return false;}}
 public string RewardText {get {string[] names={"移出","吸附","洗牌","加时"};var parts=new System.Collections.Generic.List<string>();if(rewards!=null)for(int i=0;i<Math.Min(names.Length,rewards.Length);i++)if(rewards[i]>0)parts.Add(names[i]+" ×"+rewards[i]);return string.Join("   ",parts);}}
}
