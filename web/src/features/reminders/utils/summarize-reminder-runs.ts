import type {
  ProcessAllRemindersResult,
  ReminderRunResult,
} from '@/features/reminders/schemas/reminder.schema'
import type { ReminderRunSummary } from '@/features/reminders/types/reminder.types'

const summarizeClientRun = (result: ReminderRunResult): ReminderRunSummary => ({
  processed: result.processed,
  sent: result.sent,
  failed: result.failedInvoiceIds.length,
  skipped: result.skippedInvoiceIds.length,
  clientsWithErrors: 0,
})

const summarizeAllRuns = (result: ProcessAllRemindersResult): ReminderRunSummary => ({
  processed: result.processed,
  sent: result.sent,
  failed: result.failed,
  skipped: result.skipped,
  clientsWithErrors: result.clientsWithErrors,
})

export { summarizeAllRuns, summarizeClientRun }
