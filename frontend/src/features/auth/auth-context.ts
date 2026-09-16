import { createContext } from 'react'
import type { AuthResponse } from './types'

export type Usuario = Pick<AuthResponse, 'nombre' | 'email' | 'rol'>

export interface AuthContextValue {
  usuario: Usuario | null
  estaLogueado: boolean
  guardarSesion: (respuesta: AuthResponse) => void
  cerrarSesion: () => void
}

export const AuthContext = createContext<AuthContextValue | undefined>(undefined)