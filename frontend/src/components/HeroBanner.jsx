import React from 'react';
import { Scissors, ShoppingBag, Phone, MapPin, Sparkles, CheckCircle2, Heart } from 'lucide-react';

export const HeroBanner = ({ onShopClick, onPeluqueriaClick }) => {
  return (
    <section className="relative overflow-hidden bg-gradient-to-br from-red-600 via-red-600 to-red-700 text-white rounded-3xl my-4 mx-4 sm:mx-6 lg:mx-8 shadow-xl border border-red-500/30">
      
      {/* Decorative Blur Orbs */}
      <div className="absolute top-0 right-0 -translate-y-12 translate-x-12 w-96 h-96 bg-cyan-400/25 rounded-full blur-3xl pointer-events-none" />
      <div className="absolute bottom-0 left-0 translate-y-12 -translate-x-12 w-80 h-80 bg-red-950/40 rounded-full blur-2xl pointer-events-none" />

      <div className="relative max-w-7xl mx-auto px-6 sm:px-10 py-10 sm:py-14 grid grid-cols-1 lg:grid-cols-12 gap-8 items-center">
        
        {/* Left Column: Brand Motto & CTA */}
        <div className="lg:col-span-7 space-y-6 text-center lg:text-left">
          
          {/* Sindy Authentic Paw Logo Badge */}
          <div className="inline-flex items-center gap-3 bg-slate-950/80 backdrop-blur-md px-4 py-2 rounded-2xl border border-slate-800 shadow-md">
            <img 
              src="/logo.jpg" 
              alt="Sindy Petshop Logo" 
              className="w-9 h-9 object-contain rounded-lg"
            />
            <div className="text-left">
              <span className="text-xs font-black text-white tracking-wide block uppercase">
                TIENDA PARA MASCOTAS
              </span>
              <span className="text-[10px] text-cyan-300 font-extrabold uppercase tracking-wider block">
                & PELUQUERÍA CANINA
              </span>
            </div>
          </div>

          {/* Slogan with Disney-style Disney-font feel */}
          <h1 className="text-3xl sm:text-4xl lg:text-5xl font-black tracking-tight leading-tight">
            Dale a tu mascota el <span className="text-cyan-300 underline decoration-cyan-300/40 decoration-wavy decoration-2">amor</span> que se merece
          </h1>

          <p className="text-red-100 text-sm sm:text-base max-w-xl mx-auto lg:mx-0 font-medium leading-relaxed">
            Con los mejores productos de nuestra tienda. Alimentos de calidad, accesorios y cuidado higiénico especializado en Mar del Plata.
          </p>

          {/* Key Value Props */}
          <div className="flex flex-wrap justify-center lg:justify-start gap-2.5 text-xs font-semibold text-red-50">
            <span className="flex items-center gap-1.5 bg-black/20 px-3 py-1.5 rounded-full border border-white/10">
              <CheckCircle2 className="w-3.5 h-3.5 text-cyan-300" />
              Todas las razas y tamaños
            </span>
            <span className="flex items-center gap-1.5 bg-black/20 px-3 py-1.5 rounded-full border border-white/10">
              <CheckCircle2 className="w-3.5 h-3.5 text-cyan-300" />
              Retiro en local o Envío local
            </span>
            <span className="flex items-center gap-1.5 bg-black/20 px-3 py-1.5 rounded-full border border-white/10">
              <CheckCircle2 className="w-3.5 h-3.5 text-cyan-300" />
              Av. Jara 1635
            </span>
          </div>

          {/* Action Buttons */}
          <div className="flex flex-wrap justify-center lg:justify-start gap-3 pt-2">
            <button
              onClick={onShopClick}
              className="flex items-center gap-2 px-6 py-3 bg-white text-red-600 font-extrabold text-xs uppercase tracking-wider rounded-full shadow-lg hover:bg-red-50 hover:scale-105 transition-all duration-200"
            >
              <ShoppingBag className="w-4 h-4 text-red-600" />
              Ver Productos
            </button>
            <button
              onClick={onPeluqueriaClick}
              className="flex items-center gap-2 px-6 py-3 bg-cyan-300 text-slate-950 font-extrabold text-xs uppercase tracking-wider rounded-full shadow-lg hover:bg-cyan-200 hover:scale-105 transition-all duration-200"
            >
              <Scissors className="w-4 h-4 text-slate-950" />
              Turnos Peluquería
            </button>
          </div>
        </div>

        {/* Right Column: Peluquería Canina Highlight Box */}
        <div className="lg:col-span-5">
          <div className="bg-slate-950/75 backdrop-blur-md rounded-3xl p-6 border border-slate-800 shadow-2xl relative">
            <div className="flex items-center justify-between border-b border-slate-800 pb-3 mb-4">
              <div className="flex items-center gap-3">
                <div className="w-10 h-10 bg-red-600 rounded-2xl flex items-center justify-center text-white font-bold shadow-md">
                  <Scissors className="w-5 h-5" />
                </div>
                <div>
                  <h3 className="font-extrabold text-sm text-white uppercase tracking-wider">Peluquería Canina</h3>
                  <p className="text-[11px] text-cyan-300 font-medium">Baños & Cortes Sanitarios</p>
                </div>
              </div>
              <span className="text-[10px] bg-emerald-500/20 text-emerald-400 font-extrabold px-2.5 py-1 rounded-full uppercase border border-emerald-500/30">
                Av. Jara 1635
              </span>
            </div>

            <ul className="grid grid-cols-2 gap-2 text-xs text-slate-200 font-medium mb-5">
              <li className="flex items-center gap-2 bg-slate-900/80 p-2.5 rounded-xl border border-slate-800">
                <span className="text-cyan-400 font-bold">✂️</span> Corte de razas
              </li>
              <li className="flex items-center gap-2 bg-slate-900/80 p-2.5 rounded-xl border border-slate-800">
                <span className="text-cyan-400 font-bold">🛁</span> Baños sanitarios
              </li>
              <li className="flex items-center gap-2 bg-slate-900/80 p-2.5 rounded-xl border border-slate-800">
                <span className="text-cyan-400 font-bold">🐾</span> Corte higiénico
              </li>
              <li className="flex items-center gap-2 bg-slate-900/80 p-2.5 rounded-xl border border-slate-800">
                <span className="text-cyan-400 font-bold">✨</span> Corte de uñas
              </li>
              <li className="flex items-center gap-2 bg-slate-900/80 p-2.5 rounded-xl border border-slate-800">
                <span className="text-cyan-400 font-bold">🧼</span> Limpieza de oídos
              </li>
              <li className="flex items-center gap-2 bg-slate-900/80 p-2.5 rounded-xl border border-slate-800">
                <span className="text-cyan-400 font-bold">🌸</span> Limpieza glándulas
              </li>
            </ul>

            <a
              href="https://wa.me/5492236362266?text=Hola!%20Quiero%20pedir%20un%20turno%20para%20peluquer%C3%ADa%20canina%20en%20Sindy."
              target="_blank"
              rel="noopener noreferrer"
              className="w-full flex items-center justify-center gap-2 py-3 bg-emerald-500 hover:bg-emerald-600 text-white font-extrabold text-xs uppercase tracking-wider rounded-xl shadow-lg transition-transform active:scale-95"
            >
              <Phone className="w-4 h-4" />
              Pedir Turno WA: 2236362266
            </a>
          </div>
        </div>

      </div>
    </section>
  );
};
