# Pot internal rename — 2026-11-13

Renamed internal Unity scripts, WebGL source files, admin components/API helpers and customer-support service modules from Goose/goose to Pot/pot. Updated their references and tests. Preserved pre-existing working-tree changes.

Compatibility boundaries retained: database collections/fields, goose3d identifiers, HTTP routes and headers, admin paths and permissions, cloud function names, service directories/deployment names, Unity/JS bridge symbols and callbacks, saved preferences, artwork names and historical evidence. Unity .meta files were moved unchanged; their hashes were verified.

Validation:

- Unity runtime and editor C# assemblies compiled using Unity's Roslyn compiler and existing Bee compiler arguments, with renamed paths and current scripts. Outputs were written to a temporary directory. This was not a full Unity import, WebGL build or device test.
- Admin Vite production build succeeded in a temporary output directory; 7 related tests passed.
- Server TypeScript noEmit check succeeded; 25 customer-support, voucher and welcome-message tests passed.
- All 34 WeChat bridge tests passed with the existing Goose bridge symbols.
- git diff --check passed in all three repositories.

No deployment was performed. The pot naming preference is recorded in the global Codex AGENTS.md and project instructions.
