import { type Client, clientsSchema } from '@/features/clients/schemas/client.schema'
import { httpClient } from '@/lib/http-client'

const getClients = async (): Promise<Client[]> => {
  const { data } = await httpClient.get<unknown>('/clients')
  return clientsSchema.parse(data)
}

export { getClients }
