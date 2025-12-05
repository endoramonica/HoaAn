import type { ReactNode } from 'react';
import { useAuth } from '../../lib/contexts/AuthContext';

interface RoleGuardProps {
    children: ReactNode;
    requiredRoles: string | string[];
    fallback?: ReactNode;
}

/**
 * Component để ẩn/hiện nội dung dựa trên role
 * Sử dụng trong component để kiểm soát UI elements
 */
export const RoleGuard = ({ children, requiredRoles, fallback = null }: RoleGuardProps) => {
    const { hasRole } = useAuth();

    if (!hasRole(requiredRoles)) {
        return <>{fallback}</>;
    }

    return <>{children}</>;
};
