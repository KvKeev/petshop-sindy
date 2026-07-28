import React, { createContext, useContext, useState, useEffect } from 'react';
import { api } from '../services/api';

const AuthContext = createContext();

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const [token, setToken] = useState(localStorage.getItem('sindy_jwt_token') || null);
  const [isAuthModalOpen, setIsAuthModalOpen] = useState(false);
  const [isMascotasModalOpen, setIsMascotasModalOpen] = useState(false);

  useEffect(() => {
    const savedUser = localStorage.getItem('sindy_user');
    if (savedUser && token) {
      try {
        setUser(JSON.parse(savedUser));
      } catch (e) {
        localStorage.removeItem('sindy_user');
        localStorage.removeItem('sindy_jwt_token');
      }
    }
  }, [token]);

  const loginUser = async (email, password) => {
    const data = await api.login(email, password);
    const jwtToken = data.token;
    const userData = data.cliente || { email };
    
    setToken(jwtToken);
    setUser(userData);
    localStorage.setItem('sindy_jwt_token', jwtToken);
    localStorage.setItem('sindy_user', JSON.stringify(userData));
    setIsAuthModalOpen(false);
    return userData;
  };

  const registerUser = async (nombre, email, password) => {
    const data = await api.registro(nombre, email, password);
    const jwtToken = data.token;
    const userData = data.cliente || { nombre, email };

    setToken(jwtToken);
    setUser(userData);
    localStorage.setItem('sindy_jwt_token', jwtToken);
    localStorage.setItem('sindy_user', JSON.stringify(userData));
    setIsAuthModalOpen(false);
    return userData;
  };

  const logoutUser = () => {
    setToken(null);
    setUser(null);
    localStorage.removeItem('sindy_jwt_token');
    localStorage.removeItem('sindy_user');
  };

  return (
    <AuthContext.Provider value={{
      user,
      token,
      isAuthenticated: !!token,
      loginUser,
      registerUser,
      logoutUser,
      isAuthModalOpen,
      setIsAuthModalOpen,
      isMascotasModalOpen,
      setIsMascotasModalOpen
    }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => useContext(AuthContext);
