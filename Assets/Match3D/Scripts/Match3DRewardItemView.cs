using UnityEngine;
using UnityEngine.UI;

public sealed class Match3DRewardItemView : MonoBehaviour {
    public Image icon;
    public Text caption;
    public RectTransform glow;
    void Update() {
        if (glow) glow.Rotate(0, 0, -12 * Time.unscaledDeltaTime);
    }
}
