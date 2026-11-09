# Mesh-accurate item picking

User approved: make the clickable shape follow the model and restrict picking to the item pick layer.

Keep the existing Pick colliders as a cheap broad phase, then intersect candidate items' visible render meshes. A hit in empty space inside a box is rejected so a rear visible mesh can win. Compare mesh hit distances, not box distances; a genuinely occluded item must remain blocked. Raycasts use only ItemColliderGeometry.PickLayer. Keep body/wall collision geometry, UI guarding, hold/slide/release semantics and visibility gates unchanged.

Cache immutable triangle data per readable mesh and component references per item. Handle transformed/multiple meshes; only enabled active renderers participate. Support skinned meshes by baking their current pose on candidate checks. Preserve collider-only legacy objects with an explicit fallback. Validate production mesh readability so unreadable artwork cannot silently reintroduce broad-box picking in a player build. Integrate readability preparation into model generation and build preprocessing.

Tests: front mesh blocks rear; box corner/real mesh gap selects rear; non-pick layers do not block; mesh distances beat broad-box order; hidden renderers and disabled pick colliders do not select; scaled/rotated meshes; stationary/slide selection remains functional; real catalog mesh coverage. Run in an isolated Unity project snapshot without disturbing the open editor. No deployment requested.

Design review resolved: added conservative mesh bounds under the original Pick root, and retained eligibility gating after nearest mesh ordering. Static triangle data uses a per-mesh spatial tree; current skinned poses are baked on demand. Both configured and fallback pool checkout restore the candidate bounds.
