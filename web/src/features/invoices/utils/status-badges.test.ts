import { describe, expect, it } from 'vitest'

import { STATUS_BADGES } from '@/features/invoices/utils/status-badges'

describe('STATUS_BADGES', () => {
  it('asigna una variante de color a cada estado', () => {
    expect(STATUS_BADGES.primerrecordatorio.variant).toBe('info')
    expect(STATUS_BADGES.segundorecordatorio.variant).toBe('warning')
    expect(STATUS_BADGES.desactivado.variant).toBe('muted')
  })

  it('usa una variante distinta para cada estado', () => {
    const variants = Object.values(STATUS_BADGES).map((badge) => badge.variant)

    expect(new Set(variants).size).toBe(variants.length)
  })
})
