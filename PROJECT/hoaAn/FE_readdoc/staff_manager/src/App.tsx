import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { Toaster } from 'sonner';

// Context Providers
import { AuthProvider } from './contexts/AuthContext';
import { ThemeProvider } from './contexts/ThemeContext';
import { I18nProvider } from './contexts/I18nContext';

// Layout Components
import { Layout } from './components/layout/Layout';
import { ProtectedRoute } from './components/ProtectedRoute';
import { RoleBasedRedirect } from './components/RoleBasedRedirect';

// Pages
import { LoginPage } from './pages/LoginPage';
import { DashboardPage } from './pages/DashboardPage';
import { POSPage } from './pages/POSPage';
import { CardPaymentPage } from './pages/CardPaymentPage';
import { OrdersPage } from './pages/OrdersPage';
import { InventoryPage } from './pages/InventoryPage';
import { StaffPage } from './pages/StaffPage';
import { TasksPage } from './pages/TasksPage';
import { AnalyticsPage } from './pages/AnalyticsPage';
import { CRMPage } from './pages/CRMPage';
import { LRMPage } from './pages/LRMPage';
import { HRMPage } from './pages/HRMPage';
import { UnauthorizedPage } from './pages/UnauthorizedPage';
import { ProfilePage } from './pages/ProfilePage';
import { NotificationsPage } from './pages/NotificationsPage';

// Admin Pages
import { AdminDashboardPage } from './pages/AdminDashboardPage';
import { UserManagementPage } from './pages/admin/UserManagementPage';
import { SystemSettingsPage } from './pages/admin/SystemSettingsPage';
import { AuditLogsPage } from './pages/admin/AuditLogsPage';
import { SystemMonitorPage } from './pages/admin/SystemMonitorPage';

