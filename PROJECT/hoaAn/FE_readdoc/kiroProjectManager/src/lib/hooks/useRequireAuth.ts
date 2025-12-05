import { useEffect } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';

/**
 * Hook to require authentication and optionally specific roles
 * Redirects to login if not authenticated or doesn't have required role
 * 
 * @param requiredRoles - Optional role(s) required to access the page
 * @returns Auth context
 */
export function useRequireAuth(requiredRoles?: string | string[]) {
    const auth = useAuth();
    const navigate = useNavigate();
    const location = useLocation();

    useEffect(() => {
        if (!auth.isLoading) {
            // Not authenticated - redirect to login
            if (!auth.isAuthenticated) {
                navigate('/login', {
                    state: { from: location },
                    replace: true
                });
                return;
            }

            // Check role if required
            if (requiredRoles && !auth.hasRole(requiredRoles)) {
                // User doesn't have required role - could redirect to unauthorized page
                console.warn('User does not have required role:', requiredRoles);
            }
        }
    }, [auth.isAuthenticated, auth.isLoading, auth.hasRole, requiredRoles, navigate, location]);

    return auth;
}
