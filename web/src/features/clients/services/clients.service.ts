import { useQuery } from '@tanstack/react-query'

import { getClients } from '@/features/clients/api/clients.api'

const clientKeys = {
  all: ['clients'] as const,
}

const useGetClients = () =>
  useQuery({
    queryKey: clientKeys.all,
    queryFn: getClients,
  })

export { clientKeys, useGetClients }
