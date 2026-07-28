import React, { useState } from 'react';
import { X, Trash2, Plus, Minus, ShoppingBag, Store, Truck, ArrowRight, ShieldCheck } from 'lucide-react';
import { useCart } from '../context/CartContext';
import { useAuth } from '../context/AuthContext';

export const CartDrawer = () => {
  const { cartItems, isCartOpen, setIsCartOpen, removeFromCart, updateQuantity, totalPrice, clearCart } = useCart();
  const { isAuthenticated, setIsAuthModalOpen } = useAuth();
  const [metodoEntrega, setMetodoEntrega] = useState('retiro'); // 'retiro' | 'envio'
  const [checkoutSuccess, setCheckoutSuccess] = useState(false);

  if (!isCartOpen) return null;

  const handleCheckout = () => {
    if (!isAuthenticated) {
      setIsAuthModalOpen(true);
      return;
    }
    // Simulate successful order creation
    setCheckoutSuccess(true);
    setTimeout(() => {
      clearCart();
      setCheckoutSuccess(false);
      setIsCartOpen(false);
    }, 2500);
  };

  return (
    <div className="fixed inset-0 z-50 overflow-hidden bg-slate-900/50 backdrop-blur-xs animate-in fade-in duration-200">
      <div className="absolute inset-0" onClick={() => setIsCartOpen(false)} />

      <div className="fixed inset-y-0 right-0 max-w-full flex pl-10">
        <div className="w-screen max-w-md bg-white shadow-2xl flex flex-col justify-between border-l border-slate-100">
          
          {/* Header */}
          <div className="p-4 sm:p-6 border-b border-slate-100 flex items-center justify-between bg-slate-50/50">
            <div className="flex items-center gap-2">
              <div className="w-8 h-8 rounded-full bg-red-50 text-red-600 flex items-center justify-center font-bold">
                <ShoppingBag className="w-4 h-4" />
              </div>
              <h2 className="text-lg font-extrabold text-slate-900">Tu Carrito de Compras</h2>
            </div>
            <button
              onClick={() => setIsCartOpen(false)}
              className="p-2 text-slate-400 hover:text-slate-600 hover:bg-slate-100 rounded-full transition-colors"
            >
              <X className="w-5 h-5" />
            </button>
          </div>

          {/* Cart Items List */}
          <div className="flex-1 overflow-y-auto p-4 sm:p-6 space-y-4">
            {checkoutSuccess ? (
              <div className="py-16 text-center space-y-3 animate-in zoom-in-95 duration-200">
                <div className="w-16 h-16 bg-emerald-100 text-emerald-600 rounded-full flex items-center justify-center mx-auto">
                  <ShieldCheck className="w-8 h-8" />
                </div>
                <h3 className="text-lg font-extrabold text-slate-900">¡Pedido registrado con éxito!</h3>
                <p className="text-xs text-slate-500 max-w-xs mx-auto">
                  Tu pedido ha sido procesado correctamente. ¡Gracias por confiar en Sindy Petshop!
                </p>
              </div>
            ) : cartItems.length === 0 ? (
              <div className="py-20 text-center space-y-3">
                <div className="w-16 h-16 bg-slate-100 text-slate-400 rounded-full flex items-center justify-center mx-auto">
                  <ShoppingBag className="w-8 h-8" />
                </div>
                <p className="text-sm font-bold text-slate-700">Tu carrito está vacío</p>
                <p className="text-xs text-slate-400">Descubre nuestros productos y agrégalos aquí.</p>
              </div>
            ) : (
              cartItems.map((item) => (
                <div 
                  key={item.varianteId}
                  className="flex items-center gap-3 p-3 bg-slate-50 rounded-2xl border border-slate-100 hover:border-slate-200 transition-colors"
                >
                  <img
                    src={item.imagenUrl || 'https://images.unsplash.com/photo-1601758228041-f3b2795255f1?auto=format&fit=crop&w=200&q=80'}
                    alt={item.nombre}
                    className="w-14 h-14 object-contain rounded-xl bg-white p-1 border border-slate-100 shrink-0"
                  />

                  <div className="flex-1 min-w-0">
                    <h4 className="text-xs font-bold text-slate-800 truncate">{item.nombre}</h4>
                    <p className="text-[11px] text-slate-400">
                      {item.atributo}: <span className="font-semibold text-slate-600">{item.valor}</span>
                    </p>
                    <div className="text-xs font-extrabold text-slate-900 mt-1">
                      ${(item.precio * item.cantidad).toLocaleString('es-AR')}
                    </div>
                  </div>

                  {/* Quantity controls & remove */}
                  <div className="flex flex-col items-end space-y-1">
                    <button
                      onClick={() => removeFromCart(item.varianteId)}
                      className="text-slate-300 hover:text-red-500 p-1 transition-colors"
                      title="Eliminar"
                    >
                      <Trash2 className="w-3.5 h-3.5" />
                    </button>

                    <div className="flex items-center border border-slate-200 rounded-lg overflow-hidden bg-white">
                      <button
                        onClick={() => updateQuantity(item.varianteId, item.cantidad - 1)}
                        className="px-2 py-0.5 text-xs text-slate-600 hover:bg-slate-100 font-bold"
                      >
                        -
                      </button>
                      <span className="px-2 text-xs font-bold text-slate-800">{item.cantidad}</span>
                      <button
                        onClick={() => updateQuantity(item.varianteId, item.cantidad + 1)}
                        className="px-2 py-0.5 text-xs text-slate-600 hover:bg-slate-100 font-bold"
                      >
                        +
                      </button>
                    </div>
                  </div>
                </div>
              ))
            )}
          </div>

          {/* Footer & Checkout Options */}
          {cartItems.length > 0 && !checkoutSuccess && (
            <div className="p-4 sm:p-6 border-t border-slate-100 bg-white space-y-4">
              {/* Delivery Selector */}
              <div className="space-y-2">
                <label className="text-[11px] uppercase font-bold text-slate-400 tracking-wider">
                  Método de Entrega:
                </label>
                <div className="grid grid-cols-2 gap-2">
                  <button
                    onClick={() => setMetodoEntrega('retiro')}
                    className={`p-2.5 rounded-xl border text-left text-xs font-bold transition-all flex items-center gap-2 ${
                      metodoEntrega === 'retiro'
                        ? 'border-red-500 bg-red-50 text-red-600 ring-2 ring-red-500/20'
                        : 'border-slate-200 text-slate-600 hover:bg-slate-50'
                    }`}
                  >
                    <Store className="w-4 h-4 text-red-500 shrink-0" />
                    <div>
                      <div>Retiro en Local</div>
                      <div className="text-[10px] text-slate-400 font-normal">Av. Jara 1635</div>
                    </div>
                  </button>

                  <button
                    onClick={() => setMetodoEntrega('envio')}
                    className={`p-2.5 rounded-xl border text-left text-xs font-bold transition-all flex items-center gap-2 ${
                      metodoEntrega === 'envio'
                        ? 'border-red-500 bg-red-50 text-red-600 ring-2 ring-red-500/20'
                        : 'border-slate-200 text-slate-600 hover:bg-slate-50'
                    }`}
                  >
                    <Truck className="w-4 h-4 text-cyan-500 shrink-0" />
                    <div>
                      <div>Envío a Domicilio</div>
                      <div className="text-[10px] text-slate-400 font-normal">Zona Local</div>
                    </div>
                  </button>
                </div>
              </div>

              {/* Total & Checkout */}
              <div className="pt-2 border-t border-slate-100 flex items-center justify-between">
                <span className="text-sm text-slate-600 font-medium">Total Estimado</span>
                <span className="text-2xl font-black text-slate-900">
                  ${totalPrice.toLocaleString('es-AR')}
                </span>
              </div>

              {!isAuthenticated && (
                <div className="text-center bg-amber-50 border border-amber-200 rounded-xl p-2.5 text-amber-800 text-xs font-medium">
                  🔒 Para finalizar la compra debes ingresar a tu cuenta.
                </div>
              )}

              <button
                onClick={handleCheckout}
                className="w-full flex items-center justify-center gap-2 py-3 px-6 bg-red-600 hover:bg-red-700 text-white font-extrabold text-sm rounded-xl shadow-lg transition-transform active:scale-95"
              >
                <span>{isAuthenticated ? 'Finalizar Compra' : 'Iniciar Sesión para Comprar'}</span>
                <ArrowRight className="w-4 h-4" />
              </button>
            </div>
          )}

        </div>
      </div>
    </div>
  );
};
