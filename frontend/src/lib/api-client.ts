const API_URL = import.meta.env.VITE_API_URL

// Se lanza cuando el backend responde con un código de error (4xx/5xx),
export class ApiError extends Error {
  status: number
  constructor(status: number, message: string) {
    super(message)
    this.status = status
  }
}

// Función central: arma la URL completa, agrega el JWT si hay sesión guardada,
// y devuelve el body ya parseado como JSON
async function apiFetch<T>(path: string, options: RequestInit = {}): Promise<T> {
  const token = localStorage.getItem('token')

  const headers: HeadersInit = {
    'Content-Type': 'application/json',
    ...(token ? { Authorization: `Bearer ${token}` } : {}),
    ...options.headers,
  }

  const response = await fetch(`${API_URL}${path}`, { ...options, headers })

  if (!response.ok) {
    // Intenta leer un mensaje de error del backend; si no viene, usa uno genérico.
    const body = await response.json().catch(() => null)
    throw new ApiError(response.status, body?.mensaje ?? 'Error al comunicarse con el servidor')
  }

  // Algunos endpoints (ej. el webhook, ciertos POST) responden sin body.
  if (response.status === 204) return undefined as T

  return response.json()
}

export const apiClient = {
  get: <T>(path: string) => apiFetch<T>(path),
  post: <T>(path: string, body?: unknown) =>
    apiFetch<T>(path, { method: 'POST', body: body ? JSON.stringify(body) : undefined }),
  put: <T>(path: string, body?: unknown) =>
    apiFetch<T>(path, { method: 'PUT', body: body ? JSON.stringify(body) : undefined }),
  patch: <T>(path: string, body?: unknown) =>
    apiFetch<T>(path, { method: 'PATCH', body: body ? JSON.stringify(body) : undefined }),
  delete: <T>(path: string) => apiFetch<T>(path, { method: 'DELETE' }),
}