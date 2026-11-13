using Spine.Unity;
using UnityEngine;

// Cosmetic only: never consumes gameplay randomness or changes combo/reward state.
public sealed class PotComboPraise : MonoBehaviour {
    public const string TestKey = "pot_combo_praise_ab";
    RectTransform parent;
    SkeletonGraphic graphic;
    bool treatment;
    int lastVariant;
    public SkeletonGraphic Graphic => graphic;
    public bool Treatment => treatment;
    public void Initialize(RectTransform root) { parent = root; }
    public void BeginStage(string group) {
        ResetImmediate(); lastVariant = 0; treatment = group == "B";
    }
    public void ShowCombo(int combo) {
        if (!treatment || combo < 2 || !parent || !parent.gameObject.activeInHierarchy) return;
        if (!graphic) {
            var data = Resources.Load<SkeletonDataAsset>("Match3D/PotComboPraise/PotPraiseSkeleton");
            var material = Resources.Load<Material>("Match3D/PotComboPraise/PotPraiseMaterial");
            if (!data || !material || data.GetSkeletonData(true) == null) return;
            graphic = SkeletonGraphic.NewSkeletonGraphicGameObject(data, parent, material);
            graphic.name = "Pot combo praise"; graphic.raycastTarget = false;
            graphic.rectTransform.anchoredPosition = new Vector2(65, -92);
            graphic.transform.localScale = Vector3.one * .65f;
        }
        string animation;
        if (combo < 5) animation = "appear" + combo;
        else {
            // Local rotation keeps high-combo praise varied without changing item/reward RNG.
            lastVariant = combo < 8 ? combo - 4 : lastVariant % 4 + 1;
            animation = "appear5_" + lastVariant;
        }
        graphic.gameObject.SetActive(true);
        graphic.AnimationState.ClearTracks(); graphic.Skeleton.SetToSetupPose();
        graphic.AnimationState.SetAnimation(0, animation, false);
        graphic.Update(0);
    }
    void Update() {
        if (GameManager.Instance && (GameManager.Instance.gameStatus == GameManager.GameStatus.FINISH ||
            GameManager.Instance.gameStatus == GameManager.GameStatus.FAIL)) { ResetImmediate(); return; }
        if (graphic && graphic.gameObject.activeSelf && graphic.AnimationState.GetCurrent(0)?.IsComplete == true)
            ResetImmediate();
    }
    public void ResetImmediate() {
        if (!graphic) return;
        graphic.AnimationState.ClearTracks(); graphic.Skeleton.SetToSetupPose();
        graphic.gameObject.SetActive(false);
    }
    void OnDisable() { ResetImmediate(); }
}
