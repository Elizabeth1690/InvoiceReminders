import { useEffect } from 'react'

import { useProcessReminders } from '@/features/reminders/services/reminders.service'
import { buildRunMessage } from '@/features/reminders/utils/build-run-message'

const IDLE_LABEL = 'Procesar recordatorios'
const PENDING_LABEL = 'Procesando...'
const ERROR_MESSAGE = 'No se pudo procesar. Intenta de nuevo.'

const useProcessRemindersButton = (clientId?: string) => {
  const { mutate, reset, isPending, isError, data } = useProcessReminders()

  // Al cambiar el filtro de cliente se limpia el mensaje del resultado anterior.
  useEffect(() => {
    reset()
  }, [clientId, reset])

  return {
    label: isPending ? PENDING_LABEL : IDLE_LABEL,
    isDisabled: isPending,
    resultMessage: data ? buildRunMessage(data) : null,
    errorMessage: isError ? ERROR_MESSAGE : null,
    onClick: () => mutate(clientId),
  } as const
}

export { useProcessRemindersButton }
