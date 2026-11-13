export function validPotReplayMessage(event, source, origin, session) {
  return !!session && event.source === source && event.origin === origin && event.data?.channel === 'pot-replay-v1' && event.data.session === session
}
if (typeof window !== 'undefined') {
  const session = new URLSearchParams(location.hash.slice(1)).get('session'), origin = location.origin
  const cover = document.getElementById('cover'), status = document.getElementById('status')
  let instance, ready = false, pending, closed = false, loading = false
  const emit = body => { if (!closed) parent.postMessage({ ...body, channel: 'pot-replay-v1', session }, origin) }
  const show = (text, blocked = true) => { status.textContent = text; cover.textContent = text; cover.hidden = !blocked }
  const fail = e => { show(String(e.message || e)); emit({ type: 'error', message: status.textContent }) }
  const send = () => {
    if (!instance || !ready || !pending || loading) return
    if (!((pending.version === 'pot-replay-v1' && pending.resourceVersion === 'pot-replay-resources-1') || (pending.version === 'pot-replay-v2' && pending.resourceVersion === 'pot-replay-resources-2'))) { pending = null; fail('该录像需要对应版本的播放器'); return }
    loading = true; instance.SendMessage('DailyWebPreview', 'Receive', JSON.stringify({ action: 'potReplay', replay: pending })); pending = null
  }
  window.goosePreviewEvent = data => { if (data.type === 'ready') { ready = true; emit({ type: 'ready' }); send() }; if (data.type === 'potReplayPlaying') { show('每日挑战回放', false); emit({ type: 'playing' }) }; if (data.type === 'potReplayClosed') emit({ type: 'closed' }); if (data.type === 'error') fail(data.message) }
  window.addEventListener('message', e => { if (!validPotReplayMessage(e, parent, origin, session)) return; if (e.data.type === 'seek' && instance && loading && Number.isFinite(e.data.time)) { instance.SendMessage('DailyWebPreview', 'Receive', JSON.stringify({ action: 'potReplaySeek', time: e.data.time })); return }; if (e.data.type !== 'load' || loading) return; pending = e.data.detail; send() })
  window.addEventListener('pagehide', () => { closed = true; if (instance) instance.Quit() }, { once: true })
  if (!session || parent === window) show('请从管理后台打开每日回放。')
  else fetch('config.json').then(r => { if (!r.ok) throw Error('播放器配置读取失败'); return r.json() }).then(config => new Promise((resolve, reject) => {
    const script = document.createElement('script'); script.src = 'Build/' + config.loader; script.onerror = () => reject(Error('播放器下载失败'))
    script.onload = () => window.createUnityInstance(document.getElementById('canvas'), { ...config, devicePixelRatio: Math.min(devicePixelRatio, 2), streamingAssetsUrl: 'StreamingAssets', showBanner: (text, type) => { if (type === 'error') fail(text) } }, p => show(`正在加载回放 ${Math.round(p * 100)}%`)).then(resolve, reject)
    document.body.appendChild(script)
  })).then(value => { instance = value; send() }).catch(fail)
}
