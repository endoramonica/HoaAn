/**
 * LoginPage - Trang đăng nhập (Phase 1: ĐÃ MIGRATE HOÀN TOÀN)
 * Dùng react-router-dom thuần, không onNavigate, không hybrid
 */
import { useState, type FormEvent } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../../lib/hooks/useAuth";
import { Button } from "../../components/ui/button";
import { Input } from "../../components/ui/input";
import { Label } from "../../components/ui/label";
import {
  Card,
  CardContent,
  CardDescription,
  CardFooter,
  CardHeader,
  CardTitle,
} from "../../components/ui/card";
import { Checkbox } from "../../components/ui/checkbox";
import { Separator } from "../../components/ui/separator";
import {
  Alert,
  AlertDescription,
} from "../../components/ui/alert";
import { GoogleLoginButton } from "../../components/auth/GoogleLoginButton";
import { Loader2, Flower2, Flame } from "lucide-react";

export const LoginPage = () => {
  const navigate = useNavigate();
  const { login, isLoading, error } = useAuth();
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [rememberMe, setRememberMe] = useState(false);
  const [validationErrors, setValidationErrors] = useState<
    Record<string, string>
  >({});

  const validateForm = (): boolean => {
    const errors: Record<string, string> = {};
    if (!email) errors.email = "Vui lòng nhập email";
    else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email))
      errors.email = "Email không hợp lệ";
    if (!password) errors.password = "Vui lòng nhập mật khẩu";
    else if (password.length < 6)
      errors.password = "Mật khẩu phải có ít nhất 6 ký tự";
    setValidationErrors(errors);
    return Object.keys(errors).length === 0;
  };

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    if (!validateForm()) return;
    try {
      await login({ email, password, rememberMe });
      navigate("/"); // home
    } catch (err) {
      console.error("Login failed:", err);
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-[#FFFBEB] px-4 py-12 relative overflow-hidden">
      {/* Decorative background */}
      <div className="absolute inset-0 opacity-5">
        <div className="absolute top-10 left-10">
          <Flower2 className="w-32 h-32 text-[#92400E]" />
        </div>
        <div className="absolute bottom-10 right-10">
          <Flower2 className="w-32 h-32 text-[#92400E]" />
        </div>
        <div className="absolute top-1/2 left-1/4">
          <Flame className="w-24 h-24 text-[#DC2626]" />
        </div>
        <div className="absolute top-1/3 right-1/4">
          <Flame className="w-24 h-24 text-[#F59E0B]" />
        </div>
      </div>

      <Card className="w-full max-w-md relative z-10 border-2 border-[#92400E]/20 shadow-xl">
        <CardHeader className="space-y-2 text-center bg-gradient-to-br from-[#92400E]/5 to-[#F59E0B]/5 rounded-t-lg">
          <div className="flex justify-center mb-2">
            <div className="w-16 h-16 bg-gradient-to-br from-[#92400E] to-[#DC2626] rounded-full flex items-center justify-center shadow-lg">
              <Flower2 className="w-8 h-8 text-[#FFFBEB]" />
            </div>
          </div>
          <CardTitle className="text-[#92400E]">
            Đăng Nhập
          </CardTitle>
          <CardDescription className="text-[#92400E]/70">
            Chào mừng bạn trở lại với cửa hàng đồ cúng truyền
            thống
          </CardDescription>
        </CardHeader>

        <CardContent className="pt-6 space-y-4">
          {error && (
            <Alert
              variant="destructive"
              className="border-[#DC2626] bg-[#DC2626]/10"
            >
              <AlertDescription>{error}</AlertDescription>
            </Alert>
          )}

          <form onSubmit={handleSubmit} className="space-y-4">
            <div className="space-y-2">
              <Label htmlFor="email" className="text-[#92400E]">
                Email
              </Label>
              <Input
                id="email"
                type="email"
                placeholder="example@email.com"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                className={`border-[#92400E]/30 focus:border-[#92400E] ${validationErrors.email ? "border-[#DC2626]" : ""}`}
                disabled={isLoading}
              />
              {validationErrors.email && (
                <p className="text-xs text-[#DC2626]">
                  {validationErrors.email}
                </p>
              )}
            </div>

            <div className="space-y-2">
              <Label
                htmlFor="password"
                className="text-[#92400E]"
              >
                Mật khẩu
              </Label>
              <Input
                id="password"
                type="password"
                placeholder="••••••••"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                className={`border-[#92400E]/30 focus:border-[#92400E] ${validationErrors.password ? "border-[#DC2626]" : ""}`}
                disabled={isLoading}
              />
              {validationErrors.password && (
                <p className="text-xs text-[#DC2626]">
                  {validationErrors.password}
                </p>
              )}
            </div>

            <div className="flex items-center justify-between">
              <div className="flex items-center space-x-2">
                <Checkbox
                  id="remember"
                  checked={rememberMe}
                  onCheckedChange={(checked) =>
                    setRememberMe(checked as boolean)
                  }
                  className="border-[#92400E]/30 data-[state=checked]:bg-[#92400E]"
                  disabled={isLoading}
                />
                <Label
                  htmlFor="remember"
                  className="text-sm text-[#92400E]/70 cursor-pointer"
                >
                  Ghi nhớ đăng nhập
                </Label>
              </div>
              <button
                type="button"
                onClick={() =>
                  navigate("/auth/forgot-password")
                }
                className="text-sm text-[#DC2626] hover:underline"
                disabled={isLoading}
              >
                Quên mật khẩu?
              </button>
            </div>

            <Button
              type="submit"
              className="w-full bg-gradient-to-r from-[#92400E] to-[#DC2626] text-white"
              disabled={isLoading}
            >
              {isLoading ? (
                <>
                  {" "}
                  <Loader2 className="mr-2 h-4 w-4 animate-spin" />{" "}
                  Đang đăng nhập...{" "}
                </>
              ) : (
                "Đăng nhập"
              )}
            </Button>
          </form>

          <div className="relative my-6">
            <Separator className="w-full bg-[#92400E]/20" />
            <span className="absolute left-1/2 top-1/2 -translate-x-1/2 -translate-y-1/2 bg-white px-2 text-xs text-[#92400E]/70">
              Hoặc
            </span>
          </div>

          <GoogleLoginButton
            onNavigate={(path) => navigate(path)}
          />

          <div className="text-center pt-4">
            <p className="text-sm text-[#92400E]/70">
              Chưa có tài khoản?{" "}
              <button
                onClick={() => navigate("/auth/register")}
                className="text-[#DC2626] hover:underline"
                disabled={isLoading}
              >
                Đăng ký ngay
              </button>
            </p>
          </div>
        </CardContent>

        <CardFooter className="bg-gradient-to-br from-[#92400E]/5 to-[#F59E0B]/5 rounded-b-lg">
          <button
            onClick={() => navigate("/")}
            className="text-sm text-[#92400E]/70 hover:underline w-full text-center"
            disabled={isLoading}
          >
            ← Quay về trang chủ
          </button>
        </CardFooter>
      </Card>
    </div>
  );
};

export default LoginPage;