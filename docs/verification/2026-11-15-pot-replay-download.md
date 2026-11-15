# Pot replay direct download validation — 2026-11-15

## Scope

User approved whole-file download, then explicitly removed the old-recording compatibility requirement and requested direct implementation. New client uses only `download-start` plus the signed gzip object GET. No old encrypted-file fallback or migration is added. Existing server download/detail routes remain for existing callers.

- API checks login, ready state and owner/public winning-replay authorization before signing a five-minute storage URL. It does not fetch the replay bytes.
- Digest comes from the existing content-addressed gzip filename, so no database schema change is needed.
- Client downloads raw gzip without account Authorization headers, verifies SHA-256 and compressed/decompressed size limits, then deserializes.
- Expired URLs and transient download errors get one retry with a newly authorized URL. Account changes abort/reject download. Invalid tickets, unauthorized access, corruption and oversize data fail without playback.
- Home and replay-list entries keep using the shared download flow and persistent loading indicator.

## Verification

- Service tests: 149/149 passed (`node --test test/*.test.mjs`). New tests prove that unauthorized requests never obtain a URL and successful URL issuance never reads file contents.
- Full Unity Editor compilation and 21 download checks passed. Three paired runs compare all deserialized replay content with the original downloader. Tests also cover one completion callback, expired URL retry, failed retry, denied access, invalid URL, corruption, oversized payload and account switching.
- Home integration: 33 checks passed, including delayed authorization, repeated clicks, failure retry, leaving home during download, scene preparation, first-frame readiness and replay exit.
- HTTP fixture verifies 26 old requests versus 2 direct requests; account tokens are never forwarded to storage.

## Speed comparison

Fixture: 5,022,077 bytes (4.79 MiB), 1,825 frames. Original client baseline: `f0b9b45d67b0b43ff543112daae69c8fecd8c140`. Same Unity deserializer and recording for both paths. Timings include downloading, unpacking and deserialization, exclude scene preparation.

Local HTTP fixture adds 80 ms per request and an additional 80 ms per old server-to-storage fetch. It serves bytes from memory, with no bandwidth throttle. These numbers measure the removal of serial round trips under controlled latency; they are **not production or phone download timings**.

| Path | Client requests | Run 1 | Run 2 | Run 3 | Median |
| --- | ---: | ---: | ---: | ---: | ---: |
| Original 192 KiB chunks | 26 | 4.925s | 4.868s | 4.862s | 4.868s |
| Signed whole gzip | 2 | 0.400s | 0.380s | 0.351s | 0.380s |

Median time decreased by 92.2% (approximately 12.8× faster in this controlled fixture). Raw gzip also avoids Base64 expansion; payload bytes fall approximately 25% compared with Base64, excluding headers/JSON. API server no longer reads the full storage object 26 times.

Reproduce:

```sh
python3 Tools/validation/PotReplayDownload/run.py --replay work/pot-replay-gestures/project/work/pot-replay-gestures/fixture.json.gz
python3 Tools/validation/PotHomeReplay/run.py --replay work/pot-replay-gestures/project/work/pot-replay-gestures/fixture.json.gz
```

Raw local report: `work/pot-replay-download/report.json`; Unity logs and request assertions are in the same work directory. The replay fixture is an existing local recording and is not committed.

## Release status

Not deployed or uploaded. Server must deploy before the new client. Deployment validation must check the signed storage host against the mini-game request domain allowlist and verify storage CORS for any browser preview. No online configuration was modified and no physical-device measurement is claimed. Other server working-tree changes are left intact.
