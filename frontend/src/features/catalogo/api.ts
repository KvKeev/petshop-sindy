import { apiClient } from '../../lib/api-client'
import type { PagedResult, Producto } from './types'

export function getCatalogo(pagina = 1) {
  return apiClient.get<PagedResult<Producto>>(`/productos?pagina=${pagina}`)
}