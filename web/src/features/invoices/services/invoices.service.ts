import { useQuery } from '@tanstack/react-query'

import { getInvoices } from '@/features/invoices/api/invoices.api'

const invoiceKeys = {
  all: ['invoices'] as const,
  list: (clientId?: string) => [...invoiceKeys.all, clientId ?? 'all'] as const,
}

const useGetInvoices = (clientId?: string) =>
  useQuery({
    queryKey: invoiceKeys.list(clientId),
    queryFn: () => getInvoices(clientId),
  })

export { invoiceKeys, useGetInvoices }
