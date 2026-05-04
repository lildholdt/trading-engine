const AUTH_TOKEN_KEY = "tradingEngineAuthToken";
const AUTH_USERNAME_KEY = "tradingEngineAuthUsername";
const LOGIN_ROUTE = "/login";
const AUTH_LOGIN_ENDPOINT = "/api/auth/login";

declare global {
  interface Window {
    __tradingEngineAuthFetchInstalled__?: boolean;
    __tradingEngineAuthRedirecting__?: boolean;
  }
}

export const getAuthToken = (): string | null => {
  return localStorage.getItem(AUTH_TOKEN_KEY);
};

export const setAuthToken = (token: string): void => {
  localStorage.setItem(AUTH_TOKEN_KEY, token);
};

export const getAuthUsername = (): string | null => {
  const storedUsername = localStorage.getItem(AUTH_USERNAME_KEY);
  if (storedUsername) {
    return storedUsername;
  }

  const token = getAuthToken();
  if (!token) {
    return null;
  }

  try {
    const [, payloadBase64] = token.split(".");
    if (!payloadBase64) {
      return null;
    }

    const normalizedPayload = payloadBase64.replace(/-/g, "+").replace(/_/g, "/");
    const payloadJson = atob(normalizedPayload);
    const payload = JSON.parse(payloadJson) as Record<string, unknown>;

    const nameClaim = payload["unique_name"] ?? payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"];

    return typeof nameClaim === "string" ? nameClaim : null;
  } catch {
    return null;
  }
};

export const setAuthUsername = (username: string): void => {
  localStorage.setItem(AUTH_USERNAME_KEY, username);
};

export const clearAuthToken = (): void => {
  localStorage.removeItem(AUTH_TOKEN_KEY);
  localStorage.removeItem(AUTH_USERNAME_KEY);
};

export const logoutAndRedirect = (): void => {
  clearAuthToken();

  if (typeof window === "undefined") {
    return;
  }

  if (window.location.pathname === LOGIN_ROUTE || window.__tradingEngineAuthRedirecting__) {
    return;
  }

  window.__tradingEngineAuthRedirecting__ = true;
  window.location.replace(LOGIN_ROUTE);
};

export const getAuthHeaders = (): HeadersInit => {
  const token = getAuthToken();
  if (!token) {
    return {};
  }

  return {
    Authorization: `Bearer ${token}`,
  };
};

const resolveRequestPathname = (input: RequestInfo | URL): string => {
  if (typeof input === "string") {
    return new URL(input, window.location.origin).pathname;
  }

  if (input instanceof URL) {
    return input.pathname;
  }

  return new URL(input.url, window.location.origin).pathname;
};

const shouldHandleUnauthorized = (input: RequestInfo | URL): boolean => {
  if (typeof window === "undefined") {
    return false;
  }

  if (window.location.pathname === LOGIN_ROUTE) {
    return false;
  }

  if (!getAuthToken()) {
    return false;
  }

  return resolveRequestPathname(input) !== AUTH_LOGIN_ENDPOINT;
};

export const installAuthFetchInterceptor = (): void => {
  if (typeof window === "undefined" || window.__tradingEngineAuthFetchInstalled__) {
    return;
  }

  const originalFetch = window.fetch.bind(window);

  window.fetch = async (input: RequestInfo | URL, init?: RequestInit) => {
    const response = await originalFetch(input, init);

    if (response.status === 401 && shouldHandleUnauthorized(input)) {
      logoutAndRedirect();
    }

    return response;
  };

  window.__tradingEngineAuthFetchInstalled__ = true;
};
