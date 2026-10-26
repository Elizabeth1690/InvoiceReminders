import { LuEllipsis } from 'react-icons/lu'

import { Card } from '@/components/ui/card'
import StatusBadge from '@/features/invoices/components/status-badge'
import { useInvoiceTable } from '@/features/invoices/components/use-invoice-table'
import type { Invoice } from '@/features/invoices/schemas/invoice.schema'

const COLUMNS = ['Factura', 'Cliente', 'Valor', 'Vencimiento', 'Estado'] as const

type InvoiceTableProps = {
  invoices: Invoice[]
}

const InvoiceTable = ({ invoices }: InvoiceTableProps) => {
  const { rows, isEmpty, recordsLabel } = useInvoiceTable(invoices)

  return (
    <Card className="overflow-hidden">
      <div className="flex items-start justify-between border-b border-neutral-100 px-6 py-5">
        <div>
          <h2 className="font-semibold text-neutral-900">Facturas pendientes</h2>
          <p className="mt-1 text-sm text-neutral-500">{recordsLabel}</p>
        </div>
        <LuEllipsis className="size-5 text-neutral-400" aria-hidden />
      </div>

      <div className="overflow-x-auto">
        <table className="min-w-full text-left text-sm">
          <thead className="bg-neutral-50">
            <tr>
              {COLUMNS.map((column) => (
                <th
                  key={column}
                  className="px-6 py-4 text-xs font-semibold tracking-widest text-neutral-500 uppercase"
                >
                  {column}
                </th>
              ))}
            </tr>
          </thead>
          <tbody>
            {rows.map((row) => (
              <tr
                key={row.id}
                className="border-t border-neutral-100 transition-colors hover:bg-neutral-50"
              >
                <td className="px-6 py-5 font-semibold text-primary-600">{row.number}</td>
                <td className="px-6 py-5 text-neutral-700">{row.clientName}</td>
                <td className="px-6 py-5 text-neutral-700">{row.amountLabel}</td>
                <td className="px-6 py-5 text-neutral-700">{row.dueDate}</td>
                <td className="px-6 py-5">
                  <StatusBadge status={row.status} />
                </td>
              </tr>
            ))}
            {isEmpty && (
              <tr>
                <td colSpan={COLUMNS.length} className="px-6 py-8 text-center text-neutral-500">
                  No hay facturas para mostrar.
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>
    </Card>
  )
}

export { InvoiceTable }
