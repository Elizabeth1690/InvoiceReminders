import { describe, expect, it } from 'vitest'

import { buildSummaryCards } from '@/features/invoices/utils/build-summary-cards'

describe('buildSummaryCards', () => {
  it('devuelve las cuatro tarjetas en orden', () => {
    const cards = buildSummaryCards({
      total: 9,
      primerRecordatorio: 4,
      segundoRecordatorio: 3,
      desactivado: 2,
    })

    expect(cards).toEqual([
      { id: 'total', label: 'Total facturas', value: 9 },
      { id: 'first', label: 'Primer recordatorio', value: 4 },
      { id: 'second', label: 'Segundo recordatorio', value: 3 },
      { id: 'inactive', label: 'Desactivadas', value: 2 },
    ])
  })
})
