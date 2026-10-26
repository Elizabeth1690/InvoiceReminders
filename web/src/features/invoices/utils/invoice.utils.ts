import type { Invoice, InvoiceStatus } from '@/features/invoices/types/invoice.types'

const filterInvoices = (invoices: Invoice[], search: string, status: InvoiceStatus | 'all') => {
  const normalizedSearch = search.trim().toLocaleLowerCase()
  return invoices.filter((invoice) => {
    const matchesSearch =
      !normalizedSearch ||
      invoice.number.toLocaleLowerCase().includes(normalizedSearch) ||
      invoice.clientName.toLocaleLowerCase().includes(normalizedSearch)
    return matchesSearch && (status === 'all' || invoice.status === status)
  })
}

export { filterInvoices }
