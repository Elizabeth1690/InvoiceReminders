import type { ReactNode } from 'react'

type PageHeaderProps = {
  icon: ReactNode
  eyebrow: string
  title: string
  description: string
  actions?: ReactNode
}

const PageHeader = ({ icon, eyebrow, title, description, actions }: PageHeaderProps) => (
  <header className="flex flex-wrap items-center justify-between gap-6 border-b border-neutral-200 pb-8">
    <div className="flex items-start gap-4">
      <span className="flex size-12 shrink-0 items-center justify-center rounded-xl bg-primary-50 text-primary-600">
        {icon}
      </span>
      <div>
        <p className="text-xs font-bold tracking-widest text-primary-600 uppercase">{eyebrow}</p>
        <h1 className="mt-1 text-3xl font-bold text-neutral-900">{title}</h1>
        <p className="mt-2 text-neutral-500">{description}</p>
      </div>
    </div>
    {actions && <div className="flex flex-wrap items-center gap-3">{actions}</div>}
  </header>
)

export { PageHeader }
