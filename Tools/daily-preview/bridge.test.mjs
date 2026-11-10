import assert from 'node:assert/strict'
import test from 'node:test'
import { readFile } from 'node:fs/promises'
const { validMessage } = await import('data:text/javascript;base64,' + Buffer.from(await readFile(new URL('./bridge.js', import.meta.url))).toString('base64'))
test('child only accepts the parent, exact origin, protocol and session', () => {
  const source = {}, origin = 'https://admin.example', session = 'unique'
  const e = { source, origin, data: { channel: 'goose-daily-preview-v1', session } }
  assert.equal(validMessage(e, source, origin, session), true)
  for (const bad of [{ ...e, source: {} }, { ...e, origin: 'https://evil.example' }, { ...e, data: { ...e.data, session: 'old' } }, { ...e, data: null }]) assert.equal(validMessage(bad, source, origin, session), false)
})
