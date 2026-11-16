# Pot layer model selection — 2026-11-17

User-approved behavior: empty model selection preserves random type selection; nonempty selection is exact and sets the layer type count. All eight themes contain 26 models. Switching theme clears selections. Existing published openings are not modified.

## Validation
- Cloud function suite: 59 passed, including save/import/publication and paired variant selection preservation.
- Game service suite: 174 passed. Shared contracts are byte-identical.
- Preview bridge / paired preview: 6 passed.
- Unity catalog export: 208 actual prefab thumbnails and names; eight pools and layer weights validated; 100 seeds each for empty, single, subset and all-26 selections plus invalid-list rejection.
- PotEditorBeta/run.py: all eight themes, both entry tiers, first stage, second stage initialization and retry passed (editor-checks.txt).
- Headless Chrome using real WebGL build: each of eight themes generated layers with exact sets [0,23,25] and [24]. All exported snapshots passed backend validation. Final build with latest tray changes retested on pot-meat.
- Actual React page exercised in Chromium with a local API fixture: selection, types synchronization, saving indices, switching theme and clearing selection passed. Production admin build succeeded.

## Publication and coverage
- Unity master: cbaab11b (all current model/background/editor and generation changes), c0985d48 (final tray orientations after user stopped editing).
- Tasks: 183f89c (cloud function contract), 309d0e7 (admin source, catalog, thumbnails and complete built admin).
- Game service: a2fdc5e. goose-3d-pvp-063 normal, 100% flow; BuildId 2612960376. Health and invitation readiness HTTP 200, database and login ready.
- adminGooseRecords updated and confirmed Active/success at 2026-11-17 01:38:15 (service-reported time).
- Preview: /admin/pot-preview/20261117-pot-layer-models/ — all 21 remote files match local SHA-256.
- Admin: /admin/ — index-C1upa-uZ.js; entry assets and all 208 thumbnails (211 files) match local SHA-256.
- build-inputs.json confirms the final live model catalog, tray settings and generation source are identical to staged inputs. Target source directories clean after commits. Unrelated dirty crazyCombo root-service, other cloud functions and H5 work were outside these deployment targets; no targeted changes omitted.
- No WeChat upload and no live daily level publication performed; administrators choose/save/publish levels using the new controls.

Admin: https://cloud1-1g0u68ow59705e3f-1312757046.tcloudbaseapp.com/admin/
