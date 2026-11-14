# Pot configured layer container

User approved configuration-driven layers. Crowding is intentional input for later tuning; do not estimate capacity, increase heights from item counts or model size, or add layers automatically.

With one to four layers, preserve the original 26-unit container and original height mapping (authored tops over 24 are mapped into the original spawn range). With five or more layers, preserve the first four mapped layer tops and lower geometry, then start a hollow vertical cylinder at the fourth mapped layer top with that cross-section radius. Higher layers use their configured absolute top heights. Ceiling is the higher of 26 and the last configured top plus the original fixed two-unit clearance. No divider at the join.

Example: layer tops 14,18,20,22 retain the original cone; adding 26 starts a cylinder at 22 and sets ceiling 28; adding 30 sets ceiling 32. Counts never change these dimensions. Visibility and physics retain the original height-band logic. Shared saved openings rebuild the shell from the same layer configuration, without deriving height from settled object positions. Previous automatically expanded openings require regeneration to adopt this layout.

Reuse existing admin layer count, type count and top-height fields. No admin changes or deployment are needed for this revision. Existing snapshot coordinate limits remain compatible.

Validate four/five/six-layer geometry, count independence, continuous side walls, no joining disk, stage reset, generation, camera picking, saved opening restoration and revealing remaining objects after a triple is removed. Crowding/overlap is not a reason to auto-resize or reject the configured board.

Local validation passed: four/five/six-layer geometry at 3/150/300 items per layer; 99 runtime checks across all three themes at 345/405/465 total items. Verified configured ceilings 26/28/32, picking, exact saved-state restoration, stage reset and remaining visibility after removal. Inspected the five-layer portrait capture at work/pot-capacity/pot-corrected-game.png. No publication performed.
