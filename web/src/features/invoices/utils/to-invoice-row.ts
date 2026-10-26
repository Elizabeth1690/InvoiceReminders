import type { Invoice } from '@/features/invoices/schemas/invoice.schema'
import type { InvoiceRow } from '@/features/invoices/types/invoice.types'
import { formatCurrency } from '@/utils/format'

const toInvoiceRow = (invoice: Invoice): InvoiceRow => ({
  id: invoice.id,
  number: invoice.number,
  clientName: invoice.clientName,
  amountLabel: formatCurrency(invoice.amount),
  dueDate: invoice.dueDate,
  status: invoice.status,
})

export { toInvoiceRow }
