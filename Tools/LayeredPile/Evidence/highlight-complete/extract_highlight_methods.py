from pathlib import Path
import re,json
mp=json.load(open('outputs/goose-analysis/camera-motion-evidence/game-method-map.json'));out=Path('outputs/goose-analysis/highlight-complete-evidence')
for side in ['main','sub']:
 s=Path('work/goose-'+side+'.wat').read_text();matches=list(re.finditer(r'^  \(func \$j(\d+)\b',s,re.M))
 for ix,m in enumerate(matches):
  names=mp.get(m[1],[])
  if not any(n.startswith('HighlightEffect.') for n in names):continue
  b=s[m.start():matches[ix+1].start() if ix+1<len(matches) else s.find('\n  (elem',m.start())]
  b=re.sub(r'call \$j(\d+)',lambda z:z[0]+' ;; '+','.join(mp.get(z[1],[])),b)
  (out/(side+'-j'+m[1]+'.wat')).write_text(b)
