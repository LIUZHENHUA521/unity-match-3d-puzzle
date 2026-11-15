# Pot home replay button validation

The home challenge buttons now keep their large 清北局 / 本科局 title and original colour. Only the lower reward row changes: the original packet/reward artwork for unfinished tiers, 回放待同步 after a local daily second-stage win, and 查看回放 after the server returns a matching ready winning recording. Each tier is independent. Upload finalization (including both retry/already-ready branches) notifies the home, and a bounded periodic refresh handles restart or delayed synchronization. Ready clicks download the matching recording and enter the existing replay player; download failure preserves the home.

The current server `mine` list already filters `owner` and `ready:true`. The client follows pagination and filters date/tier/stage/status/resource compatibility; `createdAt` is used only to stop scanning before today's start because the endpoint sorts it descending. No backend changes are required. Account/day checks discard stale asynchronous results. Ready state is never inferred from a local marker or a successful storage PUT.

## Photoshop assets

Source: the user's open `主界面-排版02已命名(1).psd`, 720 × 1565. The source document was not changed. Exported eight individual PNGs from temporary copies into `Assets/Match3D/Resources/Match3D/PotHomeReplaySkin/`. Positions below are top-left pixels in the original document. The reward caption combines the three original text layers; title, background and packet remain independent. Runtime status captions use the packaged Chinese font with matching tier-colour outline.

| Asset suffix | x | y | width | height |
| --- | --- | --- | --- | --- |
| EliteBase | 145 | 1080 | 441 | 182 |
| EliteTitle | 242 | 1078 | 238 | 90 |
| ElitePacket | 192 | 1168 | 50 | 61 |
| EliteReward | 243 | 1174 | 299 | 42 |
| BachelorBase | 147 | 1268 | 437 | 182 |
| BachelorTitle | 238 | 1269 | 243 | 90 |
| BachelorPacket | 194 | 1354 | 50 | 61 |
| BachelorReward | 245 | 1362 | 283 | 41 |

## Validation

`python3 Tools/validation/PotHomeReplay/run.py --replay <existing canonical v2 gzip>`: 24 Unity play-mode checks passed. Tests use a local HTTP fixture and an isolated HEAD snapshot plus feature files; unrelated in-flight same-games and cleanup changes are excluded. Runtime test accounts and temporary queue files are isolated. The existing real recording is used for the final download-to-player navigation check.

Coverage: separate sprites and original captions; pending-only local wins; account/date and tier separation; failed/incomplete/unsupported recordings excluded; upload retry and already-ready notifications; winning replay on page two (skip 20); both ready captions; correct IDs; packet hiding; download failure with one request despite repeated click; account switching and stale in-flight replies; reopening from cloud state; player navigation and clean exit. Reviewed generated 720 × 1565 normal/pending/ready screenshots. No physical-device check is claimed.

Reports and screenshots: `work/pot-home-replay/project/work/pot-home-replay/` (`checks.txt`, `requests.json`, `pot-home-normal.png`, `pot-home-pending.png`, `pot-home-ready.png`).
