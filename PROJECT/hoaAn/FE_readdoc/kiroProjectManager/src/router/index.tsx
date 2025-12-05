import { HashRouter, Routes, Route } from 'react-router-dom';
import { AuthProvider } from '../lib/contexts/AuthContext';
import { Layout } from '../layout/Layout';
import { ProtectedRoute } from '../layout/ProtectedRoute';
import { LoginPage } from '../pages/LoginPage';
import { DashboardPage } from '../pages/DashboardPage';
import { POSPage } from '../pages/POSPage';
import { ProductsPage } from '../pages/ProductsPage';
import { CustomersPage } from '../pages/CustomersPage';
import { CustomerDetailPage } from '../pages/CustomerDetailPage';
import { OrdersPage } from '../pages/OrdersPage';
import { EmployeesPage } from '../pages/EmployeesPage';
import { UsersPage } from '../pages/UsersPage';
import { UserDetailPage } from '../pages/UserDetailPage';
import { InventoryPage } from '../pages/InventoryPage';
import { MarketingPage } from '../pages/MarketingPage';
import { MarketingEditPage } from '../pages/MarketingEditPage';
import { NotificationsPage } from '../pages/NotificationsPage';
import { SettingsPage } from '../pages/SettingsPage';
import { ShiftsPage } from '../pages/ShiftsPage';
import { TasksPage } from '../pages/TasksPage';
import { SuppliersPage } from '../pages/SuppliersPage';
import { LeaveRequestsPage } from '../pages/LeaveRequestsPage';
import { StockTransfersPage } from '../pages/StockTransfersPage';
import { WorkSchedulesPage } from '../pages/WorkSchedulesPage';

export const AppRouter = () => {
    return (
        <HashRouter>
            <AuthProvider>
                    <Routes>
                <Route path="/login" element={<LoginPage />} />
                
                <Route
                    path="/"
                    element={
                        <ProtectedRoute>
                            <Layout>
                                <DashboardPage />
                            </Layout>
                        </ProtectedRoute>
                    }
                />
                
                <Route
                    path="/pos"
                    element={
                        <ProtectedRoute>
                            <Layout>
                                <POSPage />
                            </Layout>
                        </ProtectedRoute>
                    }
                />
                
                <Route
                    path="/products"
                    element={
                        <ProtectedRoute>
                            <Layout>
                                <ProductsPage />
                            </Layout>
                        </ProtectedRoute>
                    }
                />
                
                <Route
                    path="/customers"
                    element={
                        <ProtectedRoute>
                            <Layout>
                                <CustomersPage />
                            </Layout>
                        </ProtectedRoute>
                    }
                />
                
                <Route
                    path="/customers/:id"
                    element={
                        <ProtectedRoute>
                            <Layout>
                                <CustomerDetailPage />
                            </Layout>
                        </ProtectedRoute>
                    }
                />
                
                <Route
                    path="/orders"
                    element={
                        <ProtectedRoute>
                            <Layout>
                                <OrdersPage />
                            </Layout>
                        </ProtectedRoute>
                    }
                />
                
                <Route
                    path="/employees"
                    element={
                        <ProtectedRoute>
                            <Layout>
                                <EmployeesPage />
                            </Layout>
                        </ProtectedRoute>
                    }
                />
                
                <Route
                    path="/users"
                    element={
                        <ProtectedRoute>
                            <Layout>
                                <UsersPage />
                            </Layout>
                        </ProtectedRoute>
                    }
                />
                
                <Route
                    path="/users/:id"
                    element={
                        <ProtectedRoute>
                            <Layout>
                                <UserDetailPage />
                            </Layout>
                        </ProtectedRoute>
                    }
                />
                
                <Route
                    path="/inventory"
                    element={
                        <ProtectedRoute>
                            <Layout>
                                <InventoryPage />
                            </Layout>
                        </ProtectedRoute>
                    }
                />
                
                <Route
                    path="/marketing"
                    element={
                        <ProtectedRoute>
                            <Layout>
                                <MarketingPage />
                            </Layout>
                        </ProtectedRoute>
                    }
                />
                
                <Route
                    path="/marketing/edit/:id"
                    element={
                        <ProtectedRoute>
                            <Layout>
                                <MarketingEditPage />
                            </Layout>
                        </ProtectedRoute>
                    }
                />
                
                <Route
                    path="/notifications"
                    element={
                        <ProtectedRoute>
                            <Layout>
                                <NotificationsPage />
                            </Layout>
                        </ProtectedRoute>
                    }
                />
                
                <Route
                    path="/settings"
                    element={
                        <ProtectedRoute>
                            <Layout>
                                <SettingsPage />
                            </Layout>
                        </ProtectedRoute>
                    }
                />
                
                <Route
                    path="/shifts"
                    element={
                        <ProtectedRoute>
                            <Layout>
                                <ShiftsPage />
                            </Layout>
                        </ProtectedRoute>
                    }
                />
                
                <Route
                    path="/tasks"
                    element={
                        <ProtectedRoute>
                            <Layout>
                                <TasksPage />
                            </Layout>
                        </ProtectedRoute>
                    }
                />
                
                <Route
                    path="/suppliers"
                    element={
                        <ProtectedRoute>
                            <Layout>
                                <SuppliersPage />
                            </Layout>
                        </ProtectedRoute>
                    }
                />
                
                <Route
                    path="/leave-requests"
                    element={
                        <ProtectedRoute>
                            <Layout>
                                <LeaveRequestsPage />
                            </Layout>
                        </ProtectedRoute>
                    }
                />
                
                <Route
                    path="/stock-transfers"
                    element={
                        <ProtectedRoute>
                            <Layout>
                                <StockTransfersPage />
                            </Layout>
                        </ProtectedRoute>
                    }
                />
                
                <Route
                    path="/work-schedules"
                    element={
                        <ProtectedRoute>
                            <Layout>
                                <WorkSchedulesPage />
                            </Layout>
                        </ProtectedRoute>
                    }
                />
            </Routes>
            </AuthProvider>
        </HashRouter>
    );
};
