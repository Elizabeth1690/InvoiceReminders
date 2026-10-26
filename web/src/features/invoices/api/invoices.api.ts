import {
  type InvoicesSummary,
  invoicesSummarySchema,
} from '@/features/invoices/schemas/invoice.schema'
import { httpClient } from '@/lib/http-client'

const getInvoices = async (clientId?: string): Promise<InvoicesSummary> => {
  const { data } = await httpClient.get<unknown>('/invoices', { params: { clientId } })
  return invoicesSummarySchema.parse(data)
}

export { getInvoices }
