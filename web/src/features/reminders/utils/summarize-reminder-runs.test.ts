import { describe, expect, it } from 'vitest'

import {
  summarizeAllRuns,
  summarizeClientRun,
} from '@/features/reminders/utils/summarize-reminder-runs'

describe('summarize-reminder-runs', () => {
  it('summarizeClientRun cuenta las facturas fallidas y omitidas', () => {
    const summary = summarizeClientRun({
      processed: 4,
      sent: 1,
      failedInvoiceIds: ['a', 'b'],
      skippedInvoiceIds: ['c'],
    })

    expect(summary).toEqual({ processed: 4, sent: 1, failed: 2, skipped: 1, clientsWithErrors: 0 })
  })

  it('summarizeAllRuns copia los totales del servidor', () => {
    const summary = summarizeAllRuns({
      clients: [],
      processed: 5,
      sent: 4,
      failed: 1,
      skipped: 0,
      clientsWithErrors: 1,
    })

    expect(summary).toEqual({ processed: 5, sent: 4, failed: 1, skipped: 0, clientsWithErrors: 1 })
  })
})
