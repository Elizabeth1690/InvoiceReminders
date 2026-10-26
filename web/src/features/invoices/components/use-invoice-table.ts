import { useMemo } from 'react'

import type { Invoice } from '@/features/invoices/schemas/invoice.schema'
import { buildRecordsLabel } from '@/features/invoices/utils/build-records-label'
import { toInvoiceRow } from '@/features/invoices/utils/to-invoice-row'

const useInvoiceTable = (invoices: Invoice[]) => {
  const rows = useMemo(() => invoices.map(toInvoiceRow), [invoices])
  const isEmpty = rows.length === 0
  const recordsLabel = buildRecordsLabel(rows.length)

  return { rows, isEmpty, recordsLabel } as const
}

export { useInvoiceTable }
