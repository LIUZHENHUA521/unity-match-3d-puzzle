#!/usr/bin/env python3
"""Losslessly recompress generated subpackages using a standard 4 MiB Brotli window."""
from pathlib import Path
import hashlib
import json
import shutil
import subprocess
import tempfile


def digest(path):
    h = hashlib.sha256()
    with path.open('rb') as f:
        for chunk in iter(lambda: f.read(1024 * 1024), b''):
            h.update(chunk)
    return h.hexdigest()


def optimize_package(output):
    brotli = shutil.which('brotli')
    if not brotli:
        raise RuntimeError('Brotli CLI is required for release package compression')
    results = []
    for folder in ('wasmcode', 'data-package'):
        for source in sorted((Path(output) / folder).glob('*.br')):
            with tempfile.TemporaryDirectory(prefix='goose-package-') as temp:
                temp = Path(temp)
                raw, encoded, check = temp / 'raw', temp / 'encoded.br', temp / 'check'
                subprocess.run([brotli, '-d', str(source), '-o', str(raw)], check=True)
                subprocess.run([brotli, '-q', '11', '-w', '22', str(raw), '-o', str(encoded)], check=True)
                subprocess.run([brotli, '-d', str(encoded), '-o', str(check)], check=True)
                sha = digest(raw)
                if digest(check) != sha:
                    raise RuntimeError('Recompressed package changed decoded bytes: ' + source.name)
                before = source.stat().st_size
                if encoded.stat().st_size < before:
                    shutil.copyfile(encoded, source)
                results.append({'file': str(source.relative_to(output)), 'before': before,
                                'after': source.stat().st_size, 'decodedSha256': sha})
    return results


if __name__ == '__main__':
    import argparse
    parser = argparse.ArgumentParser()
    parser.add_argument('output', type=Path)
    args = parser.parse_args()
    print(json.dumps(optimize_package(args.output.resolve()), indent=2))
