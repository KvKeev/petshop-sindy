// API Client for Sindy Petshop Backend (.NET 9)

const API_BASE_URL = '/api/v1';

// Helper to get auth header
const getAuthHeaders = () => {
  const token = localStorage.getItem('sindy_jwt_token');
  return token ? { 'Authorization': `Bearer ${token}` } : {};
};

export const api = {
  // Productos & Categorías
  async getCategorias() {
    const res = await fetch(`${API_BASE_URL}/categorias`);
    if (!res.ok) throw new Error('Error al cargar categorías');
    return res.json();
  },

  async getProductos(categoriaId = null, page = 1, pageSize = 20) {
    let url = `${API_BASE_URL}/productos?page=${page}&pageSize=${pageSize}`;
    if (categoriaId) {
      url += `&categoriaId=${categoriaId}`;
    }
    const res = await fetch(url);
    if (!res.ok) throw new Error('Error al cargar productos');
    return res.json();
  },

  async getProductoDetalle(id) {
    const res = await fetch(`${API_BASE_URL}/productos/${id}`);
    if (!res.ok) throw new Error('Producto no encontrado');
    return res.json();
  },

  // Autenticación
  async login(email, password) {
    const res = await fetch(`${API_BASE_URL}/auth/login`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ email, password })
    });
    if (!res.ok) {
      const errorData = await res.json().catch(() => ({}));
      throw new Error(errorData.message || 'Credenciales inválidas');
    }
    return res.json();
  },

  async registro(nombre, email, password) {
    const res = await fetch(`${API_BASE_URL}/auth/registro`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ nombre, email, password })
    });
    if (!res.ok) {
      const errorData = await res.json().catch(() => ({}));
      throw new Error(errorData.message || 'Error en el registro');
    }
    return res.json();
  },

  // Mascotas (requiere token)
  async getMascotas() {
    const res = await fetch(`${API_BASE_URL}/mascotas`, {
      headers: getAuthHeaders()
    });
    if (!res.ok) throw new Error('Error al obtener mascotas');
    return res.json();
  },

  async crearMascota(nombre, tipo) {
    const res = await fetch(`${API_BASE_URL}/mascotas`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        ...getAuthHeaders()
      },
      body: JSON.stringify({ nombre, tipo: parseInt(tipo, 10) })
    });
    if (!res.ok) throw new Error('Error al crear mascota');
    return res.json();
  }
};
