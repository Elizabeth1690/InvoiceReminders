import type { ComponentProps } from 'react'

import { cn } from '@/lib/utils'

type CardProps = ComponentProps<'div'>

const Card = ({ className, ...props }: CardProps) => (
  <div
    className={cn('rounded-xl border border-neutral-200 bg-neutral-0 shadow-sm', className)}
    {...props}
  />
)

export default Card
