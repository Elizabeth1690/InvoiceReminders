import type { ReminderRunSummary } from '@/features/reminders/types/reminder.types'

const buildRunMessage = (summary: ReminderRunSummary): string => {
  const parts = [`${summary.sent} enviados de ${summary.processed}`]

  if (summary.failed > 0) {
    parts.push(`${summary.failed} fallidos`)
  }
  if (summary.skipped > 0) {
    parts.push(`${summary.skipped} omitidos`)
  }
  if (summary.clientsWithErrors > 0) {
    parts.push(`${summary.clientsWithErrors} cliente(s) con error`)
  }

  return parts.join(' · ')
}

export { buildRunMessage }
