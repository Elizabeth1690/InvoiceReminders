import type { InvoiceStatus } from '@/features/invoices/schemas/invoice.schema'
import type { StatusBadgeConfig } from '@/features/invoices/types/invoice.types'

const STATUS_BADGES: Record<InvoiceStatus, StatusBadgeConfig> = {
  primerrecordatorio: { label: 'Primer recordatorio', variant: 'info' },
  segundorecordatorio: { label: 'Segundo recordatorio', variant: 'warning' },
  desactivado: { label: 'Desactivado', variant: 'muted' },
}

export { STATUS_BADGES }
