# 一键模型适配工具 Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** 为 `Assets/GooseReplica/IncomingModels` 中的模型提供一键检查、生成标准 prefab、写入运行时 catalog，并生成离线浏览器预览。

**Architecture:** 新增一个 EditorWindow 和纯编辑器适配服务。服务把输入 prefab 包装成 `logicalId/Visual/Geometry` 标准层级，生成 Rigidbody、身份组件和 Pick/Body 碰撞配置；运行时增加 catalog 合并加载，使现有按 prefab 名称查询的缩放、碰撞和音效逻辑可以直接使用。预览使用 Unity Editor 的 `PreviewRenderUtility` 和 SVG 线框投影生成本地 HTML，不依赖浏览器加载 Unity 资源。

**Tech Stack:** Unity C# Editor API、PrefabUtility、AssetDatabase、PreviewRenderUtility、JsonUtility、离线 HTML/CSS/JavaScript。

---

### Task 1: 建立适配数据模型与持久 catalog

**Files:**
- Create: `Assets/GooseReplica/Scripts/ReplicaSourceCatalog.cs`
- Create: `Assets/GooseReplica/Scripts/ReplicaSourceIdentity.cs`
- Create: `Assets/GooseReplica/Resources/Replica/ModelAdapterCatalog.json`
- Modify: `Assets/GooseReplica/Scripts/ReplicaSourceProfile.cs`
- Modify: `Assets/GooseReplica/Scripts/ReplicaSourceColliders.cs`
- Modify: `Assets/GooseReplica/Scripts/ReplicaSourceAudio.cs`

- [ ] **Step 1: Define the catalog JSON contract**

Use a versioned root and complete runtime fields:

```json
{
  "version": 1,
  "items": [
    {
      "id": "new_item",
      "displayName": "New Item",
      "prefabPath": "Assets/GooseReplica/Generated/ModelAdapter/new_item/new_item.prefab",
      "normalizedToSource": 1.0,
      "colliderProfile": { "name": "new_item", "col": [0], "realCol": [1], "nodes": [] },
      "audioProfile": { "name": "new_item", "select": 0, "combine": 0, "usePitch": false, "combineDefault": false, "goose": false }
    }
  ]
}
```

- [ ] **Step 2: Add `ReplicaSourceIdentity`**

Create a `MonoBehaviour` with serialized `logicalId`, `displayName`, and `catalogVersion`, plus read-only properties. The adapter-generated root uses this component; runtime gameplay does not branch on it.

- [ ] **Step 3: Add case-insensitive catalog loading and lookup**

`ReplicaSourceCatalog` loads `Resources/Replica/ModelAdapterCatalog`, validates `version`, merges items into dictionaries using `StringComparer.OrdinalIgnoreCase`, and exposes `TryGetSize`, `TryGetColliderProfile`, and `TryGetAudioProfile`. Catalog entries override legacy entries with the same ID and log one conflict warning. Add `ClearCache()` and call it from editor reload hooks.

- [ ] **Step 4: Merge catalog lookups into existing runtime helpers**

Update `ReplicaSourceProfile.ModelScale`, `ReplicaSourceColliders` profile loading, and `ReplicaSourceAudio` profile loading to consult the catalog after legacy JSON. Keep the existing legacy behavior unchanged when no catalog entry exists. Ensure `ReplicaSourceColliders` accepts the existing `int[]` child-index paths and preserves Pick layer 7, Body layer 10, and `trigger=false` for Pick colliders.

- [ ] **Step 5: Add loader tests and commit**

Add an editor validation method that loads the catalog, verifies case-insensitive duplicate handling and all fields, then run Unity compilation/import validation. Commit with `feat: add model adapter catalog runtime contract`.

### Task 2: Implement model inspection and standard prefab generation

**Files:**
- Create: `Assets/GooseReplica/Editor/ModelAdapterService.cs`
- Create: `Assets/GooseReplica/Editor/ModelAdapterReport.cs`
- Create: `Assets/GooseReplica/Editor/ModelAdapterRules.cs`
- Modify: `Assets/GooseReplica/Editor/OriginalModelImporter.cs` only if shared normalization helpers are extracted without changing current output.

- [ ] **Step 1: Implement source scanning**

Given an input asset under `Assets/GooseReplica/IncomingModels`, instantiate a temporary copy and collect every enabled Renderer, MeshFilter, SkinnedMeshRenderer, Transform, Rigidbody, Collider, tag, layer, material, and mesh. For SkinnedMeshRenderer call `BakeMesh` before bounds calculation. Reject non-finite values and non-uniform root scale.

- [ ] **Step 2: Implement diagnostics**

Return `Pass`, `Warning`, and `Error` entries for renderer/material/shader validity, bounds, origin/axis fields, logical ID format and conflicts, Rigidbody/tag/physics defaults, Pick/Body grouping, collider shape and path validity, Pick trigger state, and MeshCollider rejection. Use the `.43f / maxBounds` normalization used by `OriginalModelImporter`.

