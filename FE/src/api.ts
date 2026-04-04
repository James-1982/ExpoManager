// src/api.ts
import axios from 'axios';

// Istanza Axios
const api = axios.create({
  baseURL: 'https://localhost:7017/api/v1',
  headers: { 'Content-Type': 'application/json' },
});

// Ottieni token valido o rinnovalo
const getValidToken = async (): Promise<string | null> => {
  let token = localStorage.getItem('token');
  const expiration = localStorage.getItem('expiration');
  const refreshToken = localStorage.getItem('refresh');

  if (!token || !expiration || new Date() >= new Date(expiration)) {
    if (!refreshToken) {
      localStorage.clear();
      window.location.href = '/';
      return null;
    }

    try {
      const res = await axios.post('https://localhost:7017/api/v1/Authentication/refresh', {
        refresh: refreshToken,
      });

      if (res.data?.token && res.data?.expiration) {
        token = res.data.token;
        localStorage.setItem('token', token!);
        localStorage.setItem('expiration', res.data.expiration);

        if (res.data.refresh) {
          localStorage.setItem('refresh', res.data.refresh);
        }
      } else {
        throw new Error('Refresh token response invalida');
      }
    } catch (err) {
      console.error('Refresh token fallito', err);
      localStorage.clear();
      window.location.href = '/';
      return null;
    }
  }

  return token;
};

// Interceptor request
api.interceptors.request.use(async (config) => {
  // Validazione base
  if (config.data) {
    if ('email' in config.data && typeof config.data.email !== 'string') {
      throw new Error('Email deve essere una stringa');
    }
    if ('password' in config.data && typeof config.data.password !== 'string') {
      throw new Error('Password deve essere una stringa');
    }
  }

  // Non aggiungere token se login
  if (config.url?.toLowerCase().includes('/authentication/login')) return config;

  const token = await getValidToken();
  if (token && config.headers) config.headers.Authorization = `Bearer ${token}`;
  return config;
});

// Interceptor response
api.interceptors.response.use(
  (response) => response,
  async (error) => {
    if (error.response?.status === 401) {
      console.warn('Unauthorized: token scaduto o non valido');
      localStorage.clear();
      window.location.href = '/';
    }
    return Promise.reject(error);
  }
);

export default api;