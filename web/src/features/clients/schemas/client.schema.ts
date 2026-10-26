import { z } from 'zod'

const clientSchema = z.object({
  id: z.string(),
  name: z.string(),
  email: z.string(),
})

const clientsSchema = z.array(clientSchema)

type Client = z.infer<typeof clientSchema>

export { clientSchema, clientsSchema }
export type { Client }
