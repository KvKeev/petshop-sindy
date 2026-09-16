import { createBrowserRouter } from 'react-router-dom'
import App from './App'
import { CatalogoPage } from '../features/catalogo/CatalogoPage'
import { LoginPage } from '../features/auth/LoginPage'

export const router = createBrowserRouter([
  { path: '/', element: <App /> },
  { path: '/catalogo', element: <CatalogoPage /> },
  { path: '/login', element: <LoginPage /> },
])