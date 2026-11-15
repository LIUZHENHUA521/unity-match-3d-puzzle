using UnityEngine;
using UnityEngine.UI;

// Fixed HUD controls complement the two-finger gesture and work with a mouse.
public sealed class BoardCameraZoomControls : MonoBehaviour {
    RectTransform root;
    Button minus, plus, reset;
    Text value;
    bool replay;
    const string SkinPath = "Match3D/PotCameraZoomSkin/";
    public void Build(RectTransform header) {
        root = new GameObject("Camera zoom", typeof(RectTransform)).GetComponent<RectTransform>();
        root.SetParent(header, false);
        root.anchorMin = root.anchorMax = new Vector2(.5f, 1);
        root.anchoredPosition = new Vector2(0, -145); root.sizeDelta = new Vector2(244, 58);
        minus = MakeButton("Zoom out", "img_btn_reduce", new Vector2(-94, -2));
        reset = MakeButton("Reset camera", "img_whiteLine_cube02", Vector2.zero);
        plus = MakeButton("Zoom in", "img_btn_add", new Vector2(94, -2));
        value = new GameObject("Live zoom ratio", typeof(RectTransform), typeof(Text)).GetComponent<Text>();
        value.transform.SetParent(reset.transform, false);
        value.rectTransform.anchorMin = Vector2.zero; value.rectTransform.anchorMax = Vector2.one;
        value.rectTransform.sizeDelta = Vector2.zero;
        value.font = Match3DDefaultFont.Current; value.fontSize = 32; value.fontStyle = FontStyle.Bold;
        value.alignment = TextAnchor.MiddleCenter; value.color = Color.white;
        value.text = "1.0 x"; value.raycastTarget = false;
        minus.onClick.AddListener(() => {Match3DFlow.Instance?.PotVibrate();BoardCameraZoom.Instance?.Step(-.2f);});
        plus.onClick.AddListener(() => {Match3DFlow.Instance?.PotVibrate();BoardCameraZoom.Instance?.Step(.2f);});
        reset.onClick.AddListener(() => {Match3DFlow.Instance?.PotVibrate();BoardCameraZoom.Instance?.Restore();});
    }
    public void SetReplayMode() {
        replay=true;root.name="Pot replay speed";
        minus.onClick.RemoveAllListeners();plus.onClick.RemoveAllListeners();reset.onClick.RemoveAllListeners();
        minus.onClick.AddListener(()=>ChangeReplaySpeed(-.5f));
        plus.onClick.AddListener(()=>ChangeReplaySpeed(.5f));
        // The centre pill is a readout, not a reset/cycle button during replay.
        reset.transition=Selectable.Transition.None;reset.interactable=false;
    }
    void ChangeReplaySpeed(float step) {
        var player=PotReplayPlayer.Instance;if(!player||!player.ControlsEnabled)return;
        Match3DFlow.Instance?.PotVibrate();player.SetSpeed(player.Speed+step);
    }
    Button MakeButton(string name, string asset, Vector2 position) {
        var go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
        var rect = go.GetComponent<RectTransform>(); rect.SetParent(root, false);
        var sprite = Resources.Load<Sprite>(SkinPath + asset);
        if (!sprite) throw new System.InvalidOperationException("Missing pot camera zoom art: " + asset);
        rect.sizeDelta = sprite.rect.size; rect.anchoredPosition = position;
        var image = go.GetComponent<Image>(); image.sprite = sprite; image.color = Color.white;
        var button = go.GetComponent<Button>(); button.targetGraphic = image;
        button.navigation = new Navigation { mode = Navigation.Mode.None };
        return button;
    }
    void LateUpdate() {
        if (!root) return;
        if(replay){
            var player=PotReplayPlayer.Instance;root.gameObject.SetActive(PotReplayPlayer.Active);
            if(!PotReplayPlayer.Active)return;
            value.text=player.Speed.ToString("0.#",System.Globalization.CultureInfo.InvariantCulture)+" ×";
            minus.interactable=player.ControlsEnabled&&player.Speed>.5f;
            plus.interactable=player.ControlsEnabled&&player.Speed<3;
            return;
        }
        var zoom = BoardCameraZoom.Instance;
        bool show = zoom && zoom.Available;
        root.gameObject.SetActive(show); if (!show) return;
        value.text = zoom.Zoom.ToString("0.0", System.Globalization.CultureInfo.InvariantCulture) + " x";
        bool enabled = zoom.CanInteract && zoom.MotionReady && !zoom.SuppressPick;
        minus.interactable = enabled && zoom.Zoom > BoardCameraZoom.MinZoom + .001f;
        plus.interactable = enabled && zoom.Zoom < BoardCameraZoom.MaxZoom - .001f;
        reset.interactable = enabled && zoom.Zoom > BoardCameraZoom.MinZoom + .001f;
    }
}
