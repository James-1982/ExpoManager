import { createRouter, createWebHistory } from 'vue-router';
import WelcomePage from '../components/WelcomePage.vue';
import HomePage from '../components/HomePage.vue';

const routes = [
  { path: '/', name: 'Welcome', component: WelcomePage },
  { path: '/home', name: 'Home', component: HomePage, meta: { requiresAuth: true } },
  { path: '/:pathMatch(.*)*', redirect: '/' },
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

router.beforeEach((to, from) => {
  const token = localStorage.getItem('token');
  const expiration = localStorage.getItem('expiration');

  // Se provo ad accedere a /home senza token → redirect login
  if (to.meta.requiresAuth && (!token || !expiration || new Date() >= new Date(expiration))) {
    return '/';
  }
});

export default router;