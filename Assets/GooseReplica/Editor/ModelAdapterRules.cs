using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public static class ModelAdapterRules {
    // Stored Prefab size and gameplay size are separate coordinate spaces.
    public const float SourceLongestSide = .43f;
    // Approximately the median longest side of the original countryside items.
    public const float DefaultGameplayLongestSide = 2.1f;
    public const float DefaultGameplayScale = DefaultGameplayLongestSide / SourceLongestSide;
    public const float BoundsTolerance = .05f;

    public static bool IsFinite(float value) => !float.IsNaN(value) && !float.IsInfinity(value);
    public static bool IsFinite(Vector3 value) => IsFinite(value.x) && IsFinite(value.y) && IsFinite(value.z);
    public static bool IsUniform(Vector3 value) => Mathf.Abs(value.x - value.y) < .0001f && Mathf.Abs(value.x - value.z) < .0001f;
    public static string NormalizeId(string value) {
        if (string.IsNullOrEmpty(value)) return string.Empty;
        var chars = new List<char>(value.Length);
        foreach (var c in value.Trim()) {
            if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9') || c == '_' || c == '-') chars.Add(c);
            else if (char.IsWhiteSpace(c)) chars.Add('_');
        }
        return new string(chars.ToArray());
    }
    public static bool IsValidId(string value) {
        if (string.IsNullOrEmpty(value)) return false;
        for (int i = 0; i < value.Length; i++) {
            var c = value[i];
            if (!((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9') || c == '_' || c == '-')) return false;
        }
        return true;
    }
    public static string Format(float value) => value.ToString("0.####", CultureInfo.InvariantCulture);
}
