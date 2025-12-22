import { expect, afterEach, vi } from 'vitest';
import '@testing-library/jest-dom';

// Mock sessionStorage
const sessionStorageMock = (() => {
  let store: Record<string, string> = {};

  return {
    getItem: (key: string) => store[key] || null,
    setItem: (key: string, value: string) => {
      store[key] = value.toString();
    },
    removeItem: (key: string) => {
      delete store[key];
    },
    clear: () => {
      store = {};
    },
  };
})();

Object.defineProperty(window, 'sessionStorage', {
  value: sessionStorageMock,
});

// Mock fetch globally
global.fetch = vi.fn();

// Clear mocks after each test
afterEach(() => {
  vi.clearAllMocks();
  sessionStorageMock.clear();
});
