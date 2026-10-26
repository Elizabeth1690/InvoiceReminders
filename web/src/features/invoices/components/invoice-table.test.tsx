import { render, screen } from '@testing-library/react'
import { describe, expect, it } from 'vitest'

import { InvoiceTable } from '@/features/invoices/components/invoice-table'

const invoice = {
  id: 'inv-1',
  clientId: 'acme',
  clientName: 'Acme',
  number: 'F-1001',
  amount: 1500000,
  dueDate: '2026-10-01',
  status: 'segundorecordatorio' as const,
}

describe('InvoiceTable', () => {
  it('muestra una fila por factura con su estado', () => {
    render(<InvoiceTable invoices={[invoice]} />)

    expect(screen.getByText('F-1001')).toBeDefined()
    expect(screen.getByText('Acme')).toBeDefined()
    expect(screen.getByText('Segundo recordatorio')).toBeDefined()
  })

  it('muestra cuántos registros se encontraron', () => {
    render(<InvoiceTable invoices={[invoice]} />)

    expect(screen.getByText('1 registro encontrado')).toBeDefined()
  })

  it('muestra el mensaje de vacío cuando no hay facturas', () => {
    render(<InvoiceTable invoices={[]} />)

    expect(screen.getByText('No hay facturas para mostrar.')).toBeDefined()
  })
})
