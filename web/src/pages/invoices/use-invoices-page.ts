import { useState } from 'react'

import { ALL_CLIENTS, type Client, useGetClients } from '@/features/clients'
import { useGetInvoices } from '@/features/invoices'

const NO_CLIENTS: Client[] = []

const useInvoicesPage = () => {
  const [selectedClientId, setSelectedClientId] = useState<string>(ALL_CLIENTS)

  const clientsQuery = useGetClients()
  const clientId = selectedClientId === ALL_CLIENTS ? undefined : selectedClientId
  const invoicesQuery = useGetInvoices(clientId)

  return {
    clients: clientsQuery.data ?? NO_CLIENTS,
    selectedClientId,
    clientId,
    onClientChange: setSelectedClientId,
    summary: invoicesQuery.data,
    isLoading: invoicesQuery.isLoading,
    isError: invoicesQuery.isError,
  } as const
}

export { useInvoicesPage }
