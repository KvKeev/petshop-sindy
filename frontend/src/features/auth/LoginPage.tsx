import { useState, type FormEvent } from 'react'
import { useNavigate } from 'react-router-dom'
import { useMutation } from '@tanstack/react-query'
import { login } from './api'
import { useAuth } from './useAuth'
import { ApiError } from '../../lib/api-client'

export function LoginPage() {
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const { guardarSesion } = useAuth()
  const navigate = useNavigate()

  const mutation = useMutation({
    mutationFn: login,
    onSuccess: (respuesta) => {
      guardarSesion(respuesta)
      navigate('/')
    },
  })

  function handleSubmit(e: FormEvent) {
    e.preventDefault()
    mutation.mutate({ email, password })
  }

  return (
    <div className="p-8 max-w-sm">
      <h1 className="text-2xl font-bold text-gray-800 mb-4">Iniciar sesión</h1>
      <form onSubmit={handleSubmit} className="space-y-3">
        <input
          type="email"
          placeholder="Email"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          className="border rounded px-3 py-2 w-full"
          required
        />
        <input
          type="password"
          placeholder="Contraseña"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          className="border rounded px-3 py-2 w-full"
          required
        />
        <button
          type="submit"
          disabled={mutation.isPending}
          className="bg-blue-600 text-white rounded px-4 py-2 w-full disabled:opacity-50"
        >
          {mutation.isPending ? 'Ingresando...' : 'Ingresar'}
        </button>
        {mutation.isError && (
          <p className="text-red-600 text-sm">
            {mutation.error instanceof ApiError ? mutation.error.message : 'Error inesperado'}
          </p>
        )}
      </form>
    </div>
  )
}