export default function App() {
  return (
    <div className="min-h-screen bg-background">
      <ThemeProvider>
        <I18nProvider>
          <AuthProvider>
            <Router>
              <Routes>
                {/* Public Routes */}
                <Route path="/login" element={<LoginPage />} />
                <Route path="/unauthorized" element={<UnauthorizedPage />} />
                
                {/* Card Payment Route (outside protected layout) */}
                <Route 
                  path="/card-payment" 
                  element={
                    <ProtectedRoute allowedRoles={['staff', 'manager']}>
                      <CardPaymentPage />
                    </ProtectedRoute>
                  } 
                />
                
                {/* Protected Routes */}
                <Route path="/" element={
                  <ProtectedRoute>
                    <Layout />
                  </ProtectedRoute>
                }>
                  {/* Dashboard - Default route */}
                  <Route index element={<RoleBasedRedirect />} />
                  
                  {/* Dashboard */}
                  <Route 
                    path="dashboard" 
                    element={
                      <ProtectedRoute allowedRoles={['staff', 'manager']}>
                        <DashboardPage />
                      </ProtectedRoute>
                    } 
                  />
                  
                  {/* POS */}
                  <Route 
                    path="pos" 
                    element={
                      <ProtectedRoute 
                        allowedRoles={['staff', 'manager']}
                        requiredPermission="pos.access"
                      >
                        <POSPage />
                      </ProtectedRoute>
                    } 
                  />
                  
                  {/* Orders */}
                  <Route 
                    path="orders" 
                    element={
                      <ProtectedRoute 
                        allowedRoles={['staff', 'manager']}
                        requiredPermission="orders.view_own"
                      >
                        <OrdersPage />
                      </ProtectedRoute>
                    } 
                  />
                  
                  {/* Inventory */}
                  <Route 
                    path="inventory" 
                    element={
                      <ProtectedRoute 
                        allowedRoles={['staff', 'manager']}
                        requiredPermission="inventory.view"
                      >
                        <InventoryPage />
                      </ProtectedRoute>
                    } 
                  />
                  
                  {/* Tasks */}
                  <Route 
                    path="tasks" 
                    element={
                      <ProtectedRoute 
                        allowedRoles={['staff', 'manager']}
                        requiredPermission="tasks.view_own"
                      >
                        <TasksPage />
                      </ProtectedRoute>
                    } 
                  />
                  
                  {/* Staff Management - Manager only */}
                  <Route 
                    path="staff" 
                    element={
                      <ProtectedRoute 
                        allowedRoles={['manager']}
                        requiredPermission="users.view_all"
                      >
                        <StaffPage />
                      </ProtectedRoute>
                    } 
                  />
                  
                  {/* Analytics - Manager only */}
                  <Route 
                    path="analytics" 
                    element={
                      <ProtectedRoute 
                        allowedRoles={['manager']}
                        requiredPermission="analytics.view_store_dashboard"
                      >
                        <AnalyticsPage />
                      </ProtectedRoute>
                    } 
                  />
                  
                  {/* CRM */}
                  <Route 
                    path="crm" 
                    element={
                      <ProtectedRoute 
                        allowedRoles={['staff', 'manager']}
                        requiredPermission="crm.view"
                      >
                        <CRMPage />
                      </ProtectedRoute>
                    } 
                  />
                  
                  {/* LRM */}
                  <Route 
                    path="lrm" 
                    element={
                      <ProtectedRoute 
                        allowedRoles={['staff', 'manager']}
                        requiredPermission="lrm.view"
                      >
                        <LRMPage />
                      </ProtectedRoute>
                    } 
                  />
                  
                  {/* HRM */}
                  <Route 
                    path="hrm" 
                    element={
                      <ProtectedRoute 
                        allowedRoles={['staff', 'manager']}
                        requiredPermission="hrm.view_own"
                      >
                        <HRMPage />
                      </ProtectedRoute>
                    } 
                  />
                  
                  {/* Profile Page - All authenticated users */}
                  <Route path="profile" element={<ProfilePage />} />
                  
                  {/* Notifications Page - All authenticated users */}
                  <Route path="notifications" element={<NotificationsPage />} />
                  
                  {/* Admin Routes */}
                  <Route 
                    path="admin" 
                    element={
                      <ProtectedRoute 
                        allowedRoles={['admin']}
                        requiredPermission="admin.super_admin"
                      >
                        <AdminDashboardPage />
                      </ProtectedRoute>
                    } 
                  />
                  
                  <Route 
                    path="admin/users" 
                    element={
                      <ProtectedRoute 
                        allowedRoles={['admin']}
                        requiredPermission="admin.manage_users"
                      >
                        <UserManagementPage />
                      </ProtectedRoute>
                    } 
                  />
                  
                  <Route 
                    path="admin/settings" 
                    element={
                      <ProtectedRoute 
                        allowedRoles={['admin']}
                        requiredPermission="admin.system_configuration"
                      >
                        <SystemSettingsPage />
                      </ProtectedRoute>
                    } 
                  />
                  
                  <Route 
                    path="admin/audit-logs" 
                    element={
                      <ProtectedRoute 
                        allowedRoles={['admin']}
                        requiredPermission="admin.super_admin"
                      >
                        <AuditLogsPage />
                      </ProtectedRoute>
                    } 
                  />
                  
                  <Route 
                    path="admin/system-monitor" 
                    element={
                      <ProtectedRoute 
                        allowedRoles={['admin']}
                        requiredPermission="admin.system_monitor"
                      >
                        <SystemMonitorPage />
                      </ProtectedRoute>
                    } 
                  />
                </Route>
                
                {/* Catch all route */}
                <Route path="*" element={<RoleBasedRedirect />} />
              </Routes>
              
              {/* Global Toast Notifications */}
              <Toaster 
                position="top-right"
                toastOptions={{
                  duration: 4000,
                  className: 'bg-background border-border',
                }}
              />
            </Router>
          </AuthProvider>
        </I18nProvider>
      </ThemeProvider>
    </div>
  );
}