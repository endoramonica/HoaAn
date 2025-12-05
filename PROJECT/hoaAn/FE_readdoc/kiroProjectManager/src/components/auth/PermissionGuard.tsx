import type { ReactNode } from 'react';
import { useAuth } from '../../lib/contexts/AuthContext';

interface PermissionGuardProps {
    children: ReactNode;
    requiredPermissions: string | string[];
    fallback?: ReactNode;
}

/**
 * Component để ẩn/hiện nội dung dựa trên permissions
 * Sử dụng trong component để kiểm soát UI elements theo quyền hạn cụ thể
 */
export const PermissionGuard = ({ children, requiredPermissions, fallback = null }: PermissionGuardProps) => {
    const { hasPermission } = useAuth();

    if (!hasPermission(requiredPermissions)) {
        return <>{fallback}</>;
    }

    return <>{children}</>;
};
