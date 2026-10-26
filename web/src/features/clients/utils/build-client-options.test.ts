import { describe, expect, it } from 'vitest'

import { ALL_CLIENTS, buildClientOptions } from '@/features/clients/utils/build-client-options'

describe('buildClientOptions', () => {
  it('antepone la opción "Todos los clientes"', () => {
    const options = buildClientOptions([{ id: 'acme', name: 'Acme', email: 'a@acme.test' }])

    expect(options).toEqual([
      { value: ALL_CLIENTS, label: 'Todos los clientes' },
      { value: 'acme', label: 'Acme' },
    ])
  })
})
