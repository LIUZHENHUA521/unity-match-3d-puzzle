# Pot account connection validation

Run `python3 Tools/validation/PotAccountConnection/run.py`. Compiles current game sources in an isolated Unity copy and runs login-readiness regressions plus error classification checks. No real login or player writes.

2026-11-16: PASS full game compilation, existing shared-login/cancellation/timeout/retry checks, and server failure/timeout/connection/malformed response classification.

Login previously had an 8-second HTTP deadline and labeled both server 5xx responses and invalid 200 payloads as network unavailable. Login now allows 20 seconds (other operations remain 8 seconds), reports distinct errors, and logs route/status/result without response bodies or tokens. Login codes are not automatically replayed.

Live diagnosis: production 050 logs showed repeated database start/read/write/commit operations exceeding 3 seconds. Public probes connected successfully but did not receive a response within 12–15 seconds. This indicates service-path degradation; it does not establish the exact cause of a particular phone request. The separate backend-latency chat owns deployment 051 and its verification. This client change has not been built/uploaded to WeChat and cannot alter the installed client yet.
