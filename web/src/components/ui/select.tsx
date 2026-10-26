import type { ComponentProps } from 'react'
import { LuChevronDown } from 'react-icons/lu'

import { cn } from '@/lib/utils'

type SelectProps = ComponentProps<'select'>

const Select = ({ className, children, ...props }: SelectProps) => (
  <div className="relative">
    <select
      className={cn(
        'h-12 w-full min-w-60 appearance-none rounded-xl border border-neutral-200 bg-neutral-0 pr-10 pl-4 text-sm font-medium text-neutral-900 shadow-sm focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-primary-600',
        className,
      )}
      {...props}
    >
      {children}
    </select>
    <LuChevronDown
      className="pointer-events-none absolute top-1/2 right-4 size-4 -translate-y-1/2 text-neutral-500"
      aria-hidden
    />
  </div>
)

export { Select }
