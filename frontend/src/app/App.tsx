import { Link } from 'react-router-dom'

function App() {
  return (
    <div className="p-8">
      <h1 className="text-2xl font-bold text-gray-800">Petshop Sindy</h1>
      <p className="text-gray-500">Esqueleto de router funcionando.</p>
      <Link to="/catalogo" className="text-blue-600 underline">
        Ver catálogo
      </Link>
    </div>
  )
}

export default App