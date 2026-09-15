import { useQuery } from '@tanstack/react-query'
import { getCatalogo } from './api'

export function CatalogoPage() {
  const { data, isLoading, isError, error } = useQuery({
    queryKey: ['catalogo'],
    queryFn: () => getCatalogo(),
  })

  if (isLoading) return <p className="p-8 text-gray-500">Cargando productos...</p>
  if (isError) return <p className="p-8 text-red-600">Error: {(error as Error).message}</p>

  return (
    <div className="p-8">
      <h1 className="text-2xl font-bold text-gray-800 mb-4">Catálogo</h1>
      <ul className="space-y-2">
        {data?.items.map((producto) => (
          <li key={producto.id} className="border-b pb-2">
            <span className="font-medium">{producto.nombre}</span>
            {' — '}
            <span className="text-gray-500">{producto.categoriaNombre}</span>
            {' — '}
            <span>desde ${producto.precioDesde}</span>
          </li>
        ))}
      </ul>
      {data?.items.length === 0 && (
        <p className="text-gray-500">No hay productos cargados todavía.</p>
      )}
    </div>
  )
}