import React, { createContext, useContext, useState, useEffect } from 'react';

const CartContext = createContext();

export const CartProvider = ({ children }) => {
  const [cartItems, setCartItems] = useState(() => {
    const savedCart = localStorage.getItem('sindy_cart');
    return savedCart ? JSON.parse(savedCart) : [];
  });
  const [isCartOpen, setIsCartOpen] = useState(false);

  useEffect(() => {
    localStorage.setItem('sindy_cart', JSON.stringify(cartItems));
  }, [cartItems]);

  const addToCart = (producto, variante, cantidad = 1) => {
    setCartItems(prev => {
      const existingIndex = prev.findIndex(item => item.varianteId === variante.id);
      if (existingIndex > -1) {
        const newCart = [...prev];
        newCart[existingIndex].cantidad += cantidad;
        return newCart;
      }
      return [
        ...prev,
        {
          productoId: producto.id,
          varianteId: variante.id,
          nombre: producto.nombre,
          atributo: variante.atributo || 'Opción',
          valor: variante.valor || 'Única',
          precio: variante.precio,
          imagenUrl: producto.imagenUrl,
          cantidad
        }
      ];
    });
    setIsCartOpen(true);
  };

  const removeFromCart = (varianteId) => {
    setCartItems(prev => prev.filter(item => item.varianteId !== varianteId));
  };

  const updateQuantity = (varianteId, newQuantity) => {
    if (newQuantity <= 0) {
      removeFromCart(varianteId);
      return;
    }
    setCartItems(prev => prev.map(item =>
      item.varianteId === varianteId ? { ...item, cantidad: newQuantity } : item
    ));
  };

  const clearCart = () => {
    setCartItems([]);
  };

  const totalCount = cartItems.reduce((acc, item) => acc + item.cantidad, 0);
  const totalPrice = cartItems.reduce((acc, item) => acc + (item.precio * item.cantidad), 0);

  return (
    <CartContext.Provider value={{
      cartItems,
      addToCart,
      removeFromCart,
      updateQuantity,
      clearCart,
      totalCount,
      totalPrice,
      isCartOpen,
      setIsCartOpen
    }}>
      {children}
    </CartContext.Provider>
  );
};

export const useCart = () => useContext(CartContext);
