#!/usr/bin/env python3
"""Build a disposable snapshot so publishing never closes or edits the open scene."""
import argparse
import json
import re
import shutil
import subprocess
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
parser = argparse.ArgumentParser()
parser.add_argument('--stage', type=Path, default=ROOT / 'Builds/WechatProject')
parser.add_argument('--prepare-only', action='store_true')
args = parser.parse_args()
stage = args.stage.resolve()
if stage == ROOT or ROOT.is_relative_to(stage):
    raise SystemExit('Build staging directory must not contain the source project')
marker = stage / '.goose-build-stage'
if stage.exists() and any(stage.iterdir()) and not marker.exists():
    raise SystemExit('Refusing to replace an unmarked staging directory')
stage.mkdir(parents=True, exist_ok=True)
marker.touch()
for name in ['Assets', 'Packages', 'ProjectSettings']:
    if (stage / name).exists(): shutil.rmtree(stage / name)
    shutil.copytree(ROOT / name, stage / name, dirs_exist_ok=True,
                    ignore=lambda directory, names: ['com.unitymcp.server'] if Path(directory).name == 'Packages' else [])
(stage / 'Tools/wechat').mkdir(parents=True, exist_ok=True)
shutil.copy2(ROOT / 'Tools/wechat/release.config.json', stage / 'Tools/wechat/release.config.json')
manifest = stage / 'Packages/manifest.json'
dependencies = json.loads(manifest.read_text())
dependencies['dependencies'].pop('com.unitymcp.server', None)
manifest.write_text(json.dumps(dependencies, indent=2) + '\n')

# These sprites are looked up by name, including theme-specific names in YAML.
# Keep every referenced name; the full reference collection stays in the source.
source = '\n'.join(p.read_text() for p in (ROOT / 'Assets/GooseReplica/Scripts').glob('*.cs'))
names = set(re.findall(r'"([^"\n]+)"', source))
themes = (ROOT / 'Assets/GooseReplica/Resources/Replica/ThemeLibrary.asset').read_text()
names.update(re.findall(r'^\s+gooseSprite: (.+)$', themes, re.M))
# Prefab-based UI references sprites by GUID instead of Resources.Load names.
# Retain those dependencies when trimming the reference art collection.
referenced_guids = set()
for asset in (stage / 'Assets').rglob('*'):
    if asset.suffix in {'.prefab', '.unity', '.asset', '.mat', '.spriteatlas'}:
        referenced_guids.update(re.findall(r'guid: ([0-9a-f]{32})', asset.read_text(errors='ignore')))
sprites = stage / 'Assets/GooseReplica/Resources/GooseArt/Sprites'
excluded = stage / 'BuildInputs/ReferenceSprites'
excluded.mkdir(parents=True, exist_ok=True)
kept, removed = [], []
for sprite in sprites.glob('*.png'):
    meta = sprite.with_suffix('.png.meta')
    guid = re.search(r'^guid: ([0-9a-f]{32})', meta.read_text(), re.M) if meta.exists() else None
    if sprite.stem in names or (guid and guid.group(1) in referenced_guids):
        kept.append(sprite.name)
        continue
    removed.append(sprite.name)
    for p in [sprite, sprite.with_suffix('.png.meta')]:
        if p.exists(): shutil.move(str(p), str(excluded / p.name))
(stage / 'BuildInputs/sprites.json').write_text(json.dumps({'kept':sorted(kept), 'excluded':sorted(removed)}, ensure_ascii=False, indent=2))
# Keep source/reference exports out of Unity Resources. The WeChat exporter packs
# Resources wholesale even when no runtime code loads these authoring assets.
reference_dirs = [
    stage / 'Assets/GooseReplica/OriginalModels/Prefabs/Legacy',
    # Authoring-only source exports are kept in the project for iteration but
    # must not enter the runtime WebGL package.
    stage / 'Assets/GooseReplica/OriginalModels/Animations',
    stage / 'Assets/FoodPile/Editor',
]
for reference_dir in reference_dirs:
    if reference_dir.exists():
        target = excluded / reference_dir.name
        if target.exists(): shutil.rmtree(target)
        shutil.move(str(reference_dir), str(target))
for reference_file in [stage / 'Assets/GooseReplica/OriginalModels/models.json',
                       stage / 'Assets/GooseReplica/OriginalModels/animations.json']:
    if reference_file.exists():
        shutil.move(str(reference_file), str(excluded / reference_file.name))
