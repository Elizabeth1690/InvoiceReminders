import type { IconType } from 'react-icons'
import { LuBell, LuCircleCheck, LuClock, LuFileText } from 'react-icons/lu'

import type { SummaryCardId } from '@/features/invoices/types/invoice.types'

const SUMMARY_CARD_ICONS: Record<SummaryCardId, IconType> = {
  total: LuFileText,
  first: LuBell,
  second: LuClock,
  inactive: LuCircleCheck,
}

export { SUMMARY_CARD_ICONS }
