const AUTH_TOKEN_KEY = "tradingEngineAuthToken";
const AUTH_USERNAME_KEY = "tradingEngineAuthUsername";

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

export const getAuthHeaders = (): HeadersInit => {
  const token = getAuthToken();
  if (!token) {
    return {};
  }

  return {
    Authorization: `Bearer ${token}`,
  };
};
