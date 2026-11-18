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
parser.add_argument('--obfuscate', action='store_true', default=True,
                    help='Protect selected private method names (always enabled for WeChat releases)')
parser.add_argument('--version', help='Release version stamped into the staged Unity player')
args = parser.parse_args()
if args.version and not re.fullmatch(r'\d+\.\d+\.\d+', args.version):
    raise SystemExit('Invalid release version')
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
if args.version:
    settings = stage / 'ProjectSettings/ProjectSettings.asset'
    content, count = re.subn(r'(?m)^  bundleVersion:.*$', '  bundleVersion: ' + args.version, settings.read_text())
    if count != 1:
        raise SystemExit('Unity bundleVersion not found exactly once')
    settings.write_text(content)
(stage / 'Tools/wechat').mkdir(parents=True, exist_ok=True)
shutil.copy2(ROOT / 'Tools/wechat/release.config.json', stage / 'Tools/wechat/release.config.json')
manifest = stage / 'Packages/manifest.json'
dependencies = json.loads(manifest.read_text())
dependencies['dependencies'].pop('com.unitymcp.server', None)
manifest.write_text(json.dumps(dependencies, indent=2) + '\n')

# Extracted reference artwork and retired PVP pages must never return to releases.
for retired in ('Assets/Match3D/Resources/GooseArt',
                'Assets/Match3D/Resources/Match3D/UI/PvpLobby.prefab',
                'Assets/Match3D/Resources/Match3D/UI/PvpGameHUD.prefab'):
    if (stage / retired).exists():
        raise SystemExit('Retired resource found: ' + retired)
source = '\n'.join(p.read_text() for p in (ROOT / 'Assets/Match3D/Scripts').glob('*.cs'))
names = set(re.findall(r'"([^"\n]+)"', source))
referenced_guids = set()
for asset in (stage / 'Assets').rglob('*'):
    if asset.suffix in {'.prefab', '.unity', '.asset', '.mat', '.spriteatlas'}:
        referenced_guids.update(re.findall(r'guid: ([0-9a-f]{32})', asset.read_text(errors='ignore')))
excluded = stage / 'BuildInputs/ReferenceSprites'
excluded.mkdir(parents=True, exist_ok=True)
# Keep source/reference exports out of Unity Resources. The WeChat exporter packs
# Resources wholesale even when no runtime code loads these authoring assets.
reference_dirs = [
    # Only LevelEditorTab loads these 23 authoring thumbnails. No runtime asset
    # references their GUIDs; retain originals but exclude them from release.
    stage / 'Assets/Resources/Editor',
    stage / 'Assets/Match3D/OriginalModels/Prefabs/Legacy',
    # Authoring-only source exports are kept in the project for iteration but
    # must not enter the runtime WebGL package.
    stage / 'Assets/Match3D/OriginalModels/Animations',
    stage / 'Assets/FoodPile/Editor',
]
for reference_dir in reference_dirs:
    if reference_dir.exists():
        target = excluded / reference_dir.name
        if target.exists(): shutil.rmtree(target)
        shutil.move(str(reference_dir), str(target))
for reference_file in [stage / 'Assets/Match3D/OriginalModels/models.json',
                       stage / 'Assets/Match3D/OriginalModels/animations.json']:
    if reference_file.exists():
        shutil.move(str(reference_file), str(excluded / reference_file.name))
replica_resources = stage / 'Assets/Match3D/Resources/Match3D'
# Authoring notes are not runtime assets. Keep licenses and all source originals.
for note in list(replica_resources.rglob('README.md')) + [
    replica_resources / 'GameplaySkin/layout-source.json',
    replica_resources / 'PotCertificate/pot-sources.json',
]:
    if note.exists():
        note.unlink()
        note.with_name(note.name + '.meta').unlink(missing_ok=True)

# WebGL block compression requires dimensions divisible by four. Both panels
# use explicit UI rectangles, so rounding a staging dimension preserves layout.
for name, height, width in [('pot-club', 1024, 620), ('pot-wechat', 776, 688),
                            ('pot-club-like-row', 224, 592), ('pot-club-comment-row', 224, 592),
                            ('pot-club-post-row', 224, 592)]:
    panel = replica_resources / 'PotBenefits' / (name + '.png')
    subprocess.run(['sips', '-z', str(height), str(width), str(panel)], check=True)

# Quantize only the approved UI skins in the disposable release copy.
# Keep original dimensions, alpha, sprite metadata and editable source artwork.
# pngquant rejects images below the quality floor instead of degrading them.
pngquant = shutil.which('pngquant')
if not pngquant:
    raise SystemExit('pngquant is required for release UI optimization (brew install pngquant)')
for skin in ('MailSkinPS', 'ProfileSkinPS', 'PotGlorySkin', 'SignInSkinPS', 'InviteSkinPS', 'PotBenefits'):
    for texture in sorted((replica_resources / skin).glob('*.png')):
        result = subprocess.run([pngquant, '--force', '--skip-if-larger', '--quality=90-100',
                                 '--speed=1', '--ext=.png', str(texture)], capture_output=True, text=True)
        if result.returncode not in (0, 98, 99):
            raise SystemExit('UI texture optimization failed: ' + str(texture) + '\n' + result.stderr)

# The procedural container must never regain a dependency on exported wall data.
if any((replica_resources / 'Environments').glob('*.json')):
    raise SystemExit('Legacy container JSON found in runtime Resources; archive it outside Assets')
