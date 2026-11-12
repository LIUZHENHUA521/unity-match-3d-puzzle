using UnityEngine;

// All distances are logical pixels at a 720-wide viewport, measured from its safe bottom.
public static class GameplayBoardLayout {
    public const float ProbeWidth = 720, ProbeHeight = 224, StoreHeight = 120;
    public static float RegionScale(Camera camera) => Mathf.Lerp(.7f, 1, Mathf.InverseLerp(960, 1280, Height(camera) - SafeBottom));
    public static float StoreY(Camera camera) => 286 + StoreHeight * RegionScale(camera) * .5f;
    public static float ProbeY(Camera camera) => StoreY(camera) + (StoreHeight + ProbeHeight) * RegionScale(camera) * .5f + 16;
    public static bool Enabled => ComboProbeController.Instance && ComboProbeController.Instance.Running;
    public static float SafeBottom => Screen.safeArea.yMin * 720f / Mathf.Max(1, Screen.width);
    public static float Height(Camera camera) => 720f / camera.aspect;
    public static float WorldPerPoint(Camera camera) => camera.orthographicSize * 2 * camera.aspect / 720;
    public static Vector3 Point(Camera camera, float x, float y, float depth) =>
        camera.ViewportToWorldPoint(new Vector3(.5f + x / 720, (y + SafeBottom) / Height(camera), depth));
    public static Rect ProbeRect { get { float s = RegionScale(Camera.main); return new Rect(-ProbeWidth * s / 2, ProbeY(Camera.main) - ProbeHeight * s / 2, ProbeWidth * s, ProbeHeight * s); } }
    public static Rect StoreRect { get { float s = RegionScale(Camera.main); return new Rect(-190 * s, StoreY(Camera.main) - StoreHeight * s / 2, 380 * s, StoreHeight * s); } }

    public static void FitBoard(Camera camera, Vector3 bowlCenter, float radius) {
        float height = Height(camera), bottom = ProbeY(camera) + ProbeHeight * RegionScale(camera) * .5f + 24 + SafeBottom;
        float topInset = Match3DGameplayHUD.HeaderInset(720, Screen.width, Screen.height, Screen.safeArea);
        float top = Mathf.Max(bottom + 100, height - Mathf.Lerp(130, 170, Mathf.InverseLerp(960, 1280, height)) - topInset);
        float diameter = radius * 2;
        // Reserve the entire bowl on short screens as well; never move physics or daily opening data.
        float worldPerPoint = Mathf.Max(Match3DThemePresentation.Size(camera.aspect) * 2 / height,
            diameter / Mathf.Max(100, top - bottom), diameter / 690);
        camera.orthographicSize = worldPerPoint * height * .5f;
        float desiredCenter = (bottom + top) * .5f;
        var original = Match3DThemePresentation.SourcePosition;
        float currentCenter = Vector3.Dot(bowlCenter - original, camera.transform.up);
        camera.transform.position = original + camera.transform.up * (currentCenter - (desiredCenter / height - .5f) * 2 * camera.orthographicSize);
    }
}
