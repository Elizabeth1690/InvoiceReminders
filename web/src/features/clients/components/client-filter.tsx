import { Select } from '@/components/ui/select'
import { useClientFilter } from '@/features/clients/components/use-client-filter'
import type { Client } from '@/features/clients/schemas/client.schema'

type ClientFilterProps = {
  clients: Client[]
  value: string
  onChange: (clientId: string) => void
}

const ClientFilter = ({ clients, value, onChange }: ClientFilterProps) => {
  const { options, handleChange } = useClientFilter(clients, onChange)

  return (
    <Select value={value} onChange={handleChange} aria-label="Filtrar por cliente">
      {options.map((option) => (
        <option key={option.value} value={option.value}>
          {option.label}
        </option>
      ))}
    </Select>
  )
}

export { ClientFilter }
