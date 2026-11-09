# Profile texture experiment reverted — 2026-11-04

User requested reverting the PNG loader. Removed the release preprocessor,
runtime loader, initialization hook and its validation script. The previous
DXT5 profile atlas experiment is also not enabled. Original profile textures
and prefab references are retained. User resource deletions are untouched.

The PNG experiment preserved original bytes but saved only 27,806 bytes in
the compressed data package. Evidence remains under Builds/Wechat/profile-png-check.
No upload or new release was performed.
