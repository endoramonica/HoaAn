import { createContext, useContext, useEffect, useState } from 'react';
import type { ReactNode } from 'react';
import { useNavigate } from 'react-router-dom';
import { authService, type User } from '../services/authService';
import { tokenStorage } from '../api/client';
import { getRolesFromToken, getPermissionsFromToken } from '../utils/jwtHelper';

interface AuthContextType {
    user: User | null;
    isLoading: boolean;
    isAuthenticated: boolean;
    roles: string[];
    permissions: string[];
    hasRole: (roles: string | string[]) => boolean;
    hasPermission: (permissions: string | string[]) => boolean;
    login: (email: string, password: string, rememberMe?: boolean) => Promise<void>;
    logout: () => Promise<void>;
    refreshUser: () => Promise<void>;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

interface AuthProviderProps {
    children: ReactNode;
}

export const AuthProvider = ({ children }: AuthProviderProps) => {
    const [user, setUser] = useState<User | null>(null);
    const [roles, setRoles] = useState<string[]>([]);
    const [permissions, setPermissions] = useState<string[]>([]);
    const [isLoading, setIsLoading] = useState(true);
    const navigate = useNavigate();

    // Load user on mount
    useEffect(() => {
        loadUser();
    }, []);

    const loadUser = async () => {
        try {
            if (authService.isAuthenticated()) {
                const token = tokenStorage.getAccessToken();
                
                if (token) {
                    // Extract roles and permissions from token
                    const tokenRoles = getRolesFromToken(token);
                    const tokenPermissions = getPermissionsFromToken(token);
                    
                    setRoles(tokenRoles);
                    setPermissions(tokenPermissions);
                    
                    // Load user data from API
                    const userData = await authService.getCurrentUser();
                    
                    // Merge token roles with user data
                    setUser({
                        ...userData,
                        role: tokenRoles.length > 0 ? tokenRoles[0] : userData.role,
                    });
                }
            }
        } catch (error) {
            console.error('Failed to load user:', error);
            tokenStorage.clearTokens();
            setUser(null);
            setRoles([]);
            setPermissions([]);
        } finally {
            setIsLoading(false);
        }
    };

    const login = async (email: string, password: string, rememberMe: boolean = false) => {
        await authService.login(email, password, rememberMe);
        await loadUser();
    };

    const logout = async () => {
        await authService.logout();
        setUser(null);
        setRoles([]);
        setPermissions([]);
        navigate('/login');
    };

    const refreshUser = async () => {
        await loadUser();
    };

    const hasRole = (requiredRoles: string | string[]): boolean => {
        if (roles.length === 0) return false;
        
        const rolesToCheck = Array.isArray(requiredRoles) ? requiredRoles : [requiredRoles];
        
        return rolesToCheck.some(role => 
            roles.some(userRole => 
                userRole.toLowerCase() === role.toLowerCase()
            )
        );
    };

    const hasPermission = (requiredPermissions: string | string[]): boolean => {
        if (permissions.length === 0) return false;
        
        const permissionsToCheck = Array.isArray(requiredPermissions) ? requiredPermissions : [requiredPermissions];
        
        return permissionsToCheck.some(permission => 
            permissions.includes(permission)
        );
    };

    const value: AuthContextType = {
        user,
        isLoading,
        isAuthenticated: !!user,
        roles,
        permissions,
        hasRole,
        hasPermission,
        login,
        logout,
        refreshUser,
    };

    return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};

export const useAuth = () => {
    const context = useContext(AuthContext);
    if (context === undefined) {
        throw new Error('useAuth must be used within an AuthProvider');
    }
    return context;
};
