import React, { useState, useEffect } from 'react';
import { X, Dog, Cat, Plus, AlertCircle, Sparkles, CheckCircle2 } from 'lucide-react';
import { useAuth } from '../context/AuthContext';
import { api } from '../services/api';

export const MascotasModal = () => {
  const { isMascotasModalOpen, setIsMascotasModalOpen, isAuthenticated } = useAuth();
  const [mascotas, setMascotas] = useState([]);
  const [nombre, setNombre] = useState('');
  const [tipo, setTipo] = useState(0); // 0=Perro, 1=Gato, 2=Ave, 3=Otro
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [showAddForm, setShowAddForm] = useState(false);

  useEffect(() => {
    if (isMascotasModalOpen && isAuthenticated) {
      loadMascotas();
    }
  }, [isMascotasModalOpen, isAuthenticated]);

  const loadMascotas = async () => {
    setLoading(true);
    try {
      const data = await api.getMascotas();
      setMascotas(data || []);
    } catch (err) {
      console.error(err);
      setError('No se pudieron cargar las mascotas');
    } finally {
      setLoading(false);
    }
  };

  const handleCreatePet = async (e) => {
    e.preventDefault();
    if (!nombre.trim()) return;
    setError(null);
    setLoading(true);

    try {
      await api.crearMascota(nombre, tipo);
      setNombre('');
      setShowAddForm(false);
      await loadMascotas();
    } catch (err) {
      setError(err.message || 'Error al agregar mascota');
    } finally {
      setLoading(false);
    }
  };

  if (!isMascotasModalOpen) return null;

  const getTipoLabel = (t) => {
    switch (t) {
      case 0: return { label: 'Perro', icon: '🐶' };
      case 1: return { label: 'Gato', icon: '🐱' };
      case 2: return { label: 'Ave', icon: '🦜' };
      default: return { label: 'Mascota', icon: '🐾' };
    }
  };

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center p-4 bg-slate-900/60 backdrop-blur-sm animate-in fade-in duration-200">
      <div className="relative w-full max-w-lg bg-white rounded-3xl shadow-2xl overflow-hidden border border-slate-100 p-6 sm:p-8 max-h-[90vh] flex flex-col">
        
        {/* Close Button */}
        <button
          onClick={() => setIsMascotasModalOpen(false)}
          className="absolute top-4 right-4 p-2 text-slate-400 hover:text-slate-600 hover:bg-slate-100 rounded-full transition-colors"
        >
          <X className="w-5 h-5" />
        </button>

        {/* Modal Header */}
        <div className="flex items-center gap-3 mb-6">
          <div className="w-12 h-12 bg-red-600 text-white rounded-2xl flex items-center justify-center font-bold shadow-md">
            <Dog className="w-6 h-6" />
          </div>
          <div>
            <h2 className="text-xl font-black text-slate-900">Mis Mascotas</h2>
            <p className="text-xs text-slate-500 font-medium">
              Agregá a tus perros y gatos para personalizar su historial de productos.
            </p>
          </div>
        </div>

        {/* Mascotas List */}
        <div className="flex-1 overflow-y-auto space-y-3 pr-1">
          {loading && <p className="text-xs text-slate-400 text-center py-4">Cargando mascotas...</p>}

          {!loading && mascotas.length === 0 && !showAddForm && (
            <div className="py-12 text-center space-y-2 bg-slate-50 rounded-2xl border border-slate-100 p-4">
              <span className="text-3xl">🐾</span>
              <p className="text-xs font-bold text-slate-700">Aún no registraste ninguna mascota</p>
              <p className="text-[11px] text-slate-400">Agregá tus perros o gatos para seguimiento.</p>
            </div>
          )}

          {mascotas.map((m) => {
            const { label, icon } = getTipoLabel(m.tipo);
            return (
              <div key={m.id} className="flex items-center justify-between p-3.5 bg-slate-50 rounded-2xl border border-slate-100">
                <div className="flex items-center gap-3">
                  <span className="text-2xl">{icon}</span>
                  <div>
                    <h4 className="text-sm font-bold text-slate-800">{m.nombre}</h4>
                    <span className="text-[10px] text-red-600 font-bold uppercase tracking-wider bg-red-50 px-2 py-0.5 rounded-md">
                      {label}
                    </span>
                  </div>
                </div>
              </div>
            );
          })}
        </div>

        {/* Add Pet Form / Toggle */}
        <div className="pt-4 border-t border-slate-100 mt-4">
          {showAddForm ? (
            <form onSubmit={handleCreatePet} className="space-y-3 bg-slate-50 p-4 rounded-2xl border border-slate-200">
              <h4 className="text-xs font-bold text-slate-900">Agregar Nueva Mascota</h4>
              <div>
                <label className="text-[11px] font-bold text-slate-600 block mb-1">Nombre</label>
                <input
                  type="text"
                  required
                  value={nombre}
                  onChange={(e) => setNombre(e.target.value)}
                  placeholder="Ej. Firulais, Lola, Misha"
                  className="w-full px-3 py-2 bg-white border border-slate-200 rounded-xl text-xs focus:ring-2 focus:ring-red-500/20 focus:border-red-500 focus:outline-none"
                />
              </div>

              <div>
                <label className="text-[11px] font-bold text-slate-600 block mb-1">Tipo de Mascota</label>
                <div className="grid grid-cols-4 gap-1.5">
                  {[
                    { id: 0, label: 'Perro', icon: '🐶' },
                    { id: 1, label: 'Gato', icon: '🐱' },
                    { id: 2, label: 'Ave', icon: '🦜' },
                    { id: 3, label: 'Otro', icon: '🐾' }
                  ].map(t => (
                    <button
                      type="button"
                      key={t.id}
                      onClick={() => setTipo(t.id)}
                      className={`p-2 rounded-xl text-xs font-bold flex flex-col items-center border transition-all ${
                        tipo === t.id
                          ? 'border-red-500 bg-red-50 text-red-600'
                          : 'border-slate-200 bg-white text-slate-600 hover:bg-slate-100'
                      }`}
                    >
                      <span className="text-base">{t.icon}</span>
                      <span className="text-[10px] mt-0.5">{t.label}</span>
                    </button>
                  ))}
                </div>
              </div>

              <div className="flex gap-2 pt-1">
                <button
                  type="button"
                  onClick={() => setShowAddForm(false)}
                  className="flex-1 py-2 text-xs font-bold text-slate-600 bg-white hover:bg-slate-200 rounded-xl border border-slate-200"
                >
                  Cancelar
                </button>
                <button
                  type="submit"
                  disabled={loading}
                  className="flex-1 py-2 text-xs font-bold text-white bg-red-600 hover:bg-red-700 rounded-xl shadow-sm"
                >
                  Guardar Mascota
                </button>
              </div>
            </form>
          ) : (
            <button
              onClick={() => setShowAddForm(true)}
              className="w-full flex items-center justify-center gap-2 py-3 bg-red-600 hover:bg-red-700 text-white font-extrabold text-xs rounded-xl shadow-md transition-colors"
            >
              <Plus className="w-4 h-4" />
              Agregar Mascota
            </button>
          )}
        </div>

      </div>
    </div>
  );
};
