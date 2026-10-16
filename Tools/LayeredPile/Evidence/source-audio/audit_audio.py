from pathlib import Path
import json,re
out=Path('outputs/goose-analysis/audio-evidence');out.mkdir(exist_ok=True)
m=json.load(open('outputs/goose-analysis/camera-motion-evidence/game-method-map.json'))
ids={int(k) for k,v in m.items() if any(x.startswith('SoundManager.') or x in ['ObjectUnit.PlaySound','ObjectUnit.BeSelect','ObjectUnit.BeClick','ObjectUnit.OnCombine','LevelManager.EndCombineObjAnim','LevelManager.GetTarget','LevelManager.GetUnit','LevelManager.OnSelectTarget'] for x in v)}
for side in ['main','sub']:
 s=Path('work/goose-'+side+'.wat').read_text();a=list(re.finditer(r'^  \(func \$j(\d+)\b',s,re.M))
 for i,match in enumerate(a):
  j=int(match[1])
  if j not in ids:continue
  b=s[match.start():a[i+1].start() if i+1<len(a) else s.find('\n  (elem',match.start())]
  (out/f'{side}-j{j}.wat').write_text(b)
  print(j,m[str(j)],len(b),'calls:',','.join(re.findall(r'call \$j(\d+)',b)))
