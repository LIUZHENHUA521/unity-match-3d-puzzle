using Spine.Unity;
using UnityEngine;

public sealed class ComboProbeAssets : ScriptableObject {
    public Sprite panel, circle, darkStar, lightStar, starFlash, sunshine, track;
    public Sprite meteor, colorStar;
    public SkeletonDataAsset ring;
    public Material ringMaterial;
    public static ComboProbeAssets Load() => Resources.Load<ComboProbeAssets>("Match3D/ComboProbe/Assets");
}
