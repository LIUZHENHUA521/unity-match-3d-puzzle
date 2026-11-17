exec(open(__file__.replace('partition.py','query.py')).read().split("for d in ['2026")[0])
import re
bounds=set()
for p in Path('/Users/liuzhenhua/Desktop/code/crazyCombo/docs/analysis/2026-11-16-magic-card-plus/sql').glob('guard_part_*.sql'):
 bounds.update(re.findall("u.openid(?:>=|<)'([^']+)'",p.read_text()))
bounds=['']+sorted(bounds)+['zzzzzzzz']
summary={}
for i,(lo,hi) in enumerate(zip(bounds,bounds[1:])):
 sql=f"SELECT JSONExtractString(more_info,'mode') mode,uniqExact(tuple(openid,event_time,more_info)) starts,uniqExact(openid) users FROM events WHERE appid='wx85d24d5dbf72b423' AND event_type='level_start' AND event_time>='2026-11-16 00:00:00' AND event_time<'2026-11-16 11:00:00' AND openid>='{lo}' AND openid<'{hi}' {exclude} GROUP BY mode"
 r=q.query('yesterday-part-'+str(i),sql)
 if not r.get('ok'):raise Exception(r)
 for row in r['rows']:
  a=summary.setdefault(row['mode'],{'starts':0,'users':0});a['starts']+=int(row['starts']);a['users']+=int(row['users'])
print(json.dumps(summary));(q.OUT/'yesterday-starts.json').write_text(json.dumps(summary))
