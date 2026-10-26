import { z } from 'zod'

const reminderRunResultSchema = z.object({
  processed: z.number(),
  sent: z.number(),
  failedInvoiceIds: z.array(z.string()),
  skippedInvoiceIds: z.array(z.string()),
})

const clientReminderRunSchema = z.object({
  clientId: z.string(),
  clientName: z.string(),
  result: reminderRunResultSchema.nullable(),
  error: z.string().nullable(),
})

const processAllRemindersResultSchema = z.object({
  clients: z.array(clientReminderRunSchema),
  processed: z.number(),
  sent: z.number(),
  failed: z.number(),
  skipped: z.number(),
  clientsWithErrors: z.number(),
})

type ReminderRunResult = z.infer<typeof reminderRunResultSchema>
type ProcessAllRemindersResult = z.infer<typeof processAllRemindersResultSchema>

export { clientReminderRunSchema, processAllRemindersResultSchema, reminderRunResultSchema }
export type { ProcessAllRemindersResult, ReminderRunResult }
