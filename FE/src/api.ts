import axios from 'axios';
import { useAuthStore } from './stores/authStore';

const api = axios.create({
  baseURL: 'https://localhost:7017/api/v1',
  headers: { 'Content-Type': 'application/json' },
});

// --- Ottieni token valido o rinnovalo ---
const getValidToken = async (): Promise<string | null> => {
  const authStore = useAuthStore();

  // Se non c'è token o è scaduto
  if (!authStore.token || !authStore.expiration || new Date() >= new Date(authStore.expiration)) {
    if (!authStore.refresh) {
      authStore.clearAuth();
      window.location.href = '/';
      return null;
    }

    try {
      await authStore.refreshToken(); // Aggiorna token ed expiration
    } catch (err) {
      console.error('Refresh token fallito', err);
      authStore.clearAuth();
      window.location.href = '/';
      return null;
    }
  }

  return authStore.token;
};

// --- Interceptor request ---
api.interceptors.request.use(async (config) => {
  // Validazioni base
  if (config.data) {
    if ('email' in config.data && typeof config.data.email !== 'string') {
      throw new Error('Email deve essere una stringa');
    }
    if ('password' in config.data && typeof config.data.password !== 'string') {
      throw new Error('Password deve essere una stringa');
    }
  }

  // Non aggiungere token se login o register
  if (
    config.url?.toLowerCase().includes('/authentication/login') ||
    config.url?.toLowerCase().includes('/authentication/register')
  ) {
    return config;
  }

  const token = await getValidToken();
  if (token && config.headers) config.headers.Authorization = `Bearer ${token}`;

  return config;
});

// --- Interceptor response ---
api.interceptors.response.use(
  (response) => response,
  async (error) => {
    const authStore = useAuthStore();

    // 401 -> token scaduto o non valido
    if (error.response?.status === 401) {
      console.warn('Unauthorized: token scaduto o non valido');
      authStore.clearAuth();
      window.location.href = '/';
    }

    return Promise.reject(error);
  }
);

export default api;