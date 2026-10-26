import { cva, type VariantProps } from 'class-variance-authority'
import type { ComponentProps } from 'react'

import { cn } from '@/lib/utils'

const badgeVariants = cva('inline-block rounded-full px-3 py-1 text-xs font-medium', {
  variants: {
    variant: {
      warning: 'bg-warning-100 text-warning-800',
      alert: 'bg-alert-100 text-alert-800',
      muted: 'bg-neutral-200 text-neutral-700',
    },
  },
  defaultVariants: { variant: 'muted' },
})

type BadgeVariant = NonNullable<VariantProps<typeof badgeVariants>['variant']>

type BadgeProps = ComponentProps<'span'> & VariantProps<typeof badgeVariants>

const Badge = ({ className, variant, ...props }: BadgeProps) => (
  <span className={cn(badgeVariants({ variant }), className)} {...props} />
)

export default Badge
export type { BadgeVariant }
