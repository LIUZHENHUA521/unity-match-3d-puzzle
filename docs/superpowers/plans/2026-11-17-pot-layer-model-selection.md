# Pot layer model selection implementation plan

Goal: Implement the approved optional exact model set on each daily layer, across all eight 26-model themes.
Architecture: Add potModelIndices to the existing layer contract. Reuse shared fixed-opening publication and existing permissions. Empty selections preserve the prior random path.
Tech stack: Unity C#, React/Ant Design, Node.js CloudBase and game server.

- [x] Update ChallengeLayer and Match3DDailyLevel: validate unique in-range indices matching types; initialize the shuffle list from the fixed set only when nonempty. Keep old random calls unchanged for empty selections.
- [x] Update identical dailyLevelContract.js / daily-level-contract.cjs: normalize optional selections, validate imported per-layer selected sets and triples, set all theme counts to 26. Cover malformed arrays, round-trip, old configs and paired variants with Node tests.
- [x] Export Unity prefab-order catalog (index, name, thumbnail), then add a reusable Ant Design multiple selector to PotDailyLevels. Disable types while selected; clear on theme changes; retain count validation. Build the admin.
- [x] Validate Unity generation across all themes with empty, one, subset and full selections, then run PotEditorBeta/run.py and build the full current-workspace WebGL authoring player.
- [x] Inspect all target project changes; commit and push deployable source. Deploy game contract dependency, adminGooseRecords, versioned authoring player and admin using MCP. Verify deployed status, remote hashes and public health; do not upload WeChat.
