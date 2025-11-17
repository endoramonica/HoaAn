/**
 * useHybridNavigate - Hook kết hợp useState navigation và React Router
 * Cho phép components hoạt động với cả hai phương thức navigation
 */

import { useCallback } from "react";
import { useNavigate } from "react-router-dom";

export function useHybridNavigate(onNavigate?: (page: string) => void) {
  const routerNavigate = useNavigate();

  // Nếu có onNavigate từ cha => dùng nó, không thì fallback sang routerNavigate
  const navigateTo = useCallback(
    (page: string) => {
      if (onNavigate) {
        onNavigate(page);
      } else {
        routerNavigate(page.startsWith("/") ? page : `/${page}`);
      }
    },
    [onNavigate, routerNavigate]
  );

  return navigateTo;
}

export default useHybridNavigate;
