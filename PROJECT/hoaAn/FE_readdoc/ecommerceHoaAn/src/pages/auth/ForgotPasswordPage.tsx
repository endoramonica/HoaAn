/**
 * ForgotPasswordPage - Trang quên mật khẩu
 * Gửi email để reset mật khẩu
 */

import { useState, FormEvent } from "react";
import { useNavigate } from "react-router-dom";
import { authService } from "../../lib/services/authService";
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
import {
  Alert,
  AlertDescription,
} from "../../components/ui/alert";
import {
  Loader2,
  Flower2,
  Mail,
  CheckCircle2,
  ArrowLeft,
  KeyRound,
} from "lucide-react";
import { toast } from "sonner@2.0.3";

export const ForgotPasswordPage = () => {
  const navigate = useNavigate();
  const [email, setEmail] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const [isSuccess, setIsSuccess] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [validationError, setValidationError] = useState("");

  const validateEmail = (): boolean => {
    if (!email) {
      setValidationError("Vui lòng nhập email");
      return false;
    }
    if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) {
      setValidationError("Email không hợp lệ");
      return false;
    }
    setValidationError("");
    return true;
  };

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();

    if (!validateEmail()) {
      return;
    }

    try {
      setIsLoading(true);
      setError(null);

      await authService.forgotPassword(email);

      setIsSuccess(true);
      toast.success("Đã gửi email khôi phục mật khẩu!");
    } catch (err: any) {
      console.error("Forgot password error:", err);
      const errorMsg =
        err.message || "Không thể gửi email khôi phục mật khẩu";
      setError(errorMsg);
      toast.error(errorMsg);
    } finally {
      setIsLoading(false);
    }
  };

  if (isSuccess) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-[#FFFBEB] px-4 py-12 relative overflow-hidden">
        {/* Background decorative elements */}
        <div className="absolute inset-0 opacity-5">
          <div className="absolute top-10 left-10">
            <Flower2 className="w-32 h-32 text-[#92400E]" />
          </div>
          <div className="absolute bottom-10 right-10">
            <Flower2 className="w-32 h-32 text-[#92400E]" />
          </div>
        </div>

        <Card className="w-full max-w-md relative z-10 border-2 border-[#92400E]/20 shadow-xl">
          <CardHeader className="space-y-2 text-center bg-gradient-to-br from-[#92400E]/5 to-[#F59E0B]/5 rounded-t-lg">
            <div className="flex justify-center mb-2">
              <div className="w-16 h-16 bg-gradient-to-br from-[#92400E] to-[#F59E0B] rounded-full flex items-center justify-center shadow-lg">
                <CheckCircle2 className="w-8 h-8 text-white" />
              </div>
            </div>
            <CardTitle className="text-[#92400E]">
              Email Đã Được Gửi!
            </CardTitle>
            <CardDescription className="text-[#92400E]/70">
              Vui lòng kiểm tra hộp thư của bạn
            </CardDescription>
          </CardHeader>

          <CardContent className="pt-6 space-y-4">
            <Alert className="border-[#F59E0B] bg-[#F59E0B]/10">
              <Mail className="h-4 w-4 text-[#F59E0B]" />
              <AlertDescription className="text-[#92400E]/80">
                Chúng tôi đã gửi hướng dẫn khôi phục mật khẩu
                đến email <strong>{email}</strong>. Vui lòng
                kiểm tra cả thư mục spam nếu không thấy email.
              </AlertDescription>
            </Alert>

            <div className="bg-[#FFFBEB] border border-[#92400E]/20 rounded-lg p-4 space-y-2">
              <h4 className="text-sm text-[#92400E]">Lưu ý:</h4>
              <ul className="text-xs text-[#92400E]/70 space-y-1 list-disc list-inside">
                <li>Link khôi phục có hiệu lực trong 24 giờ</li>
                <li>Không chia sẻ link này với bất kỳ ai</li>
                <li>
                  Nếu không yêu cầu đặt lại mật khẩu, vui lòng
                  bỏ qua email này
                </li>
              </ul>
            </div>

            <div className="flex flex-col gap-3 pt-4">
              <Button
                onClick={() => navigate("/auth/login")}
                className="w-full bg-gradient-to-r from-[#92400E] to-[#DC2626] hover:from-[#92400E]/90 hover:to-[#DC2626]/90 text-white"
              >
                Đăng nhập
              </Button>

              <Button
                variant="outline"
                onClick={() => setIsSuccess(false)}
                className="w-full border-[#92400E]/30 text-[#92400E] hover:border-[#92400E] hover:bg-[#92400E]/5"
              >
                Gửi lại email
              </Button>
            </div>
          </CardContent>

          <CardFooter className="flex flex-col space-y-2 bg-gradient-to-br from-[#92400E]/5 to-[#F59E0B]/5 rounded-b-lg">
            <button
              onClick={() => navigate("/")}
              className="text-sm text-[#92400E]/70 hover:text-[#92400E] hover:underline"
            >
              ← Quay về trang chủ
            </button>
          </CardFooter>
        </Card>
      </div>
    );
  }

  return (
    <div className="min-h-screen flex items-center justify-center bg-[#FFFBEB] px-4 py-12 relative overflow-hidden">
      {/* Background decorative elements */}
      <div className="absolute inset-0 opacity-5">
        <div className="absolute top-10 left-10">
          <Flower2 className="w-32 h-32 text-[#92400E]" />
        </div>
        <div className="absolute bottom-10 right-10">
          <Flower2 className="w-32 h-32 text-[#92400E]" />
        </div>
        <div className="absolute top-1/2 left-1/4">
          <KeyRound className="w-24 h-24 text-[#F59E0B]" />
        </div>
        <div className="absolute top-1/3 right-1/4">
          <Mail className="w-24 h-24 text-[#DC2626]" />
        </div>
      </div>

      <Card className="w-full max-w-md relative z-10 border-2 border-[#92400E]/20 shadow-xl">
        <CardHeader className="space-y-2 text-center bg-gradient-to-br from-[#92400E]/5 to-[#F59E0B]/5 rounded-t-lg">
          <div className="flex justify-center mb-2">
            <div className="w-16 h-16 bg-gradient-to-br from-[#92400E] to-[#DC2626] rounded-full flex items-center justify-center shadow-lg">
              <KeyRound className="w-8 h-8 text-[#FFFBEB]" />
            </div>
          </div>
          <CardTitle className="text-[#92400E]">
            Quên Mật Khẩu?
          </CardTitle>
          <CardDescription className="text-[#92400E]/70">
            Nhập email của bạn để nhận hướng dẫn khôi phục mật
            khẩu
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
            {/* Email Field */}
            <div className="space-y-2">
              <Label htmlFor="email" className="text-[#92400E]">
                Email đã đăng ký
              </Label>
              <div className="relative">
                <Mail className="absolute left-3 top-1/2 transform -translate-y-1/2 w-4 h-4 text-[#92400E]/50" />
                <Input
                  id="email"
                  type="email"
                  placeholder="example@email.com"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                  className={`pl-10 border-[#92400E]/30 focus:border-[#92400E] focus:ring-[#92400E]/20 ${
                    validationError ? "border-[#DC2626]" : ""
                  }`}
                  disabled={isLoading}
                />
              </div>
              {validationError && (
                <p className="text-xs text-[#DC2626]">
                  {validationError}
                </p>
              )}
            </div>

            {/* Submit Button */}
            <Button
              type="submit"
              className="w-full bg-gradient-to-r from-[#92400E] to-[#DC2626] hover:from-[#92400E]/90 hover:to-[#DC2626]/90 text-white shadow-md"
              disabled={isLoading}
            >
              {isLoading ? (
                <>
                  <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                  Đang gửi email...
                </>
              ) : (
                <>
                  <Mail className="mr-2 h-4 w-4" />
                  Gửi email khôi phục
                </>
              )}
            </Button>
          </form>

          {/* Info Box */}
          <div className="bg-[#F59E0B]/10 border border-[#F59E0B]/30 rounded-lg p-4">
            <p className="text-sm text-[#92400E]/80">
              💡 <strong>Mẹo:</strong> Hãy chắc chắn bạn nhập
              đúng email đã đăng ký tài khoản. Nếu không nhận
              được email sau 5 phút, vui lòng kiểm tra thư mục
              spam hoặc liên hệ hỗ trợ.
            </p>
          </div>

          {/* Back to Login */}
          <div className="text-center pt-2">
            <button
              onClick={() => navigate("/auth/login")}
              className="text-sm text-[#92400E] hover:text-[#92400E]/80 hover:underline inline-flex items-center"
              disabled={isLoading}
            >
              <ArrowLeft className="w-4 h-4 mr-1" />
              Quay lại đăng nhập
            </button>
          </div>
        </CardContent>

        <CardFooter className="flex flex-col space-y-2 bg-gradient-to-br from-[#92400E]/5 to-[#F59E0B]/5 rounded-b-lg">
          <div className="text-center w-full">
            <p className="text-xs text-[#92400E]/70 mb-2">
              Chưa có tài khoản?{" "}
              <button
                onClick={() => navigate("/auth/register")}
                className="text-[#DC2626] hover:text-[#DC2626]/80 hover:underline"
                disabled={isLoading}
              >
                Đăng ký ngay
              </button>
            </p>
          </div>
        </CardFooter>
      </Card>
    </div>
  );
};

export default ForgotPasswordPage;