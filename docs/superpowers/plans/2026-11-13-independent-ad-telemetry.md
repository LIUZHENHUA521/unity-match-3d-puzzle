# Independent Ad Telemetry Implementation Plan

> **For agentic workers:** Use subagent-driven implementation with root integration review. Steps use checkbox syntax.

**Goal:** Record completed 3D rewarded ads independently of settlement, with durable retry and idempotent ingestion.

**Architecture:** Native WeChat close callback persists immutable completed events before notifying Unity. A JS outbox sends only the current owner's events to the existing authenticated goose-server; the shared admin queries those events separately from settlement rewards.

**Tech Stack:** Unity C#, Emscripten jslib, WeChat storage/request, Node ESM, CloudBase transactions, React/Ant Design.

## Contract

POST /ads/events accepts one event:
```json
{"schemaVersion":1,"eventType":"completed","owner":"64-character-player-id","impressionId":"32-character-request-id","recordId":"32-character-record-id","runId":"challenge-id","stage":2,"placement":"tool_magnet","adUnitId":"adunit-example","clientVersion":"0.1","platform":"ios","occurredAt":1794568064509}
```
Placements: tool_remove, tool_magnet, tool_shuffle, tool_time, revive_clear, revive_time. Frozen owner must equal authenticated owner. Successful response: `{ok:true, impressionId, eventType:'completed'}`. Same ID/content returns success; changed immutable content returns 409. Collection goose_ad_events uses SHA256(owner + ':' + impressionId + ':completed'), stores userId=owner plus normalized event and receivedAt. New start records carry adTelemetryVersion=1; older records remain unmarked.

## Task 1: Backend (delegated, existing server checkout)
- [ ] Add `services/goose-server/src/ad-events.mjs` and `test/ad-events.test.mjs`; test unauthenticated, invalid fields, duplicate ACK and conflicting replay using the existing transactional fixture.
- [ ] Add /ads routing to server.mjs and invitations.mjs, mapping for goose_ad_events, preserve adTelemetryVersion in records.mjs.
- [ ] Run `node --test services/goose-server/test/*.test.mjs` from crazyCombo/server; document any pre-existing failures.

## Task 2: JS outbox + Unity (root, existing Unity checkout)
- [ ] Add `Assets/Match3D/Plugins/WebGL/GooseAdTelemetry.jslib` with versioned synchronous storage; prepared capacity reservation, completed transition, per-owner delivery, retry/401 refresh/quarantine and matching ACK deletion.
- [ ] Add VM fixture tests in `Tools/wechat/test-ad-telemetry.cjs`: restart, offline replay, lost response, duplicate, account switch, storage error and quarantined event bypass.
- [ ] Modify GooseReviveAds.jslib to freeze context before show; persist native completed before callback; detach reward callback on cancel while preserving displayed-ad listener and global busy state.
- [ ] Add `Match3DAdTelemetry.cs` for session binding; initialize with existing records URL, invalidate on account refresh, notify refresh through preserved callback. Expose active record context; mark new start records adTelemetryVersion=1.
- [ ] Update Match3DRewardedVideo.cs and tool offer to pass placement/context; exclude preview/editor and incomplete playback.
- [ ] Run `node Tools/wechat/test-ad-telemetry.cjs` and `node Tools/wechat/test-revive-ads.cjs`; run C# compilation/fixture where available.

## Task 3: Admin (delegated, existing tasks checkout)
- [ ] Add independent events query module under callFunctions/adminGooseRecords with same permission; list/filter/paginated query, aggregate completion count/viewers, registered-date cohort filter and capped/incomplete diagnostics when needed.
- [ ] Add page-local ad event view in existing GooseRecords.tsx, preserving route/menu/permission; separate settlement rewards and native completion, legacy/unknown markers and bounded record association.
- [ ] Add query and frontend helper tests, run them and admin production build.

## Task 4: Integration and delivery (root)
- [ ] Review backend/client contract and security, native callback lifecycle, ACK ownership and count semantics; execute regression tests and inspect generated JS inclusion.
- [ ] Stage only task-owned changes in each repository; preserve existing dirty changes.
- [ ] Provision restricted collection/indexes and deploy admin query function and shared admin using authorized existing CloudBase MCP workflow; verify hosting and authenticated query behavior. Treat existing unrelated deployment changes carefully.
- [ ] Prepare server/client release artifacts and accurately report what remains unpublished; follow session deployment authorization.
- [ ] Save validation results, update checklist and give concise status with tests, release state and remaining boundary.
