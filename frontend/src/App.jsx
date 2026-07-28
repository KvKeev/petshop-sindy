import React, { useState } from 'react';
import { AuthProvider } from './context/AuthContext';
import { CartProvider } from './context/CartContext';
import { Navbar } from './components/Navbar';
import { HeroBanner } from './components/HeroBanner';
import { ProductCatalog } from './components/ProductCatalog';
import { ProductModal } from './components/ProductModal';
import { PeluqueriaSection } from './components/PeluqueriaSection';
import { CartDrawer } from './components/CartDrawer';
import { AuthModal } from './components/AuthModal';
import { MascotasModal } from './components/MascotasModal';
import { Footer } from './components/Footer';

function MainApp() {
  const [selectedCategory, setSelectedCategory] = useState(null);
  const [searchQuery, setSearchQuery] = useState('');
  const [quickViewProduct, setQuickViewProduct] = useState(null);

  const scrollToSection = (id) => {
    const el = document.getElementById(id);
    if (el) {
      el.scrollIntoView({ behavior: 'smooth' });
    }
  };

  return (
    <div className="min-h-screen bg-slate-50 flex flex-col font-sans text-slate-800 antialiased selection:bg-red-500 selection:text-white">
      {/* Top Navigation */}
      <Navbar
        selectedCategory={selectedCategory}
        onSelectCategory={setSelectedCategory}
        searchQuery={searchQuery}
        setSearchQuery={setSearchQuery}
        onOpenPeluqueria={() => scrollToSection('peluqueria')}
      />

      {/* Main Content */}
      <main className="flex-1 space-y-6">
        <HeroBanner
          onShopClick={() => scrollToSection('catalogo')}
          onPeluqueriaClick={() => scrollToSection('peluqueria')}
        />

        <ProductCatalog
          selectedCategory={selectedCategory}
          onSelectCategory={setSelectedCategory}
          searchQuery={searchQuery}
          onQuickView={(prod) => setQuickViewProduct(prod)}
        />

        <PeluqueriaSection />
      </main>

      {/* Modals & Overlays */}
      <CartDrawer />
      <AuthModal />
      <MascotasModal />

      {quickViewProduct && (
        <ProductModal
          producto={quickViewProduct}
          onClose={() => setQuickViewProduct(null)}
        />
      )}

      {/* Footer */}
      <Footer />
    </div>
  );
}

export default function App() {
  return (
    <AuthProvider>
      <CartProvider>
        <MainApp />
      </CartProvider>
    </AuthProvider>
  );
}
