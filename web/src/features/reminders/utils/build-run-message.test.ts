import { describe, expect, it } from 'vitest'

import { buildRunMessage } from '@/features/reminders/utils/build-run-message'

describe('buildRunMessage', () => {
  it('muestra solo enviados cuando no hay incidencias', () => {
    const message = buildRunMessage({
      processed: 3,
      sent: 3,
      failed: 0,
      skipped: 0,
      clientsWithErrors: 0,
    })

    expect(message).toBe('3 enviados de 3')
  })

  it('agrega fallidos, omitidos y clientes con error cuando existen', () => {
    const message = buildRunMessage({
      processed: 10,
      sent: 6,
      failed: 2,
      skipped: 1,
      clientsWithErrors: 1,
    })

    expect(message).toBe('6 enviados de 10 · 2 fallidos · 1 omitidos · 1 cliente(s) con error')
  })
})
