public partial class Match3DFlow {
    public void ShowSignIn(){
        BlocksGameplay=true;var view=OpenModal("SevenDaySignIn");
        view.Bind("Close",()=>CloseModal());
        view.GetComponent<Match3DSignInView>().Connect(Match3DInvitations.Instance);
    }
}
