import { Link } from 'react-router-dom'
import { useAuth } from '../features/auth/useAuth'
function App() {
  const { usuario, estaLogueado, cerrarSesion } = useAuth()

  return (
    <div className="p-8 space-y-2">
      <h1 className="text-2xl font-bold text-gray-800">Petshop Sindy</h1>
      <Link to="/catalogo" className="text-blue-600 underline block">
        Ver catálogo
      </Link>
      {estaLogueado ? (
        <div>
          <p className="text-gray-700">Hola, {usuario?.nombre} ({usuario?.rol})</p>
          <button onClick={cerrarSesion} className="text-red-600 underline">
            Cerrar sesión
          </button>
        </div>
      ) : (
        <Link to="/login" className="text-blue-600 underline block">
          Iniciar sesión
        </Link>
      )}
    </div>
  )
}

export default App