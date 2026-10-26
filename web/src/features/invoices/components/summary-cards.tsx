import { Card } from '@/components/ui/card'
import { SUMMARY_CARD_ICONS } from '@/features/invoices/components/summary-cards-icons'
import { useSummaryCards } from '@/features/invoices/components/use-summary-cards'
import type { InvoicesSummary } from '@/features/invoices/schemas/invoice.schema'

type SummaryCardsProps = {
  summary: InvoicesSummary
}

const SummaryCards = ({ summary }: SummaryCardsProps) => {
  const { cards } = useSummaryCards(summary)

  return (
    <div className="grid grid-cols-1 gap-5 sm:grid-cols-2 lg:grid-cols-4">
      {cards.map(({ id, label, value }) => {
        const Icon = SUMMARY_CARD_ICONS[id]

        return (
          <Card key={id} className="p-6">
            <div className="flex items-start justify-between gap-3">
              <p className="text-sm text-neutral-600">{label}</p>
              <Icon className="size-5 shrink-0 text-neutral-400" aria-hidden />
            </div>
            <p className="mt-6 text-4xl font-bold text-neutral-900">{value}</p>
          </Card>
        )
      })}
    </div>
  )
}

export { SummaryCards }
