# Pot combo praise validation

Copy `PotComboPraiseValidation.cs` into an isolated project's `Assets/Match3D/Editor/`, then run Unity with `-batchmode -executeMethod PotComboPraiseValidation.Run`. Do not add the harness to the production Assets tree. The harness invokes the existing real-board fixture and capture layout in ComboProbeValidation. Results and screenshots: `work/pot-praise-results/`, completion marker `COMPLETE`.

Source atlas/JSON/PNG: crazyCombo/client/assets/resources/spine/combe/glow_ji.*, byte-for-byte copy (Spine 3.8.99). The PNG is premultiplied: all 91,097 partial-alpha pixels have RGB <= alpha. PotComboPraiseAssetBuilder configures the existing Spine 3.8 UI shader accordingly; no image or JSON version conversion.
