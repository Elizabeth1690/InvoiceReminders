import { cva, type VariantProps } from 'class-variance-authority'
import type { ComponentProps } from 'react'

import { cn } from '@/lib/utils'

const badgeVariants = cva('inline-block rounded-full px-3 py-1 text-xs font-semibold', {
  variants: {
    variant: {
      info: 'bg-info-100 text-info-800',
      warning: 'bg-warning-100 text-warning-800',
      muted: 'bg-neutral-100 text-neutral-600',
    },
  },
  defaultVariants: { variant: 'muted' },
})

type BadgeVariant = NonNullable<VariantProps<typeof badgeVariants>['variant']>

type BadgeProps = ComponentProps<'span'> & VariantProps<typeof badgeVariants>

const Badge = ({ className, variant, ...props }: BadgeProps) => (
  <span className={cn(badgeVariants({ variant }), className)} {...props} />
)

export { Badge }
export type { BadgeVariant }
