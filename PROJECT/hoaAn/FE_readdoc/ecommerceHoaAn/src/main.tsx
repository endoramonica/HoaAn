
  import { createRoot } from "react-dom/client";
  import App from "./App.tsx";
  import { initializeApp } from '@/api/helpers/appSetup';
  import "./index.css";
  initializeApp();
  createRoot(document.getElementById("root")!).render(<App />);
  