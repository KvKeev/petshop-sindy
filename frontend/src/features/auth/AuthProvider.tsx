import { useState, type ReactNode } from 'react'
import { AuthContext, type Usuario } from './auth-context'
import type { AuthResponse } from './types'

function leerUsuarioGuardado(): Usuario | null {
  const raw = localStorage.getItem('usuario')
  return raw ? JSON.parse(raw) : null
}

export function AuthProvider({ children }: { children: ReactNode }) {
  const [usuario, setUsuario] = useState<Usuario | null>(leerUsuarioGuardado)

  function guardarSesion(respuesta: AuthResponse) {
    localStorage.setItem('token', respuesta.token)
    const datosUsuario = { nombre: respuesta.nombre, email: respuesta.email, rol: respuesta.rol }
    localStorage.setItem('usuario', JSON.stringify(datosUsuario))
    setUsuario(datosUsuario)
  }

  function cerrarSesion() {
    localStorage.removeItem('token')
    localStorage.removeItem('usuario')
    setUsuario(null)
  }

  return (
    <AuthContext.Provider value={{ usuario, estaLogueado: !!usuario, guardarSesion, cerrarSesion }}>
      {children}
    </AuthContext.Provider>
  )
}