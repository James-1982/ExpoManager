<template>
  <div class="login-form">
    <div class="form-group">
      <label for="email">Email</label>
      <input id="email" v-model="email" type="email" placeholder="Inserisci la tua email" />
    </div>

    <div class="form-group">
      <label for="password">Password</label>
      <input id="password" v-model="password" type="password" placeholder="Inserisci la tua password" />
    </div>

    <button @click="login" :disabled="loading">
      {{ loading ? 'Accesso...' : 'Login' }}
    </button>

    <p class="error-message" v-if="errorMessage">{{ errorMessage }}</p>
  </div>
</template>

<script lang="ts" setup>
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { useAuthStore } from '../stores/authStore';

const router = useRouter();
const email = ref('');
const password = ref('');
const loading = ref(false);
const errorMessage = ref('');

const authStore = useAuthStore();

const login = async () => {
  loading.value = true;
  errorMessage.value = '';

  try {
    await authStore.login(email.value, password.value);
    router.push('/home'); // reindirizza alla HomePage dopo login
  } catch (err: any) {
    errorMessage.value = err.message || 'Errore durante il login';
  } finally {
    loading.value = false;
  }
};
</script>

<style src="../styles/components/LoginForm.css"></style>