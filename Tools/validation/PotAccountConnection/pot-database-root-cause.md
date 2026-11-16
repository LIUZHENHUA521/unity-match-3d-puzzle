# Pot account latency diagnosis — 2026-11-16

Confirmed bottleneck: the game service's 0.25 CPU quota caused substantial cgroup throttling and Node event-loop delays. SDK database-operation wall times include local callback scheduling, so the warning alone is not proof of slow Mongo query execution.

Before (version 052): busy 30-second windows showed 69–100% of CPU scheduling periods throttled; one window used 0.249 cores against a 0.25-core quota. Event-loop p99 reached 1.21 seconds in the sampled windows. The 00:29:14–00:29:44 request sample contained 27 successful logins averaging 3021ms, 25 mail lists averaging 873ms, and 20 sign-in state reads averaging 589ms.

After (version 053, deployment owned by the backend-latency chat): actual quota is 1 core; active sampled windows show 0.75–2.01% throttled periods and event-loop p99 of 23.5–31.8ms. The latest 100 successful request samples (00:33:26–00:34:05) include 24 logins averaging 253ms (200–320ms), 25 mail lists averaging 41ms (25–57ms), and 26 sign-in state reads averaging 30ms (20–46ms). One tool consume took 177ms and one refund 145ms; these two operations have too few samples for a distribution claim.

Supporting checks: six startup internal/public probes completed in 123–578ms for an entire read-plus-transaction sequence; no stable advantage from switching to public transport. SDK connection reuse already existed. No MongoSlowQuery records returned for 00:00–00:30. These do not exclude all remote database latency, but do not support an index migration or transport switch as this incident's fix.

Replay JSON parse/validation/serialization runs synchronously in the same process. An existing 40000-frame regression fixture (~20.6MB JSON) occupied ~262ms on the local Mac. This is a plausible load contributor, not a production CPU profile proving which operation consumed the quota. No additional replay changes made here.

Limits: observational windows have different live workloads; durations are server handler timing, not phone end-to-end timing. Longer-term capacity still depends on traffic. Raw sanitized samples are stored alongside this report. No player data writes or independent deployment were performed by this diagnostic chat.