replica_resources = stage / 'Assets/GooseReplica/Resources/Replica'
for reference_file in replica_resources.glob('Source*'):
    # SourceLevelCatalog is runtime data, not authoring evidence. It contains
    # the first/second-stage prefab references used by ReplicaSourceLevelPlan;
    # moving it out makes the theme challenge callback throw in WeChat builds.
    if reference_file.name in {
        'SourceLevelCatalog.asset',
        'SourceColliders.json',
        'SourceSizes.json',
        # Runtime lookup tables.  SourceAudio.LoadCatalog dereferences the
        # asset directly during layered item creation, so moving these makes
        # the creation coroutine abort with a NullReferenceException.
        'SourceAudioItems.json',
        # Runtime second-stage material/selection/combine audio.  ReplicaSourceAudio
        # loads these clips by Resources path (Replica/SourceAudio/<name>) while
        # creating and selecting layered items.  Treating the directory as
        # authoring evidence leaves a valid-looking build with silent stage 2.
        'SourceAudio',
        'SourceFlightItems.json',
        'SourceEnvironment.json',
    }:
        continue
    target = excluded / reference_file.name
    if target.exists():
        if target.is_dir(): shutil.rmtree(target)
        else: target.unlink()
    shutil.move(str(reference_file), str(target))

# The imported theme prefabs under Resources/Replica/Scenes reference the
# generated materials, meshes and textures in SourceScenes.  That directory
# is runtime data even though its contents originated as source-scene exports;
# removing it leaves the theme object instantiable but visually untextured.
source_scene_runtime = stage / 'Assets/GooseReplica/SourceScenes'
if not source_scene_runtime.exists():
    raise SystemExit('Missing runtime theme dependencies: Assets/GooseReplica/SourceScenes')
runtime_audio = replica_resources / 'SourceAudio'
if not runtime_audio.exists() or not any(runtime_audio.glob('*.wav')):
    raise SystemExit('Missing runtime second-stage audio: Resources/Replica/SourceAudio')
# The source models carry authoring-quality 1K/2K textures.  The experience
# build uses the same models but can safely import these at 512px with Unity's
# platform compression, keeping the initial WeChat package under its limit.
texture_root = stage / 'Assets/GooseReplica/OriginalModels/Textures'
for meta in texture_root.glob('*.png.meta'):
    text = meta.read_text()
    text = re.sub(r'(?m)^    maxTextureSize: \d+', '    maxTextureSize: 512', text)
    text = re.sub(r'(?m)^    textureCompression: \d+', '    textureCompression: 1', text)
    meta.write_text(text)

# Photoshop UI exports use uncompressed textures for editing. Apply high-quality
# GPU compression only to the disposable release copy; preserve source artwork
# and its resolution, sprite borders and layout in the working project.
for skin in ('GameplaySkin', 'HomeSkinPS', 'SettingsSkinPS'):
    for meta in (replica_resources / skin).glob('*.meta'):
        text = meta.read_text()
        if 'TextureImporter:' not in text:
            continue
        text = re.sub(r'(?m)^    textureCompression: \d+', '    textureCompression: 2', text)
        meta.write_text(text)
print(f'Staged {stage}; sprites kept={len(kept)}, excluded={len(removed)}', flush=True)
if args.prepare_only:
    raise SystemExit(0)
log = ROOT / 'Builds/wechat-build.log'
result = subprocess.run(['/Applications/Unity/Unity.app/Contents/MacOS/Unity', '-batchmode', '-nographics',
                         '-projectPath', str(stage), '-buildTarget', 'WebGL',
                         '-executeMethod', 'WechatReleaseBuild.Run', '-logFile', str(log)])
if result.returncode:
    if log.exists():
        errors = [line for line in log.read_text(errors='replace').splitlines()
                  if line.startswith('Exception:') or 'error CS' in line]
        if errors:
            print('\n'.join(errors[-5:]), flush=True)
    raise SystemExit(f'Unity build failed ({result.returncode}); see {log}')
output = stage / 'Builds/Wechat/minigame'
entry = (output / 'game.js').read_text()
if 'loadDataPackageFromSubpackage: true' not in entry:
    raise SystemExit('Resources fell back to CDN; refusing to publish an incomplete package')
destination = ROOT / 'Builds/Wechat/minigame'
if destination.exists():
    # Only generated output is replaced. Keep the previous successful package.
    backup = destination.with_name('minigame.previous')
    if backup.exists(): shutil.rmtree(backup)
    destination.rename(backup)
shutil.copytree(output, destination)
print(f'Build ready: {destination}; log: {log}')
