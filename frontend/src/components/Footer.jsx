import React from 'react';
import { MapPin, Phone, Scissors, Heart, Clock } from 'lucide-react';

export const Footer = () => {
  return (
    <footer className="bg-slate-950 text-slate-400 text-xs border-t border-slate-900 mt-16">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12 grid grid-cols-1 md:grid-cols-4 gap-8">
        
        {/* Brand Column */}
        <div className="space-y-3 md:col-span-1">
          <div className="flex items-center gap-3">
            <div className="w-10 h-10 bg-black rounded-xl p-1 border border-slate-800 flex items-center justify-center">
              <img src="/logo.jpg" alt="Sindy Logo" className="w-full h-full object-contain rounded-lg" />
            </div>
            <div>
              <span className="font-extrabold text-xl text-white tracking-tight block">Sindy</span>
              <span className="text-red-500 font-bold text-[10px] uppercase tracking-wider block">
                Petshop & Peluquería
              </span>
            </div>
          </div>
          <p className="text-slate-400 leading-relaxed text-[11px]">
            Tu tienda de confianza y peluquería canina en Mar del Plata. Alimentos de calidad, accesorios y estética para tus mascotas.
          </p>
          <div className="pt-1">
            <a 
              href="https://www.instagram.com/petshop_sindy/" 
              target="_blank" 
              rel="noopener noreferrer"
              className="inline-flex items-center gap-1.5 px-3 py-1.5 bg-slate-900 hover:bg-slate-800 text-pink-400 font-bold text-xs rounded-lg border border-slate-800 transition-colors"
            >
              <svg className="w-3.5 h-3.5 fill-current" viewBox="0 0 24 24">
                <path d="M12 2.163c3.204 0 3.584.012 4.85.07 3.252.148 4.771 1.691 4.919 4.919.058 1.265.069 1.645.069 4.849 0 3.205-.012 3.584-.069 4.849-.149 3.225-1.664 4.771-4.919 4.919-1.266.058-1.644.07-4.85.07-3.204 0-3.584-.012-4.849-.07-3.26-.149-4.771-1.699-4.919-4.92-.058-1.265-.07-1.644-.07-4.849 0-3.204.013-3.583.07-4.849.149-3.227 1.664-4.771 4.919-4.919 1.266-.057 1.645-.069 4.849-.069zm0-2.163c-3.259 0-3.667.014-4.947.072-4.358.2-6.78 2.618-6.98 6.98-.059 1.281-.073 1.689-.073 4.948 0 3.259.014 3.668.072 4.948.2 4.358 2.618 6.78 6.98 6.98 1.281.058 1.689.072 4.948.072 3.259 0 3.668-.014 4.948-.072 4.354-.2 6.782-2.618 6.979-6.98.059-1.28.073-1.689.073-4.948 0-3.259-.014-3.667-.072-4.947-.196-4.354-2.617-6.78-6.979-6.98-1.281-.059-1.69-.073-4.949-.073zm0 5.838c-3.403 0-6.162 2.759-6.162 6.162s2.759 6.163 6.162 6.163 6.162-2.759 6.162-6.163c0-3.403-2.759-6.162-6.162-6.162zm0 10.162c-2.209 0-4-1.79-4-4 0-2.209 1.791-4 4-4s4 1.791 4 4c0 2.21-1.791 4-4 4zm6.406-11.845c-.796 0-1.441.645-1.441 1.44s.645 1.44 1.441 1.44c.795 0 1.439-.645 1.439-1.44s-.644-1.44-1.439-1.44z"/>
              </svg>
              <span>@petshop_sindy</span>
            </a>
          </div>
        </div>

        {/* Peluquería Info */}
        <div className="space-y-2">
          <h4 className="font-bold text-white uppercase tracking-wider text-[11px]">Peluquería Canina</h4>
          <ul className="space-y-1.5 text-slate-400 text-[11px]">
            <li>• Corte de razas y tijera</li>
            <li>• Baños sanitarios pulguicida</li>
            <li>• Corte higiénico & uñas</li>
            <li>• Limpieza de oídos & glándulas</li>
            <li className="text-cyan-400 font-semibold pt-1">Atendemos todas las razas y tamaños</li>
          </ul>
        </div>

        {/* Contact Info */}
        <div className="space-y-2">
          <h4 className="font-bold text-white uppercase tracking-wider text-[11px]">Contacto & Ubicación</h4>
          <div className="space-y-2 text-[11px]">
            <div className="flex items-center gap-2">
              <MapPin className="w-4 h-4 text-red-500 shrink-0" />
              <span>Av. Jara 1635, Mar del Plata</span>
            </div>
            <div className="flex items-center gap-2">
              <Phone className="w-4 h-4 text-emerald-400 shrink-0" />
              <span>Turnos WA: 2236362266</span>
            </div>
            <div className="flex items-center gap-2">
              <Clock className="w-4 h-4 text-cyan-400 shrink-0" />
              <span>Lun a Sáb: 9:00 - 19:30 hs</span>
            </div>
          </div>
        </div>

        {/* Delivery Care */}
        <div className="space-y-2">
          <h4 className="font-bold text-white uppercase tracking-wider text-[11px]">Envíos & Retiros</h4>
          <p className="text-slate-400 leading-relaxed text-[11px]">
            • Retiro sin costo en nuestro local de Av. Jara 1635.<br />
            • Envíos a domicilio en zona local.
          </p>
          <div className="pt-2">
            <a
              href="https://wa.me/5492236362266?text=Hola!%20Consulta%20desde%20la%20web"
              target="_blank"
              rel="noopener noreferrer"
              className="inline-flex items-center gap-1.5 px-3.5 py-2 bg-emerald-600 hover:bg-emerald-700 text-white font-bold text-xs rounded-xl shadow-sm transition-colors"
            >
              <Phone className="w-3.5 h-3.5" />
              Pedir Turno por WhatsApp
            </a>
          </div>
        </div>

      </div>

      {/* Copyright */}
      <div className="border-t border-slate-900 py-4 text-center text-slate-500 text-[11px]">
        © {new Date().getFullYear()} Sindy Petshop & Peluquería Canina (Av. Jara 1635). Todos los derechos reservados.
      </div>
    </footer>
  );
};
