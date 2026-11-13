# Pot probe activation validation

Run `python3 Tools/validation/PotProbeActivation/run.py` for the complete existing probe regression including transmission, activation and pause checks. Run again with `--focused` for concurrent pickup/reordering, matched target lifetime, storage cancellation, pool token invalidation, restart and full-pile captures.

The runner copies Assets/Packages/ProjectSettings and APFS-clones Library into `work/pot-probe-activation/project`. The focused harness is copied into that isolated project's Editor directory only. Logs are `unity.log` and `focused.log`; results are the isolated project's `work/combo-probe/return-to-bowl-results` and `work/pot-probe-results`. Both runs must finish with `COMPLETE`.
