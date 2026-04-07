import { defineStore } from 'pinia';
import { ref, computed } from 'vue';
import api from '../api';
import router from '../router';

interface AuthState {
  token: string | null;
  refresh: string | null;
  expiration: string | null;
  userEmail: string | null;
}

export const useAuthStore = defineStore('auth', () => {
  // --- Stato ---
  const token = ref<string | null>(localStorage.getItem('token'));
  const refresh = ref<string | null>(localStorage.getItem('refresh'));
  const expiration = ref<string | null>(localStorage.getItem('expiration'));
  const userEmail = ref<string | null>(localStorage.getItem('userEmail'));

  // --- Computed per header Authorization ---
  const authHeader = computed(() => {
    return token.value ? { Authorization: `Bearer ${token.value}` } : {};
  });

  // --- Salvataggio su localStorage ---
  const saveAuth = () => {
    if (token.value) localStorage.setItem('token', token.value);
    else localStorage.removeItem('token');

    if (refresh.value) localStorage.setItem('refresh', refresh.value);
    else localStorage.removeItem('refresh');

    if (expiration.value) localStorage.setItem('expiration', expiration.value);
    else localStorage.removeItem('expiration');

    if (userEmail.value) localStorage.setItem('userEmail', userEmail.value);
    else localStorage.removeItem('userEmail');
  };

  // --- Clear auth ---
  const clearAuth = () => {
    token.value = null;
    refresh.value = null;
    expiration.value = null;
    userEmail.value = null;
    saveAuth();
  };

  // --- Login ---
  const login = async (email: string, password: string) => {
    try {
      const res = await api.post('/Authentication/login', { email, password });
      const data = res.data;

      token.value = data.token;
      refresh.value = data.refresh;
      expiration.value = data.expiration;
      userEmail.value = email;

      saveAuth();
    } catch (err: any) {
      clearAuth();
      throw new Error(err.response?.data?.message || 'Login fallito');
    }
  };

  // --- Logout ---
  const logout = async () => {
    try {
      if (token.value) {
        await api.post('/Authentication/logout', null, { headers: authHeader.value });
      }
    } catch (err) {
      console.warn('Logout fallito', err);
    } finally {
      clearAuth();
      router.push('/');
    }
  };

  // --- Refresh token ---
  const refreshToken = async () => {
    if (!refresh.value) throw new Error('Nessun refresh token disponibile');

    try {
      const res = await api.post('/Authentication/refresh', { refreshToken: refresh.value });
      const data = res.data;

      token.value = data.token;
      expiration.value = data.expiration;
      saveAuth();
    } catch (err) {
      clearAuth();
      router.push('/');
      throw new Error('Refresh token fallito');
    }
  };

  // --- Controllo se loggato ---
  const isLogged = computed(() => {
    return token.value && expiration.value && new Date() < new Date(expiration.value);
  });

  return {
    token,
    refresh,
    expiration,
    userEmail,
    authHeader,
    login,
    logout,
    refreshToken,
    clearAuth,
    isLogged,
  };
});