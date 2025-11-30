/**
 * ProtectedAction Component
 * Wraps actions that require authentication
 */

import { type ReactNode } from "react";
import { useAuth } from "@/lib/hooks/useAuth";
import { toast } from "sonner";

interface ProtectedActionProps {
  children: (onClick: () => void) => ReactNode;
  onAction: () => void | Promise<void>;
}

export const ProtectedAction = ({
  children,
  onAction,
}: ProtectedActionProps) => {
  const { requireAuth } = useAuth();

  const handleClick = async () => {
    try {
      requireAuth();
      await onAction();
    } catch (error: any) {
      if (error?.code !== "AUTH_REQUIRED") {
        console.error("Protected action error:", error);
      }
    }
  };

  return <>{children(handleClick)}</>;
};
