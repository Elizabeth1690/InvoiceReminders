import {
  processAllRemindersResultSchema,
  reminderRunResultSchema,
} from '@/features/reminders/schemas/reminder.schema'
import type { ReminderRunSummary } from '@/features/reminders/types/reminder.types'
import {
  summarizeAllRuns,
  summarizeClientRun,
} from '@/features/reminders/utils/summarize-reminder-runs'
import { httpClient } from '@/lib/http-client'

const processClientReminders = async (clientId: string): Promise<ReminderRunSummary> => {
  const { data } = await httpClient.post<unknown>(
    `/clients/${encodeURIComponent(clientId)}/reminders/process`,
  )
  return summarizeClientRun(reminderRunResultSchema.parse(data))
}

const processAllReminders = async (): Promise<ReminderRunSummary> => {
  const { data } = await httpClient.post<unknown>('/reminders/process')
  return summarizeAllRuns(processAllRemindersResultSchema.parse(data))
}

/** Sin `clientId` procesa todos los clientes (la orquestación ocurre en el servidor). */
const processReminders = (clientId?: string): Promise<ReminderRunSummary> =>
  clientId === undefined ? processAllReminders() : processClientReminders(clientId)

export { processReminders }
