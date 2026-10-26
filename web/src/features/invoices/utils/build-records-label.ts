const buildRecordsLabel = (count: number): string =>
  count === 1 ? '1 registro encontrado' : `${count} registros encontrados`

export { buildRecordsLabel }
