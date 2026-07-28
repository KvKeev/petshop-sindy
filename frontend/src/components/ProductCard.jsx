import React, { useState } from 'react';
import { ShoppingBag, Check, Info, Dog, Cat, Package } from 'lucide-react';
import { useCart } from '../context/CartContext';

export const ProductCard = ({ producto, onQuickView }) => {
  const { addToCart } = useCart();
  const variantes = producto.variantes || [];
  const [selectedVarianteIndex, setSelectedVarianteIndex] = useState(0);
  const [added, setAdded] = useState(false);

  const activeVariante = variantes[selectedVarianteIndex] || {
    precio: producto.precio || 0,
    atributo: 'Estándar',
    valor: 'Única',
    stockDisponibleWeb: 10
  };

  const handleAddToCart = (e) => {
    e.stopPropagation();
    addToCart(producto, activeVariante, 1);
    setAdded(true);
    setTimeout(() => setAdded(false), 1500);
  };

  // Image placeholder generator based on category or name
  const getImagePlaceholder = () => {
    if (producto.imagenUrl) return producto.imagenUrl;
    const isDog = (producto.categoriaNombre || producto.nombre || '').toLowerCase().includes('perro');
    const isCat = (producto.categoriaNombre || producto.nombre || '').toLowerCase().includes('gato');
    
    if (isDog) return 'https://images.unsplash.com/photo-1543466835-00a7907e9de1?auto=format&fit=crop&w=600&q=80';
    if (isCat) return 'https://images.unsplash.com/photo-1514888286974-6c03e2ca1dba?auto=format&fit=crop&w=600&q=80';
    return 'https://images.unsplash.com/photo-1601758228041-f3b2795255f1?auto=format&fit=crop&w=600&q=80';
  };

  return (
    <div 
      onClick={() => onQuickView && onQuickView(producto)}
      className="group bg-white rounded-2xl border border-slate-100 shadow-sm hover:shadow-xl hover:border-red-100 transition-all duration-300 flex flex-col justify-between overflow-hidden cursor-pointer"
    >
      {/* Product Image & Badges */}
      <div className="relative aspect-square w-full bg-slate-50 overflow-hidden flex items-center justify-center p-4">
        <img
          src={getImagePlaceholder()}
          alt={producto.nombre}
          className="w-full h-full object-contain group-hover:scale-105 transition-transform duration-300"
          loading="lazy"
        />

        {/* Category Pill */}
        {producto.categoriaNombre && (
          <span className="absolute top-3 left-3 bg-white/90 backdrop-blur-md text-slate-700 text-[10px] font-bold px-2.5 py-1 rounded-full shadow-sm border border-slate-100">
            {producto.categoriaNombre}
          </span>
        )}

        {/* Quick View Icon */}
        <button 
          onClick={(e) => { e.stopPropagation(); onQuickView(producto); }}
          className="absolute top-3 right-3 opacity-0 group-hover:opacity-100 transition-opacity bg-white p-2 rounded-full shadow-md text-slate-600 hover:text-red-600"
          title="Ver detalle"
        >
          <Info className="w-4 h-4" />
        </button>
      </div>

      {/* Product Body */}
      <div className="p-4 flex-1 flex flex-col justify-between space-y-3">
        <div>
          <h3 className="font-bold text-sm text-slate-800 line-clamp-2 group-hover:text-red-600 transition-colors">
            {producto.nombre}
          </h3>
          {producto.descripcion && (
            <p className="text-xs text-slate-500 line-clamp-2 mt-1">
              {producto.descripcion}
            </p>
          )}
        </div>

        {/* Variant Selector Buttons */}
        {variantes.length > 1 && (
          <div className="space-y-1" onClick={(e) => e.stopPropagation()}>
            <label className="text-[10px] uppercase tracking-wider text-slate-400 font-bold">
              Seleccionar variante:
            </label>
            <div className="flex flex-wrap gap-1.5">
              {variantes.map((v, idx) => (
                <button
                  key={v.id || idx}
                  onClick={() => setSelectedVarianteIndex(idx)}
                  className={`text-xs px-2.5 py-1 rounded-lg border font-medium transition-all ${
                    selectedVarianteIndex === idx
                      ? 'bg-red-50 text-red-600 border-red-500 font-bold shadow-xs'
                      : 'bg-slate-50 text-slate-600 border-slate-200 hover:bg-slate-100'
                  }`}
                >
                  {v.valor || `${v.atributo}: ${v.valor}`}
                </button>
              ))}
            </div>
          </div>
        )}

        {/* Price & Cart CTA */}
        <div className="pt-2 border-t border-slate-100 flex items-center justify-between gap-2">
          <div>
            <div className="text-xs text-slate-400 font-medium">Precio</div>
            <div className="text-lg font-black text-slate-900">
              ${activeVariante.precio.toLocaleString('es-AR')}
            </div>
          </div>

          <button
            onClick={handleAddToCart}
            className={`flex items-center gap-1.5 px-4 py-2 rounded-xl text-xs font-bold transition-all duration-200 shadow-sm ${
              added
                ? 'bg-emerald-600 text-white'
                : 'bg-red-600 hover:bg-red-700 text-white hover:shadow-md active:scale-95'
            }`}
          >
            {added ? (
              <>
                <Check className="w-3.5 h-3.5" />
                Agregado
              </>
            ) : (
              <>
                <ShoppingBag className="w-3.5 h-3.5" />
                Agregar
              </>
            )}
          </button>
        </div>
      </div>
    </div>
  );
};
