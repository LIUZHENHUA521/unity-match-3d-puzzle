# Pot replay upload recovery

User approved the four-point repair: upload recovery first, cache, memory pressure, release/diagnostic version. No new publication requested in this turn.

## Evidence and limits
26 stage-2 wins ended Nov16 17:00–23:01 CST; zero corresponding replay indexes, seven gzip sessions. Storage listing and one direct missing-file query confirm no matching staging files. CLS finalization logs do not identify these seven; cannot retrospectively prove a shared client error. SDK confirms PUT upload protocol. Do not fabricate an error diagnosis or mark these records synced.

## Design
- Maintain compatible endpoints, IDs, payload format and local keys. Finalization remains authenticated and idempotent. Missing objects distinguish from service failures. Add bounded phase/version/status diagnostics. Run diagnostic coroutine independently of upload queue, including retries, but avoid active gameplay memory pressure. Report latest persisted state, round-robin retry failures to avoid blocking other records.
- Pack before requesting credentials. Persist completed gzip atomically as `.packed`; interrupted `.packing` is disposable. Keep ready/pending metadata/source until packed commit succeeds. Retain compressed queue after upload failures/restarts. For ready records keep manifest; remove recording source only after gzip rename. For legacy pending records retain source (no unvalidated migration). Retry upload failures without recompressing. Original full payloads/quarantined wins never evicted.
- Reconcile committed packed files and synced leftovers on restart. Complete uploads remove all siblings only after server confirmation. Cleanup abandoned packing files only during idle startup and owner scope. Compress complete pending recordings before network authorization, reducing retained raw cache. Cache limit still protects disk; never delete unsynced full wins to make space.
- Low memory: first warning flushes pending frames and reduces sampling from10Hz to2Hz while events and checkpoints remain. Repeated/severe warning safely stops recording; release StringBuilder capacity. Avoid temporary LINQ allocations in hot pose capture. No claim of guaranteed recovery under OS termination.
- Report actual WeChat runtime version via getAccountInfoSync plus build version stamp where available; preserve old clientVersion fallback. Publish build script injects specified version into staged PlayerSettings only. Diagnostic includes replay version, phase, HTTP status and bounded error. No credentials or signed URLs in diagnostics.

## Validation
Node bridge upload success/error/cancel and filesystem tests; backend diagnostics validation/ownership and missing object handling; Unity real-serializer queue/crash/retry/packed retention and pressure policy tests; build/version script tests. Physical device success remains a separate verification requirement if no device connection is available. Existing seven need client update and retained complete local files; server cannot reconstruct absent data.
