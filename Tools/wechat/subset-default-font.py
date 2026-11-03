#!/usr/bin/env python3
"""Build the packaged fallback from a full Noto CJK font, retaining common Chinese and project text."""
import argparse
from pathlib import Path
from fontTools import subset
from fontTools.ttLib import TTFont

ROOT = Path(__file__).resolve().parents[2]
parser = argparse.ArgumentParser(description=__doc__)
parser.add_argument('--source', type=Path, required=True)
args = parser.parse_args()
output = ROOT / 'Assets/GooseReplica/Resources/Replica/Fonts/DefaultChinese.otf'
characters = set(chr(c) for c in range(32, 127))
for lead in range(0xA1, 0xF8):
    for trail in range(0xA1, 0xFF):
        try:
            characters.update(bytes((lead, trail)).decode('gb2312'))
        except UnicodeDecodeError:
            pass
for path in (ROOT / 'Assets/GooseReplica').rglob('*'):
    if path.suffix in {'.cs', '.json', '.asset', '.prefab', '.unity'}:
        characters.update(path.read_text(errors='ignore'))
font = TTFont(args.source)
available = set(font.getBestCmap())
required = {ord(c) for c in characters} & available
options = subset.Options()
options.layout_features = ['*']
worker = subset.Subsetter(options=options)
worker.populate(unicodes=required)
worker.subset(font)
font.save(output)
with TTFont(output) as result:
    assert required <= set(result.getBestCmap()), 'Subset lost required characters'
print(f'Fallback font: {len(required)} characters, {output.stat().st_size:,} bytes')
