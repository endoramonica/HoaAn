import React from 'react';
import { useAuth } from '../contexts/AuthContext';
import { PermissionWrapperProps } from '../types';

export const PermissionWrapper: React.FC<PermissionWrapperProps> = ({ 
  children, 
  permission, 
  fallback = null 
}) => {
  const { hasPermission } = useAuth();

  if (!hasPermission(permission)) {
    return <>{fallback}</>;
  }

  return <>{children}</>;
};