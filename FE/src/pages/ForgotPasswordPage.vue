<template>
  <div class="auth-page">
    <div class="auth-container">
      <h1>Recupero Password</h1>

      <form @submit.prevent="sendResetLink">
        <div class="form-group">
          <label>Email</label>
          <input v-model="email" type="email" placeholder="Inserisci la tua email" required />
        </div>

        <button type="submit" class="btn-auth":disabled="loading">
          {{ loading ? 'Invio...' : 'Invia link di reset' }}
        </button>
      </form>

      <p v-if="message">{{ message }}</p>

      <router-link to="/">Torna al login</router-link>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import api from '../api';

const email = ref('');
const loading = ref(false);
const message = ref('');

const sendResetLink = async () => {
  loading.value = true;
  message.value = '';

  // Prepara il payload
  const payload = {
    email: email.value,
    redirectUrl: window.location.origin + '/reset-password', // URL dinamico per reset
  };

  console.log('Payload che sto inviando:', payload);
// Si ferma qui se DevTools aperto

  try {
    await api.post('/Authentication/forgot-password', payload, {
      headers: { 'Content-Type': 'application/json' },
    });
    message.value = 'Controlla la tua email per il link di reset';
  } catch (err: any) {
    message.value = err.response?.data?.message || 'Errore durante il reset della password';
  } finally {
    loading.value = false;
  }
};
</script>

<style src="../styles/components/Auth.css"></style>