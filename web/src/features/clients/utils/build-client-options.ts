import type { Client } from '@/features/clients/schemas/client.schema'
import type { ClientOption } from '@/features/clients/types/client.types'

const ALL_CLIENTS = ''

const buildClientOptions = (clients: Client[]): ClientOption[] => [
  { value: ALL_CLIENTS, label: 'Todos los clientes' },
  ...clients.map((client) => ({ value: client.id, label: client.name })),
]

export { ALL_CLIENTS, buildClientOptions }
