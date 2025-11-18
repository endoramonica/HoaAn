import React from 'react';
import { Link, useNavigate, useLocation } from 'react-router-dom';
import { Button } from '../components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '../components/ui/card';
import { ShieldX, ArrowLeft, Home, RefreshCw } from 'lucide-react';
import { useAuth } from '../contexts/AuthContext';
import { useI18n } from '../contexts/I18nContext';

export const UnauthorizedPage: React.FC = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const { user } = useAuth();
  const { t } = useI18n();

  const handleGoBack = () => {
    if (window.history.length > 1) {
      navigate(-1);
    } else {
      // Redirect based on role
      const redirectTo = user?.role === 'admin' ? '/admin' : '/dashboard';
      navigate(redirectTo);
    }
  };

  const handleRefresh = () => {
    window.location.reload();
  };

  const currentPath = location.pathname;
  const isAdminPath = currentPath.includes('/staff') || currentPath.includes('/analytics');

  return (
    <div className="min-h-screen flex items-center justify-center bg-background p-4">
      <Card className="w-full max-w-lg">
        <CardHeader className="text-center">
          <div className="mx-auto w-16 h-16 bg-destructive/10 rounded-full flex items-center justify-center mb-4">
            <ShieldX className="w-8 h-8 text-destructive" />
          </div>
          <CardTitle className="text-2xl mb-2">Access Denied</CardTitle>
          <p className="text-sm text-muted-foreground">
            You don't have permission to access this page
          </p>
        </CardHeader>
        <CardContent className="space-y-6">
          <div className="bg-muted/50 p-4 rounded-lg space-y-2">
            <p className="font-medium">Current user: {user?.name || 'Unknown'}</p>
            <p className="text-sm text-muted-foreground">Role: {user?.role || 'Unknown'}</p>
            <p className="text-sm text-muted-foreground">Requested page: {currentPath}</p>
          </div>

          {isAdminPath && user?.role === 'staff' && (
            <div className="bg-yellow-50 dark:bg-yellow-900/20 p-4 rounded-lg border border-yellow-200 dark:border-yellow-800">
              <p className="text-sm text-yellow-800 dark:text-yellow-200">
                This page requires manager privileges. Contact your manager for access.
              </p>
            </div>
          )}

          <div className="space-y-3">
            <Button onClick={handleGoBack} className="w-full" variant="default">
              <ArrowLeft className="w-4 h-4 mr-2" />
              Go Back
            </Button>

            <Button 
              asChild 
              className="w-full" 
              variant="outline"
              onClick={() => navigate(user?.role === 'admin' ? '/admin' : '/dashboard')}
            >
              <Link to={user?.role === 'admin' ? '/admin' : '/dashboard'}>
                <Home className="w-4 h-4 mr-2" />
                {user?.role === 'admin' ? 'Admin Dashboard' : 'Dashboard'}
              </Link>
            </Button>

            <Button onClick={handleRefresh} className="w-full" variant="outline">
              <RefreshCw className="w-4 h-4 mr-2" />
              Refresh Page
            </Button>
          </div>

          <div className="text-center pt-4 border-t">
            <p className="text-xs text-muted-foreground">
              If you believe this is an error, please contact your system administrator.
            </p>
          </div>
        </CardContent>
      </Card>
    </div>
  );
};