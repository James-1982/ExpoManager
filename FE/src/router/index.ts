// src/router/index.ts
import { createRouter, createWebHistory } from 'vue-router';
import WelcomePage from '../pages/WelcomePage.vue';
import HomePage from '../pages/HomePage.vue';
import ForgotPasswordPage from '../pages/ForgotPasswordPage.vue';
import ResetPasswordPage from '../pages/ResetPasswordPage.vue';
import { useAuthStore } from '../stores/authStore';

const routes = [
  { path: '/', component: WelcomePage , meta: { requiresAuth: false }},
  { path: '/home', component: HomePage, meta: { requiresAuth: true } },
  { path: '/forgot-password', component: ForgotPasswordPage },
  { path: '/reset-password', component: ResetPasswordPage },
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

// Guard per autenticazione
// Guard per autenticazione
router.beforeEach((to, from) => {
  const authStore = useAuthStore();
  const token = authStore?.token;
  const expiration = authStore?.expiration ? new Date(authStore.expiration) : null;
  const now = new Date();

  console.log('Navigazione a:', to.path, 'token:', token, 'expiration:', expiration);

  // Se la route richiede login e token non valido
  if (to.meta.requiresAuth && (!token || !expiration || now >= expiration)) {
    authStore.clearAuth?.();
    return '/'; // redirect a WelcomePage
  }

  // Se sei loggato e stai andando alla WelcomePage
  if (to.path === '/' && token && expiration && now < expiration) {
    return '/home';
  }

  return true; // continua normalmente
});

export default router;