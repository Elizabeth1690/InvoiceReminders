import type { SummaryCardItem, SummaryCounts } from '@/features/invoices/types/invoice.types'

const buildSummaryCards = (summary: SummaryCounts): SummaryCardItem[] => [
  { id: 'total', label: 'Total facturas', value: summary.total },
  { id: 'first', label: 'Primer recordatorio', value: summary.primerRecordatorio },
  { id: 'second', label: 'Segundo recordatorio', value: summary.segundoRecordatorio },
  { id: 'inactive', label: 'Desactivadas', value: summary.desactivado },
]

export { buildSummaryCards }
