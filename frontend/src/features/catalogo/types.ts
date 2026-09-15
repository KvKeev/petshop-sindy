export interface Producto {
  id: number
  nombre: string
  imagenUrl: string | null
  categoriaNombre: string
  precioDesde: number
}

export interface PagedResult<T> {
  items: T[]
  total: number
  pagina: number
  tamanioPagina: number
}