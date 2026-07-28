import React, { useState } from 'react';
import { ShoppingBag, User, Search, MapPin, Phone, Scissors, Heart, LogOut, Dog } from 'lucide-react';
import { useAuth } from '../context/AuthContext';
import { useCart } from '../context/CartContext';

export const Navbar = ({ onSelectCategory, selectedCategory, searchQuery, setSearchQuery, onOpenPeluqueria }) => {
  const { user, isAuthenticated, logoutUser, setIsAuthModalOpen, setIsMascotasModalOpen } = useAuth();
  const { totalCount, setIsCartOpen } = useCart();
  const [showUserMenu, setShowUserMenu] = useState(false);

  return (
    <header className="sticky top-0 z-40 w-full shadow-sm bg-white/95 backdrop-blur-md border-b border-slate-100">
      {/* Top Announcement Bar */}
      <div className="bg-slate-900 text-white text-xs py-2 px-4">
        <div className="max-w-7xl mx-auto flex flex-wrap justify-between items-center gap-2">
          <div className="flex items-center space-x-4">
            <span className="flex items-center gap-1.5 font-medium text-slate-300">
              <MapPin className="w-3.5 h-3.5 text-red-500" />
              Av. Jara 1635, Mar del Plata
            </span>
            <span className="hidden md:inline text-slate-700">|</span>
            <span className="hidden md:flex items-center gap-1.5 font-medium text-slate-300">
              <Scissors className="w-3.5 h-3.5 text-cyan-400" />
              Peluquería Canina & Tienda de Mascotas
            </span>
          </div>

          <div className="flex items-center space-x-4">
            <a 
              href="https://www.instagram.com/petshop_sindy/" 
              target="_blank" 
              rel="noopener noreferrer"
              className="flex items-center gap-1.5 text-slate-300 hover:text-pink-400 transition-colors font-medium"
            >
              <svg className="w-3.5 h-3.5 text-pink-400 fill-current" viewBox="0 0 24 24">
                <path d="M12 2.163c3.204 0 3.584.012 4.85.07 3.252.148 4.771 1.691 4.919 4.919.058 1.265.069 1.645.069 4.849 0 3.205-.012 3.584-.069 4.849-.149 3.225-1.664 4.771-4.919 4.919-1.266.058-1.644.07-4.85.07-3.204 0-3.584-.012-4.849-.07-3.26-.149-4.771-1.699-4.919-4.92-.058-1.265-.07-1.644-.07-4.849 0-3.204.013-3.583.07-4.849.149-3.227 1.664-4.771 4.919-4.919 1.266-.057 1.645-.069 4.849-.069zm0-2.163c-3.259 0-3.667.014-4.947.072-4.358.2-6.78 2.618-6.98 6.98-.059 1.281-.073 1.689-.073 4.948 0 3.259.014 3.668.072 4.948.2 4.358 2.618 6.78 6.98 6.98 1.281.058 1.689.072 4.948.072 3.259 0 3.668-.014 4.948-.072 4.354-.2 6.782-2.618 6.979-6.98.059-1.28.073-1.689.073-4.948 0-3.259-.014-3.667-.072-4.947-.196-4.354-2.617-6.78-6.979-6.98-1.281-.059-1.69-.073-4.949-.073zm0 5.838c-3.403 0-6.162 2.759-6.162 6.162s2.759 6.163 6.162 6.163 6.162-2.759 6.162-6.163c0-3.403-2.759-6.162-6.162-6.162zm0 10.162c-2.209 0-4-1.79-4-4 0-2.209 1.791-4 4-4s4 1.791 4 4c0 2.21-1.791 4-4 4zm6.406-11.845c-.796 0-1.441.645-1.441 1.44s.645 1.44 1.441 1.44c.795 0 1.439-.645 1.439-1.44s-.644-1.44-1.439-1.44z"/>
              </svg>
              <span>@petshop_sindy</span>
            </a>
            <span className="text-slate-700">|</span>
            <a 
              href="https://wa.me/5492236362266?text=Hola!%20Consulta%20desde%20la%20web%20de%20Sindy" 
              target="_blank" 
              rel="noopener noreferrer"
              className="flex items-center gap-1.5 font-bold text-emerald-400 hover:text-emerald-300 transition-colors"
            >
              <Phone className="w-3.5 h-3.5" />
              <span>Turnos: 2236362266</span>
            </a>
          </div>
        </div>
      </div>

      {/* Main Navbar */}
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-3 flex items-center justify-between gap-4">
        
        {/* Authentic Logo Branding */}
        <div 
          onClick={() => { onSelectCategory(null); setSearchQuery(''); }}
          className="flex items-center gap-3 cursor-pointer group"
        >
          {/* Logo Container with Dark Glow */}
          <div className="relative w-12 h-12 bg-black rounded-2xl p-1 shadow-md border border-slate-800 flex items-center justify-center group-hover:scale-105 transition-transform duration-200 overflow-hidden">
            <img 
              src="/logo.jpg" 
              alt="Logo Sindy Petshop" 
              className="w-full h-full object-contain rounded-xl"
            />
          </div>

          <div>
            <div className="flex items-center gap-1.5">
              <span className="font-black text-2xl tracking-tight text-slate-900 font-sans">
                Sindy
              </span>
              <span className="bg-red-600 text-white font-extrabold text-[10px] px-2 py-0.5 rounded-full uppercase tracking-wider shadow-xs">
                Petshop
              </span>
            </div>
            <p className="text-[11px] text-slate-500 font-medium hidden sm:block">
              Tienda & Peluquería Canina
            </p>
          </div>
        </div>

        {/* Search Bar */}
        <div className="flex-1 max-w-lg hidden md:block">
          <div className="relative">
            <Search className="absolute left-3.5 top-1/2 -translate-y-1/2 w-4 h-4 text-slate-400" />
            <input
              type="text"
              value={searchQuery}
              onChange={(e) => setSearchQuery(e.target.value)}
              placeholder="¿Qué estás buscando para tu mascota? (ej. Royal Canin, Alimento, Shampoo)..."
              className="w-full pl-10 pr-4 py-2.5 text-xs bg-slate-100/90 border border-slate-200 rounded-full focus:outline-none focus:ring-2 focus:ring-red-500/20 focus:border-red-500 focus:bg-white transition-all placeholder:text-slate-400 font-medium"
            />
            {searchQuery && (
              <button 
                onClick={() => setSearchQuery('')}
                className="absolute right-3 top-1/2 -translate-y-1/2 text-xs text-slate-400 hover:text-slate-600 bg-slate-200 rounded-full w-4 h-4 flex items-center justify-center"
              >
                ✕
              </button>
            )}
          </div>
        </div>

        {/* Action Buttons */}
        <div className="flex items-center space-x-2 sm:space-x-3">
          {/* Peluquería Shortcut Pill */}
          <button
            onClick={onOpenPeluqueria}
            className="flex items-center gap-1.5 px-3.5 py-2 text-xs font-bold text-slate-900 bg-cyan-100 hover:bg-cyan-200 border border-cyan-300 rounded-full transition-all shadow-xs"
          >
            <Scissors className="w-3.5 h-3.5 text-slate-900" />
            <span className="hidden sm:inline">Peluquería Canina</span>
          </button>

          {/* User Account / Auth */}
          <div className="relative">
            {isAuthenticated ? (
              <div className="relative">
                <button
                  onClick={() => setShowUserMenu(!showUserMenu)}
                  className="flex items-center gap-2 px-3 py-1.5 text-sm font-medium text-slate-700 hover:text-red-600 hover:bg-red-50 rounded-full transition-colors border border-slate-200"
                >
                  <div className="w-6 h-6 rounded-full bg-red-600 text-white flex items-center justify-center text-xs font-bold">
                    {user?.nombre ? user.nombre.charAt(0).toUpperCase() : 'U'}
                  </div>
                  <span className="hidden sm:inline max-w-[100px] truncate font-bold text-xs">
                    {user?.nombre || 'Mi Cuenta'}
                  </span>
                </button>

                {/* User Dropdown */}
                {showUserMenu && (
                  <div className="absolute right-0 mt-2 w-48 bg-white rounded-2xl shadow-xl border border-slate-100 py-2 z-50 animate-in fade-in slide-in-from-top-2 duration-150">
                    <div className="px-4 py-2 border-b border-slate-100">
                      <p className="text-xs font-bold text-slate-900 truncate">{user?.nombre || 'Usuario'}</p>
                      <p className="text-[11px] text-slate-500 truncate">{user?.email}</p>
                    </div>
                    <button
                      onClick={() => {
                        setShowUserMenu(false);
                        setIsMascotasModalOpen(true);
                      }}
                      className="w-full text-left px-4 py-2 text-xs text-slate-700 hover:bg-slate-50 flex items-center gap-2 font-semibold"
                    >
                      <Dog className="w-3.5 h-3.5 text-red-500" />
                      Mis Mascotas
                    </button>
                    <button
                      onClick={() => {
                        setShowUserMenu(false);
                        logoutUser();
                      }}
                      className="w-full text-left px-4 py-2 text-xs text-red-600 hover:bg-red-50 flex items-center gap-2 font-bold border-t border-slate-50"
                    >
                      <LogOut className="w-3.5 h-3.5" />
                      Cerrar Sesión
                    </button>
                  </div>
                )}
              </div>
            ) : (
              <button
                onClick={() => setIsAuthModalOpen(true)}
                className="flex items-center gap-1.5 px-3.5 py-2 text-xs font-bold text-slate-700 bg-slate-100 hover:bg-slate-200 rounded-full transition-colors"
              >
                <User className="w-4 h-4 text-slate-600" />
                <span className="hidden sm:inline">Ingresar</span>
              </button>
            )}
          </div>

          {/* Cart Button */}
          <button
            onClick={() => setIsCartOpen(true)}
            className="relative p-2.5 bg-red-50 text-red-600 hover:bg-red-600 hover:text-white rounded-full transition-all duration-200 border border-red-100 shadow-xs"
            aria-label="Carrito de Compras"
          >
            <ShoppingBag className="w-5 h-5" />
            {totalCount > 0 && (
              <span className="absolute -top-1 -right-1 bg-red-600 text-white font-black text-[10px] w-5 h-5 rounded-full flex items-center justify-center border-2 border-white shadow-sm">
                {totalCount}
              </span>
            )}
          </button>
        </div>
      </div>
    </header>
  );
};
