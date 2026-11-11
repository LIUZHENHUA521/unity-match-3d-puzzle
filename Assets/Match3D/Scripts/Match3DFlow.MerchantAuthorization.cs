using System;
public partial class Match3DFlow {
    internal bool CanPromptMerchantAuthorization=>StartupComplete&&ScreenName=="home"&&!modal&&!(rewardPresenter&&rewardPresenter.IsShowing);
    internal Match3DCommonDialog ShowMerchantAuthorization(Action skip,Action authorize){
        if(!CanPromptMerchantAuthorization)return null;
        BlocksGameplay=true;
        var dialog=OpenModal("CommonDialog").GetComponent<Match3DCommonDialog>();
        dialog.Configure("开通微信收款授权","是否现在授权接收奖励？\n授权后，符合规则并审核通过的奖励可发放到微信零钱。\n也可以暂不授权，继续游戏。","暂不授权","立即授权",skip,authorize);
        return dialog;
    }
    internal void CloseMerchantAuthorization(Match3DCommonDialog dialog){if(dialog&&modal==dialog.transform)CloseModal();}
}
