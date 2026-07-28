import React from 'react';
import { Scissors, Phone, MapPin, Sparkles, Check, Heart, Shield, Calendar } from 'lucide-react';

export const PeluqueriaSection = () => {
  const servicios = [
    { titulo: 'Corte de Razas', desc: 'Estética especializada según estándar o preferencia del dueño.' },
    { titulo: 'Baños Sanitarios', desc: 'Tratamiento efectivo pulguicida y garrapaticida para mantener la salud de la piel.' },
    { titulo: 'Corte Higiénico', desc: 'Despeje de zonas sensibles para el máximo confort y limpieza de tu mascota.' },
    { titulo: 'Corte de Uñas', desc: 'Cuidado seguro y profesional de las garras para prevenir molestias.' },
    { titulo: 'Limpieza de Oídos', desc: 'Higiene profunda del canal auditivo para prevenir otitis e infecciones.' },
    { titulo: 'Limpieza de Glándulas', desc: 'Vaciado y cuidado glandular especializado para el bienestar canino.' },
  ];

  return (
    <section id="peluqueria" className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12">
      <div className="bg-gradient-to-br from-slate-900 via-slate-800 to-slate-900 rounded-3xl p-8 sm:p-12 text-white shadow-2xl relative overflow-hidden border border-slate-700/50">
        {/* Background Accents */}
        <div className="absolute top-0 right-0 w-80 h-80 bg-red-600/20 rounded-full blur-3xl pointer-events-none" />
        <div className="absolute bottom-0 left-0 w-70 h-70 bg-cyan-500/20 rounded-full blur-3xl pointer-events-none" />

        <div className="relative z-10 grid grid-cols-1 lg:grid-cols-12 gap-8 items-center">
          {/* Header & Services List */}
          <div className="lg:col-span-8 space-y-6">
            <div className="inline-flex items-center gap-2 bg-red-600/30 text-red-300 border border-red-500/40 px-3.5 py-1.5 rounded-full text-xs font-extrabold uppercase tracking-wider">
              <Scissors className="w-3.5 h-3.5 text-cyan-400" />
              Peluquería Canina Sindy
            </div>

            <h2 className="text-3xl sm:text-4xl font-black tracking-tight leading-tight">
              Cuidado y estética profesional para <span className="text-cyan-400">todas las razas</span> y tamaños
            </h2>

            <p className="text-slate-300 text-sm sm:text-base leading-relaxed">
              En Sindy mimamos a tu mascota con productos de primera calidad como Dermapet. Brindamos baños higiénicos, tratamientos dermatológicos y cortes específicos para que tu perro se vea y se sienta increíble.
            </p>

            {/* Grid of 6 Services */}
            <div className="grid grid-cols-1 sm:grid-cols-2 gap-4 pt-2">
              {servicios.map((s, idx) => (
                <div key={idx} className="bg-white/5 backdrop-blur-sm p-4 rounded-2xl border border-white/10 flex items-start gap-3 hover:bg-white/10 transition-colors">
                  <div className="w-8 h-8 rounded-full bg-red-600/30 text-cyan-400 flex items-center justify-center shrink-0 mt-0.5 border border-red-500/30">
                    <Check className="w-4 h-4" />
                  </div>
                  <div>
                    <h4 className="font-bold text-sm text-white">{s.titulo}</h4>
                    <p className="text-xs text-slate-400 mt-0.5">{s.desc}</p>
                  </div>
                </div>
              ))}
            </div>
          </div>

          {/* Booking Card */}
          <div className="lg:col-span-4 bg-white/10 backdrop-blur-md rounded-2xl p-6 border border-white/20 text-center space-y-4">
            <div className="w-16 h-16 bg-red-600 text-white rounded-2xl flex items-center justify-center mx-auto shadow-lg">
              <Scissors className="w-8 h-8" />
            </div>

            <div>
              <h3 className="text-lg font-black text-white">Reservá tu Turno</h3>
              <p className="text-xs text-slate-300 mt-1">
                Atención personalizada de lunes a sábado en nuestro local.
              </p>
            </div>

            <div className="space-y-2 py-2 text-xs text-slate-200">
              <div className="flex items-center justify-center gap-1.5 bg-black/20 py-2 px-3 rounded-xl">
                <MapPin className="w-4 h-4 text-cyan-400" />
                <span>Av. Jara 1635, Mar del Plata</span>
              </div>
              <div className="flex items-center justify-center gap-1.5 bg-black/20 py-2 px-3 rounded-xl font-bold">
                <Phone className="w-4 h-4 text-emerald-400" />
                <span>WhatsApp: 2236362266</span>
              </div>
            </div>

            <a
              href="https://wa.me/5492236362266?text=Hola!%20Quiero%20pedir%20un%20turno%20para%20peluquer%C3%ADa%20canina%20en%20Sindy."
              target="_blank"
              rel="noopener noreferrer"
              className="w-full flex items-center justify-center gap-2 py-3 bg-emerald-500 hover:bg-emerald-600 text-white font-bold text-xs rounded-xl shadow-lg transition-transform active:scale-95"
            >
              <Calendar className="w-4 h-4" />
              Solicitar Turno por WhatsApp
            </a>
          </div>
        </div>
      </div>
    </section>
  );
};
