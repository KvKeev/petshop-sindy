import { QueryClient } from '@tanstack/react-query'

export const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      // Evita refetchear automáticamente solo por volver a la pestaña del navegador
      refetchOnWindowFocus: false,
      staleTime: 30_000,
    },
  },
})