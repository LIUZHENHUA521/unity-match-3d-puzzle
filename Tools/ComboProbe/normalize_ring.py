#!/usr/bin/env python3
"""Reproduce the narrowly validated import copy; never change the archived export."""
import argparse
import hashlib
import json
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
SOURCE = ROOT / "Tools/ComboProbe/Original/colorfulCircles_spine.json"
DESTINATION = ROOT / "Assets/Match3D/Resources/Match3D/ComboProbe/colorfulCircles_spine.json"
SHA256 = "e11b5f82893f1b81fe4268761d50d0a15c57968f7bbdcf37498a83042a020232"

parser = argparse.ArgumentParser(description=__doc__)
parser.add_argument("--check", action="store_true", help="Verify the existing import without writing it")
args = parser.parse_args()
original = SOURCE.read_bytes()
if hashlib.sha256(original).hexdigest() != SHA256:
    raise SystemExit("Export changed: revalidate its runtime compatibility before normalizing")
data = json.loads(original)
if any(data.get(key) for key in ("ik", "transform", "path", "physics")):
    raise SystemExit("This normalization was only validated for the constraint-free ring export")
normalized = original.replace(b'"spine":"3.8.75"', b'"spine":"3.8.0"', 1)
expected = json.loads(normalized)
data["skeleton"]["spine"] = "3.8.0"
assert data == expected
if args.check:
    if DESTINATION.read_bytes() != normalized:
        raise SystemExit("Imported JSON differs from the validated header-only normalization")
    print("PASS original hash and header-only runtime import")
else:
    DESTINATION.write_bytes(normalized)
    print(f"Wrote {DESTINATION}")
