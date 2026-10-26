import { Badge } from '@/components/ui/badge'
import type { InvoiceStatus } from '@/features/invoices/schemas/invoice.schema'
import { STATUS_BADGES } from '@/features/invoices/utils/status-badges'

type StatusBadgeProps = {
  status: InvoiceStatus
}

const StatusBadge = ({ status }: StatusBadgeProps) => {
  const { label, variant } = STATUS_BADGES[status]

  return <Badge variant={variant}>{label}</Badge>
}

export default StatusBadge
