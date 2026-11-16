# Implementation plan
1. Add a persistent per-owner local replay ledger, display states, and bounded diagnostic delivery.
2. Add compact frame serialization and last-emitted pose comparisons; extend checkpoint interval.
3. Make queue selection win-first and retries per record; recover legacy files safely; retain complete payloads.
4. Add authenticated diagnostic ingestion to existing replay API, preserving game outcomes.
5. Run Unity long-run, queue/state/restart and existing gzip regressions; run backend suite.
6. Audit missing online win payloads with CloudBase tools and validate any candidates before recovery. Document limits and release requirements.
