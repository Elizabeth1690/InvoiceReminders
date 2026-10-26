import { act, renderHook, waitFor } from '@testing-library/react'
import { beforeEach, describe, expect, it, vi } from 'vitest'

import { processReminders } from '@/features/reminders/api/reminders.api'
import { useProcessRemindersButton } from '@/features/reminders/components/use-process-reminders-button'
import { createWrapper } from '@/test/create-wrapper'

vi.mock('@/features/reminders/api/reminders.api', () => ({ processReminders: vi.fn() }))

const processRemindersMock = vi.mocked(processReminders)

const successSummary = (processed: number) => ({
  processed,
  sent: processed,
  failed: 0,
  skipped: 0,
  clientsWithErrors: 0,
})

describe('useProcessRemindersButton', () => {
  beforeEach(() => {
    processRemindersMock.mockReset()
  })

  it('empieza sin mensajes y con el botón habilitado', () => {
    const { result } = renderHook(() => useProcessRemindersButton(), { wrapper: createWrapper() })

    expect(result.current.label).toBe('Procesar recordatorios')
    expect(result.current.isDisabled).toBe(false)
    expect(result.current.resultMessage).toBeNull()
    expect(result.current.errorMessage).toBeNull()
  })

  it('procesa el cliente indicado y muestra el resultado', async () => {
    processRemindersMock.mockResolvedValue(successSummary(2))
    const { result } = renderHook(() => useProcessRemindersButton('acme'), {
      wrapper: createWrapper(),
    })

    act(() => result.current.onClick())

    await waitFor(() => expect(result.current.resultMessage).toBe('2 enviados de 2'))
    expect(processRemindersMock).toHaveBeenCalledWith('acme')
  })

  it('procesa todos los clientes cuando no hay filtro', async () => {
    processRemindersMock.mockResolvedValue(successSummary(5))
    const { result } = renderHook(() => useProcessRemindersButton(), { wrapper: createWrapper() })

    act(() => result.current.onClick())

    await waitFor(() => expect(result.current.resultMessage).toBe('5 enviados de 5'))
    expect(processRemindersMock).toHaveBeenCalledWith(undefined)
  })

  it('muestra el mensaje de error cuando falla', async () => {
    processRemindersMock.mockRejectedValue(new Error('boom'))
    const { result } = renderHook(() => useProcessRemindersButton(), { wrapper: createWrapper() })

    act(() => result.current.onClick())

    await waitFor(() =>
      expect(result.current.errorMessage).toBe('No se pudo procesar. Intenta de nuevo.'),
    )
    expect(result.current.resultMessage).toBeNull()
  })

  it('limpia el resultado anterior al cambiar de cliente', async () => {
    processRemindersMock.mockResolvedValue(successSummary(1))
    const { result, rerender } = renderHook(
      ({ clientId }: { clientId?: string }) => useProcessRemindersButton(clientId),
      { wrapper: createWrapper(), initialProps: { clientId: 'acme' } },
    )

    act(() => result.current.onClick())
    await waitFor(() => expect(result.current.resultMessage).not.toBeNull())

    rerender({ clientId: 'globex' })

    await waitFor(() => expect(result.current.resultMessage).toBeNull())
  })
})
