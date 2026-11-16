# 2026-11-16 replay upload recovery

## Production investigation (read-only)
- 17:00–23:01 CST: 26 stage-2 wins, no matching replay index. Seven gzip upload sessions exist; none has a corresponding incoming file in the storage listing. Direct query of one exact incoming key also returned missing.
- Seven record prefixes: f5c1a458, a0fd060a, 90e861a7, 5f9b8a6b, 87b60f07, fb56133c, 97a81a45. Rechecked their diagnostic fields and same-owner diagnostic records for today's level: none available. CLS finalization logs likewise do not identify their client failure.
- Expanded today's diagnostic search found two other records with `writeSync:fail the maximum size of the file storage limit is exceeded`, one with `云存储返回 403`, and several paused/network retries. These are NOT evidence that all seven failed for one identical reason.
- Seven historical individual root causes remain unconfirmed without client diagnostics. No production outcomes or replay indexes were modified; absent files cannot be reconstructed on server.

## Changes
- Complete gzip is committed by `.packing` → `.packed` rename, before original manifest-backed journal cleanup. Upload failures retain gzip; next retry does not recompress. Legacy `.pending` originals remain retained. Authorization is fetched after packing to avoid spending credential lifetime on compression.
- Confirm cloud readiness first; missing file returns retry/upload path, service errors retain payload. Server downloads directly from COS instead of CDN, and persists bounded finalization phase/status/error in its upload session.
- Diagnostic coroutine runs independently, rotates attempted records, and records pack/ticket/PUT/finalization, HTTP status, actual processing client version and replay version. Native COS XML error code is captured without tokens or signed URLs.
- Startup removes abandoned `.packing`/old `.upload`; synced leftovers are retried for cleanup. Under >96MiB local pressure, only explicitly failed raw journals lacking complete manifests or committed gzip are reclaimed, within current owner. Complete/quarantined wins remain. Existing 128MiB admission guard stays.
- First memory warning lowers10Hz sampling to2Hz, flushes/reduces buffer, preserves event-triggered frames and checkpoints. Severe level15 or repeated warning within30s stops for game safety; duplicate callbacks within1s ignored unless critical. Pose renderer capture avoids repeated LINQ allocations.
- Records use WeChat current package version where provided; staged Unity bundleVersion is set by publish `--version` without modifying source PlayerSettings. Diagnostic version describes the processing client, not falsely rewriting old record versions.

## Verification
- Backend full suite, native binary/upload bridge tests, release script tests pass (see logs).
- Unity30min /330objects /18001samples: raw42194777bytes, gzip1776523bytes. Tests additionally cover interruption retaining raw, atomic gzip reuse, crash after rename, server-confirmed cleanup after restart, cache pressure keeping another owner's files and complete wins, memory downgrade policy, diagnostic fairness.
- Home HTTP fixture passes complete/retry/idempotency, persisted backoff, account switch, captions, playback/download/cancel/return.
- No connected physical phone test was performed; WeChat bridge tests use a native API fixture, Unity tests use editor. This does not prove the seven historical client uploads have recovered.
- This turn changes code only; no service deployment/client upload or production replay mutation performed.

## Recovery expectation
An affected player needs the updated client and retained complete local payload. It will retry on the home screen while online. Partial/missing frames cannot become a complete winning replay. After deployment, verify actual per-record phase and ready indexes before marking incident resolved.
