import React, { useState } from 'react';
import { Navigate, useLocation } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import { useI18n } from '../contexts/I18nContext';
import { Button } from '../components/ui/button';
import { Input } from '../components/ui/input';
import { Label } from '../components/ui/label';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '../components/ui/card';
import { Alert, AlertDescription } from '../components/ui/alert';
import { Loader2 } from 'lucide-react';

export const LoginPage: React.FC = () => {
  const { login, user, isLoading } = useAuth();
  const { t } = useI18n();
  const location = useLocation();
  
  const [email, setEmail] = useState('staff@store.com');
  const [password, setPassword] = useState('password');
  const [error, setError] = useState('');
  const [loginLoading, setLoginLoading] = useState(false);

  if (user) {
    // Redirect based on role after login
    const redirectTo = user.role === 'admin' ? '/admin' : '/dashboard';
    return <Navigate to={redirectTo} replace />;
  }

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setLoginLoading(true);

    try {
      const success = await login(email, password);
      if (!success) {
        setError(t('auth.invalidCredentials'));
      }
    } catch (err) {
      setError('An error occurred during login');
    } finally {
      setLoginLoading(false);
    }
  };

  if (isLoading) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-primary"></div>
      </div>
    );
  }

  return (
    <div className="min-h-screen flex items-center justify-center bg-background">
      <Card className="w-full max-w-md">
        <CardHeader className="space-y-1">
          <CardTitle className="text-2xl text-center">Store POS</CardTitle>
          <CardDescription className="text-center">
            {t('auth.login')}
          </CardDescription>
        </CardHeader>
        <CardContent>
          <form onSubmit={handleSubmit} className="space-y-4">
            {error && (
              <Alert variant="destructive">
                <AlertDescription>{error}</AlertDescription>
              </Alert>
            )}
            
            <div className="space-y-2">
              <Label htmlFor="email">{t('auth.email')}</Label>
              <Input
                id="email"
                type="email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                required
                disabled={loginLoading}
              />
            </div>
            
            <div className="space-y-2">
              <Label htmlFor="password">{t('auth.password')}</Label>
              <Input
                id="password"
                type="password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                required
                disabled={loginLoading}
              />
            </div>

            <Button type="submit" className="w-full" disabled={loginLoading}>
              {loginLoading && <Loader2 className="mr-2 h-4 w-4 animate-spin" />}
              {t('auth.login')}
            </Button>
          </form>

          <div className="mt-6 text-sm text-muted-foreground">
            <p><strong>Demo Credentials:</strong></p>
            <p>Staff: staff@store.com / password</p>
            <p>Manager: manager@store.com / password</p>
            <p>Admin: admin@store.com / password</p>
          </div>
        </CardContent>
      </Card>
    </div>
  );
};