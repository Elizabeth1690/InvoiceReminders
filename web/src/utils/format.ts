const currencyFormatter = new Intl.NumberFormat('es-CO', {
  style: 'currency',
  currency: 'COP',
  maximumFractionDigits: 0,
})

const formatCurrency = (value: number): string => currencyFormatter.format(value)

export { formatCurrency }
