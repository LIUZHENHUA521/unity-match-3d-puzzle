"""Opt-in build-time name protection. Never writes into the source Assets folder."""
import hashlib
import json
import re
import shutil
import subprocess
from pathlib import Path

UNITY = Path('/Applications/Unity/Unity.app/Contents')

def prepare(root, stage, enabled):
    source = root / 'Tools/wechat/obfuscation'
    target = stage / 'Tools/wechat/obfuscation'
    report = stage / 'BuildInputs/PotObfuscation'
    if target.exists():
        shutil.rmtree(target)
    if report.exists():
        shutil.rmtree(report)
    if not enabled:
        return
    # Serialized UnityEvents and other literal method references must retain names.
    methods = [s.strip().split('::')[1] for s in (source / 'methods.txt').read_text().splitlines()
               if s.strip() and not s.lstrip().startswith('#')]
    for asset in (stage / 'Assets').rglob('*'):
        if asset.suffix in {'.prefab', '.unity', '.asset', '.jslib', '.js'}:
            text = asset.read_text(errors='replace')
            for method in methods:
                if f'm_MethodName: {method}\n' in text or f'"{method}"' in text or f"'{method}'" in text:
                    raise RuntimeError(f'String/serialized reference to protected method {method}: {asset}')
    shutil.copytree(source, target, ignore=shutil.ignore_patterns('__pycache__'))
    subprocess.run([str(UNITY / 'MonoBleedingEdge/bin/mcs'),
                    '-r:' + str(UNITY / 'Managed/Unity.Cecil.dll'),
                    '-out:' + str(target / 'PotAssemblyNames.exe'),
                    '-main:PotAssemblyNames', str(target / 'PotAssemblyNames.cs'),
                    str(target / 'PotAssemblyVerify.cs')], check=True)
    shutil.copy2(UNITY / 'Managed/Unity.Cecil.dll', target)
    shutil.copy2(source / 'PotReleaseObfuscation.cs', stage / 'Assets/Match3D/Editor/PotReleaseObfuscation.cs')

def input_hashes(stage):
    result = {}
    for folder in ('Assets', 'Packages', 'ProjectSettings'):
        for path in sorted((stage / folder).rglob('*')):
            if not path.is_file() or path.name.startswith('PotReleaseObfuscation.'):
                continue
            result[str(path.relative_to(stage))] = hashlib.sha256(path.read_bytes()).hexdigest()
    return result

def verify(stage, output, enabled):
    report = stage / 'BuildInputs/PotObfuscation'
    if enabled:
        receipt = report / 'receipt.txt'
        expected = sum(bool(s.strip()) and not s.lstrip().startswith('#') for s in (stage / 'Tools/wechat/obfuscation/methods.txt').read_text().splitlines())
        if not receipt.exists() or f'methods={expected}\n' not in receipt.read_text():
            raise RuntimeError('Requested obfuscation did not produce the expected receipt')
        cpp_root = stage / 'Library/Bee/artifacts/WebGL/il2cppOutput/cpp'
        cpp = '\n'.join(p.read_text(errors='replace') for p in cpp_root.glob('Assembly-CSharp*.cpp'))
        evidence = []
        for index, key in enumerate(s.strip() for s in (stage / 'Tools/wechat/obfuscation/methods.txt').read_text().splitlines()
                                    if s.strip() and not s.lstrip().startswith('#')):
            type_name, old_name = key.split('::')
            type_name = type_name.rsplit('/', 1)[-1]
            new_name = 'p' + format(index, 'x')
            if not re.search(r'\b' + re.escape(type_name + '_' + new_name) + r'_m[0-9A-Fa-f]+\s*\(', cpp):
                raise RuntimeError(f'Protected method missing from IL2CPP output: {key}')
            if re.search(r'\b' + re.escape(type_name + '_' + old_name) + r'_m[0-9A-Fa-f]+\s*\(', cpp):
                raise RuntimeError(f'Original method still present in IL2CPP output: {key}')
            evidence.append(key + ' -> ' + new_name)
        (report / 'il2cpp-verification.txt').write_text('\n'.join(evidence) + '\n')
        # SDK produces symbol maps for diagnostics/incremental conversion. They are
        # excluded by its upload config; also keep them physically outside this trial package.
        for symbol in output.glob('*.symbols.unityweb*'):
            shutil.move(str(symbol), str(report / symbol.name))
        for file in output.rglob('*'):
            if file.is_file() and (file.suffix in {'.pdb', '.mdb', '.map', '.cs', '.dll', '.exe'}
                                   or file.name in {'mapping.tsv', 'receipt.txt', 'methods.txt'}):
                raise RuntimeError(f'Private build artifact in release: {file}')
    sizes = {str(p.relative_to(output)): p.stat().st_size for p in sorted(output.rglob('*')) if p.is_file()}
    ignored = json.loads((output / 'project.config.json').read_text()).get('packOptions', {}).get('ignore', [])
    def excluded(name):
        return any((rule['type'] == 'suffix' and name.endswith(rule['value'])) or
                   (rule['type'] == 'folder' and (name == rule['value'] or name.startswith(rule['value'].rstrip('/') + '/'))) or
                   (rule['type'] == 'file' and name == rule['value']) for rule in ignored)
    result = {'obfuscated': enabled, 'directoryBytes': sum(sizes.values()),
              'packageBytes': sum(size for name, size in sizes.items() if not excluded(name)),
              'wasmBytes': sum(size for name, size in sizes.items() if name.startswith('wasmcode/') and '.wasm' in name), 'files': sizes}
    (stage / 'BuildInputs/pot-package-sizes.json').write_text(json.dumps(result, indent=2) + '\n')
    return result
