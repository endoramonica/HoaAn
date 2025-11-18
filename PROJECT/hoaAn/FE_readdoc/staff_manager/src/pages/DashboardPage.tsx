import React, { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import { StaffDashboard } from '../components/dashboards/StaffDashboard';
import { ManagerDashboard } from '../components/dashboards/ManagerDashboard';
import { ROLES } from '../utils/constants';

export const DashboardPage: React.FC = () => {
  const { user } = useAuth();
  const navigate = useNavigate();

  // Redirect admin users to admin dashboard
  useEffect(() => {
    if (user?.role === ROLES.ADMIN) {
      navigate('/admin', { replace: true });
    }
  }, [user, navigate]);

  // Render appropriate dashboard based on user role
  switch (user?.role) {
    case ROLES.STAFF:
      return <StaffDashboard />;
    case ROLES.MANAGER:
      return <ManagerDashboard />;
    case ROLES.ADMIN:
      return null; // Will redirect
    default:
      return (
        <div className="flex items-center justify-center h-64">
          <div className="text-center">
            <h2>Access Denied</h2>
            <p className="text-muted-foreground mt-2">
              You don't have permission to access this dashboard.
            </p>
          </div>
        </div>
      );
  }
};