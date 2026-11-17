#!/usr/bin/env python3
"""Compare completed baseline/protected stages only when their recorded inputs match."""
import argparse
import json
from pathlib import Path
parser=argparse.ArgumentParser()
parser.add_argument('baseline', type=Path)
parser.add_argument('protected', type=Path)
args=parser.parse_args()
a,b=args.baseline,args.protected
inputs=[json.loads((p/'BuildInputs/pot-input-hashes.json').read_text()) for p in (a,b)]
diff=[key for key in inputs[0].keys()|inputs[1].keys() if inputs[0].get(key)!=inputs[1].get(key)]
if diff:raise SystemExit('Build inputs differ: '+', '.join(diff))
sizes=[json.loads((p/'BuildInputs/pot-package-sizes.json').read_text()) for p in (a,b)]
if sizes[0]['obfuscated'] or not sizes[1]['obfuscated']:raise SystemExit('Expected baseline then protected')
result={'identicalInputFiles':len(inputs[0]),'metrics':{}}
for metric in ('packageBytes','wasmBytes'):
    before,after=sizes[0][metric],sizes[1][metric]
    result['metrics'][metric]={'before':before,'after':after,'delta':after-before,'percent':round((after-before)/before*100,4)}
print(json.dumps(result,indent=2))
