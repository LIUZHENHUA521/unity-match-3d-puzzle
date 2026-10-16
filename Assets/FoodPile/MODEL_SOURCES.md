# Imported model sources

Source repository: https://github.com/Yuming0929/goose-catch
Source files: src/assets/{cheese,ice-cream,sandwich,coockie-man,toast,pancake-big,sandwich-toast,hotdog}.glb

These are converted copies, not newly authored artwork. The GLB base-color textures and original mesh UVs are preserved. Geometry is centered, scaled to 0.46 Unity units at its longest dimension, and converted from glTF handedness. Unity prefabs use convex mesh colliders and rigidbodies. The source webpage uses seven of these models; the unused toast is included as the eighth prototype type.

No asset license was established from the source checkout. This attribution does not grant commercial redistribution rights; verify the original model license before publishing the assets in a game.

Tools/convert_food_models.py regenerates editor-only intermediate JSON and textures from a local source checkout. Level > Build Food Pile Challenge creates native meshes, materials, prefabs, settings and a separate scene. The game does not require a runtime glTF plugin.

## Meshy strawberry (2026-10-13)
Provided by the user: Meshy_AI_一个独立的卡通_1012195159_texture.glb, created in their Meshy account using Meshy 6 and texturing. Generated while the account was Free with CC BY 4.0 selected; retain attribution to the creator / Meshy and the CC BY 4.0 license (https://creativecommons.org/licenses/by/4.0/). Later subscription does not establish a retroactive license change. Modifications: simplified from 627,128 to 6,000 triangles; base color resized to 512px; other PBR maps omitted; normalized dimensions; low-gloss Unity material and simple physics collider. Original source remains in the user Downloads directory, outside the game build.
