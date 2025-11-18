import React, { createContext, useContext, useState, useEffect } from 'react';
import { User, Store } from '../types';
import { ROLES, ROLE_PERMISSIONS } from '../utils/constants';
import { mockUsers, mockStores } from '../services/mockData';

interface AuthContextType {
  user: User | null;
  currentStore: Store | null;
  stores: Store[];
  login: (email: string, password: string) => Promise<boolean>;
  logout: () => void;
  switchStore: (storeId: string) => void;
  switchRole: (role: string) => void; // For testing purposes
  hasPermission: (permission: string) => boolean;
  isLoading: boolean;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [user, setUser] = useState<User | null>(null);
  const [currentStore, setCurrentStore] = useState<Store | null>(null);
  const [stores] = useState<Store[]>(mockStores);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    // Check for saved session
    const savedUser = localStorage.getItem('user');
    const savedStore = localStorage.getItem('currentStore');
    
    if (savedUser && savedStore) {
      setUser(JSON.parse(savedUser));
      setCurrentStore(JSON.parse(savedStore));
    }
    
    setIsLoading(false);
  }, []);

  const login = async (email: string, password: string): Promise<boolean> => {
    setIsLoading(true);
    
    // Mock authentication
    const foundUser = mockUsers.find(u => u.email === email);
    if (foundUser && password === 'password') {
      const userWithPermissions = {
        ...foundUser,
        permissions: ROLE_PERMISSIONS[foundUser.role as keyof typeof ROLE_PERMISSIONS] || []
      };
      
      const userStore = stores.find(s => s.id === foundUser.storeId);
      
      setUser(userWithPermissions);
      setCurrentStore(userStore || stores[0]);
      
      localStorage.setItem('user', JSON.stringify(userWithPermissions));
      localStorage.setItem('currentStore', JSON.stringify(userStore || stores[0]));
      
      setIsLoading(false);
      return true;
    }
    
    setIsLoading(false);
    return false;
  };

  const logout = () => {
    setUser(null);
    setCurrentStore(null);
    localStorage.removeItem('user');
    localStorage.removeItem('currentStore');
  };

  const switchStore = (storeId: string) => {
    const store = stores.find(s => s.id === storeId);
    if (store) {
      setCurrentStore(store);
      localStorage.setItem('currentStore', JSON.stringify(store));
    }
  };

  const switchRole = (role: string) => {
    if (user) {
      const updatedUser = {
        ...user,
        role,
        permissions: ROLE_PERMISSIONS[role as keyof typeof ROLE_PERMISSIONS] || []
      };
      setUser(updatedUser);
      localStorage.setItem('user', JSON.stringify(updatedUser));
    }
  };

  const hasPermission = (permission: string): boolean => {
    if (!user) return false;
    // Admin has ALL permissions automatically
    if (user.role === ROLES.ADMIN) return true;
    return user.permissions.includes(permission);
  };

  return (
    <AuthContext.Provider value={{
      user,
      currentStore,
      stores,
      login,
      logout,
      switchStore,
      switchRole,
      hasPermission,
      isLoading
    }}>
      {children}
    </AuthContext.Provider>
  );
};