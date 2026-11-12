using Spine.Unity;
using UnityEngine;

public sealed class ComboProbeAssets : ScriptableObject {
    public Sprite panel, circle, darkStar, lightStar, sunshine, track;
    public SkeletonDataAsset ring;
    public Material ringMaterial;
    public static ComboProbeAssets Load() => Resources.Load<ComboProbeAssets>("Match3D/ComboProbe/Assets");
}
