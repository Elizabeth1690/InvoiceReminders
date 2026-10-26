import { describe, expect, it } from 'vitest'

import { toInvoiceRow } from '@/features/invoices/utils/to-invoice-row'

describe('toInvoiceRow', () => {
  it('formatea el valor como moneda y conserva el resto de los campos', () => {
    const row = toInvoiceRow({
      id: 'inv-1',
      clientId: 'acme',
      clientName: 'Acme',
      number: 'F-1',
      amount: 1500000,
      dueDate: '2026-10-01',
      status: 'primerrecordatorio',
    })

    expect(row.amountLabel).toContain('1.500.000')
    expect(row).toMatchObject({ id: 'inv-1', number: 'F-1', status: 'primerrecordatorio' })
  })
})
