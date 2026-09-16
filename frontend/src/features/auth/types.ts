export interface AuthResponse {
  token: string
  nombre: string
  email: string
  rol: string
}

export interface LoginPayload {
  email: string
  password: string
}