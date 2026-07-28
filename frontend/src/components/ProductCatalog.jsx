import React, { useState, useEffect } from 'react';
import { api } from '../services/api';
import { ProductCard } from './ProductCard';
import { Filter, Dog, Cat, Sparkles, AlertCircle, RefreshCw } from 'lucide-react';

export const ProductCatalog = ({ selectedCategory, onSelectCategory, searchQuery, onQuickView }) => {
  const [categorias, setCategorias] = useState([]);
  const [productos, setProductos] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    loadCatalogData();
  }, []);

  const loadCatalogData = async () => {
    setLoading(true);
    setError(null);
    try {
      const [catData, prodData] = await Promise.all([
        api.getCategorias().catch(() => []),
        api.getProductos().catch(() => ({ items: [] }))
      ]);
      setCategorias(catData || []);
      
      // Extract items list
      const itemsList = Array.isArray(prodData) ? prodData : (prodData.items || []);
      setProductos(itemsList);
    } catch (err) {
      console.error('Error al cargar catálogo:', err);
      setError('No se pudieron obtener los productos. Asegúrate de que el backend .NET esté corriendo.');
    } finally {
      setLoading(false);
    }
  };

  // Filter products by selected category & search query
  const filteredProducts = productos.filter(p => {
    const matchesCategory = !selectedCategory || p.categoriaId === selectedCategory;
    const query = searchQuery.toLowerCase().trim();
    const matchesSearch = !query || 
      p.nombre.toLowerCase().includes(query) || 
      (p.descripcion && p.descripcion.toLowerCase().includes(query)) ||
      (p.categoriaNombre && p.categoriaNombre.toLowerCase().includes(query));
    return matchesCategory && matchesSearch;
  });

  return (
    <section id="catalogo" className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      {/* Category Pills & Filters */}
      <div className="flex flex-col md:flex-row md:items-center justify-between gap-4 mb-8">
        <div>
          <div className="flex items-center gap-2">
            <span className="text-red-600 font-bold text-xs uppercase tracking-wider bg-red-50 px-2.5 py-1 rounded-full border border-red-100">
              Nuestro Catálogo
            </span>
          </div>
          <h2 className="text-2xl sm:text-3xl font-extrabold text-slate-900 mt-1">
            Productos destacados para tus mascotas
          </h2>
        </div>

        {/* Category Selector Pills */}
        <div className="flex flex-wrap items-center gap-2">
          <button
            onClick={() => onSelectCategory(null)}
            className={`px-4 py-2 rounded-full text-xs font-bold transition-all ${
              selectedCategory === null
                ? 'bg-red-600 text-white shadow-md shadow-red-500/20'
                : 'bg-white text-slate-600 border border-slate-200 hover:bg-slate-50'
            }`}
          >
            Todos
          </button>

          {categorias.map(cat => (
            <button
              key={cat.id}
              onClick={() => onSelectCategory(cat.id)}
              className={`px-4 py-2 rounded-full text-xs font-bold transition-all ${
                selectedCategory === cat.id
                  ? 'bg-red-600 text-white shadow-md shadow-red-500/20'
                  : 'bg-white text-slate-600 border border-slate-200 hover:bg-slate-50'
              }`}
            >
              {cat.nombre}
            </button>
          ))}
        </div>
      </div>

      {/* Loading State */}
      {loading && (
        <div className="py-20 text-center flex flex-col items-center justify-center space-y-3">
          <div className="w-10 h-10 border-4 border-red-600 border-t-transparent rounded-full animate-spin" />
          <p className="text-sm font-medium text-slate-500">Cargando productos de Sindy...</p>
        </div>
      )}

      {/* Error State */}
      {!loading && error && (
        <div className="bg-red-50 border border-red-200 text-red-700 rounded-2xl p-6 text-center max-w-lg mx-auto my-8">
          <AlertCircle className="w-8 h-8 text-red-500 mx-auto mb-2" />
          <p className="text-sm font-bold mb-1">{error}</p>
          <p className="text-xs text-red-600 mb-4">Verifica que la API en http://localhost:5171 esté en ejecución.</p>
          <button
            onClick={loadCatalogData}
            className="inline-flex items-center gap-2 px-4 py-2 bg-red-600 text-white text-xs font-bold rounded-xl hover:bg-red-700 transition-colors shadow-sm"
          >
            <RefreshCw className="w-3.5 h-3.5" />
            Reintentar
          </button>
        </div>
      )}

      {/* Empty State */}
      {!loading && !error && filteredProducts.length === 0 && (
        <div className="py-16 text-center bg-white rounded-3xl border border-slate-100 p-8 max-w-md mx-auto">
          <div className="w-14 h-14 bg-red-50 text-red-500 rounded-full flex items-center justify-center mx-auto mb-3">
            <Filter className="w-6 h-6" />
          </div>
          <h3 className="font-bold text-slate-800 text-base mb-1">No se encontraron productos</h3>
          <p className="text-xs text-slate-500 mb-4">
            Intenta cambiar el término de búsqueda o selecciona otra categoría.
          </p>
          <button
            onClick={() => { onSelectCategory(null); }}
            className="px-4 py-2 bg-slate-100 hover:bg-slate-200 text-slate-700 text-xs font-bold rounded-full transition-colors"
          >
            Ver todos los productos
          </button>
        </div>
      )}

      {/* Products Grid */}
      {!loading && !error && filteredProducts.length > 0 && (
        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
          {filteredProducts.map(prod => (
            <ProductCard
              key={prod.id}
              producto={prod}
              onQuickView={onQuickView}
            />
          ))}
        </div>
      )}
    </section>
  );
};
