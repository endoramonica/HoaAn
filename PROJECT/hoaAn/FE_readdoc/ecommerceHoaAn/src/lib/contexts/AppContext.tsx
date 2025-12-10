import { createContext, useContext, useState, type ReactNode } from 'react';
import {type  Prayer } from '../../components/spiritual/PrayerForm';

interface AppContextType {
  // UI State
  showSpiritualChat: boolean;
  setShowSpiritualChat: (show: boolean) => void;
  showSupportChat: boolean;
  setShowSupportChat: (show: boolean) => void;
  
  // Prayer State
  prayers: Prayer[];
  addPrayer: (prayer: Prayer) => void;

  // Service Booking State
  selectedServiceProductId: string | null;
  setSelectedServiceProductId: (productId: string | null) => void;
}

const AppContext = createContext<AppContextType | undefined>(undefined);

export function AppProvider({ children }: { children: ReactNode }) {
  const [showSpiritualChat, setShowSpiritualChat] = useState(false);
  const [showSupportChat, setShowSupportChat] = useState(false);
  const [prayers, setPrayers] = useState<Prayer[]>([]);
  const [selectedServiceProductId, setSelectedServiceProductId] = useState<string | null>(null);

  const addPrayer = (prayer: Prayer) => {
    setPrayers(prev => [...prev, prayer]);
  };

  return (
    <AppContext.Provider
      value={{
        showSpiritualChat,
        setShowSpiritualChat,
        showSupportChat,
        setShowSupportChat,
        prayers,
        addPrayer,
        selectedServiceProductId,
        setSelectedServiceProductId,
      }}
    >
      {children}
    </AppContext.Provider>
  );
}

export function useApp() {
  const context = useContext(AppContext);
  if (context === undefined) {
    throw new Error('useApp must be used within an AppProvider');
  }
  return context;
}
