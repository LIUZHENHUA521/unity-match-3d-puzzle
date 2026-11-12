using UnityEngine;

// All distances are logical pixels at a 720-wide viewport, measured from its safe bottom.
public static class GameplayBoardLayout {
    public const float ProbeWidth = 720, ProbeHeight = 210;
    public const float ProbeArtScale = 1.15f;
    public static float RegionScale(Camera camera) => Mathf.Lerp(.7f, 1, Mathf.InverseLerp(960, 1280, Height(camera) - SafeBottom));
    // Main tray image bounds end at y=292; its visible contour is inset further.
    // Keep touch regions separate while removing excess blank space between panels.
    public static float ProbeY(Camera camera) => 296 + ProbeHeight * .5f;
    public static bool Enabled => ComboProbeController.Instance && ComboProbeController.Instance.Running;
    public static float SafeBottom => Screen.safeArea.yMin * 720f / Mathf.Max(1, Screen.width);
    public static float Height(Camera camera) => 720f / camera.aspect;
    public static float WorldPerPoint(Camera camera) => camera.orthographicSize * 2 * camera.aspect / 720;
    public static Vector3 Point(Camera camera, float x, float y, float depth) =>
        camera.ViewportToWorldPoint(new Vector3(.5f + x / 720, (y + SafeBottom) / Height(camera), depth));
    public static Rect ProbeRect => new Rect(-ProbeWidth / 2, ProbeY(Camera.main) - ProbeHeight / 2, ProbeWidth, ProbeHeight);

    public static void FitBoard(Camera camera, Vector3 bowlCenter, float radius) {
        // Preserve exactly the pre-feature projection. Only translate the camera in its plane.
        float height = Height(camera);
        camera.orthographicSize = Match3DThemePresentation.Size(camera.aspect);
        // Keep the existing raised bowl composition when tightening the toolbar gap.
        // This clearance includes the badge glow, which extends outside its hit region.
        float bottom = 328 + 224 * RegionScale(camera) + SafeBottom;
        var original = Match3DThemePresentation.SourcePosition;
        float originalCenter = Vector3.Dot(bowlCenter - original, camera.transform.up) / WorldPerPoint(camera) + height * .5f;
        float requiredCenter = bottom + radius / WorldPerPoint(camera);
        float desiredCenter = Mathf.Max(originalCenter, requiredCenter);
        float currentCenter = Vector3.Dot(bowlCenter - original, camera.transform.up);
        camera.transform.position = original + camera.transform.up * (currentCenter - (desiredCenter / height - .5f) * 2 * camera.orthographicSize);
    }
}
