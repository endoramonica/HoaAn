import React from 'react';
import { Navigate, useLocation } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';

interface ProtectedRouteProps {
  children: React.ReactNode;
  allowedRoles?: string[];
  allowedStores?: string[];
  requiredPermission?: string;
  featureFlag?: string;
}

export const ProtectedRoute: React.FC<ProtectedRouteProps> = ({ 
  children, 
  allowedRoles,
  allowedStores,
  requiredPermission,
  featureFlag
}) => {
  const { user, currentStore, isLoading, hasPermission } = useAuth();
  const location = useLocation();

  if (isLoading) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-primary"></div>
      </div>
    );
  }

  if (!user) {
    return <Navigate to="/login" state={{ from: location }} replace />;
  }

  // Check role permissions
  if (allowedRoles && !allowedRoles.includes(user.role)) {
    return <Navigate to="/unauthorized" replace />;
  }

  // Check specific permission
  if (requiredPermission && !hasPermission(requiredPermission)) {
    return <Navigate to="/unauthorized" replace />;
  }

  // Check store permissions if specified
  if (allowedStores && currentStore && !allowedStores.includes(currentStore.id)) {
    return <Navigate to="/unauthorized" replace />;
  }

  // TODO: Check feature flags when implemented
  // if (featureFlag && !isFeatureEnabled(featureFlag)) {
  //   return <Navigate to="/unauthorized" replace />;
  // }

  return <>{children}</>;
};