import { useMutation, useQueryClient } from '@tanstack/react-query'

import { invoiceKeys } from '@/features/invoices'
import { processReminders } from '@/features/reminders/api/reminders.api'

const useProcessReminders = () => {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (clientId?: string) => processReminders(clientId),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: invoiceKeys.all }),
  })
}

export { useProcessReminders }
