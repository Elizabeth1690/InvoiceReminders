import type { ComponentProps } from 'react'

import { cn } from '@/lib/utils'

type SelectProps = ComponentProps<'select'>

const Select = ({ className, ...props }: SelectProps) => (
  <select
    className={cn(
      'rounded-lg border border-neutral-300 bg-neutral-0 px-3 py-2 text-sm text-neutral-900',
      className,
    )}
    {...props}
  />
)

export default Select
