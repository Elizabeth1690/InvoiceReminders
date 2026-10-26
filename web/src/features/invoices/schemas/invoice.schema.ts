import { z } from 'zod'

const invoiceStatusSchema = z.enum(['primerrecordatorio', 'segundorecordatorio', 'desactivado'])

const invoiceSchema = z.object({
  id: z.string(),
  clientId: z.string(),
  clientName: z.string(),
  number: z.string(),
  amount: z.number(),
  dueDate: z.string(),
  status: invoiceStatusSchema,
})

const invoicesSummarySchema = z.object({
  total: z.number(),
  primerRecordatorio: z.number(),
  segundoRecordatorio: z.number(),
  desactivado: z.number(),
  invoices: z.array(invoiceSchema),
})

type InvoiceStatus = z.infer<typeof invoiceStatusSchema>
type Invoice = z.infer<typeof invoiceSchema>
type InvoicesSummary = z.infer<typeof invoicesSummarySchema>

export { invoiceSchema, invoicesSummarySchema, invoiceStatusSchema }
export type { Invoice, InvoicesSummary, InvoiceStatus }