- [ ] **Step 3: Implement logical ID and update policy**

Normalize user ID or filename to `[A-Za-z0-9_-]+`, compare case-insensitively against existing generated prefabs and catalog entries, and expose `Create` versus explicit `Update` mode. Do not touch `ThemeLibrary` or `SourceLevelCatalog`.

- [ ] **Step 4: Generate the staged prefab**

Create `logicalId` root with `Collectable` tag, standard Rigidbody (`mass=.15`, `drag=.6`, `angularDrag=.8`, Interpolate, ContinuousDynamic, non-kinematic, collision enabled), `ReplicaSourceIdentity`, and first child `Visual`. Put original content under `Visual/Geometry`; keep `Visual.localPosition` and rotation zero, applying center offset below it. Create/validate `Pick` and `Body` subtrees and serialize their child-index paths and primitive collider shapes into the catalog profile.

- [ ] **Step 5: Implement staging and rollback**

Write to `Generated/ModelAdapter/.staging/<requestId>`, generate report and catalog candidate, then replace the target directory only after errors are clear. Back up an existing target/catalog before update. On failure remove staging and write the failure report to `Generated/ModelAdapter/_diagnostics/<requestId>.json`.

- [ ] **Step 6: Add adapter validation and commit**

Create `ModelAdapterValidation.cs` to load generated prefabs and assert identity, tag, Rigidbody, renderer, bounds, Pick/Body groups, collider paths, catalog entry, and runtime lookups. Commit with `feat: generate validated model adapter prefabs`.

### Task 3: Build the Unity EditorWindow workflow

**Files:**
- Create: `Assets/GooseReplica/Editor/ModelAdapterWindow.cs`

- [ ] **Step 1: Add menu and input selection**

Add `Goose/Model Adapter`. The window lists prefab/model assets under `Assets/GooseReplica/IncomingModels`, exposes logical ID, display name, target theme, Create/Update mode, and a “Generate collider draft” toggle.

- [ ] **Step 2: Add scan and generation actions**

Buttons: `Scan`, `Generate collider draft`, `Generate adapter prefab`, and `Open browser preview`. Disable generation when errors remain; show the report grouped by severity and the exact asset path for each issue.

- [ ] **Step 3: Add editor refresh behavior**

Refresh the asset list after `AssetDatabase.Refresh`, clear catalog caches when the window reloads, and display the last generated report/diagnostics path.

### Task 4: Generate the offline browser preview

**Files:**
- Create: `Assets/GooseReplica/Editor/ModelAdapterPreview.cs`
- Create: `Assets/GooseReplica/Editor/ModelAdapterHtml.cs`
- Create: `Assets/GooseReplica/Editor/ModelAdapterPreviewTemplate.html`

- [ ] **Step 1: Render the model views**

Use `PreviewRenderUtility` on a temporary generated prefab and export perspective, top, front, and side PNGs. Use baked SkinnedMesh bounds and the same `Visual/Geometry` transforms as the report.

- [ ] **Step 2: Project collision overlays**

For Box/Sphere/Capsule, calculate transformed sample points in the same view camera and emit SVG line overlays for visual bounds, Pick, Body, origin, and axes. Use distinct colors and include a legend.

- [ ] **Step 3: Write the self-contained HTML**

Write `index.html`, images, SVG overlays, and `report.json` into the adapter output directory. The page shows model views, dimensions, normalized scale, origin offsets, Rigidbody values, collider counts, logical ID and severity-filtered diagnostics.

- [ ] **Step 4: Open and validate the preview**

Open the file with `Application.OpenURL(new Uri(path).AbsoluteUri)` and add an editor validation that checks all referenced files exist and the HTML contains the generated logical ID/report path.

### Task 5: End-to-end verification and documentation

**Files:**
- Modify: `Assets/GooseReplica/Editor/VisualSceneBatch.cs` only to add the adapter validation command if it follows existing batch conventions.
- Create: `Assets/GooseReplica/Editor/ModelAdapterEndToEndValidation.cs`

- [ ] **Step 1: Verify representative inputs**

Run the adapter against a no-collider model, primitive collider model, multi-node Pick/Body model, and SkinnedMeshRenderer model. Verify expected warnings/errors and generated report paths.

- [ ] **Step 2: Verify runtime behavior**

Load the generated catalog and prefab in an editor fixture, perform an ignore-Trigger Pick raycast, a Body collision simulation, `ModelScale` lookup, collider profile lookup, and audio profile lookup.

- [ ] **Step 3: Run regression validations**

Run existing source import, object pool, selection, and theme validation as the baseline; then run adapter-specific validation with a temporary catalog/theme fixture. Do not publish or upload a build.

- [ ] **Step 4: Commit final implementation**

Run `git diff --check`, record Unity validation output in `Temp/model-adapter-validation.txt`, and commit with `feat: add one-click model adapter workflow`.
