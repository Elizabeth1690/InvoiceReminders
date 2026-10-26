import type { BadgeVariant } from '@/components/ui/badge'
import type {
  Invoice,
  InvoicesSummary,
  InvoiceStatus,
} from '@/features/invoices/schemas/invoice.schema'

type SummaryCounts = Omit<InvoicesSummary, 'invoices'>

type SummaryCardId = 'total' | 'first' | 'second' | 'inactive'

type SummaryCardItem = {
  id: SummaryCardId
  label: string
  value: number
}

type InvoiceRow = {
  id: string
  number: string
  clientName: string
  amountLabel: string
  dueDate: string
  status: InvoiceStatus
}

type StatusBadgeConfig = {
  label: string
  variant: BadgeVariant
}

export type {
  Invoice,
  InvoiceRow,
  InvoiceStatus,
  StatusBadgeConfig,
  SummaryCardId,
  SummaryCardItem,
  SummaryCounts,
}
