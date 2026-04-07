<!-- src/pages/WelcomePage.vue -->
<template>
  <div class="auth-page">
    <div class="auth-container">
      <h1>Benvenuto in Expo Manager</h1>

      <div class="auth-buttons">
        <button :class="{ active: selected === 'login' }" @click="selected = 'login'">
          Login
        </button>
        <button :class="{ active: selected === 'register' }" @click="selected = 'register'">
          Registrati
        </button>
      </div>

      <div class="auth-form">
        <LoginForm v-if="selected === 'login'" @login-success="goHome" />
        <RegisterForm v-else />
      </div>

      <!-- Link per password -->
      <div v-if="selected === 'login'" class="auth-links">
        <router-link to="/forgot-password">
            Hai dimenticato la password?
        </router-link>
      </div>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { ref, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import LoginForm from '../components/LoginForm.vue';
import RegisterForm from '../components/RegisterForm.vue';

const selected = ref<'login' | 'register'>('login');
const router = useRouter();

// Se l'utente è già loggato, lo mando subito a home
onMounted(() => {
  const token = localStorage.getItem('token');
  const expiration = localStorage.getItem('expiration');
  if (token && expiration && new Date() < new Date(expiration)) {
    router.replace('/home'); // vai subito a home
  }
});

// Funzione chiamata dal LoginForm quando login va a buon fine
function goHome() {
  router.push('/home');
}
</script>

<style src="../styles/components/Auth.css"></style>