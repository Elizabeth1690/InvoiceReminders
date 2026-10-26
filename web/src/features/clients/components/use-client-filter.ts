import { type ChangeEvent, useMemo } from 'react'

import type { Client } from '@/features/clients/schemas/client.schema'
import { buildClientOptions } from '@/features/clients/utils/build-client-options'

const useClientFilter = (clients: Client[], onChange: (clientId: string) => void) => {
  const options = useMemo(() => buildClientOptions(clients), [clients])

  const handleChange = (event: ChangeEvent<HTMLSelectElement>): void => {
    onChange(event.target.value)
  }

  return { options, handleChange } as const
}

export { useClientFilter }
