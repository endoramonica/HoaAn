import React, { useState } from 'react';
import { Bell, Moon, Sun, Globe, Store, LogOut, User } from 'lucide-react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../contexts/AuthContext';
import { useTheme } from '../../contexts/ThemeContext';
import { useI18n } from '../../contexts/I18nContext';
import { Button } from '../ui/button';
import { Avatar, AvatarFallback, AvatarImage } from '../ui/avatar';
import { NotificationModal } from '../modals/NotificationModal';
import { ProfileModal } from '../modals/ProfileModal';
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from '../ui/dropdown-menu';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '../ui/select';
import { Badge } from '../ui/badge';
import { THEMES, LANGUAGES, ROLES } from '../../utils/constants';

export const TopBar: React.FC = () => {
  const { user, currentStore, stores, logout, switchStore, switchRole } = useAuth();
  const { theme, toggleTheme } = useTheme();
  const { language, setLanguage, t } = useI18n();
  const navigate = useNavigate();
  
  const [notificationModalOpen, setNotificationModalOpen] = useState(false);
  const [profileModalOpen, setProfileModalOpen] = useState(false);

  const handleStoreChange = (storeId: string) => {
    switchStore(storeId);
  };

  const handleLanguageChange = (newLanguage: string) => {
    setLanguage(newLanguage);
  };

  const handleRoleSwitch = (newRole: string) => {
    switchRole(newRole);
  };

  const handleNotificationClick = () => {
    setNotificationModalOpen(true);
  };

  const handleProfileClick = () => {
    setProfileModalOpen(true);
  };

  return (
    <div className="flex items-center justify-between px-6 py-4 bg-background border-b border-border">
      <div className="flex items-center space-x-4">
        {/* Store Selector */}
        <div className="flex items-center space-x-2">
          <Store className="w-4 h-4 text-muted-foreground" />
          <Select value={currentStore?.id} onValueChange={handleStoreChange}>
            <SelectTrigger className="w-48">
              <SelectValue />
            </SelectTrigger>
            <SelectContent>
              {stores.map((store) => (
                <SelectItem key={store.id} value={store.id}>
                  {store.name}
                </SelectItem>
              ))}
            </SelectContent>
          </Select>
        </div>

        {/* Current User Role Badge */}
        <Badge variant="secondary">
          {user?.role === ROLES.ADMIN ? 'Admin' : user?.role === ROLES.MANAGER ? 'Manager' : 'Staff'}
        </Badge>

        {/* Development: Role Switcher */}
        <Select value={user?.role} onValueChange={handleRoleSwitch}>
          <SelectTrigger className="w-32">
            <SelectValue />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value={ROLES.STAFF}>Staff</SelectItem>
            <SelectItem value={ROLES.MANAGER}>Manager</SelectItem>
            <SelectItem value={ROLES.ADMIN}>Admin</SelectItem>
          </SelectContent>
        </Select>
      </div>

      <div className="flex items-center space-x-2">
        {/* Language Selector */}
        <DropdownMenu>
          <DropdownMenuTrigger asChild>
            <Button variant="ghost" size="sm">
              <Globe className="w-4 h-4" />
            </Button>
          </DropdownMenuTrigger>
          <DropdownMenuContent>
            <DropdownMenuItem onClick={() => handleLanguageChange(LANGUAGES.EN)}>
              English
            </DropdownMenuItem>
            <DropdownMenuItem onClick={() => handleLanguageChange(LANGUAGES.VI)}>
              Tiếng Việt
            </DropdownMenuItem>
          </DropdownMenuContent>
        </DropdownMenu>

        {/* Theme Toggle */}
        <Button variant="ghost" size="sm" onClick={toggleTheme}>
          {theme === THEMES.DARK ? (
            <Sun className="w-4 h-4" />
          ) : (
            <Moon className="w-4 h-4" />
          )}
        </Button>

        {/* Notifications */}
        <Button variant="ghost" size="sm" onClick={handleNotificationClick}>
          <Bell className="w-4 h-4" />
        </Button>

        {/* User Menu */}
        <DropdownMenu>
          <DropdownMenuTrigger asChild>
            <Button 
              variant="ghost" 
              className="p-0"
              onClick={() => navigate('/profile')}
            >
              <Avatar className="w-8 h-8 cursor-pointer">
                <AvatarImage src={user?.avatar} />
                <AvatarFallback>
                  {user?.name?.split(' ').map(n => n[0]).join('').toUpperCase()}
                </AvatarFallback>
              </Avatar>
            </Button>
          </DropdownMenuTrigger>
          <DropdownMenuContent align="end">
            <DropdownMenuLabel>{user?.name}</DropdownMenuLabel>
            <DropdownMenuLabel className="text-xs text-muted-foreground font-normal">
              {user?.email}
            </DropdownMenuLabel>
            <DropdownMenuSeparator />
            <DropdownMenuItem onClick={() => navigate('/profile')}>
              <User className="w-4 h-4 mr-2" />
              {t('nav.profile')}
            </DropdownMenuItem>
            <DropdownMenuItem onClick={() => navigate('/notifications')}>
              <Bell className="w-4 h-4 mr-2" />
              {t('nav.notifications')}
            </DropdownMenuItem>
            <DropdownMenuSeparator />
            <DropdownMenuItem onClick={logout}>
              <LogOut className="w-4 h-4 mr-2" />
              {t('nav.logout')}
            </DropdownMenuItem>
          </DropdownMenuContent>
        </DropdownMenu>
      </div>
      
      {/* Modals */}
      <NotificationModal 
        open={notificationModalOpen} 
        onOpenChange={setNotificationModalOpen} 
      />
      <ProfileModal 
        open={profileModalOpen} 
        onOpenChange={setProfileModalOpen} 
      />
    </div>
  );
};