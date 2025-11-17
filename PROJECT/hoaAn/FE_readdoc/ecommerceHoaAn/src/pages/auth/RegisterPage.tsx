/**
 * RegisterPage - Trang đăng ký với thiết kế văn hóa Việt Nam
 * Sử dụng màu nâu ấm, vàng, đỏ nghi lễ và nền kem
 */

import { useState, FormEvent } from "react";
import { useAuth } from "../../lib/hooks/useAuth";
import { useNavigate } from "react-router-dom";
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
import { Separator } from "../../components/ui/separator";
import {
  Alert,
  AlertDescription,
} from "../../components/ui/alert";
import { GoogleLoginButton } from "../../components/auth/GoogleLoginButton";
import {
  Loader2,
  Flower2,
  Flame,
  CheckCircle2,
} from "lucide-react";

export const RegisterPage = () => {
  const navigate = useNavigate();
  const { register, isLoading, error } = useAuth();
  const [formData, setFormData] = useState({
    email: "",
    password: "",
    confirmPassword: "",
    fullName: "",
    phoneNumber: "",
  });
  const [validationErrors, setValidationErrors] = useState<
    Record<string, string>
  >({});
  const [showSuccess, setShowSuccess] = useState(false);

  const validateForm = (): boolean => {
    const errors: Record<string, string> = {};

    // Full name validation
    if (!formData.fullName) {
      errors.fullName = "Vui lòng nhập họ tên";
    } else if (formData.fullName.length < 2) {
      errors.fullName = "Họ tên phải có ít nhất 2 ký tự";
    }

    // Email validation
    if (!formData.email) {
      errors.email = "Vui lòng nhập email";
    } else if (
      !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(formData.email)
    ) {
      errors.email = "Email không hợp lệ";
    }

    // Phone number validation (optional)
    if (
      formData.phoneNumber &&
      !/^[0-9]{10}$/.test(
        formData.phoneNumber.replace(/\s/g, ""),
      )
    ) {
      errors.phoneNumber = "Số điện thoại phải có 10 chữ số";
    }

    // Password validation
    if (!formData.password) {
      errors.password = "Vui lòng nhập mật khẩu";
    } else if (formData.password.length < 6) {
      errors.password = "Mật khẩu phải có ít nhất 6 ký tự";
    } else if (
      !/(?=.*[a-z])(?=.*[A-Z])(?=.*\d)/.test(formData.password)
    ) {
      errors.password =
        "Mật khẩu phải có chữ hoa, chữ thường và số";
    }

    // Confirm password validation
    if (!formData.confirmPassword) {
      errors.confirmPassword = "Vui lòng xác nhận mật khẩu";
    } else if (formData.password !== formData.confirmPassword) {
      errors.confirmPassword = "Mật khẩu xác nhận không khớp";
    }

    setValidationErrors(errors);
    return Object.keys(errors).length === 0;
  };

  const handleChange = (field: string, value: string) => {
    setFormData((prev) => ({ ...prev, [field]: value }));
    // Clear validation error when user types
    if (validationErrors[field]) {
      setValidationErrors((prev) => {
        const newErrors = { ...prev };
        delete newErrors[field];
        return newErrors;
      });
    }
  };

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();

    if (!validateForm()) {
      return;
    }

    try {
      await register({
        email: formData.email,
        password: formData.password,
        confirmPassword: formData.confirmPassword,
        fullName: formData.fullName,
        phoneNumber: formData.phoneNumber || undefined,
      });

      // Hiển thị thông báo thành công
      setShowSuccess(true);

      // Chuyển hướng sau 2 giây
      setTimeout(() => {
        navigate("/");
      }, 2000);
    } catch (err) {
      // Error được xử lý bởi useAuth hook
      console.error("Register failed:", err);
    }
  };

  if (showSuccess) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-[#FFFBEB] px-4 py-12">
        <Card className="w-full max-w-md border-2 border-[#92400E]/20 shadow-xl">
          <CardContent className="pt-12 pb-12 text-center space-y-4">
            <div className="flex justify-center">
              <div className="w-20 h-20 bg-gradient-to-br from-[#92400E] to-[#F59E0B] rounded-full flex items-center justify-center shadow-lg">
                <CheckCircle2 className="w-10 h-10 text-white" />
              </div>
            </div>
            <h2 className="text-2xl text-[#92400E]">
              Đăng ký thành công!
            </h2>
            <p className="text-[#92400E]/70">
              Chào mừng bạn đến với cửa hàng đồ cúng truyền
              thống
            </p>
            <div className="flex items-center justify-center space-x-1 text-[#92400E]/70">
              <Loader2 className="w-4 h-4 animate-spin" />
              <span className="text-sm">
                Đang chuyển hướng...
              </span>
            </div>
          </CardContent>
        </Card>
      </div>
    );
  }

  return (
    <div className="min-h-screen flex items-center justify-center bg-[#FFFBEB] px-4 py-12 relative overflow-hidden">
      {/* Background decorative elements */}
      <div className="absolute inset-0 opacity-5">
        <div className="absolute top-10 right-10">
          <Flower2 className="w-32 h-32 text-[#92400E]" />
        </div>
        <div className="absolute bottom-10 left-10">
          <Flower2 className="w-32 h-32 text-[#92400E]" />
        </div>
        <div className="absolute top-1/2 right-1/4">
          <Flame className="w-24 h-24 text-[#DC2626]" />
        </div>
        <div className="absolute top-1/3 left-1/4">
          <Flame className="w-24 h-24 text-[#F59E0B]" />
        </div>
      </div>

      <Card className="w-full max-w-md relative z-10 border-2 border-[#92400E]/20 shadow-xl">
        <CardHeader className="space-y-2 text-center bg-gradient-to-br from-[#92400E]/5 to-[#F59E0B]/5 rounded-t-lg">
          <div className="flex justify-center mb-2">
            <div className="w-16 h-16 bg-gradient-to-br from-[#F59E0B] to-[#92400E] rounded-full flex items-center justify-center shadow-lg">
              <Flower2 className="w-8 h-8 text-[#FFFBEB]" />
            </div>
          </div>
          <CardTitle className="text-[#92400E]">
            Đăng Ký Tài Khoản
          </CardTitle>
          <CardDescription className="text-[#92400E]/70">
            Tạo tài khoản để trải nghiệm dịch vụ đồ cúng truyền
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
            {/* Full Name Field */}
            <div className="space-y-2">
              <Label
                htmlFor="fullName"
                className="text-[#92400E]"
              >
                Họ và tên{" "}
                <span className="text-[#DC2626]">*</span>
              </Label>
              <Input
                id="fullName"
                type="text"
                placeholder="Nguyễn Văn A"
                value={formData.fullName}
                onChange={(e) =>
                  handleChange("fullName", e.target.value)
                }
                className={`border-[#92400E]/30 focus:border-[#92400E] focus:ring-[#92400E]/20 ${
                  validationErrors.fullName
                    ? "border-[#DC2626]"
                    : ""
                }`}
                disabled={isLoading}
              />
              {validationErrors.fullName && (
                <p className="text-xs text-[#DC2626]">
                  {validationErrors.fullName}
                </p>
              )}
            </div>

            {/* Email Field */}
            <div className="space-y-2">
              <Label htmlFor="email" className="text-[#92400E]">
                Email <span className="text-[#DC2626]">*</span>
              </Label>
              <Input
                id="email"
                type="email"
                placeholder="example@email.com"
                value={formData.email}
                onChange={(e) =>
                  handleChange("email", e.target.value)
                }
                className={`border-[#92400E]/30 focus:border-[#92400E] focus:ring-[#92400E]/20 ${
                  validationErrors.email
                    ? "border-[#DC2626]"
                    : ""
                }`}
                disabled={isLoading}
              />
              {validationErrors.email && (
                <p className="text-xs text-[#DC2626]">
                  {validationErrors.email}
                </p>
              )}
            </div>

            {/* Phone Number Field */}
            <div className="space-y-2">
              <Label
                htmlFor="phoneNumber"
                className="text-[#92400E]"
              >
                Số điện thoại{" "}
                <span className="text-[#92400E]/50">
                  (Tùy chọn)
                </span>
              </Label>
              <Input
                id="phoneNumber"
                type="tel"
                placeholder="0901234567"
                value={formData.phoneNumber}
                onChange={(e) =>
                  handleChange("phoneNumber", e.target.value)
                }
                className={`border-[#92400E]/30 focus:border-[#92400E] focus:ring-[#92400E]/20 ${
                  validationErrors.phoneNumber
                    ? "border-[#DC2626]"
                    : ""
                }`}
                disabled={isLoading}
              />
              {validationErrors.phoneNumber && (
                <p className="text-xs text-[#DC2626]">
                  {validationErrors.phoneNumber}
                </p>
              )}
            </div>

            {/* Password Field */}
            <div className="space-y-2">
              <Label
                htmlFor="password"
                className="text-[#92400E]"
              >
                Mật khẩu{" "}
                <span className="text-[#DC2626]">*</span>
              </Label>
              <Input
                id="password"
                type="password"
                placeholder="••••••••"
                value={formData.password}
                onChange={(e) =>
                  handleChange("password", e.target.value)
                }
                className={`border-[#92400E]/30 focus:border-[#92400E] focus:ring-[#92400E]/20 ${
                  validationErrors.password
                    ? "border-[#DC2626]"
                    : ""
                }`}
                disabled={isLoading}
              />
              {validationErrors.password && (
                <p className="text-xs text-[#DC2626]">
                  {validationErrors.password}
                </p>
              )}
              <p className="text-xs text-[#92400E]/60">
                Mật khẩu phải có ít nhất 6 ký tự, bao gồm chữ
                hoa, chữ thường và số
              </p>
            </div>

            {/* Confirm Password Field */}
            <div className="space-y-2">
              <Label
                htmlFor="confirmPassword"
                className="text-[#92400E]"
              >
                Xác nhận mật khẩu{" "}
                <span className="text-[#DC2626]">*</span>
              </Label>
              <Input
                id="confirmPassword"
                type="password"
                placeholder="••••••••"
                value={formData.confirmPassword}
                onChange={(e) =>
                  handleChange(
                    "confirmPassword",
                    e.target.value,
                  )
                }
                className={`border-[#92400E]/30 focus:border-[#92400E] focus:ring-[#92400E]/20 ${
                  validationErrors.confirmPassword
                    ? "border-[#DC2626]"
                    : ""
                }`}
                disabled={isLoading}
              />
              {validationErrors.confirmPassword && (
                <p className="text-xs text-[#DC2626]">
                  {validationErrors.confirmPassword}
                </p>
              )}
            </div>

            {/* Submit Button */}
            <Button
              type="submit"
              className="w-full bg-gradient-to-r from-[#F59E0B] to-[#92400E] hover:from-[#F59E0B]/90 hover:to-[#92400E]/90 text-white shadow-md"
              disabled={isLoading}
            >
              {isLoading ? (
                <>
                  <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                  Đang đăng ký...
                </>
              ) : (
                "Đăng ký"
              )}
            </Button>
          </form>

          {/* Divider */}
          <div className="relative my-6">
            <div className="absolute inset-0 flex items-center">
              <Separator className="w-full bg-[#92400E]/20" />
            </div>
            <div className="relative flex justify-center text-xs uppercase">
              <span className="bg-white px-2 text-[#92400E]/70">
                Hoặc
              </span>
            </div>
          </div>

          {/* Google Sign Up */}
          <GoogleLoginButton onNavigate={navigate} isSignUp />

          {/* Login Link */}
          <div className="text-center pt-4">
            <p className="text-sm text-[#92400E]/70">
              Đã có tài khoản?{" "}
              <button
                onClick={() => navigate("/auth/login")}
                className="text-[#DC2626] hover:text-[#DC2626]/80 hover:underline"
                disabled={isLoading}
              >
                Đăng nhập ngay
              </button>
            </p>
          </div>
        </CardContent>

        <CardFooter className="flex flex-col space-y-2 bg-gradient-to-br from-[#92400E]/5 to-[#F59E0B]/5 rounded-b-lg">
          <button
            onClick={() => navigate("home")}
            className="text-sm text-[#92400E]/70 hover:text-[#92400E] hover:underline"
            disabled={isLoading}
          >
            ← Quay về trang chủ
          </button>
        </CardFooter>
      </Card>
    </div>
  );
};

export default RegisterPage;