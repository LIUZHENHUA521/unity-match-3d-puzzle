"""Lift baked shadows in the purchased food atlases during source import."""
from io import BytesIO

import numpy as np
from PIL import Image, ImageDraw, ImageFilter


def model_mask(model, size):
    width, height = size
    mask = Image.new('L', size)
    draw = ImageDraw.Draw(mask)
    uv = [(point['x'] * width, (1 - point['y']) * height) for point in model['uv']]
    triangles = model['triangles']
    for i in range(0, len(triangles), 3):
        draw.polygon([uv[j] for j in triangles[i:i + 3]], fill=255)
    # Include the UV gutter for bilinear filtering and mip generation.
    return np.asarray(mask.filter(ImageFilter.MaxFilter(5))) != 0


def brighten_atlas(source, models, pack):
    image = Image.open(BytesIO(source)).convert('RGBA')
    pixels = np.array(image)
    rgb = pixels[:, :, :3].astype(np.float32)
    value = rgb.max(axis=2)
    strength = np.zeros(value.shape, dtype=np.float32)
    paper = np.zeros_like(strength)
    for model in models:
        mask = model_mask(model, image.size)
        # A dark ingredient should still look darker than a white package.
        reference = max(1, np.percentile(value[mask], 85))
        shadow = np.clip(1 - (value[mask] / reference) ** 2, 0, 1)
        if pack == 'grocery' and model['name'] == 'RiceBag':
            # The back panel wraps around the UV seam in this 1024px atlas.
            y, x = np.mgrid[:image.height, :image.width]
            luma = rgb @ np.array([.2126, .7152, .0722], dtype=np.float32)
            paper = (mask * np.clip((abs(x - 690) - 87) / 25, 0, 1)
                     * np.clip((y - 435) / 20, 0, 1) * np.clip((629 - y) / 20, 0, 1)
                     * np.clip((luma - 28) / 49, 0, 1)
                     * np.clip((rgb[:, :, 0] - rgb[:, :, 1] + 1) / 5, 0, 1))
        else:
            strength[mask] = np.maximum(strength[mask], shadow)
    # Keep hue in saturated shadows. Fade the adjustment near black to retain ink.
    fill = .32 if pack == 'grocery' else .24
    lift = fill * strength * np.clip(value / 55, 0, 1) * (255 - value)
    tint = (rgb + 18) / (value[:, :, None] + 18)
    corrected = rgb + lift[:, :, None] * tint
    # Neutral fill for the rice label, slightly brighter than the first .36 proof.
    corrected = np.maximum(corrected, rgb + (.48 * paper[:, :, None]) * (255 - rgb))
    pixels[:, :, :3] = np.rint(np.clip(corrected, 0, 255)).astype(np.uint8)
    output = BytesIO()
    Image.fromarray(pixels).save(output, format='PNG')
    return output.getvalue()
