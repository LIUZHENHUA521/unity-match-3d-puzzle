# Short-transition container

User approved the “短段圆滑” candidate in the interactive comparison on 2026-11-10.

Final revision: the user explicitly selected an upright short-transition
container after identifying the unwanted lean. Stage 2 tilt is now 0 degrees.
Both stages now retain the exact first-stage camera pose and pan transform for the
entire run. Stage transitions reuse the presentation without any camera/pan pose
writes. Aligning only the pan center while rotating the camera changed the rim,
handle and backdrop projection and caused a visible twitch; that approach is
superseded. An upright collision shell does not require a straight-down camera.
Shape radii, height, transition band and item sizes stay
fixed. The side-view height gap is partly the existing hidden/parked lower-item
mechanism, and is not claimed to be eliminated by straightening the shell.
Stage 1 retains its original 25-box hierarchy, spawn sampling, pan and camera pose.
The upright second-stage shell has world origin (0, 0, -3). This fixed horizontal
translation balances the ~3-unit upward projection of the main visible band at
height ~21 under the shared 82-degree camera. It preserves all local dimensions,
world Y heights and gravity; it does not continuously follow or move the pile.
Spawn, containment, undo and shuffle share the translated coordinate conversion.
Offsets/directions use rotation only, not position conversion. Low remaining piles
must also stay visibly within the pan; this correction is not established merely
by comparing empty-pan screenshots.
This changes the solo container and necessary spawn integration, not a complete
rewrite of the game or a legal clearance. Historical reverse-engineering records
remain truthful and retained outside runtime assets.

## Approved geometry

Container-local Y height 26; 16 sides; apothem 1.15 at the bottom, 4.8 at the top;
root has zero rotation (upright). Radius is 1.15 + .05*y below y=10; between 10
and 14 use cubic Hermite between radii 1.65 and 2.1 with endpoint slopes .05 and
.225; above 14 radius is 2.1 + .225*(y-14). At y=12 the opposite-side width is
3.575. Mesh rings at 0,6,10,11,12,13,14,20,26 match the approved faceted preview.
Closed flat floor at local y=0 and safety ceiling underside at y=26; the safety
lid extends outward by 2 units to prevent thin pieces resolving through it. Neither shrinks the
specified interior. Physics uses the faceted surface, not an ideal analytic curve.

## Implementation

- Add a dedicated procedural container component with a closed, static hollow-wall
  MeshCollider plus floor and ceiling BoxColliders. The concave wall must not be
  marked convex, which would fill the playable interior. Weld shared vertices,
  orient inner faces inward, close the wall volume with annular rims (not disks
  across the cavity), and own/destroy generated mesh
  and material. A smooth non-bouncing container material is independent of old
  source-resource names. Contact layers preserve the existing picking exclusion.
- BoardEnvironment becomes a compatibility facade over the new shape, retaining
  the theme key only for game presentation. Collision geometry does not load old
  environment JSON. Both solo stages retain the boundary owner; after the first-stage items have
  cleared, disable the original subtree immediately and create the new second-stage
  shell. Restarting stage 2 creates the new shell directly. Unsupported theme identifiers must not resurrect old
  geometry.
- Generate positions from the new cross-section and height envelope. Existing
  item counts, authored layer data, visual size, input/merge timings, and rigidbody globals
  stay fixed to isolate this change. Daily envelopes above 24 are affinely remapped
  to maximum 24, anchored at minimum 1.5, including their initial visibility limit.
  The fitted world Y also drives creation scheduling. Existing non-container algorithms are not
  claimed to be independently rewritten.
- After item scale/colliders are prepared but before growth/physics registration,
  fit the complete solid collider footprint inside the new container. Use collider
  local geometry transformed into container coordinates, not Physics.SyncTransforms
  for each object. Deterministic placement adds no random draws. Preserve an already
  valid placement; adjust invalid placements; fail clearly if an item cannot fit.
  Use a conservative footprint over the item's lowest local Y to guarantee clearance
  from all 16 sides. Layered visibility registration uses the actual fitted height.
- Check undo/tool handoffs for invalid positions; fit at physical handoff if needed,
  not continuously every simulation frame. This must not clamp normal dynamics.
- Archive original container JSON and Unity metadata in Tools/LayeredPile/Evidence.
  Keep an exact first-stage copy as Resources/Match3D/FirstStageContainer.json at
  the user's request. Exclude the old second-stage environment data from runtime;
  no claim is made that the retained first-stage geometry has been rewritten.

## Validation

Use a disposable project snapshot so the user's open Unity scene is not closed.
Record baseline from pre-change source, then run the selected candidate with fixed
seeds and current model themes. Check generated mesh topology, plane normals,
interior ray hits, fully-sized/rotated items at supported scale limits, no wall
overlap at spawn, 15-item tutorial, 300-item second stage, stage-specific geometry and pan alignment, pickup,
triples, undo, shuffle/shake, Home/restart cleanup and missing old JSON dependency.
Compare pile bounds, exposed items and physics timing as descriptive measurements;
do not equate a few simulations with identical hand feel or phone performance.
Capture rendered samples where possible. Final report distinguishes editor results
from WeChat device performance and the user's subjective playtest.

No production upload or backend/admin changes are part of this task.

Spec review completed: hollow-wall topology, scaled primitive bounds, undo/shuffle
handoff order and height-30 daily envelope cases incorporated before implementation.
