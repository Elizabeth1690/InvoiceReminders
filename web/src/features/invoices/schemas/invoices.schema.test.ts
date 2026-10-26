import { describe, expect, it } from 'vitest'

import { invoicesSummarySchema } from '@/features/invoices/schemas/invoice.schema'

const validInvoice = {
  id: 'inv-1',
  clientId: 'acme',
  clientName: 'Acme',
  number: 'F-1',
  amount: 1500000,
  dueDate: '2026-10-01',
  status: 'primerrecordatorio',
}

const buildSummary = (invoice: Record<string, unknown>) => ({
  total: 1,
  primerRecordatorio: 1,
  segundoRecordatorio: 0,
  desactivado: 0,
  invoices: [invoice],
})

describe('invoicesSummarySchema', () => {
  it('acepta una respuesta válida de la API', () => {
    const result = invoicesSummarySchema.safeParse(buildSummary(validInvoice))

    expect(result.success).toBe(true)
  })

  it('rechaza un estado que el frontend no conoce', () => {
    const result = invoicesSummarySchema.safeParse(
      buildSummary({ ...validInvoice, status: 'otro' }),
    )

    expect(result.success).toBe(false)
  })

  it('rechaza una respuesta con un campo de tipo incorrecto', () => {
    const result = invoicesSummarySchema.safeParse(
      buildSummary({ ...validInvoice, amount: '1500000' }),
    )

    expect(result.success).toBe(false)
  })
})
