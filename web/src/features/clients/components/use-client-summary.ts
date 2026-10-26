import type { ClientOption } from '@/features/clients/types/client.types'

const useClientSummary = (clients: ClientOption[]) => {
  return { total: clients.length }
}

export { useClientSummary }
