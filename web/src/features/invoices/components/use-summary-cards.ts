import { useMemo } from 'react'

import type { InvoicesSummary } from '@/features/invoices/schemas/invoice.schema'
import { buildSummaryCards } from '@/features/invoices/utils/build-summary-cards'

const useSummaryCards = (summary: InvoicesSummary) => {
  const cards = useMemo(() => buildSummaryCards(summary), [summary])

  return { cards } as const
}

export { useSummaryCards }
