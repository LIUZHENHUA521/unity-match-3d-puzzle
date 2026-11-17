# Pot independent highlight shaders

User request: independently replace Outline, Glow and Overlay while retaining selection and absorption feedback, then show rendered results. Existing gameplay timing, palette, resource paths and GUIDs are compatibility requirements.

Replace all three shader bodies, not translated identifiers or decompiler instructions. Outline expands clip positions using projected normals and the caller's directional offset. Glow uses a pulsing shell with view-angle opacity. Overlay uses an alpha-blended texture-aware tint, triangle pulse, depth testing and polygon offset. Names use Pot/Highlight. Stencil passes retain the caller's numeric mask 2 and never write it. WeChat continues using the stencil-free Overlay plus existing ScreenHalo path. No change to the selection/tool lifecycle or other shaders.

Alternative: replace the whole highlighter with a post-process; rejected here because it adds render targets/platform risk beyond the requested three files. Exact original-pixel reproduction is not the goal.

Review concerns incorporated: handle inverse-transpose normals, perspective clip w, aspect ratio and orthographic view vectors; preserve destination alpha; ensure occluded surfaces do not tint foreground objects. Tests use a disposable project and actual current food models, covering both stencil and depth-only paths, two tool targets, cancellation/expiry, no black/pink surface, halo visibility and occlusion. Capture fixed-view plain/selection/absorption images. Editor depth-only rendering is not WeChat device validation.

Provenance: newly authored from functional requirements, standard rendering operations and the existing C# property contract. Prior source auditing has occurred; this is not a claim of clean-room separation. Historic source evidence remains untouched. ItemHighlight and the rest of the rendering stack are outside this replacement and are not being declared independently rewritten.