for reference_file in replica_resources.glob('Source*'):
    # SourceLevelCatalog is runtime data, not authoring evidence. It contains
    # the first/second-stage prefab references used by LevelLayoutConfig;
    # moving it out makes the theme challenge callback throw in WeChat builds.
    if reference_file.name in {
        'SourceLevelCatalog.asset',
        'SourceColliders.json',
        'SourceSizes.json',
        # Current selection, combine and reward clips; retired material/voice/tool
        # audio and its source mapping table have been removed.
        'SourceAudio',
        'SourceFlightItems.json',
    }:
        continue
    target = excluded / reference_file.name
    if target.exists():
        if target.is_dir(): shutil.rmtree(target)
        else: target.unlink()
    shutil.move(str(reference_file), str(target))

# The current board owns its surviving dependencies; extracted scene exports
# have been intentionally removed and must not be required for publishing.
if not (replica_resources / 'Scenes/Match3DBoard.prefab').exists():
    raise SystemExit('Missing current Match3DBoard prefab')
runtime_audio = replica_resources / 'SourceAudio'
required_audio = {'DM-CGS-32.wav', 'unfa_cartoon_pop.wav', 'PotRewardReceived.mp3'}
if not required_audio.issubset({p.name for p in runtime_audio.glob('*')}):
    raise SystemExit('Missing current selection/combine/reward audio')
if any(runtime_audio.glob('S_*')) or (ROOT / 'Assets/Match3D/Resources/Match3D/SourceAudioItems.json').exists():
    raise SystemExit('Retired material/voice/tool audio or source mapping has returned')
# The source models carry authoring-quality 1K/2K textures.  The experience
# build uses the same models but imports these at 256px with Unity's
# platform compression, keeping the initial WeChat package under its limit.
texture_root = stage / 'Assets/Match3D/OriginalModels/Textures'
for meta in texture_root.glob('*.png.meta'):
    text = meta.read_text()
    text = re.sub(r'(?m)^    maxTextureSize: \d+', '    maxTextureSize: 256', text)
    text = re.sub(r'(?m)^    textureCompression: \d+', '    textureCompression: 1', text)
    meta.write_text(text)

# Photoshop UI exports use uncompressed textures for editing. Apply high-quality
# GPU compression only to the disposable release copy; preserve source artwork
# and its resolution, sprite borders and layout in the working project. Release
# textures above 1024px are scaled by Unity's importer, preserving sprite geometry.
# Preserve the loading-page JPEG for runtime decoding without shipping a large
# uncompressed GPU texture in the data package.
shutil.copy2(replica_resources / 'HomeSkinPS/img_Homepage_bg2.jpg',
             replica_resources / 'HomeBackgroundOriginal.bytes')
for skin in ('GameplaySkin', 'HomeSkinPS', 'SettingsSkinPS', 'InviteSkinPS', 'PotBenefits'):
    for meta in (replica_resources / skin).glob('*.meta'):
        text = meta.read_text()
        if 'TextureImporter:' not in text:
            continue
        text = re.sub(r'(?m)^    textureCompression: \d+', '    textureCompression: 2', text)
        text = re.sub(r'(?m)^(\s+maxTextureSize: )(\d+)',
                      lambda m: m[1] + str(min(int(m[2]), 1024)), text)
        meta.write_text(text)

# Hidden alternate tray layers from the PSD are not runtime content. Only omit
# them if no serialized asset or runtime literal references the image.
for name in ('img_Blue_box1', 'img_GreyBlue_box', 'image_add_circle'):
    image = replica_resources / 'GameplaySkin' / (name + '.png')
    meta = image.with_suffix('.png.meta')
    if not image.exists() or not meta.exists():
        continue
    guid = re.search(r'^guid: ([0-9a-f]{32})', meta.read_text(), re.M)
    if not guid or guid.group(1) in referenced_guids or any(name in literal for literal in names):
        continue
    for asset in (image, meta):
        shutil.move(str(asset), str(excluded / asset.name))
# Long music tracks dominate the remaining audio budget. Compress only the
# release copy; preserve the source tracks, duration, channels and short effects.
for relative in (
    'Assets/Match3D/Audio/Licensed/Carefree.mp3.meta',
):
    meta = stage / relative
    text = meta.read_text()
    text = re.sub(r'(?m)^(    quality: ).+$', r'\g<1>0.15', text)
    text = text.replace('forceToMono: 0', 'forceToMono: 1')
    text = text.replace('sampleRateSetting: 0', 'sampleRateSetting: 2')
    text = text.replace('sampleRateOverride: 44100', 'sampleRateOverride: 32000')
    meta.write_text(text)
from pot_obfuscation import prepare, input_hashes, verify
prepare(ROOT, stage, args.obfuscate)
(stage / 'BuildInputs/pot-input-hashes.json').write_text(json.dumps(input_hashes(stage), sort_keys=True, indent=2) + '\n')
print(f'Staged {stage}; retired reference art excluded; obfuscation={args.obfuscate}', flush=True)
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
from optimize_package import optimize_package, optimize_cover_jpegs
print('Lossless package compression: ' + json.dumps(optimize_package(output)), flush=True)
print('Lossless cover compression: ' + json.dumps(optimize_cover_jpegs(output)), flush=True)
entry = (output / 'game.js').read_text()
if 'loadDataPackageFromSubpackage: true' not in entry:
    raise SystemExit('Resources fell back to CDN; refusing to publish an incomplete package')
metrics = verify(stage, output, args.obfuscate)
print('Pot package sizes: ' + json.dumps({k: v for k, v in metrics.items() if k != 'files'}), flush=True)
destination = ROOT / 'Builds/Wechat/minigame'
if destination.exists():
    # Only generated output is replaced. Keep the previous successful package.
    backup = destination.with_name('minigame.previous')
    if backup.exists(): shutil.rmtree(backup)
    destination.rename(backup)
shutil.copytree(output, destination)
print(f'Build ready: {destination}; log: {log}')
