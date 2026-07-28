import React, { useState } from 'react';
import { X, ShoppingBag, Check, ShieldCheck, Truck, Store } from 'lucide-react';
import { useCart } from '../context/CartContext';

export const ProductModal = ({ producto, onClose }) => {
  const { addToCart } = useCart();
  const variantes = producto?.variantes || [];
  const [selectedVarianteIndex, setSelectedVarianteIndex] = useState(0);
  const [cantidad, setCantidad] = useState(1);
  const [added, setAdded] = useState(false);

  if (!producto) return null;

  const activeVariante = variantes[selectedVarianteIndex] || {
    precio: producto.precio || 0,
    atributo: 'Estándar',
    valor: 'Única',
    stockDisponibleWeb: 10
  };

  const handleAddToCart = () => {
    addToCart(producto, activeVariante, cantidad);
    setAdded(true);
    setTimeout(() => {
      setAdded(false);
      onClose();
    }, 1200);
  };

  const getImagePlaceholder = () => {
    if (producto.imagenUrl) return producto.imagenUrl;
    const isDog = (producto.categoriaNombre || producto.nombre || '').toLowerCase().includes('perro');
    const isCat = (producto.categoriaNombre || producto.nombre || '').toLowerCase().includes('gato');
    
    if (isDog) return 'https://images.unsplash.com/photo-1543466835-00a7907e9de1?auto=format&fit=crop&w=600&q=80';
    if (isCat) return 'https://images.unsplash.com/photo-1514888286974-6c03e2ca1dba?auto=format&fit=crop&w=600&q=80';
    return 'https://images.unsplash.com/photo-1601758228041-f3b2795255f1?auto=format&fit=crop&w=600&q=80';
  };

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center p-4 bg-slate-900/60 backdrop-blur-sm animate-in fade-in duration-200">
      <div className="relative w-full max-w-2xl bg-white rounded-3xl shadow-2xl overflow-hidden border border-slate-100 max-h-[90vh] flex flex-col md:flex-row">
        {/* Close Button */}
        <button
          onClick={onClose}
          className="absolute top-4 right-4 z-10 p-2 text-slate-400 hover:text-slate-600 hover:bg-slate-100 rounded-full transition-colors"
        >
          <X className="w-5 h-5" />
        </button>

        {/* Left: Image Container */}
        <div className="w-full md:w-1/2 bg-slate-50 p-6 flex items-center justify-center">
          <img
            src={getImagePlaceholder()}
            alt={producto.nombre}
            className="w-full max-h-72 object-contain rounded-xl"
          />
        </div>

        {/* Right: Product Info */}
        <div className="w-full md:w-1/2 p-6 flex flex-col justify-between overflow-y-auto">
          <div className="space-y-4">
            {/* Category Pill */}
            {producto.categoriaNombre && (
              <span className="inline-block bg-red-50 text-red-600 text-xs font-bold px-3 py-1 rounded-full border border-red-100 uppercase tracking-wider">
                {producto.categoriaNombre}
              </span>
            )}

            <div>
              <h2 className="text-xl font-bold text-slate-900 leading-tight">
                {producto.nombre}
              </h2>
              {producto.descripcion && (
                <p className="text-xs text-slate-600 mt-2 leading-relaxed">
                  {producto.descripcion}
                </p>
              )}
            </div>

            {/* Variant Selector */}
            {variantes.length > 0 && (
              <div className="space-y-2 pt-2 border-t border-slate-100">
                <label className="text-xs font-bold text-slate-700 uppercase tracking-wider block">
                  Variante disponible:
                </label>
                <div className="grid grid-cols-2 gap-2">
                  {variantes.map((v, idx) => (
                    <button
                      key={v.id || idx}
                      onClick={() => setSelectedVarianteIndex(idx)}
                      className={`p-2.5 rounded-xl text-left border text-xs font-medium transition-all ${
                        selectedVarianteIndex === idx
                          ? 'border-red-500 bg-red-50/50 text-red-600 font-bold ring-2 ring-red-500/20'
                          : 'border-slate-200 hover:border-slate-300 text-slate-700'
                      }`}
                    >
                      <div className="text-[11px] text-slate-400 font-semibold">{v.atributo}</div>
                      <div>{v.valor}</div>
                      <div className="text-xs font-bold text-slate-900 mt-1">${v.precio.toLocaleString('es-AR')}</div>
                    </button>
                  ))}
                </div>
              </div>
            )}

            {/* Quantity Selector */}
            <div className="flex items-center space-x-3 pt-2">
              <span className="text-xs font-bold text-slate-700 uppercase">Cantidad:</span>
              <div className="flex items-center border border-slate-200 rounded-xl overflow-hidden bg-slate-50">
                <button
                  onClick={() => setCantidad(Math.max(1, cantidad - 1))}
                  className="px-3 py-1.5 text-slate-600 hover:bg-slate-200 text-xs font-bold transition-colors"
                >
                  -
                </button>
                <span className="px-4 text-xs font-bold text-slate-800">{cantidad}</span>
                <button
                  onClick={() => setCantidad(cantidad + 1)}
                  className="px-3 py-1.5 text-slate-600 hover:bg-slate-200 text-xs font-bold transition-colors"
                >
                  +
                </button>
              </div>
            </div>

            {/* Delivery Perks */}
            <div className="space-y-1.5 pt-2 text-[11px] text-slate-500">
              <div className="flex items-center gap-1.5">
                <Store className="w-3.5 h-3.5 text-red-500" />
                Retiro gratis en local: <strong>Av. Jara 1635</strong>
              </div>
              <div className="flex items-center gap-1.5">
                <Truck className="w-3.5 h-3.5 text-cyan-500" />
                Envío a domicilio disponible en zona local
              </div>
            </div>
          </div>

          {/* Action Footer */}
          <div className="pt-4 border-t border-slate-100 flex items-center justify-between gap-4 mt-4">
            <div>
              <span className="text-xs text-slate-400 block font-medium">Total</span>
              <span className="text-xl font-extrabold text-slate-900">
                ${(activeVariante.precio * cantidad).toLocaleString('es-AR')}
              </span>
            </div>

            <button
              onClick={handleAddToCart}
              className={`flex-1 flex items-center justify-center gap-2 py-3 px-6 rounded-2xl text-xs font-bold transition-all shadow-md ${
                added
                  ? 'bg-emerald-600 text-white'
                  : 'bg-red-600 hover:bg-red-700 text-white hover:shadow-lg active:scale-95'
              }`}
            >
              {added ? (
                <>
                  <Check className="w-4 h-4" />
                  ¡Agregado al Carrito!
                </>
              ) : (
                <>
                  <ShoppingBag className="w-4 h-4" />
                  Agregar al Carrito
                </>
              )}
            </button>
          </div>
        </div>
      </div>
    </div>
  );
};
