import importlib.util,json
from pathlib import Path
s=importlib.util.spec_from_file_location('q','/Users/liuzhenhua/Desktop/code/crazyCombo/docs/analysis/2026-11-16-magic-card-plus/query.py');q=importlib.util.module_from_spec(s);s.loader.exec_module(q)
q.OUT=Path(__file__).resolve().parent
exclude=" AND hex(SHA256(openid)) NOT IN ('2C111186BF7A563FD38435C153E609C64BDA13230050212ED9E8BCCF2EC96602')"
for d in ['2026-11-16','2026-11-17']:
 for kind,sql in [('starts',f"SELECT JSONExtractString(more_info,'mode') mode, count() raw, uniqExact(tuple(openid,event_time,more_info)) starts,uniqExact(openid) users FROM events WHERE appid='wx85d24d5dbf72b423' AND event_type='level_start' AND event_time>='{d} 00:00:00' AND event_time<'{d} 11:00:00' {exclude} GROUP BY mode"),('ads',f"SELECT placement,ad_group,uniqExact(tuple(openid,impression_id)) ads,uniqExact(openid) users FROM ad_events_v2 FINAL WHERE appid='wx85d24d5dbf72b423' AND occurred_at>='{d} 00:00:00' AND occurred_at<'{d} 11:00:00' AND event_type='completed' AND ingest_verified=1 AND impression_id!='' {exclude} GROUP BY placement,ad_group")]:
  print(d,kind,json.dumps(q.query(d+'-'+kind,sql),ensure_ascii=False),flush=True)
