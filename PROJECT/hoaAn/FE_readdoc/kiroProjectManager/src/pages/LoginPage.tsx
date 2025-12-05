import { useState } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { useAuth } from '../lib/contexts/AuthContext';
import { Button } from '../components/ui/Button';
import { Input } from '../components/ui/Input';
import { Card } from '../components/ui/Card';
import { toast } from 'sonner';
import { LogIn } from 'lucide-react';

export const LoginPage = () => {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [rememberMe, setRememberMe] = useState(false);
    const [isLoading, setIsLoading] = useState(false);
    const navigate = useNavigate();
    const location = useLocation();
    const { login, hasRole } = useAuth();

    // Get the page user was trying to access before being redirected to login
    const from = (location.state as any)?.from?.pathname || '/';

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setIsLoading(true);

        try {
            await login(email, password, rememberMe);
            
            // Check if user has admin role
            if (!hasRole(['admin', 'administrator'])) {
                toast.error('Bạn không có quyền truy cập. Chỉ Admin mới có thể đăng nhập.');
                setIsLoading(false);
                return;
            }

            toast.success('Đăng nhập thành công!');
            
            // Navigate back to the page user was trying to access
            navigate(from, { replace: true });
        } catch (error: any) {
            toast.error(error.message || 'Đăng nhập thất bại');
            setIsLoading(false);
        }
    };

    return (
        <div className="min-h-screen flex items-center justify-center bg-gray-50 px-4">
            <Card className="w-full max-w-md">
                <div className="text-center mb-6">
                    <h1 className="text-3xl font-bold text-gray-900 mb-2">
                        Đồ Cúng Store
                    </h1>
                    <p className="text-gray-600">
                        Đăng nhập vào hệ thống quản lý
                    </p>
                </div>

                <form onSubmit={handleSubmit} className="space-y-4">
                    <Input
                        label="Email"
                        type="email"
                        value={email}
                        onChange={(e) => setEmail(e.target.value)}
                        placeholder="admin@example.com"
                        required
                    />

                    <Input
                        label="Mật khẩu"
                        type="password"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        placeholder="••••••••"
                        required
                    />

                    <div className="flex items-center">
                        <input
                            id="remember-me"
                            type="checkbox"
                            checked={rememberMe}
                            onChange={(e) => setRememberMe(e.target.checked)}
                            className="w-4 h-4 text-green-600 border-gray-300 rounded focus:ring-green-500"
                        />
                        <label
                            htmlFor="remember-me"
                            className="ml-2 text-sm text-gray-700"
                        >
                            Ghi nhớ đăng nhập
                        </label>
                    </div>

                    <Button
                        type="submit"
                        variant="primary"
                        className="w-full"
                        isLoading={isLoading}
                        icon={<LogIn className="w-5 h-5" />}
                    >
                        Đăng nhập
                    </Button>
                </form>
            </Card>
        </div>
    );
};
