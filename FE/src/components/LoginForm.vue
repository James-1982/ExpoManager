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
import api from '../api';

const email = ref('');
const password = ref('');
const loading = ref(false);
const errorMessage = ref('');

const emit = defineEmits<{
  (e: 'login-success'): void
}>();

const login = async () => {
  loading.value = true;
  errorMessage.value = '';

  try {
    const res = await api.post('/Authentication/login', { 
      email: email.value, 
      password: password.value 
    });

    // Salva token ed email in localStorage
    localStorage.setItem('token', res.data.token);
    localStorage.setItem('expiration', res.data.expiration);
    localStorage.setItem('user_email', email.value); // <-- nuovo

    emit('login-success'); // avvisa WelcomePage che il login è andato a buon fine
  } catch (err: any) {
    errorMessage.value = err.response?.data?.message || 'Errore durante il login';
  } finally {
    loading.value = false;
  }
};
</script>

<style src="../styles/components/LoginForm.css"></style>