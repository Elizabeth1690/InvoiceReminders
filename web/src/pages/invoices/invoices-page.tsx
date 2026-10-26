import { LuBell } from 'react-icons/lu'

import { PageHeader } from '@/components/ui/page-header'
import { ClientFilter } from '@/features/clients'
import { InvoiceTable, SummaryCards } from '@/features/invoices'
import { ProcessRemindersButton } from '@/features/reminders'
import { useInvoicesPage } from '@/pages/invoices/use-invoices-page'

const InvoicesPage = () => {
  const { clients, selectedClientId, clientId, onClientChange, summary, isLoading, isError } =
    useInvoicesPage()

  return (
    <main className="mx-auto max-w-7xl space-y-8 px-6 py-10">
      <PageHeader
        icon={<LuBell className="size-6" aria-hidden />}
        eyebrow="Cuentas por cobrar"
        title="Recordatorios de facturas"
        description="Supervisa y gestiona tus comunicaciones de pago."
        actions={
          <>
            <ClientFilter clients={clients} value={selectedClientId} onChange={onClientChange} />
            <ProcessRemindersButton clientId={clientId} />
          </>
        }
      />

      {isLoading && <p className="text-neutral-500">Cargando...</p>}
      {isError && <p className="text-danger-600">No se pudo cargar la información.</p>}

      {summary && (
        <>
          <SummaryCards summary={summary} />
          <InvoiceTable invoices={summary.invoices} />
        </>
      )}
    </main>
  )
}

export default InvoicesPage
