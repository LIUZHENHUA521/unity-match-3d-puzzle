using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Bind rewards immediately, reveal them only when their shooting star lands.
// Animation never grants/removes logical rewards and cannot resurrect a stale target.
public sealed class ComboProbeRewardFeedback : MonoBehaviour {
    public const float FlightSeconds = .44f, RevealSeconds = .28f;
    sealed class Flight {
        public long token; public int column; public float elapsed;
        public Image meteor, burst;
        public bool landed;
    }
    readonly List<Flight> flights = new List<Flight>();
    ComboProbeController owner;
    ComboProbeView view;
    ComboProbeAssets assets;
    RectTransform layer;
    public int ActiveCount => flights.Count;
    public int Revision { get; private set; }
    public bool IsPending(long token) {
        foreach (var flight in flights) if (flight.token == token && !flight.landed) return true;
        return false;
    }
    public float RevealScale(long token) {
        Flight flight = null;
        foreach (var current in flights) if (current.token == token) { flight = current; break; }
        if (flight == null) return 1;
        if (!flight.landed) return 0;
        float t = Mathf.Clamp01((flight.elapsed - FlightSeconds) / RevealSeconds);
        return t < .6f ? Mathf.Lerp(.25f, 1.15f, 1 - Mathf.Pow(1 - t / .6f, 3)) : Mathf.Lerp(1.15f, 1, (t - .6f) / .4f);
    }
    public void Initialize(ComboProbeController controller, ComboProbeView display, ComboProbeAssets art) {
        owner = controller; view = display; assets = art;
        layer = new GameObject("Reward star effects", typeof(RectTransform)).GetComponent<RectTransform>();
        layer.gameObject.layer = 5; layer.SetParent(view.Root, false);
        layer.anchorMin = Vector2.zero; layer.anchorMax = Vector2.one; layer.sizeDelta = Vector2.zero;
    }
    public void Play(int column, long token) {
        foreach (var current in flights) if (current.token == token) return;
        float delay = 0;
        foreach (var current in flights) if (current.column == column && !current.landed)
            delay = Mathf.Max(delay, FlightSeconds - current.elapsed + .04f);
        var flight = new Flight { token = token, column = column, elapsed = -delay };
        flight.meteor = Picture("Flying reward star", assets.meteor, new Vector2(115,69) * .8f);
        // The supplied meteor's star is at x=79; its tail extends to the left.
        flight.meteor.rectTransform.pivot = new Vector2(79f/115, .5f);
        if (owner.State.Combo >= ComboProbeChanceFeedback.ColorCombo) {
            var head = Picture("Rainbow reward star", assets.colorStar, new Vector2(32,32));
            head.transform.SetParent(flight.meteor.transform, false);
            head.rectTransform.anchorMin = head.rectTransform.anchorMax = flight.meteor.rectTransform.pivot;
            head.rectTransform.anchoredPosition = Vector2.zero;
        }
        flight.burst = Picture("Reward landing flash", assets.starFlash, new Vector2(100,100));
        flight.burst.gameObject.SetActive(false); flight.meteor.gameObject.SetActive(false);
        flights.Add(flight); Revision++;
    }
    Image Picture(string name, Sprite sprite, Vector2 size) {
        var go = new GameObject(name, typeof(RectTransform), typeof(Image)); go.layer = 5;
        go.transform.SetParent(layer, false);
        var image = go.GetComponent<Image>(); image.sprite = sprite; image.raycastTarget = false;
        image.rectTransform.sizeDelta = size; return image;
    }
    void Update() {
        if (!owner || !view.Root.gameObject.activeInHierarchy || Time.deltaTime <= 0) return;
        for (int i = flights.Count - 1; i >= 0; i--) {
            var flight = flights[i];
            if (!owner.State.Contains(flight.token) || !owner.Target(flight.token)) { Remove(i); continue; }
            flight.elapsed += Time.deltaTime;
            if (flight.elapsed < 0) continue;
            Vector2 end = layer.InverseTransformPoint(view.Slot(flight.column).transform.position);
            if (flight.elapsed < FlightSeconds) {
                Vector2 start = layer.InverseTransformPoint(view.Chances.transform.TransformPoint(new Vector3(0,88,0)));
                Vector2 control = (start + end) * .5f + Vector2.up * 65;
                float t = flight.elapsed / FlightSeconds;
                float u = t * t * (3 - 2 * t);
                flight.meteor.gameObject.SetActive(true);
                flight.meteor.rectTransform.anchoredPosition = (1-u)*(1-u)*start + 2*(1-u)*u*control + u*u*end;
                Vector2 tangent = (1-u)*(control-start)+u*(end-control);
                flight.meteor.rectTransform.localRotation = Quaternion.Euler(0,0,Mathf.Atan2(tangent.y,tangent.x)*Mathf.Rad2Deg);
                flight.meteor.color = new Color(1,1,1,Mathf.Clamp01(t/.12f));
            } else {
                if (!flight.landed) {
                    flight.landed = true; Revision++;
                    flight.meteor.gameObject.SetActive(false); flight.burst.gameObject.SetActive(true);
                }
                float t = Mathf.Clamp01((flight.elapsed - FlightSeconds) / RevealSeconds);
                flight.burst.rectTransform.anchoredPosition = end;
                flight.burst.rectTransform.localScale = Vector3.one * Mathf.Lerp(.55f,1.4f,t);
                flight.burst.color = new Color(1,1,1,(1-t)*.75f);
                if (t >= 1) Remove(i);
            }
        }
    }
    void Remove(int index) {
        var flight = flights[index];
        if (flight.meteor) { flight.meteor.gameObject.SetActive(false); Destroy(flight.meteor.gameObject); }
        if (flight.burst) { flight.burst.gameObject.SetActive(false); Destroy(flight.burst.gameObject); }
        flights.RemoveAt(index); Revision++;
    }
    public void ResetImmediate() { for (int i=flights.Count-1;i>=0;i--) Remove(i); }
    void OnDisable() { ResetImmediate(); }
}
