import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import { useI18n } from '../contexts/I18nContext';
import { useTheme } from '../contexts/ThemeContext';
import { Button } from '../components/ui/button';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '../components/ui/card';
import { Input } from '../components/ui/input';
import { Label } from '../components/ui/label';
import { Avatar, AvatarFallback, AvatarImage } from '../components/ui/avatar';
import { Separator } from '../components/ui/separator';
import { Switch } from '../components/ui/switch';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '../components/ui/select';
import { Badge } from '../components/ui/badge';
import { Breadcrumb } from '../components/ui/breadcrumb';
import { 
  User, Mail, Shield, Store, LogOut, Moon, Sun, 
  Globe, Edit2, Save, X, Camera 
} from 'lucide-react';
import { toast } from 'sonner';

export const ProfilePage: React.FC = () => {
  const { user, logout } = useAuth();
  const { t, language, setLanguage } = useI18n();
  const { theme, setTheme } = useTheme();
  const navigate = useNavigate();
  
  const [isEditing, setIsEditing] = useState(false);
  const [formData, setFormData] = useState({
    name: user?.name || '',
    email: user?.email || '',
  });

  const handleLogout = () => {
    logout();
    toast.success(t('auth.logoutSuccess'));
    navigate('/login');
  };

  const handleSave = () => {
    // TODO: API call to update profile
    toast.success('Profile updated successfully');
    setIsEditing(false);
  };

  const handleCancel = () => {
    setFormData({
      name: user?.name || '',
      email: user?.email || '',
    });
    setIsEditing(false);
  };

  const getRoleColor = (role: string) => {
    switch (role) {
      case 'admin': return 'bg-purple-500';
      case 'manager': return 'bg-blue-500';
      case 'staff': return 'bg-green-500';
      default: return 'bg-gray-500';
    }
  };

  const breadcrumbItems = [
    { label: t('nav.dashboard'), href: user?.role === 'admin' ? '/admin' : '/dashboard' },
    { label: t('nav.profile'), href: '/profile' },
  ];

  return (
    <div className="space-y-6">
      <Breadcrumb items={breadcrumbItems} />
      
      <div className="flex items-center justify-between">
        <div>
          <h1 className="mb-2">{t('profile.title')}</h1>
          <p className="text-muted-foreground">{t('profile.description')}</p>
        </div>
        <Button variant="destructive" onClick={handleLogout}>
          <LogOut className="mr-2 h-4 w-4" />
          {t('auth.logout')}
        </Button>
      </div>

      <div className="grid gap-6 md:grid-cols-3">
        {/* Profile Information */}
        <div className="md:col-span-2 space-y-6">
          <Card>
            <CardHeader>
              <div className="flex items-center justify-between">
                <div>
                  <CardTitle>{t('profile.information')}</CardTitle>
                  <CardDescription>{t('profile.informationDesc')}</CardDescription>
                </div>
                {!isEditing ? (
                  <Button variant="outline" size="sm" onClick={() => setIsEditing(true)}>
                    <Edit2 className="mr-2 h-4 w-4" />
                    {t('common.edit')}
                  </Button>
                ) : (
                  <div className="flex gap-2">
                    <Button variant="outline" size="sm" onClick={handleCancel}>
                      <X className="mr-2 h-4 w-4" />
                      {t('common.cancel')}
                    </Button>
                    <Button size="sm" onClick={handleSave}>
                      <Save className="mr-2 h-4 w-4" />
                      {t('common.save')}
                    </Button>
                  </div>
                )}
              </div>
            </CardHeader>
            <CardContent className="space-y-6">
              <div className="flex items-center gap-6">
                <div className="relative">
                  <Avatar className="h-24 w-24">
                    <AvatarImage src={user?.avatar} alt={user?.name} />
                    <AvatarFallback className="text-2xl">
                      {user?.name.charAt(0).toUpperCase()}
                    </AvatarFallback>
                  </Avatar>
                  {isEditing && (
                    <Button 
                      size="icon" 
                      variant="secondary" 
                      className="absolute bottom-0 right-0 h-8 w-8 rounded-full"
                    >
                      <Camera className="h-4 w-4" />
                    </Button>
                  )}
                </div>
                <div className="flex-1">
                  <h3>{user?.name}</h3>
                  <p className="text-sm text-muted-foreground">{user?.email}</p>
                  <div className="flex gap-2 mt-2">
                    <Badge className={getRoleColor(user?.role || '')}>
                      {user?.role}
                    </Badge>
                    <Badge variant="outline">
                      <Store className="mr-1 h-3 w-3" />
                      {user?.storeId}
                    </Badge>
                  </div>
                </div>
              </div>

              <Separator />

              <div className="space-y-4">
                <div className="grid gap-2">
                  <Label htmlFor="name">
                    <User className="inline mr-2 h-4 w-4" />
                    {t('profile.name')}
                  </Label>
                  <Input
                    id="name"
                    value={formData.name}
                    onChange={(e) => setFormData({ ...formData, name: e.target.value })}
                    disabled={!isEditing}
                  />
                </div>

                <div className="grid gap-2">
                  <Label htmlFor="email">
                    <Mail className="inline mr-2 h-4 w-4" />
                    {t('profile.email')}
                  </Label>
                  <Input
                    id="email"
                    type="email"
                    value={formData.email}
                    onChange={(e) => setFormData({ ...formData, email: e.target.value })}
                    disabled={!isEditing}
                  />
                </div>

                <div className="grid gap-2">
                  <Label>
                    <Shield className="inline mr-2 h-4 w-4" />
                    {t('profile.role')}
                  </Label>
                  <Input value={user?.role} disabled />
                </div>

                <div className="grid gap-2">
                  <Label>
                    <Store className="inline mr-2 h-4 w-4" />
                    {t('profile.store')}
                  </Label>
                  <Input value={user?.storeId} disabled />
                </div>
              </div>
            </CardContent>
          </Card>

          {/* Security */}
          <Card>
            <CardHeader>
              <CardTitle>{t('profile.security')}</CardTitle>
              <CardDescription>{t('profile.securityDesc')}</CardDescription>
            </CardHeader>
            <CardContent className="space-y-4">
              <div className="flex items-center justify-between">
                <div>
                  <p className="font-medium">{t('profile.password')}</p>
                  <p className="text-sm text-muted-foreground">
                    {t('profile.passwordDesc')}
                  </p>
                </div>
                <Button variant="outline">
                  {t('profile.changePassword')}
                </Button>
              </div>
            </CardContent>
          </Card>
        </div>

        {/* Preferences */}
        <div className="space-y-6">
          <Card>
            <CardHeader>
              <CardTitle>{t('profile.preferences')}</CardTitle>
              <CardDescription>{t('profile.preferencesDesc')}</CardDescription>
            </CardHeader>
            <CardContent className="space-y-6">
              {/* Theme Toggle */}
              <div className="space-y-2">
                <Label className="flex items-center gap-2">
                  {theme === 'dark' ? (
                    <Moon className="h-4 w-4" />
                  ) : (
                    <Sun className="h-4 w-4" />
                  )}
                  {t('profile.theme')}
                </Label>
                <div className="flex items-center gap-2">
                  <span className="text-sm">{t('profile.light')}</span>
                  <Switch
                    checked={theme === 'dark'}
                    onCheckedChange={(checked) => setTheme(checked ? 'dark' : 'light')}
                  />
                  <span className="text-sm">{t('profile.dark')}</span>
                </div>
              </div>

              <Separator />

              {/* Language Selection */}
              <div className="space-y-2">
                <Label className="flex items-center gap-2">
                  <Globe className="h-4 w-4" />
                  {t('profile.language')}
                </Label>
                <Select value={language} onValueChange={setLanguage}>
                  <SelectTrigger>
                    <SelectValue />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="en">English</SelectItem>
                    <SelectItem value="vi">Tiếng Việt</SelectItem>
                  </SelectContent>
                </Select>
              </div>
            </CardContent>
          </Card>

          {/* Account Info */}
          <Card>
            <CardHeader>
              <CardTitle>{t('profile.accountInfo')}</CardTitle>
            </CardHeader>
            <CardContent className="space-y-4 text-sm">
              <div>
                <p className="text-muted-foreground">{t('profile.accountCreated')}</p>
                <p className="font-medium">
                  {user?.createdAt ? new Date(user.createdAt).toLocaleDateString() : '-'}
                </p>
              </div>
              <div>
                <p className="text-muted-foreground">{t('profile.lastLogin')}</p>
                <p className="font-medium">
                  {user?.lastLogin ? new Date(user.lastLogin).toLocaleDateString() : '-'}
                </p>
              </div>
              <div>
                <p className="text-muted-foreground">{t('profile.status')}</p>
                <Badge variant={user?.isActive ? 'default' : 'secondary'}>
                  {user?.isActive ? t('profile.active') : t('profile.inactive')}
                </Badge>
              </div>
            </CardContent>
          </Card>
        </div>
      </div>
    </div>
  );
};
