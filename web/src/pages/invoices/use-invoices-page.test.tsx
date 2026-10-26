import { act, renderHook, waitFor } from '@testing-library/react'
import { beforeEach, describe, expect, it, vi } from 'vitest'

import { httpClient } from '@/lib/http-client'
import { useInvoicesPage } from '@/pages/invoices/use-invoices-page'
import { buildAxiosResponse } from '@/test/axios-response'
import { createWrapper } from '@/test/create-wrapper'

vi.mock('@/lib/http-client', () => ({ httpClient: { get: vi.fn(), post: vi.fn() } }))

const getMock = vi.mocked(httpClient.get)

const clients = [{ id: 'acme', name: 'Acme', email: 'a@acme.test' }]
const emptySummary = {
  total: 0,
  primerRecordatorio: 0,
  segundoRecordatorio: 0,
  desactivado: 0,
  invoices: [],
}

describe('useInvoicesPage', () => {
  beforeEach(() => {
    getMock.mockReset()
    getMock.mockImplementation((url: string) =>
      Promise.resolve(buildAxiosResponse(url === '/clients' ? clients : emptySummary)),
    )
  })

  it('carga clientes y facturas sin filtro al iniciar', async () => {
    const { result } = renderHook(() => useInvoicesPage(), { wrapper: createWrapper() })

    await waitFor(() => expect(result.current.summary).toEqual(emptySummary))

    expect(result.current.clients).toEqual(clients)
    expect(result.current.clientId).toBeUndefined()
  })

  it('consulta las facturas del cliente seleccionado', async () => {
    const { result } = renderHook(() => useInvoicesPage(), { wrapper: createWrapper() })
    await waitFor(() => expect(result.current.summary).toBeDefined())

    act(() => result.current.onClientChange('acme'))

    await waitFor(() =>
      expect(getMock).toHaveBeenCalledWith('/invoices', { params: { clientId: 'acme' } }),
    )
    expect(result.current.clientId).toBe('acme')
    expect(result.current.selectedClientId).toBe('acme')
  })

  it('marca error cuando falla la carga de facturas', async () => {
    getMock.mockRejectedValue(new Error('boom'))
    const { result } = renderHook(() => useInvoicesPage(), { wrapper: createWrapper() })

    await waitFor(() => expect(result.current.isError).toBe(true))
  })
})
