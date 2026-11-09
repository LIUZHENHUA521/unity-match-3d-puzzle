# Archived container reference data

Moved from `Assets/Match3D/Resources/Match3D` on 2026-11-10 when the user
selected the independently parameterized short-transition container. Original
JSON bytes and Unity metadata are retained here for provenance and comparison.
These archive files are outside Unity's Assets tree. Following the user's later
request to preserve stage 1, an exact copy of CountrysideStoveFirstStage.json and
its metadata is retained at Resources/Match3D/FirstStageContainer.json. The old
second-stage and combined reference exports remain excluded from runtime.

- `Environments/CountrysideStove.json`: prior second-stage collision hierarchy.
- `Environments/CountrysideStoveFirstStage.json`: prior first-stage hierarchy.
- `SourceEnvironment.json`: earlier environment reference export.

The replacement is `Assets/Match3D/Scripts/SoftTransitionContainer.cs`.
This archive does not imply that other game systems or assets have been audited
or independently rewritten. Do not treat the container change as legal clearance.
