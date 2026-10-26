import { LuSend } from 'react-icons/lu'

import { Button } from '@/components/ui/button'
import { useProcessRemindersButton } from '@/features/reminders/components/use-process-reminders-button'

type ProcessRemindersButtonProps = {
  clientId?: string
}

const ProcessRemindersButton = ({ clientId }: ProcessRemindersButtonProps) => {
  const { label, isDisabled, resultMessage, errorMessage, onClick } =
    useProcessRemindersButton(clientId)

  return (
    <div className="relative">
      <Button type="button" disabled={isDisabled} onClick={onClick}>
        <LuSend className="size-4" aria-hidden />
        {label}
      </Button>

      {resultMessage && (
        <span
          role="status"
          className="absolute top-full right-0 mt-2 text-sm whitespace-nowrap text-neutral-600"
        >
          {resultMessage}
        </span>
      )}
      {errorMessage && (
        <span
          role="alert"
          className="absolute top-full right-0 mt-2 text-sm whitespace-nowrap text-danger-600"
        >
          {errorMessage}
        </span>
      )}
    </div>
  )
}

export default ProcessRemindersButton
