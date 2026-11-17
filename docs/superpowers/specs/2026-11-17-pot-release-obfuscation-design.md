# Pot release-only method name obfuscation

Approved conversation scope: commit existing work first; change build tooling only, never gameplay source; first version name obfuscation only, no control-flow/string encryption. No upload is requested.

Use Unity 2022.3 IPostBuildPlayerScriptDLLs in the disposable release project. A small standalone Mono.Cecil tool uses Unity's bundled runtime/library and an explicit external allowlist of private algorithm methods in ChallengePlan and InputController. Keep type names, fields, signatures, Unity callbacks, public APIs, serialized data and protocols unchanged. Rewrite resolved references before renaming definitions. Reject missing/unsafe targets and string-based references; fail the build if requested protection is absent. This is deliberately limited protection, not full obfuscation of every gameplay method; compiler-generated names and strings may still disclose structure.

Tools live under Tools/wechat/obfuscation. build.py installs the callback in the staged project's Editor folder only for an explicit --obfuscate opt-in. Ordinary source/editor builds stay unchanged. Baseline and protected builds must use the same captured workspace input; record hashes and compare final compressed WASM and package bytes. A private mapping/report lives outside the minigame directory in ignored Builds, never shipped.

Validation: fixture for private direct/delegate/nested references and rejected string/unsafe/missing methods; inspect real assembly for instruction equivalence and preserved non-target names; compare seeded ChallengePlan output in isolated Unity with original and modified assemblies; complete actual WebGL/WeChat build; compare sizes. Report any unperformed device/gameplay checks explicitly. No business source refactoring or resource changes.

Implementation sequence: checkpoint; configure explicit methods; implement standalone rewriter and staged callback; integrate opt-in staging and receipt check; run fixtures and actual builds; compare sizes and verify source hashes unchanged; commit tooling separately.
