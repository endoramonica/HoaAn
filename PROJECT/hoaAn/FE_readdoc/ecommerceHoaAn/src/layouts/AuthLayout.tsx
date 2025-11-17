import { Outlet } from "react-router-dom";

/**
 * Layout for authentication pages (Login, Register, Forgot Password)
 * No header or footer for clean auth experience
 */
export function AuthLayout() {
  return (
    <div className="min-h-screen">
      <Outlet />
    </div>
  );
}
