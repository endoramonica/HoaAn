// import React, { createContext, useContext, useState, useEffect } from "react";

// interface User {
//   email: string;
// }

// interface AuthContextType {
//   user: User | null;
//   login: (email: string, password: string) => Promise<boolean>;
//   logout: () => void;
//   isLoading: boolean;
// }

// const AuthContext = createContext<AuthContextType | undefined>(undefined);

// export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
//   const [user, setUser] = useState<User | null>(null);
//   const [isLoading, setIsLoading] = useState(true);

//   useEffect(() => {
//     // Kiểm tra token trong localStorage khi reload
//     const token = localStorage.getItem("token");
//     const email = localStorage.getItem("userEmail");

//     if (token && email) {
//       setUser({ email });
//     }
//     setIsLoading(false);
//   }, []);

//   const login = async (email: string, password: string) => {
//     try {
//       const response = await fetch("https://localhost:7131/api/v1/Auth/login", {
//         method: "POST",
//         headers: {
//           "Content-Type": "application/json",
//           Accept: "application/json",
//         },
//         body: JSON.stringify({ email, password }),
//       });

//       const result = await response.json();
//       console.log("🔐 Login result:", result);

//       if (!response.ok || !result.success) {
//         console.error("❌ Login failed:", result.message);
//         return false;
//       }

//       // ✅ Lưu token và refreshToken vào localStorage
//       localStorage.setItem("token", result.data.token);
//       localStorage.setItem("refreshToken", result.data.refreshToken);
//       localStorage.setItem("tokenExpiry", result.data.expires);
//       localStorage.setItem("userEmail", email);

//       // Cập nhật context
//       setUser({ email });
//       return true;
//     } catch (error) {
//       console.error("🚨 Login error:", error);
//       return false;
//     }
//   };

//   const logout = () => {
//     localStorage.removeItem("token");
//     localStorage.removeItem("refreshToken");
//     localStorage.removeItem("tokenExpiry");
//     localStorage.removeItem("userEmail");
//     setUser(null);
//   };

//   return (
//     <AuthContext.Provider value={{ user, login, logout, isLoading }}>
//       {children}
//     </AuthContext.Provider>
//   );
// };

// export const useAuth = (): AuthContextType => {
//   const context = useContext(AuthContext);
//   if (!context) {
//     throw new Error("useAuth must be used within an AuthProvider");
//   }
//   return context;
// };
