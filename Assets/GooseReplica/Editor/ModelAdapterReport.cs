using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public sealed class ModelAdapterReport {
    public string requestId;
    public string inputPath;
    public string logicalId;
    public string displayName;
    public string generatedPrefabPath;
    public string previewPath;
    public string status;
    public float normalizedScale;
    public Vector3 visualSize;
    public Vector3 visualCenter;
    public Vector3 bottom;
    public float longestSide;
    public bool hasRigidbody;
    public bool hasPickCollider;
    public bool hasBodyCollider;
    public int colliderCount;
    public List<ModelAdapterIssue> issues = new List<ModelAdapterIssue>();

    public bool HasErrors {
        get { for (int i = 0; i < issues.Count; i++) if (issues[i].severity == ModelAdapterSeverity.Error) return true; return false; }
    }
    public void Add(ModelAdapterSeverity severity, string code, string message, string path = null) {
        issues.Add(new ModelAdapterIssue { severity = severity, code = code, message = message, path = path });
    }
}

public enum ModelAdapterSeverity { Pass, Warning, Error }

[Serializable]
public sealed class ModelAdapterIssue {
    public ModelAdapterSeverity severity;
    public string code;
    public string message;
    public string path;
}
