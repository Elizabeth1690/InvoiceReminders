import { AxiosHeaders, type AxiosResponse } from 'axios'

const buildAxiosResponse = <T>(data: T): AxiosResponse<T> => ({
  data,
  status: 200,
  statusText: 'OK',
  headers: {},
  config: { headers: new AxiosHeaders() },
})

export { buildAxiosResponse }
