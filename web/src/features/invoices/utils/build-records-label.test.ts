import { describe, expect, it } from 'vitest'

import { buildRecordsLabel } from '@/features/invoices/utils/build-records-label'

describe('buildRecordsLabel', () => {
  it('usa el singular cuando hay un solo registro', () => {
    expect(buildRecordsLabel(1)).toBe('1 registro encontrado')
  })

  it('usa el plural en cualquier otro caso', () => {
    expect(buildRecordsLabel(0)).toBe('0 registros encontrados')
    expect(buildRecordsLabel(9)).toBe('9 registros encontrados')
  })
})
