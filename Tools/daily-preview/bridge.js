// The trusted admin owns persistence. This player never receives credentials.
export function validMessage(event, source, origin, session) {
  return event.source === source && event.origin === origin && event.data?.channel === 'goose-daily-preview-v1' && event.data.session === session
}
if (typeof window !== 'undefined') {
  const session = new URLSearchParams(location.hash.slice(1)).get('session')
  const origin = location.origin
  const status = document.getElementById('status'), cover = document.getElementById('cover')
  let instance, ready = false, pending, closed = false
  const emit = data => { if (!closed && session && parent !== window) parent.postMessage({ ...data, channel: 'goose-daily-preview-v1', session }, origin) }
  const show = (message, blocked = true) => { status.textContent = message; cover.textContent = message; cover.hidden = !blocked }
  const fail = error => { show('试玩加载失败，请关闭后重新打开：' + String(error.message || error)); emit({ type: 'error', message: status.textContent }) }
  window.addEventListener('error', event => { if (event.message) fail(event.error || event.message) })
  window.addEventListener('unhandledrejection', event => fail(event.reason || '运行异常'))
  function send(command) { if (instance && ready) instance.SendMessage('DailyWebPreview', 'Receive', JSON.stringify(command)); else pending = command }
  window.goosePreviewEvent = data => {
    if (data.type === 'ready') { ready = true; show('游戏已就绪，等待候选…'); if (pending && instance) { send(pending); pending = null } }
    if (data.type === 'status') show(data.message)
    if (data.type === 'playing') show(data.message, false)
    if (data.type === 'error') show(data.message)
    emit(data)
  }
  window.addEventListener('message', event => {
    if (!validMessage(event, parent, origin, session)) return
    const data = event.data
    if (data.type === 'load') { show('正在准备候选…'); send({ action: 'load', level: data.level, hasOpening: !!data.level?.opening }) }
    if (data.type === 'play' && (data.stage === 1 || data.stage === 2)) { show('正在还原开局…'); send({ action: 'play', stage: data.stage }) }
  })
  window.addEventListener('pagehide', () => { closed = true; if (instance) instance.Quit() }, { once: true })
  if (!session || parent === window) show('请从管理后台的候选试玩入口打开。')
  else {
    const timeout = setTimeout(() => fail(new Error('下载超过 3 分钟')), 180000)
    fetch('config.json').then(r => { if (!r.ok) throw new Error('资源配置不可用'); return r.json() }).then(config => new Promise((resolve, reject) => {
      const script = document.createElement('script'); script.src = 'Build/' + config.loader
      script.onerror = () => reject(new Error('游戏加载器下载失败'))
      script.onload = () => window.createUnityInstance(document.getElementById('canvas'), { ...config, devicePixelRatio: Math.min(devicePixelRatio, 2), streamingAssetsUrl: 'StreamingAssets', showBanner: (text, type) => { if (type === 'error') fail(text) } }, p => show('正在加载 Unity 试玩… ' + Math.round(p * 100) + '%')).then(resolve, reject)
      document.body.appendChild(script)
    })).then(unity => { instance = unity; clearTimeout(timeout); if (ready && pending) { send(pending); pending = null } }).catch(e => { clearTimeout(timeout); fail(e) })
  }
}
